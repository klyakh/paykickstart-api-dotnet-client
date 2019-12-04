using KeksCS.PayKickstartApi.Dto;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace KeksCS.PayKickstartApi
{
    /// <summary>
    /// Seems that dates are in UTC.
    /// </summary>
    public class PayKickstartApiClient : IDisposable
    {
        private readonly string _authToken;

        public const string ApiEndpoint = "https://app.paykickstart.com/api/";


        private HttpClient _http;
        protected HttpClient Http
        {
            get
            {
                if(_http == null)
                {
                    _http = new HttpClient
                    {
                        BaseAddress = new Uri(ApiEndpoint)
                    };
                    _http.DefaultRequestHeaders.Accept.Clear();
                    _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }

                return _http;
            }
        }


        public PayKickstartApiClient(string authToken)
        {
            _authToken = authToken;
        }

        //private HttpClient GetHttpClient()
        //{
        //    var result = new HttpClient();
        //    result.BaseAddress = new Uri(ApiEndpoint);
        //    result.DefaultRequestHeaders.Accept.Clear();
        //    result.DefaultRequestHeaders.Accept.Add(
        //        new MediaTypeWithQualityHeaderValue("application/json"));

        //    return result;
        //}


        /// <summary>
        /// Docs: https://support.paykickstart.com/api/#cancel-subscription
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="fireEvent"></param>
        /// <param name="cancelAt"></param>
        /// <returns></returns>
        public async Task<(bool, JObject)> CancelSubscriptionJsonAsync(string invoiceId, bool fireEvent = true, DateTimeOffset? cancelAt = null)
        {
            var fields = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("invoice_id", invoiceId),
                new KeyValuePair<string, string>("auth_token", _authToken),
                new KeyValuePair<string, string>("fire_event", fireEvent ? "1" : "0")
            };

            if(cancelAt != null)
            {
                var subscriptionNextDateUnix = cancelAt.Value.ToUnixTimeSeconds();
                fields.Add(new KeyValuePair<string, string>("cancel_at", subscriptionNextDateUnix.ToString()));
            }

            var content = new FormUrlEncodedContent(fields);
            var response = await Http.PostAsync("subscriptions/cancel", content);
            var resultContent = await response.Content.ReadAsStringAsync();

            var jsonResponse = JObject.Parse(resultContent);
            return ((string)jsonResponse["success"] == "1", jsonResponse);
        }

        /// <summary>
        /// Get's date when subscription gets expired. Then calls cancel API with cancelAt = expire date + "00:00:01"
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="fireEvent"></param>
        /// <returns></returns>
        public async Task<(bool, DateTimeOffset, JObject)> CancelSubscriptionAtEndJsonAsync(string invoiceId, bool fireEvent = true)
        {
            var purchaseJson = await GetPurchaseAsJson(invoiceId);
            var subscriptionNextDateStr = (string)purchaseJson["subscriptions"][0]["next_date"];
            if (!DateTimeOffset.TryParse(subscriptionNextDateStr, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var subscriptionNextDate))
            {
                throw new ApplicationException("Can't parse subscription's next_date: " + subscriptionNextDateStr);
            }

            var subscriptionCancelDate = new DateTimeOffset(subscriptionNextDate.Year, subscriptionNextDate.Month, subscriptionNextDate.Day, 0, 0, 1, TimeSpan.Zero);

            (bool success, JObject response) = await CancelSubscriptionJsonAsync(invoiceId, fireEvent, subscriptionCancelDate);

            return (success, subscriptionCancelDate, response);
        }


        public async Task<(bool, JObject)> ReactivateSubscriptionJson(string invoiceId, DateTimeOffset date)
        {
            var fields = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("invoice_id", invoiceId),
                new KeyValuePair<string, string>("auth_token", _authToken),
                new KeyValuePair<string, string>("date", date.ToUnixTimeSeconds().ToString())
            };

            var content = new FormUrlEncodedContent(fields);
            var response = await Http.PostAsync("subscriptions/re-activate", content);
            var resultContent = await response.Content.ReadAsStringAsync();

            var jsonResponse = JObject.Parse(resultContent);

            return ((bool)jsonResponse["status"], jsonResponse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchaseId">invoice_id from IPN</param>
        /// <returns></returns>
        public async Task<JObject> GetPurchaseAsJson(string purchaseId)
        {
            string url = $"purchase/get?auth_token={_authToken}&id={purchaseId}";
            var response = await Http.GetAsync(url);
            var resultContent = await response.Content.ReadAsStringAsync();

            var jsonResponse = JObject.Parse(resultContent);
            return jsonResponse;
        }


        //public async Task<JObject> NewPurchaseUsingRefPurchase(string refPurchaseId,   string campaignId, string productId, )
        //{

        //}

        /// <summary>
        /// About passing arrays in POST in PHP - https://www.php.net/manual/en/function.http-build-query.php
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<(bool, JObject)> ProratedSubscriptionUpgrade(ProratedSubscriptionUpgradeInputDto input)
        {
            var fields = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("auth_token", _authToken),
                new KeyValuePair<string, string>("first_name", input.FirstName),
                new KeyValuePair<string, string>("last_name", input.LastName),
                new KeyValuePair<string, string>("email", input.Email),
                new KeyValuePair<string, string>("product", input.CampaignId),
                new KeyValuePair<string, string>("plan", input.ProductId),
                new KeyValuePair<string, string>("ref_purchase", input.RefPurchaseId),
                new KeyValuePair<string, string>("has_trial", "1"),
                new KeyValuePair<string, string>("trial_amount", string.Format("{0:N2}", input.UpgradePrice)),
            };

            var trialPeriod = input.NextBillingDate - DateTimeOffset.UtcNow;
            fields.Add(new KeyValuePair<string, string>("trial_days", trialPeriod.Days.ToString()));

            if(input.CustomFields != null)
            {
                foreach(var cf in input.CustomFields)
                {
                    fields.Add(new KeyValuePair<string, string>($"custom_field[{cf.Key}]", cf.Value));
                }
            }

            var content = new FormUrlEncodedContent(fields);
            var response = await Http.PostAsync("purchase", content);
            var resultContent = await response.Content.ReadAsStringAsync();

            var jsonResponse = JObject.Parse(resultContent);
            var success = jsonResponse["errors"] == null;
            return (success, jsonResponse);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if(_http != null)
                    {
                        _http.Dispose();
                        _http = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PayKickstartApiClient()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}

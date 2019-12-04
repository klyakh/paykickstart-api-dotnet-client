using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace KeksCS.PayKickstartApi.Api
{
    /// <summary>
    /// PK object represents business model not correctly. In case a product is a subscription, Purchase represents subscription, and inner subscriptions represent part of the subsription data.
    /// Need to think if I want to fix this in my object model. Kind of incapsulate and hide this discrepancy.
    /// </summary>
    public class Purchase
    {
        public double Amount { get; set; }
        public string BuyerFirstName { get; set; }
        public string BuyerLastName { get; set; }
        public string BuyerEmail { get; set; }

        public Purchase(JToken source)
        {
            Amount = double.Parse((string)source["amount"], CultureInfo.InvariantCulture);
            BuyerFirstName = (string)source["buyer_first_name"];
            BuyerLastName = (string)source["buyer_last_name"];
            BuyerEmail = (string)source["buyer_email"];

            Subscriptions = new List<Subscription>()
            {
                new Subscription(source["subscriptions"][0])
            };

            Transactions = new List<Transaction>();
            foreach(var t in source["transactions"])
            {
                var transaction = new Transaction(t);
                Transactions.Add(transaction);
            }
        }

        public IList<Subscription> Subscriptions { get; set; }
        public IList<Transaction> Transactions { get; set; }

        private int GetDaysTillPaidPeriodEnd()
        {
            var difference = Subscriptions[0].NextDate - DateTimeOffset.UtcNow;
            return difference.Days;
        }

        public double GetSubscriptionRemainingAmount()
        {
            var lastTransaction = Transactions[Transactions.Count - 1];
            var paymentPeriod = Subscriptions[0].NextDate - lastTransaction.CreatedAt;
            double amountPerDay = lastTransaction.Amount / paymentPeriod.Days;

            int daysTillPaidPeriodEnd = GetDaysTillPaidPeriodEnd();
            return amountPerDay * daysTillPaidPeriodEnd;
        }
    }
}

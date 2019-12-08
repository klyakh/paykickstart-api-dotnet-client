using System;
using System.Collections.Generic;
using System.Text;

namespace KeksCS.PayKickstartApi.Dto
{
    public class NewPurchaseInputDto
    {
        public string BuyerFirstName { get; set; }
        public string BuyerLastName { get; set; }
        public string BuyerEmail { get; set; }
        public string CampaignId { get; set; }
        public string ProductId { get; set; }
        public string RefPurchaseId { get; set; }

        public bool HasTrial { get; set; }
        public double? TrialAmount { get; set; }
        public int? TrialDays { get; set; }

        public IDictionary<string, string> CustomFields { get; set; }


        public bool IsValid()
        {
            //TODO: downside of such validation - no good error message. Need another approach.

            if(string.IsNullOrWhiteSpace(BuyerFirstName) || string.IsNullOrWhiteSpace(BuyerFirstName) || string.IsNullOrWhiteSpace(BuyerEmail))
            {
                return false;
            }

            if(string.IsNullOrWhiteSpace(CampaignId) || string.IsNullOrWhiteSpace(ProductId))
            {
                return false;
            }

            return true;
        }
    }
}

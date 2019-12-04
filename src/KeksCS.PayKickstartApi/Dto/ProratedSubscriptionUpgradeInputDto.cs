using System;
using System.Collections.Generic;
using System.Text;

namespace KeksCS.PayKickstartApi.Dto
{
    public class ProratedSubscriptionUpgradeInputDto
    {
        public string CampaignId { get; set; }
        public string ProductId { get; set; }
        public double UpgradePrice { get; set; }
        public DateTimeOffset NextBillingDate { get; set; }
        public string RefPurchaseId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public IDictionary<string, string> CustomFields { get; set; }
    }
}

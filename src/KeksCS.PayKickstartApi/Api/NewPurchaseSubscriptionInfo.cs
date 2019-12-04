using System;
using System.Collections.Generic;
using System.Text;

namespace KeksCS.PayKickstartApi.Api
{
    public class NewPurchaseSubscriptionInfo : NewPurchaseProductInfo
    {
        public double? ProratedUpgradePrice { get; set; }
        public DateTimeOffset? NextBillingDate { get; set; }
    }
}

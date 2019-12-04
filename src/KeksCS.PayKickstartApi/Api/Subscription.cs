using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace KeksCS.PayKickstartApi.Api
{
    public class Subscription
    {
        public DateTimeOffset NextDate { get; set; }

        public Subscription(JToken source)
        {
            var nextDateStr = (string)source["next_date"];
            if (!DateTimeOffset.TryParse(nextDateStr, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var nextDate))
            {
                throw new ApplicationException("Can't parse subscription's next_date: " + nextDateStr);
            }
            NextDate = nextDate;
        }
    }
}

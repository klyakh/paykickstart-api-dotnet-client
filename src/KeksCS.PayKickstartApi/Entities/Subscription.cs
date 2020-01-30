using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace KeksCS.PayKickstartApi.Entities
{
    public class Subscription : EntityBase
    {
        public DateTimeOffset NextDate { get; set; }

        public DateTime? TrialEnds
        {
            get
            {
                var str = (string)Source["trial_ends"];
                if(string.IsNullOrWhiteSpace(str))
                {
                    return null;
                }
                else
                {
                    return DateTime.Parse(str);
                }
            }
        }

        public Subscription(JToken source) : base(source)
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

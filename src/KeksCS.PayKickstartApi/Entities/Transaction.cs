using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace KeksCS.PayKickstartApi.Entities
{
    public class Transaction
    {
        public DateTimeOffset CreatedAt { get; set; }

        public double Amount { get; set; }

        public Transaction(JToken source)
        {
            var createdAtStr = (string)source["created_at"];
            if (!DateTimeOffset.TryParse(createdAtStr, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var createdAt))
            {
                throw new ApplicationException("Can't parse subscription's next_date: " + createdAtStr);
            }
            CreatedAt = createdAt;

            Amount = double.Parse((string)source["amount"], CultureInfo.InvariantCulture);
        }
    }
}

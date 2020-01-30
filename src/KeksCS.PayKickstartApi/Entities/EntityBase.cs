using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeksCS.PayKickstartApi.Entities
{
    public class EntityBase
    {
        public JToken Source { get; }

        public EntityBase(JToken source)
        {
            Source = source;
        }
    }
}

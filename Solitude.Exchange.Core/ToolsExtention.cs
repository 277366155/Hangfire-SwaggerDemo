using System;
using System.Collections.Generic;
using System.Text;

namespace Solitude.Exchange.Core
{
  public static   class ToolsExtention
    {
        public static string ToJson(this Object obj)
        {
            if (obj == null)
            {
                return "";
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
    }
}

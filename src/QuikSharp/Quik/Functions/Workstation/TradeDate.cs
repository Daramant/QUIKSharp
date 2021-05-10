using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Quik.Functions.Workplace
{
    /// <summary>
    /// Дата текущей торговой сессии.
    /// </summary>
    public class TradeDate
    {
        /// <summary>
        /// Торговая дата в виде строки «ДД.ММ.ГГГГ».
        /// </summary>
        [JsonProperty("date")]
        public string Date { get; set; }

        /// <summary>
        /// Год.
        /// </summary>
        [JsonProperty("year")]
        public int Year { get; set; }

        /// <summary>
        /// Месяц.
        /// </summary>
        [JsonProperty("month")]
        public int Month { get; set; }

        /// <summary>
        /// День.
        /// </summary>
        [JsonProperty("day")]
        public int Day { get; set; }
    }
}

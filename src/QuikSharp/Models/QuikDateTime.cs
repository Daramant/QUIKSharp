// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using Newtonsoft.Json;
using System;

namespace QuikSharp.DataStructures
{
    /// <summary>
    /// Формат даты и времени, используемый таблицах.
    /// Для корректного отображения даты и времени все параметры должны быть заданы.
    /// </summary>
    public class QuikDateTime
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Микросекунды игнорируются в текущей версии.
        /// </summary>
        [JsonProperty("mcs")]
        public int Microsecond { get; set; }

        /// <summary>
        /// Миллисекунды.
        /// </summary>
        [JsonProperty("ms")]
        public int Millisecond { get; set; }

        /// <summary>
        /// Секунды.
        /// </summary>
        [JsonProperty("sec")]
        public int Second { get; set; }

        /// <summary>
        /// Минуты.
        /// </summary>
        [JsonProperty("min")]
        public int Minute { get; set; }

        /// <summary>
        /// Часы.
        /// </summary>
        [JsonProperty("hour")]
        public int Hour { get; set; }

        /// <summary>
        /// День.
        /// </summary>
        [JsonProperty("day")]
        public int Day { get; set; }

        /// <summary>
        /// Номер дня недели (Monday is 1).
        /// </summary>
        [JsonProperty("week_day")]
        public int WeekDay { get; set; }

        /// <summary>
        /// Месяц.
        /// </summary>
        [JsonProperty("month")]
        public int Month { get; set; }

        /// <summary>
        /// Год.
        /// </summary>
        [JsonProperty("year")]
        public int Year { get; set; }

        // ReSharper restore InconsistentNaming

        /// <summary>
        ///
        /// </summary>
        /// <param name="qdt"></param>
        /// <returns></returns>
        public static explicit operator DateTime(QuikDateTime qdt)
        {
            return new DateTime(qdt.Year, qdt.Month, qdt.Day, qdt.Hour, qdt.Minute, qdt.Second, qdt.Millisecond);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static explicit operator QuikDateTime(DateTime dt)
        {
            return new QuikDateTime
            {
                Year = dt.Year,
                Month = dt.Month,
                Day = dt.Day,
                Hour = dt.Hour,
                Minute = dt.Minute,
                Second = dt.Second,
                Millisecond = dt.Millisecond,
                Microsecond = 0,
                WeekDay = (int)dt.DayOfWeek
            };
        }
    }
}
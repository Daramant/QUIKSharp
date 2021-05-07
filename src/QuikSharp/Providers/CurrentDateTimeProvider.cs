using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Providers
{
    public class CurrentDateTimeProvider : IDateTimeProvider
    {
        private static readonly long Epoch = (new DateTime(1970, 1, 1, 3, 0, 0, 0)).Ticks / 10000L;

        /// <inheritdoc/>
        public long NowInMilliseconds => DateTime.Now.Ticks / 10000L - Epoch;

        /// <inheritdoc/>
        public DateTime Now => DateTime.Now;

        /// <inheritdoc/>
        public DateTime UtcNow => DateTime.UtcNow;
    }
}

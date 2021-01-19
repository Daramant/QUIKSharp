using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Providers
{
    public class CurrentDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}

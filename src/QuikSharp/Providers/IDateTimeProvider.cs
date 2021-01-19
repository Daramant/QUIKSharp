using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Providers
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}

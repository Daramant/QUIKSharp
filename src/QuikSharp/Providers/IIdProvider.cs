using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Providers
{
    public interface IIdProvider
    {
        int GetUniqueCommandId();

        int GetUniqueTransactionId();
    }
}

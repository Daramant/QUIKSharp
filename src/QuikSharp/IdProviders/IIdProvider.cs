using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.IdProviders
{
    public interface IIdProvider
    {
        int GetUniqueCommandId();

        int GetUniqueTransactionId();
    }
}

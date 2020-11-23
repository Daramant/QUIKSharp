using QuikSharp.DataStructures.Transaction;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Transactions
{
    public delegate void TransactionEventHandler<in TArgs>(Transaction sender, TArgs args)
        where TArgs : EventArgs;
}

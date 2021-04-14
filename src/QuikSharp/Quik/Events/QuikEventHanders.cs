using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using QuikSharp.Quik;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.QuikEvents
{
    public delegate void QuikEventHandler<in TArgs>(IQuik sender, TArgs args)
        where TArgs : EventArgs;
}

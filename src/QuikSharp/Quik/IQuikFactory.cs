using QuikSharp.Quik.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Quik
{
    public interface IQuikFactory
    {
        IQuik Create(QuikClientOptions options);
    }
}

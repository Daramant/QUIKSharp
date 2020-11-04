using QuikSharp.QuikService;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Quik
{
    public interface IQuikFactory
    {
        IQuik Create(QuikServiceOptions options);
    }
}

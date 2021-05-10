using QuikSharp.Messages;
using QuikSharp.Quik.Client;
using QuikSharp.TypeConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Quik.Functions.TableRows
{
    public class TableRowFunctions : FunctionsBase, ITableRowFunctions
    {
        public TableRowFunctions(
            IMessageFactory messageFactory,
            IQuikClient quikClient,
            ITypeConverter typeConverter)
            : base(messageFactory, quikClient, typeConverter)
        { }
    }
}

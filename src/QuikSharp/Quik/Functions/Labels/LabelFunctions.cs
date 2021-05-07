using QuikSharp.Extensions;
using QuikSharp.Messages;
using QuikSharp.Quik.Client;
using QuikSharp.TypeConverters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.Labels
{
    public class LabelFunctions : FunctionsBase, ILabelFunctions
    {
        public LabelFunctions(
            IMessageFactory messageFactory,
            IQuikClient quikClient,
            ITypeConverter typeConverter)
            : base(messageFactory, quikClient, typeConverter)
        { }

        /// <inheritdoc/>
        public Task<decimal> AddLabelAsync(decimal price, string curDate, string curTime, string hint, string path, string tag, string alignment, decimal backgnd)
        {
            return ExecuteCommandAsync<decimal>("addLabel", 
                new[] { TypeConverter.ToString(price), curDate, curTime, hint, path, tag, alignment, TypeConverter.ToString(backgnd) });
        }

        /// <inheritdoc/>
        public Task<bool> DelLabelAsync(string tag, decimal id)
        {
            return ExecuteCommandAsync<bool>("delLabel", new[] { tag, TypeConverter.ToString(id) });
        }

        /// <inheritdoc/>
        public Task<bool> DelAllLabelsAsync(string tag)
        {
            return ExecuteCommandAsync<string, bool>("delAllLabels", tag);
        }
    }
}

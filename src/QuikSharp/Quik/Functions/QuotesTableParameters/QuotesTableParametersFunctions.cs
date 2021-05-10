using QuikSharp.DataStructures;
using QuikSharp.Messages;
using QuikSharp.Quik.Client;
using QuikSharp.TypeConverters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.QuotesTableParameters
{
    public class QuotesTableParametersFunctions : FunctionsBase, IQuotesTableParametersFunctions
    {
        public QuotesTableParametersFunctions(
            IMessageFactory messageFactory,
            IQuikClient quikClient,
            ITypeConverter typeConverter)
            : base(messageFactory, quikClient, typeConverter)
        { }

        /// <inheritdoc/>
        public Task<bool> ParamRequestAsync(string classCode, string secCode, ParamName paramName)
        {
            return ExecuteCommandAsync<bool>("paramRequest", new[] { classCode, secCode, TypeConverter.ToString(paramName) });
        }

        /// <inheritdoc/>
        public Task<bool> CancelParamRequestAsync(string classCode, string secCode, ParamName paramName)
        {
            return ExecuteCommandAsync<bool>("cancelParamRequest", new[] { classCode, secCode, TypeConverter.ToString(paramName) });
        }
    }
}

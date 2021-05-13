using QuikSharp.DataStructures;
using QuikSharp.Messages;
using QuikSharp.Quik.Client;
using QuikSharp.TypeConverters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.Custom
{
    /// <summary>
    /// Дополнительные пользовательские функции.
    /// </summary>
    public class CustomFunctions : FunctionsBase, ICustomFunctions
    {
        /// <inheritdoc/>
        public new IMessageFactory MessageFactory => base.MessageFactory;

        /// <inheritdoc/>
        public new ITypeConverter TypeConverter => base.TypeConverter;

        public CustomFunctions(
            IMessageFactory messageFactory,
            IQuikClient quikClient,
            ITypeConverter typeConverter)
            : base(messageFactory, quikClient, typeConverter)
        { }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<OptionBoard>> GetOptionBoardAsync(string classCode, string secCode)
        {
            return ExecuteCommandAsync<IReadOnlyCollection<OptionBoard>>("getOptionBoard", new[] { classCode, secCode });
        }
    }
}

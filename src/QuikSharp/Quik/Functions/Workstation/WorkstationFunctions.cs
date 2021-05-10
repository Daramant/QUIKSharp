using QuikSharp.DataStructures;
using QuikSharp.Messages;
using QuikSharp.Quik.Client;
using QuikSharp.TypeConverters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.Workplace
{
    public class WorkstationFunctions : FunctionsBase, IWorkstationFunctions
    {
        public WorkstationFunctions(
            IMessageFactory messageFactory,
            IQuikClient quikClient,
            ITypeConverter typeConverter)
            : base(messageFactory, quikClient, typeConverter)
        { }

        /// <inheritdoc/>
        public Task<MoneyLimit> GetMoneyAsync(string clientCode, string firmId, string tag, string currCode)
        {
            return ExecuteCommandAsync<MoneyLimit>("getMoney", new[] { clientCode, firmId, tag, currCode });
        }

        /// <inheritdoc/>
        public Task<MoneyLimitEx> GetMoneyExAsync(string firmId, string clientCode, string tag, string currCode, int limitKind)
        {
            return ExecuteCommandAsync<MoneyLimitEx>("getMoneyEx",
                new[] { firmId, clientCode, tag, currCode, TypeConverter.ToStringLookup(limitKind) });
        }

        /// <inheritdoc/>
        public Task<DepoLimit> GetDepoAsync(string clientCode, string firmId, string secCode, string account)
        {
            return ExecuteCommandAsync<DepoLimit>("getDepo", new[] { clientCode, firmId, secCode, account });
        }

        /// <inheritdoc/>
        public Task<DepoLimitEx> GetDepoExAsync(string firmId, string clientCode, string secCode, string accID, int limitKind)
        {
            return ExecuteCommandAsync<DepoLimitEx>("getDepoEx",
                new[] { firmId, clientCode, secCode, accID, TypeConverter.ToStringLookup(limitKind) });
        }

        /// <inheritdoc/>
        public Task<FuturesLimits> GetFuturesLimitAsync(string firmId, string accId, int limitType, string currCode)
        {
            return ExecuteCommandAsync<FuturesLimits>("getFuturesLimit",
                new[] { firmId, accId, TypeConverter.ToStringLookup(limitType), currCode });
        }

        /// <inheritdoc/>
        public Task<FuturesClientHolding> GetFuturesHoldingAsync(string firmId, string accId, string secCode, int posType)
        {
            return ExecuteCommandAsync<FuturesClientHolding>("getFuturesHolding",
                new[] { firmId, accId, secCode, TypeConverter.ToStringLookup(posType) });
        }

        /// <inheritdoc/>
        public Task<SecurityInfo> GetSecurityInfoAsync(string classCode, string secCode)
        {
            return ExecuteCommandAsync<SecurityInfo>("getSecurityInfo", new[] { classCode, secCode });
        }

        /// <inheritdoc/>
        public Task<TradeDate> GetTradeDateAsync()
        {
            return ExecuteCommandAsync<string, TradeDate>("getTradeDate", string.Empty);
        }

        /// <inheritdoc/>
        public Task<OrderBook> GetQuoteLevel2Async(string classCode, string secCode)
        {
            return ExecuteCommandAsync<OrderBook>("getQuoteLevel2", new[] { classCode, secCode });
        }

        /// <inheritdoc/>
        public Task<ParamTable> GetParamExAsync(string classCode, string secCode, ParamName paramName, TimeSpan? timeout = null)
        {
            return ExecuteCommandAsync<ParamTable>("getParamEx", 
                new[] { classCode, secCode, TypeConverter.ToString(paramName) }, timeout: timeout);
        }

        /// <inheritdoc/>
        public Task<ParamTable> GetParamEx2Async(string classCode, string secCode, ParamName paramName, TimeSpan? timeout = null)
        {
            return ExecuteCommandAsync<ParamTable>("getParamEx2", 
                new[] { classCode, secCode, TypeConverter.ToString(paramName) }, timeout: timeout);
        }

        /// <inheritdoc/>
        public Task<PortfolioInfo> GetPortfolioInfoAsync(string firmId, string clientCode)
        {
            return ExecuteCommandAsync<PortfolioInfo>("getPortfolioInfo", new[] { firmId, clientCode });
        }

        /// <inheritdoc/>
        public Task<PortfolioInfoEx> GetPortfolioInfoExAsync(string firmId, string clientCode, int limitKind)
        {
            return ExecuteCommandAsync<PortfolioInfoEx>("getPortfolioInfoEx",
                new[] { firmId, clientCode, TypeConverter.ToStringLookup(limitKind) });
        }

        /// <inheritdoc/>
        public Task<string> GetTrdAccByClientCodeAsync(string firmId, string clientCode)
        {
            return ExecuteCommandAsync<string>("getTrdAccByClientCode", new[] { firmId, clientCode });
        }

        /// <inheritdoc/>
        public Task<string> GetClientCodeByTrdAccAsync(string firmId, string trdAccId)
        {
            return ExecuteCommandAsync<string>("getClientCodeByTrdAcc", new[] { firmId, trdAccId });
        }

        /// <inheritdoc/>
        public Task<bool> IsUcpClientAsync(string firmId, string client)
        {
            return ExecuteCommandAsync<bool>("isUcpClient", new[] { firmId, client });
        }
    }
}

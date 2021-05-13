using QuikSharp.Quik.Functions.Candles;
using QuikSharp.Quik.Functions.Classes;
using QuikSharp.Quik.Functions.Custom;
using QuikSharp.Quik.Functions.Debug;
using QuikSharp.Quik.Functions.Labels;
using QuikSharp.Quik.Functions.OrderBooks;
using QuikSharp.Quik.Functions.QuotesTableParameters;
using QuikSharp.Quik.Functions.Services;
using QuikSharp.Quik.Functions.TableRows;
using QuikSharp.Quik.Functions.Workplace;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.Quik.Functions
{
    public class QuikFunctions : IQuikFunctions
    {
        /// <inheritdoc/>
        public IServiceFunctions Service { get; }

        /// <inheritdoc/>
        public ITableRowFunctions TableRows { get; set; }

        /// <inheritdoc/>
        public IClassFunctions Class { get; }

        /// <inheritdoc/>
        public IWorkstationFunctions Workstation { get; set; }

        /// <inheritdoc/>
        public ICandleFunctions Candles { get; }

        /// <inheritdoc/>
        public ILabelFunctions Labels { get; }

        /// <inheritdoc/>
        public IOrderBookFunctions OrderBook { get; }

        /// <inheritdoc/>
        public IQuotesTableParametersFunctions QuotesTableParameters { get; set; }

        /// <inheritdoc/>
        public IDebugFunctions Debug { get; }

        /// <inheritdoc/>
        public ICustomFunctions Custom { get; }

        public QuikFunctions(
            IServiceFunctions serviceFunctions,
            ITableRowFunctions tableRowFunctions,
            IClassFunctions classFunctions,
            IWorkstationFunctions workstationFunctions,
            ICandleFunctions candleFunctions,
            ILabelFunctions labelFunctions,
            IOrderBookFunctions orderBookFunctions,
            IQuotesTableParametersFunctions quotesTableParametersFunctions,
            IDebugFunctions debugFunctions,
            ICustomFunctions customFunctions)
        {
            Service = serviceFunctions;
            TableRows = tableRowFunctions;
            Class = classFunctions;
            Workstation = workstationFunctions;
            Candles = candleFunctions;
            Labels = labelFunctions;
            OrderBook = orderBookFunctions;
            QuotesTableParameters = quotesTableParametersFunctions;
            Debug = debugFunctions;
            Custom = customFunctions;
        }
    }
}

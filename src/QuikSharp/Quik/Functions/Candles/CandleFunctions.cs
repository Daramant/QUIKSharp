// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using QuikSharp.DataStructures;
using QuikSharp.Messages;
using QuikSharp.Quik.Client;
using QuikSharp.TypeConverters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuikSharp.Quik.Functions.Candles
{
    /// <summary>
    /// Функции для получения свечей
    /// </summary>
    public class CandleFunctions : FunctionsBase, ICandleFunctions
    {
        public CandleFunctions(
            IMessageFactory messageFactory,
            IQuikClient quikClient,
            ITypeConverter typeConverter)
            : base(messageFactory, quikClient, typeConverter)
        { }

        /// <inheritdoc/>
        public Task<int> GetNumCandlesAsync(string graphicTag)
        {
            return ExecuteCommandAsync<string, int>("get_num_candles", graphicTag);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<Candle>> GetAllCandlesAsync(string graphicTag)
        {
            return GetCandlesAsync(graphicTag, 0, 0, 0);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<Candle>> GetCandlesAsync(string graphicTag, int line, int first, int count)
        {
            return ExecuteCommandAsync<IReadOnlyCollection<Candle>>("get_candles", 
                new[] { graphicTag, TypeConverter.ToStringLookup(line), TypeConverter.ToStringLookup(first), TypeConverter.ToStringLookup(count) });
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<Candle>> GetAllCandlesAsync(string classCode, string securityCode, CandleInterval interval)
        {
            //Параметр count == 0 говорт о том, что возвращаются все доступные свечи.
            return GetLastCandlesAsync(classCode, securityCode, interval, 0);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyCollection<Candle>> GetLastCandlesAsync(string classCode, string securityCode, CandleInterval interval, int count)
        {
            return ExecuteCommandAsync<IReadOnlyCollection<Candle>>("get_candles_from_data_source", 
                new[] { classCode, securityCode, TypeConverter.ToString((int)interval), TypeConverter.ToStringLookup(count) });
        }

        /// <inheritdoc/>
        public Task SubscribeAsync(string classCode, string securityCode, CandleInterval interval)
        {
            return ExecuteCommandAsync<string>("subscribe_to_candles", 
                new[] { classCode, securityCode, TypeConverter.ToString((int)interval) });
        }

        /// <inheritdoc/>
        public Task UnsubscribeAsync(string classCode, string securityCode, CandleInterval interval)
        {
            return ExecuteCommandAsync<string>("unsubscribe_from_candles", 
                new[] { classCode, securityCode, TypeConverter.ToString((int)interval) });
        }

        /// <inheritdoc/>
        public Task<bool> IsSubscribedAsync(string classCode, string securityCode, CandleInterval interval)
        {
            return ExecuteCommandAsync<bool>("is_subscribed", 
                new[] { classCode, securityCode, TypeConverter.ToString((int)interval) });
        }
    }
}
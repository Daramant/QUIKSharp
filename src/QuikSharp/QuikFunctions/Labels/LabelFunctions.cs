﻿using QuikSharp.Extensions;
using QuikSharp.Messages;
using QuikSharp.QuikClient;
using QuikSharp.TypeConverters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.QuikFunctions.Labels
{
    public class LabelFunctions : ILabelFunctions
    {
        private readonly IQuikClient _quikClient;
        private readonly ITypeConverter _typeConverter;

        public LabelFunctions(
            IQuikClient quikClient,
            ITypeConverter typeConverter)
        {
            _quikClient = quikClient;
            _typeConverter = typeConverter;
        }

        public async Task<decimal> AddLabelAsync(decimal price, string curDate, string curTime, string hint, string path, string tag, string alignment, decimal backgnd)
        {
            var response = await _quikClient.SendAsync<Result<decimal>>(
                    (new Command<string[]>(new[] { _typeConverter.ToString(price), curDate, curTime, hint, path, tag, alignment, _typeConverter.ToString(backgnd) }, "addLabel")))
                .ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> DelLabelAsync(string tag, decimal id)
        {
            await _quikClient.SendAsync<Result<string>>(
                (new Command<string[]>(new[] { tag, _typeConverter.ToString(id) }, "delLabel"))).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> DelAllLabelsAsync(string tag)
        {
            await _quikClient.SendAsync<Result<string>>(
                (new Command<string>(tag, "delAllLabels"))).ConfigureAwait(false);
            return true; // TODO: Возвращать результат из quik.
        }
    }
}

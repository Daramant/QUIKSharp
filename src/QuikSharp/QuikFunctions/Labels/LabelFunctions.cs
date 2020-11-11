using QuikSharp.Messages;
using QuikSharp.QuikClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuikSharp.QuikFunctions.Labels
{
    public class LabelFunctions : ILabelFunctions
    {
        private readonly IQuikClient _quikClient;

        public LabelFunctions(IQuikClient quikClient)
        {
            _quikClient = quikClient;
        }

        public async Task<double> AddLabelAsync(double price, string curDate, string curTime, string hint, string path, string tag, string alignment, double backgnd)
        {
            var response = await _quikClient.SendAsync<Result<double>>(
                    (new Command<string>(price + "|" + curDate + "|" + curTime + "|" + hint + "|" + path + "|" + tag + "|" + alignment + "|" + backgnd, "addLabel")))
                .ConfigureAwait(false);
            return response.Data;
        }

        public async Task<bool> DelLabelAsync(string tag, double id)
        {
            await _quikClient.SendAsync<Result<string>>(
                (new Command<string>(tag + "|" + id, "delLabel"))).ConfigureAwait(false);
            return true;
        }

        public async Task<bool> DelAllLabelsAsync(string tag)
        {
            await _quikClient.SendAsync<Result<string>>(
                (new Command<string>(tag, "delAllLabels"))).ConfigureAwait(false);
            return true;
        }
    }
}

using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using System;

namespace QuikSharp.QuikEvents
{
    public interface IQuikEventsInvoker
    {
        void OnConnectedToQuik(int port);

        void OnDisconnectedFromQuik();

        void OnAccountBalance(AccountBalance accBal);

        void OnAccountPosition(AccountPosition accPos);

        void OnAllTrade(AllTrade allTrade);

        void OnCleanUp();

        void OnClose();

        void OnConnected();

        void OnDepoLimit(DepoLimitEx dLimit);

        void OnDepoLimitDelete(DepoLimitDelete dLimitDel);

        void OnDisconnected();

        void OnFirm(Firm frm);

        void OnFuturesClientHolding(FuturesClientHolding futPos);

        void OnFuturesLimitChange(FuturesLimits futLimit);

        void OnFuturesLimitDelete(FuturesLimitDelete limDel);

        void OnMoneyLimit(MoneyLimitEx mLimit);

        void OnMoneyLimitDelete(MoneyLimitDelete mLimitDel);

        void OnOrder(Order order);

        void OnParam(Param par);

        void OnQuote(OrderBook orderBook);

        void OnStop(int signal);

        void OnStopOrder(StopOrder stopOrder);

        void OnTrade(Trade trade);

        void OnTransReply(TransactionReply reply);

        void OnNewCandle(Candle candle);
    }
}
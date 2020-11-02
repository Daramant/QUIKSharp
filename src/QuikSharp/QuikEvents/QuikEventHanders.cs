using QuikSharp.DataStructures;
using QuikSharp.DataStructures.Transaction;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuikSharp.QuikEvents
{
    /// <summary>
    /// A handler for events without arguments
    /// </summary>
    public delegate void VoidHandler();

    /// <summary>
    /// Обработчик события OnInit
    /// </summary>
    /// <param name="port">Порт обмена данными</param>
    public delegate void InitHandler(int port);

    /// <summary>
    ///
    /// </summary>
    /// <param name="orderbook"></param>
    public delegate void QuoteHandler(OrderBook orderbook);

    /// <summary>
    /// Обработчик события OnStop
    /// </summary>
    public delegate void StopHandler(int signal);

    /// <summary>
    /// Обработчик события OnAllTrade
    /// </summary>
    /// <param name="allTrade"></param>
    public delegate void AllTradeHandler(AllTrade allTrade);

    /// <summary>
    ///
    /// </summary>
    /// <param name="transReply"></param>
    public delegate void TransReplyHandler(TransactionReply transReply);

    /// <summary>
    /// Обработчик события OnOrder
    /// </summary>
    /// <param name="order"></param>
    public delegate void OrderHandler(Order order);

    /// <summary>
    /// Обработчик события OnTrade
    /// </summary>
    /// <param name="trade"></param>
    public delegate void TradeHandler(Trade trade);

    /// <summary>
    /// Обработчик события OnParam
    /// </summary>
    /// <param name="par">lua table with class_code, sec_code</param>
    public delegate void ParamHandler(Param par);

    /// <summary>
    /// Обработчик события OnStopOrder
    /// </summary>
    /// <param name="stopOrder"></param>
    public delegate void StopOrderHandler(StopOrder stopOrder);

    /// <summary>
    /// Обработчик события OnAccountBalance
    /// </summary>
    /// <param name="accBal"></param>
    public delegate void AccountBalanceHandler(AccountBalance accBal);

    /// <summary>
    /// Обработчик события OnAccountPosition
    /// </summary>
    /// <param name="accPos"></param>
    public delegate void AccountPositionHandler(AccountPosition accPos);

    /// <summary>
    /// Обработчик события OnDepoLimit
    /// </summary>
    /// <param name="dLimit"></param>
    public delegate void DepoLimitHandler(DepoLimitEx dLimit);

    /// <summary>
    /// Обработчик события OnDepoLimitDelete
    /// </summary>
    /// <param name="dLimitDel"></param>
    public delegate void DepoLimitDeleteHandler(DepoLimitDelete dLimitDel);

    /// <summary>
    /// Обработчик события OnFirm
    /// </summary>
    /// <param name="frm"></param>
    public delegate void FirmHandler(Firm frm);

    /// <summary>
    /// Обработчик события OnFuturesClientHolding
    /// </summary>
    /// <param name="futPos"></param>
    public delegate void FuturesClientHoldingHandler(FuturesClientHolding futPos);

    /// <summary>
    /// Обработчик события OnFuturesLimitChange
    /// </summary>
    /// <param name="futLimit"></param>
    public delegate void FuturesLimitHandler(FuturesLimits futLimit);

    /// <summary>
    /// Обработчик события OnFuturesLimitDelete
    /// </summary>
    /// <param name="limDel"></param>
    public delegate void FuturesLimitDeleteHandler(FuturesLimitDelete limDel);

    /// <summary>
    /// Обработчик события OnMoneyLimit
    /// </summary>
    /// <param name="mLimit"></param>
    public delegate void MoneyLimitHandler(MoneyLimitEx mLimit);

    /// <summary>
    /// Обработчик события OnMoneyLimitDelete
    /// </summary>
    /// <param name="mLimitDel"></param>
    public delegate void MoneyLimitDeleteHandler(MoneyLimitDelete mLimitDel);

    public delegate void CandleHandler(Candle candle);
}

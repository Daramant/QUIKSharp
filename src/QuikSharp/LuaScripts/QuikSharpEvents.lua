--~ Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
--~ Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

package.path = package.path..";"..".\\?.lua;"..".\\?.luac"

local QuikSharpEvents = {}

---------------------------------
-- Функции обратного вызова.
---------------------------------

-- Функция вызывается терминалом QUIK при получении изменений текущей позиции по счету.
function OnAccountBalance(acc_bal)
    local msg = {}
    msg.t = timemsec()
    msg.data = acc_bal
    msg.n = "AccountBalance"
    sendEvent(msg)
end

-- Функция вызывается терминалом QUIK при изменении позиции участника по денежным средствам. 
function OnAccountPosition(acc_pos)
    local msg = {}
    msg.t = timemsec()
    msg.data = acc_pos
    msg.n = "AccountPosition"
    sendEvent(msg)
end

-- Функция вызывается терминалом QUIK при получении обезличенной сделки.
function OnAllTrade(alltrade)
    if is_connected then
        local msg = {}
        msg.t = timemsec()
        msg.n = "AllTrade"
        msg.data = alltrade
        sendEvent(msg)
    end
end


-- Функция вызывается терминалом QUIK в следующих случаях: 
-- * смена сервера QUIK внутри торговой сессии; 
-- * смена пользователя, которым выполняется подключение к серверу QUIK, внутри торговой сессии; 
-- * смена сессии. 
function OnCleanUp()
end

-- Функция вызывается перед закрытием терминала QUIK и при выгрузке файла qlua.dll. 
function OnClose()
    if is_connected then
        local msg = {}
        msg.n = "Close"
        msg.t = timemsec()
        msg.data = ""
        sendEvent(msg)
    end
    CleanUp()
end

-- Функция вызывается терминалом QUIK при установлении связи с сервером QUIK и получении терминалом описания 
-- хотя бы одного класса. Если в течение торгового дня терминал получает новый класс, то функция вызывается 
-- еще раз, при этом параметр вызова flag принимает значение «false». 
function OnConnected(flag)
    local msg = {}
    msg.n = "Connected"
    msg.t = timemsec()
    msg.data = ""
    sendEvent(msg)
end

-- Функция вызывается терминалом QUIK при получении изменений позиции по инструментам.
function OnDepoLimit(dlimit)
    local msg = {}
    msg.t = timemsec()
    msg.data = dlimit
    msg.n = "DepoLimit"
    sendEvent(msg)
end

-- Функция вызывается терминалом QUIK при удалении позиции клиента по инструментам.
function OnDepoLimitDelete(dlimit_del)
    local msg = {}
    msg.t = timemsec()
    msg.data = dlimit_del
    msg.n = "DepoLimitDelete"
    sendEvent(msg)
end

-- Функция вызывается терминалом QUIK при отключении от сервера QUIK.
function OnDisconnected()
    local msg = {}
    msg.n = "Disconnected"
    msg.t = timemsec()
    msg.data = ""
    sendEvent(msg)
end

-- Функция вызывается терминалом QUIK при получении описания новой фирмы от сервера.
function OnFirm(firm)
    local msg = {}
    msg.t = timemsec()
    msg.data = firm
    msg.n = "Firm"
    sendEvent(msg)
end

-- Функция вызывается терминалом QUIK при изменении позиции по срочному рынку.
function OnFuturesClientHolding(fut_pos)
    local msg = {}
    msg.t = timemsec()
    msg.data = fut_pos
    msg.n = "FuturesClientHolding"
    sendEvent(msg)
end

-- Функция вызывается терминалом QUIK при получении изменений ограничений по срочному рынку.
function OnFuturesLimitChange(fut_limit)
    local msg = {}
    msg.t = timemsec()
    msg.data = fut_limit
    msg.n = "FuturesLimitChange"
    sendEvent(msg)
end

-- Функция вызывается терминалом QUIK при удалении лимита по срочному рынку.
function OnFuturesLimitDelete(lim_del)
    local msg = {}
    msg.t = timemsec()
    msg.data = lim_del
    msg.n = "FuturesLimitDelete"
    sendEvent(msg)
end

-- Функция вызывается терминалом QUIK перед вызовом функции main(). В качестве параметра принимает 
-- значение полного пути к запускаемому скрипту.
function OnInit(script_path)
    if is_connected then
        local msg = {}
        msg.n = "Init"
        msg.t = timemsec()
        msg.data = script_path
        sendEvent(msg)
    end
    log("QUIK# is initialized from "..script_path, 0)
end

-- Функция вызывается терминалом QUIK при получении изменений по денежной позиции.
function OnMoneyLimit(mlimit)
    local msg = {}
    msg.t = timemsec()
    msg.data = mlimit
    msg.n = "MoneyLimit"
    sendEvent(msg)
end

-- Функция вызывается терминалом QUIK при удалении денежной позиции.
function OnMoneyLimitDelete(mlimit_del)
    local msg = {}
    msg.t = timemsec()
    msg.data = mlimit_del
    msg.n = "MoneyLimitDelete"
    sendEvent(msg)
end

-- Функция вызывается терминалом QUIK при получении внебиржевой заявки или при изменении параметров существующей внебиржевой заявки.
function OnNegDeal(neg_deals)
end

-- Функция вызывается терминалом QUIK при получении сделки для исполнения или при изменении параметров существующей сделки для исполнения.
function OnNegTrade(neg_trade)
end

-- Функция вызывается терминалом QUIK при получении новой заявки или при изменении параметров существующей заявки.
function OnOrder(order)
    local msg = {}
    msg.t = timemsec()
    msg.id = nil
    msg.data = order
    msg.n = "Order"
    sendEvent(msg)
end

-- Функция вызывается терминалом QUIK при изменении текущих параметров.
function OnParam(class_code, sec_code)
    local msg = {}
    msg.n = "Param"
    msg.t = timemsec()
	local dat = {}
	dat.class_code = class_code
	dat.sec_code = sec_code
    msg.data = dat
    sendEvent(msg)
end

-- Функция вызывается терминалом QUIK при получении изменения стакана котировок.
function OnQuote(class_code, sec_code)
    if is_connected then
        local msg = {}
        msg.n = "Quote"
        msg.t = timemsec()
        local server_time = getInfoParam("SERVERTIME")
        local status, ql2 = pcall(getQuoteLevel2, class_code, sec_code)
        if status then
            msg.data = ql2
            msg.data.class_code = class_code
            msg.data.sec_code = sec_code
            msg.data.server_time = server_time
            sendEvent(msg)
        else
            OnError(ql2)
        end
    end
end

-- Функция вызывается терминалом QUIK при остановке скрипта из диалога управления и при закрытии терминала QUIK.
function OnStop(flag)
    isStarted = false

    if is_connected then
        local msg = {}
        msg.n = "Stop"
        msg.t = timemsec()
        msg.data = flag
        sendEvent(msg)
    end
    log("QUIK# stopped. You could keep script running when closing QUIK and the script will start automatically the next time you start QUIK", 1)
    CleanUp()
    --	send disconnect
    return 1000
end

-- Функция вызывается терминалом QUIK при получении новой стоп-заявки или при изменении параметров существующей стоп-заявки.
function OnStopOrder(stop_order)
	local msg = {}
    msg.t = timemsec()
    msg.data = stop_order
    msg.n = "StopOrder"
    sendEvent(msg)
end

-- Функция вызывается терминалом QUIK при получении сделки или при изменении параметров существующей сделки.
function OnTrade(trade)
    local msg = {}
    msg.t = timemsec()
    msg.id = nil
    msg.data = trade
    msg.n = "Trade"
    sendEvent(msg)
end

-- Функция OnTransReply вызывается терминалом QUIK при получении ответа на транзакцию пользователя, отправленную 
-- с помощью любого плагина Рабочего места QUIK (в том числе QLua). Для транзакций, отправленных с помощью 
-- Trans2quik.dll, QPILE или динамической загрузки транзакций из файла, функция не вызывается.
function OnTransReply(trans_reply)
    local msg = {}
    msg.t = timemsec()
    msg.id = nil
    msg.data = trans_reply
    msg.n = "TransReply"
    sendEvent(msg)
end









local function CleanUp()
    closeLog()
end

function OnQuikSharpDisconnected()
    -- TODO any recovery or risk management logic here
end

function OnError(message)
	if is_connected then
		local msg = {}
		msg.t = timemsec()
		msg.n = "Error"
		msg.data = "Lua error: " .. message
		sendEvent(msg)
	end
end

return QuikSharpEvents
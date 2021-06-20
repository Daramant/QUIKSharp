--~ Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
--~ Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

package.path = package.path..";"..".\\?.lua;"..".\\?.luac"

local QuikSharpEvents = {}

---------------------------------
-- ������� ��������� ������.
---------------------------------

-- ������� ���������� ���������� QUIK ��� ��������� ��������� ������� ������� �� �����.
function OnAccountBalance(acc_bal)
    local msg = {}
    msg.t = timemsec()
    msg.data = acc_bal
    msg.n = "AccountBalance"
    sendEvent(msg)
end

-- ������� ���������� ���������� QUIK ��� ��������� ������� ��������� �� �������� ���������. 
function OnAccountPosition(acc_pos)
    local msg = {}
    msg.t = timemsec()
    msg.data = acc_pos
    msg.n = "AccountPosition"
    sendEvent(msg)
end

-- ������� ���������� ���������� QUIK ��� ��������� ������������ ������.
function OnAllTrade(alltrade)
    if is_connected then
        local msg = {}
        msg.t = timemsec()
        msg.n = "AllTrade"
        msg.data = alltrade
        sendEvent(msg)
    end
end


-- ������� ���������� ���������� QUIK � ��������� �������: 
-- * ����� ������� QUIK ������ �������� ������; 
-- * ����� ������������, ������� ����������� ����������� � ������� QUIK, ������ �������� ������; 
-- * ����� ������. 
function OnCleanUp()
end

-- ������� ���������� ����� ��������� ��������� QUIK � ��� �������� ����� qlua.dll. 
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

-- ������� ���������� ���������� QUIK ��� ������������ ����� � �������� QUIK � ��������� ���������� �������� 
-- ���� �� ������ ������. ���� � ������� ��������� ��� �������� �������� ����� �����, �� ������� ���������� 
-- ��� ���, ��� ���� �������� ������ flag ��������� �������� �false�. 
function OnConnected(flag)
    local msg = {}
    msg.n = "Connected"
    msg.t = timemsec()
    msg.data = ""
    sendEvent(msg)
end

-- ������� ���������� ���������� QUIK ��� ��������� ��������� ������� �� ������������.
function OnDepoLimit(dlimit)
    local msg = {}
    msg.t = timemsec()
    msg.data = dlimit
    msg.n = "DepoLimit"
    sendEvent(msg)
end

-- ������� ���������� ���������� QUIK ��� �������� ������� ������� �� ������������.
function OnDepoLimitDelete(dlimit_del)
    local msg = {}
    msg.t = timemsec()
    msg.data = dlimit_del
    msg.n = "DepoLimitDelete"
    sendEvent(msg)
end

-- ������� ���������� ���������� QUIK ��� ���������� �� ������� QUIK.
function OnDisconnected()
    local msg = {}
    msg.n = "Disconnected"
    msg.t = timemsec()
    msg.data = ""
    sendEvent(msg)
end

-- ������� ���������� ���������� QUIK ��� ��������� �������� ����� ����� �� �������.
function OnFirm(firm)
    local msg = {}
    msg.t = timemsec()
    msg.data = firm
    msg.n = "Firm"
    sendEvent(msg)
end

-- ������� ���������� ���������� QUIK ��� ��������� ������� �� �������� �����.
function OnFuturesClientHolding(fut_pos)
    local msg = {}
    msg.t = timemsec()
    msg.data = fut_pos
    msg.n = "FuturesClientHolding"
    sendEvent(msg)
end

-- ������� ���������� ���������� QUIK ��� ��������� ��������� ����������� �� �������� �����.
function OnFuturesLimitChange(fut_limit)
    local msg = {}
    msg.t = timemsec()
    msg.data = fut_limit
    msg.n = "FuturesLimitChange"
    sendEvent(msg)
end

-- ������� ���������� ���������� QUIK ��� �������� ������ �� �������� �����.
function OnFuturesLimitDelete(lim_del)
    local msg = {}
    msg.t = timemsec()
    msg.data = lim_del
    msg.n = "FuturesLimitDelete"
    sendEvent(msg)
end

-- ������� ���������� ���������� QUIK ����� ������� ������� main(). � �������� ��������� ��������� 
-- �������� ������� ���� � ������������ �������.
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

-- ������� ���������� ���������� QUIK ��� ��������� ��������� �� �������� �������.
function OnMoneyLimit(mlimit)
    local msg = {}
    msg.t = timemsec()
    msg.data = mlimit
    msg.n = "MoneyLimit"
    sendEvent(msg)
end

-- ������� ���������� ���������� QUIK ��� �������� �������� �������.
function OnMoneyLimitDelete(mlimit_del)
    local msg = {}
    msg.t = timemsec()
    msg.data = mlimit_del
    msg.n = "MoneyLimitDelete"
    sendEvent(msg)
end

-- ������� ���������� ���������� QUIK ��� ��������� ����������� ������ ��� ��� ��������� ���������� ������������ ����������� ������.
function OnNegDeal(neg_deals)
end

-- ������� ���������� ���������� QUIK ��� ��������� ������ ��� ���������� ��� ��� ��������� ���������� ������������ ������ ��� ����������.
function OnNegTrade(neg_trade)
end

-- ������� ���������� ���������� QUIK ��� ��������� ����� ������ ��� ��� ��������� ���������� ������������ ������.
function OnOrder(order)
    local msg = {}
    msg.t = timemsec()
    msg.id = nil
    msg.data = order
    msg.n = "Order"
    sendEvent(msg)
end

-- ������� ���������� ���������� QUIK ��� ��������� ������� ����������.
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

-- ������� ���������� ���������� QUIK ��� ��������� ��������� ������� ���������.
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

-- ������� ���������� ���������� QUIK ��� ��������� ������� �� ������� ���������� � ��� �������� ��������� QUIK.
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

-- ������� ���������� ���������� QUIK ��� ��������� ����� ����-������ ��� ��� ��������� ���������� ������������ ����-������.
function OnStopOrder(stop_order)
	local msg = {}
    msg.t = timemsec()
    msg.data = stop_order
    msg.n = "StopOrder"
    sendEvent(msg)
end

-- ������� ���������� ���������� QUIK ��� ��������� ������ ��� ��� ��������� ���������� ������������ ������.
function OnTrade(trade)
    local msg = {}
    msg.t = timemsec()
    msg.id = nil
    msg.data = trade
    msg.n = "Trade"
    sendEvent(msg)
end

-- ������� OnTransReply ���������� ���������� QUIK ��� ��������� ������ �� ���������� ������������, ������������ 
-- � ������� ������ ������� �������� ����� QUIK (� ��� ����� QLua). ��� ����������, ������������ � ������� 
-- Trans2quik.dll, QPILE ��� ������������ �������� ���������� �� �����, ������� �� ����������.
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
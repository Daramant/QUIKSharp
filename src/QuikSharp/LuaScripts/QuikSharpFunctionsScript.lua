local Config = require "QuikSharpFunctionsScriptConfig"
local Server = require "Server"
local LoggerFactory = require "LoggerFactory"
local Utilities = require "Utilities"
local QuikSharpFunctions = require "QuikSharpFunctions"
require "QuikSharpEvents"

local loggerFactory = LoggerFactory:new(Config.logging)
local logger = loggerFactory:get("QuikSharpScript")

local Serializer = require "Serializer"
local serializer = Serializer:new()

local server = Server:new()
local isScriptStarted = false
local waitForClientConnectionAttemptCount = 0

if Utilities.isQuik() then
    local quikVersion = Utilities.getQuikVersion()

    if quikVersion ~= nil then
        Utilities.addPathsByQuikVersion(quikVersion)
        logger:info("������ ��������� QUIK: '" .. quikVersion .. "'. ������������ cpath: '" .. package.cpath .. "'.", 0)
    else
		message("������� QuikSharpFunctionsScript �� ������� ���������� ������ ��������� QUIK.", 3)
        logger:error("������� QuikSharpFunctionsScript �� ������� ���������� ������ ��������� QUIK.")
	end
else
    logger:info("������ QuikSharpFunctionsScript ������� ��� ��������� QUIK.")
    main()
end

-- �������, ����������� �������� ����� ���������� � �������. ��� �� ���������� �������� QUIK ������� ��������� �����. 
-- ������ ��������� ����������, ���� �������� ������� main(). ��� ���������� ������ ������� main() ������ ��������� 
-- � ��������� �����������. ���� ������ ��������� � ��������� �����������, �� �� ���������� ������� ������� ��������� 
-- ������� ��������� QUIK, ������������ � ���� �������. 
function main()
    logger:info("������ QuikSharpFunctionsScript �������.")

    local success, err = pcall(doMain)
    
    if success then
        logger:info("���������� ������� QuikSharpFunctionsScript ������� ���������.")
    else
        logger:error("��� ��������� ������� QuikSharpFunctionsScript �������� ������: " .. err)
    end

    logger:dispose()
end

function doMain()
    isScriptStarted = true
    server:init(Config.server)

    while isScriptStarted do
        local success, err = pcall(doLoop)

        if not success then
            logger:error("������ � ����� ��������� �������: " .. err);
            Utilities.delay(Config.script.loopDelayOnError)
        end
    end
end

function doLoop()
    if not server:isClientConnected() then
        self.waitForClientConnection()

        if server:isClientConnected() then
            waitForClientConnectionAttemptCount = 0
            logger:info("����������� ���������� � ��������: " .. server:getClientInfo())
        else
            waitForClientConnectionAttemptCount += 1

            if waitForClientConnectionAttemptCount >= Config.script.waitForClientConnectionAttemptLoggingInterval then
                waitForClientConnectionAttemptCount = 0
                logger:info("������ QuikSharpFunctionsScript ������� ����������� ��������...")
            end

            Utilities.delay(Config.script.waitForClientConnectionDelay)
            return
        end
    end

    local commandString = server:receive()
    logger:debug("�������� ��������: '" .. commandString .. "'.");

    local command = serializer:deserialize(commandString)

    local result = QuikSharpFunctions.callFunction(command)
    
    if result.name == "Error" then
        logger:error("�� ������� ��������� �������: '", commandString, "'. ������: '", result, "'.");
    else
        logger:debug("��������� ���������� �������: '", result, "'.");
    end
    
    local resultString = buildResultString(result)

    logger:debug("������������ ������� ���������: '", resultString, "'.");

    server:send(resultString)
end

function buildResultString(result)
    local header = {}
    header.cid = result.id

    if (result.name == "Error")
        header.status = "Error"
    else
        header.status = "Ok"
    end

    local headerString = serializer:serialize(header)
    local bodyString = serializer:serialize(result.data)

    return headerString .. bodyString
end
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
        logger:info("Версия терминала QUIK: '" .. quikVersion .. "'. Используются cpath: '" .. package.cpath .. "'.", 0)
    else
		message("Скрипту QuikSharpFunctionsScript не удалось определить версию терминала QUIK.", 3)
        logger:error("Скрипту QuikSharpFunctionsScript не удалось определить версию терминала QUIK.")
	end
else
    logger:info("Скрипт QuikSharpFunctionsScript запущен вне терминала QUIK.")
    main()
end

-- Функция, реализующая основной поток выполнения в скрипте. Для ее выполнения терминал QUIK создает отдельный поток. 
-- Скрипт считается работающим, пока работает функция main(). При завершении работы функции main() скрипт переходит 
-- в состояние «остановлен». Если скрипт находится в состоянии «остановлен», то не происходит вызовов функций обработки 
-- событий терминала QUIK, содержащихся в этом скрипте. 
function main()
    logger:info("Скрипт QuikSharpFunctionsScript запущен.")

    local success, err = pcall(doMain)
    
    if success then
        logger:info("Выполнение скрипта QuikSharpFunctionsScript успешно завершено.")
    else
        logger:error("При выполении скрипта QuikSharpFunctionsScript возникла ошибка: " .. err)
    end

    logger:dispose()
end

function doMain()
    isScriptStarted = true
    server:init(Config.server)

    while isScriptStarted do
        local success, err = pcall(doLoop)

        if not success then
            logger:error("Ошибка в цикле обработки комманд: " .. err);
            Utilities.delay(Config.script.loopDelayOnError)
        end
    end
end

function doLoop()
    if not server:isClientConnected() then
        self.waitForClientConnection()

        if server:isClientConnected() then
            waitForClientConnectionAttemptCount = 0
            logger:info("Установлено соединение с клиентом: " .. server:getClientInfo())
        else
            waitForClientConnectionAttemptCount += 1

            if waitForClientConnectionAttemptCount >= Config.script.waitForClientConnectionAttemptLoggingInterval then
                waitForClientConnectionAttemptCount = 0
                logger:info("Скрипт QuikSharpFunctionsScript ожидает подключения клиентов...")
            end

            Utilities.delay(Config.script.waitForClientConnectionDelay)
            return
        end
    end

    local commandString = server:receive()
    logger:debug("Получена комманда: '" .. commandString .. "'.");

    local command = serializer:deserialize(commandString)

    local result = QuikSharpFunctions.callFunction(command)
    
    if result.name == "Error" then
        logger:error("Не удалось выполнить команду: '", commandString, "'. Ошибка: '", result, "'.");
    else
        logger:debug("Результат выполнения команды: '", result, "'.");
    end
    
    local resultString = buildResultString(result)

    logger:debug("Отправляемый клиенту результат: '", resultString, "'.");

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
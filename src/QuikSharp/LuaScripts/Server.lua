local socket = require ("socket")

local Server = {}

function Server:new()
	local obj = {}
	self.__index = self
	return setmetatable(obj, self) 
end

function Server:init(options)
	self.server = socket.bind(options.host, options.port, 1)
	self.server:settimeout(options.timeout)

	self.client = nil

	return self
end

function Server:isClientConnected()
	return self.client ~= nil
end

function Server:getClientInfo()
    return self.client:getpeername()
end

function Server:waitForClientConnection()
	local success, client, error = pcall(self.server.accept, self.server)
	if success then
        if client then
            self.client = client;
		    return true
        else
            self.client = nil
            return false
        end
	else
        self.client = nil
		error(client)
	end
end

function Server:receive()
    if self.client == nil then
        error("Не задан клиент.")
    end

    local success, data, err = pcall(self.client.receive, self.client)

    if success then
        if data then
            return data
        else
            self:disconnectClient()
            error("Не удалось получить данные от клиента. Ошибка: " .. err)
        end
    else
        self:disconnectClient()
        error("Ошибка при получении данных от клиента. Ошибка: " .. data)
    end
end

function Server:disconnectClient()
    if self.client ~= nil then
        pcall(self.client.close, self.client)
        self.client = nil
    end
end

function Server:send(data)
    if self.client == nil then
        error("Не задан клиент.")
    end

    local success, result, err = pcall(self.client.send, self.client, data .. '\n')

    if success then
        if result == nil then
            self:disconnectClient()
            error("Не удалось отправить данные клиенту. Ошибка: " .. err)
        end
    else
        self:disconnectClient()
        error("Ошибка при отправке данных клиенту. Ошибка: " .. result)
    end
end

return Server
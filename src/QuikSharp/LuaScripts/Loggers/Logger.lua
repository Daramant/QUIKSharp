local LoggerEnums = require "LoggerEnums"
local Serializer = require "Serializer"
local Logger = {}

function Logger:new()
    local obj = {}
    self.__index = self
    return setmetatable(obj, self) 
end

function Logger:init(level, name)
    self.level = level
    self.name = name
    self.serializer = Serializer:new()
    return self
end

function Logger:debug(...)
    if self.level <= LoggerEnums.level.debug then
        self:log('debug', arg)
    end
end

function Logger:info(...)
    if self.level <= LoggerEnums.level.info then
        self:log('info', arg)
    end
end

function Logger:warning(...)
    if self.level <= LoggerEnums.level.warning then
        self:log('warning', arg)
    end
end

function Logger:errors(...)
    if self.level <= LoggerEnums.level.errors then
        self:log('error', arg)
    end
end

function Logger:buildMessage(args)
    local t = { }
    for index, value in ipairs(args) do
        local valueType = 

        if type (value) == 'table' then
            t[#t+1] = self.serializer:serialize(value)
        else
            t[#t+1] = tostring(v)
        end
    end

    return table.concat(t, " ")
end

function Logger:log(level, message)
end

function Logger:dispose()
end

return Logger
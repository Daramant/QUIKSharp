local Utilities = require "Utilities"
local LoggerEnums = require "LoggerEnums"
local DebugLogger = require "DebugLogger"
local FileLogger = require "FileLogger"
local ConsoleLogger = require "ConsoleLogger"
local NullLogger = require "NullLogger"

local LoggerFactory = {}

function LoggerFactory:new(config)
    local obj = {}
    obj.config = config
  
    obj.modeSwitch = Utilities.switch {
        [LoggerEnums.mode.none] = function (mode, name) return NullLogger:new():init(config.level, name) end,
        [LoggerEnums.mode.debug] = function (mode, name) return DebugLogger:new():init(config.level, name) end,
        [LoggerEnums.mode.file] = function (mode, name) return FileLogger:new():init(config.level, name, config.filePath) end,
        [LoggerEnums.mode.console] = function (mode, name) return ConsoleLogger:new():init(config.level, name) end,
        default = function (mode, name) error("Режим логирования: '" .. mode .. "' не поддерживается (LoggerName: '" .. name .. "').") end
    }
  
    self.__index = self
    return setmetatable(obj, self)
end

function LoggerFactory:get(name)
    return self.modeSwitch:case(self.config.mode, name)
end

return LoggerFactory
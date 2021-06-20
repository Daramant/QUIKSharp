local Utilities = require "Utilities"
local io = require "io"
local Logger = require "Logger"

local FileLogger = Logger:new()

function FileLogger:init(level, name, filePath)
    Logger.init(self, level, name)
    self.fileName = filePath .. os.date("%Y.%m.%d") .. " " .. name .. ".log"
    self:openLogFile()
    return self
end

function FileLogger:log(level, args)
    self.file:write(tostring(os.date("%H:%M:%S")) .. " [" .. self.name .. "] [" .. level .. "] " .. self:buildMessage(args) .. "\n")
    self.file:flush()
end

function FileLogger:dispose()
    self.file:flush()
    self.file:close()
end

function FileLogger:openLogFile()
    local file, err = io.open(self.fileName, "a")

    if not file then
        error("Не удалось открывать файл: ;" .. self.fileName .. "'. Error: " .. err)
    end

    self.file = file
end

return FileLogger
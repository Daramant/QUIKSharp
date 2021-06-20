local Utilities = require "Utilities"
local Logger = require "Logger"

local ConsoleLogger = Logger:new()

function ConsoleLogger:log(level, args)
	print(tostring(os.date("%H:%M:%S")) .. " [" .. self.name .. "] [" .. level .. "] " .. self:buildMessage(args))
end

return ConsoleLogger
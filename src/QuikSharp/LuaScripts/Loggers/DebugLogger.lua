local Utilities = require "Utilities"
local Logger = require "Logger"

local DebugLogger = Logger:new()

function DebugLogger:log(level, args)
	print(tostring(os.date("%H:%M:%S")) .. " [" .. self.name .. "] [" .. level .. "] " .. self:buildMessage(args))
end

return DebugLogger
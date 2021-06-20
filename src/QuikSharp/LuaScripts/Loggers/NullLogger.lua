local Logger = require "Logger"

local NullLogger = Logger:new()

function NullLogger:log(level, message)
end

return NullLogger
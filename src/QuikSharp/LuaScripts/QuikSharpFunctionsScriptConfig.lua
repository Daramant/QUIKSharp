local LoggerEnums = require "LoggerEnums"

local Config = {
  logging = {
    mode = LoggerEnums.mode.file,
    level = LoggerEnums.level.debug,
    filePath = getScriptPath() .. "\\Logs\\"
  },

  server = {
    host = "127.0.0.1",
    port = 34130,
    timeout = 1 -- In seconds.
  }

  script = {
    loopDelayOnError = 100 -- In milliseconds.
    waitForClientConnectionDelay = 100 -- In milliseconds.
    waitForClientConnectionAttemptLoggingInterval = 20 
  }
}

return Config
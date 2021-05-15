local LoggerEnums = require "LoggerEnums"

local Config = {
  logging = {
    mode = LoggerEnums.mode.file,
    level = LoggerEnums.level.debug,
    filePath = getScriptPath() .. "\\Logs\\"
  },

  server = {
    commands = {
        host = "127.0.0.1",
        port = 34130
    },
    events = {
        host = "127.0.0.1",
        port = 34131
    }
  }
}

return Config
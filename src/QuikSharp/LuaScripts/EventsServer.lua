local socket = require ("socket")
local json = require ("dkjson")
local EventsServer = {}

-- high precision current time
function timemsec()
    local st, res = pcall(socket.gettime)
    if st then
        return (res) * 1000
    else
        log("unexpected error in timemsec", 3)
        error("unexpected error in timemsec")
    end
end

-- when true will show QUIK message for log(...,0)
is_debug = false

-- log files

function openLog()
    os.execute("mkdir \""..script_path.."\\logs\"")
    local lf = io.open (script_path.. "\\logs\\QUIK#_"..os.date("%Y%m%d")..".log", "a")
    if not lf then
        lf = io.open (script_path.. "\\QUIK#_"..os.date("%Y%m%d")..".log", "a")
    end
    return lf
end

-- closes log
function closeLog()
    if logfile then
        pcall(logfile:close(logfile))
    end
end

logfile = openLog()

--- Write to log file and to Quik messages
function log(msg, level)
    if not msg then msg = "" end
    if level == 1 or level == 2 or level == 3 or is_debug then
        -- only warnings and recoverable errors to Quik
        if message then
            pcall(message, msg, level)
        end
    end
    if not level then level = 0 end
    local logLine = "LOG "..level..": "..msg
    print(logLine)
    local msecs = math.floor(math.fmod(timemsec(), 1000));
    if logfile then
        pcall(logfile.write, logfile, os.date("%Y-%m-%d %H:%M:%S").."."..msecs.." "..logLine.."\n")
        pcall(logfile.flush, logfile)
    end
end

-- current connection state
is_connected = false
local command_server
local event_server
local command_client
local event_client

--- accept client on server
local function getResponseServer()
    log('Waiting for a response client...', 0)
	if not command_server then
		log("Cannot bind to command_server, probably the port is already in use", 3)
	else
		while true do
			local status, client, err = pcall(command_server.accept, command_server )
			if status and client then
				return client
			else
				log(err, 3)
			end
		end
	end
end

local function getCallbackClient()
    log('Waiting for a callback client...', 0)
	if not event_server then
		log("Cannot bind to event_server, probably the port is already in use", 3)
	else
		while true do
			local status, client, err = pcall(event_server.accept, event_server)
			if status and client then
				return client
			else
				log(err, 3)
			end
		end
	end
end

function EventsServer.connect(command_host, command_port, event_host, event_port)
    if not command_server then
        command_server = socket.bind(command_host, command_port, 1)
    end
    if not event_server then
        event_server = socket.bind(event_host, event_port, 1)
    end

    if not is_connected then
        log('QUIK# is waiting for client connection...', 1)
        if command_client then
            log("is_connected is false but the response client is not nil", 3)
            -- Quik crashes without pcall
            pcall(command_client.close, command_client)
        end
        if event_client then
            log("is_connected is false but the callback client is not nil", 3)
            -- Quik crashes without pcall
            pcall(event_client.close, event_client)
        end
        command_client = getResponseServer()
        event_client = getCallbackClient()
        if command_client and event_client then
            is_connected = true
            log('QUIK# client connected', 1)
        end
    end
end

local function disconnected()
    is_connected = false
    log('QUIK# client disconnected', 1)
    if command_client then
        pcall(command_client.close, command_client)
        command_client = nil
    end
    if event_client then
        pcall(event_client.close, event_client)
        event_client = nil
    end
    OnQuikSharpDisconnected()
end

--- get a decoded message as a table
function receiveCommand()
    if not is_connected then
        return nil, "not conencted"
    end
    local status, requestString= pcall(command_client.receive, command_client)
    if status and requestString then
        local msg_table, err = from_json(requestString)
        if err then
            log(err, 3)
            return nil, err
        else
            return msg_table
        end
    else
        disconnected()
        return nil, err
    end
end

function sendResult(msg_table)
    -- if not set explicitly then set CreatedTime "t" property here
    -- if not msg_table.t then msg_table.t = timemsec() end
    local responseString = to_json(msg_table)
    if is_connected then
        local status, res = pcall(command_client.send, command_client, responseString..'\n')
        if status and res then
            return true
        else
            disconnected()
            return nil, err
        end
    end
end

function sendEvent(msg)
    -- if not set explicitly then set CreatedTime "t" property here
    -- if not msg.t then msg.t = timemsec() end
    local event_string = to_json(msg)
    if is_connected then
        local status, res = pcall(event_client.send, event_client, event_string..'\n')
        if status and res then
            return true
        else
            disconnected()
            return nil, err
        end
    end
end

return EventsServer

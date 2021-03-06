--~ Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
--~ Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

local socket = require ("socket")
local json = require ("dkjson")
local qsutils = {}

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

function qsutils.connect(command_host, command_port, event_host, event_port)
    if not command_server then
        command_server = socket.bind(command_host, command_port, 1)
    end
    if not event_server then
        event_server = socket.bind(event_host, event_port, 1)
    end

    if not is_connected then
        log('QUIK# is waiting for client connection...', 1)
        
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

return qsutils

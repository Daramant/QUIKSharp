local json = require ("dkjson")

local Serializer = {}

function Serializer:new()
	local obj = {}
	self.__index = self
	return setmetatable(obj, self) 
end

function Serializer:serialize(data)
	return json.encode(msg, { indent = false })
end

function Serializer:deserialize(data)
	return json.decode(str, 1, json.null)
end

return Serializer
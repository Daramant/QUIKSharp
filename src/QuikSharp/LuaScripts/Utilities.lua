local socket = require ("socket")

local Utilities = {}

function Utilities.extend(child, parent)
    return setmetatable(child, { __index = parent }) 
end

function Utilities.switch(t)
    t.case = function (self, caseValue, data)
        local f = self[caseValue] or self.default
        if f then
            if type(f) == "function" then
                return f(caseValue, data, self)
            else
                error("case ".. tostring(caseValue) .. " not a function")
            end
        end
        return nil
    end
    return t
end

function Utilities.addTable(table)
    for key, value in pairs(table) do
        self[key] = value
    end
end

function Utilities.isQuik()
    if getScriptPath then 
        return true 
    else 
        return false 
    end
end

function Utilities.getScriptPath()
    if getScriptPath then 
        return getScriptPath()
    else 
        return "." 
    end
end

function Utilities.getQuikVersion()
    if not Utilities.isQuik() then
        return nil
    end

	quikVersion = getInfoParam("VERSION")

	if quikVersion ~= nil then
		quikVersion = tonumber(quikVersion:match("%d+%.%d+"))
	end

	return quikVersion
end

function Utilities.getLibPathByQuikVersion(quikVersion)
    if quikVersion == nil then
        return
    end

    -- MD dynamic, requires MSVCRT
    -- MT static, MSVCRT is linked statically with luasocket
    -- package.cpath contains info.exe working directory, which has MSVCRT, so MT should not be needed in theory, 
    -- but in one issue someone said it doesn't work on machines that do not have Visual Studio. 
    local linkage = "MD"
    local libPath = "\\clibs"

	if quikVersion >= 8.5 then
        libPath = libPath.."64\\53_"..linkage.."\\"
	elseif quikVersion >= 8 then
        libPath = libPath.."64\\5.1_"..linkage.."\\"
	else
		libPath = libPath.."\\5.1_"..linkage.."\\"
	end

    return libPath
end

function Utilities.addPathsByQuikVersion(quikVersion)
    if quikVersion == nil then
        return
    end

    local scriptPath = Utilities.getScriptPath()
    local libPath = Utilities.getLibPathByQuikVersion(quikVersion)

    package.path = package.path .. ";" .. scriptPath .. "\\?.lua;" .. scriptPath .. "\\?.luac"..";"..".\\?.lua;"..".\\?.luac"
    package.cpath = package.cpath .. ";" .. scriptPath .. libPath .. '?.dll'..";".. '.' .. libPath .. '?.dll'
end

function Utilities.delay(milliseconds)
    if sleep then
        sleep(milliseconds)
    else
        socket.sleep(milliseconds / 1000)
    end
end

function Utilities.getTime()
    return socket.gettime() * 1000
end

return Utilities
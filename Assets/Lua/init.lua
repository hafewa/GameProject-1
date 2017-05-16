require("common/init")
require("framework/init")
require("gameEventType")

--高亮打印
function lgyPrint(log)
    if true then --控制输出log开关
        LGYLOG.Log(log .. "")
    end
end

-- string_table = require("globalization/zh/string_table")
-- lgyPrint = LGYLOG.Log
GameObject = UnityEngine.GameObject
Object = UnityEngine.Object
Input = UnityEngine.Input
networkMgr = LuaHelper.GetNetManager()
allTimeTickerTb = {}--贯穿整个游戏的定时器




local network_manager = require "manager/network_manager"
networkMgr:SetLuaTable(network_manager())
networkMgr:SendConnect()

Resources = UnityEngine.Resources

require("uiscripts/wnd_base")
require "uiscripts/cm_gameinit_pan"
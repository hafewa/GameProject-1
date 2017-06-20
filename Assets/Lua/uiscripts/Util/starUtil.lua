--[[
    卡牌升星相关数据表的工具类
    armycardstar_c
    armycardstarcost_c
]]

local class = require("common/middleclass")
starUtil = class("starUtil")

--[[
    获取升星所提升的卡牌生命值
    cardId  卡牌id
    starLv  卡牌星级
]]
function starUtil:getCardStarHP(cardId, starLv)
    local uid = tonumber(string.format("%d%.2d",cardId,starLv))
    return sdata_armycardstar_data:GetFieldV("CardStarHP", uid)
end
--[[
    获取升星所提升的卡牌火力值
    cardId  卡牌id
    starLv  卡牌星级
]]
function starUtil:getCardStarAttack(cardId, starLv)
    local uid = tonumber(string.format("%d%.2d",cardId,starLv))
    return sdata_armycardstar_data:GetFieldV("CardStarAttack", uid)
end
--[[
    获取升星所提升的卡牌防御值
    cardId  卡牌id
    starLv  卡牌星级
]]
function starUtil:getCardStarDefense(cardId, starLv)
    local uid = tonumber(string.format("%d%.2d", cardId, starLv))
    return sdata_armycardstar_data:GetFieldV("CardStarDefense", uid)
end

--[[
    获取升至该星级所需卡牌碎片数量
    starLv  卡牌星级
]]
function starUtil:getUpStarNeedFragment(starLv)
    return sdata_armycardstarcost_data:GetFieldV("CardNum", starLv) 
end
--[[
    获取升至该星级兵牌数量
    starLv  卡牌星级
]]
function starUtil:getUpStarNeedCoin(starLv)
    return sdata_armycardstarcost_data:GetFieldV("Coin", starLv) 
end


return starUtil
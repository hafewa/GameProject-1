--region *.lua
--Date 20160616
--武将招募相关
--作者 gyt

HHeroClass = classWC()

--英雄属性名枚举
HInfo = {
	DataID = "HeroID",
	HC = "HC",
    LT = "LT",
	BT = "BT",
	Pid = "Pid",

} 

function HHeroClass:BindSyncObj(ownerPlayer,syncObj)
	self.Obj = syncObj
	self.OwnerPlayer = ownerPlayer
end

function HHeroClass:GetAttr(attr)
	return self.Obj:GetValue(attr)
end
function HHeroClass:GetNumberAttr(attr)
	return tonumber(self:GetAttr(attr))
end



--endregion
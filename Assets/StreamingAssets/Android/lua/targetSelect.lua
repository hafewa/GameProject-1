

TargetSelecter = {num = 0}
local json = require('json')  

-- �½�����
function TargetSelecter:new(o)
	o = o or {}
	setmetatable(o, self)
	self.__index = self
	return o
end

-- ��ȡ���󻺴� �������δ�����򻺴�
function TargetSelecter:Instance()
	if self.instance == nil then
		self.instance = self:new()
	end
	return self.instance
end


-- �������ñ��ȡ��λ��ѡȨ��ֵ
function GetWightWithName(oneTarget1, index, typeList, searchRange)
	
	-- Ȩ��ֵ
	local wight = 0
	-- Ӳɸѡ(�����������ų�)
	if index <= 3 and index >=1 then
		
		local targetValue
		for k, v in pairs(typeList) do
			
			targetValue = oneTarget1[k]
			-- �Ƿ��Ǹ�����
			if  targetValue ~= nil and targetValue == 1 then
				-- �������Ƿ�����ѡ
				if v >= 0 then
					wight = wight + v;
				else
					-- Ȩ��ֵΪ��, �����Ͳ�����Ϊ��ѡ��, ����-1
					wight = -1
					break;
				end
			end
		end
		-- ��ɸѡ(�����ų�)
	elseif index == 4 then
		
		-- �Ƕ� (180 - ��ǰ�н�) / 180 * �Ƕ�Ȩ��
		if typeList.RangeMax > 0 then
			wight = wight + (180 - oneTarget1.Angle) / 180 * typeList.AngleMin
		end
		-- Ѫ�� (���Ѫ�� - ��ǰѪ��)/���Ѫ�� * ����Ȩ��
		if typeList.HealthMax > 0 then
			wight = wight + (oneTarget1.HealthMax - oneTarget1.Health) / oneTarget1.HealthMax * typeList.HealthMax
		end
		if typeList.HealthMin > 0 then
			wight = wight + oneTarget1.Health / oneTarget1.HealthMax * typeList.HealthMin
		end
		-- ���� (������ - ��ǰ����)/������ * ����Ȩ��
		if searchRange > 0 then
			wight = wight + (searchRange - oneTarget1.Range) / searchRange * typeList.RangeMax
		end
	end
	
	-- print(index, "Ȩ�ؽ��:", wight)
	-- ����Ȩ�ؽ��
	return wight
end

-- ����ͬһ�ȼ���Ȩ���б�
function GetOneLevelWightList(oneTarget, typeLevel)
	-- ����Ȩ���б�
	local sumWight = 0
	local nowWigth = 0
	for kForTypeLevel, vForTypeLevel in ipairs(typeLevel.typeList) do
		
		nowWigth = GetWightWithName(oneTarget, kForTypeLevel, vForTypeLevel, typeLevel.SearchRange)
		if (nowWigth < 0)then
			sumWight = -1
			break;
		end
		sumWight = sumWight + nowWigth
	end
	
	--print("Ȩ���ܺ�:", sumWight)
	return {wight = sumWight, target = oneTarget}
end

-- ������������
-- from  ��1
-- to    ��2
-- return ��������
function GetDistance(from, to)
	local result = 0
	if from ~= nil and to ~= nil then
		local x = from.x - to.x
		local y = from.y - to.y
		result = math.sqrt(x*x + y*y)
	end
	return result
end

-- targetDataList - ����Ŀ���б�(��񻯵�����)
-- selectId - ѡ������ID
-- return - �������ݻ�ȡ��ѡ���б�
-- ���ݽṹ: ID, TargetSelectTypeId, ��Ӫ����ID,
-- type1(�յؽ���), type2(̹��,�ؾ�,����,������,����),
-- type3(����, ����, ���, ����), ����, ����, �Ƕ�
function SelectTarget(targetDataList, selectId)
	-- ����ID��ȡ�����������������
	local selectData = TargetSelectCacheData[selectId]
	-- ���鳤��
	local len = table.getn(targetDataList)
	-- �����б�
	local result = {}
	-- ���Ȩ��
	local maxWight = 0
	-- ���Ȩ�ص�index
	local maxWightIndex = 0
	-- ��������
	for i =1,len do
		-- ��ȡ��λ
		local oneTarget = targetDataList[i]
		--[[for k,v in pairs(targetDataList) do
			print(111,k,v)
		end--]]
		--print(oneTarget.Name, table.getn(targetDataList))
		-- ���㵥λ��ѡ��λ�ľ���
		oneTarget.Range = GetDistance(selectData.selecterPos, oneTarget.Position)
		-- ��Type��������ֹ���
		-- ��Ȩ��ֵ
		local wightData = GetOneLevelWightList(oneTarget, selectData)
		-- ��Ȩ�����Ӧ���ݷ���ͬһ���б�
		result[i] = wightData
		-- ��ȡȨ����������
		if maxWight < wightData.wight then
			maxWight = wightData.wight
			maxWightIndex = i
		end
	end
	
	-- �����ݰ���Ȩ��ֵ�Ӵ�С����
	table.sort(result, function (a,b)
		return a.wight > b.wight
	end)
	
	-- ������ɸѡ����
	if selectData.otherType ~= nil then
		
		-- �����Ҫ��Ŀ����Χ�뾶�ڵ�λ
		if  selectData.otherType.scatteringRadius ~= nill and
			selectData.otherType.scatteringRadius > 0 and
			maxWightIndex > 0 then
			
			-- ɢ������µ�ɸѡ�б�
			local newResultForScattering = {}
			-- Ȩ��ֵ������
			local maxWightItem = result[maxWightIndex]
			-- ���Ȩ��Ŀ��λ��
			local maxWightPos = maxWightItem.target.Position
			
			-- ���������
			local distance
			-- ����λ������ �����㲻�ܾ��뵱ǰ�������������뾶�ľ���
			distance = GetDistance(maxWightPos, selectData.selecterPos)
			-- print(distance)
			-- print("searchedPos", maxWightPos.x, maxWightPos.y)
			-- ������Ȩ��Ŀ��λ����������λ����С��ɢ��뾶, ��λ�������������ɢ��뾶��Ⱦ���
			if distance < selectData.otherType.scatteringRadius and distance > 0 then
				local x = maxWightPos.x - selectData.selecterPos.x
				local y = maxWightPos.y - selectData.selecterPos.y
				local lengthRatio = selectData.otherType.scatteringRadius / distance
				x = x * lengthRatio
				y = y * lengthRatio
				maxWightPos.x = x + selectData.selecterPos.x
				maxWightPos.y = y + selectData.selecterPos.y
			end
			-- TODO ���㾫׼��
			-- print("searchPos", maxWightPos.x, maxWightPos.y)
			
			-- ������Ȩ��ֵ��Χ�ĵ�λ(����)
			for i = 1, len do
				-- ��ȡ��λ
				local item = result[i]
				
				-- ���㵥λ�Ƿ������Ȩ��Ŀ��ķ�Χ��
				-- print(GetDistance(maxWightPos, item.target.Position))
				distance = GetDistance(maxWightPos, item.target.Position)
				if distance <= selectData.otherType.scatteringRadius then
					table.insert(newResultForScattering, item)
				end
			end
			-- �滻������
			result = newResultForScattering
		end
		
		-- ��ȡ�����ָ�������Ķ���
		if selectData.otherType.targetCount ~= nill and selectData.otherType.targetCount > 0 then
			
			local maxCount = selectData.otherType.targetCount
			-- �������������ڵ�ǰ��ѡ������ ����Ҫ�ָ��б�ֱ�ӷ����б�
			if maxCount <= len then
				
				-- �·����б�
				local newResultForMaxCount = {}
				-- �ָ��б�ֻ����Ŀ����������λ
				for i = 1, selectData.otherType.targetCount do
					
					newResultForMaxCount[i] = result[i]
				end
				result = newResultForMaxCount
			end
		end
	end

	-- TODO ����ת��Ϊ����stirng
	return json.encode(result)
end


function SearchTargetWithJson(jsonData, selectId)
	if(jsonData == nil) then
		return nil
	end
	
	--print(jsonData.jsonData)
	local data = json.decode(jsonData.jsonData)
	return SelectTarget(data.data, selectId)
end


-- TODO ------------------��̬����, �����ȴ������, ֮�������ݱ�������--------------
TargetSelectCacheData = {
[10000] = {
id = 10000,
typeList = {
[1] = {
Surface = 100,
Air = 0,
Build = 100
},
[2] = {
Tank = 10,
LV = 10,
Cannon = 10,
Aircraft = 10,
Soldier = 10
},
[3] = {
Hide = -1,
HideZd = -1,
Taunt = 10000
},
[4] = {
RangeMin = 10,
RangeMax = 10,
HealthMin = 0,
HealthMax = 10,
AngleMin = 10
}
},
otherType = {
-- ��׼��
accuracy = 0.6,
-- ɢ��뾶
scatteringRadius = 100,
},

-- ������λ��λ��
selecterPos = {x = 1, y = 1},
SearchRange = 100
},
[10001] = {
id = 10000,
typeList = {
[1] = {
Surface = 100,
Air = 1,
Build = 100
},
[2] = {
Tank = 10,
LV = 10,
Cannon = 10,
Aircraft = 10,
Soldier = 10
},
[3] = {
Hide = 1,
HideZd = 1,
Taunt = 10000
},
[4] = {
RangeMin = 10,
RangeMax = 10,
HealthMin = 0,
HealthMax = 10,
AngleMin = 10
}
},
otherType = {
-- Ŀ������
targetCount = 10
},

-- ������λ��λ��
selecterPos = {x = 1, y = 1},
SearchRange = 100
}
}

-- ---------------------------��������---------------------------
local onetarget = {
Surface = 1,
Air = 0,
Build = 0,
Tank = 0,
LV = 0,
Cannon = 1,
Aircraft = 0,
Soldier = 0,
Hide = 0,
HideZd = 0,
Taunt = 0,
Health = 1,
HealthMax = 100,
Angle = 0,
Position = {x = 0, y = 0}
}

local onetarget2 = {
Surface = 1,
Air = 0,
Build = 0,
Tank = 0,
LV = 0,
Cannon = 1,
Aircraft = 0,
Soldier = 0,
Hide = 0,
HideZd = 0,
Taunt = 0,
Health = 1,
HealthMax = 100,
Angle = 0,
Position = {x = 0, y = 10}
}

local onetarget3 = {
Surface = 1,
Air = 0,
Build = 0,
Tank = 0,
LV = 0,
Cannon = 1,
Aircraft = 0,
Soldier = 0,
Hide = 0,
HideZd = 0,
Taunt = 0,
Health = 10,
HealthMax = 100,
Angle = 0,
Position = {x = 10, y = 10}
}

testData = {}
--GetOneLevelWightList(onetarget, TargetSelectCacheData[10000])

--local targetSelecter = TargetSelecter:Instance()
--local result = SelectTarget({onetarget, onetarget2, onetarget3}, 10001)
local jsonData = {jsonData = "{'data' : [{'Name' : 1,'Surface' : 0,'Air' : 1,'Build' : 0,'Tank' : 0,'LV' : 1,'Cannon' : 0,'Aircraft' : 0,'Soldier' : 0,'HealthMax' : 10,'Health' : 2,'Angle' : 13.06557,'Position' : { 'x' : 70.07961,'y' : 63.13879}},{'Name' : 2,'Surface' : 0,'Air' : 1,'Build' : 0,'Tank' : 0,'LV' : 0,'Cannon' : 1,'Aircraft' : 0,'Soldier' : 0,'HealthMax' : 10,'Health' : 3,'Angle' : 6.497635,'Position' : { 'x' : 39.12148,'y' : 63.10413}},{'Name' : 3,'Surface' : 0,'Air' : 1,'Build' : 0,'Tank' : 0,'LV' : 1,'Cannon' : 0,'Aircraft' : 0,'Soldier' : 0,'HealthMax' : 10,'Health' : 4,'Angle' : 40.75872,'Position' : { 'x' : 82.15903,'y' : 91.01767}},{'Name' : 4,'Surface' : 0,'Air' : 1,'Build' : 0,'Tank' : 0,'LV' : 0,'Cannon' : 0,'Aircraft' : 1,'Soldier' : 0,'HealthMax' : 10,'Health' : 5,'Angle' : 19.13668,'Position' : { 'x' : 78.14133,'y' : 43.07502}},{'Name' : 6,'Surface' : 0,'Air' : 1,'Build' : 0,'Tank' : 1,'LV' : 0,'Cannon' : 0,'Aircraft' : 0,'Soldier' : 0,'HealthMax' : 10,'Health' : 2,'Angle' : 13.15622,'Position' : { 'x' : 57.07938,'y' : 95.13892}},{'Name' : 7,'Surface' : 0,'Air' : 1,'Build' : 0,'Tank' : 0,'LV' : 0,'Cannon' : 0,'Aircraft' : 1,'Soldier' : 0,'HealthMax' : 10,'Health' : 3,'Angle' : 35.13593,'Position' : { 'x' : 29.02162,'y' : 83.15854}},{'Name' : 8,'Surface' : 0,'Air' : 1,'Build' : 0,'Tank' : 0,'LV' : 0,'Cannon' : 0,'Aircraft' : 1,'Soldier' : 0,'HealthMax' : 10,'Health' : 4,'Angle' : 40.75872,'Position' : { 'x' : 46.15902,'y' : 94.01767}},{'Name' : 9,'Surface' : 0,'Air' : 1,'Build' : 0,'Tank' : 0,'LV' : 0,'Cannon' : 1,'Aircraft' : 0,'Soldier' : 0,'HealthMax' : 10,'Health' : 5,'Angle' : 38.02183,'Position' : { 'x' : 4.013609,'y' : 70.15942}}]}"}
local result = SearchTargetWithJson(jsonData, 10000)
--local decodeData = json.decode(jsonData)
print (result)
--local encodeData = json.encode(result)
--print (encodeData)
--[[for k,v in ipairs(result) do
	print(k,v.wight)
end--]]


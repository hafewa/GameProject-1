--region *.lua
--Date
--���ļ���[BabeLua]����Զ�����

local wnd_HeroPerviewClass = class(wnd_base)
    wnd_HeroPerview = nil


function wnd_HeroPerviewClass:Start()
	wnd_HeroPerview = self
	self:Init(WND.HeroPerview)
end

function wnd_HeroPerviewClass:OnNewInstance()
	self.bIsShowStar = false 
    self:BindUIEvent("bg_fuzzy",UIEventType.Click,"OnClose")

    self:FindWidget("btn_earn/txt"):GetComponent(CMUILabel.Name):SetValue(SData_Id2String.Get(5193))
    self:FindWidget("hero_bg/title/txt"):GetComponent(CMUILabel.Name):SetValue(SData_Id2String.Get(5192))
    self:FindWidget("skill_info_widget/title"):GetComponent(CMUILabel.Name):SetValue(SData_Id2String.Get(5014))
    self:FindWidget("soldier_info_widget/title"):GetComponent(CMUILabel.Name):SetValue(SData_Id2String.Get(5015))
end
function wnd_HeroPerviewClass:SetHero(id)
	self.ID = id
	--self.ID = 1007
end

function wnd_HeroPerviewClass:OnShowDone()

    --������������Ԥ��������ʾ����Ϣ�Լ���ȡ;��
	self:SetWidgetActive("info_widget/bg",false)
	self:SetWidgetActive("hero_piece_widget",false)

    local MaHeroInfoList = SData_Hero.GetHero(self.ID)
	self:SetLabel("basic_info_widget/hero_name",MaHeroInfoList:Name())
	local staNum = MaHeroInfoList:MorenXing()

	--�佫������Ӫͼ��
	local typeicon = self.instance:FindWidget("hero_type")
	local typeiconUI= typeicon:GetComponent(CMUISprite.Name)
	typeiconUI:SetSpriteName("t"..MaHeroInfoList:TypeIcon())

	--�Ǽ�
	self:BuildStar(staNum)
	self.bIsShowStar = true 
	--����
	self.skillTable = MaHeroInfoList:Skills()
	local Skills = SData_Skill.GetSkill(self.skillTable[1])
	local skillName = Skills:Name()
	local skillIcon = Skills:Icon()
	self:SetLabel("skillname_bg/txt",skillName)
	local sprite = self.instance:FindWidget( "skill1/skill_con")
	local jnTB = sprite:GetComponent(CMUISprite.Name)
	jnTB:SetSpriteName( "skillicon"..skillIcon)
	--ʿ��
	local Army = SData_Army.GetRow(MaHeroInfoList:Army())
	self:SetLabel("soldier_info_widget/soldier_name",Army:Name())
	--�佫ͷ��
	local m_item = self.instance:FindWidget("head_widget/piece_icon")
	local cmHead = m_item:GetComponent(CMUISprite.Name)
	MaHeroInfoList:SetHeroIcon(cmHead)
	--�����ռ�
	local PlayerJH = Player:GetJianghunNum(self.ID)
	local pro = self.instance:FindWidget( "piece_process_bg" )
	local pro_pro = pro:GetComponent(CMUIProgressBar.Name)
	pro_pro:SetValue(PlayerJH/sdata_keyvalue:GetFieldV(staNum.."xingSuipian",1))	--���ӵ���佫�Ľ�����
	self:SetWidgetActive("piece_process_bg/num",true)
	local MaxPieces = sdata_keyvalue:GetFieldV(staNum.."xingSuipian",1)
	self:SetLabel("piece_process_bg/num",string.sformat(SData_Id2String.Get(5005),PlayerJH,MaxPieces))

	--������
	local Banshen = self.instance:FindWidget( "hero_img" )
	local HeroBanshen = Banshen:GetComponent(CMUIHeroBanshen.Name)
	HeroBanshen:SetIcon(self.ID,false)
	--��ȡ
	self:BindUIEvent("btn_earn",UIEventType.Click,"showHuoqu")
end
--
function wnd_HeroPerviewClass:showHuoqu()
	wnd_gainmethod:ShowData(2)
	wnd_gainmethod:Show()
end
function wnd_HeroPerviewClass:BuildStar(Num)
	local m_item = self.instance:FindWidget("stargrid/star")	
	if self.bIsShowStar == false  then
		for k = 1, 8 do
			local newItem = GameObject.InstantiateFromPreobj(m_item,self.instance:FindWidget("stargrid"))
			newItem:SetName("sta"..k)
		end
	end
	for k = 1 ,8 do
		local obj = "stargrid/sta"..k
		self:SetWidgetActive(obj,false)
		if k == Num or k < Num then
			self:SetWidgetActive(obj,true)
		end
	end
	local container = self.instance:FindWidget("stargrid")
	local cmTable = container:GetComponent(CMUIGrid.Name)
	cmTable:Reposition()
end
function wnd_HeroPerviewClass:ShowHero()

end
--
function wnd_HeroPerviewClass:OnClose()
    self:Hide()
end

function wnd_HeroPerviewClass:OnLostInstance() 
end
 return wnd_HeroPerviewClass.new

--endregion

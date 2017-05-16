local class = require("common/middleclass")
wnd_cardyc = class("wnd_cardyc", wnd_base)

--插槽装备状态
EquipState = {
    Enable_NotEnough= 0,  --未激活 材料不足
    Active = 1,           --已激活
    Enable_Enough = 2,    --未激活 有材料
}

--协同状态
synergyState = {
    unactive = 0,--未激活
    canActive = 1,--可激活
    activated = 2,--已解锁
}
local maxStarLv = 7
local maxQualityLv = 16
local maxUserRoleLv = 80
local maxSkillLv = 16
local maxArmyLv  = 8
local maxArmyUnionLv = 17
--local maxQualityLv = tonumber(#sdata_ArmyCardQualityShow_data.mData.body)
--local maxArmyLv  = #sdata_ArmyCardUseLimitCost_data.mData.body

--显示主面板
function wnd_cardyc:OnShowDone()
	print("wnd_cardyc:OnShowDone.......")
   	self:initUI()
end

function wnd_cardyc:initUI()
	print("wnd_cardyc:initUI.......")
    self.bgFramePanel = self.transform:Find("bgframe"):GetComponent("UIWidget")
    local btn_back = self.transform:Find("bgframe/Btn_back").gameObject
    UIEventListener.Get(btn_back).onPress = function(btn_back, args)
        if args then
            print("back.......")--返回上一界面
        end
    end

    self:getDatas()--服务器数据
    --
    local leftPanel = self.transform:Find("left"):GetComponent("UIWidget")
    local rightPanel = self.transform:Find("right"):GetComponent("UIWidget")   
    local btnTabPanel = self.transform:Find("Btn_tab"):GetComponent("UIWidget")      
    self.rightPanel = rightPanel

    --详情
    local btn_detail = leftPanel.transform:Find("card/Btn_details").gameObject
    UIEventListener.Get(btn_detail).onPress = function(btn_detail, args)
        if args then
            print("card btn_detail ....")
        end
    end

    --canUpStar redhint
    local btn_upStar = leftPanel.transform:Find("Btn_upStar").gameObject
    self.isInitUpStarLayer = false
   	UIEventListener.Get(btn_upStar).onPress = function(btn_upStar, args)
        if args then
            self:upStarCallBack()
        end
    end

    --canUpLevel redhint
    local btn_upLevel = leftPanel.transform:Find("Btn_upLevel").gameObject
    self.isInitUpLvLayer = false
    UIEventListener.Get(btn_upLevel).onPress = function(btn_upLevel, args)
        if args then
            self:upLevelCallBack()
        end
    end
    self.leftPanel = leftPanel

    --btn_left
    local btn_left = leftPanel.transform:Find("Btn_left").gameObject
    UIEventListener.Get(btn_left).onPress = function(btn_left, args)
        if args then
            self:leftCallBack()
        end
    end
    --btn_right
    local btn_right = leftPanel.transform:Find("Btn_right").gameObject
    UIEventListener.Get(btn_right).onPress = function(btn_right, args)
        if args then
            self:rightCallBack()
        end
    end

    self.btn_information = btnTabPanel.transform:Find("Btn_information").gameObject
    self.btn_skill = btnTabPanel.transform:Find("Btn_skill").gameObject
    self.btn_soldier = btnTabPanel.transform:Find("Btn_soldier").gameObject
    self.btn_synergy = btnTabPanel.transform:Find("Btn_synergy").gameObject

    self.informationPanel = self.rightPanel.transform:Find("information").gameObject
    self.skillPanel = self.rightPanel.transform:Find("skill").gameObject
    self.soldierPanel = self.rightPanel.transform:Find("soldier").gameObject
    self.synergyPanel = self.rightPanel.transform:Find("synergy").gameObject

    self.children = {self.btn_information,self.btn_skill,self.btn_soldier,self.btn_synergy}
    self.bodyNodes = {self.informationPanel,self.skillPanel,self.soldierPanel,self.synergyPanel}   
    for i=1,#self.children do
        local tabLab =  self.children[i].transform:Find("lab"):GetComponent("UILabel")
        tabLab.text = sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20019+i) 
    end

    self.isSlotInit = false--mark
    self.isfiveSIinit = false
    self.isAttrItemInit = false
    self.isInitAcFrame = false
    self.isInitxtLayer = false
    self.tabTable = {{self.btn_information, self.informationPanel},{self.btn_skill, self.skillPanel},{self.btn_soldier, self.soldierPanel},{self.btn_synergy, self.synergyPanel}}
    self:tabControl(self.tabTable,1,1)    
   
    --初始化刷新卡牌和信息内容
    self:refreshLeftCard()
    self:infBody()
end

function wnd_cardyc:getDatas()
    print("================wnd_cardyc:getDatas=======================")
    if currencyTbl ~= nil and cardTbl ~= nil and userRoleTbl~=nil  then
    --if nil then
        self.coinNum = currencyTbl["gold"] --金币
        self.diamond = currencyTbl["diamond"] --钻石
        self.totalSkPt = currencyTbl["skillpt"] --技能点
        self.expPool = currencyTbl["expPool"]--经验池
        self.badgeNum = currencyTbl["coin"] --兵牌
        self.tili = currencyTbl["tili"] --兵牌

        print("cardTbl:"..#cardTbl)
        for k,v in pairs(cardTbl) do
            self.cardId = v.id
            self.exp  = v.exp
            self.starLv = v.star
            self.soldierLv = v.slv
            self.qualityLv = v.rlv
            self.cardFragment = v.num
            self.slot = v.slot
            self.skillTbl = v.skill
            self.teamTbl = v.team--协同表
        end
        self.useRoleExp = userRoleTbl["exp"]
         --背包
        self.itemList = userRoleTbl["item"]  
    end
    --[[
    print("currencyTbl:"..#currencyTbl)
    print("cardTbl:"..#cardTbl)
    print("userRoleTbl:"..#userRoleTbl)
    --]]
    self.slotState = self.slot
    print( string.format("id:%d, exp:%d, star:%d, slv:%d, rlv:%d, num:%d", self.cardId, self.exp, self.starLv, self.soldierLv, self.qualityLv, self.cardFragment) )
    self.userRoleLv =  tonumber( sdata_UserRose_data:a(sdata_UserRose_data.I_UserExp,self.useRoleExp) )
    self.cardlv =  tonumber( sdata_ArmyCardExp_data:a(sdata_ArmyCardExp_data.I_CardExp,self.exp) )--卡牌等级
    self.cardExp = self.exp - tonumber(sdata_ArmyCardExp_data:GetFieldV("CardExp", self.cardlv))--exp_1
    print("self.cardExp:" .. self.cardExp)
    print( string.format("roleLv:%d, cardlv:%d",self.userRoleLv, self.cardlv) )
    self.nextStarlv = self:GetNextLv(self.starLv, maxStarLv)
    self.nextQualityLv = self:GetNextLv(self.qualityLv,maxQualityLv)

    --背包
    self.itemList = {}
    --[[
    self.itemList = {
        {itemId = 1, num = 10},--
        {itemId = 2, num = 2},--
        {itemId = 4, num = 100},--
        {itemId = 10001, num = 2},--
    }
    --]]
    print("================wnd_cardyc:getDatas=======================")
end

function wnd_cardyc:leftCallBack()
    print("btn_left..")
end

function wnd_cardyc:rightCallBack()
     print("btn_right..")
end

function wnd_cardyc:refreshLeftCard()
    --card
    local cardPanel = self.leftPanel.transform:Find("card"):GetComponent("UIWidget")
    local cardImgSp = cardPanel.transform:Find("cardImg_Sp"):GetComponent("UISprite") --卡牌图
    local cardNameLab = cardPanel.transform:Find("cardName_Lab"):GetComponent("UILabel") --卡牌名+品质
    local cardLevelLab = cardPanel.transform:Find("cardLevel_Lab"):GetComponent("UILabel") --卡牌等级
    local trainCostLab = cardPanel.transform:Find("costBgSp/trainCost_Lab"):GetComponent("UILabel") --卡牌费用
    local cardStarsPanel = cardPanel.transform:Find("Stars_widget"):GetComponent("UIWidget") --星星panel

    cardNameLab.text = self:GetName(self.cardId, self.qualityLv)
    cardNameLab.color = self:getColor(self.qualityLv)
    cardLevelLab.text = string.format("Lv." .. self.cardlv)
    trainCostLab.text = sdata_armycardbase_data:GetFieldV("TrainCost",self.cardId)
    --星星
    for i=1,maxStarLv do
       local starDark = cardStarsPanel.transform:Find("darkstar_"..i).gameObject
       local star = cardStarsPanel.transform:Find("star_"..i).gameObject
       starDark:SetActive(true)
       star:SetActive(true)
       if i>self.starLv then
           star:SetActive(false)
       end
    end
end

function wnd_cardyc:refreshRightBody()
    print("refreshRightBody,,tabIndex:"..self.index)
    if self.index == 1 then
        self:infBody()
    elseif self.index == 2 then
        self:skillBody()
    elseif self.index == 3 then
        self:sodilerBody()
    elseif self.index == 4 then
        self:synergyBody()
    end
end

--升星
function wnd_cardyc:canUpStar()
    --星级；卡牌碎片-兵牌数量
    local needFragment =  sdata_ArmyCardStarCost_data:GetFieldV("CardNum", self.nextStarlv)
    local needCoin =  sdata_ArmyCardStarCost_data:GetFieldV("Coin", self.nextStarlv) 
    if self.starLv<maxStarLv and needFragment<=self.cardFragment and needCoin <=self.badgeNum  then
        --redPointHint
        return true
    end
    return false
end

function wnd_cardyc:upStarCallBack()
     print("card upStar....")
     if self.starLv >= maxStarLv then
        print("卡牌已满星..")
        return
     end
     self:showUpStarLayer()
end

function wnd_cardyc:showUpStarLayer()
    --self.isInitSUpLayer = false
     if not self.isInitUpStarLayer then
        self.upStarPanel = GameObjectExtension.InstantiateFromPacket("ui_cardyc", "upStarPanel",  self.gameObject)
        self.upStarPanel.name = "upStarPanel"
        local name1 = "cardBeforePanel"
        self:createACFrame(self.upStarPanel.gameObject,name1,Vector3(-262,0,0),20,self.cardId,self.cardlv,self.armyCardLv,self.starLv)
        local name2 = "cardAfterPanel"
        self:createACFrame(self.upStarPanel.gameObject,name2,Vector3(-7,0,0),20,self.cardId,self.cardlv,self.armyCardLv,self.nextStarlv)
        self.upStarPanel:SetActive(false)
        self.isInitUpStarLayer = true
        self.isInitUpStarSLayer = false
     end

    self:refreshAcframe(self.upStarPanel,"cardBeforePanel",self.cardId,self.cardlv,self.qualityLv,self.starLv)
    self:refreshAcframe(self.upStarPanel,"cardAfterPanel",self.cardId,self.cardlv,self.qualityLv,self.nextStarlv)

    local panel = self.upStarPanel.transform:Find("skillPanel").gameObject
    local skillNameLab  = panel.transform:Find("skillNameLab"):GetComponent("UILabel")
    panel:SetActive(false)
    if self.nextStarlv <= 5  and self.nextStarlv > 3 then
        panel:SetActive(true)
        local uid = tonumber(string.format("%d%.3d",self.cardId,self.cardlv))--通过卡牌id和卡牌等级联合获取
        print("x:" .. uid)
        local skillid = sdata_ArmyBase_data:GetFieldV("Skill"..self.nextStarlv, uid)
        print("skillid:" ..skillid)--通过skillid获取skillname --@1
        skillNameLab.text = sdata_Skill_data:GetFieldV("Name",tonumber(skillid)) --解锁技能名
        local skillName_btn = panel.transform:Find("skillNameLab").gameObject
        UIEventListener.Get(skillName_btn).onPress = function(skillName_btn, args)
            if args then
                print("skillid"..skillid ) --弹出技能介绍框
            end
        end
    end

    local costNameLab_1 = self.upStarPanel.transform:Find("costNameLab_1"):GetComponent("UILabel")
    local costSp_1 = self.upStarPanel.transform:Find("costSp_1"):GetComponent("UISprite")
    local numLab_1 = self.upStarPanel.transform:Find("numLab_1"):GetComponent("UILabel")
    local costNameLab_2 = self.upStarPanel.transform:Find("costNameLab_2"):GetComponent("UILabel")
    local costSp_2 = self.upStarPanel.transform:Find("costSp_2"):GetComponent("UISprite")
    local numLab_2 = self.upStarPanel.transform:Find("numLab_2"):GetComponent("UILabel")
    local neednumLab_1 = self.upStarPanel.transform:Find("neednumLab_1"):GetComponent("UILabel")
    local neednumLab_2 = self.upStarPanel.transform:Find("neednumLab_2"):GetComponent("UILabel")
    costNameLab_1.text = sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20027)
    costNameLab_2.text = sdata_armycardbase_data:GetFieldV("Name", self.cardId)
    
    --兵牌
    local needCoin =  sdata_ArmyCardStarCost_data:GetFieldV("Coin", self.nextStarlv)  
    neednumLab_1.text = string.format("X%d",needCoin)
    --local lab1 = string.format("X%d(%s%d)",needCoin,sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20028),self.badgeNum)
    local lab1 = string.format("([97ff03]%s%d[-])",sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20028),self.badgeNum)
    if self.badgeNum < needCoin then
        --lab1 = string.format("X%d(%s[FF0000]%d[-])",needCoin,sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20028),self.badgeNum) --300  [FF0000 ]num[-]
        lab1 = string.format("(%s[ff2121]%d[-])",sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20028),self.badgeNum) 
    end
    numLab_1.text = lab1
    --卡碎片
    local needFragment =  sdata_ArmyCardStarCost_data:GetFieldV("CardNum", self.nextStarlv) 
    neednumLab_2.text = string.format("X%d",needFragment)
    local lab2 = string.format("([97ff03]%s%d[-])",sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20028),self.cardFragment)
    if self.cardFragment < needFragment then
        lab2 = string.format("(%s[ff2121]%d[-])",sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20028),self.cardFragment)
    end
    numLab_2.text = lab2

    --Btn_backSp
    local btn_back = self.upStarPanel.transform:Find("Btn_backSp").gameObject 
    UIEventListener.Get(btn_back).onPress = function(btn_back, args)
        if args then
            self.upStarPanel:SetActive(false)
        end
    end

    local btn_sx = self.upStarPanel.transform:Find("Btn_sx").gameObject 
    UIEventListener.Get(btn_sx).onPress = function(btn_sx, args)
        if args then
            self:sxCallBack()
        end
    end
    self.upStarPanel:SetActive(true)
end

function wnd_cardyc:sxCallBack()
    print("btn_sx callBack...")
    --是否达到最大星级
    if self.starLv >= maxStarLv then
        print("卡牌已达最大星级")
        return
    end
    --所需碎片是否足够
    local needFragment =  sdata_ArmyCardStarCost_data:GetFieldV("CardNum", self.nextStarlv) 
    if needFragment > self.cardFragment then
        print("卡牌升星所需碎片不足")
        return
    end
    --所需兵牌是否足够
    local needCoin =  sdata_ArmyCardStarCost_data:GetFieldV("Coin", self.nextStarlv) 
    if needCoin > self.badgeNum then
        print("卡牌升星所需兵牌不足")
        return
    end

    --发送请求升星
    --提升星级扣道具，升星成功
    SendPB_10010(1001)
    self.cardFragment = self.cardFragment - needFragment
    self.badgeNum = self.badgeNum - needCoin
    self.upStarPanel:SetActive(false)
    self:showUpStarSus()
    self.starLv = self.nextStarlv
    self.nextStarlv = self:GetNextLv(self.starLv, maxStarLv)
    --refreshLeft + refreshRight
    self:refreshLeftCard()
    self:refreshRightBody()
end

function wnd_cardyc:showUpStarSus()
    if not self.isInitUpStarSLayer then
        self.upStarSuccessPanel = GameObjectExtension.InstantiateFromPacket("ui_cardyc", "upStarSuccess",  self.gameObject)
        self.upStarSuccessPanel.name = "upStarSuccess"
        local name1 = "cardBeforePanel"
        self:createACFrame(self.upStarSuccessPanel.gameObject,name1,Vector3(-190,55,0),20,self.cardId,self.cardlv,self.armyCardLv,self.starLv)
        local name2 = "cardAfterPanel"
        self:createACFrame(self.upStarSuccessPanel.gameObject,name2,Vector3(190,55,0),20,self.cardId,self.cardlv,self.armyCardLv,self.nextStarlv)
        self.upStarSuccessPanel:SetActive(false)
        self.isInitUpStarSLayer = true
    end
    self.upStarSuccessPanel:SetActive(true) 
    self:refreshAcframe(self.upStarSuccessPanel,"cardBeforePanel",self.cardId,self.cardlv,self.qualityLv,self.starLv)
    self:refreshAcframe(self.upStarSuccessPanel,"cardAfterPanel",self.cardId,self.cardlv,self.qualityLv,self.nextStarlv)

    for i=1,3 do
        --生命 火力 防御成长
        local des = self.upStarSuccessPanel.transform:Find("desLab_"..i):GetComponent("UILabel")
        des.text = sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20037+i-1)
        local beforeLab = self.upStarSuccessPanel.transform:Find("Label_a"..i):GetComponent("UILabel")
        local afterLab = self.upStarSuccessPanel.transform:Find("Label_b"..i):GetComponent("UILabel")
        self.cardId = 1001
        --upsSp_1
        local uida = tonumber(string.format("%d%.2d",self.cardId,self.starLv))
        print("x:" .. uida)
        local attacka = sdata_ArmyCardStar_data:GetFieldV("CardStarHP", uida)
        print("attacka:" ..attacka) --@1

        local uidb = tonumber(string.format("%d%.2d",self.cardId,self.nextStarlv))
        print("y:" .. uidb)
        local attackb = sdata_ArmyCardStar_data:GetFieldV("CardStarHP", uidb)
        print("attackb:" ..attackb) --@1
        --CardStarAttack","CardStarHP","CardStarDefense"
        if i==2 then
            attacka = sdata_ArmyCardStar_data:GetFieldV("CardStarAttack", uida)
            attackb = sdata_ArmyCardStar_data:GetFieldV("CardStarAttack", uidb)
        elseif i==3 then
            attacka = sdata_ArmyCardStar_data:GetFieldV("CardStarDefense", uida)
            attackb = sdata_ArmyCardStar_data:GetFieldV("CardStarDefense", uidb)
        end
        beforeLab.text = attacka
        afterLab.text = attackb
    end
    
    local btn_back = self.upStarSuccessPanel.transform:Find("clickPanel").gameObject 
    UIEventListener.Get(btn_back).onPress = function(btn_back, args)
        if args then
            self.upStarSuccessPanel:SetActive(false) 
        end
    end
end

--升级
function wnd_cardyc:canUpLevel()
    --a.  卡牌当前等级提升所需经验≤经验池中的经验
    --b.  卡牌等级<角色等级
    local needExp = sdata_ArmyCardExp_data:GetFieldV("CardExp",self:GetNextLv(self.cardlv,maxUserRoleLv)) --当前等级提升所需经验
    print("needExp:".. needExp)
    if needExp <= self.expPool then 
        if self.cardlv < self.userRoleLv then
            --redPointHint
            return true
        end
    end
    return false
end

function wnd_cardyc:upLevelCallBack()
    print("card upLevelCallBack....")
    if self.cardlv >= self.userRoleLv then
       print("请先提升角色等级..")
       return
    end

    if not self.isInitUpLvLayer then
        self.upLevelPanel = GameObjectExtension.InstantiateFromPacket("ui_cardyc", "upLevelPanel",  self.gameObject)
        self.upLevelPanel.name = "upLevelPanel"
        local name1 = "cardPanel"
        self:createACFrame(self.upLevelPanel.gameObject,name1,Vector3(-186,48,0),20,self.cardId,self.cardlv,self.armyCardLv,self.starLv)
        self.upLevelPanel:SetActive(false)
        self.isInitUpLvLayer = true
    end
    self:refreshUpLevelBody()
    self.upLevelPanel:SetActive(true)
    --jmxs pb
    local btn_upLevelOne = self.upLevelPanel.transform:Find("Btn_upLevelOne").gameObject
    UIEventListener.Get(btn_upLevelOne).onPress = function(btn_upLevelOne, args)
        if args then
            self:upLevelOneCallBack()
        end
    end

    local btn_upLevelTen = self.upLevelPanel.transform:Find("Btn_upLevelTen").gameObject
    UIEventListener.Get(btn_upLevelTen).onPress = function(btn_upLevelTen, args)
        if args then
            self:upLevelTenCallBack()
        end
    end

    local btn_upLevelBack = self.upLevelPanel.transform:Find("Btn_backSp").gameObject
    UIEventListener.Get(btn_upLevelBack).onPress = function(btn_upLevelBack, args)
        if args then
            print("back....")
            self.upLevelPanel:SetActive(false)
        end
    end   
end

--升级界面
function wnd_cardyc:refreshUpLevelBody()
    print(" wnd_cardyc:refreshUpLevelBody ")
   -- self.expPool = currencyTbl["expPool"]--经验池
    for k,v in pairs(cardTbl) do
        self.exp  = v.exp
    end
    print("exp:" .. self.exp)
    self.cardlv =  tonumber( sdata_ArmyCardExp_data:a(sdata_ArmyCardExp_data.I_CardExp,self.exp) )--卡牌等级
    self.cardExp = self.exp - tonumber(sdata_ArmyCardExp_data:GetFieldV("CardExp", self.cardlv))--exp_1
    print("卡牌经验下限self.cardExp:" .. self.cardExp)
    print("可分配经验expPool:"..self.expPool)
    print("卡牌总exp:" .. self.exp)
    self:refreshAcframe(self.upLevelPanel,"cardPanel",self.cardId,self.cardlv,self.qualityLv,self.starLv)

    --等级
    local cardPanel = self.upLevelPanel.transform:Find("cardPanel"):GetComponent("UIWidget")
    local cardLevLab = cardPanel.transform:Find("bkSp/lv_Lab"):GetComponent("UILabel")
    cardLevLab.text = self.cardlv
    
    --exp progress
    local expProBar = self.upLevelPanel.transform:Find("expProgressBar_Sp")
    local expProLab = self.upLevelPanel.transform:Find("expProgressBar_Sp/expProLab"):GetComponent("UILabel")
    local needExp = sdata_ArmyCardExp_data:GetFieldV("CardExp",self:GetNextLv(self.cardlv, maxUserRoleLv))--升级下一级所需经验
    expProLab.text = string.format("%d/%d",self.cardExp,needExp)
    local uiSlide = expProBar:GetComponent("UISlider")
    print("value:".. self.cardExp/needExp)
    uiSlide.value = self.cardExp/needExp

    --可分配exp
    local cardLevLab = self.upLevelPanel.transform:Find("allotExp_Lab"):GetComponent("UILabel")
    cardLevLab.text = string.format("%s:%d",sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20036),self.expPool)
end

function wnd_cardyc:upLevelOneCallBack()
    print("card btn_upLevelOne ....")
    print("exp:"..self.exp)
    local needExp = sdata_ArmyCardExp_data:GetFieldV("CardExp",self:GetNextLv(self.cardlv,maxUserRoleLv))--升级下一级所需经验
    print("nextexp:".. needExp )
    print("cardexp:" .. self.cardExp)
    print("expPool:" .. self.expPool)
    print("userRoleLv:" .. self.userRoleLv)
    if self.cardlv >= self.userRoleLv then
        print("卡牌等级不能超过角色等级，请先提升角色等级")
        return
    end

    if self.expPool == 0 then
        print("可分配经验不足") 
        return
    end

    local upNum = 1--默认提升一级  等级提升 扣经验，刷新
    if self.expPool < (needExp-self.cardExp) then
        upNum = 0 --经验池经验转移至卡牌上
    end
    SendPB_10009(self.cardId, upNum)
    --返回成功
    self.expPool = self.expPool-(needExp-self.cardExp) --经验池
    self:refreshUpLevelBody()
    self:refreshLeftCard()
    self:refreshRightBody()
end

function wnd_cardyc:upLevelTenCallBack()
    print("card btn_upLevelTen ....")
    print("exp:"..self.exp)
    local needNow= sdata_ArmyCardExp_data:GetFieldV("CardExp",self.cardlv)--当前等级所需经验 
    local needExp = sdata_ArmyCardExp_data:GetFieldV("CardExp",self:GetNextLv(self.cardlv+10,maxUserRoleLv))--升级下10级所需经验
    print("nextexp:".. needExp )
    print("cardexp:" .. self.cardExp)
    print("expPool:" .. self.expPool)
    print("userRoleLv:" .. self.userRoleLv)
    if self.cardlv >= self.userRoleLv then
        print("卡牌等级不能超过角色等级，请先提升角色等级")
        return
    end

    if self.expPool == 0 then
        print("可分配经验不足") 
        return
    end

    local upNum = 10--默认提升一级  等级提升 扣经验，刷新
    if self.expPool < (needExp+self.cardExp-needNow) then
        local expzy = self.expPool + self.exp
        local lv  =  tonumber( sdata_ArmyCardExp_data:a(sdata_ArmyCardExp_data.I_CardExp,expzy) )--卡牌等级
        needExp =  tonumber(sdata_ArmyCardExp_data:GetFieldV("CardExp", lv)) -self.exp--exp_1
        upNum = self.cardlv - lv --经验池经验转移至卡牌上 --能提几级提几级
    else
        if self.cardlv+10 > self.userRoleLv then
            upNum = self.userRoleLv - self.cardlv
            needExp =  tonumber(sdata_ArmyCardExp_data:GetFieldV("CardExp", self.cardlv+upNum))-self.exp --exp_1
        end
    end
    SendPB_10009(self.cardId, upNum)
    --返回成功
    self.expPool = self.expPool-(needExp-self.cardExp) --经验池
    self:refreshUpLevelBody()
    self:refreshLeftCard()
    self:refreshRightBody()
end

--信息
function wnd_cardyc:infBody()
    -- refresh information body
    local go = self.informationPanel.transform
    local hpLab = go:Find("hp_Lab"):GetComponent("UILabel") --生命
    local msLab = go:Find("ms_Lab"):GetComponent("UILabel") --秒伤
    local powerLab = go:Find("power_Lab"):GetComponent("UILabel") --火力
    local teamLab = go:Find("team_Lab"):GetComponent("UILabel") --队伍armysum
    local defLab = go:Find("def_Lab"):GetComponent("UILabel") --防御
    local targetLab = go:Find("aimGeneralType_Lab"):GetComponent("UILabel") --目标
    local scLab = go:Find("sc_Lab"):GetComponent("UILabel") --射程
    local rangeLab = go:Find("range_Lab"):GetComponent("UILabel") --范围attacktype
    
    --基础信息refresh
    hpLab.text = 88

    --兵种克制
    local kzpanel = go:Find("armykzPanel"):GetComponent("UIWidget")
    local armyType =  kzpanel.transform:Find("armyTypeSp"):GetComponent("UISprite")--兵种图
    --armyTypeL =  kzpanel.transform:Find("desArmyTypeSp"):GetComponent("UISprite")
    --armyTypeR =  kzpanel.transform:Find("addArmyTypeSp"):GetComponent("UISprite")
  
    local btn_advanced = go:Find("advanced_Sp").gameObject
    local btn_equipAll = go:Find("equipAll_Sp").gameObject
    local advanceCostPanel = go:Find("advanceCost").gameObject
    --local costSp = advanceCostPanel.transform:Find("cost_Sp"):GetComponent("UISprite") --晋阶消耗资源图
    local costLab = advanceCostPanel.transform:Find("cost_Lab"):GetComponent("UILabel") --晋阶消耗金币数

    --军勋章
    ---[[
    --local armyMedal = go:Find("armyMedal"):GetComponent("UIWidget")
    self.itemIdTbl = {} --军功章晋升所需材料
    self.itemNeedNumTbl = {} 
    self.itemNumTbl = {0,0,0,0}--现有数量表

    --CardEquip1","Point1","ItemID1","Num1
    local uid = tonumber(string.format("%d%.2d",self.cardId,self.qualityLv))--通过卡牌id和军阶等级联合获取
    print("x:" .. uid)

    for k,v in pairs(cardTbl) do
        self.slotState = v.slot
    end
    for i=1,#self.slotState do
        local cardEquip = sdata_ArmyCardQuality_data:GetFieldV("CardEquip"..i,uid)
        local itemID = sdata_ArmyCardQuality_data:GetFieldV("ItemID"..i,uid)
        local num = sdata_ArmyCardQuality_data:GetFieldV("Num"..i,uid)
        print( string.format("cardEquip:%d, itemId:%d, num:%d",cardEquip, itemID,num))
        self.itemIdTbl[i] = itemID
        self.itemNeedNumTbl[i] = num

        for _, v in ipairs(self.itemList) do
            if v.itemId == self.itemIdTbl[i] then
                self.itemNumTbl[i] = v.num
                break
            end
        end

        print("neednum：".. self.itemNeedNumTbl[i])
        print("itemNumTbl".. self.itemNumTbl[i])
        if self.slotState[i] == EquipState.Enable_NotEnough and (self.itemNeedNumTbl[i] <= self.itemNumTbl[i]) then
            self.slotState[i] = EquipState.Enable_Enough
        end
    end

    print("slotN:"..#self.slotState)
    for k,v in pairs(self.slotState) do
        print(k,v)
    end

    self.isInitMedalItemLayer = false
    self.initGainLayer  = false
    for i=1,#self.slotState do
        if not self.isSlotInit then
           local name = string.format("armyMedalP_%d",i)
           local v3 = Vector3(-150+(i-1)*100, -156 ,0)
           self:createAMFrame(self.informationPanel,name,v3,self.itemIdTbl[i],self.slotState[i])
        end
    end
    self.isSlotInit = true
    print("军阶插槽所需材料id表: itemIdTbl...")
    for k,v in pairs(self.itemIdTbl) do
        print(k,v)
    end

    for i=1,#self.itemIdTbl do
        local armyMedalPanel  =  go:Find("armyMedalP_"..i):GetComponent("UIWidget").transform
        local btn_armyMedal = armyMedalPanel:Find("itemSp").gameObject
        UIEventListener.Get(btn_armyMedal).onPress = function(btn_armyMedal, args)
            if args then
                self.slotIndex = i
                print("self.slotIndex:"..self.slotIndex)
                self:showArmyItem(self.slotIndex)
            end
        end
    end

    local isEquipAll = false
    for i,v in ipairs(self.slotState) do
        if v == EquipState.Active then
            isEquipAll = true
        else
            isEquipAll = false
            break
        end
    end
    if isEquipAll then
        --插槽全部装备可进阶
        btn_equipAll:SetActive(false)

        btn_advanced:SetActive(true)
        advanceCostPanel:SetActive(true)
        costLab.text = sdata_ArmyCardQualityCost_data:GetV(sdata_ArmyCardQualityCost_data.I_Gold, self.nextQualityLv) --晋升所需金币
    else
       btn_advanced:SetActive(false)
       advanceCostPanel:SetActive(false)
       btn_equipAll:SetActive(true)
    end
    self.isEquipAll = isEquipAll

    --晋阶
    self.isInitAmSLayer = false
    UIEventListener.Get(btn_advanced).onPress = function(btn_advanced, args)
        if args then
            self:upArmyMedal()
        end
    end

    --一键装备
    UIEventListener.Get(btn_equipAll).onPress = function(btn_equipAll, args)
        if args then
            self:equipAllCallBack()
        end
    end
end

--晋阶
function wnd_cardyc:upArmyMedal()
   print("card btn_advanced....")
   local limitLv = sdata_ArmyCardQualityShow_data:GetFieldV("CardLevel", self.nextQualityLv)
   if self.cardlv < limitLv then
        print(string.format("晋阶需要%d级", limitLv))--不满足：提示晋阶需要xx级
        return
   end
   --sendMessage
   SendPB_10012(self.cardId)
   --刷新插槽为新军阶插槽，清空激活状态
    --显示晋阶成功界面，提示属性变化，与卡牌军阶变化
    self:showAdSuccess()
    --改变军阶等级
    print("qualityLv1:" .. self.qualityLv)
    for k,v in pairs(cardTbl) do
        self.qualityLv = v.rlv
    end
    print("qualityLv2:" .. self.qualityLv)
    self.nextQualityLv = self:GetNextLv(self.qualityLv, maxQualityLv)
    --刷新信息界面
    self:refreshRightBody()
end

--晋升成功
function wnd_cardyc:showAdSuccess(qualityLv)
    if not self.isInitAmSLayer then
        self.adSuccessPanel = GameObjectExtension.InstantiateFromPacket("ui_cardyc", "adSuccess",  self.gameObject)
        self.adSuccessPanel.name = "adSuccess"
        local name1 = "cardBeforePanel"
        self:createACFrame(self.adSuccessPanel.gameObject,name1,Vector3(-190,55,0),20,self.cardId,self.cardlv,self.armyCardLv,self.starLv)
        local name1 = "cardAfterPanel"
        self:createACFrame(self.adSuccessPanel.gameObject,name1,Vector3(190,55,0),20,self.cardId,self.cardlv,self.armyCardLv,self.starLv)
        self.adSuccessPanel:SetActive(false)
        self.isInitAmSLayer = true
    end
    self.adSuccessPanel:SetActive(true) 
    self:refreshAcframe(self.adSuccessPanel,"cardBeforePanel",self.cardId,self.cardlv,self.qualityLv,self.starLv)
    self:refreshAcframe(self.adSuccessPanel,"cardAfterPanel",self.cardId,self.cardlv,self.nextQualityLv,self.starLv)

    local btn_back = go:Find("clickPanel").gameObject 
    UIEventListener.Get(btn_back).onPress = function(btn_back, args)
        if args then
            self.adSuccessPanel:SetActive(false) 
        end
    end
    local go = self.adSuccessPanel.transform
    local uid = tonumber(string.format("%d%.2d",self.cardId,self.qualityLv))--通过卡牌id和军阶等级联合获取
    print("x:" .. uid)
    for i=1,4 do
        local cardEquip = sdata_ArmyCardQuality_data:GetFieldV("CardEquip"..i,uid)
        --4个属性
        local des = go:Find("desLab_"..i):GetComponent("UILabel")
        -- des.text = sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20037+i-1)
        local beforeLab = go:Find("Label_a"..i):GetComponent("UILabel")
        local afterLab = go:Find("Label_b"..i):GetComponent("UILabel")
    end
end

function wnd_cardyc:equipAllCallBack()
    print("card btn_equipAll....")
    print("slot" .. #self.slotState)
    for i=1,#self.slotState do
        if self.slotState[i] == EquipState.Enable_Enough and (self.itemNeedNumTbl[i] <= self.itemNumTbl[i]) then
            SendPB_10013(self.cardId, i-1)
            --errno==0
            for k,v in pairs(cardTbl) do
              self.slotState = cardTbl[k]["slot"]
            end
            for k,v in pairs(self.slotState) do
                print(k,v)
            end
            for _, v in ipairs(self.itemList) do
                if v.itemId == self.itemIdTbl[index] then
                    print("num:"..v.num)
                   -- v.num = v.num - self.itemNeedNumTbl[index] --背包资源扣除
                    break
                end
            end
            self:refreshRightBody()
            --errno~=0
        end
    end
end

function wnd_cardyc:equipCallBack(index)
    print("card btn_equipCallBack....self.slotIndex:"..self.slotIndex) --点击后消耗道具，完成插槽激活，改变界面内容
    SendPB_10013(self.cardId, self.slotIndex-1)
    --errno==0 
    for k,v in pairs(cardTbl) do
      self.slotState = cardTbl[k]["slot"]
    end
    for k,v in pairs(self.slotState) do
        print(k,v)
    end
    for _, v in ipairs(self.itemList) do
        if v.itemId == self.itemIdTbl[index] then
            print("num:"..v.num)
            -- v.num = v.num - self.itemNeedNumTbl[index] --背包资源扣除 -@a1
            break
        end
    end
    self:refreshRightBody()
end

--军功章内容
function wnd_cardyc:showArmyItem(index)
    local itemId = self.itemIdTbl[index]
    local state = self.slotState[index]
    print("itemId:" .. itemId)
    if not self.isInitMedalItemLayer then
        self.medalItemPanel = GameObjectExtension.InstantiateFromPacket("ui_cardyc", "medalItemPanel",  self.gameObject)
        self.medalItemPanel.name = "medalItemPanel"
        self.medalItemPanel:SetActive(false)
        self.isInitMedalItemLayer = true
    end
    self.medalItemPanel:SetActive(true)
    local go = self.medalItemPanel.transform
    local itemImg = go:Find("ItemBk/itemImg_Sp"):GetComponent("UISprite")--物品图
    local itemNameLab = go:Find("itemName_Lab"):GetComponent("UILabel") --物品名
    local addDesLab = go:Find("addDes_Lab"):GetComponent("UILabel") --属性加成
    local needLab = go:Find("Sprite/need_Lab"):GetComponent("UILabel") --需要
    local haveLab = go:Find("Sprite/xy_Lab"):GetComponent("UILabel")--现有
    local btn_equip = go:Find("Btn_equip").gameObject  --装备
    local btn_gain = go:Find("Btn_gain").gameObject--获得
    local btn_activate = go:Find("Btn_activate").gameObject --已激活

    local uid = tonumber(string.format("%d%.2d",self.cardId,self.qualityLv))--通过卡牌id和军阶等级联合获取
    local cardEquip = sdata_ArmyCardQuality_data:GetFieldV("CardEquip"..index,uid)
    --print("cardEquip:"..cardEquip)
    itemNameLab.text = "物品名"
    --itemNameLab.color = Color(1,0,0,1)--红  军阶颜色
    --addDesLab.text = --索引至Attribute表

    local haveNum = self.itemNumTbl[index]
    local needNum = self.itemNeedNumTbl[index]
    local needPanel = go:Find("Sprite").gameObject
    if state == EquipState.Active then--激活
        needPanel:SetActive(false)
    else
        haveLab.text = string.format("%s%d",sdata_UILiteral:GetV(sdata_UILiteral.I_Literal,20043),haveNum)
        needLab.text = string.format("%s%d",sdata_UILiteral:GetV(sdata_UILiteral.I_Literal,20044),needNum)
        haveLab.color = Color(1,1,1,1)--黑
        if haveNum < needNum then
            haveLab.color = Color(1,0,0,1)--红  
        end
        needPanel:SetActive(true)
    end

    btn_equip:SetActive(false)
    btn_gain:SetActive(false)
    btn_activate:SetActive(false)
    if state == EquipState.Enable_NotEnough then 
        btn_gain:SetActive(true)
    elseif state == EquipState.Enable_Enough then
        btn_equip:SetActive(true)
    elseif state == EquipState.Active then
        btn_activate:SetActive(true)
    end

    UIEventListener.Get(btn_equip).onPress = function(btn_equip, args)
        if args then
            self:equipCallBack(index)
        end
    end

    UIEventListener.Get(btn_gain).onPress = function(btn_gain, args)
        if args then
            print("获得，，，") --点击后跳转至军功章获取界面 获得途径界面
            self:GainWayLayer()
        end
    end

    UIEventListener.Get(btn_activate).onPress = function(btn_activate, args)
        if args then
            print("已激活..")
        end
    end

    --返回
    local btn_back = self.medalItemPanel.transform:Find("Btn_backSp").gameObject 
    UIEventListener.Get(btn_back).onPress = function(btn_back, args)
        if args then
            print("返回")
            self.medalItemPanel:SetActive(false)
        end
    end
end

function wnd_cardyc:GainWayLayer()
    self.medalItemPanel:SetActive(false)
    if not self.initGainLayer then
        self.gainWayPanel = GameObjectExtension.InstantiateFromPacket("ui_cardyc", "gainLayer",  self.gameObject)
        self.gainWayPanel.name = "gainWayPanel"
        self.gainWayPanel:SetActive(false)
        self.initGainLayer = true
    end
    self.gainWayPanel:SetActive(true)
    --返回
    local btn_back = self.gainWayPanel.transform:Find("Btn_backSp").gameObject 
    UIEventListener.Get(btn_back).onPress = function(btn_back, args)
        if args then
            print("返回")
            self.gainWayPanel:SetActive(false)
        end
    end
end

--技能
function wnd_cardyc:skillBody()
    -- refresh skill body
    --local skillPointSp = self.skillPanel.transform:Find("skillPont_Sp"):GetComponent("UISprite") --技能点图
    self.pointNumLab = self.skillPanel.transform:Find("bgSp/pointNum_Lab"):GetComponent("UILabel") --总技能点数
    local btn_pointReset = self.skillPanel.transform:Find("bgSp/pointReset_Sp").gameObject --重置点数

    self.pointNumLab.text = self.totalSkPt
    self.isInitSptReset = false
    UIEventListener.Get(btn_pointReset).onPress = function(btn_pointReset, args)
        if args then
            self:skPtResetLayer()
        end
    end

    self.sItemIDTbl = {}--技能id表
    local uid = tonumber(string.format("%d%.3d",self.cardId,self.cardlv))--通过卡牌id和卡牌等级联合获取
    print("===========skilluid:" .. uid)
    for i=1,5 do
        local skillid = sdata_ArmyBase_data:GetFieldV("Skill"..i, uid)
        table.insert(self.sItemIDTbl, skillid)
    end

    self.sItemLvTbl = {0,0,0,0,0} ---技能等级
    for k,v in pairs(cardTbl) do
        self.skillTbl = v.skill
    end

    for i=1,5 do
        for _,v in pairs(self.skillTbl) do
            if v.id == i-1 then
                self.sItemLvTbl[i] = v.lv
            end
        end
    end
    local spTbl = {{x=0,y=91,z=0},{x=-142.5,y=-29,z=0},{x=142.5,y=-29,z=0},{x=-100.5,y=-195,z=0},{x=100.5,y=-195,z=0}}--图的位置 --100.5
    for i=1,5 do
        if not self.isfiveSIinit then
            local position = Vector3(spTbl[i].x, spTbl[i].y, spTbl[i].z)
            local name = string.format("skillFrame_%d",i)
            self:createSItem(self.skillPanel.gameObject,name,position,1.3,self.sItemIDTbl[i],self.sItemLvTbl[i],4,i) --技能框
           -- self:refreshSframe(self.skillPanel,name,self.sItemIDTbl[i],self.sItemLvTbl[i],i)
            self.isInitSUpLayer = false
        end
        --self:refreshSframe(self.skillPanel,name,self.sItemIDTbl[i],self.sItemLvTbl[i],i)
    end
    self.isfiveSIinit = true
end

function wnd_cardyc:canSItemup()
    --a.    技能等级<卡牌军阶
    --b.  升级所需技能点≤持有技能点
    local slv = 1
    if  slv < self.cardlv then
        if self.skcost <= self.totalSkPt then
            --redPointHint
            return true
        end
    end
    return false
end

function wnd_cardyc:showSItemUp(i,itemId,lv)
    if not self.isInitSUpLayer then
        self.sItemUpPanel = GameObjectExtension.InstantiateFromPacket("ui_cardyc", "skillItemUpLayer",  self.gameObject)
        self.sItemUpPanel.name = "skillItemUpLayer"
        local position = Vector3(-210,60,0)
        local depthNum = 20
        self:createSItem(self.sItemUpPanel,"skillFrame",position,1.3,itemId,lv,depthNum,i)
        self.sItemUpPanel:SetActive(false)
        self.isInitSUpLayer = true
    end
    print(string.format("id:%d,i:%d,lv:%d",itemId,i,lv))
    self:refreshSItemLayer(i,itemId,lv)--初始化/刷新界面内容
    local btn_unlock = self.sItemUpPanel.transform:Find("Btn_unLockSp").gameObject 
    UIEventListener.Get(btn_unlock).onPress = function(btn_unlock, args)
        if args then
            print(string.format("btn_unlock...卡牌%d星解锁",i))--解锁
            --按钮未不可点击状态，文字显示为“卡牌x星解锁”
        end
    end

    local btn_upLv = self.sItemUpPanel.transform:Find("Btn_updateSp").gameObject
    UIEventListener.Get(btn_upLv).onPress = function(btn_upLv, args)
        if args then
            self:sItemUpCallBack(i,itemId,lv)
        end
    end 

    if lv == 0 then
        lv = 1 --未解锁显示1级属性
        --解锁技能界面
        btn_unlock:SetActive(true)
        btn_upLv:SetActive(false)
    else
        btn_unlock:SetActive(false)
        btn_upLv:SetActive(true)
    end

    
    local btn_back = self.sItemUpPanel.transform:Find("Btn_backSp").gameObject
    UIEventListener.Get(btn_back).onPress = function(btn_back, args)
        if args then
            self.sItemUpPanel:SetActive(false)
        end
    end
    self.sItemUpPanel:SetActive(true)
end

function wnd_cardyc:refreshSItemLayer(i,itemId,lv)
    self:refreshSframe(self.sItemUpPanel,"skillFrame",itemId,lv,i)
    self.nextSItemLv = self:GetNextLv(lv,maxSkillLv)

    local lv_Lab = self.sItemUpPanel.transform:Find("lv_Lab"):GetComponent("UILabel")
    lv_Lab.text = sdata_UILiteral:GetV(sdata_UILiteral.I_Literal,20041)
    local sdes_Lab = self.sItemUpPanel.transform:Find("skilldes_Lab"):GetComponent("UILabel")--说明描述
    --sdes_Lab.text = string.format("%s%s",sdata_UILiteral:GetV(sdata_UILiteral.I_Literal,20042),string)

    local btn_upLv = self.sItemUpPanel.transform:Find("Btn_updateSp").gameObject
    local costLab = btn_upLv.transform:Find("skillPtNum_Lab"):GetComponent("UILabel")--升级消耗技能点
    self.skcost= sdata_ArmyCardSkillCost_data:GetFieldV("SkillPt",self.nextSItemLv)--mark
    costLab.text =  self.skcost
    --技能升级按钮显示
    if self.skcost>self.totalSkPt then
        costLab.color = Color(255/255,0/255,0/255,255/255)--红
    end

    local lvProLab = self.sItemUpPanel.transform:Find("lvProgress_Lab"):GetComponent("UILabel") 
    lvProLab.text = string.format("[f15c03]%d[-]/%d",lv,16)--当前技能/技能总级
end

function wnd_cardyc:sItemUpCallBack(index,itemId,lv)
    --a.    技能等级<卡牌军阶  --b.  升级所需技能点≤持有技能点
   print("sItem btn_upLv.callback...")
    --判断技能等级<卡牌军阶
    if self.nextSItemLv >= self.cardlv then
        print("请先提升卡牌军阶")
        return
    end

    --判断升级所需技能点≤持有技能点
    if self.skcost>self.totalSkPt then
        print("技能点数不足，请前往xxx获取")
        return
    end
    print("up,,,") --sendMessage --提升技能 -刷新
    SendPB_10014(self.cardId, index-1)
    --总技能点
    self.totalSkPt = self.totalSkPt - self.skcost
    self:refreshSItemLayer(index,itemId,self:GetNextLv(lv,maxSkillLv))
    self:refreshRightBody()
end

function wnd_cardyc:skPtResetLayer()
    print("技能点重置,,,")
    if not self.isInitSptReset then
        self.sPtRPanel = GameObjectExtension.InstantiateFromPacket("ui_cardyc", "sPtReset",  self.gameObject)
        self.sPtRPanel.name = "sPtReset"

        local name = "aCardFrame"
        self:createACFrame(self.sPtRPanel.gameObject,name,Vector3(-199,40,0),20,self.cardId,self.cardlv,self.qualityLv,self.starLv)
        self.sPtRPanel:SetActive(false)
        self.isInitSptReset = true
    end
    self:refreshAcframe(self.sPtRPanel,"aCardFrame",self.cardId,self.cardlv,self.qualityLv,self.starLv)


    local title_Lab = self.sPtRPanel.transform:Find("title_Lab"):GetComponent("UILabel") 
    title_Lab.text = sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20029)--标题
    local des_Lab = self.sPtRPanel.transform:Find("des_Lab"):GetComponent("UILabel")
    des_Lab.text = sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20030)--描述
    --back_btn
    local btn_back = self.sPtRPanel.transform:Find("Btn_backSp").gameObject
    UIEventListener.Get(btn_back).onPress = function(btn_back, args)
        if args then
            self.sPtRPanel:SetActive(false)
        end
    end
    self.comCostmNum = 100--普通重置消耗钻石
    self.perCostNum = 300--完美重置消耗钻石
    local comCostNumLab = self.sPtRPanel.transform:Find("Sprite/comCostNumLab"):GetComponent("UILabel")
    comCostNumLab.text = self.comCostmNum
    local perCostNumLab = self.sPtRPanel.transform:Find("Sprite/perCostNumLab"):GetComponent("UILabel")
    perCostNumLab.text = self.perCostNum

    local btn_comReset = self.sPtRPanel.transform:Find("Btn_comReset").gameObject
    UIEventListener.Get(btn_comReset).onPress = function(btn_comReset, args)
        if args then
            self:comResetCallBack()
        end
    end
    
    local btn_perReset = self.sPtRPanel.transform:Find("Btn_perReset").gameObject
    UIEventListener.Get(btn_perReset).onPress = function(btn_perReset, args)
        if args then
            self:perResetCallBAck()
        end
    end
    self.sPtRPanel:SetActive(true)
end

function wnd_cardyc:comResetCallBack()
    print("comResetCallBack")
    --已解锁的技能等级变为1级，返还升级所用的技能卡数目的80%加至总技能卡中
    SendPB_10015(self.cardId, self.comCostmNum)
    self.totalSkPt = currencyTbl["skillpt"] --技能点
    print("skillpt:"..currencyTbl["skillpt"])
    self.pointNumLab.text = self.totalSkPt
end

function wnd_cardyc:perResetCallBAck()
    print("perResetCallBAck")--100%
    SendPB_10015(self.cardId, self.perCostNum)
    self.totalSkPt = currencyTbl["skillpt"] --技能点
    print("skillpt:"..currencyTbl["skillpt"])
    self.pointNumLab.text = self.totalSkPt
end

--兵员
function wnd_cardyc:sodilerBody()
    print("soldier")
    --卡
    if not self.isInitAcFrame then
        local position = Vector3(0,136,0)
        self:createACFrame(self.soldierPanel,"cardFame",position,4,self.cardId,self.cardlv,self.starLv,self.qualityLv)
        self.isInitAcFrame = true
    end
    
    print("id:"..self.cardId)
    print("lv:"..self.cardlv)
    print("qualityLv:"..self.qualityLv)
    print("starlv:"..self.starLv)
    self:refreshAcframe(self.soldierPanel,"cardFame",self.cardId,self.cardlv,self.qualityLv,self.starLv)
    self:refreshSBody()

    local costNameLab1 = self.soldierPanel.transform:Find("cardName_Lab"):GetComponent("UILabel") --卡牌名
    local costNameLab2 = self.soldierPanel.transform:Find("badgeName_Lab"):GetComponent("UILabel")--兵牌
    costNameLab1.text = sdata_armycardbase_data:GetFieldV("Name", self.cardId)
    costNameLab2.text = sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20027)
    local cardSp = self.soldierPanel.transform:Find("soldier_Sp"):GetComponent("UISprite")--卡牌图
    --cardSp.spriteName = sdata_armycardbase_data:GetV(sdata_armycardbase_data.I_IconID, self.cardId)
    local badgeSp = self.soldierPanel.transform:Find("badge_Sp"):GetComponent("UISprite")--卡牌图
    -- badgeSp.spriteName = ""--

    local bnt_up = self.soldierPanel.transform:Find("Btn_soldierUpLv").gameObject
    UIEventListener.Get(bnt_up).onPress = function(bnt_up, args)
        if args then
           self:sUpCallBack()
        end
    end
    self:canSoldierUp()
end

function wnd_cardyc:canSoldierUp()
    --兵员等级未满可升级 --材料够
    print("tblnum:" .. #sdata_ArmyCardUseLimitCost_data.mData.body)
    local maxArmyLv  = #sdata_ArmyCardUseLimitCost_data.mData.body
    if self.soldierLv < maxArmyLv then
        if self.cardCostN <= self.cardFragment then
            if self.badgeCostN <= self.badgeNum then
                --redPointhint
                return true
            end
        end
    end
    return false
end

function wnd_cardyc:refreshSBody()
    --for k,v in pairs(cardTbl) do
       -- self.soldierLv = v.slv
    --end 
    --兵员等级上限
    local sLvProLab = self.soldierPanel.transform:Find("lvProgress_Lab"):GetComponent("UILabel")
    local lvLabel = sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20041)
    sLvProLab.text = string.format("%s%d/%d",lvLabel,self.soldierLv,maxArmyLv)--兵员等级/兵员等级上限
    local sdesLab = self.soldierPanel.transform:Find("des_Lab"):GetComponent("UILabel")
    local desLabel = sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20042)
    --sdesLab.text = string.format("%s%s",desLabel,sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20031))--说明
    --sdesLab.text = string.format("%s%d(%d)",desLabel, )--@1张数

    local neednumLab1 = self.soldierPanel.transform:Find("neednumLab_1"):GetComponent("UILabel")--neednumLab_1
    local neednumLab2 = self.soldierPanel.transform:Find("neednumLab_2"):GetComponent("UILabel")
    local numLab1 = self.soldierPanel.transform:Find("numLab_1"):GetComponent("UILabel")--neednumLab_1
    local numLab2 = self.soldierPanel.transform:Find("numLab_2"):GetComponent("UILabel")

    self.cardCostN = sdata_ArmyCardUseLimitCost_data:GetFieldV("Card",self.soldierLv)
    self.badgeCostN = sdata_ArmyCardUseLimitCost_data:GetFieldV("Coin",self.soldierLv)
    neednumLab1.text = string.format("x%d",self.cardCostN)
    neednumLab2.text = string.format("x%d",self.badgeCostN)

    self.cardFragment=300
    --local s = sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20028)
    --print(string.len(s))
    local str1 = string.format("(%s%d)",sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20028),self.cardFragment)
   --Color(255/255,165/255,0/255,255/255)--1e95b7
  -- print(string.sub(str1, 8, -2))
    if self.cardCostN > self.cardFragment then
        numLab1.color = Color(255/255,255/255,246/255,255/255)
        str1 = string.format("(%s%d)",sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20028),self.cardFragment) --标红
        -- numLab1.color = Color(255/255,0/255,0/255,255/255)--
    else
        numLab1.color = Color(0/255,255/255,246/255,255/255)--
    end
    numLab1.text = str1

    local str2 = string.format("(%s%d)",sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20028),self.badgeNum)
    numLab2.color = Color(0/255,255/255,246/255,255/255)
    if self.badgeCostN > self.badgeNum then
        numLab1.color = Color(255/255,255/255,246/255,255/255)
        str2 = string.format("(%s[ff0000]%d[-])",sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20028),self.badgeNum)
    end
    numLab2.text = str2
end

function wnd_cardyc:sUpCallBack()
    print("soldierLv up CallBack..")
    --判断等级
    local maxArmyLv  = #sdata_ArmyCardUseLimitCost_data.mData.body
    if self.soldierLv >= maxArmyLv then
        print("兵员等级已达上限，不可提升")
        return
    end
    --判断卡牌碎片是否足够
    if self.cardCostN > self.cardFragment then
        print("卡牌碎片不足")
        return
    end
    --判断兵牌是否足够
    if self.badgeCostN > self.badgeNum then
        print("兵牌碎片不足")
        return
    end
    
    print("up,,,")
    --发消息,提升等级,刷新界面
    SendPB_10011(self.cardId,1)
    self.soldierLv = self.soldierLv +1
    self.cardFragment = self.cardFragment - self.cardCostN
    self.badgeNum = self.badgeNum - self.badgeCostN 
    self:refreshSBody()
end

--协同
function wnd_cardyc:synergyBody()
    print("synergy")
    self.xtItemIDTbl = {}--协同材料
    self.xtState = {1,2,2,0}
    self.armyUnionLv = {1,2,3,1}
    
    for i=1, #self.armyUnionLv do
        local auid = tonumber(string.format("%d%.2d",self.cardId,self.armyUnionLv[i]))--通过卡牌id和协同等级联合获取
        print(string.format("auid:%d",auid))
        if not self.isAttrItemInit then
            local itemFarme = GameObjectExtension.InstantiateFromPacket("ui_cardyc", "itemPanel", self.synergyPanel.gameObject)
            itemFarme.name = string.format("itemFarme_%d",i)
            itemFarme.transform.localPosition = Vector3(0, 180-104*(i-1), 0)

            UIEventListener.Get(itemFarme).onPress = function(itemFarme, args)
                if args then
                    self:icCallBack(i)
                end
            end
        end
        --self.auID = tonumber(string.format("%d%.2d",self.cardId,self.armyUnionLv[i]))
        --RequireCardID1","RequireCardStar1","RequireCardLevel1","RequireCardQuality1
        local itemId = sdata_ArmyCardUnion_data:GetFieldV("RequireCardID"..i, auid)--需要的材料id
        local rStar = sdata_ArmyCardUnion_data:GetFieldV("RequireCardStar"..i, auid)
        local rLv = sdata_ArmyCardUnion_data:GetFieldV("RequireCardLevel"..i, auid)
        local rQuality = sdata_ArmyCardUnion_data:GetFieldV("RequireCardQuality"..i, auid)

        table.insert(self.xtItemIDTbl, itemId)
        print(string.format("x:%d , itemId：%d", auid, itemId))

        local itemcell = self.synergyPanel.transform:Find("itemFarme_"..i):GetComponent("UIWidget")
        local itemSp = itemcell.transform:Find("Sprite"):GetComponent("UISprite")
        local plusSp = itemcell.transform:Find("plusSp").gameObject
        local upSp = itemcell.transform:Find("upSp").gameObject
        local atdAddLab = itemcell.transform:Find("Container/atdAddLab"):GetComponent("UILabel")
        local atdAddNextLab = itemcell.transform:Find("Container/atdAddNextLab"):GetComponent("UILabel")
        --local auidnext = tonumber(string.format("%d%.2d",self.cardId,self.armyUnionLv[i]+1))--通过卡牌id和协同等级联合获取
        --atdAddLab.text = string.format()
        atdAddNextLab.color = Color(255/255,165/255,0/255,255/255)
        --atdAddNextLab.gameObject:SetActive(false)
        plusSp:SetActive(false)
        upSp:SetActive(false)
        --atdAddNextLab:SetActive(false)
        print("lv:" .. self.armyUnionLv[i])
        print("state:" .. self.xtState[i])
        if  self.xtState[i] == synergyState.unactive then
            --itemSp.spriteName = "yijijiemian_juxing_touxiangkuang_weijihuo"
            --未解锁 + 和nextatdLab隐藏
            itemSp.color = Color(105/255,105/255,105/255,105/255)
            atdAddLab.color =  Color(105/255,105/255,105/255,105/255)
        elseif self.xtState[i] == synergyState.canActive  then
            plusSp:SetActive(true)
        elseif self.xtState[i] == synergyState.activated  then
            atdAddNextLab.gameObject:SetActive(true)
            upSp:SetActive(true)
        end
        
    end
    self.isAttrItemInit = true
    local count = 0
    for i=1,#self.xtState do
        if self.xtState[i] == synergyState.activated then
            count = count+1
        end
    end
    print("count" .. count)
    local atbAddPanel = self.synergyPanel.transform:Find("atbAddPanel"):GetComponent("UIWidget")
    
    --随便哪行都一样
    local auid = tonumber(string.format("%d%.2d",self.cardId,self.armyUnionLv[1]))
    print("auID"..auid)
    ---[[
    for i=1,3 do
        
        local atbAddLab = atbAddPanel.transform:Find("atbAddLab_"..i):GetComponent("UILabel")
        --UnionAttributeAdd1  AddPoint1
        local utadd = sdata_ArmyCardUnion_data:GetFieldV("UnionAttributeAdd"..i, auid)
        local addpoint = sdata_ArmyCardUnion_data:GetFieldV("AddPoint"..i,auid)
        print(string.format("utadd=%d, addpoint=%d",utadd,addpoint))
        atbAddLab.text = string.format("type%d+%d (have %d xt)",utadd,addpoint*100,i+1)  --类型+加成

        if count >= i+1 then
            atbAddLab.text = string.format("[00ff00]type%d+%d[-]",utadd,addpoint*100)  --类型+加成
        end
    end
    --]]
end

function wnd_cardyc:canSynergyUp()
    --协同所需的条件达成，但未点击激活
    --redPointHint
    return false
end

function wnd_cardyc:icCallBack(index)
    print("itemFarme.."..index)
    --local maxArmyUnionLv = #sdata_ArmyCardUnionCost_data.mData.body
    ---协同等级
    if self.armyUnionLv[index] >= maxArmyUnionLv then
        print("已达协同等级上限")
        return 
    end
    --self.xtState = {2,3,3,1}
    --self.armyUnionLv = {1,2,3,1}
    --self.xtItemIDTbl
    if self.xtState[index] == synergyState.unactive then
        print("需求卡牌未达到条件 x卡x星可以激活")
    elseif self.xtState[index] == synergyState.canActive then
        print("kjh")
        self:xtupLayer(index)
    elseif self.xtState[index] == synergyState.activated then
        print("up")
        self:xtupLayer(index)
    end
end

--协同升级
function wnd_cardyc:xtupLayer(index)
    print("xtup,,,")
    if not self.isInitxtLayer then
        self.xtPanel = GameObjectExtension.InstantiateFromPacket("ui_cardyc", "xtupLayer",  self.gameObject)
        self.xtPanel.name = "xtPanel"
        self:createACFrame(self.xtPanel.gameObject,"cardFrame",Vector3(-220,20,0),20,self.cardId,self.cardlv,self.armyCardLv,self.starLv)
        self.xtPanel:SetActive(false)
        self.isInitxtLayer = true
    end
    self:refreshAcframe(self.xtPanel,"cardFrame",self.cardId,self.cardlv,self.armyCardLv,self.starLv)
    self.xtPanel:SetActive(true)
    local xtLab = self.xtPanel.transform:Find("xtLab"):GetComponent("UILabel")
    -- xtLab.text = "令xx为xx协同作战" --升级xx对xx的协同作战能力  -- 材料名 卡名
    local tipLab = self.xtPanel.transform:Find("tipLab"):GetComponent("UILabel")
    tipLab.text = sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20032)
    local titleLab = self.xtPanel.transform:Find("title_Lab"):GetComponent("UILabel")
    if self.xtState[index] == synergyState.activated then --已激活
         titleLab.text = sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20035)
    elseif self.xtState[index] ==synergyState.canActive then --可激活
         titleLab.text = sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20034)
    end

    print("carname:" .. self:GetName(self.cardId))
   
    local costNameLab_1 = self.xtPanel.transform:Find("costNameLab_1"):GetComponent("UILabel")
    local costSp_1 = self.xtPanel.transform:Find("costSp_1"):GetComponent("UISprite")
    local costNameLab_2 = self.xtPanel.transform:Find("costNameLab_2"):GetComponent("UILabel")
    local costSp_2 = self.xtPanel.transform:Find("costSp_2"):GetComponent("UISprite")
    costNameLab_1.text = sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20027)
    costNameLab_2.text = sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20033)
    self:initRFXtcost(index)
    
    local btn_cancle = self.xtPanel.transform:Find("btn_cancle").gameObject
    UIEventListener.Get(btn_cancle).onPress = function(btn_cancle, args)
        if args then
            self.xtPanel:SetActive(false)
        end
    end
    local btn_ok = self.xtPanel.transform:Find("btn_ok").gameObject
    UIEventListener.Get(btn_ok).onPress = function(btn_ok, args)
        if args then
            self:okCallBack(index)
        end
    end
    local btn_back = self.xtPanel.transform:Find("Btn_backSp").gameObject
    UIEventListener.Get(btn_back).onPress = function(btn_back, args)
        if args then
            self.xtPanel:SetActive(false)
        end
    end
end

function wnd_cardyc:okCallBack(index)
    --兵牌
    local needgold =  sdata_ArmyCardUnionCost_data:GetFieldV("Gold", self.armyUnionLv[index])  
    if self.badgeNum < needgold then
        print("兵牌不够..")
        return
    end
    
    --金币
    local needCoin =  sdata_ArmyCardUnionCost_data:GetFieldV("Coin", self.armyUnionLv[index]) 
    if self.coinNum < needCoin then
        print("金币不够..")
        return
    end
    
    --等级限制
    if self.cardlv <= self.armyUnionLv[index] then
        print("请先提升卡牌等级..")
        return
    end

    print("ok")
    --sendMessage 扣资源refresh
    local needgold =  sdata_ArmyCardUnionCost_data:GetFieldV("Gold", self.armyUnionLv[index])  
    local needCoin =  sdata_ArmyCardUnionCost_data:GetFieldV("Coin", self.armyUnionLv[index]) 
    self.badgeNum = self.badgeNum - needgold
    self.coinNum = self.coinNum - needCoin
    self.armyUnionLv[index] = self:GetNextLv(self.armyUnionLv[index], maxArmyUnionLv)
    self:initRFXtcost(index)
end

function wnd_cardyc:initRFXtcost(index)
    local neednumLab1 = self.xtPanel.transform:Find("neednumLab_1"):GetComponent("UILabel")
    local neednumLab2 = self.xtPanel.transform:Find("neednumLab_2"):GetComponent("UILabel")
    local numLab1 = self.xtPanel.transform:Find("numLab_1"):GetComponent("UILabel")
    local numLab2 = self.xtPanel.transform:Find("numLab_2"):GetComponent("UILabel")
    --兵牌
    local needgold =  sdata_ArmyCardUnionCost_data:GetFieldV("Gold", self.armyUnionLv[index])  
    neednumLab1.text = string.format("x%d",needgold)

    local str1 = string.format("(%s%d)",sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20028),self.badgeNum)--97ff03
    if self.badgeNum < needgold then
        str1 = string.format("(%s[FF0000]%d[-])",sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20028),self.badgeNum) --300  [FF0000 ]num[-]
    end
    numLab1.text = str1
    --金币
    local needCoin =  sdata_ArmyCardUnionCost_data:GetFieldV("Coin", self.armyUnionLv[index]) 
    neednumLab2.text = string.format("x%d",needCoin)
    local str2 = string.format("(%s%d)",sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20028),self.coinNum)
    if self.coinNum < needCoin then
        str2 = string.format("(%s[FF0000]%d[-])",sdata_UILiteral:GetV(sdata_UILiteral.I_Literal, 20028),self.coinNum) 
    end
    numLab2.text = str2
end


function wnd_cardyc:createSItem(parent,name,Vector3,scale,itemId,lv,depthNum,index) --技能框
    print("createSItem...itemid:"..itemId)
    local go = GameObjectExtension.InstantiateFromPacket("ui_cardyc", "skillFrame", parent)
    --print("tag:"..go.tag)
    go.name = name
    go.transform:FindChild("img_Sp"):GetComponent("UISprite").depth = depthNum
    --print(string.format("x=%d,y=%d,z=%d",spTbl[i].x, spTbl[i].y, spTbl[i].z))
    go.transform.localPosition = Vector3
    go.transform.localScale = Vector3(scale, scale, scale)
    local imgSp = go.transform:Find("img_Sp").gameObject --技能图
    local lvLab = go.transform:Find("lv_Lab"):GetComponent("UILabel") --技能等级
    local nameLab = go.transform:Find("name_Lab"):GetComponent("UILabel") --技能名
    local bkSprite = go.transform:Find("bkSprite").gameObject
    local bklockSp = go.transform:Find("bklockSp").gameObject

    lvLab.depth = depthNum+1
    nameLab.depth = depthNum
    lvLab.text = lv

    bklockSp:SetActive(false)
    bkSprite:SetActive(true)
    if lv == 0 then
        bklockSp:SetActive(true)
        bkSprite:SetActive(false)
    end
    UIEventListener.Get(imgSp).onPress = function(imgSp, args)
        if args then
            self:showSItemUp(index,itemId,lv)
        end
    end
    self:refreshSframe(parent,name,itemId,lv,index)
end

function wnd_cardyc:refreshSframe(parent,panelName,itemId,skilllv,index)
    print(string.format("id:%d,lv:%d,index:%d",itemId,skilllv,index))
    --refresh skillFrameInfo
    if not parent then
        return
    end
    if not panelName then
        return
    end

    local panel = parent.transform:Find(panelName):GetComponent("UIWidget") 
    local imgSp = panel.transform:Find("img_Sp"):GetComponent("UISprite")
    local nameLab = panel.transform:Find("name_Lab"):GetComponent("UILabel")
    local lvLab = panel.transform:Find("lv_Lab"):GetComponent("UILabel")
    local bkSprite = panel.transform:Find("bkSprite").gameObject
    local bklockSp = panel.transform:Find("bklockSp").gameObject

    if parent == self.sItemUpPanel and skilllv==0 then
        skilllv = skilllv+1 --未解锁显示一级
    end
    if itemId then
        --imgSp.spriteName = sdata_armycardbase_data:GetV(sdata_armycardbase_data.I_IconID, itemId)--@1技能图
        --print("id:" .. self.sItemIDTbl[index])
        nameLab.text = sdata_Skill_data:GetFieldV("Name",self.sItemIDTbl[index])  --解锁技能名
    end

    if skilllv then
        lvLab.text = skilllv
    end
    bklockSp:SetActive(false)
    bkSprite:SetActive(true)

    local limitStr = sdata_UILiteral:GetV(sdata_UILiteral.I_Literal,20045)
    if skilllv==0 then
        if index == 5 then
            limitStr = sdata_UILiteral:GetV(sdata_UILiteral.I_Literal,20046)
        end
        nameLab.text = limitStr --index .."_xjs" --几星解锁
        imgSp.color = Color(123/255,123/255,123/255,123/255)
        bklockSp:SetActive(true)
        bkSprite:SetActive(false)
    end
end

function wnd_cardyc:createACFrame(parent,name,Vector3,depthNum,cardId,lv,qualitylv,starlv)
    local go = GameObjectExtension.InstantiateFromPacket("ui_cardyc", "a", parent).transform
    go.name = name
    parent.transform:Find(name):GetComponent("UIWidget").depth = depthNum
    go:FindChild("bkSp"):GetComponent("UISprite").depth = depthNum
    go:FindChild("bkSp/Sprite"):GetComponent("UISprite").depth = depthNum
    go:FindChild("img_Sp"):GetComponent("UISprite").depth = depthNum
    go.localPosition = Vector3
    local imgSp = go:Find("img_Sp").gameObject --卡牌图
    local lvLab = go:Find("bkSp/lv_Lab"):GetComponent("UILabel") --等级
    local medalSp = go:Find("medal_Sp"):GetComponent("UISprite") --军阶图
    lvLab.depth = depthNum+1
    medalSp.depth = depthNum+1
    local sp =  go:Find("StarPanel"):GetComponent("UIWidget") 
    sp.depth = depthNum+1
    --卡等级星级军阶图卡图
    lvLab.text = lv
    --self:refreshAcframe(parent,name,cardId,lv,qualitylv,starlv)
end

function wnd_cardyc:refreshAcframe(parent,panelName,cardId,cardlv,qualitylv,starlv)
    --refresh acardFrameInfo
    if not parent then
        print("no parent..")
        return
    end
    if not panelName then
        return
    end

    local cardPanel = parent.transform:Find(panelName):GetComponent("UIWidget") 
    local cardImgAfter = cardPanel.transform:Find("img_Sp"):GetComponent("UISprite")
    local cardMedalAfter = cardPanel.transform:Find("medal_Sp"):GetComponent("UISprite")
    local cardLvAfter = cardPanel.transform:Find("bkSp/lv_Lab"):GetComponent("UILabel")
    local starPanelA = cardPanel.transform:Find("StarPanel"):GetComponent("UIWidget") 

    if cardId then
        --cardImgAfter.spriteName = sdata_armycardbase_data:GetV(sdata_armycardbase_data.I_IconID, cardId)--@1
    end
    if cardlv then
        cardLvAfter.text = cardlv
    end
    if starlv then
        for i=1,maxStarLv do
           local star = starPanelA.transform:Find("star_"..i).gameObject
           star:SetActive(true)
           if i > starlv then
             star:SetActive(false)
           end
        end
    end
    if qualitylv then
        cardMedalAfter.spriteName = sdata_ArmyCardQualityShow_data:GetV(sdata_ArmyCardQualityShow_data.I_QualityIcon, qualitylv)
    end
end

function wnd_cardyc:createAMFrame(parent,name,Vector3,itemId,state)
    local go = GameObjectExtension.InstantiateFromPacket("ui_cardyc", "armyMedalPanel", parent).transform
    go.name = name
    go.localPosition = Vector3
    self:refershAMFrame(parent,name,itemId,state)
end

function wnd_cardyc:refershAMFrame(parent,name,itemId,state)
    --refresh acardFrameInfo
    if not parent then
        print("no parent..")
        return
    end
    if not name then
        return
    end
    local go = parent.transform:Find(name):GetComponent("UIWidget").transform
    local bkSp = go:Find("Sprite").gameObject --正常边框
    local lockSp = go:Find("lockSp").gameObject--未激活边框
    local plusSp = go:Find("plusSp").gameObject--+
    local itemSp = go:Find("itemSp"):GetComponent("UISprite")--材料图

    if itemId then
        --itemSp.spriteName = sdata_armycardbase_data:GetV(sdata_armycardbase_data.I_IconID, cardId)--@1
    end
    bkSp:SetActive(true)
    lockSp:SetActive(false)
    if state == EquipState.Enable_NotEnough then
        itemSp.color = Color(123/255,123/255,123/255,123/255)
        plusSp:SetActive(false)
        bkSp:SetActive(false)
        lockSp:SetActive(true)
    elseif state == EquipState.Enable_Enough then
        itemSp.color = Color(255/255,255/255,255/255,255/255)
        plusSp:SetActive(true)
    elseif state == EquipState.Active then
        itemSp.color = Color(255/255,255/255,255/255,255/255)
        plusSp:SetActive(false)
    end
end

function wnd_cardyc:GetNextLv(lv, maxlv)
    if lv  then
        if lv < maxlv then
            return lv+1
        end
        return maxlv
    end
end

--获得卡牌名(名+品质)
function wnd_cardyc:GetName(cardId, qualityLv)
    --cardId is not nil
    local name = sdata_armycardbase_data:GetFieldV("Name", cardId) --@1
    if qualityLv ~= nil then
        local plusNum = sdata_ArmyCardQualityShow_data:GetFieldV("PlusNum", qualityLv)--@1
        if plusNum>0 then 
            name = string.format("%s+%d",name,plusNum)
        end
    end
    return name
end

--根据品阶获得颜色
function wnd_cardyc:getColor(qualityLv)
    local colorType = sdata_ArmyCardQualityShow_data:GetFieldV("QualityColor", qualityLv)
    if colorType == 1 then
        Color = Color(255/255,255/255,255/255,255/255)--白
    elseif  colorType == 2 then
         Color = Color(0/255,128/255,0/255,255/255)--绿
    elseif  colorType == 3 then
         Color = Color(0/255,0/255,255/255,255/255)--蓝
    elseif  colorType == 4 then
         Color = Color(128/255,0/255,128/255,255/255)--紫
    elseif  colorType == 5 then
         Color = Color(255/255,165/255,0/255,255/255)--橙
    elseif  colorType == 6 then
         Color = Color(255/255,0/255,0/255,255/255)--红
    end
    return Color
end

--标签页
function wnd_cardyc:tabControl(tabTable, activePageIndex, isBtnAllStateReacted)
    print(" tabControl..")
    --[[
    self.children = {}
    self.bodyNodes = {}
    for i,v in pairs(self.tabTable) do
        table.insert(self.children, v[1])
        if nil ~= v[2] then
         table.insert(self.bodyNodes, i, v[2])
        end
    end
    --]]
    self.index = 1
    if nil ~= activePageIndex and activePageIndex <= #tabTable then
        self.index = activePageIndex
    end
    print("selfindex = "..self.index)
    for k,v in pairs(self.children) do
        local lightSp = v.transform:Find("lightSp").gameObject
        lightSp:SetActive(false)
        if k == self.index then
            lightSp:SetActive(true)
        end
    end
    for i,btn in pairs(self.children) do
        UIEventListener.Get(btn).onPress = function(sender)
            if not isBtnAllStateReacted then
                print(" not isBtnAllStateReacted..")
                if i == self.index then
                    return
                end
            end
            --print("index = " .. self.index)
            if i == self.index then
                return
            else
                self:changedTab(i)
                self:tabBtnPressed(btn, i)
            end
        end
    end
    self:showTabBody()
end

function wnd_cardyc:showTabBody()
    print("showTabBody....")
    -- set the firsttime button and body
    for i,v in pairs(self.children) do
        if i == self.index then
            --v:setBrightStyle()
            print("set btn highlight..")
        end
    end
    if 0 ~= #self.bodyNodes then
        for i,v in pairs(self.bodyNodes) do
            if i ~= self.index then
                v:SetActive(false)
            else
                v:SetActive(true)
            end
        end
    end  
end

function wnd_cardyc:changedTab(newindex)
    print("newindex = " .. newindex)
    if 0 ~= #self.bodyNodes then
        self.bodyNodes[self.index]:SetActive(false)
        self.bodyNodes[newindex]:SetActive(true)
    end
    self.oldindex = self.index
    self.index = newindex

    for k,v in pairs(self.children) do
        local lightSp = v.transform:Find("lightSp").gameObject
        lightSp:SetActive(false)
        if k == self.index then
            lightSp:SetActive(true)
        end
    end
end

function wnd_cardyc:tabBtnPressed(sender, index)
    print(" tabBtnPressed..index = " .. index)
    if index == 1 then
        self:infBody()
    elseif index == 2 then
        self:skillBody()
    elseif index == 3 then
        self:sodilerBody()
    elseif index == 4 then
        self:synergyBody()
    end
end

-- start --
-- @description 设置按钮是否真的去改变状态,为了如果点此按钮实际是无效的操作，请调用这个函数
-- end --
function wnd_cardyc:revokeChanged()
    self:changedTab(self.oldindex)
end

function wnd_cardyc:getTabControlIndex()
    return self.index
end

-- start --
-- @description 直接跳到index这个页签
-- @param index 页签序号
-- @return nil
-- end --
function wnd_cardyc:jumpToTab(index, isCallback)
    self:changedTab(index)
    if isCallback == nil or isCallback then
        self.tabBtnPressed( self.children[index], index)
    end
end


function wnd_cardyc:OnHideFinish()
	print("wnd_cardyc:OnHideFinish.......")--隐藏主面板
end

function wnd_cardyc:RegStart()
	print("wnd_cardyc:RegStart.......")
    wnd_cardyc = self
end

function wnd_cardyc:OnLostInstance()
	print("wnd_cardyc:OnLostInstance.......")
end

return wnd_cardyc
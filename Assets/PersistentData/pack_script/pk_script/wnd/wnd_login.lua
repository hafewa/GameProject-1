--region *.lua
--Date 20150804
--登陆界面
--作者 wenchuan

function UserSound()
	if UserDatas == nil then return end
	local soundVolume = UserDatas.GetUserData("sound")
	if soundVolume==nil or soundVolume == "" then
		QKGameSetting.SetVolume(1)
	else
		QKGameSetting.SetVolume(tonumber(soundVolume))
	end
end

function UserMusic()
	if UserDatas == nil then return end
	local musicVolume = UserDatas.GetUserData("music")
	if musicVolume==nil or musicVolume == "" then
		BackgroundMusicManage.SetVolume(1)
	else
		BackgroundMusicManage.SetVolume(tonumber(musicVolume))
	end
end

wnd_login = nil--单例

local class_wnd_login = class(wnd_base)

local isFirstShow = true

function class_wnd_login:Start()
	self:Init(WND.Login )
	wnd_login = self
	self.currzone = -1 --当前zone
	self.ptLoginWndShowing = false--平台登录窗口是否处于显示中
end

--账户验证通过
function class_wnd_login:OnLoginSuccess(_)
	print("OnLoginSuccess#1")
end

function class_wnd_login:coLoginSuccess(param)
	print("coLoginSuccess#1")
	--等待逻辑资源装载完成
	while(not GameInit.LogicPogressManage:IsComplete()) do
		Yield()
	end
	print("coLoginSuccess#2")
	--请求获取角色信息
	--local jsonNM = QKJsonDoc.NewMap()
	--jsonNM:Add("n","PlyInfo")
	-- 发送请求
	--YQ2GameConn:SendRequest(jsonNM,0,true,self,self.OnPlyInfoFinished)
	self.OnPlyInfoFinished()
end

function class_wnd_login:OnPlyInfoFinished()
	
	print("coLoginSuccess#3")
	--界面助手
	if (CodingEasyer == nil)then
		CodingEasyer = CodingEasyerClass.new();
	end
	
	WndManage.Show("ui_prefight",0.5)
	print("coLoginSuccess#4")
	--填充数据
	PlayerData.data.dbg = true --(tonumber(reJsonDoc:GetValue("dbg"))==1)--是否调试模式
	print("coLoginSuccess#5")

	BackgroundMusicManage.Stop(0.2)
	BackgroundMusicManage.Play("uisound","onlinemusic1",1)
	wnd_login:Hide()--隐藏登录界面
	print("hide LoginSuccess#6")
	
	--wnd_tuiguan:Show()--显示主界面
	--wnd_tuiguan:SyncFakeGK()
	
	--print("战斗对象引用-------", Battlefield);
	Battlefield.LoadSceneOnly()
	Battlefield.LoadMainSceneActors()
	--Battlefield.ShowGrid()
	
	UserMusic()
	UserSound()
	
	EventHandles.OnLoginSuccess:Call(nil)
	
	
	--print("player level:", Player:GetAttr(PlayerAttrNames.Level))
	print("lua OnLoginSuccess #2")
	
end

function class_wnd_login:GetCurrZoneInfo()
	
end
--有对象互访逻辑的初始化
function class_wnd_login:CrossInit()
	
end

class_wnd_login.SuperShow = class_wnd_login.Show


function class_wnd_login:OnShowDone()
	--显示主面板
	hadJumpedToTuiguan = false --还没有进入到推关界面
	local mainpanel = self.instance:FindWidget("mainpanel")
	mainpanel:SetActive(true)
	if Cookies.HasKey("BGMUSIC") then
		if tonumber(Cookies.GetInt("BGMUSIC")) == 0 then
			self.bIsBackGoundmis = false
		else
			self.bIsBackGoundmis = true
		end
	else
		self.bIsBackGoundmis = true
	end
	self:Setmusic()
	BackgroundMusicManage.Play("uisound","logmusic1",1)
	self:BindUIEvent ("music_btn", UIEventType.Click, "Setmusic")
	
	
	--将CodingEasyer放到这里是为了在login加载完成的时候能够预加载wnd_tuiguan窗口
	if (CodingEasyerClass ~= nil)then
		CodingEasyer = CodingEasyerClass.new();
		if (wnd_tuiguan ~= nil)then
			wnd_tuiguan:PreLoad()
		end
	end
end
function class_wnd_login:Setmusic()
	self:SetWidgetActive("music_btn/open",self.bIsBackGoundmis)
	if self.bIsBackGoundmis then
		self.bIsBackGoundmis = false
		BackgroundMusicManage.SetVolume(1)
		QKGameSetting.SetVolume(1)
		BackgroundMusicManage.Play("uisound","logmusic1",1)
		Cookies.SetInt("BGMUSIC",1)
	else
		BackgroundMusicManage.SetVolume(0)
		QKGameSetting.SetVolume(0)
		Cookies.SetInt("BGMUSIC",0)
		self.bIsBackGoundmis = true
	end
	
end

function class_wnd_login:LoginInfoIsActived()
	return true
end

function class_wnd_login:UpdateLoginUserDisplay()
	
	print("class_wnd_login:UpdateLoginUserDisplay #1");
	--显示上次登录成功的角色名,以及登陆/注销 按钮显隐
	local cm_label_username = self:GetLabel("mainpanel/last_user_name")
	print("class_wnd_login:UpdateLoginUserDisplay #2");
	local btn_unlogin = self.instance:FindWidget("mainpanel/btn_unlogin")
	print("class_wnd_login:UpdateLoginUserDisplay #3");
	local btn_login = self.instance:FindWidget("mainpanel/btn_login")
	print("class_wnd_login:UpdateLoginUserDisplay #4");
	local hasUInfo = self:LoginInfoIsActived()
	print("class_wnd_login:UpdateLoginUserDisplay #5");
	
	if (not hasUInfo) then
		cm_label_username:SetValue("")
	else
		local _,_,plyName = GameCookies.GetLoginInfo()
		cm_label_username:SetValue(plyName)
	end
	
	btn_unlogin:SetActive(hasUInfo)
	btn_login:SetActive(not hasUInfo)
end

function class_wnd_login:OnNewInstance()
	
	--账户登录按钮
	self:BindUIEvent("mainpanel/btn_login",UIEventType.Click,"OnUserLoginClick")
	
	--注销登录按钮
	self:BindUIEvent("mainpanel/btn_unlogin",UIEventType.Click,"OnUnLoginClick")
	
	--开始游戏按钮
	self:BindUIEvent("mainpanel/btn_go",UIEventType.Click,"OnGoClick")
	self.bIsBackGoundmis = false
end


--- <summary>
--- 开始游戏
--- </summary>
function class_wnd_login:OnGoClick(gameObject,_)
	StartCoroutine(self,self.coLoginSuccess,{})
	--显示登录界面
	--GamePlatformLogic.ShowLogin()--GamePlatformID.QK
end

--- <summary>
--- 显示用户登录对话框
--- </summary>
function class_wnd_login:OnUserLoginClick(gameObject,_)
	
	print("class_wnd_login:OnUserLoginClick")
end

--- <summary>
--- 注销登陆
--- </summary>
function class_wnd_login:OnUnLoginClick(gameObject,_)
	--清除用户存档
	GameCookies.SaveLoginInfo(nil,nil,nil)
	
	--刷新显示
	self:OnShowDone()
end

function class_wnd_login:ShowLoginMain()
	self.ptLoginWndShowing = false;
	
	--显示主面板
	local mainpanel = self.instance:FindWidget("mainpanel")
	mainpanel:SetActive(true)
end

function class_wnd_login:ShowLoginWnd()
	
end

function class_wnd_login:OnLostInstance()
end

function class_wnd_login:Show(duration)
	
	self:_Show(duration)
end

function class_wnd_login:Hide(duration)
	
	self:_Hide(duration)
end

return class_wnd_login.new

--endregion
--region *.lua
--Date 20160704
--OOSyncList


 OOSyncList = classWC()

 function OOSyncList:ctor(parent,listName)
    self.lName = listName
    self.SyncObj = parent:GetChild(listName,true)
 end

 function OOSyncList:Foreach(callBack)
    --print("OOSyncList:Foreach",self.SyncObj)
    if(self.SyncObj==nil) then return end

    self.SyncObj:Foreach(callBack)
 end

--endregion
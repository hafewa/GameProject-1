local data = require "username_girl"
local class_sdata_username_girl = classWC(luacsv) 

--- <summary>
--- 随机取一个
--- </summary>
function class_sdata_username_girl:RandItem()
    local count = #self.mData.body
    local key = math.floor(math.random(1,count))
    return self:GetFieldV("NameGirl",key)
end


return  class_sdata_username_girl.new(data)
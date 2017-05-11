JsonParse = {}


--- <summary>
--- �����յ��İ�
--- </summary>
--- <returns type="QKJsonDoc"></returns>
function JsonParse.RecvNM(nmstr)
	local jsonnm = QKJsonDoc.NewMap()
	jsonnm:Parse(nmstr)
	return jsonnm
end 

--- <summary>
--- ���� : ���ɷ��Ͱ�JsonDoc
--- nmhead : string ����
--- </summary>
--- <returns type="QKJsonDoc"></returns>
function JsonParse.SendNM(nmhead)
	local jsonnm = QKJsonDoc.NewMap()
    jsonnm:Add("n",nmhead)
	return jsonnm
end

--- <summary>
--- ���� : ��json�ĵ�ת����lua��
--- jsonDoc : QKJsonDoc
--- </summary>
--- <returns type="table"></returns>
function JsonParse.JsonDoc2Table(jsonDoc)
    local re = {}
    local eachFunc = function(k,v) 
        re[k] = v
    end
    jsonDoc:Foreach(eachFunc)
    return re
end

function JsonParse.JsonDocKey2Array(jsonDoc)
    local re = {}
    local eachFunc = function(k,_) 
        table.insert(re,k)
    end
    jsonDoc:Foreach(eachFunc)
    return re
end

--- <summary>
--- ���� : ��json�ĵ���key����Ϊ�������͵�����
--- jsonDoc : QKJsonDoc
--- </summary>
--- <returns type="table"></returns>
function JsonParse.JsonDocKey2NumberArray(jsonDoc)
    local re = {}
    local eachFunc = function(k,_)
        table.insert(re,tonumber(k) )
    end
    jsonDoc:Foreach(eachFunc)
    return re
end
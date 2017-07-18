# ForceJson

[![License](https://img.shields.io/badge/license-Apache%202-4EB1BA.svg)](https://www.apache.org/licenses/LICENSE-2.0.html)

C#和Asp.Net版的Json解析工具类，是从java中移植JsonObject到.Net中的，使用方法和api和java中的JsonObject完全一致

### 创建JSONObject示例
```
JSONObject json = new JSONObject();  
json.Put("sex", "男");  
json.Put("age", 123);  
json.Put("name", "张三");  
Console.WriteLine(json.ToString()); 
``` 


### 创建JSONArray示例
```
JSONObject json = new JSONObject();  
json.Put("sex", "男");  
json.Put("age", 123);  
json.Put("name", "张三");  
JSONArray array = new JSONArray();  
array.Put(json);  
Console.WriteLine(array.ToString());  
```

### 解析示例
```
JSONObject json = new JSONObject("{'sex':'男','name':'张三','data':[{'book':'一本书'},{'book':'二本书'}]}");  
Console.WriteLine(json.Get("sex"));  
Console.WriteLine(json.Get("name"));  
JSONArray datas = json.GetJSONArray("data");  
Console.WriteLine(datas.GetJSONObject(0).GetString("book"));  
```
### 下载
[Download](https://github.com/ccwant/ForceJson/releases) 

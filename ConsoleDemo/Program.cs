using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.force.json;

namespace ConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建混合json
            //{"content":[{"sex":"男","age":123,"name":"张三"}]}
            JSONObject rootJson = new JSONObject();

            JSONObject json = new JSONObject();
            json.Put("sex", "男");
            json.Put("age", 123);
            json.Put("name", "张三");
            JSONArray array = new JSONArray();
            array.Put(json);

            rootJson.Put("content", array);

            Console.WriteLine(rootJson.ToString());

            //解析json
            JSONObject test = new JSONObject("{'content':[{'id':1,'phone':'123456','loginTime':'2016-07-12 14:56:53','userName':'admin','password':'admin','registerTime':''}],'result':100,'action':'QUERY_TICKET_REQUEST','msg':'success'}");
            Console.WriteLine(test.ToString());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace com.force.json
{
    /// <summary>
    /// <para>这是一个JSON数组，用来封装常规的JSON对象</para>
    /// <para>使用方法.</para>
    /// <para>  JSONArray myArrayList = new JSONArray();</para>
    /// <para>  JSONObject obj1 = new JSONObject();</para>
    /// <para>  obj1.put(&quot;JSON&quot;, &quot;Hello, World!&quot;);</para>
    /// <para>  myArrayList.put(obj1);</para>
    /// <para>  myArrayList.toString();</para>
    /// <para>---------------</para>
    /// <para>输出字符串:</para>
    /// <para><code>    [{"JSON": "Hello, World"}]</code>.</para>
    /// <para>author: <code>cc_want</code></para>
    /// <para>version: <code>1.0.0</code></para>
    /// </summary>
    public class JSONArray
    {
        private List<Object> myArrayList;
        public JSONArray()
        {
            this.myArrayList = new List<Object>();
        }
        public JSONArray(JSONTokener x)
            : this()
        {

            if (x.NextClean() != '[')
            {
                throw x.SyntaxError("A JSONArray text must start with '['");
            }
            if (x.NextClean() != ']')
            {
                x.Back();
                for (; ; )
                {
                    if (x.NextClean() == ',')
                    {
                        x.Back();
                        this.myArrayList.Add(JSONObject.NULL);
                    }
                    else
                    {
                        x.Back();
                        this.myArrayList.Add(x.NextValue());
                    }
                    switch (x.NextClean())
                    {
                        case ',':
                            if (x.NextClean() == ']')
                            {
                                return;
                            }
                            x.Back();
                            break;
                        case ']':
                            return;
                        default:
                            throw x.SyntaxError("Expected a ',' or ']'");
                    }
                }
            }
        }
        public JSONArray(String source) : this(new JSONTokener(source))
        {

        }
        /// <summary>
        /// 根据索引位置，获取JSONArray
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns></returns>
        public JSONArray GetJSONArray(int index)
        {
            Object obj = this.Get(index);
            if (obj is JSONArray)
            {
                return (JSONArray)obj;
            }
            throw new JSONException("JSONArray[" + index + "] is not a JSONArray.");
        }
        /// <summary>
        /// 根据索引位置，获取JSONObject
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns></returns>
        public JSONObject GetJSONObject(int index)
        {
            Object obj = this.Get(index);
            if (obj is JSONObject)
            {
                return (JSONObject)obj;
            }
            throw new JSONException("JSONArray[" + index + "] is not a JSONObject.");
        }
        /// <summary>
        /// 根据索引位置，获取String
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns></returns>
        public String GetString(int index)
        {
            Object obj = this.Get(index);
            if (obj is String)
            {
                return (String)obj;
            }
            throw new JSONException("JSONArray[" + index + "] not a string.");
        }
        /// <summary>
        /// 根据索引位置，判断对象是否为空
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns></returns>
        public Boolean IsNull(int index)
        {
            return JSONObject.NULL.Equals(this.Opt(index));
        }
        /// <summary>
        /// 根据索引位置，获取对象
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns></returns>
        public Object Get(int index)
        {
            Object obj = this.Opt(index);
            if (obj == null)
            {
                throw new JSONException("JSONArray[" + index + "] not found.");
            }
            return obj;
        }
        /// <summary>
        /// 获取JSONArray中的对象数量
        /// </summary>
        /// <returns>对象数量</returns>
        public int Length()
        {
            return this.myArrayList.Count;
        }
        public List<Object> GetList()
        {
            return this.myArrayList;
        }

        public Object Opt(int index)
        {
            return (index < 0 || index >= this.Length()) ? null : this.myArrayList[index];
        }
        public JSONArray OptJSONArray(int index)
        {
            Object o = this.Opt(index);
            return o is JSONArray ? (JSONArray)o : null;
        }
        public JSONObject OptJSONObject(int index)
        {
            Object o = this.Opt(index);
            return o is JSONObject ? (JSONObject)o : null;
        }
        public String OptString(int index)
        {
            return this.OptString(index, "");
        }
        public String OptString(int index, String defaultValue)
        {
            Object obj = this.Opt(index);
            return JSONObject.NULL.Equals(obj) ? defaultValue : obj
                    .ToString();
        }
        /// <summary>
        /// 向JSONArray中插入布尔型对象
        /// </summary>
        /// <param name="value">Boolean</param>
        /// <returns></returns>
        public JSONArray Put(Boolean value)
        {
            this.Put(value ? true : false);
            return this;
        }
        /// <summary>
        /// 向JSONArray中插入对象
        /// </summary>
        /// <param name="value">对象</param>
        /// <returns>JSONArray</returns>
        public JSONArray Put(Object value)
        {
            this.myArrayList.Add(value);
            return this;
        }
        /// <summary>
        /// 根据索引位置移除对象
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns>移除状态</returns>
        public Boolean Remove(int index)
        {
            if (index >= 0 && index < this.Length())
            {
                this.myArrayList.RemoveAt(index);
                return true;
            }
            return false;
        }
        public String Join(String separator)
        {
            int len = this.Length();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < len; i += 1)
            {
                if (i > 0)
                {
                    sb.Append(separator);
                }
                sb.Append(JSONObject.ValueToString(this.myArrayList[i]));
            }
            return sb.ToString();
        }
        /// <summary>
        /// 将JSONArray中的值取出转换为JSONObject
        /// </summary>
        /// <param name="names">JSONArray</param>
        /// <returns>JSONObject</returns>
        public JSONObject ToJSONObject(JSONArray names)
        {
            if (names == null || names.Length() == 0 || this.Length() == 0)
            {
                return null;
            }
            JSONObject jo = new JSONObject();
            for (int i = 0; i < names.Length(); i += 1)
            {
                jo.Put(names.GetString(i), this.Opt(i));
            }
            return jo;
        }
       
        public String ToString(int indentFactor)
        {
            StringWriter sw = new StringWriter();
            return this.Write(sw, indentFactor, 0).ToString();
        }
        public StringWriter Write(StringWriter writer)
        {
            return this.Write(writer, 0, 0);
        }
        public StringWriter Write(StringWriter writer, int indentFactor, int indent)
        {
            try
            {
                Boolean commanate = false;
                int length = this.Length();
                writer.Write('[');

                if (length == 1)
                {
                    JSONObject.WriteValue(writer, this.myArrayList[0],
                            indentFactor, indent);
                }
                else if (length != 0)
                {
                    int newindent = indent + indentFactor;

                    for (int i = 0; i < length; i += 1)
                    {
                        if (commanate)
                        {
                            writer.Write(',');
                        }
                        if (indentFactor > 0)
                        {
                            writer.Write('\n');
                        }
                        JSONObject.Indent(writer, newindent);
                        JSONObject.WriteValue(writer, this.myArrayList[i],
                                indentFactor, newindent);
                        commanate = true;
                    }
                    if (indentFactor > 0)
                    {
                        writer.Write('\n');
                    }
                    JSONObject.Indent(writer, indent);
                }
                writer.Write(']');
                return writer;
            }
            catch (IOException e)
            {
                throw new JSONException(e.Message);
            }
        }
        /// <summary>
        /// 将JOSNArray转换为字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString()
        {
            try
            {
                return this.ToString(0);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}

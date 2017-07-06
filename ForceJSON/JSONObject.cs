using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Runtime;

namespace com.force.json
{
    /// <summary>
    /// <para>这是一个JSON对象，用来封装常规的JSON字符</para>
    /// <para>使用方法.</para>
    /// <para>JSONObject myString = new JSONObject();</para>
    ///  <para>myString.put(&quot;JSON&quot;, &quot;Hello, World!&quot;);</para>
    /// <para>myString.toString();</para>
    /// <para>---------------</para>
    /// <para>输出字符串:<code>{"JSON": "Hello, World"}</code>.</para>
    /// <para>author: <code>cc_want</code></para>
    /// <para>version: <code>1.0.0</code></para>
    /// </summary>
    public class JSONObject
    {
        /// <summary>
        /// sonobject.null等效值
        /// </summary>
        public class Null
        {
            /// <summary>
            /// 克隆
            /// </summary>
            /// <returns></returns>
            protected Object Clone()
            {
                return this;
            }
            /// <summary>
            /// 判断是否为Null
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override Boolean Equals(Object obj)
            {
                return obj == null || obj == this;
            }

            /// <summary>
            /// 转换成字符串输出
            /// </summary>
            /// <returns></returns>
            public override String ToString()
            {
                return "null";
            }
        }
        Map<string,object> map { get; set; } 
        public static Null NULL = new Null();

        public JSONObject()
        {
            this.map = new Map<string, object>();
        }
        public JSONObject(Map<string, object> map)
        {
            this.map = (map == null ? new Map<string, object>() : map);
        }
        public JSONObject(string s)
            : this(new JSONTokener(s))
        {

        }
        public JSONObject(JSONTokener x)
            : this()
        {
            char c;
            String key;
            if (x.NextClean() != '{')
            {
                throw x.SyntaxError("A JSONObject text must begin with '{'");
            }
            for (; ; )
            {
                c = x.NextClean();
                switch (c)
                {
                    case '\0':
                        throw x.SyntaxError("A JSONObject text must end with '}'");
                    case '}':
                        return;
                    default:
                        x.Back();
                        key = x.NextValue().ToString();
                        break;
                }

                // The key is followed by ':'.

                c = x.NextClean();
                if (c != ':')
                {
                    throw x.SyntaxError("Expected a ':' after a key");
                }
                this.PutOnce(key, x.NextValue());

                // Pairs are separated by ','.

                switch (x.NextClean())
                {
                    case ';':
                    case ',':
                        if (x.NextClean() == '}')
                        {
                            return;
                        }
                        x.Back();
                        break;
                    case '}':
                        return;
                    default:
                        throw x.SyntaxError("Expected a ',' or '}'");
                }
            }
        }
        public static string Quote(string str)
        {
            if ((str == null) || (str.Length == 0))
            {
                return "\"\"";
            }
            char c = '\0';//null

            int len = str.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            char[] s = str.ToCharArray();
            sb.Append('"');
            for (int i = 0; i < len; i++)
            {
                char b = c;
                c = s[i];
                switch (c)
                {
                    case '"':
                    case '\\':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '/':
                        if (b == '<')
                        {
                            sb.Append('\\');
                        }
                        sb.Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    default:
                        if ((c < ' ') || ((c >= '?') && (c < '?')) || ((c >= '?') && (c < '?')))
                        {
                            //String t = "000" + Integer.toHexString(c);
                            //sb.Append("\\u" + t.substring(t.length() - 4));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            sb.Append('"');
            return sb.ToString();
        }
        public static StringWriter Quote(String str, StringWriter w)
        {
            if (str == null || str.Length == 0)
            {
                w.Write("\"\"");
                return w;
            }

            char b;
            char c = (char)0;
            String hhhh;
            int i;
            int len = str.Length;

            w.Write('"');
            for (i = 0; i < len; i += 1)
            {
                b = c;
                c = str.CharAt(i).ToCharArray()[0];
                switch (c)
                {
                    case '\\':
                    case '"':
                        w.Write('\\');
                        w.Write(c);
                        break;
                    case '/':
                        if (b == '<')
                        {
                            w.Write('\\');
                        }
                        w.Write(c);
                        break;
                    case '\b':
                        w.Write("\\b");
                        break;
                    case '\t':
                        w.Write("\\t");
                        break;
                    case '\n':
                        w.Write("\\n");
                        break;
                    case '\f':
                        w.Write("\\f");
                        break;
                    case '\r':
                        w.Write("\\r");
                        break;
                    default:
                        if (c < ' ' || (c >= '\u0080' && c < '\u00a0')
                                || (c >= '\u2000' && c < '\u2100'))
                        {
                            w.Write("\\u");
                            hhhh = c.ToString();
                            w.Write("0000", 0, 4 - hhhh.Length);
                            w.Write(hhhh);
                        }
                        else
                        {
                            w.Write(c);
                        }
                        break;
                }
            }
            w.Write('"');
            return w;
        }
        public JSONObject PutOnce(String key, Object value)
        {
            if ((key != null) && (value != null))
            {
                if (Opt(key) != null)
                {
                    throw new JSONException("Duplicate key \"" + key + "\"");
                }
                Put(key, value);
            }
            return this;
        }
        public Object Opt(String key)
        {
            return key == null ? null : this.map.Get(key);
        }
        public static string NextString(char[] quote)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            for (; ; )
            {
                char c = quote[i];
                i++;
                switch (c)
                {
                    case '\0':
                    case '\n':
                    case '\r':
                        throw new Exception("Unterminated string");
                    case '\\':
                        c = quote[i]; i++;
                        switch (c)
                        {
                            case 'b':
                                sb.Append('\b');
                                break;
                            case 't':
                                sb.Append('\t');
                                break;
                            case 'n':
                                sb.Append('\n');
                                break;
                            case 'f':
                                sb.Append('\f');
                                break;
                            case 'r':
                                sb.Append('\r');
                                break;
                            case 'u':
                                //sb.Append((char)Integer.parseInt(next(4), 16));
                                break;
                            case 'x':
                                // sb.Append((char)Integer.parseInt(next(2), 16));
                                break;
                            case 'c':
                            case 'd':
                            case 'e':
                            case 'g':
                            case 'h':
                            case 'i':
                            case 'j':
                            case 'k':
                            case 'l':
                            case 'm':
                            case 'o':
                            case 'p':
                            case 'q':
                            case 's':
                            case 'v':
                            case 'w':
                            default:
                                sb.Append(c);
                                break;
                        }
                        break;
                    default:

                        return sb.ToString();


                }
            }
        }
        /// <summary>
        /// 获取JOSN中的Int对象
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public int GetInt(string key)
        {
            object obj = this.Get(key);
            try
            {
                return obj is int ? ((int)obj)
                        : Convert.ToInt32((string)obj);
            }
            catch (Exception e)
            {
                throw new JSONException("JSONObject[" + Quote(key) + "] is not an int.");
            }
        }
        /// <summary>
        /// 获取JOSN中的String对象
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public string GetString(string key)
        {
            return Get(key).ToString();
        }
        /// <summary>
        /// 获取JOSN中的JSONArray对象
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>JSONArray</returns>
        public JSONArray GetJSONArray(String key)
        {
            Object obj = this.Get(key);
            if (obj is JSONArray)
            {
                return (JSONArray)obj;
            }
            throw new JSONException("JSONObject[" + Quote(key) + "] is not a JSONArray.");
        }
        /// <summary>
        /// 获取JOSN中的JSONObject对象
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>JSONObject</returns>
        public JSONObject GetJSONObject(String key)
        {
            Object obj = this.Get(key);
            if (obj is JSONObject)
            {
                return (JSONObject)obj;
            }
            throw new JSONException("JSONObject[" + Quote(key) + "] is not a JSONObject.");
        }
        /// <summary>
        /// 根据key获取JOSN对象的值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public object Get(string key)
        {
            object o = Opt(key);
            if (o == null)
            {
                throw new JSONException("JSONObject[" + Quote(key) + "] not found.");
            }
            return o;
        }
        public static object StringToValue(string s)
        {
            if (s.Equals(""))
            {
                return s;
            }
            char b = s.CharAt(0).ToCharArray()[0];
            if (((b >= '0') && (b <= '9')) || (b == '.') || (b == '-') || (b == '+'))
            {
                if (b == '0')
                {
                    if ((s.Length > 2) && ((s.CharAt(1).ToCharArray()[0] == 'x') || (s.CharAt(1).ToCharArray()[0] == 'X')))
                    {
                        try
                        {
                            return Convert.ToInt32(s.Substring(2), 16);
                        }
                        catch (Exception e) { }
                    }
                    else
                    {
                        try
                        {
                            return Convert.ToInt32(s, 8);
                        }
                        catch (Exception e) { }
                    }
                }
                try
                {
                    if ((s.IndexOf('.') > -1) || (s.IndexOf('e') > -1) || (s.IndexOf('E') > -1))
                    {
                        return Double.Parse(s);
                    }


                    return Convert.ToInt32(s);

                }
                catch (Exception f) { }
            }
            return s;
        }
        public static string ValueToString(object value)
        {
            if ((value == null) || (value.Equals(null)))
            {
                return "null";
            }
            if (value is JSONString)
            {
                object obj;
                try
                {
                    obj = ((JSONString)value).ToJSONString();
                }
                catch (Exception e)
                {
                    throw new JSONException(e.Message);
                }
                if (obj is String)
                {
                    return (String)obj;
                }
                throw new JSONException("Bad value from toJSONString: " + obj);
            }
            if ((value is int))
            {
                return NumberToString((int)value);
            }
            if (value is Boolean)
            {
                return value.ToString();
            }
            if (value is JSONObject)
            {
                return ((JSONObject)value).ToString();
            }
            if (value is JSONArray)
            {
                return ((JSONArray)value).ToString();
            }
            return Quote(value.ToString());
        }
        public static string NumberToString(int n)
        {
            string s = n.ToString();
            if ((s.IndexOf('.') > 0) && (s.IndexOf('e') < 0)
                    && (s.IndexOf('E') < 0))
            {
                while (s.EndsWith("0"))
                {
                    s = s.Substring(0, s.Length - 1);
                }
                if (s.EndsWith("."))
                {
                    s = s.Substring(0, s.Length - 1);
                }
            }
            return s;
        }
        /// <summary>
        /// 获取JSON对象的数量
        /// </summary>
        /// <returns>数量</returns>
        public int Length()
        {
            return this.map.GetSize();
        }
        public static void Indent(StringWriter writer, int indent)
        {
            for (int i = 0; i < indent; i += 1)
            {
                writer.Write(' ');
            }
        }
        public static StringWriter WriteValue(StringWriter writer, Object value,
            int indentFactor, int indent)
        {
            if (value == null || value.Equals(null))
            {
                writer.Write("null");
            }
            else if (value is JSONObject)
            {
                ((JSONObject)value).Write(writer, indentFactor, indent);
            }
            else if (value is JSONArray)
            {
                ((JSONArray)value).Write(writer, indentFactor, indent);
            }
            else if (value is int)
            {
                writer.Write(NumberToString((int)value));
            }
            else if (value is Boolean)
            {
                writer.Write(value.ToString());
            }
            else if (value is JSONString)
            {
                Object o;
                try
                {
                    o = ((JSONString)value).ToJSONString();
                }
                catch (Exception e)
                {
                    throw new JSONException(e.Message);
                }
                writer.Write(o != null ? o.ToString() : Quote(value.ToString()));
            }
            else
            {
                Quote(value.ToString(), writer);
            }
            return writer;
        }
        public StringWriter Write(StringWriter writer, int indentFactor, int inden)
        {
            try
            {

                Boolean commanate = false;
                int length = this.Length();
                writer.Write('{');

                if (length == 1)
                {

                    List<object> keys = map.Keys.ToList<object>();
                    Object key = keys[0];
                    writer.Write(Quote(key.ToString()));
                    writer.Write(':');
                    if (indentFactor > 0)
                    {
                        writer.Write(' ');
                    }
                    WriteValue(writer, this.map.Get(key.ToString()), indentFactor, inden);
                }
                else if (length != 0)
                {
                    int newindent = inden + indentFactor;
                    foreach (var a in map)
                    {
                        Object key = a.Key;
                        if (commanate)
                        {
                            writer.Write(',');
                        }
                        if (indentFactor > 0)
                        {
                            writer.Write('\n');
                        }
                        Indent(writer, newindent);
                        writer.Write(Quote(key.ToString()));
                        writer.Write(':');
                        if (indentFactor > 0)
                        {
                            writer.Write(' ');
                        }
                        WriteValue(writer, this.map.Get(key.ToString()), indentFactor, newindent);
                        commanate = true;
                    }
                    if (indentFactor > 0)
                    {
                        writer.Write('\n');
                    }
                    Indent(writer, inden);
                }
                writer.Write('}');
                return writer;
            }
            catch (IOException exception)
            {
                throw new JSONException(exception.Message);
            }
        }
        /// <summary>
        /// 添加JSON对象
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>JSONObject</returns>
        public JSONObject Put(string key, object value)
        {
            if (key == null)
            {
                throw new JSONException("Null key.");
            }
            if (value != null)
            {
                this.map.Put(key, value);
            }
            else
            {
                Remove(key);
            }
            return this;
        }
        /// <summary>
        /// 移除JSON对象
        /// </summary>
        /// <param name="key">键</param>
        public void Remove(string key)
        {
            this.map.Remove(key);
        }
        /// <summary>
        /// 获取所有的key
        /// </summary>
        /// <returns></returns>
        public List<string> GetKeys()
        {
            return this.map.GetKeys();
        }
        /// <summary>
        /// 获取所有的value
        /// </summary>
        /// <returns></returns>
        public List<object> GetValues()
        {
            return this.map.GetValues();
        }
        /// <summary>
        /// 将JSONObject转换为字符串
        /// </summary>
        /// <returns>JSON字符串</returns>
        public override string ToString()
        {
            try
            {

                string sb = "{";

                foreach (var a in map)
                {

                    if (sb.Length > 1)
                    {
                        sb += ',';
                    }
                    sb += (Quote(a.Key.ToString()));
                    sb += (':');
                    sb += (ValueToString(this.map.Get(a.Key)));
                }

                sb += ('}');
                return sb;
            }
            catch (Exception e)
            {
            }
            return null;
        }
    }

    public static class CharAtExtention
    {
        public static string CharAt(this string s, int index)
        {
            if ((index >= s.Length) || (index < 0))
                return "";
            return s.Substring(index, 1);
        }
    }

}

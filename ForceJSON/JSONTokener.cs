using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace com.force.json
{
    public class JSONTokener
    {
        private int index;
        private StringReader reader;
        private char lastChar;
        private Boolean useLastChar;
        public JSONTokener(StringReader reader)
        {
            this.reader = reader;

            this.useLastChar = false;
            this.index = 0;
        }
        public JSONTokener(String s): this(new StringReader(s))
        {

        }
        public void Back()
        {
            if ((this.useLastChar) || (this.index <= 0))
            {
                throw new JSONException("Stepping back two steps is not supported");
            }
            this.index -= 1;
            this.useLastChar = true;
        }
        public static int Dehexchar(char c)
        {
            if ((c >= '0') && (c <= '9'))
            {
                return c - '0';
            }
            if ((c >= 'A') && (c <= 'F'))
            {
                return c - '7';
            }
            if ((c >= 'a') && (c <= 'f'))
            {
                return c - 'W';
            }
            return -1;
        }
        public Boolean More()
        {
            char nextChar = Next();
            if (nextChar == 0)
            {
                return false;
            }
            Back();
            return true;
        }

        public char Next()
        {
            if (this.useLastChar)
            {
                this.useLastChar = false;
                if (this.lastChar != 0)
                {
                    this.index += 1;
                }
                return this.lastChar;
            }
            int c;
            try
            {
                c = this.reader.Read();
            }
            catch (IOException exc)
            {
                throw new JSONException("IOException;" + exc.Message);
            }
            if (c <= 0)
            {
                this.lastChar = '\0';
                return '\0';
            }
            this.index += 1;
            this.lastChar = ((char)c);
            return this.lastChar;
        }

        public char next(char c)
        {
            char n = Next();
            if (n != c)
            {
                throw SyntaxError("Expected '" + c + "' and instead saw '" + n + "'");
            }
            return n;
        }

        public String Next(int n)
        {
            if (n == 0)
            {
                return "";
            }
            char[] buffer = new char[n];
            int pos = 0;
            if (this.useLastChar)
            {
                this.useLastChar = false;
                buffer[0] = this.lastChar;
                pos = 1;
            }
            try
            {
                int len;
                while ((pos < n) && ((len = this.reader.Read(buffer, pos, n - pos)) != -1))
                {
                    pos += len;
                }
            }
            catch (IOException exc)
            {
                throw new JSONException("IOException:" + exc.Message);
            }
            this.index += pos;
            if (pos < n)
            {
                throw SyntaxError("Substring bounds error");
            }
            this.lastChar = buffer[(n - 1)];
            return new String(buffer);
        }
        public char NextClean()
        {
            for (; ; )
            {
                char c = Next();
                if ((c == 0) || (c > ' '))
                {
                    return c;
                }
            }
        }
        public String NextString(char quote)
        {
            StringBuilder sb = new StringBuilder();

            char c;
            for (; ; )
            {
                c = this.Next();
                switch (c)
                {
                    case '\0':
                    case '\n':
                    case '\r':
                        throw this.SyntaxError("Unterminated string");
                    case '\\':
                        c = this.Next();
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
                                sb.Append((char)Convert.ToInt32(this.Next(4), 16));
                                break;
                            case '"':
                            case '\'':
                            case '\\':
                            case '/':
                                sb.Append(c);
                                break;
                            default:
                                throw this.SyntaxError("Illegal escape.");
                        }
                        break;
                    default:
                        if (c == quote)
                        {
                            return sb.ToString();
                        }
                        sb.Append(c);
                        break;
                }
            }
        }

        public String NextTo(char d)
        {
            StringBuilder sb = new StringBuilder();
            for (; ; )
            {
                char c = Next();
                if ((c == d) || (c == 0) || (c == '\n') || (c == '\r'))
                {
                    if (c != 0)
                    {
                        Back();
                    }
                    return sb.ToString().Trim();
                }
                sb.Append(c);
            }
        }
        public String NextTo(String delimiters)
        {
            StringBuilder sb = new StringBuilder();
            for (; ; )
            {
                char c = Next();
                if ((delimiters.IndexOf(c) >= 0) || (c == 0) || (c == '\n') || (c == '\r'))
                {
                    if (c != 0)
                    {
                        Back();
                    }
                    return sb.ToString().Trim();
                }
                sb.Append(c);
            }
        }
        public Object NextValue()
        {
            char c = NextClean();
            switch (c)
            {
                case '"':
                case '\'':
                    return NextString(c);
                case '{':
                    Back();
                    return new JSONObject(this);
                case '(':
                case '[':
                    Back();
                    return new JSONArray(this);
            }
            StringBuilder sb = new StringBuilder();
            while ((c >= ' ') && (",:]}/\\\"[{;=#".IndexOf(c) < 0))
            {
                sb.Append(c);
                c = Next();
            }
            Back();

            String s = sb.ToString().Trim();
            if (s.Equals(""))
            {
                throw SyntaxError("Missing value");
            }
            return JSONObject.StringToValue(s);
        }
        public char SkipTo(char to)
        {
            char c;
            try
            {
                int startIndex = this.index;
                // this.reader.Mark(int.MaxValue);
                do
                {
                    c = Next();
                    if (c == 0)
                    {
                        //this.reader.Reset()
                        this.index = startIndex;
                        return c;
                    }
                } while (c != to);
            }
            catch (IOException exc)
            {
                throw new JSONException("IOException:" + exc.Message);
            }
            Back();
            return c;
        }
        public JSONException SyntaxError(String message)
        {
            return new JSONException(message + ToString());
        }

        public override String ToString()
        {
            return " at character " + this.index;
        }

    }
}

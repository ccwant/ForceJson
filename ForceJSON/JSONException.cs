using System;
using System.Collections.Generic;
using System.Text;

namespace com.force.json
{
    public class JSONException : ApplicationException
    {
        public JSONException()
        {

        }
        public JSONException(string message)
        {
            Console.WriteLine("JSONException=" + message);
        }

        public JSONException(string message, Exception inner): base(message, inner)
        {
        }
    }
}

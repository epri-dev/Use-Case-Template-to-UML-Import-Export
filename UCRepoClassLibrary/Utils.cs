using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;

namespace UCRepoClassLibrary
{
    public static class Utils
    {
        public static string GetShortText(string text)
        {
            string vowels = "aeiou";
            text = new string(text.Where(c => !vowels.Contains(c)).ToArray());

            var subs = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string result = String.Empty;

            foreach (var sub in subs)
            {
                if (result != String.Empty)
                    result += "-";

                result += sub.Substring(0, Math.Min(sub.Length, 3));
            }

            return result;
        }

        public static string getLocation(this XElement element)
        {
            if (element == null)
                return "";

            var location = element.Attribute("location");
            if (location != null)
            {
                //if (!String.IsNullOrEmpty(location.Value))
                return location.Value;
            }

            return getLocation(element.Parent);
        }
    }

    public class EpriException : Exception
    {
        public string location { get; set; }

        public EpriException(string location)
        {
            this.location = location;
        }

        public EpriException(string message, string location)
            : base(message)
        {
            this.location = location;
        }

        public EpriException(string message, Exception inner, string location)
            : base(message, inner)
        {
            this.location = location;
        }

        public EpriException(SerializationInfo info, StreamingContext context, string location)
            : base(info, context)
        {
            this.location = location;
        }


    }
}

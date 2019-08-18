using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace TvShows.Services
{
    public class UtilService
    {
        public UtilService() { }

        public string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
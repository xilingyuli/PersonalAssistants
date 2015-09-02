using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace PersonalAssistants
{
    class NetInfoCatcher
    {
        public static string getInfo(int y,int m,int d)
        {
            Encoding myEncoding = Encoding.GetEncoding("gb2312");
            string result;

            HttpWebRequest myRequest =
            (HttpWebRequest)WebRequest.Create("http://www.laohuangli.net/"+y+"/"+y+"-"+m+"-"+d+".html");
            myRequest.Method = "GET";
            try
            {
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), myEncoding);
                result = reader.ReadToEnd();
                int status = (int)myResponse.StatusCode;
                reader.Close();
            }
            catch (System.Net.WebException e)
            {
                result = e.Message;
            }
            return result;
        }

        public static string[] InfoPicker(int y,int m,int d)
        {
            string s1 = "", s2 = "";
            try
            {
                string[] lines = getInfo(y, m, d).Split(new char[] { '\n' });
                for (int i = 0; i < lines.Length; i++)
                    if (lines[i].IndexOf("【今日老黄历所宜】") != -1)
                    {
                        s1 = (lines[i + 15]).Replace("<br/>", "　");
                    }
                    else if (lines[i].IndexOf("【今日老黄历所忌】") != -1)
                    {
                        s2 = (lines[i + 15]).Replace("<br/>", "　");
                    }
                if (s1.IndexOf('【') != -1)
                    s1 = s1.Substring(0, s1.IndexOf('【')) + s1.Substring(s1.IndexOf('】') + 1);
                if (s2.IndexOf('【') != -1)
                    s2 = s2.Substring(0, s2.IndexOf('【')) + s2.Substring(s2.IndexOf('】') + 1);
                s1 = "该日所宜\n\n" + s1.Substring(s1.IndexOf('>') + 1, s1.LastIndexOf('<') - s1.IndexOf('>') - 1);
                s2 = "该日所忌\n\n" + s2.Substring(s2.IndexOf('>') + 1, s2.LastIndexOf('<') - s2.IndexOf('>') - 1);
            }
            catch(Exception o)
            { ;}
            return new string[]{s1,s2};
        }
    }
}

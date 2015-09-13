using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Windows.Forms;
using PersonalAssistants;

namespace PersonalAssistants{

    enum ApproDayType:int
    {
        Yesterdayminus2=-3, Yesterdayminus1=-2, Yesterday=-1,Today=0,
        Tomorrow=1, Tomorrowplus1=2, Tomorrowplus2=3,None
    };

    class SentenceParse {
        private static string API_KEY = "93T7C804oNouYjRP0lonJJvhCFNJtqdyWWIDegTo";
        private CalendarExchange CE;
        private string parsingSentence;
        public string[] splitResult;
        public SentenceParse() { }
        public SentenceParse(string s) {
            splitResult = getSplit(s);
            parsingSentence = s;
            CE = new CalendarExchange();
        }
        public static string[] getSplit(string input) {
            Encoding myEncoding = Encoding.GetEncoding("utf-8");  
            string result;

            HttpWebRequest myRequest =
            (HttpWebRequest)WebRequest.Create("http://ltpapi.voicecloud.cn/analysis/?api_key=" + API_KEY + "&text=" + HttpUtility.UrlEncode(input,myEncoding) + "&pattern=ws&format=plain");
            myRequest.Method = "GET";
            try {
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), myEncoding);
                result = reader.ReadToEnd();
                int status = (int)myResponse.StatusCode;
                reader.Close();
            } catch (System.Net.WebException e) {
                result = e.Message;
            }
            string[] tmp=result.Split(' ');
            List<String> ls=new List<String>();
            for (int i = 0; i < tmp.Length; i++){
                string[] t2 = tmp[i].Split('月');
                for (int j = 0; j < t2.Length - 1; j++)
                    ls.Add(t2[j] + "月");
                if(t2[t2.Length - 1].Length>0)
                    ls.Add(t2[t2.Length - 1]);
            }
            return ls.ToArray();
        }
        private string checkYearString(string s)
        {
            if (s.EndsWith("年") && CE.getYear(s.Substring(0, s.Length - 1)) > 0)
            {
                return s.Substring(0, s.Length - 1);
            }
            return null;
        }
        private string checkMonthString(string prev,string s)
        {
            if (s.EndsWith("月") && CE.getYinMonth("2015", s.Substring(0, s.Length - 1)) > 0)
            {
                if (prev == "闰")
                    return "闰" + s.Substring(0, s.Length - 1);
                else return s.Substring(0, s.Length - 1);
            }
            if (s.StartsWith("大年"))
            {
                int ans = CE.getYinMonth(getYear() != null ? getYear() : DateTime.Now.Year.ToString(), s);
                return ans.ToString();
            }
            return null;
        }
        private string checkDayString(string prev,string s)
        {
            if ((s.EndsWith("日") || s.EndsWith("号")) && CE.getDay(s.Substring(0,s.Length - 1)) > 0)
            {
                return s.Substring(0, s.Length - 1);
            }
            if (prev.EndsWith("月") && CE.getDay(s) > 0)
            {
                return s;
            }
            if (s.StartsWith("大年") && CE.getDay(s.Substring(2)) > 0)
            {
                return s.Substring(2);
            }
            return null;
        }
        public string getYear()
        {
            if (splitResult == null) return null;
            int len = splitResult.Length;
            for (int i = 0; i < len; i++) {
                string tmp = checkYearString(splitResult[i]);
                if (tmp != null)
                    return tmp;
            }
            return null;
        }
        public string getMonth()
        {
            if (splitResult == null) return null;
            int len = splitResult.Length;
            for (int i = 0; i < len; i++)
            {
                string tmp = checkMonthString(i > 0 ? splitResult[i - 1] : " ", splitResult[i]);
                if (tmp != null)
                    return tmp;
            }
            return null;
        }
        public string getDay()
        {
            if (splitResult == null) return null;
            int len = splitResult.Length;
            for (int i = 0; i < len; i++)
            {
                string tmp = checkDayString(i > 0 ? splitResult[i - 1] : " ", splitResult[i]);
                if (tmp != null)
                    return tmp;
            }
            return null;
        }

        public bool hasExactDate()
        {
            return getDay() != null;
        }

        public int ApproDate()
        {
            string[] ApproName = new string[] { "大前天","前天","昨天","今天", "明天", "后天", "大后天" };
            int ans = int.MinValue;
            for (int i = 0; i < splitResult.Length; i++){
                for (int j = 0; j < 7; j++)
                    if (splitResult[i] == ApproName[j])
                    {
                        if (ans == int.MinValue) ans = j - 3;
                        else ans += j - 3;
                        break;
                    }
            }
            return ans;
        }

        private bool hasYinDate()
        {
            string[] keyWord = new string[]{"正月","腊月","大年",
                "廿一","廿二","廿三","廿四","廿五","廿六","廿七","廿八","廿九",
                "初一","初二","初三","初四","初五","初六","初七","初八","初九","初十"};
            foreach (string i in keyWord)
            {
                if (parsingSentence.IndexOf(i) > 0) return true;
            }
            return false;
        }

        public bool isExpectYang()
        {
            string[] YinName = new string[] { "阴历","农历" };
            string[] YangName = new string[] { "阳历", "公历" };
            bool dayshown=false;
            bool prevYin = false,prevYang=false;
            for (int i = 0; i < splitResult.Length; i++)
            {
                if (splitResult[i] == "阴历" || splitResult[i] == "农历")
                {
                    if (dayshown) return false;
                    prevYin = true;
                }
                if (splitResult[i] == "阳历" || splitResult[i] == "公历")
                {
                    if (dayshown) return true;
                    prevYang = true;
                }
                if (checkDayString(i > 0 ? splitResult[i - 1] : " ", splitResult[i]) != null)
                {
                    dayshown = true;
                    if (prevYin) return true;
                }
            }
            if (!dayshown)
                return !prevYin;
            else if (hasYinDate()) return true;
            else return false;
        }
    }
}

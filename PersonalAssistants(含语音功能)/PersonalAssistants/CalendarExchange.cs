using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace PersonalAssistants
{
    class CalendarExchange
    {
        static private ChineseLunisolarCalendar calender;
        static private DateTime date;
        static private string[] MonthName = 
        { "*", "正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "腊" };
        static private string[] MonthNameYang = 
        { "*", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二" };
        static private string[] DayName = 
            {"*","初一","初二","初三","初四","初五",
             "初六","初七","初八","初九","初十",
             "十一","十二","十三","十四","十五",
             "十六","十七","十八","十九","二十",
             "廿一","廿二","廿三","廿四","廿五",      
             "廿六","廿七","廿八","廿九","三十","三十一"};
        static private string[] DayNameYang = 
            {"*","一","二","三","四","五",
             "六","七","八","九","十",
             "十一","十二","十三","十四","十五",
             "十六","十七","十八","十九","二十",
             "二十一","二十二","二十三","二十四","二十五",      
             "二十六","二十七","二十八","二十九","三十","三十一"};
        static private string[] NumInChinese = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
        public CalendarExchange()
        {
            calender = new ChineseLunisolarCalendar();
        }
        public int[] getYangDate()
        {
            return new int[] { date.Year, date.Month, date.Day };
        }
        public string getDate(int move, bool isWantYang)
        {
            date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            date = date.AddDays(move);
            if (isWantYang)
                return getYinToYang();
            else
                return getYangToYin();
        }
        public string setYangDate(int year,int month,int day)
        {
            try
            {
                if (year != 0 && month != 0 && day != 0)
                    date = new DateTime(year, month, day);
                else if (year == 0 && month != 0 && day != 0)
                    date = new DateTime(DateTime.Now.Year, month, day);
                else if (year == 0 && month == 0 && day != 0)
                    date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day);
                else
                    return "请重新输入包含合法日期的指令";
                return getYangToYin();
            }
            catch(Exception o)
            {
                return "请重新输入包含合法日期的指令";
            }
        }
        public string getYangToYin()
        {
            if (date == null)
                return "请输入指令";
            try
            {
                string x = "农历 ： ";
                x += calender.GetYear(date) + "年";
                int leapMonth = calender.GetLeapMonth(calender.GetYear(date));
                int month = calender.GetMonth(date);
                if (leapMonth > 0)
                {
                    if (leapMonth == month)
                        x += "闰" + MonthName[month - 1];
                    else if (month > leapMonth)
                        x += MonthName[month - 1];
                    else x += MonthName[month];
                }
                else
                {
                    x += MonthName[calender.GetMonth(date)];
                }
                x += "月";
                x += DayName[calender.GetDayOfMonth(date)];
                return x;
            }
            catch(Exception o)
            {
                return "不支持此转换";
            }
        }
        public string setYinDate(int year,int month,int day)
        {
            try
            {
                if(year==-1||month==-1||day==-1)
                    return "请重新输入包含合法日期的指令";
                if (year != 0 && month != 0 && day != 0)
                    date = new DateTime(year, month, day, calender);
                else if (year == 0 && month != 0 && day != 0)
                    date = new DateTime(calender.GetYear(DateTime.Now), month, day, calender);
                else if (year == 0 && month == 0 && day != 0)
                    date = new DateTime(calender.GetYear(DateTime.Now), calender.GetMonth(DateTime.Now), day, calender);
                else
                    return "请重新输入包含合法日期的指令";
                return getYinToYang();
            }
            catch(Exception o)
            {
                return "请重新输入包含合法日期的指令";
            }
        }
        public string getYinToYang()
        {
            if (date == null)
                return "请输入指令";
            string x = "阳历 ： ";
            x += date.Year + "年";
            x += date.Month + "月";
            x += date.Day + "日";
            return x;
        }
        public int getYear(String ystr)
        {
            if (ystr == null)
                return 0;
            int ans = 0,len=ystr.Length;
            for (int i = 0; i < len; i++)
            {
                ans *= 10;
                int j;
                for (j = 0; j <= 9; j++)
                {
                    if (ystr[i] == NumInChinese[j][0] || ystr[i] == j.ToString()[0])
                    {
                        ans += j;
                        break;
                    }
                }
                if(j==10)
                {
                    return -1;
                }
            }
            return ans;
        }
        public int getYinMonth(String ystr, string m)
        {
            if (m == null)
                return 0;
            try
            {
                int y = getYear(ystr);
                int leapMonth;
                if (y != 0)
                    leapMonth = calender.GetLeapMonth(y);
                else
                    leapMonth = calender.GetLeapMonth(calender.GetYear(DateTime.Now));
                if (m == "大年三十")
                {
                    return leapMonth > 0 ? 13 : 12;
                }
                string trueMonth;
                int ans = 0;
                if (m[0] == '闰')
                {
                    trueMonth = m.Substring(1);
                    ans = 1;
                }
                else
                {
                    trueMonth = m;
                }
                if (trueMonth.StartsWith("大年"))
                    ans = 1;
                else
                {
                    for (int j = 1; j <= 12; j++)
                        if (trueMonth == MonthName[j] || trueMonth == j.ToString() || trueMonth == MonthNameYang[j])
                        {
                            ans += j;
                            if (leapMonth >= 1 && j > leapMonth)
                                ans++;
                            break;
                        }
                }
                return ans;
            }
            catch(Exception o)
            {
                return -1;
            }
        }
        public int getYangMonth(string m)
        {
            if (m == null)
                return 0;
            for (int j = 1; j <= 12; j++)
                if (m == j.ToString() || m == MonthNameYang[j])
                {
                    return j;
                }
            return -1;
        }
        public int getDay(string d)
        {
            if (d == null)
                return 0;
            for (int i = 1; i <= 31; i++)
                if (d == DayName[i]||d == i.ToString()||d == DayNameYang[i])
                    return i;
            return -1;
        }
    }
}

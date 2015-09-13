using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PersonalAssistants
{
    public partial class Form1 : Form
    {
        CalendarExchange ce;
        public Form1()
        {
            InitializeComponent();
            ce = new CalendarExchange();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.label1.Text = "请稍候";
            this.label2.Text = "请稍候";
            this.label3.Text = "请稍候";
            this.Refresh();
            SentenceParse sp = new SentenceParse(this.textBox1.Text);
            if (!sp.hasExactDate()&&int.MinValue==sp.ApproDate())
                this.label1.Text = "请重新输入";
            else if (!sp.hasExactDate())
            {
                this.label1.Text = ce.getDate((int)sp.ApproDate(), sp.isExpectYang());
                this.label2.Text = NetInfoCatcher.InfoPicker(ce.getYangDate()[0], ce.getYangDate()[1], ce.getYangDate()[2])[0];
                this.label3.Text = NetInfoCatcher.InfoPicker(ce.getYangDate()[0], ce.getYangDate()[1], ce.getYangDate()[2])[1];
            }
            else if (sp.isExpectYang())
            {
                int y = ce.getYear(sp.getYear());
                int m = ce.getYinMonth(sp.getYear(), sp.getMonth());
                int d = ce.getDay(sp.getDay());
                this.label1.Text = ce.setYinDate(y, m, d);
                this.label2.Text = NetInfoCatcher.InfoPicker(ce.getYangDate()[0], ce.getYangDate()[1],ce.getYangDate()[2])[0];
                this.label3.Text = NetInfoCatcher.InfoPicker(ce.getYangDate()[0], ce.getYangDate()[1], ce.getYangDate()[2])[1];
            }
            else if (!sp.isExpectYang())
            {
                int y = ce.getYear(sp.getYear());
                int m = ce.getYangMonth(sp.getMonth());
                int d = ce.getDay(sp.getDay());
                this.label1.Text = ce.setYangDate(y, m, d);
                this.label2.Text = NetInfoCatcher.InfoPicker(ce.getYangDate()[0], ce.getYangDate()[1], ce.getYangDate()[2])[0];
                this.label3.Text = NetInfoCatcher.InfoPicker(ce.getYangDate()[0], ce.getYangDate()[1], ce.getYangDate()[2])[1];
            }
            else
                this.label1.Text = "请重新输入";
        }

        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int mciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);

        private bool sndstate = false;

        private void btnSnd_Click(object sender, EventArgs e)
        {
            string sndAddress = "c:\\bop\\recognize_snd.wav";
            if (sndstate == false)
            {
                sndstate = true;
                btnSnd.Text = "Stop";
                mciSendString("open new Type waveaudio Alias recsound", "", 0, 0);
                mciSendString("record recsound", "", 0, 0);
            }
            else
            {
                mciSendString("save recsound "+sndAddress, "", 0, 0);
                mciSendString("close recsound ", "", 0, 0);
                sndstate = false;
                btnSnd.Text = "Record";
                textBox1.Text = "Recognizeing, Please Wait.";
                this.Refresh();
                string ans=VoiceRecognize.SendRequest(sndAddress);
                textBox1.Text = ans;
                if (!ans.StartsWith("Error") && ans.Length > 0)
                {
                    button1_Click(sender, e);
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BruteEngine
{
    public partial class CaptchaForm : Form
    {
        public CaptchaForm(Stream str )
        {
            InitializeComponent();
            ms = (MemoryStream)str;
            pictureBox1.Image = Image.FromStream(str);
            //Text = App.CapCount.ToString();
            //App.CapCount++;
        }
        MemoryStream ms;
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                Close();
        }

        private void CaptchaForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (!Directory.Exists("captcha"))
                    Directory.CreateDirectory("captcha");
                if (textBox1.Text.Length > 3)
                    File.WriteAllBytes("captcha\\" + textBox1.Text + ".png", ms.ToArray());
            }
            catch { }
        }
    }
}

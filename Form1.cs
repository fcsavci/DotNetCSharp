using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LicenseClass lc = new LicenseClass();
            //textBox1.Text= lc.LizenzKey;
            //lc.LizenzKey = "abcdefghijklmnoöprstuüwvyz";
            
            textBox2.Text = lc.SwapHex(textBox1.Text);

            textBox4.Text = lc.getMd5( textBox3.Text);

            textBox6.Text = lc.GetSHA1(textBox5.Text);

        }
    }
}

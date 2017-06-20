using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YoutubePlayground.UI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            var result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private Boolean isStarted = false;
        private void button1_Click(object sender, EventArgs e)
        {
            if (isStarted)
            {
                Controller.GetController().VFW.Pause();
                button1.Text = "Pause Monitoring";
            }
            else
            {
                Controller.GetController().VFW.SetPath(textBox1.Text);
                Controller.GetController().VFW.Start();
                button1.Text = "Start Monitoring";
            }
        }
    }
}

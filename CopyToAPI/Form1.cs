using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO; 

namespace CopyToAPI
{
    public partial class Form1 : Form
    {
        public string mcsfilename = @"";
        private DateTime appstarted;
        public static MCS_API.Source thisSource = new MCS_API.Source() { Id = 1, MachineName = "UNKNOWN", Name = "UNKNOWN", IPAddress="99" }; 

        int filesSelCnt = 0; 
        public Form1()
        {
            InitializeComponent();
        }
        private int postCount = 0; 
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (checkBox1.Checked == true)
                {
                    postCount++; 
                    MCS_API.Helpers.dlayer mcs = new MCS_API.Helpers.dlayer();

                    string result = mcs.callPost();
                    if (postCount == 1)
                    {
                        textBox1.Text = textBox1.Text + Environment.NewLine + "POST Request Sent...File Synced(" + result + ")";
                        textBox1.Text = textBox1.Text + Environment.NewLine + "      - Timestamp " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " DocNumber : " + MCS_API.Helpers.dlayer.DocNumber.ToString();
                    }
                    
                }
                
              
            }
            catch (Exception err)
            {
                textBox1.Text = textBox1.Text + Environment.NewLine + "Application Error: " + err.Message;    
            }

            StatusVal_Label.Text = "Active";

            int lines = textBox1.Lines.Length;

            if (lines > 2000)
            {
                textBox1.Clear();
                textBox1.Text = "ApplicationStarted " + appstarted.ToShortDateString() + " " + appstarted.ToShortTimeString();
                textBox1.Text = textBox1.Text + Environment.NewLine + "Target API: " + MCS_API.Helpers.dlayer.apiaddress + Environment.NewLine + "Application Status: enabled";
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string[] filenames = openFileDialog1.FileNames;

            if (filenames.Count() > 0)
            {
                MCS_API.Helpers.dlayer.mcsaddress = filenames[0];
                label2.Text = filenames[0];
                filesSelCnt++;
                textBox1.Text = textBox1.Text +  Environment.NewLine + "File Selected -> " + MCS_API.Helpers.dlayer.mcsaddress + " " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToShortTimeString();
                if (System.IO.File.Exists(filenames[0]))
                {
                    timer1.Enabled = true;
                    checkBox1.Checked = true;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AddAppToStartUp();  
            setSource(); 
            appstarted = DateTime.Now;
            textBox1.Text = "ApplicationStarted " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
            textBox1.Text = textBox1.Text + Environment.NewLine + "Target API: " + MCS_API.Helpers.dlayer.apiaddress + Environment.NewLine + "Application Status: disabled"; 
        }

        private void AddAppToStartUp()
        {
            try
            {
                string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                string exPath = Application.ExecutablePath.ToString();
                System.IO.File.Copy(exPath, startupPath + "//CopyToAPI.EXE", true);
            } catch (Exception e)
            {
                textBox1.Text = textBox1.Text + Environment.NewLine + "Error copying app exe to startup folder";
            }
            
        }
        private async Task setSource()
        {
            MCS_API.Helpers.dlayer dlayer = new MCS_API.Helpers.dlayer();

            thisSource = await dlayer.getSourceId();

            if (thisSource != null && thisSource.Id > 1)
            {
                MCS_API.Helpers.dlayer.Setdocnumber();

                if (MCS_API.Helpers.dlayer.mcsaddress != null && MCS_API.Helpers.dlayer.mcsaddress.Length > 0 && System.IO.File.Exists(MCS_API.Helpers.dlayer.mcsaddress))
                {
                    timer1.Enabled = true;
                    checkBox1.Checked = true;
                }
                textBox1.Text = textBox1.Text + Environment.NewLine + "SourceId: " + thisSource.Id.ToString() + " source initialized";
            } else
            {
                textBox1.Text = textBox1.Text + Environment.NewLine + "Source not Set";
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                if (System.IO.File.Exists(MCS_API.Helpers.dlayer.mcsaddress))
                {
                    timer1.Enabled = true;
                    checkBox1.Text = "enabled";
                    StatusVal_Label.Text = "Active";
                    textBox1.Text = textBox1.Text + Environment.NewLine + "Transfer enabled " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
                } else
                {
                    MessageBox.Show("File Not Selected");
                }
            }
            else if (checkBox1.Checked == false)
            {
                timer1.Enabled = false;
                checkBox1.Text = "disabled";
                StatusVal_Label.Text = "Down";
                textBox1.Text = textBox1.Text + Environment.NewLine + "Transfer disabled " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Every Minute")
            {
                timer1.Interval = 60000;
            } else if (comboBox1.Text == "Every 15 Minutes")
            {
                timer1.Interval = 900000;
            } else if (comboBox1.Text == "Every 30 Minutes")
            {
                timer1.Interval = 1800000;
            } else if (comboBox1.Text == "Every 45 Minutes")
            {
                timer1.Interval = 2700000;
            } else if (comboBox1.Text == "Every Hour")
            {
                timer1.Interval = 3600000;
            }
        }
    }
}

using Instagram_Email_Scrape.Class;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Instagram_Email_Scrape.Class.CCsv;

namespace Instagram_Email_Scrape
{
    public partial class FrmMain : Form
    {
        List<string> list = new List<string>();
        private CFormControl f = new CFormControl();
        private CWebBrowser wb = new CWebBrowser();

        private ThreadStart emailThread;
        private Thread emailThread_Thread;
        private IWebDriver navigator;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            btn_Start.Visible = false;
            btn_Stop.Visible = true;
            emailThread = new ThreadStart(StartThread);
            emailThread_Thread = new Thread(emailThread);
            emailThread_Thread.Start();
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void Stop()
        {
            UpdateData();
            btn_Start.Visible = true;
            btn_Stop.Visible = false;
            try { navigator.Quit(); } catch (Exception) { }
            try { emailThread_Thread.Abort(); } catch (Exception) { }
        }
        private void StartThread()
        {
            navigator = wb.googleChrome();
            try
            {
                for (int i = 0; i < dgv_data.Rows.Count; i++)
                {
                    try
                    {
                        if (Convert.ToBoolean(dgv_data.Rows[i].Cells[0].Value))
                        {
                            Instagram instagram = GetData(i);
                            navigator.Navigate().GoToUrl("https://www.instagram.com/" + instagram.Name);

                            int followers = 0;
                            int following = 0;
                            int post = 0;

                            try { followers = int.Parse(instagram.Followers); } catch (Exception) { }
                            try { following = int.Parse(instagram.Following); } catch (Exception) { }
                            try { post = int.Parse(instagram.Posts); } catch (Exception) { }

                            try
                            {
                                instagram.Image = navigator.FindElement(By.XPath("//img[@class='_6q-tv']")).GetAttribute("src");
                                instagram.Posts = ChangeNumber(navigator.FindElement(By.XPath("(//span[@class='g47SY '])[1]")).Text);
                                instagram.Followers = ChangeNumber(navigator.FindElement(By.XPath("(//span[@class='g47SY '])[2]")).Text);
                                instagram.Following = ChangeNumber(navigator.FindElement(By.XPath("(//span[@class='g47SY '])[3]")).Text);
                            }
                            catch (Exception)
                            {
                            }

                            try { instagram.new_Followers = (int.Parse(instagram.Followers) - followers).ToString(); } catch (Exception) { }
                            try { instagram.new_Following = (int.Parse(instagram.Following) - following).ToString(); } catch (Exception) { }
                            try { instagram.new_Posts = (int.Parse(instagram.Posts) - post).ToString(); } catch (Exception) { }

                            if (instagram.Image != "")
                            {
                                Image img = new CFormControl().ChangeImageSize(instagram.Image, 50, 50);
                                dgv_data.Rows[i].Cells[2].Value = img;
                            }
                            dgv_data.Rows[i].Cells[3].Value = instagram.Image;
                            dgv_data.Rows[i].Cells[6].Value = instagram.Followers;
                            dgv_data.Rows[i].Cells[7].Value = instagram.Following;
                            dgv_data.Rows[i].Cells[8].Value = instagram.Posts;
                            dgv_data.Rows[i].Cells[9].Value = instagram.new_Followers;
                            dgv_data.Rows[i].Cells[10].Value = instagram.new_Following;
                            dgv_data.Rows[i].Cells[11].Value = instagram.new_Posts;
                            dgv_data.Rows[i].Selected = true;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }
            Stop();
        }

        private void UpdateData()
        {
            try
            {
                List<Instagram> list = new List<Instagram>();
                for (int i = 0; i < dgv_data.Rows.Count; i++)
                {
                    try
                    {
                        Instagram instagram = GetData(i);
                        list.Add(instagram);
                    }
                    catch (Exception)
                    {
                    }
                }
                if (list.Count != 0)
                {
                    new CCsv().SaveCsv(list, "data.csv");
                }
            }
            catch (Exception)
            {
            }
        }

        private async void SendEmailAsync(Instagram instagram)
        {
            string subject = "Instagram weekly report";
            string contect = File.ReadAllText("index.html");
            string from = "support@omni.com";
            string apiName = tb_name.Text;
            string apiKey = tb_key.Text;

            contect = contect.Replace("profileimage", instagram.Image);
            contect = contect.Replace("Followers : 0", "Followers : " + instagram.Followers);
            contect = contect.Replace("Following : 0", "Following : " + instagram.Following);
            contect = contect.Replace("Posts : 0", "Posts : " + instagram.Posts);

            contect = contect.Replace("Followers : +0", "Followers : +" + instagram.new_Followers);
            contect = contect.Replace("Following : +0", "Following : +" + instagram.new_Following);
            contect = contect.Replace("Posts : +0", "Posts : +" + instagram.new_Posts);

            await new CEmailSend().SendGridEmail(subject, contect, from, new string[] { instagram.Email }, apiName, apiKey);
        }

        private string ChangeNumber(string number)
        {
            number = number.Replace(",", "");
            try
            {
                if (number.Contains("m"))
                {
                    number = number.Replace("m", "");
                    var n = double.Parse(number) * 1000000;
                    return n.ToString();
                }
            }
            catch (Exception)
            {
            }
            try
            {
                if (number.Contains("k"))
                {
                    number = number.Replace("k", "");
                    var n = double.Parse(number) * 1000;
                    return n.ToString();
                }
            }
            catch (Exception)
            {
            }
            return number;
        }

        private Instagram GetData(int i)
        {
            Instagram instagram = new Instagram();
            try { instagram.Image = dgv_data.Rows[i].Cells[3].Value.ToString(); } catch (Exception) { }
            try { instagram.Name = dgv_data.Rows[i].Cells[4].Value.ToString(); } catch (Exception) { }
            try { instagram.Email = dgv_data.Rows[i].Cells[5].Value.ToString(); } catch (Exception) { }
            try { instagram.Followers = dgv_data.Rows[i].Cells[6].Value.ToString(); } catch (Exception) { }
            try { instagram.Following = dgv_data.Rows[i].Cells[7].Value.ToString(); } catch (Exception) { }
            try { instagram.Posts = dgv_data.Rows[i].Cells[8].Value.ToString(); } catch (Exception) { }
            try { instagram.new_Followers = dgv_data.Rows[i].Cells[9].Value.ToString(); } catch (Exception) { }
            try { instagram.new_Following = dgv_data.Rows[i].Cells[10].Value.ToString(); } catch (Exception) { }
            try { instagram.new_Posts = dgv_data.Rows[i].Cells[11].Value.ToString(); } catch (Exception) { }
            return instagram;
        }


        private void AddData(Instagram instagram)
        {
            Image img = null;
            if (instagram.Image != "")
            {
                img = new CFormControl().ChangeImageSize(instagram.Image, 50, 50);
            }

            CheckForIllegalCrossThreadCalls = false;
            this.Invoke((MethodInvoker)delegate
            {
                dgv_data.RowTemplate.Height = 50;
                dgv_data.ScrollBars = ScrollBars.Vertical;
                dgv_data.Rows.Add(false, (dgv_data.RowCount + 1).ToString(), img, instagram.Image, instagram.Name, instagram.Email, instagram.Followers, instagram.Following, instagram.Posts, instagram.new_Followers, instagram.new_Following, instagram.new_Posts);
            });
        }


        private void btn_Save_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            savefile.FileName = date + ".csv";
            savefile.Filter = "csv files (*.csv)|*.csv";
            string path = "";
            List<Instagram> list = new List<Instagram>();
            if (savefile.ShowDialog() == DialogResult.OK)
            {
                path = Path.GetFullPath(savefile.FileName);
                for (int i = 0; i < dgv_data.Rows.Count; i++)
                {
                    try
                    {
                        Instagram instagram = GetData(i);
                        list.Add(instagram);
                    }
                    catch (Exception)
                    {
                    }
                }
                new CCsv().SaveCsv(list, path);
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                var collection = new CCsv().ReadCsv<Instagram>("data.csv");
                foreach (var item in collection)
                {
                    if (item.Name != "")
                    {
                        AddData(item);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void btn_open_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open csv File";
            theDialog.Filter = "csv files|*.csv";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                dgv_data.Rows.Clear();
                string path = Path.GetFullPath(theDialog.FileName);

                try
                {
                    var collection = new CCsv().ReadCsv<Instagram>(path);
                    foreach (var item in collection)
                    {
                        if (item.Name != "")
                        {
                            AddData(item);
                        }
                    }
                }
                catch (Exception)
                {
                }                
            }
        }

        private void cb_data_CheckedChanged(object sender, EventArgs e)
        {
            f.CheckAll(dgv_data, cb_data);
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            f.DeleteRowsNumber(dgv_data);
            List<Instagram> list = new List<Instagram>();
            for (int i = 0; i < dgv_data.Rows.Count; i++)
            {
                try
                {
                    Instagram instagram = GetData(i);
                    list.Add(instagram);
                }
                catch (Exception)
                {
                }
            }
            new CCsv().SaveCsv(list, "data.csv");
        }

        private void btn_email_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgv_data.Rows.Count; i++)
            {
                Instagram instagram = GetData(i);
                if (Convert.ToBoolean(dgv_data.Rows[i].Cells[0].Value) && instagram.Followers != "")
                {                   

                    SendEmailAsync(instagram);
                }                
            }
            MessageBox.Show("Sent Email");
        }
    }
}

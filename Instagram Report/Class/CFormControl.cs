using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Instagram_Email_Scrape.Class
{
    class CFormControl
    {
        public string EmailSearch(string text)
        {
            Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);
            MatchCollection emailMatches = emailRegex.Matches(text);
            foreach (Match emailMatch in emailMatches)
            {
                if (!emailMatch.Value.ToLower().Contains(".png") && !emailMatch.Value.ToLower().Contains(".jpg"))
                    return emailMatch.Value;
            }
            return "";
        }

        public Image ChangeImageSize(string str, int x, int y)
        {
            Image img = null;
            try
            {
                WebClient wc = new WebClient();
                byte[] bytes = wc.DownloadData(str);
                MemoryStream ms = new MemoryStream(bytes);
                img = Image.FromStream(ms);
                img = (Image)(new Bitmap(img, new Size(x, y)));
            }
            catch (Exception)
            {
            }
            return img;
        }

        public void CheckAll(DataGridView dataGridView, CheckBox checkbox)
        {
            if (checkbox.CheckState.ToString() == "Checked")
            {
                for (int i = 0; i < dataGridView.Rows.Count; i++)
                {
                    dataGridView.Rows[i].Cells[0].Value = true;
                }
            }
            else
            {
                for (int i = 0; i < dataGridView.Rows.Count; i++)
                {
                    dataGridView.Rows[i].Cells[0].Value = false;
                }
            }
        }

        public void DeleteRowsNumber(DataGridView dataGridView)
        {

            for (int i = dataGridView.Rows.Count - 1; i >= 0; i--)
            {
                if (Convert.ToBoolean(dataGridView.Rows[i].Cells[0].Value))
                    dataGridView.Rows.RemoveAt(i);
            }
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                dataGridView.Rows[i].Cells[1].Value = (i + 1).ToString();
            }
        }
    }
}

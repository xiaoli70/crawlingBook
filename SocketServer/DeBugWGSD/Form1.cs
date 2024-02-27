using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DeBugWGSD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();//1287
        }
        #region 实现拖动 初始化

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private bool isMouseDown = false;
        private Point mouseOffset;

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = true;
                mouseOffset = new Point(-e.X, -e.Y);
            }
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                this.Location = mousePos;
            }
            //NanUI
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {


            textBox1.ScrollBars = (ScrollBars)RichTextBoxScrollBars.Both;

            this.MinimumSize = new System.Drawing.Size(1, 1);


        }
        #endregion


        string searchbook = "https://www.biqugeu.net/searchbook.php?keyword=<<bookname>>";//http://www.biquge5200.cc/4040183/147090695.html
        string searchurl = "http://www.biquge5200.cc//40_40183/147477399.html";
        string baseurl = "http://www.biquge5200.cc/";
        string nextChapter = null;
        string upChapter = null;
        string html = null;
        string bookname = null;
        string bookTitle = null;
        string ChapterContent;
        string regex1 = "<h1>(?<bookname>.*?)</h1>";
        string regex2 = "<a href=\"/.*?\" target=\"_top\" class=\"pre\">上一章</a> ← <a href=\"/.*?/\" target=\"_top\" title=\"\" class=\"back\">章节列表</a> → <a href=\"(?<nextChapter>.*?)\" target=\"_top\" class=\"next\"";
        string regex2224 = "<a href=\"/(?<bookname1>.*?)\">上一章(.*?</a>)";
        string regex22s = "<a href=\"/(?<bookname>.*?)\">下一章</a>";
        string regex22 = "<a href=\"/(?<bookname1>.*?)\">上一章</a> &larr; <a href=\"/8_8187/\">章节目录</a> &rarr; <a href=\"/(?<bookname>.*?)\">下一章</a>";
        string regex3 = "<title>(?<booktitle>.*?)</title>";
        string regex4 = "<div id=\"content\" class=\"(?<data1>.*?)\">(?<data>.*?)</div>"; /*(?<data>.*?)<br/><br/>*/
        string boole = "S";

        /// <summary>
        /// 读取内容
        /// </summary>
        public void Getbook()
        {

            restart: try
            {
                html = GetHtml(searchurl);
                html = html.Replace("<div id='gc1' class='gcontent1'><script type='text/javascript'>try{ggauto();} catch(ex){}</script></div>", "");
                if (!string.IsNullOrEmpty(html))
                {
                    ChapterContent = "";
                    nextChapter = Regex.Match(html, regex22).Groups["bookname"].ToString();
                    searchurl = baseurl + nextChapter;
                    upChapter = Regex.Match(html, regex22).Groups["bookname1"].ToString();
                    upChapter = baseurl + upChapter;
                    bookname = Regex.Match(html, regex1).Groups["bookname"].ToString();
                    ChapterContent += "\r\n";
                    ChapterContent += bookname;
                    ChapterContent += "\r\n";
                    bookTitle = Regex.Match(html, regex3).Groups["booktitle"].ToString();
                    string book1 = Regex.Match(html, regex4).Groups["data"].ToString().Trim().Replace("<p>", "").Replace("</p>", "");
                    ChapterContent += book1.ToString().Trim();
                    textBox1.Text = ChapterContent.Replace("。", "。\n\n").Replace("　　", "");
                }
            }
            catch (WebException we)
            {

                Console.WriteLine("远程主机强迫关闭了一个现有的连接,重新爬取当前章节。。。");
                goto restart;
            }
        }
        static string menuurl = "http://www.biquge5200.cc/8_8187";//8_8187
        static string regex6 = "<dd><a href=\"(?<url>.*?)\">(?<name>.*?)</a></dd>";
        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        public static List<book> MuluBook()
        {
            
            List<book> list = new List<book>();
            string html = GetHtml(menuurl);
            if (!string.IsNullOrEmpty(html))
            {
                MatchCollection match = Regex.Matches(html, regex6);
                foreach (Match item in match)
                {
                    string book = Regex.Match(item.Value, regex6).Groups["name"].ToString();
                    list.Add(new book() { name = book, url = Regex.Match(item.Value, regex6).Groups["url"].ToString() });
                    //list2.Add(book, Regex.Match(item.Value, regex6).Groups["url"].ToString());
                }
            }
            return list;
        }
        public static string GetHtml(string url = "")
        {
            string htmlCode;
            HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            webRequest.Timeout = 30000;
            webRequest.Method = "GET";
            webRequest.UserAgent = "Mozilla/4.0";
            webRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
            HttpWebResponse webResponse = (System.Net.HttpWebResponse)webRequest.GetResponse();
            string contentype = webResponse.Headers["Content-Type"];
            Regex regex = new Regex("charset\\s*=\\s*[\\W]?\\s*([\\w-]+)", RegexOptions.IgnoreCase);
            if (webResponse.ContentEncoding.ToLower() == "gzip")//如果使用了GZip则先解压
            {
                using (System.IO.Stream streamReceive = webResponse.GetResponseStream())
                {
                    using (var zipStream = new System.IO.Compression.GZipStream(streamReceive, System.IO.Compression.CompressionMode.Decompress))
                    {

                        //匹配编码格式
                        if (regex.IsMatch(contentype))
                        {
                            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                            Encoding encoding = System.Text.Encoding.GetEncoding("GB2312");

                            //Encoding ending = Encoding.GetEncoding(regex.Match(contentype).Groups[1].Value.Trim());
                            using (StreamReader sr = new System.IO.StreamReader(zipStream, encoding))
                            {
                                htmlCode = sr.ReadToEnd();
                            }
                        }
                        else
                        {
                            using (StreamReader sr = new System.IO.StreamReader(zipStream, Encoding.UTF8))
                            {
                                htmlCode = sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            else
            {
                using (System.IO.Stream streamReceive = webResponse.GetResponseStream())
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(streamReceive, Encoding.Default))
                    {
                        htmlCode = sr.ReadToEnd();
                    }
                }
            }
            return htmlCode;
        }
        string str = null;
        List<book> booklist = null;

        private void uPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            searchurl = upChapter;
            Getbook();
        }
        private void downToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Getbook();
        }

        private void cataLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripComboBox1.Items.Clear();
            booklist = MuluBook();
            for (int i = 0; i < booklist.Count; i++)
            {
                toolStripComboBox1.Items.Add((i + 1) + "." + booklist[i].name);
            }
        }

        private void okToolStripMenuItem_Click(object sender, EventArgs e)
        {
            str = toolStripComboBox1.Text;
            if (booklist.Count > 0 && str != "")
            {
                searchurl = baseurl + booklist[Convert.ToInt32(str.Split('.')[0]) - 1].url;
                Getbook();
                toolStripComboBox1.Items.Clear();
            }
            else
            {
                searchurl = baseurl + booklist[Convert.ToInt32(textBox1.Text)].url;
                Getbook();
                toolStripComboBox1.Items.Clear();
            }

        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            menuurl = "http://www.biquge5200.cc/8_8187";//8_8187
        }

        private void 万ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            menuurl = "http://www.biquge5200.cc/0_844";//8_8187
        }
    }


    public class book
    {
        public string name { get; set; }
        public string url { get; set; }

    }
}
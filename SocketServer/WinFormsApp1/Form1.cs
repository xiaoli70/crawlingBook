using System.Data;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using SqlSugar;
using WinFormsApp1;
using System.Windows.Forms;


namespace DeBugWGSD
{
    public partial class Form1 : Form
    {
        private readonly ISqlSugarClient _sqlSugar;
        public Form1(ISqlSugarClient sqlSugar)
        {
            InitializeComponent();//1287
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
        
        _sqlSugar = sqlSugar;
        }
        //ISqlSugarClient sqlSugar = new SqlSugarClientT().sqlSugarClient;
        //private void textBox1_TextChanged(object sender, EventArgs e)
        //{
        //    this.WindowState = FormWindowState.Minimized;
        //    this.ShowInTaskbar = false;//禁用（隐藏）任务栏图标
        //    notifyIcon1.Visible = true;//显示系统托盘图标
        //}
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
        private async void Form1_Load(object sender, EventArgs e)
        {

            //textBox1.BorderStyle = BorderStyle.FixedSingle;
            //this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //this.BackColor = Color.LimeGreen;
            //this.TransparencyKey = Color.LimeGreen;
            //FormBorderStyle = FormBorderStyle.None;
            textBox1.ScrollBars = (ScrollBars)RichTextBoxScrollBars.Both;
            //textBox1.BorderStyle = BorderStyle.FixedSingle;

            this.MinimumSize = new System.Drawing.Size(1, 1);
            searchurl = (await _sqlSugar.Queryable<BookCache>()
                 .OrderBy(it => it.Id, OrderByType.Desc)// 获取
                .FirstAsync()).Path;
            await Getbook();

        }

        string searchbook = "https://www.biqugeu.net/searchbook.php?keyword=<<bookname>>";//http://www.biquge5200.cc/4040183/147090695.html
        string searchurl = "https://www.bqgbe.com/book/5062/2.html";
        string searchcontent = null;
        string baseurl = "https://www.bqgbe.com/";
        string nextChapter = null;
        string upChapter = null;
        string html = null;
        string bookname = null;
        string bookTitle = null;
        string ChapterContent;
        string regex1 = "<h1 class=\"wap_none\">(?<bookname>.*?)</h1>";
        string regex2 = "<a href=\"/.*?\" target=\"_top\" class=\"pre\">上一章</a> ← <a href=\"/.*?/\" target=\"_top\" title=\"\" class=\"back\">章节列表</a> → <a href=\"(?<nextChapter>.*?)\" target=\"_top\" class=\"next\"";
        string regex222 = "<a href=\"/(?<bookname1>.*?)\">上一章</a> &larr; <a href=\"/40_40183/\">章节目录</a> &rarr; <a href=\"/(?<bookname>.*?)\">下一章</a>";
        string regex22 = "<a href=\"(?s)(?<bookname1>.*?)\" id=\"pb_prev\" class=\"Readpage_up\">上一章</a><a href=\"/book/5062/\" id=\"pb_mulu\" class=\"Readpage_up\">目录</a><a href=\"(?s)(?<bookname>.*?)\" id=\"pb_next\" class=\"Readpage_down js_page_down\">下一章</a>";
        string regexss = "<a href=\"(?<bookname1>.*?)\" id=\"pb_prev\" class=\"Readpage_up\">上一章</a>";
        string regexdd = "<a href=\"(?<booktitle>.*?)\" id=\"pb_next\" class=\"Readpage_down js_page_down\">下一章</a>";
        string regex3 = "<title>(?<booktitle>.*?)</title>";
        string regex4 = "<div id=\"content\">(?<data>.*?)</div>"; /*(?<data>.*?)<br/><br/>*/
        string regex333 = "<div id=\"chaptercontent\" class=\"Readarea ReadAjax_content\">(?s)(?<data>.*?)</div>"; /*(?<data>.*?)<br/><br/>*/
        string boole = "S";
        List<BookVal> list = new List<BookVal>();
        
        public async Task Getbook()
        {

            restart: try
            {
                

                html = await GetHtmlAsync(searchurl);
                //html = GetHtml(searchurl);
                html = html.Replace("<div id='gc1' class='gcontent1'><script type='text/javascript'>try{ggauto();} catch(ex){}</script></div>", "");
                if (!string.IsNullOrEmpty(html))
                {
                    ChapterContent = "";
                    nextChapter = Regex.Match(html, regexdd).Groups["booktitle"].ToString();
                    searchurl = baseurl + nextChapter;
                    upChapter = Regex.Match(html, regexss).Groups["bookname1"].ToString();
                    upChapter = baseurl + upChapter;
                    bookname = Regex.Match(html, regex1).Groups["bookname"].ToString();
                    ChapterContent += "\r\n";
                    ChapterContent += bookname;
                    ChapterContent += "\r\n";
                    bookTitle = Regex.Match(html, regex3).Groups["booktitle"].ToString();
                    string book1 = Regex.Match(html, regex333).Groups["data"].ToString().Trim().Replace("<br />", "").Replace("</p>", "");
                    ChapterContent += book1.ToString().Trim();
                    //Console.WriteLine(ChapterContent.Replace("。", "。\n\n").Replace("　　", "") + "-------加载完毕！");
                    textBox1.Text = ChapterContent.Replace("。", "。\n\n").Replace("　　", "");
                    if (list.Count == 0)
                    {
                        await CaChebook();
                    }
                }

                //}
            }
            catch (WebException we)
            {
                int lastDotIndex = searchurl.LastIndexOf('.');

                if (lastDotIndex != -1)
                {
                    // 从最后一个点开始向前找数字的起始位置
                    int startIndex = lastDotIndex - 1;
                    while (startIndex >= 0 && char.IsDigit(searchurl[startIndex]))
                    {
                        startIndex--;
                    }

                    // 提取最后一个点前的数字
                    string numberStr = searchurl.Substring(startIndex + 1, lastDotIndex - startIndex - 1);

                    // 将数字加1，并替换原字符串中的数字部分
                    if (int.TryParse(numberStr, out int number))
                    {
                        number += 1;

                        string newUrl = searchurl.Substring(0, startIndex + 1) + number.ToString() + searchurl.Substring(lastDotIndex);
                        Console.WriteLine(newUrl);
                    }
                }
                Console.WriteLine("远程主机强迫关闭了一个现有的连接,重新爬取当前章节。。。");
                textBox1.Text = "远程主机强迫关闭了一个现有的连接,重新爬取当前章节。。。";
                //goto restart;
            }
        }
        /// <summary>
        /// 缓存到List
        /// </summary>
        public async Task CaChebook() {
            //string next = searchurl;
            await Task.Run(async () => {
                int count = 0;
                while (count < 3)
                {
                    // 在这里执行你想要循环执行的操作
                    string html =  await GetHtmlAsync(searchurl);
                    //html = GetHtml(searchurl);
                    html = html.Replace("<div id='gc1' class='gcontent1'><script type='text/javascript'>try{ggauto();} catch(ex){}</script></div>", "");
                    if (!string.IsNullOrEmpty(html))
                    {
                        ChapterContent = "";
                        nextChapter = Regex.Match(html, regexdd).Groups["booktitle"].ToString();
                        searchurl = baseurl + nextChapter;
                        upChapter = Regex.Match(html, regexss).Groups["bookname1"].ToString();
                        upChapter = baseurl + upChapter;
                        bookname = Regex.Match(html, regex1).Groups["bookname"].ToString();
                        ChapterContent += "\r\n";
                        ChapterContent += bookname;
                        ChapterContent += "\r\n";
                        bookTitle = Regex.Match(html, regex3).Groups["booktitle"].ToString();
                        string book1 = Regex.Match(html, regex333).Groups["data"].ToString().Trim().Replace("<br />", "").Replace("</p>", "");
                        ChapterContent += book1.ToString().Trim();
                        //Console.WriteLine(ChapterContent.Replace("。", "。\n\n").Replace("　　", "") + "-------加载完毕！");
                        list.Add(new BookVal() { content = ChapterContent.Replace("。", "。\n\n").Replace("　　", ""), url = upChapter });
                    }
                    // 计数递增
                    count++;
                }
                MessageBox.Show("缓存完成");
            });
            


        }



        static string menuurl = "https://www.bqgbe.com/book/5062/";
        static string regex6 = "<dd><a href=\"(?<url>.*?)\">(?<name>.*?)</a></dd>";
        static string regex66 = "<dd><a href =\"(?<url>.*?)\">(?<name>.*?)</a></dd>";

        public static List<book> MuluBook()
        {
            // HttpWebRequest req = (HttpWebRequest)WebRequest.Create(menuurl);
            List<book> list = new List<book>();
            string html = GetHtml(menuurl);
            if (!string.IsNullOrEmpty(html))
            {
                MatchCollection match = Regex.Matches(html, regex66);

                foreach (Match item in match)
                {
                    string book = Regex.Match(item.Value, regex66).Groups["name"].ToString();
                    list.Add(new book() { name = book, url = Regex.Match(item.Value, regex66).Groups["url"].ToString() });
                    //list2.Add(book, Regex.Match(item.Value, regex6).Groups["url"].ToString());  
                }
            }
            return list;

        }
        public static async Task<string> GetHtmlAsync(string url = "")
        {
            string htmlCode;

            // 通过静态字段或依赖注入的方式重用 HttpClient
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");

                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();

                    using (Stream streamReceive = await response.Content.ReadAsStreamAsync())
                    {
                        using (var zipStream = new System.IO.Compression.GZipStream(streamReceive, System.IO.Compression.CompressionMode.Decompress, leaveOpen: true))
                        {
                            // 匹配编码格式
                            Regex regex = new Regex("charset\\s*=\\s*[\\W]?\\s*([\\w-]+)", RegexOptions.IgnoreCase);

                            Encoding encoding = null;

                            if (regex.IsMatch(response.Content.Headers.ContentType?.ToString()))
                            {
                                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                                encoding = System.Text.Encoding.GetEncoding("UTF-8");
                            }
                            else
                            {
                                encoding = Encoding.UTF8;
                            }

                            using (StreamReader sr = new StreamReader(zipStream, encoding, detectEncodingFromByteOrderMarks: false, bufferSize: 4096, leaveOpen: true))
                            {
                                htmlCode = await sr.ReadToEndAsync();
                            }
                        }
                    }
                }
            }

            return htmlCode;
        }
        public static async Task<string> GetHtmlAsync1(string url = "")
        {
            string htmlCode;

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30000);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");

                HttpResponseMessage response = await client.GetAsync(url);

                string contentype = response.Content.Headers.ContentType?.ToString();

                if (response.Content.Headers.ContentEncoding.Contains("gzip"))
                {
                    using (Stream streamReceive = await response.Content.ReadAsStreamAsync())
                    {
                        using (var zipStream = new System.IO.Compression.GZipStream(streamReceive, System.IO.Compression.CompressionMode.Decompress))
                        {
                            // 匹配编码格式
                            Regex regex = new Regex("charset\\s*=\\s*[\\W]?\\s*([\\w-]+)", RegexOptions.IgnoreCase);

                            if (regex.IsMatch(contentype))
                            {
                                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                                Encoding encoding = System.Text.Encoding.GetEncoding("UTF-8");

                                using (StreamReader sr = new StreamReader(zipStream, encoding))
                                {
                                    htmlCode = await sr.ReadToEndAsync();
                                }
                            }
                            else
                            {
                                using (StreamReader sr = new StreamReader(zipStream, Encoding.UTF8))
                                {
                                    htmlCode = await sr.ReadToEndAsync();
                                }
                            }
                        }
                    }
                }
                else
                {
                    using (Stream streamReceive = await response.Content.ReadAsStreamAsync())
                    {
                        using (StreamReader sr = new StreamReader(streamReceive, Encoding.Default))
                        {
                            htmlCode = await sr.ReadToEndAsync();
                        }
                    }
                }
            }

            return htmlCode;
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

                            Encoding encoding = System.Text.Encoding.GetEncoding("UTF-8");

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



        private async void downToolStripMenuItem_Click(object sender, EventArgs e)
        {
             //
            if (list.Count>0) {
                textBox1.Text = list[0].content.ToString();
                await _sqlSugar.Insertable(new BookCache { Path = list[0].url, ModifyDate = DateTime.Now }).ExecuteCommandAsync();
                list.RemoveAt(0);
            } else {
                await _sqlSugar.Insertable(new BookCache { Path = searchurl, ModifyDate = DateTime.Now }).ExecuteCommandAsync();
                await Getbook();
            }

        }

        private async void cataLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripComboBox1.Items.Clear();
            booklist = MuluBook();
            for (int i = 0; i < booklist.Count; i++)
            {
                //comboBox1.Items.Add((i + 1) + "." + booklist[i].name);
                toolStripComboBox1.Items.Add((i + 1) + "." + booklist[i].name);
            }
        }

        private void okToolStripMenuItem_Click(object sender, EventArgs e)

        {

            str = toolStripComboBox1.Text;
            searchurl = baseurl + booklist[Convert.ToInt32(str.Split('.')[0]) - 1].url;
            _sqlSugar.Insertable(new BookCache { Path = searchurl, ModifyDate = DateTime.Now }).ExecuteCommand();
            Getbook();
            //toolStripComboBox1.Items.Clear();


        }

        private void 最小化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (WindowState == FormWindowState.Normal)
            //{
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;//禁用（隐藏）任务栏图标
            notifyIcon1.Visible = true;//显示系统托盘图标
            //}
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = true;
                notifyIcon1.Visible = false;
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.Z)  // 例如，Ctrl + S 组合键
            {
                // 执行你的操作
                //MessageBox.Show("Ctrl + S 被按下！");
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;//禁用（隐藏）任务栏图标
                notifyIcon1.Visible = true;//显示系统托盘图标
            }
        }
    }


    public class book
    {
        public string name { get; set; }
        public string url { get; set; }

    }
    public class BookVal
    {
        public string content { get; set; }
        public string url { get; set; }

    }
}
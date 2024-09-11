using System.Data;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO.Compression;
using System.IO.Packaging;
using System.Security.Policy;
using System.Net.Http.Headers;
using LiteDB;


namespace CrawlFiction;

public partial class Form1 : Form
{
    private bool isMouseDown = false;
    private Point mouseOffset;
    private static readonly HttpClient httpClient = new HttpClient(new HttpClientHandler
    {
        
    });
    private LiteDbContext _dbContext;

    LiteCollection<Customer> _customerCollection;
    private string searchUrl;
    private string baseUrl = "http://www.biquge5200.cc/";
    private string nextChapter;
    private string upChapter;
    private string chapterContent;
    private List<BookVal> list = new List<BookVal>();
    private List<book> booklist = new List<book>();

    private const string searchBookUrlTemplate = "https://www.biqugeu.net/searchbook.php?keyword=<<bookname>>";
    private string menuUrl = "http://www.biquge5200.cc/8_8187";
    private const string regex1 = "<h1>(?<bookname>.*?)</h1>";//
    private const string regex2 = "<a href=\"(?<bookname1>.*?)\" id=\"pb_prev\" class=\"Readpage_up\">上一章</a>";
    private const string regex3 = "<title>(?<booktitle>.*?)</title>";
    private const string regex4 = "<div id=\"content\" class=\"(?<data1>.*?)\">(?<data>.*?)</div>";
    private const string regex4s = "<div id=\"chaptercontent\" class=\"Readarea ReadAjax_content\">(?<data>.*?)</div>";
    private const string regex5 = "&rarr; <a href=\"/(?<bookname>.*?)\">下一章</a>";
    private const string regex5s = "<a href=\"/(?<bookname>.*?)\" id=\"pb_next\" class=\"Readpage_down js_page_down\">下一章</a>";
    private const string regex6 = "<dd><a href=\"(?<url>.*?)\">(?<name>.*?)</a></dd>";
    private const string regex7 = "<dd><a href =\"(?<url>.*?)\">(?<name>.*?)</a></dd>";


    private string regex22 = "<a href=\"/(?<bookname1>.*?)\">上一章</a> &larr; <a href=\"(?<url>.*?)\">章节目录</a> &rarr; <a href=\"/(?<bookname>.*?)\">下一章</a>";

    static string regex66 = "";


   
    public Form1()
    {
        InitializeComponent();
        this.KeyDown += new KeyEventHandler(Form1_KeyDown);
        this.Load += new EventHandler(Form1_Load);
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        _dbContext = new LiteDbContext(@"MyData.db");
        _customerCollection = _dbContext.Customers;

        var result = _customerCollection.FindAll().FirstOrDefault() ;
        if (result != null) { 
            searchUrl = result.Name;
            GetBookAsync();
        }
    }

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
    }

    private void Form_MouseUp(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            isMouseDown = false;
        }
    }

    private void CacheSection() {

        var customer = new Customer
        {
            Id = 1,
            Name = searchUrl,
        };
        var reult = _customerCollection.Update(customer);
        if (!reult)
        {

            var result = _customerCollection.Insert(customer);
        }

    }

    private async Task GetBookAsync()
    {
        try
        {//1023
            string html = await GetHtmlAsync(searchUrl);

            CacheSection();

            html = html.Replace("<div id='gc1' class='gcontent1'><script type='text/javascript'>try{ggauto();} catch(ex){}</script></div>", "");
            if (!string.IsNullOrEmpty(html))
            {
                chapterContent = "";
                nextChapter = Regex.Match(html, regex5).Groups["bookname"].ToString();
                if (nextChapter == "")
                    nextChapter = Regex.Match(html, regex5s).Groups["bookname"].ToString(); baseUrl = "https://www.bg60.cc/";
                searchUrl = baseUrl + nextChapter;
                upChapter = Regex.Match(html, regex22).Groups["bookname1"].ToString();
                if (string.IsNullOrEmpty(upChapter)) 
                    upChapter = Regex.Match(html, regex2).Groups["bookname1"].ToString();
                upChapter = baseUrl + upChapter;
                string bookName = Regex.Match(html, regex1).Groups["bookname"].ToString();
                

                string bookTitle = Regex.Match(html, regex3).Groups["booktitle"].ToString();
                if (bookName == "")
                    bookName = bookTitle;
                    string bookContent = Regex.Match(html, regex4).Groups["data"].ToString().Trim().Replace("<br />", "").Replace("</p>", "");
                if (bookContent == "") { 
                    Regex regex = new Regex(regex4s, RegexOptions.Singleline);
                    Match match = regex.Match(html);

                    if (match.Success)
                    {
                        bookContent = match.Groups["data"].Value.ToString().Replace("<br />", "");
                        
                    }
                }
                chapterContent += $"{bookName}\r\n{bookContent}";
                richTextBox1.Text = chapterContent.Replace("　　", "\n");
            }
        }
        catch (WebException ex)
        {
            HandleWebException(ex);
            MessageBox.Show($"Error initializing database: {ex.Message}");
        }
    }


    private void HandleWebException(WebException ex)
    {
        int lastDotIndex = searchUrl.LastIndexOf('.');

        if (lastDotIndex != -1)
        {
            int startIndex = lastDotIndex - 1;
            while (startIndex >= 0 && char.IsDigit(searchUrl[startIndex]))
            {
                startIndex--;
            }

            string numberStr = searchUrl.Substring(startIndex + 1, lastDotIndex - startIndex - 1);

            if (int.TryParse(numberStr, out int number))
            {
                number += 1;
                searchUrl = searchUrl.Substring(0, startIndex + 1) + number + searchUrl.Substring(lastDotIndex);
            }
        }
    }

   

    public static async Task<string> GetHtmlAsync(string url)
    {
        string htmlCode = string.Empty;

        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/4.0");
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("deflate"));

            try
            {
                using (HttpResponseMessage response = await httpClient.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    string contentType = response.Content.Headers.ContentType.ToString();
                    var contentEncoding = response.Content.Headers.ContentEncoding;

                    using (Stream streamReceive = await response.Content.ReadAsStreamAsync())
                    {
                        Stream decompressedStream = DecompressStream(streamReceive, contentEncoding);
                        Encoding encoding = GetEncoding(contentType);
                        using (StreamReader sr = new StreamReader(decompressedStream, encoding))
                        {
                            htmlCode = await sr.ReadToEndAsync();
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"请求失败: {e.Message}");
                throw;
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("请求超时。");
                throw;
            }
        }

        return htmlCode;
    }

    public static async Task<string> GetHtmlAsync1(string url)
    {
        string htmlCode = string.Empty;

        httpClient.Timeout = TimeSpan.FromSeconds(30);
        //httpClient.DefaultRequestHeaders.Add("User-Agent", GetRandomUserAgent());
        //httpClient.DefaultRequestHeaders.Add("Cookie", $"SessionId={Guid.NewGuid()}");
        // 设置用户代理  
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");

        // 设置接受编码头  
        httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

        try
        {
            using (HttpResponseMessage response = await httpClient.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                using (Stream streamReceive = await response.Content.ReadAsStreamAsync())
                {
                    Stream decompressedStream = DecompressStream(streamReceive, response.Content.Headers.ContentEncoding);
                    Encoding encoding = GetEncoding(response.Content.Headers.ContentType?.ToString());
                    using (StreamReader sr = new StreamReader(decompressedStream, encoding))
                    {
                        htmlCode = await sr.ReadToEndAsync();
                    }
                }
            }


        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"请求失败: {e.Message}");
            throw;
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("请求超时。");
            throw;
        }

        return htmlCode;
    }

    private static Stream DecompressStream(Stream stream, IEnumerable<string> encoding)
    {
        if (encoding.Contains("gzip"))
        {
            return new GZipStream(stream, CompressionMode.Decompress);
        }
        if (encoding.Contains("deflate"))
        {
            return new DeflateStream(stream, CompressionMode.Decompress);
        }
        if (encoding.Contains("br"))
        {
            return new BrotliStream(stream, CompressionMode.Decompress);
        }
        return stream;
    }

    private static Encoding GetEncoding(string contentType)
    {
        Regex regex = new Regex("charset\\s*=\\s*[\\W]?\\s*([\\w-]+)", RegexOptions.IgnoreCase);
        if (regex.IsMatch(contentType))
        {
            string charset = regex.Match(contentType).Groups[1].Value;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Encoding.GetEncoding(charset);
        }
        return Encoding.UTF8;
    }

    private static string GetRandomUserAgent()
    {
        List<string> userAgents = new List<string>
        {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Firefox/103.0",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Edge/108.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Linux; Android 10; Pixel 3 XL Build/QQ1A.200205.002) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.132 Mobile Safari/537.36"
        };
        Random random = new Random();
        int index = random.Next(userAgents.Count);
        return userAgents[index];
    }



    string str = null;


    private async void UPToolStripMenuItem_Click(object sender, EventArgs e)
    {
        searchUrl = upChapter;
        await GetBookAsync();
    }
    private void uPToolStripMenuItem_Click(object sender, EventArgs e)
    {
        searchUrl = upChapter;
        GetBookAsync();
    }



    private async void downToolStripMenuItem_Click(object sender, EventArgs e)
    {
        /*//
        if (list.Count > 0)
        {
            richTextBox1.Text = list[0].content.ToString();
            await _sqlSugar.Insertable(new BookCache { Path = list[0].url, ModifyDate = DateTime.Now }).ExecuteCommandAsync();
            list.RemoveAt(0);
        }
        else
        {
            await _sqlSugar.Insertable(new BookCache { Path = searchurl, ModifyDate = DateTime.Now }).ExecuteCommandAsync();
            
        }*/
        await GetBookAsync();

    }

    private async void cataLogToolStripMenuItem_Click(object sender, EventArgs e)
    {
        toolStripComboBox1.Items.Clear();

        string html = await GetHtmlAsync(menuUrl); 
        MatchCollection matches = Regex.Matches(html, regex6);

        if(matches.Count <= 0)
        {
            matches = Regex.Matches(html, regex7);
        }
        foreach (Match item in matches)
        {
            string book = item.Groups["name"].Value.ToString();
            booklist.Add(new book() { name = book, url = item.Groups["url"].Value.ToString() });
            int index = booklist.Count;
            toolStripComboBox1.Items.Add((index) + "." + book);
            //list2.Add(book, Regex.Match(item.Value, regex6).Groups["url"].ToString());  
        }

    }



    private void okToolStripMenuItem_Click(object sender, EventArgs e)

    {

        str = toolStripComboBox1.Text;
        searchUrl = baseUrl + booklist[Convert.ToInt32(str.Split('.')[0]) - 1].url;
        //_sqlSugar.Insertable(new BookCache { Path = searchurl, ModifyDate = DateTime.Now }).ExecuteCommand();
        GetBookAsync();
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

    private void 斗破ToolStripMenuItem_Click(object sender, EventArgs e)
    {
        //menuUrl = "http://www.biquge5200.cc/0_844";
        menuUrl = "https://www.bg60.cc/book/502/";
        baseUrl = "https://www.bg60.cc/";


    }

    private void 万古ToolStripMenuItem_Click(object sender, EventArgs e)
    {
        menuUrl = "http://www.biquge5200.cc/8_8187";
        baseUrl = "http://www.biquge5200.cc/";
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
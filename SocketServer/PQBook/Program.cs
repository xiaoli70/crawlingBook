using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace PQBook
{
    public class Program
    {
        public static void Main(string[] args)
        {


            //Console.WriteLine("请输入需要爬取的小说！");

            //string novelName = Console.ReadLine();
            //try
            //{
            //    //searchurl = searchbook.Replace("<<bookname>>", novelName);
            //    HttpWebRequest req1 = (HttpWebRequest)WebRequest.Create(searchurl);
            //    req1.Method = "GET";
            //    req1.Accept = "text/html";
            //    req1.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/537.36";
            //    HttpWebResponse res1 = (HttpWebResponse)req1.GetResponse();
            //    using (StreamReader reader = new StreamReader(res1.GetResponseStream()))
            //    {
            //        html = reader.ReadToEnd();
            //        if (!string.IsNullOrEmpty(html))
            //        {
            //            //Console.WriteLine(html);
            //            html = html.Replace("\n", "").Replace("\t", "").Replace("\r", "");
            //            searchcontent = Regex.Match(html, regex5).Groups["bookurl"].ToString();
            //            if (searchcontent == "")
            //            {
            //                Console.WriteLine("没有找到该小说！");
            //            }
            //            searchurl = baseurl + searchcontent;
            //        }
            //    }
            //}
            //catch (WebException we)
            //{
            //    Console.WriteLine(we.Message);
            //}
            //try
            //{
            //    HttpWebRequest req1 = (HttpWebRequest)WebRequest.Create(searchurl);
            //    req1.Method = "GET";
            //    req1.Accept = "text/html";
            //    req1.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/537.36";
            //    HttpWebResponse res1 = (HttpWebResponse)req1.GetResponse();
            //    using (StreamReader reader = new StreamReader(res1.GetResponseStream()))
            //    {
            //        html = reader.ReadToEnd();
            //        if (!string.IsNullOrEmpty(html))
            //        {
            //            //Console.WriteLine(html);
            //            html = html.Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace(" ", "");
            //            searchcontent = Regex.Matches(html, regex6)[1].Groups["bookfirst"].ToString();
            //            searchurl = baseurl + searchcontent;
            //        }
            //    }
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
            string searchbook = "https://www.biqugeu.net/searchbook.php?keyword=<<bookname>>";
            string searchurl = "http://www.biquge5200.cc//40_40183/147477399.html";
            string searchcontent = null;
            string baseurl = "http://www.biquge5200.cc/";
            string nextChapter = null;
            string html = null;
            string bookname = null;
            string bookTitle = null;
            string ChapterContent;
            string regex1 = "<h1>(?<bookname>.*?)</h1>";
            string regex2 = "<a href=\"/.*?\" target=\"_top\" class=\"pre\">上一章</a> ← <a href=\"/.*?/\" target=\"_top\" title=\"\" class=\"back\">章节列表</a> → <a href=\"(?<nextChapter>.*?)\" target=\"_top\" class=\"next\"";
            string regex22 = "<a href=\"/(?<bookname1>.*?)\">上一章</a> &larr; <a href=\"/40_40183/\">章节目录</a> &rarr; <a href=\"/(?<bookname>.*?)\">下一章</a>";
            string regex3 = "<title>(?<booktitle>.*?)</title>";
            string regex4 = "<div id=\"content\">(?<data>.*?)</div>"; /*(?<data>.*?)<br/><br/>*/
            string regex5 = "<div class=\"image\">\\s*<a href=\"/(?<bookurl>.*?)\"";
            string regex6 = "<dt>.*?</dt><dd><ahref=\"/(?<bookfirst>.*?)\">.*?</a></dd>";
            string boole = "S";
            
            do
            {

                restart: try
                {
                    if (boole == "S") {
                        List<book> lists = MuluBook();
                        int i = 1;
                        foreach (var item in lists)
                        {
                            Console.WriteLine(i + "." + item.name);
                            i++;
                        }
                        Console.WriteLine("请输入指令");
                        int num= Convert.ToInt32(Console.ReadLine());
                        searchurl = baseurl + lists[num-1].url;
                    }
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(searchurl);
                    req.Method = "GET";
                    req.Accept = "text/html;charset=gbk";
                    req.AllowAutoRedirect = true;
                    req.Headers.Add("Encoding", "gzip,deflate");
                    req.Headers[HttpRequestHeader.AcceptLanguage] = "zh-CN,zh";
                    req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0)";
                    HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                    using (StreamReader reader = new StreamReader(res.GetResponseStream(), System.Text.Encoding.Default))
                    {
                        html = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(html))
                        {
                            ChapterContent = "";
                            //获取下一章
                            nextChapter = Regex.Match(html, regex22).Groups["bookname"].ToString();
                            searchurl = baseurl + nextChapter;
                            bookname = Regex.Match(html, regex1).Groups["bookname"].ToString();
                            ChapterContent += "\r\n";
                            ChapterContent += bookname;
                            ChapterContent += "\r\n";
                            bookTitle = Regex.Match(html, regex3).Groups["booktitle"].ToString();
                            string book1 = Regex.Match(html, regex4).Groups["data"].ToString().Trim().Replace("<p>", "").Replace("</p>", "");
                            ChapterContent += book1.ToString().Trim();
                            Console.WriteLine(ChapterContent.Replace("。","。\n\n").Replace("　　", "") + "-------加载完毕！");
                            AddBookToTXT(ChapterContent, bookTitle);
                        }

                    }
                }
                catch (WebException we)
                {
                    //Console.WriteLine(we.Message);
                    Console.WriteLine("远程主机强迫关闭了一个现有的连接,重新爬取当前章节。。。");
                    goto restart;
                }
                Console.WriteLine("请输入指令Y/N/S");
                boole = Console.ReadLine();
            } while (nextChapter.Contains("html") && boole != "N");//当下一章链接没有跳转时结束
        }

        static string url = "http://www.biquge5200.cc/40_40183";
        static string regex6 = "<dd><a href=\"(?<url>.*?)\">(?<name>.*?)</a></dd>";
        static List<book> list = new List<book>();
        //static SortedList<string, string> list2 = new SortedList<string, string>();
        public static List<book> MuluBook() {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.Accept = "text/html;charset=gbk";
            req.AllowAutoRedirect = true;
            req.Headers.Add("Encoding", "gzip,deflate");
            req.Headers[HttpRequestHeader.AcceptLanguage] = "zh-CN,zh";
            req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0)";
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            using (StreamReader reader = new StreamReader(res.GetResponseStream(), System.Text.Encoding.Default))
            {
                string html = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(html))
                {

                    MatchCollection match = Regex.Matches(html, regex6);

                    foreach (Match item in match)
                    {
                        string book = Regex.Match(item.Value, regex6).Groups["name"].ToString();
                        list.Add(new book() { name=book, url=Regex.Match(item.Value, regex6).Groups["url"].ToString() });
                        //list2.Add(book, Regex.Match(item.Value, regex6).Groups["url"].ToString());
                    }


                }
                return list;
            }


        }

        /// <summary>
        /// 将内容保存到txt文件
        /// </summary>
        /// <param name="logstring">内容</param>
        /// <param name="pathName">书名</param>
        public static void AddBookToTXT(string logstring, string pathName)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + pathName + ".txt";
            if (!File.Exists(path))
            {
                FileStream stream = File.Create(path);
                stream.Close();
                stream.Dispose();
            }
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(logstring);
            }
        }
        //定一个解码gzip压缩格式网页的方法
        private static string getGzip(string u)
        {
            StringBuilder sb = new StringBuilder(204800);//200K对于频繁拼接的字符串，用stringbuilder比string节约内存和提升性能
            WebClient wc = new WebClient();//定义一个发送和接收web数据的公用方法类。
            wc.Headers[HttpRequestHeader.AcceptEncoding] = "gzip,deflate";//接收gzip类型的数据
            wc.Headers[HttpRequestHeader.AcceptLanguage] = "zh-CN,zh";//指定请求头的语言类型为中文， 
            wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            byte[] buffer = wc.DownloadData(u);//将 wc对象的downloaddata()方法下载到的资源存入本地buffer中
            GZipStream g = new GZipStream((Stream)(new MemoryStream(buffer)), CompressionMode.Decompress);//定义一个压缩或者解压流的对象，设置为解压
            byte[] tmpbuffer = new byte[buffer.Length];//定一个20K的临时字节数组
            int len = g.Read(tmpbuffer, 0, tmpbuffer.Length); // 
            while (len > 0)
            {
                sb.Append(Encoding.Default.GetString(tmpbuffer, 0, len));  //转换成相应的格式，比如使用的是GBK我们默认就是default,如果是UTF-8就写成UTF-8。这个可以通过右键查看源码找到编码格式。
                len = g.Read(tmpbuffer, 0, 20480);
            }
            g.Close();
            return sb.ToString();
        }


        public static int addintsum(int a,int b) {
            return a + b;
        
        }



    }
    public class book{
        public string name { get; set; }
        public string url { get; set; }
}
}
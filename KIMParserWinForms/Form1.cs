using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static KIMParserWinForms.LOCAL_DB;

namespace KIMParserWinForms
{
    public partial class Form1 : Form
    {
        private string connectionString = @"Data Source=d:\mydb.db;";

        public string NewDocumentTextForMeToPlayWith { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            //string word = "ru"; //какое-то слово
            //if (!e.Url.ToString().Contains(word)) //если не содержит
            //    e.Cancel = true; //не переходим
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (var client = new CookieAwareWebClient())
            {
                string url = "http://oge.fipi.ru/os/xmodules/qprint/openlogin.php?proj=2F5EE3B12FE2A0EA40B06BF61A015416";
                client.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                client.DownloadData(url);
                var cookies = client.CookieContainer.GetCookies(new Uri(url));
                var prefCookie = cookies["PHPSESSID"];
                webBrowser1.Navigate(url, "", null, "Cookie: " + prefCookie.Value + Environment.NewLine);
                ////string url = "http://oge.fipi.ru/os/xmodules/qprint/openlogin.php?proj=2F5EE3B12FE2A0EA40B06BF61A015416";
                //string url = "http://www.fipi.ru/content/otkrytyy-bank-zadaniy-oge";
                //client.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                //client.DownloadData(url);
                //var cookies = client.CookieContainer.GetCookies(new Uri(url));
                //var prefCookie = cookies["PHPSESSID"];
                //if (prefCookie != null)
                //    webBrowser1.Navigate(url, "", null, "Cookie: " + prefCookie.Value + Environment.NewLine);
                ////webBrowser1.Navigate("http://oge.fipi.ru/os/xmodules/qprint/index.php?proj=2F5EE3B12FE2A0EA40B06BF61A015416", "", null, "Cookie: " + prefCookie.Value + Environment.NewLine);
            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            byte[] buffer = null;
            string fileMp3Name = "";
            string subject = "";
            string theme = "";
            bool hasAudio = false;
            HtmlElementCollection htmlcol = ((WebBrowser)sender).Document.GetElementsByTagName("TABLE");
            StringBuilder sb = new StringBuilder();
            Regex regexPath = new Regex(@"ShowPictureQ2WH\('(.+[.mp3])'");
            Regex regexFile = new Regex(@"[^\/\/]*$");
            for (int i = 0; i < htmlcol.Count; i++)
            {
                if (!htmlcol[i].InnerText.Contains("Открытый банк заданий") && htmlcol[i].InnerText.Length > 10)
                {
                    Match mp = regexPath.Match(htmlcol[i].InnerHtml);
                    if (mp.Success == true)
                    {
                        hasAudio = true;
                        string rootMp3Path = @"http://oge.fipi.ru/os/";
                        using (var client = new WebClient())
                        {
                            Match mf = regexFile.Match(mp.Groups[1].Value);
                            fileMp3Name = mf.Value;
                            buffer = new WebClient().DownloadData(rootMp3Path + mp.Groups[1].Value);
                        }
                    }
                    // Извлекаем название предмета
                    List<HtmlElement> subjhtml = GetElementsByTagAndClassName(((WebBrowser)sender).Document, "span", "projname");
                    if (subjhtml != null && subjhtml.Count != 0)
                    {
                        Regex regexSubject = new Regex(@"^ Открытый банк заданий ГИА-9 \/(.+)");
                        Match ms = regexSubject.Match(subjhtml[0].InnerText);
                        if (ms.Success == true)
                        {
                            subject = ms.Groups[1].Value;
                        }
                    }
                    // Извлекаем название темы
                    List<HtmlElement> themehtml = GetElementsByTagAndClassName(((WebBrowser)sender).Document, "table", "header");
                    if (themehtml != null && themehtml.Count != 0)
                    {
                        Regex regex = new Regex(@".+?(?= \()");
                        Match mt = regex.Match(themehtml[0].InnerText);
                        if (mt.Success == true)
                        {
                            theme = mt.Value;
                        }
                    }
                    sb.Append(htmlcol[i].InnerText)
                        .Append(Environment.NewLine)
                        .Append("==========================================================================================")
                        .Append(Environment.NewLine);
                    //StoreToDatabase(fileMp3Name, buffer, htmlcol[i].InnerText, subject, theme, hasAudio);
                    hasAudio = false;
                }
            }
            string path = @"d:\lolparse.txt";
            if (!File.Exists(path))
            {
                File.WriteAllText(path, sb.ToString());
            }
            File.AppendAllText(path, sb.ToString());

            //WalkSubjects();
            //webBrowser1.Navigate("http://oge.fipi.ru/os/xmodules/qprint/index.php?proj=0E1FA4229923A5CE4FC368155127ED90");
            //webBrowser1.Navigate("http://oge.fipi.ru/os/xmodules/qprint/index.php?proj=0FA4DA9E3AE2BA1547B75F0B08EF6445");
            //webBrowser1.Navigate("http://oge.fipi.ru/os/xmodules/qprint/index.php?proj=74676951F093A0754D74F2D6E7955F06");
            //webBrowser1.Navigate("http://oge.fipi.ru/os/xmodules/qprint/index.php?proj=7FF0B02E53DFBCDE4F56B0148BE9A236");
            //webBrowser1.Navigate("http://oge.fipi.ru/os/xmodules/qprint/index.php?proj=3CBBE97571208D9140697A6C2ABE91A0");
            //webBrowser1.Navigate("http://oge.fipi.ru/os/xmodules/qprint/index.php?proj=6B2CD4C77304B2A3478E5A5B61F6899A");
            //webBrowser1.Navigate("http://oge.fipi.ru/os/xmodules/qprint/index.php?proj=DE0E276E497AB3784C3FC4CC20248DC0");
            //webBrowser1.Navigate("http://oge.fipi.ru/os/xmodules/qprint/index.php?proj=A2AC67AE354EBC5242C49482CBC13451");
            //webBrowser1.Navigate("http://oge.fipi.ru/os/xmodules/qprint/index.php?proj=AE63AB28A2D28E194A286FA5A8EB9A78");
            //webBrowser1.Navigate("http://oge.fipi.ru/os/xmodules/qprint/index.php?proj=2F5EE3B12FE2A0EA40B06BF61A015416");
            //webBrowser1.Navigate("http://oge.fipi.ru/os/xmodules/qprint/index.php?proj=B24AFED7DE6AB5BC461219556CCA4F9B");
            //webBrowser1.Navigate("http://oge.fipi.ru/os/xmodules/qprint/index.php?proj=2A4C52ED5AC1ADA644B8BBF169FEC0FC");
            //webBrowser1.Navigate("http://oge.fipi.ru/os/xmodules/qprint/index.php?proj=33B3A93C5A6599124B04FB95616C835B");
            //MessageBox.Show(sb.ToString());
        }

        private void WalkSubjects()
        {
            foreach (HtmlElement link in webBrowser1.Document.Links)
            {
                link.Click += new HtmlElementEventHandler(this.LinkClicked);
            }
        }

        private void LinkClicked(object sender, HtmlElementEventArgs e)
        {
            //while (webBrowser1.ReadyState != WebBrowserReadyState.Complete)
            //{
            //    Application.DoEvents();
            //};

            //Thread.Sleep(1000);

            ////StreamReader sr = new StreamReader(this.webBrowser1.DocumentStream, Encoding.GetEncoding("Windows-1251"));
            ////string source = sr.ReadToEnd();
            //MessageBox.Show(NewDocumentTextForMeToPlayWith);
        }

        private void StoreToDatabase(string filename, byte[] buffer, string task, string subjectName, string theme, bool hasAudio)
        {
            DataConnection dc = null;
            try
            {
                SQLiteTools.CreateDatabase(@"d:\mydb.db", false);
                using (dc = SQLiteTools.CreateDataConnection(connectionString))
                using (var db = new LOCAL_DB(dc.DataProvider, connectionString))
                {
                    var sp = db.DataProvider.GetSchemaProvider();
                    var dbSchema = sp.GetSchema(db);
                    if (!dbSchema.Tables.Any(t => t.TableName == "KIMStorage"))
                    {
                        db.CreateTable<KIMStorage>();
                    }
                    db.kIMStorages.Insert(() => new KIMStorage
                    {
                        HasAudio = hasAudio,
                        AudioFile = buffer,
                        KimId = Guid.NewGuid(),
                        Subject = subjectName,
                        Theme = theme,
                        Task = task,
                        CreateTime = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                string status = string.Format("При выполнении запроса к базе данных произошла ошибка: {0}", ex.ToString());
                MessageBox.Show(status);
            }
            finally
            {
                if (dc != null)
                {
                    dc.Close();
                    dc = null;
                }
            }
        }

        private List<HtmlElement> GetElementsByTagAndClassName(HtmlDocument doc, string tag = "", string className = "")
        {
            List<HtmlElement> lst = new List<HtmlElement>();
            bool empty_tag = String.IsNullOrEmpty(tag);
            bool empty_cn = String.IsNullOrEmpty(className);
            if (empty_tag && empty_cn) return lst;
            HtmlElementCollection elmts = empty_tag ? doc.All : doc.GetElementsByTagName(tag);
            if (empty_cn)
            {
                lst.AddRange(elmts.Cast<HtmlElement>());
                return lst;
            }
            for (int i = 0; i < elmts.Count; i++)
            {
                if (elmts[i].GetAttribute("className") == className)
                {
                    lst.Add(elmts[i]);
                }
            }
            return lst;
        }
    }

    class CookieAwareWebClient : WebClient
    {
        public CookieAwareWebClient()
        {
            CookieContainer = new CookieContainer();
        }

        public CookieContainer CookieContainer { get; private set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            var httpRequest = request as HttpWebRequest;
            if (httpRequest != null)
            {
                httpRequest.CookieContainer = CookieContainer;
            }
            return request;
        }
    }
}

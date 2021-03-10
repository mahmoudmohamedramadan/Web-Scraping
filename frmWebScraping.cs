using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace Scraping_Revision {
    public partial class frmWebScraping : Form {
        public frmWebScraping() {
            InitializeComponent();
        }
        bool enter = true;
        private HtmlNode GetPageHtml() {
            Uri uri_target = new Uri("https://haraj.com.sa");
            WebClient wc = new WebClient() { Encoding = Encoding.UTF8 };
            string html = wc.DownloadString(uri_target);
            var docs = new HtmlAgilityPack.HtmlDocument();
            docs.LoadHtml(html);
            return docs.DocumentNode;
        }
        private void btnGetData_Click(object sender, EventArgs e) {
            try {
                List<HtmlNode> listPostsNode = new List<HtmlNode>();
                listPostsNode = GetPageHtml().Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("tagMain")).First().Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("postlist")).First().Descendants("div").Where(x => x.GetAttributeValue("class", "").Contains("post  ")).ToList();
                foreach (var post in listPostsNode) {
                    //get post title
                    string postTitle = post.Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("postInfo")).First().Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("postTitle")).First().Element("a").InnerText;
                    //get post time
                    string postTime = post.Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("postInfo")).First().Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("postExtraInfo")).First().Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("postExtraInfoPart")).Last().Element("span").InnerText;
                    //get post's owner location
                    string postOwnerLocation = post.Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("postInfo")).First().Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("postExtraInfo")).Last().Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("postExtraInfoPart")).First().Element("a").InnerText;
                    //get post's owner name
                    string postOwnerName = post.Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("postInfo")).First().Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("postExtraInfo")).Last().Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("postExtraInfoPart")).Last().Element("a").InnerText;
                    //get post image
                    string postImage = post.Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("postImg")).First().Element("a").Element("img").GetAttributeValue("src", "");
                    WebRequest request = WebRequest.Create(postImage);
                    using (WebResponse response = request.GetResponse()) {
                        using (Stream stream = response.GetResponseStream()) {
                            DGVData.Rows.Add(postOwnerName, postTitle, postOwnerLocation, postTime, Image.FromStream(stream));
                        }
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnGetCityData_Click(object sender, EventArgs e) {
            try {
                if (enter) {
                    frmWaitData frm = new frmWaitData();
                    frm.Show();
                    List<HtmlNode> listCityNodes = new List<HtmlNode>();
                    listCityNodes = GetPageHtml().Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("filters")).First().Descendants("select").Where(x => x.GetAttributeValue("class", "").Equals(" form-control-alt")).First().Descendants("option").ToList();
                    foreach (var city in listCityNodes) {
                        cmBoxCity.Items.Add(city.InnerText);
                    }
                    frm.Close();
                    enter = false;
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using ODKnew.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ODKnew.Controllers
{
        
    public class OdkController : ApiController
    {

        [BasicAuthentication]
        [Route("downloadform/{letak}")]
        [HttpGet]
        public HttpResponseMessage Downloads(string letak)
        {
            string text = File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\forms\\"+Base64Decode(letak)+".xml"));
            return new HttpResponseMessage() { Content = new StringContent(text, Encoding.UTF8, "text/xml"), StatusCode = HttpStatusCode.OK };
            //return new HttpResponseMessage() { Content = new StringContent(text), StatusCode = HttpStatusCode.OK };

        }
        [BasicAuthentication]
        [Route("downloadmanifest/{letak}")]
        [HttpGet]
        public HttpResponseMessage kemenangan(string letak)
        {
            
            if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\forms\\" + Base64Decode(letak))))
            {
                using (var stringWriter = new StringWriter())

                using (XmlWriter writer = XmlWriter.Create(stringWriter))
                {
                    writer.WriteStartElement("manifest", "http://openrosa.org/xforms/xformsManifest");
                    writer.WriteAttributeString("xmlns", "http://openrosa.org/xforms/xformsManifest");
                    string[] array1 = Directory.GetFiles(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\forms\\" + Base64Decode(letak) ));
                    foreach (string a in array1)
                    {
                        string md5 = "md5:" + MD5Hash(File.ReadAllText(@a));
                        writer.WriteStartElement("mediafile");
                        writer.WriteElementString("filename", a.Split('\\')[a.Split('\\').Length-1]);
                        writer.WriteElementString("hash", "md5:"+md5);
                        writer.WriteElementString("downloadUrl", "https://pandakecil.localtunnel.me/downloadfile/" + letak+"/"+Base64Encode(a.Split('\\')[a.Split('\\').Length - 1]));
                        writer.WriteEndElement();

                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();
                    return new HttpResponseMessage() { Content = new StringContent(stringWriter.GetStringBuilder().ToString(), Encoding.UTF8, "text/xml"), StatusCode = HttpStatusCode.OK };
                }
            }

            else
            {
                return new HttpResponseMessage() { Content = new StringContent("directory "+Base64Decode(letak)+" not found"), StatusCode = HttpStatusCode.NotFound };
            }
            
        }
        
        [Route("downloadfile/{letak}/{nama}")]
        [ActionName("File")]
        [HttpGet]
        public HttpResponseMessage Foke(string nama,string letak)
        {
               
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new System.IO.FileStream(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\forms\\"+Base64Decode(letak)+"\\"+ Base64Decode(nama)), System.IO.FileMode.Open);
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = Base64Decode(nama)
            };
            return response;
        }
        [BasicAuthentication]
        [Route("formList")]
        [HttpGet]
        public HttpResponseMessage Gendeng()
        {
            if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\forms")))
            {
                using (var stringWriter = new StringWriter())

                using (XmlWriter writer = XmlWriter.Create(stringWriter))
                {
                    writer.WriteStartElement("xforms", "http://openrosa.org/xforms/xformsList");
                    writer.WriteAttributeString("xmlns", "http://openrosa.org/xforms/xformsList");
                    string[] array1 = Directory.GetFiles(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\forms"),"*.xml");
                    foreach (string a in array1) {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(File.ReadAllText(@a));
                        XmlNodeList punuk = doc.GetElementsByTagName("instance");
                        string name= doc.GetElementsByTagName("h:title")[0].InnerText;
                        string id = punuk[0].FirstChild.Name;
                        string version = punuk[0].FirstChild.Attributes["version"]==null?null: punuk[0].FirstChild.Attributes["version"].Value;
                        string md5 = "md5:" + MD5Hash(File.ReadAllText(@a));
                        string downloadForm = id;
                        writer.WriteStartElement("xform");
                        writer.WriteElementString("name", name);
                        writer.WriteElementString("formID", id);
                        writer.WriteElementString("hash", md5);
                        if (version != null) {
                            writer.WriteElementString("version", version);
                        }
                        writer.WriteElementString("downloadUrl", "https://pandakecil.localtunnel.me/downloadform/"+Base64Encode(id));
                        
                        if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\forms\\" +id + "-media")))
                        {
                            writer.WriteElementString("manifestUrl", "https://pandakecil.localtunnel.me/downloadmanifest/" + Base64Encode(id + "-media"));
                        }
                        writer.WriteEndElement();

                    }
                    
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();
                    return new HttpResponseMessage() { Content = new StringContent(stringWriter.GetStringBuilder().ToString(), Encoding.UTF8, "text/xml"), StatusCode = HttpStatusCode.OK };
                }
            }
            else {
                return new HttpResponseMessage() { Content = new StringContent("Directory forms not found"), StatusCode = HttpStatusCode.NotFound };
            }
        }
        [Route("submission")]
        [HttpHead]
        public HttpResponseMessage Posos(string deviceID)
        {
            string lucu;
            try {
                lucu=HttpContext.Current.Request.Headers.GetValues("Authorization")[0];
            }
            catch (Exception e) {
                lucu = null;
            }
            if (lucu != null)
            {
                HttpContext.Current.Response.AppendHeader("X-Openrosa-Version", "1.0");
                HttpContext.Current.Response.AppendHeader("Location", "https://pandakecil.localtunnel.me/submission?deviceID="+HttpUtility.UrlEncode(deviceID) );
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.NoContent };
            }
            else {
                HttpContext.Current.Response.AppendHeader("X-Openrosa-Version", "1.0");
                HttpContext.Current.Response.AppendHeader("Www-Authenticate", "Basic realm=ODKnew");
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.Unauthorized };
            }
            
          }
        [BasicAuthentication]
        [Route("submission")]
        [HttpPost]
        public HttpResponseMessage RequestBody(string deviceID)
        {
          
            var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
            bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
            var bodyText = bodyStream.ReadToEnd();
            var kosim = HttpContext.Current.Request.ContentType.Split(';')[1].Split('=')[1];
            var cuki = bodyText.Split(new String[] { "--" + kosim }, StringSplitOptions.None)[1].Split(
    new[] { System.Environment.NewLine },
    StringSplitOptions.None
)[5];
            if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\submissions\\" + deviceID.Replace(':', '-'))))
            {
                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\submissions\\" + deviceID.Replace(':', '-')));
                System.IO.File.WriteAllText(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\submissions\\" + deviceID.Replace(':', '-') + "\\data.xml"), cuki);
                saveSubmission(deviceID.Replace(':', '-'));
            }
            else {
                System.IO.File.WriteAllText(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\submissions\\" + deviceID.Replace(':', '-') + "\\data.xml"), cuki);
                saveSubmission(deviceID.Replace(':', '-'));
            }
            var files = HttpContext.Current.Request.Files.Count > 1 ?
         HttpContext.Current.Request.Files : null;
            if (files != null) {
                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\submissions\\" + deviceID.Replace(':', '-') + "\\files")))
                {
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\submissions\\" + deviceID.Replace(':', '-') + "\\files"));
                    for (var i = 1; i < files.Count; i++)
                    {
                        if (files[i].ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(files[i].FileName);

                            var path = Path.Combine(
                                System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\submissions\\" + deviceID.Replace(':', '-') + "\\files"),
                                fileName
                            );

                            files[i].SaveAs(path);
                        }
                    }
                }
                else {
                    for (var i = 1; i < files.Count; i++)
                    {
                        if (files[i].ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(files[i].FileName);

                            var path = Path.Combine(
                                System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\submissions\\" + deviceID.Replace(':', '-') + "\\files"),
                                fileName
                            );

                            files[i].SaveAs(path);
                        }
                    }
                }
                               
            
            }
            return new HttpResponseMessage() { Content=new StringContent("saved"), StatusCode= HttpStatusCode.Created };
        }
        [Route("membacaxml")]
        [HttpGet]
        public HttpResponseMessage RequestBodys()
        {
            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.Load(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\submissions\\imei-863673039095013\\data.xml"));
            //string nodeList = xmlDoc.DocumentElement.Name;
            
            return new HttpResponseMessage() { Content = new StringContent("peredise"), StatusCode = HttpStatusCode.Created };
        }
        

        [Route("semua")]
        [HttpGet]
        public HttpResponseMessage Lucu()
        {
            IList<Book> books;
            using (ISession session = NHibernateSession.OpenSession())  // Open a session to conect to the database
            {
                books = session.Query<Book>().ToList(); //  Querying to get all the books
            }
                      
            return new HttpResponseMessage { Content = new StringContent(books[0].Title), StatusCode = HttpStatusCode.OK };
            
        }
        [Route("semua/add")]
        [HttpGet]
        public HttpResponseMessage Lucu2()
        {
            IList<Book> books;
            Book book = new Book();     //  Creating a new instance of the Book
            book.Title = "sss";
            book.Genre = "sasa";
            book.Author = "sasasa";
            using (ISession session = NHibernateSession.OpenSession())  // Open a session to conect to the database
            {
                using (ITransaction transaction = session.BeginTransaction())   //  Begin a transaction
                {
                    session.Save(book); //  Save the book in session
                    transaction.Commit();   //  Commit the changes to the database
                }
                
            }
            return new HttpResponseMessage { Content = new StringContent("celesai"), StatusCode = HttpStatusCode.OK };

        }
        [Route("createxml")]
        [HttpGet]
        public HttpResponseMessage Lucu4()
        {
            CreateOdkForm punuk = new CreateOdkForm();
            List<FormItem> pakan = new List<FormItem>();
            pakan.Add(new FormItem() {  value = "horror",label="horror" });
            pakan.Add(new FormItem() {  value = "fantasy",label="fantasy" });
            pakan.Add(new FormItem() {  value = "funny",label="funny" });
            punuk.createInputForm("title","whats the title?",new Tipe().TipeString,new Required().TRUE,null);
            punuk.createSelect1Form("genre", "what is the genre?", null, null, null, pakan);
            punuk.createInputForm("author", "whats the name of the author?", new Tipe().TipeString, new Required().TRUE, null);
            return new HttpResponseMessage { Content = new StringContent(punuk.createTheForm("kampan",1).ToString()), StatusCode = HttpStatusCode.OK };
        }
        [Route("lanjutkanwes")]
        [HttpGet]
        public HttpResponseMessage Lucu3()
        {
            using (var stringWriter = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create(stringWriter))
            {
                //XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                

                //XmlSerializer serializer = new XmlSerializer(typeof(Hhtml));
                //Hhtml po = new Hhtml();
                //Bind kolo = new Bind();
                //kolo.nodeset = "campan";
                //Bind kolo2 = new Bind();
                //kolo2.nodeset = "campan";
                //Bind kolo3 = new Bind();
                //kolo3.nodeset = "campan";
                //List<Bind> kancing = new List<Bind>();
                //kancing.Add(kolo);
                //kancing.Add(kolo2);
                //kancing.Add(kolo3);
                //Model pancing = new Model();
                
                //pancing.bind=kancing;
                //po.model = pancing;
                ////bindeng kolo = new bindeng();
                ////kolo.refrain = "cukil";
                ////bindeng kolo2 = new bindeng();
                ////kolo2.refrain = "cukil";
                ////bindeng kolo3 = new bindeng();
                ////kolo3.refrain = "cukil";
                ////bindeng[] lapis = { kolo,kolo2,kolo3};
                ////po.head=lapis;

                 XNamespace semelenes = "http://www.w3.org/2002/xforms";
                XNamespace h = "http://www.w3.org/1999/xhtml";
                XNamespace jr = "http://openrosa.org/javarosa";
                XNamespace orx = "http://openrosa.org/xforms";
                XNamespace odk = "http://opendatakit.org/xforms";
                XNamespace xsd = "http://www.w3.org/2001/XMLSchema";
                XElement root = new XElement(h+"html");
                root.Add(new XAttribute("xmlns", semelenes));
                root.Add(new XAttribute(XNamespace.Xmlns + "h", h));
                root.Add(new XAttribute(XNamespace.Xmlns + "jr", jr));
                root.Add(new XAttribute(XNamespace.Xmlns + "orx", orx));
                root.Add(new XAttribute(XNamespace.Xmlns + "odk", odk));
                root.Add(new XAttribute(XNamespace.Xmlns + "xsd", xsd));
                var ndas = new XElement(h + "head");
                ndas.Add(new XElement(h+"title","lucu"));
                var ledom = new XElement( "model");
                var instansi = new XElement("instance");
                var datamu = new XElement("data");
                datamu.Add(new XAttribute("id","lucu"));
                datamu.Add(new XAttribute("version", 1));
                datamu.Add(new XElement("firstname",""));
                datamu.Add(new XElement("lastname", ""));
                datamu.Add(new XElement("age", ""));
                datamu.Add(new XElement("meta", new XElement("instanceID")));
                instansi.Add(datamu);
                ledom.Add(instansi);
                ledom.Add(new XElement("bind",new XAttribute("nodeset", "/data/firstname")
                    , new XAttribute("type", "string")
                    , new XAttribute("required", "true()")));
                ledom.Add(new XElement("bind", new XAttribute("nodeset", "/data/lastname")
                    , new XAttribute("type", "string")));
                ledom.Add(new XElement("bind", new XAttribute("nodeset", "/data/age")
                    , new XAttribute("type", "int")));
                ledom.Add(new XElement("bind", new XAttribute("nodeset", "/data/meta/instanceID")
                    , new XAttribute("type", "string")
                    , new XAttribute("preload", "uid")));

                ndas.Add(ledom);
                var awak = new XElement(h + "body");
                awak.Add(new XElement("input",new XAttribute("ref","/data/firstname"),new XElement("label","your name")));
                awak.Add(new XElement("input", new XAttribute("ref", "/data/lastname"), new XElement("label", "your lastname")));
                awak.Add(new XElement("input", new XAttribute("ref", "/data/age"), new XElement("label", "your age")));
                root.Add(ndas);
                root.Add(awak);
                root.Save(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\submissions\\lucu.xml"));
                //return new HttpResponseMessage { Content = new StringContent(stringWriter.GetStringBuilder().ToString(), Encoding.Unicode, "text/xml"), StatusCode = HttpStatusCode.OK };
                return new HttpResponseMessage { Content = new StringContent(root.ToString()), StatusCode = HttpStatusCode.OK };
            }

            

        }
        public Buku submissionData(string path)
        {
            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.Load(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\submissions\\" + path + "\\data.xml"));
            //string nodeList = xmlDoc.DocumentElement.Name;
            //string datanya = xmlDoc.DocumentElement.SelectSingleNode("/" + nodeList + "/" + name).FirstChild.Value;
            
            string pathnda = System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\submissions\\" + path + "\\data.xml");

            XmlSerializer serializer = new XmlSerializer(typeof(Buku));

            StreamReader reader = new StreamReader(pathnda);
            Buku buku = (Buku)serializer.Deserialize(reader);
            reader.Close();
            return buku;
        }
        public void saveSubmission(string path)
        {
            IList<Book> books;
            Book book = new Book();     //  Creating a new instance of the Book
            book.Title = submissionData(path).title;
            book.Genre = submissionData(path).genre;
            book.Author = submissionData(path).author;
            using (ISession session = NHibernateSession.OpenSession())  // Open a session to conect to the database
            {
                using (ITransaction transaction = session.BeginTransaction())   //  Begin a transaction
                {
                    session.Save(book); //  Save the book in session
                    transaction.Commit();   //  Commit the changes to the database
                }

            }
        }
        //hash md5
        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
        //encode base64
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        //decode base64
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }


    }
}

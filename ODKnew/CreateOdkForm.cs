using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Schema;
using System.Xml.Linq;

namespace ODKnew
{
    
    public class CreateOdkForm
    {
        //public string title;
        //public string version;
        public List<FormType> formTipe=new List<FormType>();
        public void createInputForm(string dataName,string label,string tipe,string required,string readOnly) {
            FormType panda = new FormType();
            panda.dataName = dataName;
            panda.name = "input";
            panda.label = label;
            panda.tipe = tipe;
            panda.required = required;
            panda.readOnly = readOnly;
            formTipe.Add(panda);
        }
        public void createUploadForm(string dataName, string label, string tipe, string required, string readOnly) {
            FormType panda = new FormType();
            panda.dataName = dataName;
            panda.name = "upload";
            panda.label = label;
            panda.binaryTipe = tipe;
            panda.required = required;
            panda.readOnly = readOnly;
            formTipe.Add(panda);
        }
        public void createSelectForm(string dataName, string label, string tipe, string required, string readOnly, List<FormItem> item) {
            FormType panda = new FormType();
            panda.dataName = dataName;
            panda.name = "select";
            panda.label = label;
            panda.tipe = tipe;
            panda.required = required;
            panda.readOnly = readOnly;
            panda.item = item;
            formTipe.Add(panda);
        }
        public void createSelect1Form(string dataName, string label, string tipe, string required, string readOnly, List<FormItem> item) {
            FormType panda = new FormType();
            panda.dataName = dataName;
            panda.name = "select1";
            panda.label = label;
            panda.tipe = tipe;
            panda.required = required;
            panda.readOnly = readOnly;
            panda.item = item;
            formTipe.Add(panda);
        }
        public XElement createTheForm(string title,int version)
        {
            XNamespace semelenes = "http://www.w3.org/2002/xforms";
            XNamespace h = "http://www.w3.org/1999/xhtml";
            XNamespace jr = "http://openrosa.org/javarosa";
            XNamespace orx = "http://openrosa.org/xforms";
            XNamespace odk = "http://opendatakit.org/xforms";
            XNamespace xsd = "http://www.w3.org/2001/XMLSchema";
            XElement root = new XElement(h + "html");
            root.Add(new XAttribute("xmlns", semelenes));
            root.Add(new XAttribute(XNamespace.Xmlns + "h", h));
            root.Add(new XAttribute(XNamespace.Xmlns + "jr", jr));
            root.Add(new XAttribute(XNamespace.Xmlns + "orx", orx));
            root.Add(new XAttribute(XNamespace.Xmlns + "odk", odk));
            root.Add(new XAttribute(XNamespace.Xmlns + "xsd", xsd));
            var ndas = new XElement(h + "head");
            ndas.Add(new XElement(h + "title", title));
            var ledom = new XElement("model");
            var instansi = new XElement("instance");
            var datamu = new XElement(title);
            datamu.Add(new XAttribute("id", title));
            datamu.Add(new XAttribute("version", version));
            foreach (FormType element in formTipe)
            {
                datamu.Add(new XElement(element.dataName, ""));
            }
            //datamu.Add(new XElement("firstname", ""));
            //datamu.Add(new XElement("lastname", ""));
            //datamu.Add(new XElement("age", ""));
            datamu.Add(new XElement("meta", new XElement("instanceID")));
            instansi.Add(datamu);
            ledom.Add(instansi);
            foreach (FormType element in formTipe)
            {
                var bind = new XElement("bind");
                bind.Add(new XAttribute("nodeset", "/" + title + "/" + element.dataName));
                
                switch (element.name)
                {
                    case "upload":
                        bind.Add(new XAttribute("type", "binary"));
                        break;
                    case "input":
                        bind.Add(new XAttribute("type", element.tipe));
                        break;
                    case "select":
                        bind.Add(new XAttribute("type", "select"));
                        break;
                    case "select1":
                        bind.Add(new XAttribute("type", "select1"));
                        break;
                }
                if (element.required != null) {
                    bind.Add(new XAttribute("required", element.required));
                }
                if (element.readOnly != null)
                {
                    bind.Add(new XAttribute("readonly", element.readOnly));
                }
                ledom.Add(bind);
                
            }
            //ledom.Add(new XElement("bind", new XAttribute("nodeset", "/data/firstname")
            //    , new XAttribute("type", "string")
            //    , new XAttribute("required", "true()")));
            //ledom.Add(new XElement("bind", new XAttribute("nodeset", "/data/lastname")
            //    , new XAttribute("type", "string")));
            //ledom.Add(new XElement("bind", new XAttribute("nodeset", "/data/age")
            //    , new XAttribute("type", "int")));
            ledom.Add(new XElement("bind", new XAttribute("nodeset", "/"+title+"/meta/instanceID")
                , new XAttribute("type", "string")
                , new XAttribute("preload", "uid")));

            ndas.Add(ledom);
            var awak = new XElement(h + "body");
            foreach (FormType element in formTipe)
            {
                switch (element.name)
                {
                    case "upload":
                        awak.Add(new XElement(element.name, new XAttribute("ref", "/" + title + "/" + element.dataName), new XElement("label", element.label)
                            , new XElement("mediatype", element.binaryTipe)));
                        break;
                    case "input":
                        awak.Add(new XElement(element.name, new XAttribute("ref", "/" + title + "/" + element.dataName), new XElement("label", element.label)));
                        break;
                    case "select":
                        var select = new XElement(element.name);
                        select.Add(new XAttribute("ref", "/" + title + "/" + element.dataName));
                        select.Add(new XElement("label", element.label));
                        foreach (FormItem lucu in element.item) {
                            var item = new XElement("item");
                            item.Add(new XElement("value", lucu.value));
                            if (lucu.fileName != null && lucu.fileType != null)
                            {
                                item.Add(new XElement("value", "jr://"+lucu.fileType+"/" + lucu.fileName, new XAttribute("form", lucu.fileType)));
                            }
                            else {
                                item.Add(new XElement("label", lucu.label));
                            }
                            select.Add(item);
                        }

                        awak.Add(select);
                        break;
                    case "select1":
                        var select1 = new XElement(element.name);
                        select1.Add(new XAttribute("ref", "/" + title + "/" + element.dataName));
                        select1.Add(new XElement("label", element.label));
                        foreach (FormItem lucu in element.item)
                        {
                            var item = new XElement("item");
                            item.Add(new XElement("value", lucu.value));
                            if (lucu.fileName != null && lucu.fileType != null)
                            {
                                item.Add(new XElement("value", "jr://" + lucu.fileType + "/" + lucu.fileName, new XAttribute("form", lucu.fileType)));
                            }
                            else
                            {
                                item.Add(new XElement("label", lucu.label));
                            }
                            select1.Add(item);
                        }

                        awak.Add(select1);
                        break;
                }
                
            }
            //    awak.Add(new XElement("input", new XAttribute("ref", "/data/firstname"), new XElement("label", "your name")));
            //awak.Add(new XElement("input", new XAttribute("ref", "/data/lastname"), new XElement("label", "your lastname")));
            //awak.Add(new XElement("input", new XAttribute("ref", "/data/age"), new XElement("label", "your age")));
            root.Add(ndas);
            root.Add(awak);
            root.Save(System.Web.HttpContext.Current.Server.MapPath("~\\App_Data\\forms\\" + title + ".xml"));
            return root;
        }
    }
    public class FormType {
        public string dataName;
        public string name;
        public string label;
        public string tipe;
        public string required;
        public string readOnly;
        public string binaryTipe;
        public List<FormItem> item;
    }
    public class FormItem {
        public string fileType;
        public string fileName;
        public string label;
        public string value;

    }
    public class FileTypeItem {
        public string image = "image";
        public string audio = "audio";
        public string video = "video";

    }
    

    public class Tipe {
        public string TipeString = "string";
        public string TipeInt = "int";
        public string TipeBoolean = "boolean";
        public string TipeDecimal = "decimal";
        public string TipeDate = "date";
        public string TipeTime = "time";
        public string TipeDateTime = "dateTime";
        public string TipeGeopoint = "geopoint";
        public string TipeGeotrace = "geotrace";
        public string TipeGeoshape = "geoshape";
        public string TipeBinary = "binary";
        public string TipeBarcode = "barcode";
        public string TipeIntent = "intent";
    }
    public class binaryTipe {
        public string image = "image/*";
        public string audio = "audio/*";
        public string video = "video/*";
    }
    public class Required {
        public string TRUE = "true()";
        public string FALSE = "false()";
    }
    public class ReadOnly
    {
        public string TRUE = "true()";
        public string FALSE = "false()";
    }


}
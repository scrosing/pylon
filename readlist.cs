using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;


namespace testup
{
    class Program
    {
        private string getlistxml(string func, string list)
        {
            string ret = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                + "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">"
                + "<soap:Body>";
            ret += "<" + func + " xmlns='http://schemas.microsoft.com/sharepoint/soap/'>"
                + "<listName>" + list + "</listName>"
                + "<queryOptions><QueryOptions>"
                + "<IncludeMandatoryColumns>TRUE</IncludeMandatoryColumns>"
                + "<ViewAttributes Scope='RecursiveAll'/>"
                + "<DateInUtc>TRUE</DateInUtc>"
                + "</QueryOptions></queryOptions>"
                //+ "<updates>"
                //+ "<Batch OnError=\"Continue\">"
                //+ "<Method ID=\"1\" Cmd=\"Delete\">"
                //+ "<Field Name='ID'>9</Field>"
                //+ "<Field Name='FileRef'>sites/DES Bizops/Alpine/DataFiles/Vendor Staffing 20130823 ALP.xlsx</Field>"
                //+ "</Method> </Batch>"
                //+ "</updates>"
                + "</" + func + ">"
                + "</soap:Body>"
                + "</soap:Envelope>";
            return ret;
        }

        private string getContentByName(string strxml, string strname)
        {
            int startid = 0;
            int endid = 0;
            string strtmp = " " + strname + "=";
            startid = strxml.IndexOf(strtmp);
            if (startid > 0)
            {
                startid += strtmp.Length;
                endid = strxml.IndexOf("'", startid + 1);
                if (endid > 0)
                {
                    return strxml.Substring(startid + 1, endid - startid - 1);
                }
            }
            return "";
        }

        static void Main(string[] args)
        {
            string uriString = "";
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(uriString);
            string destpath = "";
            req.Method = "POST";
            req.UseDefaultCredentials = true;
            req.Host = "sharepoint";
            req.ContentType = "text/xml;charset=utf-8";
            string strVendor = "Stream";
            req.Method = "POST";
            req.UseDefaultCredentials = true;
            req.Host = "sharepoint";
            req.ContentType = "text/xml;charset=utf-8";
            req.Headers["SOAPAction"] = "\"http://schemas.microsoft.com/sharepoint/soap/GetListItems\"";
            Program a = new Program();
            string sends = a.getlistxml("GetListItems", strVendor);

            byte[] byteArray = Encoding.UTF8.GetBytes(sends);
            req.ContentLength = byteArray.Length;
            ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            Stream dataStream = req.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            System.Net.WebResponse res = req.GetResponse();
            System.IO.Stream srcFile = res.GetResponseStream();
            int BUFFER_SIZE = 1024 * 1024;
            byte[] buffer = new byte[BUFFER_SIZE];
            int bytesRead = 0;
            string strxml = "";
            while ((bytesRead = srcFile.Read(buffer, 0, BUFFER_SIZE)) > 0)
            {
                strxml += System.Text.Encoding.Default.GetString(buffer, 0, bytesRead);
            }
            int startid = 0;
            int endid = 0;
            int counts = 0;
            startid = strxml.IndexOf("<rs:data ItemCount=");
            endid = strxml.IndexOf("</rs:data>");
            if (startid >= 0)
            {
                strxml = strxml.Substring(startid, endid - startid);
                startid = strxml.IndexOf("\"");
                endid = strxml.IndexOf("\"", startid + 1);
                counts = int.Parse((strxml.Substring(startid + 1, endid - startid - 1)));
                string filename = "";
                string fileref = "";
                int version = 0;
                int sep = 0;
                int fileno = 0;
                for (int i = 0; i < counts; i++)
                {
                    startid = strxml.IndexOf("<z:row");
                    endid = strxml.IndexOf("/>");
                    filename = a.getContentByName(strxml, "ows_LinkFileName");
                    fileref = a.getContentByName(strxml, "ows_FileRef");
                    sep = fileref.IndexOf("#");
                    if (sep > 1)
                    {
                        try
                        {
                            fileno = int.Parse(fileref.Substring(0, sep - 1));
                        }
                        catch
                        {
                            strxml = strxml.Substring(endid, strxml.Length - endid);
                            continue;
                        }
                        fileref = fileref.Substring(sep + 1);
                        fileref.Substring(fileref.IndexOf("#") + 1);
                        if (filename.Length > 15)
                        {
                            if (filename.Substring(0, 15) == "Vendor Staffing")
                            {
                                if (version < int.Parse(filename.Substring(16, 8)))
                                {
                                    version = int.Parse(filename.Substring(16, 8));

                                    string strdown = uriString.Replace("_vti_bin/lists.asmx", strVendor + "/DataFiles/" + filename);
                                    req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strdown);
                                    req.UseDefaultCredentials = true;
                                    res = (System.Net.HttpWebResponse)req.GetResponse();

                                    srcFile = res.GetResponseStream();
                                    System.IO.FileStream destFile = new System.IO.FileStream(destpath + filename, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None, 65536, true);

                                    bytesRead = 0;
                                    while ((bytesRead = srcFile.Read(buffer, 0, BUFFER_SIZE)) > 0)
                                    {
                                        destFile.Write(buffer, 0, bytesRead);
                                    }
                                    destFile.Flush();
                                    srcFile.Close();
                                    destFile.Close();
                                }
                            }
                        }
                    }
                    strxml = strxml.Substring(endid, strxml.Length - endid);
                }
            }
        }
    }
}


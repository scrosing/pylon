using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;



namespace testup
{
    class Program
    {
        static void Main(string[] args)
        {
            string uriString = "";
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(uriString);
            req.Method = "POST";
            req.UseDefaultCredentials = true;
            req.Host = "sharepoint";
            req.ContentType = "text/xml;charset=utf-8";
            string sends = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                + "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">"
                + "<soap:Body>"
                + "<GetListItems xmlns='http://schemas.microsoft.com/sharepoint/soap/'>"
                + "<listName>Shared Documents</listName>"
                + "</GetListItems>"
                + "</soap:Body>"
                + "</soap:Envelope>";
            byte[] byteArray = Encoding.UTF8.GetBytes(sends);
            req.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            req.ContentLength = byteArray.Length;
            Stream dataStream = req.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            System.Net.WebResponse res = req.GetResponse();
            Console.WriteLine(((System.Net.HttpWebResponse)res).StatusDescription);
            Console.WriteLine(res.ContentLength);
            System.IO.Stream srcFile = res.GetResponseStream();
            int BUFFER_SIZE = 1024 * 1024;
            byte[] buffer = new byte[BUFFER_SIZE];
            int bytesRead = 0;
            string strxml = "";
            StringBuilder output = new StringBuilder();
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
                Console.Write(strxml);
                counts = int.Parse((strxml.Substring(startid + 1, endid - startid - 1)));
                string filename = "";
                int version = 0;
                for (int i = 0; i < counts; i++)
                {
                    startid = strxml.IndexOf(" ows_LinkFilename=");
                    if (startid >= 0)
                    {
                        startid += " ows_LinkFilename=".Length;
                        endid = strxml.IndexOf("'", startid + 1);
                        filename = strxml.Substring(startid + 1, endid - startid - 1);
                        if (filename.Substring(0, 15) == "Vendor Staffing")
                        {
                            version = int.Parse(filename.Substring(16, 8));
                        }

                        strxml = strxml.Substring(endid, strxml.Length - endid);
                    }
                }
                Console.Write(strxml);
            }
        }
    }
}

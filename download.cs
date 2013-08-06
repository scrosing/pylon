using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testsp
{
    class Program
    {
        static void Main(string[] args)
        {
            string uriString = "";
            string destpath = "";
            int BUFFER_SIZE = 1024 * 1024;
            int TIMEOUT = 1000 * 60 * 15;
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(uriString);
            req.UseDefaultCredentials = true;
            req.Timeout = TIMEOUT;
            req.ReadWriteTimeout = TIMEOUT;

            System.Net.HttpWebResponse res = (System.Net.HttpWebResponse)req.GetResponse();

            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                System.IO.Stream srcFile = res.GetResponseStream();
                System.IO.FileStream destFile = new System.IO.FileStream(destpath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None, 65536, true);

                byte[] buffer = new byte[BUFFER_SIZE];
                int bytesRead = 0;
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

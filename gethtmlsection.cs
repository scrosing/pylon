using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace pht
{
    class Program
    {
        static void Main(string[] args)
        {
            string strurl = "http://support-preview.xbox.com/en-US/";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(strurl);
            req.Timeout = System.Threading.Timeout.Infinite;
            req.ReadWriteTimeout = System.Threading.Timeout.Infinite;
            req.KeepAlive = true;
            WebResponse res = (WebResponse)req.GetResponse();

            Stream str = res.GetResponseStream();
            string strfinal = "";

            byte[] b = new byte[1024 * 1024];
            int bl = 0;
            int i = 0;
            string strxml = "";
            while ((bl = str.Read(b, 0, b.Length)) > 0)
            {
                strxml += Encoding.Default.GetString(b, 0, bl);
            }
            int istart = 0;
            int icount = 1;
            int iflag = 0;
            int il = 0;
            string strwd = "";
            istart = strxml.IndexOf("<div class=\"homePage\">");
            for (i = istart + 1; i < strxml.Length && icount > 0; i++)
            {
                if (iflag == 0 && strxml[i] == '<')
                {
                    iflag = 1;
                }
                if (iflag == 1)
                {
                    strwd += strxml[i];
                    if (strxml[i] == '>')
                    {
                        iflag = 0;
                        if (strwd == "</div>")
                        {
                            icount--;
                        }
                        il = "<div>".Length;
                        if (strwd.Length > il && strwd.Substring(0, il - 1) == "<div")
                        {
                            icount++;
                        }
                        strwd = "";
                    }
                }
            }
            if (icount > 0)
            {
                return;
            }
            strxml = strxml.Substring(istart, i - istart).Replace("\r\n", "");
            icount = 1;
            iflag = 0;
            istart = strxml.IndexOf("homePageProductTilesBlock");
            strwd = "";
            if (istart < 0)
            {
                return;
            }

            for (i = istart + 1; i < strxml.Length && icount > 0; i++)
            {
                if (iflag == 0 && strxml[i] == '<')
                {
                    iflag = 1;
                }
                if (iflag == 1)
                {
                    strwd += strxml[i];
                    if (strxml[i] == '>')
                    {
                        if (strwd == "</div>")
                        {
                            icount--;
                        }
                        if (icount == 2 && strwd.Length > 7 && strwd.Substring(0, 7) == "<a href")
                        {
                            istart = strwd.IndexOf("\"");
                            if (istart < 0)
                            {
                                return;
                            }
                            strwd = strwd.Substring(istart + 1, strwd.Length - istart - 1);
                            istart = strwd.IndexOf("\"");
                            if (istart < 0)
                            {
                                return;
                            }
                            strwd = strwd.Substring(0, istart);
                            if (strwd.Substring(0, 1) == "/")
                            {
                                strwd = strurl + strwd.Substring(1, strwd.Length - 1);
                            }
                            req = (HttpWebRequest)HttpWebRequest.Create(strwd);
                            res = (WebResponse)req.GetResponse();

                            str = res.GetResponseStream();

                            b = new byte[1024 * 1024];
                            bl = 0;
                            string strxml0 = "";
                            while ((bl = str.Read(b, 0, b.Length)) > 0)
                            {
                                strxml0 += Encoding.Default.GetString(b, 0, bl);
                            }
                            istart = strxml0.IndexOf("<div id=\"subCategoryRightContainer\"");
                            if (istart < 0)
                            {
                                return;
                            }
                            int j = 0;
                            icount = 1;
                            iflag = 0;
                            for (j = istart + 1; j < strxml0.Length && icount > 0; j++)
                            {
                                if (iflag == 0 && strxml0[j] == '<')
                                {
                                    iflag = 1;
                                }
                                if (iflag == 1)
                                {
                                    strwd += strxml0[j];
                                    if (strxml0[j] == '>')
                                    {
                                        iflag = 0;
                                        if (strwd == "</div>")
                                        {
                                            icount--;
                                        }
                                        il = "<div>".Length;
                                        if (strwd.Length > il && strwd.Substring(0, il - 1) == "<div")
                                        {
                                            icount++;
                                        }
                                        strwd = "";
                                    }
                                }
                            }
                            if (icount > 0)
                            {
                                return;
                            }
                            strxml0 = strxml0.Substring(istart, j - istart).Replace("\r\n", "");
                            strfinal += strxml0;
                            if (istart < 0)
                            {
                                continue;
                            }
                        }
                        il = "<div>".Length;
                        if (strwd.Length > il && strwd.Substring(0, il - 1) == "<div")
                        {
                            icount++;
                        }
                        iflag = 0;
                        strwd = "";
                    }
                }
            }
            strfinal = strxml + "<div>" + strfinal + "</div>";
            Console.Write(strfinal);
            Console.Write("");

        }
    }
}


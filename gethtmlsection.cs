            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(strurl);
            WebResponse res = (WebResponse)req.GetResponse();

            Stream str = res.GetResponseStream();

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
                        int il = "<div>".Length;
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
            string ret = strxml.Substring(istart, i - istart);

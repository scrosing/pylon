string strcf = "../../meta/Schema.xml";
            XmlDocument xml = new XmlDocument();
            xml.Load(strcf);
            XmlNodeList xnl = xml.GetElementsByTagName("table");
            for (int i = 0; i < xnl.Count; i++)
            {
                XmlNode xn = xnl[i];
                TreeNode treeNode;
                TreeNode[] tarr = new TreeNode[1];

                XmlNodeList xncl = xn.SelectNodes("column");
                for (int j = 0; j < xncl.Count; j++)
                {
                    XmlNode xcn = xncl[j];
                    treeNode = new TreeNode(xcn.Attributes[0].Value);
                    Array.Resize(ref tarr, j + 1);
                    tarr[j] = treeNode;
                }
                treeNode = new TreeNode(xn.Attributes[0].Value, tarr);
                treeView1.Nodes.Add(treeNode);
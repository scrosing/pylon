using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace WorldMETL
{
    public partial class _Default : System.Web.UI.Page
    {
        int month;
        int year;

        string[] strTargetMetric = {"CPE Top Box %", "CPE Bottom Box %", "CritSit Top Box %", "CritSit Bottom Box %", "Avg Daily Created Incident Volume"};
        string[] strMetric = { "TB", "BB", "CritTB", "CritBB", "Volume"};
        string[] strFM = { "July", "August", "September", "October", "November", "December", "January", "February", "March", "April", "May", "June" };
        protected void Page_Load(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            month = now.Month;
            year = now.Year;
            month += 6;
            if (month > 12)
            {
                month -= 12;
                year++;
            }

            int way = 5;
            if (way == 1)
            {
                readGeo();
            }
            else if (way == 2)
            {
                readTarget();
            }
            else if (way == 3)
            {
                readCPE();
            }
            else if (way == 4)
            {
                readCritsitCPE();
            }
            else if (way == 5)
            {
                readCloud_CreatedVolum();
            }
            else
            {
                readVolTarget();
            }
        }
        int readCloud_CreatedVolum()
        {
            string strSQL = "SELECT 'May' as [Month], Area, Region, cast([Avg Daily Created Volume] as float) FROM [App_Sandbox].[dbo].[On_Prem_CreatedVolum_YTD] where region<>'unknown';";
            strSQL = strSQL.Replace("%year", year.ToString());
            strSQL = strSQL.Replace("%fm01", (month / 10).ToString());
            strSQL = strSQL.Replace("%fm02", (month % 10).ToString());
            string strConn = ConfigurationManager.ConnectionStrings["dbGeo"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConn);
            DataTable dt = new DataTable();
            try
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand(strSQL, sqlConn);
                SqlDataAdapter sqlAd = new SqlDataAdapter(sqlCmd);
                sqlAd.Fill(dt);
            }
            catch
            {
                return -1;
            }
            finally
            {
                sqlConn.Close();
            }

            return processCloud_CreatedVolum(dt);
        }
        int processCloud_CreatedVolum(DataTable dt)
        {
             if (dt.Rows.Count == 0)
            {
                return -1;
            }
            int iMon = 0;
            int iL2 = 1;
            int iL3 = 2;
            int ivol = 3;

            int i, j, k, l;
            string jsonCPE = "{\"cpe\":[%CPE]}";
            string jsonTime = "{\"month\": %month, \"geo\":[%GEO]}";
            string jsonGeo = "{\"L3\":\"%L3\", \"L2\":\"%L2\", \"Vol\": %vol}";
            string strCPE = "";
            string[] strGeo = new string[12];
            float[] vol = new float[12];
            string line = "";
            for (i = 0; i < strGeo.Length; i++)
            {
                strGeo[i] = "";
                vol[i] = 0;
            }
            for (i = 0; i < dt.Rows.Count; i++)
            {
                for (k = 0; k < strFM.Length; k++)
                {
                    if (dt.Rows[i][iMon].ToString().Contains(strFM[k]))
                    {
                        line = jsonGeo.Replace("%L3", dt.Rows[i][iL3].ToString());
                        line = line.Replace("%L2", dt.Rows[i][iL2].ToString());
                        line = line.Replace("%vol", dt.Rows[i][ivol].ToString());
                        vol[k] += float.Parse(dt.Rows[i][ivol].ToString());
                        if (strGeo[k] != "")
                        {
                            strGeo[k] += ", ";
                        }
                        strGeo[k] += line;
                    }
                }
            }
            for (i = 0; i < strGeo.Length; i++)
            {
                if (vol[i] > 0)
                {
                    line = jsonTime.Replace("%month", (i + 1).ToString());
                    line = line.Replace("%GEO", strGeo[i]);
                    if (strCPE != "")
                    {
                        strCPE += ", ";
                    }
                    strCPE += line;
                }
            }
            Response.Write(jsonCPE.Replace("%CPE", strCPE));
     
            return 0;
        }
        int readVolTarget()
        {
            string strSQL = "select MetricName, [Org Inclusion], GeoCustomer, [YTDFM%fm01%fm02] from dbo.MasterTargetFile where servicecluster='premier' and FiscalYear = %year and productcluster ='unknown' order by metricname,[org inclusion],geocustomer";
            strSQL = strSQL.Replace("%year", year.ToString());
            strSQL = strSQL.Replace("%fm01", (month / 10).ToString());
            strSQL = strSQL.Replace("%fm02", (month % 10).ToString());
            string strConn = ConfigurationManager.ConnectionStrings["dbGeo"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConn);
            DataTable dt = new DataTable();
            try
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand(strSQL, sqlConn);
                SqlDataAdapter sqlAd = new SqlDataAdapter(sqlCmd);
                sqlAd.Fill(dt);
            }
            catch
            {
                return -1;
            }
            finally
            {
                sqlConn.Close();
            }

            return processVolTarget(dt);
        }
        int processVolTarget(DataTable dt)
        {
            int i, k;
            int iMet = 0;
            int iOrg = 1;
            int iGeo = 2;
            int iTar = 3;
            int iCount = strTargetMetric.Length;
            string jsonTar = "{\"Tar\":[%TAR]}";
            string jsontar = "{\"Metric\":\"%Met\", \"Tar\": [%tar]}";
            string strtar = "{\"Geo\":\"%geo\", \"Org\":\"%Org\", \"Value\": %val}";
            string[] jsonTargets = new string[iCount];
            for (i = 0; i < iCount; i++)
            {
                jsonTargets[i] = "";
            }
            string line;

            if (dt.Rows.Count == 0)
            {
                return -1;
            }
            for (i = 0; i < dt.Rows.Count; i++)
            {
                for (k = 0; k < strTargetMetric.Length; k++)
                {
                    if (dt.Rows[i][iMet].ToString() == strTargetMetric[k])
                    {
                        line = strtar.Replace("%geo", dt.Rows[i][iGeo].ToString());
                        line = line.Replace("%Org", dt.Rows[i][iOrg].ToString());
                        line = line.Replace("%val", dt.Rows[i][iTar].ToString());
                        if (jsonTargets[k] != "")
                        {
                            jsonTargets[k] += ", ";
                        }
                        jsonTargets[k] += line;
                    }
                }
            }
            line = "";
            for (i = 0; i < iCount; i++)
            {
                if (i > 0)
                {
                    line += ", ";
                }
                line += jsontar.Replace("%Met", strMetric[i]).Replace("%tar", jsonTargets[i]);
            }
            Response.Write(jsonTar.Replace("%TAR", line));

            return 0;
        }
        int readTarget()
        {
            
            string strSQL = "SELECT [MetricName] ,[GeoCustomer] ,[YTDFM%fm01%fm02] FROM [dbo].[MasterTargetFile_Budget] (nolock) where GeoCustomer <> 'Unknown' and [ServiceCluster] = 'Premier' and [ProductCluster] = 'Unknown' and [FiscalYear] = %year order by 1;";
            strSQL = strSQL.Replace("%year", year.ToString());
            strSQL = strSQL.Replace("%fm01", (month / 10).ToString());
            strSQL = strSQL.Replace("%fm02", (month % 10).ToString());
            string strConn = ConfigurationManager.ConnectionStrings["dbGeo"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConn);
            DataTable dt = new DataTable();
            try
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand(strSQL, sqlConn);
                SqlDataAdapter sqlAd = new SqlDataAdapter(sqlCmd);
                sqlAd.Fill(dt);
            }
            catch
            {
                return -1;
            }
            finally
            {
                sqlConn.Close();
            }

            return processTarget(dt);
        }
        int processTarget(DataTable dt)
        {
            int i, k;
            int iMet = 0;
            int iGeo = 1;
            int iTar = 2;
            int iCount = strTargetMetric.Length;
            string jsonTar = "{\"Tar\":[%TAR]}";
            string jsontar = "{\"Metric\":\"%Met\", \"Tar\": [%tar]}";
            string strtar = "{\"Geo\":\"%geo\", \"Value\": %val}";
            string[] jsonTargets = new string[iCount];
            for (i = 0; i < iCount; i++)
            {
                jsonTargets[i] = "";
            }
            string line;

            if (dt.Rows.Count == 0)
            {
                return -1;
            }
            for (i = 0; i < dt.Rows.Count; i++)
            {
                for (k = 0; k < strTargetMetric.Length; k++)
                {
                    if (dt.Rows[i][iMet].ToString() == strTargetMetric[k])
                    {
                        line = strtar.Replace("%geo", dt.Rows[i][iGeo].ToString());
                        line = line.Replace("%val", dt.Rows[i][iTar].ToString());
                        if (jsonTargets[k] != "")
                        {
                            jsonTargets[k] += ", ";
                        }
                        jsonTargets[k] += line;
                    }
                }
            }
            line = "";
            for (i = 0; i < iCount; i++)
            {
                if (i > 0)
                {
                    line += ", ";
                }
                line += jsontar.Replace("%Met", strMetric[i]).Replace("%tar", jsonTargets[i]);
            }
            Response.Write(jsonTar.Replace("%TAR", line));

            return 0;
        }
        int readGeo()
        {
            string strSQL = "select distinct [DetegoLevel2], [DetegoLevel3], [ISOCountryShortCode] from [dbo].[CubeDimGeography] (nolock) where len([ISOCountryShortCode]) = 2 order by 1, 2, 3";
            string strConn = ConfigurationManager.ConnectionStrings["dbGeo"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConn);
            DataTable dt = new DataTable();
            try
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand(strSQL, sqlConn);
                SqlDataAdapter sqlAd = new SqlDataAdapter(sqlCmd);
                sqlAd.Fill(dt);
            }
            catch
            {
                return -1;
            }
            finally
            {
                sqlConn.Close();
            }
            
            return processGeo(dt);
        }
        int processGeo(DataTable dt)
        {
            string jsonGeo = "{\"Geo\":[%REGION]}";
            string jsonRegion = "{\"L3\":\"%L3\", \"L2\":\"%L2\", \"L4\":[%L4]}";
            int iL2 = 0;
            int iL3 = 1;
            int iL4 = 2;
            int i;
            string strRegion = "";
            string strRegions = "";
            string strCountry = "";
            
            if (dt.Rows.Count == 0)
            {
                return -1;
            }
            i = 0;
            strRegion = jsonRegion.Replace("%L2", dt.Rows[i][iL2].ToString());
            strRegion = strRegion.Replace("%L3", dt.Rows[i][iL3].ToString());
            strCountry = "\"" + dt.Rows[i][iL4].ToString() + "\"";
            for (i = 1; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][iL3].ToString() != dt.Rows[i - 1][iL3].ToString() || i == dt.Rows.Count - 1)
                {
                    if (strRegions != "")
                    {
                        strRegions += ", ";
                    }
                    strRegion = strRegion.Replace("%L4", strCountry);
                    strRegions += strRegion;
                    strRegion = jsonRegion.Replace("%L2", dt.Rows[i][iL2].ToString());
                    strRegion = strRegion.Replace("%L3", dt.Rows[i][iL3].ToString());
                    strCountry = "\"" + dt.Rows[i][iL4].ToString() + "\"";
                    continue;
                }
                strCountry += ", \"" + dt.Rows[i][iL4].ToString() + "\""; 
            }
            jsonGeo = jsonGeo.Replace("%REGION", strRegions);

            Response.Write(jsonGeo);
            return 0;
        }
        int readCPE()
        {
            string strSQL = "select [Fiscal Month] , [Gegion], [Area], round([TB Count], 0) as [TB Count], round([BB Count], 0) as [BB Count], [Survey Response Count] from [dbo].[IBSCPE] (nolock) where [Fiscal Year] >= 'FY %year' and [Fiscal Year] >= 'FY 2013' and [Gegion] <> 'unknown';";
            strSQL = strSQL.Replace("%year", (year - 1).ToString());

            string strConn = ConfigurationManager.ConnectionStrings["dbGeo"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConn);
            DataTable dt = new DataTable();
            try
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand(strSQL, sqlConn);
                SqlDataAdapter sqlAd = new SqlDataAdapter(sqlCmd);
                sqlAd.Fill(dt);
            }
            catch
            {
                return -1;
            }
            finally
            {
                sqlConn.Close();
            }

            return processCPE(dt);
        }
        int processCPE(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                return -1;
            }
            int iMon = 0;
            int iL3 = 1;
            int iL2 = 2;
            int iTB = 3;
            int iBB = 4;
            int iSur = 5;

            int i, j, k, l;
            string jsonCPE = "{\"cpe\":[%CPE]}";
            string jsonTime = "{\"month\": %month, \"geo\":[%GEO]}";
            string jsonGeo = "{\"L3\":\"%L3\", \"L2\":\"%L2\", \"TB\":%tb, \"BB\":%bb, \"Survey\": %sur}";
            string strCPE = "";
            string[] strGeo = new string[12];
            int[] TB = new int[12];
            int[] BB = new int[12];
            int[] Survey = new int[12];
            string line = "";
            for (i = 0; i < strGeo.Length; i++)
            {
                strGeo[i] = "";
                TB[i] = 0;
                BB[i] = 0;
                Survey[i] = 0;
            }
            for (i = 0; i < dt.Rows.Count; i++)
            {
                for (k = 0; k < strFM.Length; k++)
                {
                    if (dt.Rows[i][iMon].ToString().Contains(strFM[k]))
                    {
                        line = jsonGeo.Replace("%L3", dt.Rows[i][iL3].ToString());
                        line = line.Replace("%L2", dt.Rows[i][iL2].ToString());
                        line = line.Replace("%tb", dt.Rows[i][iTB].ToString());
                        line = line.Replace("%bb", dt.Rows[i][iBB].ToString());
                        line = line.Replace("%sur", dt.Rows[i][iSur].ToString());
                        TB[k] += int.Parse(dt.Rows[i][iTB].ToString());
                        BB[k] += int.Parse(dt.Rows[i][iBB].ToString());
                        Survey[k] += int.Parse(dt.Rows[i][iSur].ToString());
                        if (strGeo[k] != "")
                        {
                            strGeo[k] += ", ";
                        }
                        strGeo[k] += line;
                    }
                }
            }
            for (i = 0; i < strGeo.Length; i++)
            {
                if (Survey[i] > 0)
                {
                    line = jsonTime.Replace("%month", (i + 1).ToString());
                    line = line.Replace("%GEO", strGeo[i]);
                    if (strCPE != "")
                    {
                        strCPE += ", ";
                    }
                    strCPE += line;
                }
            }
            Response.Write(jsonCPE.Replace("%CPE", strCPE));
            return 0;
        }
        int readCritsitCPE()
        {
            string strSQL = "select [Fiscal Month] , [Gegion], [Area], round(isnull([TB Count], 0), 0) as [TB Count], round(isnull([BB Count], 0), 0) as [BB Count], [Survey Response Count] from [dbo].[IBSCritsitCPE] (nolock) where [Fiscal Year] = 'FY %year' and [Gegion] <> 'unknown';";
            strSQL = strSQL.Replace("%year", year.ToString());

            string strConn = ConfigurationManager.ConnectionStrings["dbGeo"].ConnectionString;
            SqlConnection sqlConn = new SqlConnection(strConn);
            DataTable dt = new DataTable();
            try
            {
                sqlConn.Open();
                SqlCommand sqlCmd = new SqlCommand(strSQL, sqlConn);
                SqlDataAdapter sqlAd = new SqlDataAdapter(sqlCmd);
                sqlAd.Fill(dt);
            }
            catch
            {
                return -1;
            }
            finally
            {
                sqlConn.Close();
            }

            return processCritsitCPE(dt);
        }
        int processCritsitCPE(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                return -1;
            }
            int iMon = 0;
            int iL3 = 1;
            int iL2 = 2;
            int iTB = 3;
            int iBB = 4;
            int iSur = 5;

            int i, j, k, l;
            string jsonCPE = "{\"cpe\":[%CPE]}";
            string jsonTime = "{\"month\": %month, \"geo\":[%GEO]}";
            string jsonGeo = "{\"L3\":\"%L3\", \"L2\":\"%L2\", \"TB\":%tb, \"BB\":%bb, \"Survey\": %sur}";
            string strCPE = "";
            string[] strGeo = new string[12];
            int[] TB = new int[12];
            int[] BB = new int[12];
            int[] Survey = new int[12];
            string line = "";
            for (i = 0; i < strGeo.Length; i++)
            {
                strGeo[i] = "";
                TB[i] = 0;
                BB[i] = 0;
                Survey[i] = 0;
            }
            for (i = 0; i < dt.Rows.Count; i++)
            {
                for (k = 0; k < strFM.Length; k++)
                {
                    if (dt.Rows[i][iMon].ToString().Contains(strFM[k]))
                    {
                        line = jsonGeo.Replace("%L3", dt.Rows[i][iL3].ToString());
                        line = line.Replace("%L2", dt.Rows[i][iL2].ToString());
                        line = line.Replace("%tb", dt.Rows[i][iTB].ToString());
                        line = line.Replace("%bb", dt.Rows[i][iBB].ToString());
                        line = line.Replace("%sur", dt.Rows[i][iSur].ToString());
                        TB[k] += int.Parse(dt.Rows[i][iTB].ToString());
                        BB[k] += int.Parse(dt.Rows[i][iBB].ToString());
                        Survey[k] += int.Parse(dt.Rows[i][iSur].ToString());
                        if (strGeo[k] != "")
                        {
                            strGeo[k] += ", ";
                        }
                        strGeo[k] += line;
                    }
                }
            }
            for (i = 0; i < strGeo.Length; i++)
            {
                if (Survey[i] > 0)
                {
                    line = jsonTime.Replace("%month", (i + 1).ToString());
                    line = line.Replace("%GEO", strGeo[i]);
                    if (strCPE != "")
                    {
                        strCPE += ", ";
                    }
                    strCPE += line;
                }
            }
            Response.Write(jsonCPE.Replace("%CPE", strCPE));
            return 0;
        }
    }
}

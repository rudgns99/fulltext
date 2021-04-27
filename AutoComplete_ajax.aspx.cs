using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Xml;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Text.RegularExpressions;

public partial class AutoComplete_ajax : WSF.Web.Page
{
    public static readonly Database db = DatabaseFactory.CreateDatabase("ConnectDB");
    protected void Page_Load(object sender, EventArgs e)
    {
        SetNoCachePage();
        string keyword = string.IsNullOrEmpty(Request["q"]) ? string.Empty : Request["q"];
        keyword = keyword.Replace(@"""", @"""""");
        string[] keywords = keyword.Trim().Split(' ');
        string searchCondition = string.Empty;
        for (int i = 0; i < keywords.Length; i++)
        {
            if (i != 0) searchCondition += @" AND """ + keywords[i] + @"""";
            else searchCondition += @"""" + keywords[i] + @"""";
        }

        string JsonStr = "[";
        string SqlQuery = "SELECT TOP 10 A.keyword FROM (SELECT keyword,count FROM TP_KR_SEARCH_LOG WHERE CONTAINS(keyword,@contains_search_condition,LANGUAGE N'Korean')) AS A ORDER BY A.count desc";
        using (DbCommand dbcomm = db.GetSqlStringCommand(SqlQuery))
        {
            db.AddInParameter(dbcomm, "@contains_search_condition", DbType.String, searchCondition);
            using (DataSet ds = db.ExecuteDataSet(dbcomm))
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (i > 0) JsonStr += ",";
                    JsonStr += @"""" + JsonEncode(ds.Tables[0].Rows[i]["keyword"].ToString()) + @"""";
                }
            }
        }
        JsonStr += "]";
        Response.Write(JsonStr);
    }
    private string JsonEncode(string str)
    {
        return str.Replace("'", "\'").Replace(@"""", @"\""").Replace(@"\\", @"\\\\");
    }
}
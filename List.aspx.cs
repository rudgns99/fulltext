using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Microsoft.Practices.EnterpriseLibrary;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;

public partial class List : WSF.Web.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        DisplayPage();
    }
    public static readonly Database db = DatabaseFactory.CreateDatabase("ConnectDB");
    public static List<string> _ListQuery = new List<string> { "kw" };
    protected void DisplayPage()
    {
        int CurrentPage = GetQueryInt("p", 1);

        if (CurrentPage < 1)
            return;
        int CountPerPage = GetQueryInt("cpp", 30);
        int TotalCount = 0;
        string Keyword = GetQuery("kw");
        txt_keyword.Value = Keyword;
        string kw = Keyword.Replace(@"""", @"""""");
        string searchCondition = string.Empty;
        if (kw != string.Empty)
        {
            string[] sentanceSplit = kw.Split(' ');
            for (int i = 0; i < sentanceSplit.Length; i++)
            {
                if (i != 0) searchCondition += @" AND """ + sentanceSplit[i] + @"""";
                else searchCondition += @"""" + sentanceSplit[i] + @"""";
            }
            string SqlQuery = "IF EXISTS(SELECT num FROM TP_KR_SEARCH_LOG WHERE keyword=@search_word) ";
            SqlQuery += "UPDATE TP_KR_SEARCH_LOG SET count=count+1 WHERE keyword=@search_word ";
            SqlQuery += "ELSE INSERT INTO TP_KR_SEARCH_LOG(keyword) VALUES(@search_word)";
            using (DbCommand dbcomm = db.GetSqlStringCommand(SqlQuery))
            {
                db.AddInParameter(dbcomm, "@search_word", DbType.String, Keyword);
                db.ExecuteNonQuery(dbcomm);
            }
        }
        using (DbCommand cmd = db.GetStoredProcCommand("UP_KR_SELECT_GETLIST_TRANSLATIONLIST"))
        {
            db.AddInParameter(cmd, "@CurPage", DbType.Int32, CurrentPage);
            db.AddInParameter(cmd, "@CntPerPage", DbType.Int32, CountPerPage);
            db.AddInParameter(cmd, "@SearchCondition", DbType.String, searchCondition);
            db.AddOutParameter(cmd, "@TotalCount", DbType.Int32, 4);
            using (DataSet ds = db.ExecuteDataSet(cmd))
            {
                TotalCount = WSF.Helper.StringUtil.GetInt(db.GetParameterValue(cmd, "@TotalCount"));
                lvHotlineReportList.DataSource = ds;
                lvHotlineReportList.DataBind();

                WSF.Helper.Pager.DisplayPageBar(litPager, CurrentPage, CountPerPage, TotalCount, 10, WSF.Helper.Pager.GetSelectedQueryString(_ListQuery));
            }
        }
    }

    protected void lvHotlineReportList_ItemBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            Literal litNo = (Literal)e.Item.FindControl("litNo");
            Literal litKorean = (Literal)e.Item.FindControl("litKorean");
            Literal litEnglish = (Literal)e.Item.FindControl("litEnglish");
            Literal litPercent = (Literal)e.Item.FindControl("litPercent");


            string Num = WSF.Helper.StringUtil.GetString(((DataRowView)((ListViewDataItem)e.Item).DataItem)["NUM"]);
            string Korean = WSF.Helper.StringUtil.GetString(((DataRowView)((ListViewDataItem)e.Item).DataItem)["korean_full"]);
            string English = WSF.Helper.StringUtil.GetString(((DataRowView)((ListViewDataItem)e.Item).DataItem)["english_full"]);

            litNo.Text = Num;
            litKorean.Text = Korean;
            litEnglish.Text = English;
        }
    }
}
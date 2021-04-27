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


public partial class Result : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
    public static readonly Database db = DatabaseFactory.CreateDatabase("ConnectDB");
    private string TranslateText(string sentance)
    {
        string applicationid = "A70C584051881A30549986E65FF4B92B95B353A5";
        string fromlanguage = "ko";//어디 언어로부터~
        string translatedText = string.Empty;
        string texttotranslate = sentance;
        string tolanguage = "en";//번역하고싶은 언어
        string uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?appId=" + applicationid + "&text=" + HttpUtility.UrlEncode(texttotranslate, System.Text.Encoding.UTF8) + "&from=" + fromlanguage + "&to=" + tolanguage;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

        WebResponse response = request.GetResponse();

        XmlReader xmlreader = XmlReader.Create(response.GetResponseStream());
        if (xmlreader.ReadToFollowing("string"))
        {
            translatedText = xmlreader.ReadElementContentAsString();
        }
        response.Close();
        return translatedText;
    }
    private string GitTranslateText(string sentance, out decimal percent)
    {
        sentance = sentance.Replace(@"""", @"""""").Trim();
        string translatedText = string.Empty;
        percent = 0;
        using (DbCommand cmdTarget = db.GetSqlStringCommand("SELECT TOP 1 english_full FROM dbo.[TP_KR_FULLTEXT_TRANSLATION] WHERE REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(korean_full,' ',''),'.',''),',',''),'!',''),'?','')=@InputContents"))
        {
            db.AddInParameter(cmdTarget, "@InputContents", DbType.String, sentance.Replace(" ", string.Empty)
                                                                                  .Replace(".", string.Empty)
                                                                                  .Replace(",", string.Empty)
                                                                                  .Replace("!", string.Empty)
                                                                                  .Replace("?", string.Empty));
            translatedText = Convert.ToString(db.ExecuteScalar(cmdTarget));
            percent = 100;
        }
        if (string.IsNullOrEmpty(translatedText))
        {
            string[] sentanceSplit = sentance.Split(' ');
            int sentanceLangthLimit = ((sentanceSplit.Length - 1) * 3) + sentance.Length;
            string searchCondition = string.Empty;
            for (int i = 0; i < sentanceSplit.Length; i++)
            {
                if (i != 0) searchCondition += @" AND """ + sentanceSplit[i] + @"""";
                else searchCondition += @"""" + sentanceSplit[i] + @"""";
            }
            string sc = searchCondition;
            string SqlQuery = "SELECT AA.english_full,(1000-BB.[RANK])/10 AS RK FROM ";
            SqlQuery += "(SELECT * FROM dbo.[TP_KR_FULLTEXT_TRANSLATION] WHERE LEN(korean_full)<@ContentsLength) AS AA ";
            SqlQuery += "INNER JOIN ";
            SqlQuery += "CONTAINSTABLE(dbo.[TP_KR_FULLTEXT_TRANSLATION],korean_full,@contains_search_condition,LANGUAGE N'Korean',1) AS BB ";
            SqlQuery += "ON AA.full_index=BB.[KEY] ";
            SqlQuery += "WHERE BB.[RANK]<=100";
            //Response.Write(SqlQuery);
            //Response.End();
            using (DbCommand cmdTarget = db.GetSqlStringCommand(SqlQuery))
            {
                db.AddInParameter(cmdTarget, "@ContentsLength", DbType.Int32, sentanceLangthLimit);
                db.AddInParameter(cmdTarget, "@contains_search_condition", DbType.String, searchCondition);
                using (DataSet ds = db.ExecuteDataSet(cmdTarget))
                {
                    translatedText = WSF.Data.DBHelper.GetStringFromTable(ds, 0, 0, "english_full");
                    percent = Math.Round(WSF.Data.DBHelper.GetDecimalFromTable(ds, 0, 0, "RK"), 0, MidpointRounding.AwayFromZero);
                }

            }
        }
        return string.IsNullOrEmpty(translatedText) ? string.Empty : translatedText;
    }

    protected void btn_Translate_Click(object sender, EventArgs e)
    {
        if (txttext.Text.Trim() == string.Empty)
        {
            return;
        }
        List<int> resultlist = SentenceIndexList(txttext.Text);
        //// 잘라서 번역하는 부분
        int cutstartindex = 0;
        string sentance = string.Empty;
        decimal percent = 0;
        string fullresulttext = string.Empty;

        for (int i = 0; i < resultlist.Count; i++)
        {
            sentance = txttext.Text.Trim().Substring(cutstartindex, (resultlist[i] + 1) - cutstartindex);

            cutstartindex = resultlist[i] + 1;
            string gt = GitTranslateText(sentance, out percent);
            if (gt != string.Empty)
            {
                if (sentance.Trim(' ').StartsWith("\r\n")) fullresulttext += "<br/>";
                fullresulttext += gt + @" <img src=""Img/git.gif"" style=""height:15px;""/> (" + percent.ToString() + "%)";
            }
            else
            {
                fullresulttext += TranslateText(sentance).Replace("\n", "<br/>") + @" <img src=""Img/bing.gif"" style=""height:15px;""/>";
            }
        }

        h_cc.InnerHtml = fullresulttext;
    }
    protected void btn_Registration_Click(object sender, EventArgs e)
    {
        if (txttext.Text.Trim() == string.Empty)
        {
            return;
        }
        List<int> resultlist = SentenceIndexList(txttext.Text);
        int cutstartindex = 0;
        string sentance = string.Empty;
        string fullresulttext = string.Empty;

        for (int i = 0; i < resultlist.Count; i++)
        {
            sentance = txttext.Text.Substring(cutstartindex, (resultlist[i] + 1) - cutstartindex);
            cutstartindex = resultlist[i] + 1;
            string tlt = TranslateText(sentance);
            string sql = "INSERT INTO dbo.[TP_KR_FULLTEXT_TRANSLATION] (korean_full,english_full) VALUES (@InputContents, @InputEnglish)";
            using (DbCommand cmdTarget = db.GetSqlStringCommand(sql))
            {
                db.AddInParameter(cmdTarget, "@InputContents", DbType.String, sentance.Trim());
                db.AddInParameter(cmdTarget, "@InputEnglish", DbType.String, tlt.Trim());

                db.ExecuteNonQuery(cmdTarget);


            }
        }
        Response.Redirect("Result.aspx");
    }
    private List<int> SentenceIndexList(string text)
    {
        text = text.Trim();
        string reg = @"([가-힣]{1,})(다|요|오|까)(\.|\?|\!)";
        Regex r = new Regex(reg);
        int sindex = 0;
        int resultindex = 0;
        List<int> resultlist = new List<int>();

        MatchCollection mc = r.Matches(text);

        if (mc.Count > 0)
        {
            foreach (Match endword in mc)
            {
                resultindex = text.IndexOf(endword.Value, sindex) + (endword.Length - 1);
                resultlist.Add(resultindex);
                sindex = resultindex + 1;
            }
            if (text.Length - 1 >= sindex)
            {
                resultlist.Add(text.Length - 1);
            }
        }
        else
        {
            resultlist.Add(text.Length - 1);
        }
        return resultlist;
    }
}

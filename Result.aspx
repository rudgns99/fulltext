<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Result.aspx.cs" Inherits="Result" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Css/Style.css" rel="stylesheet" type="text/css" />
    <script src="Js/jquery-1.7.2.min.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table style="width: 760px; border: 0px;" cellspacing="0" cellpadding="0">
                <tr>
                    <td style="height: 10px;"></td>
                </tr>
            </table>
            <table style="width: 760px; border: 0px;" cellspacing="0" cellpadding="0">
                <tr>
                    <td style="height: 30px">
                        <span class="subject1">영작</span>
                    </td>
                </tr>
                <tr>
                    <td style="height: 2px; background-color: #e2e2e2;"></td>
                </tr>
                <tr>
                    <td style="padding-top: 5px; padding-bottom: 5px;">
                        <a href="List.aspx" target="_blank">문장 리스트</a></td>
                </tr>
            </table>
            <table style="width: 760px;" cellspacing="0" cellpadding="0" class="lv_list">
                <tr>
                    <td style="text-align: center; width: 120px;">Korean
                    </td>
                    <td style="width: 515px;">
                        <asp:TextBox ID="txttext" runat="server" Width="508px" Height="69px" TextMode="MultiLine" MaxLength="2000"></asp:TextBox>
                    </td>
                    <td rowspan="2" style="text-align: center;">&nbsp;<asp:Button ID="btn_Translate" runat="server" OnClick="btn_Translate_Click" Text="영작" BackColor="#99CCFF" />
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center; width: 120px;">English
                    </td>
                    <td style="width: 515px;">
                        <div id="h_cc" runat="server" style="width: 508px; height: 69px;"></div>
                    </td>

                </tr>
            </table>
            <asp:Button ID="btn_Registration" runat="server" Text="등록" OnClick="btn_Registration_Click" BackColor="#99CCFF" />
            &nbsp;<br />
            종결 어미+종결 부호 : 다. 요. 오. 까. 다? 요? 오? 까? 다! 요! 오! 까!<br />
            여러 문장 영작 시에는 종결 어미+종결를 기준으로 문장을 분리하여 문장 별로 영작이 실행됩니다.<br />
            종결 어미+종결 부호가 없을 경우 한문장으로 영작이 실행됩니다.
        </div>
    </form>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#txttext').keydown(function (e) {
                if ($(this).val() == '' && (e.keyCode == 32 || e.keyCode == 13)) {
                    return false;
                }
            });
        });
    </script>
</body>
</html>

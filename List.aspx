<%@ Page Language="C#" AutoEventWireup="true" CodeFile="List.aspx.cs" Inherits="List" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="/Css/Style.css" rel="stylesheet" type="text/css" />
    <link href="Css/jquery-ui.min.css" rel="stylesheet" />
    <script src="Js/jquery-1.7.2.min.js"></script>
    <script src="Js/jquery-ui.min.js"></script>
</head>
<body>
    <form id="form1" runat="server" onsubmit="return false;">
        <div id="DivVisitReport" class="stdDiv_Type7" style="width: 765px">
            <table style="width: 760px; border: 0px;" cellspacing="0" cellpadding="0">
                <tr>
                    <td style="height: 30px">
                        <span class="subject1">리스트</span>
                    </td>
                </tr>
                <tr>
                    <td style="height: 2px; background-color: #e2e2e2;"></td>
                </tr>
                <tr>
                    <td style="padding-top: 5px; padding-bottom: 5px;">
                        <input type="text" id="txt_keyword" style="width: 560px;" maxlength="450" runat="server" />&nbsp;<input type="button" class="button_blue3" id="btn_search" value="검색" />&nbsp;<input type="button" class="button_blue3" id="btn_all" value="전체" />
                    </td>
                </tr>
            </table>
            <asp:ListView ID="lvHotlineReportList" runat="server" OnItemDataBound="lvHotlineReportList_ItemBound">
                <LayoutTemplate>
                    <table border="0" cellpadding="0" cellspacing="0" style="width: 100%" id="lvHotlineReportListTable" runat="server"
                        class="lv_list">
                        <thead>
                            <tr>
                                <th style="width: 30px;">KEY
                                </th>
                                <th>한국어
                                </th>
                                <th>영어
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr id="itemPlaceholder" runat="server" />
                        </tbody>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr runat="server">
                        <td style="width: 50px; text-align: center;">
                            <asp:Literal ID="litNo" runat="server"></asp:Literal>
                        </td>
                        <td style="padding-left: 5px; padding-right: 5px;">
                            <asp:Literal ID="litKorean" runat="server"></asp:Literal>
                        </td>
                        <td style="padding-left: 5px; padding-right: 5px;">
                            <asp:Literal ID="litEnglish" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </ItemTemplate>
                <EmptyDataTemplate>
                    <div>
                        검색된 데이터 또는 데이터가 없습니다.
                    </div>
                </EmptyDataTemplate>
            </asp:ListView>
            <!--paging-->
            <div id="divPager" class="pager" style="border-bottom: white 0px solid; text-align: center; border-left: white 0px solid; height: 20px; vertical-align: top; border-top: white 0px solid; border-right: white 0px solid;">
                <asp:Literal ID="litPager" runat="server"></asp:Literal>
            </div>
        </div>
    </form>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#btn_search').click(function () {
                document.location.href = "?kw=" + encodeURIComponent($.trim($('#txt_keyword').val()));
            });
            $('#btn_all').click(function () {
                document.location.href = "list.aspx";
            });
            $('#txt_keyword')
                .focus()
                .keydown(function (e) {
                    if (e.keyCode == 13) {
                        document.location.href = "?kw=" + encodeURIComponent($.trim($('#txt_keyword').val()));
                    }
                })
                .autocomplete({
                    minLength: 2,
                    source: function (request, response) {
                        $.ajax({
                            url: "AutoComplete_ajax.aspx",
                            dataType: "json",
                            data: {
                                q: request.term
                            },
                            success: function (data) {
                                response(data);
                            }
                        });
                    },
                    select: function (event, ui) {
                    }
                });
        })
    </script>
</body>
</html>

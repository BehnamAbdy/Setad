<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Outbox.aspx.cs" Inherits="User_Outbox" %>

<%@ OutputCache Location="None" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" dir="rtl">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="margin: 10px 125px 10px 0px;">
        <asp:ListView ID="lstInbox" runat="server" DataSourceID="ObjectDataSource1">
            <LayoutTemplate>
                <table id="list-header">
                    <tr>
                        <th style="width: 20px;">
                        </th>
                        <th style="padding: 0 5px; width: 300px;">
                            موضوع
                        </th>
                        <th style="width: 150px;">
                            گیرنده
                        </th>
                        <th style="padding: 0 2px; width: 44px;">
                            تاریخ
                        </th>
                        <th style="width: 52px;">
                        </th>
                    </tr>
                </table>
                <div id="itemPlaceHolder" runat="server">
                </div>
                <div id="pager">
                    <asp:DataPager ID="DataPager1" runat="server" PagedControlID="lstInbox" PageSize="15">
                        <Fields>
                            <asp:NextPreviousPagerField FirstPageText="<< " ShowFirstPageButton="True" ShowNextPageButton="False"
                                PreviousPageText=" < " ButtonCssClass="PageButton" />
                            <asp:NumericPagerField />
                            <asp:NextPreviousPagerField LastPageText=" >>" ShowLastPageButton="True" ShowPreviousPageButton="False"
                                NextPageText=" > " ButtonCssClass="PageButton" />
                        </Fields>
                    </asp:DataPager>
                </div>
            </LayoutTemplate>
            <ItemTemplate>
                <table class="message">
                    <tr>
                        <td class="msg-index">
                            <%#Container.DataItemIndex + 1%>
                        </td>
                        <td class="subject">
                            <%#Eval("Subject")%>
                        </td>
                        <td style="text-align: center; width: 150px;">
                        </td>
                        <td class="date">
                            <%#Public.ToPersianDateTime(Eval("SubmitDate"))%>
                        </td>
                        <td style="width: 50px; text-align: center;">
                            <img src="../App_Themes/Default/images/mail-sended.png" style="cursor: pointer; margin-top: 4px;"
                                onclick="javascript:readMessage(this)" title="پیام" alt="پیام" />
                            <input type="hidden" id="hdnBody" value='<%#Eval("Body")%>' />
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
            <EmptyDataTemplate>
                <h1>
                    پیامی فرستاده نشده</h1>
            </EmptyDataTemplate>
        </asp:ListView>
    </div>
    <br />
    <div id="dialog">
        <table class="exit-bar">
            <tr>
                <td>
                </td>
                <td align="left">
                    <img src="../App_Themes/Default/images/close.gif" style="cursor: pointer;" onclick="javascript:hideDialog()"
                        alt="بستن" title="بستن" />
                </td>
            </tr>
        </table>
        <p>
        </p>
    </div>
    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" EnablePaging="True" OnSelecting="ObjectDataSource1_Selecting"
        SelectCountMethod="GetOutboxCount" SelectMethod="LoadOutbox" TypeName="Paging"
        EnableViewState="False">
        <SelectParameters>
            <asp:QueryStringParameter Name="UserInRoleID" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <script src="../Scripts/jquery-1.4.3.min.js" type="text/javascript"></script>
    <script src="../Scripts/javascript.js" type="text/javascript"></script>
    <script type="text/javascript">

        parent.document.getElementById('myiframe').style.height = '525px';

        function readMessage(item) {
            var aryPosition = objectPosition(item);
            $('#dialog p').html($(item).next('#hdnBody').val());
            $('#dialog').css({ 'left': aryPosition[0], 'top': aryPosition[1] });
            $('#dialog').show(300);
        }

        function hideDialog() {
            $('#dialog p').html('');
            $('#dialog').hide(300);
        }

    </script>
    </form>
</body>
</html>

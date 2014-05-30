<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <table border="0" cellpadding="0" cellspacing="0" style="border-bottom: solid 1px #e7e7f5;
        margin: 0px auto 20px auto; width: 45%;">
        <tr>
            <td align="center">
                <img src="../App_Themes/Default/images/inbox.png" onclick="javascript:load(0)" style="cursor: pointer;"
                    title="پیام های دریافتی" />
            </td>
            <td align="center">
                <img src="../App_Themes/Default/images/mail-new.png" onclick="javascript:load(1)"
                    style="cursor: pointer;" title="پیام به کاربران سیستم" id="imgMessage" />
            </td>
            <td align="center">
                <img src="../App_Themes/Default/images/outbox.png" onclick="javascript:load(2)" style="cursor: pointer;"
                    title="پیام های فرستاده شده" />
            </td>
        </tr>
    </table>
    <iframe name="frmContent" src="Inbox.aspx" id="myiframe" style="text-align: center"
        width="100%" frameborder="0" scrolling="no"></iframe>
    <script src="../Scripts/jquery-1.4.3.min.js" type="text/javascript"></script>
    <script type="text/javascript">

        function load(mode) {
            switch (mode) {
                case 0:
                    window.open('Inbox.aspx', 'frmContent');
                    break;

                case 1:
                    window.open('SendMessage.aspx', 'frmContent');
                    break;

                case 2:
                    window.open('Outbox.aspx', 'frmContent');
                    break;
            }
        }

    </script>
</asp:Content>

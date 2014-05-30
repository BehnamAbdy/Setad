<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="CalanderWorkly.aspx.cs"
    Inherits="BaseInfo_CalanderWorkly" Title="تنظیم تقویم کاری" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <div class="page-title">
        تنظیم تقویم کاری
    </div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <fieldset style="margin-right: 13%; padding: 20px 100px; width: 50%;">
                <legend style="font-size: 15px">روزهای تعطیل</legend>
                <br />
                <table style="width: 350px;">
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkSaturday" runat="server" CssClass="checkbox" Text="شنبه" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkTuseDay" runat="server" CssClass="checkbox" Text="سه شنبه" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkFriday" runat="server" CssClass="checkbox" Text="جمعه" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkSunday" runat="server" CssClass="checkbox" Text="یکشنبه" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkWedneseday" runat="server" CssClass="checkbox" Text="چهارشنبه" />
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkMonday" runat="server" CssClass="checkbox" Text="دوشنبه" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkThurseday" runat="server" CssClass="checkbox" Text="پنج شنبه" />
                        </td>
                        <td>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td class="fieldName">
                            سال کاری :
                        </td>
                        <td>
                            <asp:TextBox ID="txtYear" runat="server" CssClass="textbox" Width="100px" MaxLength="4"
                                onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button ID="btnAdd" runat="server" OnClientClick="javascript:return preCheck()"
                                OnClick="btnSave_Click" Text="ذخیره" CssClass="button" />
                        </td>
                    </tr>
                </table>
            </fieldset>
            <br />
            <fieldset style="margin-right: 13%; padding: 20px 100px; width: 50%;">
                <legend style="font-size: 15px">تنظیمات سال کاری</legend>
                <br />
                <table>
                    <tr>
                        <td class="fieldName">
                            سال کاری :
                        </td>
                        <td>
                            <asp:DropDownList ID="drpYear" runat="server" CssClass="dropdown" DataTextField="SolarYear">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <input type="button" id="btnSettings" class="button" value="تنظیم" onclick="javascript:return changeSettings()" />
                        </td>
                    </tr>
                </table>
            </fieldset>
            <div class="button-box">
                <asp:Label ID="lblMessage" runat="server" CssClass="lbl-msg" EnableViewState="false" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <script src="../Scripts/javascript.js" type="text/javascript"></script>
    <script type="text/javascript">

        function changeSettings() {
            var url = 'CalanderMonthly.aspx?year=' + document.getElementById('<%=drpYear.ClientID%>').value;
            showdialog(url, null, 400, 540, false);
            return false;
        }

        function preCheck() {
            var result = true;
            if (parseInt(document.getElementById('<%=txtYear.ClientID%>').value) > 1300) {
                for (var i = 0; i < document.getElementById('<%=drpYear.ClientID%>').options.length; i++) {
                    if (document.getElementById('<%=drpYear.ClientID%>').options[i].innerHTML == document.getElementById('<%= txtYear.ClientID%>').value) {
                        result = confirm('آیا میخواهید اطلاعات سال ' + document.getElementById('<%=txtYear.ClientID%>').value + ' را ویرایش کنید؟')
                    }
                }
            }
            else {
                result = false;
                document.getElementById('<%=txtYear.ClientID%>').value = '';
                window.alert('سال وارد شده معتبر نمی باشد');
            }

            return result;
        }
        
    </script>
</asp:Content>

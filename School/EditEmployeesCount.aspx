<%@ Page Title="ویرایش آمار پرسنل" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="EditEmployeesCount.aspx.cs" Inherits="School_EditEmployeesCount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <div class="page-title">
        ویرایش آمار پرسنل
    </div>
    <asp:UpdatePanel runat="server" ID="pnl">
        <ContentTemplate>
            <table style="margin-right: 140px;">
                <tr>
                    <td class="fieldName">
                        کد آموزشگاه :
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="txtSchoolCode" runat="server" CssClass="textbox-search" AutoPostBack="True"
                            Width="80px" OnTextChanged="txtSchoolCode_TextChanged" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                        <asp:Button ID="btnSchoolCodeSearch" runat="server" CssClass="button-search" OnClientClick="javascript:return schoolsList()"
                            TabIndex="-1" />
                        <asp:TextBox ID="txtSchoolName" runat="server" CssClass="label-color" Width="281px"
                            ReadOnly="true"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtSchoolCode"
                            ErrorMessage="*"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
            <table style="margin-right: 140px;">
                <tr>
                    <td class="fieldName">
                        مقطع :
                    </td>
                    <td>
                        <asp:TextBox ID="txtLevel" runat="server" CssClass="label-color" ReadOnly="true"
                            Width="130px"></asp:TextBox>
                    </td>
                    <td class="fieldName">
                        جنسیت :
                    </td>
                    <td>
                        <asp:TextBox ID="txtGender" runat="server" CssClass="label-color" ReadOnly="true"
                            Width="130px"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <br />
            <table style="margin-right: 140px;">
                <tr>
                    <td class="fieldName">
                        آمار فعلی پرسنل ثابت
                    </td>
                    <td>
                        <asp:TextBox ID="txtCurrentFixed" runat="server" CssClass="label-color" Width="80px"
                            Enabled="false"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        آمار جدید
                    </td>
                    <td>
                        <asp:TextBox ID="txtFixed" runat="server" CssClass="textbox" MaxLength="3" Width="80px"
                            onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        توضیحات
                    </td>
                    <td>
                        <asp:TextBox ID="txtFixedComment" runat="server" CssClass="textbox" MaxLength="250"
                            Width="300px" Height="40px" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div class="button-box">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        آمار فعلی پرسنل متغیر
                    </td>
                    <td>
                        <asp:TextBox ID="txtCurrentChangable" runat="server" CssClass="label-color" Width="80px"
                            Enabled="false"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        آمار جدید
                    </td>
                    <td>
                        <asp:TextBox ID="txtChangable" runat="server" CssClass="textbox" MaxLength="3" Width="80px"
                            onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        توضیحات
                    </td>
                    <td>
                        <asp:TextBox ID="txtChangableComment" runat="server" CssClass="textbox" MaxLength="250"
                            Width="300px" Height="40px" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <div class="button-box">
                <asp:Button UseSubmitBehavior="false" ID="btnSave" runat="server" CssClass="button"
                    Text="ذخیره" OnClick="btnSave_Click" />
                <asp:Label ID="lblMessage" runat="server" CssClass="lbl-msg" EnableViewState="false" />
            </div>
            <asp:HiddenField ID="hdnSchoolId" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="pnl">
        <ProgressTemplate>
            <center id="callback-panel">
                <div>
                    درحال بارگذاری...
                </div>
            </center>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <script src="../Scripts/javascript.js" type="text/javascript"></script>
    <script type="text/javascript">

        function schoolsList() {
            var school = showSchoolsList();
            if (school != null) {
                $get('<%=hdnSchoolId.ClientID %>', document).value = school[0].Data[1];
                $get('<%=txtSchoolCode.ClientID %>', document).value = school[0].Data[2];
                return true;
            }
            return false;
        }

    </script>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="NewUser.aspx.cs" Inherits="User_NewUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <div class="page-title">
        برای ساخت کاربر جدید فرم زیر را تکمیل نمایید
    </div>
    <table style="margin-right: 170px;">
        <tr>
            <td class="fieldName">
                سمت :
            </td>
            <td>
                <asp:DropDownList ID="drpRoles" runat="server" CssClass="dropdown-large">
                    <asp:ListItem Value="0" Text="- انتخاب کنید -"></asp:ListItem>
                    <%--<asp:ListItem Value="3" Text="مدیر آموزشگاه"></asp:ListItem>--%>
                    <asp:ListItem Value="2" Text="مدیر منطقه"></asp:ListItem>
                    <asp:ListItem Value="1" Text="مدیر سیستم"></asp:ListItem>
                </asp:DropDownList>
                <asp:CompareValidator ID="CompareValidator2" runat="server" ErrorMessage="*" ControlToValidate="drpRoles"
                    ValueToCompare="0" Operator="NotEqual" Type="String"></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <td class="fieldName">
                نام کاربری :
            </td>
            <td>
                <asp:TextBox ID="txtUserName" runat="server" CssClass="textbox-large"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtUserName"
                    ErrorMessage="*"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="fieldName">
                گذرواژه :
            </td>
            <td>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="textbox-large" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPassword"
                    ErrorMessage="*"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="fieldName">
                تکرار گذرواژه :
            </td>
            <td>
                <asp:TextBox ID="txtRePassword" runat="server" CssClass="textbox-large" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtRePassword"
                    ErrorMessage="*"></asp:RequiredFieldValidator>
                <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="*" ControlToValidate="txtRePassword"
                    ControlToCompare="txtPassword"></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <td class="fieldName">
                نام :
            </td>
            <td>
                <asp:TextBox ID="txtName" runat="server" CssClass="textbox-large"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtName"
                    ErrorMessage="*"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="fieldName">
                نام خانوادگی :
            </td>
            <td>
                <asp:TextBox ID="txtFamily" runat="server" CssClass="textbox-large"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtFamily"
                    ErrorMessage="*"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="fieldName">
                جنسیت :
            </td>
            <td>
                <asp:DropDownList ID="drpGender" runat="server" CssClass="dropdown-large">
                    <asp:ListItem Text="زن"></asp:ListItem>
                    <asp:ListItem Text="مرد"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="fieldName">
                تلفن :
            </td>
            <td>
                <asp:TextBox ID="txtPhone" runat="server" CssClass="textbox-large" MaxLength="15"
                    onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
            </td>
        </tr>
    </table>
    <div class="button-box">
        <asp:Button UseSubmitBehavior="false" ID="btnSave" runat="server" CssClass="button"
            Text="ذخیره" OnClick="btnSave_Click" />
        <asp:Label ID="lblMessage" runat="server" CssClass="lbl-msg" EnableViewState="false" />
    </div>
    <script src="../Scripts/javascript.js" type="text/javascript"></script>
</asp:Content>

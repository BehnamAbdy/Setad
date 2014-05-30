<%@ Page Title="ویرایش پروفایل" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Profile.aspx.cs" Inherits="User_Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <div class="page-title">
        پروفایل کاربر
    </div>
    <table style="margin: 0px auto;">
        <tr>
            <td class="fieldName">
                نام :
            </td>
            <td>
                <asp:TextBox ID="txtName" runat="server" CssClass="textbox-large"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtName"
                    ErrorMessage="*"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="fieldName">
                نام خانوادگی :
            </td>
            <td>
                <asp:TextBox ID="txtFamily" runat="server" CssClass="textbox-large"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtFamily"
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
                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender6" runat="server"
                    FilterType="Numbers" TargetControlID="txtPhone" />
                <asp:TextBox ID="txtPhone" runat="server" CssClass="textbox-large" MaxLength="15"></asp:TextBox>
            </td>
        </tr>
    </table>
    <div class="button-box">
        <asp:Button UseSubmitBehavior="false" ID="btnSave" runat="server" CssClass="button"
            Text="ذخیره" OnClick="btnSave_Click" />
    </div>
</asp:Content>

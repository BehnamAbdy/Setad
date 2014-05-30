<%@ Page Title="ویرایش گذرواژه" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="ChangePassword.aspx.cs" Inherits="User_ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <div class="page-title">
        ویرایش گذرواژه
    </div>
    <center>
        <p id="warning">
            گذرواژه جدید نباید کمتر از 5 حرف باشد
        </p>
    </center>
    <asp:UpdatePanel ID="pnl" runat="server">
        <ContentTemplate>
            <table style="margin-right: 200px;">
                <tr>
                    <td class="fieldName">
                        نام کاربری :
                    </td>
                    <td style="width: 400px;">
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
                        *
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        گذرواژه جدید :
                    </td>
                    <td>
                        <asp:TextBox ID="txtNewPassword" runat="server" CssClass="textbox-large" TextMode="Password"></asp:TextBox>*
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtNewPassword"
                            ErrorMessage="*"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="valPasswordLength" runat="server" ControlToValidate="txtNewPassword"
                            SetFocusOnError="true" Display="Dynamic" ValidationExpression="\w{5,}" ErrorMessage="طول گذرواژه نباید کمتر از 5 حرف باشد."
                            ToolTip="طول گذرواژه نباید کمتر از 5 حرف باشد." Font-Size="11px"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        تکرار گذرواژه :
                    </td>
                    <td>
                        <asp:TextBox ID="txtRePassword" runat="server" CssClass="textbox-large" TextMode="Password"></asp:TextBox>*
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtRePassword"
                            ErrorMessage="*"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="*" ControlToValidate="txtRePassword"
                            ControlToCompare="txtNewPassword"></asp:CompareValidator>
                    </td>
                </tr>
            </table>
            <div class="button-box">
                <asp:Button ID="btnSave" runat="server" CssClass="button" Text="تایید" OnClick="btnSave_Click" />
                <asp:Label ID="lblMessage" runat="server" CssClass="lbl-msg" EnableViewState="false"></asp:Label>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

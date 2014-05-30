<%@ Page Title="ورود به سامانه ستاد تغذیه استان فارس" Language="C#" MasterPageFile="~/Site.master"
    AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <table width="100%">
        <tr>
            <td>
                <div class="header">
                </div>
                <div id="login-box">
                    <div id="login-header">
                        <span>ورود به سامانه ستاد تغذیه استان فارس </span>
                    </div>
                    <asp:Panel runat="server" DefaultButton="btnLogin">
                        <table style="margin: 40px 50px; width: 450px">
                            <tr>
                                <td style="font-size: 14px; text-align: left;">
                                    نام کاربری :
                                </td>
                                <td style="width: 15px;">
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUserName" runat="server" CssClass="textbox-login" />
                                    <asp:RequiredFieldValidator ID="valRequireUserName" runat="server" ControlToValidate="txtUserName"
                                        SetFocusOnError="True" Text="*" ValidationGroup="Login" />
                                </td>
                            </tr>
                            <tr>
                                <td height="10px">
                                </td>
                                <td height="10px">
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td style="font-size: 14px; text-align: left;">
                                    گذرواژه :
                                </td>
                                <td style="width: 25px;">
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="textbox-login" />
                                    <asp:RequiredFieldValidator ID="valRequirePassword" runat="server" ControlToValidate="txtPassword"
                                        SetFocusOnError="True" Text="*" ValidationGroup="Login" />
                                </td>
                            </tr>
                            <tr>
                                <td height="10px">
                                </td>
                                <td height="10px">
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td style="width: 25px;">
                                </td>
                                <td>
                                    <asp:Button ID="btnLogin" runat="server" Text="ورود" CssClass="button-login" ValidationGroup="Login"
                                        OnClick="btnLogin_Click" />
                                    <asp:Label ID="lblMessage" runat="server" ForeColor="#fd6f42" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </div>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
        <tr>
            <td>
                <div class="clear">
                </div>
                <div class="footer">
                </div>
            </td>
        </tr>
    </table>
</asp:Content>

<%@ Page Title="ویرایش آمار دانش آموزان" Language="C#" MasterPageFile="~/Site.master"
    AutoEventWireup="true" CodeFile="EditStudentsCount.aspx.cs" Inherits="School_EditStudentsCount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <div class="page-title">
        ویرایش آمار دانش آموزان
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
            <asp:GridView ID="grdSubLevels" runat="server" AutoGenerateColumns="False" GridLines="None"
                HorizontalAlign="Center" CellPadding="4" ForeColor="#333333" Font-Size="16px"
                Width="600px" DataKeyNames="SubLevelID">
                <RowStyle Wrap="True" BackColor="#F7F6F3" ForeColor="#333333" />
                <Columns>
                    <asp:TemplateField HeaderText="پایه مقطع آموزشی">
                        <ItemTemplate>
                            <span style="font-size: 14px;">
                                <%#Eval("SubLevelName")%>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="تعداد پسر">
                        <ItemTemplate>
                            <asp:TextBox ID="txtBoysCount" runat="server" CssClass="textbox" Enabled='<%# ActivateByGender((byte)Eval("Gender"), "M") %>'
                                Text='<%# Eval("BoysCount") %>' Width="60px" MaxLength="4" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvBoys" runat="server" Enabled='<%# ActivateByGender((byte)Eval("Gender"), "M") %>'
                                ErrorMessage="*" ControlToValidate="txtBoysCount"></asp:RequiredFieldValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="تعداد دختر">
                        <ItemTemplate>
                            <asp:TextBox ID="txtGirlsCount" runat="server" CssClass="textbox" Enabled='<%# ActivateByGender((byte)Eval("Gender"), "F") %>'
                                Text='<%# Eval("GirlsCount") %>' Width="60px" MaxLength="4" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvGirls" runat="server" Enabled='<%# ActivateByGender((byte)Eval("Gender"), "F") %>'
                                ErrorMessage="*" ControlToValidate="txtGirlsCount"></asp:RequiredFieldValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <EmptyDataTemplate>
                    <div style="text-align: center; width: 700px; direction: ltr; color: #bb4462; background-color: #ffffc7;">
                        .آیتمی وجود ندارد</div>
                </EmptyDataTemplate>
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <EditRowStyle BackColor="#999999" />
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            </asp:GridView>
            <br />
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

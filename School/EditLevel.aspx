<%@ Page Title="ویرایش مقطع آموزشگاه" Language="C#" MasterPageFile="~/Site.master"
    AutoEventWireup="true" CodeFile="EditLevel.aspx.cs" Inherits="School_EditLevel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <div class="page-title">
        ویرایش مقطع آموزشگاه
    </div>
    <asp:UpdatePanel ID="pnl" runat="server">
        <ContentTemplate>
            <table style="margin-right: 120px;">
                <tr>
                    <td class="fieldName">
                        منطقه آموزشی :
                    </td>
                    <td>
                        <asp:TextBox ID="txtAreaCode" runat="server" CssClass="textbox-search" MaxLength="6"
                            onchange="javascript:getArea()" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                        <input type="button" id="btnAreaSearch" class="button-search" tabindex="-1" onclick="javascript:return areaList()" />
                        <asp:TextBox ID="txtAreaName" runat="server" CssClass="label-color" Width="250px"
                            Enabled="false" />
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        کد آموزشگاه :
                    </td>
                    <td>
                        <asp:TextBox ID="txtSchoolCode" runat="server" CssClass="textbox-search" AutoPostBack="true"
                            OnTextChanged="txtSchoolCode_TextChanged" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                        <asp:Button ID="btnSchoolCodeSearch" runat="server" CssClass="button-search" OnClientClick="javascript:return schoolsList()"
                            TabIndex="-1" />
                        <asp:TextBox ID="txtSchoolName" runat="server" CssClass="label-color" Width="250px"
                            Enabled="false"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        جنسیت :
                    </td>
                    <td>
                        <asp:DropDownList ID="drpGender" runat="server" CssClass="dropdown-large" Enabled="false">
                            <asp:ListItem Text="پسرانه"></asp:ListItem>
                            <asp:ListItem Text="دخترانه"></asp:ListItem>
                            <asp:ListItem Text="مختلط" Selected="true"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        مقطع آموزشگاه :
                    </td>
                    <td>
                        <asp:DropDownList ID="drpSchoolLevel" runat="server" CssClass="dropdown-large" Enabled="false"
                            DataTextField="LevelName" DataValueField="LevelID" AutoPostBack="True" OnSelectedIndexChanged="drpSchoolLevel_SelectedIndexChanged">
                        </asp:DropDownList>
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
                            <asp:TextBox ID="txtBoysCount" runat="server" CssClass="textbox" Enabled='<%# ActivateByGender("M") %>'
                                Text='<%# Eval("BoysCount") %>' Width="60px" MaxLength="4" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvBoys" runat="server" Enabled='<%# ActivateByGender("M") %>'
                                ErrorMessage="*" ControlToValidate="txtBoysCount"></asp:RequiredFieldValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="تعداد دختر">
                        <ItemTemplate>
                            <asp:TextBox ID="txtGirlsCount" runat="server" CssClass="textbox" Enabled='<%# ActivateByGender("F") %>'
                                Text='<%# Eval("GirlsCount") %>' Width="60px" MaxLength="4" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvGirls" runat="server" Enabled='<%# ActivateByGender("F") %>'
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
            <div class="button-box" style="width: 630px;">
                <asp:Button UseSubmitBehavior="false" ID="btnSaveLevel" runat="server" CssClass="button"
                    Enabled="false" Text="ذخیره مقطع جدید" OnClick="btnSaveLevel_Click" />
                <asp:Label ID="lblMessage" runat="server" CssClass="lbl-msg" EnableViewState="false" />
            </div>
            <asp:HiddenField ID="hdnSchoolId" runat="server" />
            <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="pnl">
                <ProgressTemplate>
                    <center id="callback-panel">
                        <div>
                            درحال بارگذاری...
                        </div>
                    </center>
                </ProgressTemplate>
            </asp:UpdateProgress>
        </ContentTemplate>
    </asp:UpdatePanel>
    <script src="../Scripts/jquery-1.4.3.min.js" type="text/javascript"></script>
    <script src="../Scripts/javascript.js" type="text/javascript"></script>
    <script type="text/javascript">

        function areaList() {
            var area = showAreasList();
            if (area != null) {
                $get('<%=txtAreaCode.ClientID %>', document).value = area[0].Data[0];
                $get('<%=txtAreaName.ClientID %>', document).value = area[0].Data[1];
            }
            return false;
        }

        function getArea() {
            var code = $get('<%=txtAreaCode.ClientID %>', document).value;
            if (code == '') {
                $get('<%=txtAreaName.ClientID %>', document).value = '';
                $get('<%=txtSchoolCode.ClientID %>', document).value = '';
                $get('<%=txtSchoolName.ClientID %>', document).value = '';
                return;
            }

            $.ajax({
                type: 'POST',
                data: '{ code:' + code + '}',
                url: '../Utility.asmx/GetArea',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (response) {
                    if (response.d != null) {
                        $get('<%=txtAreaName.ClientID %>', document).value = response.d;
                    }
                    else {
                        $get('<%=txtAreaCode.ClientID %>', document).value = '';
                        $get('<%=txtAreaName.ClientID %>', document).value = '';
                        $get('<%=txtAreaCode.ClientID %>', document).focus();
                    }
                }
            });
        }

        function schoolsList() {
            if ($get('<%=txtAreaCode.ClientID %>', document).value != '') {
                var school = showAreaSchoolsList($get('<%=txtAreaCode.ClientID %>', document).value);
                if (school != null) {
                    $get('<%=txtSchoolCode.ClientID %>', document).value = school[0].Data[2];
                    $get('<%=txtSchoolName.ClientID %>', document).value = school[0].Data[3];
                    return true;
                }
            }
            alert('منطقه مورد نظرتان را انتخاب کنبد');
            return false;
        }

    </script>
</asp:Content>

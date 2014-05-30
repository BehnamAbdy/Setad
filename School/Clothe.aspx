<%@ Page Title="فرم پوشاک آموزشگاه" Language="C#" MasterPageFile="~/Site.master"
    AutoEventWireup="true" CodeFile="Clothe.aspx.cs" Inherits="School_Clothe" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <div class="page-title">
        فرم پوشاک آموزشگاه
    </div>
    <asp:UpdatePanel ID="pnl" runat="server">
        <ContentTemplate>
            <center>
                <table>
                    <tr>
                        <td class="fieldName">
                            کد آموزشگاه :
                        </td>
                        <td colspan="3" align="right">
                            <asp:TextBox ID="txtSchoolCode" runat="server" CssClass="textbox-search" AutoPostBack="True"
                                OnTextChanged="txtSchoolCode_TextChanged" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                            <asp:Button ID="btnSchoolCodeSearch" runat="server" CssClass="button-search" OnClientClick="javascript:return schoolsList()"
                                TabIndex="-1" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtSchoolCode"
                                ErrorMessage="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            نام آموزشگاه :
                        </td>
                        <td>
                            <asp:TextBox ID="txtSchoolName" runat="server" CssClass="label-color" Width="220px"
                                ReadOnly="true"></asp:TextBox>
                        </td>
                        <td class="fieldName">
                            نام منطقه / شهرستان :
                        </td>
                        <td>
                            <asp:TextBox ID="txtCity" runat="server" CssClass="label-color" Width="220px" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            پایه :
                        </td>
                        <td align="right">
                            <asp:DropDownList ID="drpSublevel" runat="server" CssClass="dropdown-large" AutoPostBack="True"
                                OnSelectedIndexChanged="drpMonths_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td class="fieldName">
                            سال تحصیلی :
                        </td>
                        <td class="fieldName" style="text-align: center;">
                            <asp:Label ID="lblCycle" runat="server" Style="direction: ltr; color: #3b76ee; font: bold 16px Arial;"></asp:Label>
                        </td>
                    </tr>
                </table>
            </center>
            <br />
            <div style="height: 25px; padding-right: 188px;">
                <asp:DropDownList ID="drpStuff_1" runat="server" CssClass="dropdown" DataValueField="CycleClotheID"
                    DataTextField="StuffName" Style="float: right; width: 125px;" onchange="javascript:checkSelectedStuff(1)">
                </asp:DropDownList>
                <asp:DropDownList ID="drpStuff_2" runat="server" CssClass="dropdown" DataValueField="CycleClotheID"
                    DataTextField="StuffName" Style="float: right; width: 125px;" onchange="javascript:checkSelectedStuff(2)">
                </asp:DropDownList>
                <asp:DropDownList ID="drpStuff_3" runat="server" CssClass="dropdown" DataValueField="CycleClotheID"
                    DataTextField="StuffName" Style="float: right; width: 125px;" onchange="javascript:checkSelectedStuff(3)">
                </asp:DropDownList>
                <asp:DropDownList ID="drpStuff_4" runat="server" CssClass="dropdown" DataValueField="CycleClotheID"
                    DataTextField="StuffName" Style="float: right; width: 125px;" onchange="javascript:checkSelectedStuff(4)">
                </asp:DropDownList>
                <asp:DropDownList ID="drpStuff_5" runat="server" CssClass="dropdown" DataValueField="CycleClotheID"
                    DataTextField="StuffName" Style="float: right; width: 125px;" onchange="javascript:checkSelectedStuff(5)">
                </asp:DropDownList>
            </div>
            <asp:GridView ID="grdStudentClothes" runat="server" AutoGenerateColumns="False" GridLines="Vertical"
                HorizontalAlign="Center" Font-Names="Tahoma" CellPadding="4" ForeColor="#333333"
                Width="800px">
                <RowStyle Wrap="True" BackColor="#F7F6F3" ForeColor="#333333" />
                <Columns>
                    <asp:TemplateField HeaderText="ردیف" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <span style="font-size: 14px;">
                                <%#Eval("RowNumber")%>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="نام و نام خانوادگی">
                        <ItemTemplate>
                            <asp:TextBox ID="txtStudentName" runat="server" CssClass="textbox"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkGood_1" runat="server" CssClass="checkbox" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkGood_2" runat="server" CssClass="checkbox" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkGood_3" runat="server" CssClass="checkbox" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkGood_4" runat="server" CssClass="checkbox" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkGood_5" runat="server" CssClass="checkbox" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <EditRowStyle BackColor="#999999" />
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            </asp:GridView>
            <div class="button-box">
                <asp:Button UseSubmitBehavior="false" ID="btnSave" runat="server" CssClass="button"
                    Text="ذخیره" OnClick="btnSave_Click" />
                <asp:Label ID="lblMessage" runat="server" CssClass="lbl-msg" EnableViewState="false" />
            </div>
            <asp:HiddenField ID="hdnSchId" runat="server" />
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
                $get('<%=hdnSchId.ClientID %>', document).value = school[0].Data[1];
                $get('<%=txtSchoolCode.ClientID %>', document).value = school[0].Data[2];
                return true;
            }
            return false;
        }

        function checkSelectedStuff(drpIndex) {
            var drp;
            switch (drpIndex) {
                case 1:
                    drp = $get('<%=drpStuff_1.ClientID %>', document);
                    if (drp.selectedIndex > 0) {
                        if (drp.selectedIndex == $get('<%=drpStuff_2.ClientID %>', document).selectedIndex ||
                            drp.selectedIndex == $get('<%=drpStuff_3.ClientID %>', document).selectedIndex ||
                            drp.selectedIndex == $get('<%=drpStuff_4.ClientID %>', document).selectedIndex ||
                            drp.selectedIndex == $get('<%=drpStuff_5.ClientID %>', document).selectedIndex) {
                            drp.selectedIndex = 0;
                        }
                    }
                    break;

                case 2:
                    drp = $get('<%=drpStuff_2.ClientID %>', document);
                    if (drp.selectedIndex > 0) {
                        if (drp.selectedIndex == $get('<%=drpStuff_1.ClientID %>', document).selectedIndex ||
                            drp.selectedIndex == $get('<%=drpStuff_3.ClientID %>', document).selectedIndex ||
                            drp.selectedIndex == $get('<%=drpStuff_4.ClientID %>', document).selectedIndex ||
                            drp.selectedIndex == $get('<%=drpStuff_5.ClientID %>', document).selectedIndex) {
                            drp.selectedIndex = 0;
                        }
                    }
                    break;

                case 3:
                    drp = $get('<%=drpStuff_3.ClientID %>', document);
                    if (drp.selectedIndex > 0) {
                        if (drp.selectedIndex == $get('<%=drpStuff_1.ClientID %>', document).selectedIndex ||
                            drp.selectedIndex == $get('<%=drpStuff_2.ClientID %>', document).selectedIndex ||
                            drp.selectedIndex == $get('<%=drpStuff_4.ClientID %>', document).selectedIndex ||
                            drp.selectedIndex == $get('<%=drpStuff_5.ClientID %>', document).selectedIndex) {
                            drp.selectedIndex = 0;
                        }
                    }
                    break;

                case 4:
                    drp = $get('<%=drpStuff_4.ClientID %>', document);
                    if (drp.selectedIndex > 0) {
                        if (drp.selectedIndex == $get('<%=drpStuff_1.ClientID %>', document).selectedIndex ||
                            drp.selectedIndex == $get('<%=drpStuff_2.ClientID %>', document).selectedIndex ||
                            drp.selectedIndex == $get('<%=drpStuff_3.ClientID %>', document).selectedIndex ||
                            drp.selectedIndex == $get('<%=drpStuff_5.ClientID %>', document).selectedIndex) {
                            drp.selectedIndex = 0;
                        }
                    }
                    break;

                case 5:
                    drp = $get('<%=drpStuff_5.ClientID %>', document);
                    if (drp.selectedIndex > 0) {
                        if (drp.selectedIndex == $get('<%=drpStuff_1.ClientID %>', document).selectedIndex ||
                            drp.selectedIndex == $get('<%=drpStuff_2.ClientID %>', document).selectedIndex ||
                            drp.selectedIndex == $get('<%=drpStuff_3.ClientID %>', document).selectedIndex ||
                            drp.selectedIndex == $get('<%=drpStuff_4.ClientID %>', document).selectedIndex) {
                            drp.selectedIndex = 0;
                        }
                    }
                    break;
            }
        }
    </script>
</asp:Content>

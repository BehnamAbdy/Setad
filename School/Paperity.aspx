<%@ Page Title="توزیع نوشت افزار" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Paperity.aspx.cs" Inherits="School_Paperity" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <div class="page-title">
        خلاصه وضعیت توزیع نوشت افزار
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
                            ماه توزیع :
                        </td>
                        <td align="right">
                            <asp:DropDownList ID="drpMonths" runat="server" CssClass="dropdown-large" AutoPostBack="True"
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
                <br />
                <asp:GridView ID="grdPaperities" runat="server" AutoGenerateColumns="False" ShowFooter="true"
                    CellPadding="4" ForeColor="#333333" GridLines="None" OnRowDataBound="grdPaperities_RowDataBound">
                    <RowStyle BackColor="#EFF3FB" />
                    <Columns>
                    </Columns>
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <EditRowStyle BackColor="#2461BF" />
                    <AlternatingRowStyle BackColor="White" />
                </asp:GridView>
            </center>
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

        function getGridSum() {
            var table = document.getElementById('<%= grdPaperities.ClientID %>');
            var verticalArray = new Array(); // Holds sum of each column          
            var total = 0;

            for (var i = 1; i < table.rows.length - 1; i++) {
                var horizontalSum = 0;
                for (var j = 1; j < table.rows[0].cells.length - 1; j++) {
                    if (table.rows[i].cells[j].childNodes[0].value != '') {
                        var cellValue = parseInt(isNaN(table.rows[i].cells[j].childNodes[0].value) ? 0 : table.rows[i].cells[j].childNodes[0].value);
                        verticalArray[j] = (isNaN(verticalArray[j]) ? 0 : verticalArray[j]) + cellValue;
                        horizontalSum += cellValue;
                    }
                }
                total += isNaN(horizontalSum) ? 0 : horizontalSum;
                table.rows[i].cells[table.rows[0].cells.length - 1].childNodes[0].value = horizontalSum;
            }

            for (var l = 1; l < table.rows[0].cells.length - 1; l++) {
                table.rows[table.rows.length - 1].cells[l].childNodes[0].value = isNaN(verticalArray[l]) ? 0 : verticalArray[l];
            }

            table.rows[table.rows.length - 1].cells[table.rows[0].cells.length - 1].childNodes[0].value = total;
        }

        function checkGoods(drp, rowIndex) {
            var table = document.getElementById('<%= grdPaperities.ClientID %>');
            if (drp.selectedIndex == 0) {
                for (var j = 1; j < table.rows[rowIndex].cells.length; j++) {
                    table.rows[rowIndex].cells[j].childNodes[0].value = '';
                }
                getGridSum();
                return;
            }
            else {
                for (var i = 1; i < drp.options.length - 1; i++) {
                    if (i == rowIndex) {
                        continue;
                    }
                    if (table.rows[i].cells[0].childNodes[0].selectedIndex == drp.selectedIndex) // This good is already selected
                    {
                        drp.selectedIndex = 0;
                        for (var j = 1; j < table.rows[rowIndex].cells.length; j++) {
                            table.rows[rowIndex].cells[j].childNodes[0].value = '';
                        }
                        getGridSum();
                        break;
                    }
                }
            }
        }
    
    </script>
</asp:Content>

<%@ Page Title="گزارش پوشاک پایه ها " Language="C#" MasterPageFile="~/Site.master"
    AutoEventWireup="true" CodeFile="ClotheReport.aspx.cs" Inherits="Report_ClotheReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <div class="page-title">
        گزارش پوشاک پایه ها
    </div>
    <asp:UpdatePanel ID="pnl" runat="server">
        <ContentTemplate>
            <table style="margin-right: 130px;">
                <tr>
                    <td class="fieldName">
                        دوره :
                    </td>
                    <td>
                        <asp:DropDownList ID="drpCycle" runat="server" DataValueField="CycleID" DataTextField="CycleName"
                            CssClass="dropdown" AutoPostBack="True" Width="200px" OnSelectedIndexChanged="drpCycle_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="*" ControlToValidate="drpCycle"
                            ValueToCompare="0" Operator="GreaterThan" Type="Integer" ValidationGroup="s"></asp:CompareValidator>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        کالا :
                    </td>
                    <td>
                        <asp:DropDownList ID="drpStuffs" runat="server" CssClass="dropdown" DataTextField="StuffName"
                            DataValueField="CycleClotheID" Width="200px">
                        </asp:DropDownList>
                        <asp:CompareValidator ID="CompareValidator2" runat="server" ErrorMessage="*" ControlToValidate="drpStuffs"
                            ValidationGroup="s" ValueToCompare="0" Operator="GreaterThan" Type="Integer"></asp:CompareValidator>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        منطقه آموزشی :
                    </td>
                    <td>
                        <asp:TextBox ID="txtAreaCode" runat="server" CssClass="textbox-search" MaxLength="6"
                            Width="70px" onchange="javascript:getArea()" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                        <asp:Button ID="btnAreaSearch" runat="server" CssClass="button-search" TabIndex="-1"
                            OnClientClick="javascript:return areaList()" />
                        <asp:TextBox ID="txtAreaName" runat="server" CssClass="label-color" Width="300px"
                            Enabled="false" />
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        کد آموزشگاه :
                    </td>
                    <td>
                        <asp:TextBox ID="txtSchoolCode" runat="server" CssClass="textbox-search" Width="70px"
                            AutoPostBack="True" OnTextChanged="txtSchoolCode_TextChanged" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                        <asp:Button ID="btnSchoolCodeSearch" runat="server" CssClass="button-search" OnClientClick="javascript:return schoolsList()"
                            TabIndex="-1" />
                        <asp:TextBox ID="txtSchoolName" runat="server" CssClass="label-color" Width="300px"
                            Enabled="false"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        پایه :
                    </td>
                    <td>
                        <asp:DropDownList ID="drpSubLevel" runat="server" CssClass="dropdown" DataValueField="SchoolSubLevelID"
                            DataTextField="SubLevelName" Width="200px">
                        </asp:DropDownList>
                        <asp:Button ID="btnSearch" runat="server" CssClass="button" Text="جستجو" OnClick="btnSearch_Click"
                            ValidationGroup="s" />
                    </td>
                </tr>
                <tr>
                    <td class="fieldName" align="left">
                        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="pnl">
                            <ProgressTemplate>
                                <img src="../App_Themes/Default/images/ajax-loader.gif" />
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </td>
                    <td style="color: #bf9607; font: bold 20px Arial;">
                        <asp:LinkButton ID="lnkSum" Text="تعداد کل :" runat="server" OnClick="lnkSum_Click" />
                        <asp:Label ID="lblTotal" runat="server" Font-Size="18" ForeColor="#d55f0e"></asp:Label>
                    </td>
                </tr>
            </table>
            <div style="margin: 10px 10px 5px 0px;">
                <asp:ListView ID="lstClothe" runat="server" ItemPlaceholderID="itemPlaceHolder">
                    <LayoutTemplate>
                        <table id="list-header">
                            <tr>
                                <th style="width: 30px;">
                                </th>
                                <th style="width: 80px;">
                                    کد آموزشگاه
                                </th>
                                <th style="width: 200px;">
                                    نام آموزشگاه
                                </th>
                                <th>
                                    پایه
                                </th>
                                <th>
                                    منطقه
                                </th>
                                <th>
                                    نام کالا
                                </th>
                                <th>
                                    تعداد
                                </th>
                            </tr>
                        </table>
                        <div id="itemPlaceHolder" runat="server">
                        </div>
                        <div id="pager">
                            <asp:DataPager ID="DataPager1" runat="server" PagedControlID="lstClothe" PageSize="15">
                                <Fields>
                                    <asp:NextPreviousPagerField FirstPageText="<< " ShowFirstPageButton="True" ShowNextPageButton="False"
                                        PreviousPageText=" < " ButtonCssClass="PageButton" />
                                    <asp:NumericPagerField />
                                    <asp:NextPreviousPagerField LastPageText=" >>" ShowLastPageButton="True" ShowPreviousPageButton="False"
                                        NextPageText=" > " ButtonCssClass="PageButton" />
                                </Fields>
                            </asp:DataPager>
                        </div>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <table class="list-item-even">
                            <tr>
                                <td style="width: 30px;">
                                    <%#Container.DataItemIndex + 1%>
                                </td>
                                <td style="width: 80px;">
                                    <%#Eval("SchoolCode")%>
                                </td>
                                <td style="width: 200px;">
                                    <%#Eval("SchoolName")%>
                                </td>
                                <td>
                                    <%#Eval("SubLevelName")%>
                                </td>
                                <td>
                                    <%#Eval("AreaName")%>
                                </td>
                                <td>
                                    <%#Eval("StuffName")%>
                                </td>
                                <td>
                                    <%#Eval("ClotheCount")%>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <table class="list-item-odd">
                            <tr>
                                <td style="width: 30px;">
                                    <%#Container.DataItemIndex + 1%>
                                </td>
                                <td style="width: 80px;">
                                    <%#Eval("SchoolCode")%>
                                </td>
                                <td style="width: 200px;">
                                    <%#Eval("SchoolName")%>
                                </td>
                                <td>
                                    <%#Eval("SubLevelName")%>
                                </td>
                                <td>
                                    <%#Eval("AreaName")%>
                                </td>
                                <td>
                                    <%#Eval("StuffName")%>
                                </td>
                                <td>
                                    <%#Eval("ClotheCount")%>
                                </td>
                            </tr>
                        </table>
                    </AlternatingItemTemplate>
                    <EmptyDataTemplate>
                        <h1>
                            آیتمی یافت نشد</h1>
                    </EmptyDataTemplate>
                </asp:ListView>
            </div>
            <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" EnablePaging="True" OnSelecting="ObjectDataSource1_Selecting"
                SelectCountMethod="GetSubLevelClothesCount" SelectMethod="LoadSubLevelClothes"
                TypeName="Paging" EnableViewState="False">
                <SelectParameters>
                    <asp:QueryStringParameter Name="CycleClotheId" Type="Int32" />
                    <asp:QueryStringParameter Name="AreaCode" Type="Int32" />
                    <asp:QueryStringParameter Name="SchoolCode" Type="Int32" />
                    <asp:QueryStringParameter Name="SubLevelId" Type="Int32" />
                </SelectParameters>
            </asp:ObjectDataSource>
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
                for (var i = $get('<%=drpSubLevel.ClientID %>', document).options.length - 1; i >= 0; i--) {
                    $get('<%=drpSubLevel.ClientID %>', document).options[i] = null;
                }
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
            return false;
        }

    </script>
</asp:Content>

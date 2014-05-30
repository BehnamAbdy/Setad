<%@ Page Title="گزارش ویرایش آمار پرسنل" Language="C#" MasterPageFile="~/Site.master"
    AutoEventWireup="true" CodeFile="EpmloyeesArchive.aspx.cs" Inherits="Report_EpmloyeesArchive" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <div class="page-title">
        گزارش ویرایش آمار پرسنل
    </div>
    <asp:UpdatePanel ID="pnl" runat="server">
        <ContentTemplate>
            <table style="margin-right: 130px;">
                <tr>
                    <td class="fieldName">
                        از تاریخ :
                    </td>
                    <td>
                        <userControl:Date ID="txtDateFrom" runat="server" Required="false" />
                    </td>
                    <td class="fieldName">
                        تا تاریخ :
                    </td>
                    <td>
                        <userControl:Date ID="txtDateTo" runat="server" Required="false" />
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        منطقه آموزشی :
                    </td>
                    <td colspan="3">
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
                    <td colspan="3">
                        <asp:TextBox ID="txtSchoolCode" runat="server" CssClass="textbox-search" onchange="javascript:getSchool()"
                            Width="70px" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                        <asp:Button ID="btnSchoolCodeSearch" runat="server" CssClass="button-search" OnClientClick="javascript:return schoolsList()"
                            TabIndex="-1" />
                        <asp:TextBox ID="txtSchoolName" runat="server" CssClass="label-color" Width="300px"
                            Enabled="false"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        پرسنل :
                    </td>
                    <td colspan="3">
                        <asp:DropDownList ID="drpEmployeeType" runat="server" CssClass="dropdown" Width="200px">
                            <asp:ListItem Value="B" Text="- همه موارد -"></asp:ListItem>
                            <asp:ListItem Value="F" Text="پرسنل ثابت"></asp:ListItem>
                            <asp:ListItem Value="C" Text="پرسنل متغیر"></asp:ListItem>
                        </asp:DropDownList>
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
                    <td colspan="3" style="color: #bf9607; font: bold 20px Arial;">
                        <asp:Button ID="btnSearch" runat="server" CssClass="button" Text="جستجو" OnClick="btnSearch_Click" />
                    </td>
                </tr>
            </table>
            <br />
            <div style="margin: 10px 95px 5px 0px;">
                <asp:ListView ID="lstArchive" runat="server" ItemPlaceholderID="itemPlaceHolder">
                    <LayoutTemplate>
                        <table id="list-header">
                            <tr>
                                <th style="width: 40px;">
                                </th>
                                <th style="width: 70px;">
                                    تاریخ
                                </th>
                                <th style="width: 80px;">
                                    کد آموزشگاه
                                </th>
                                <th style="width: 200px;">
                                    نام آموزشگاه
                                </th>
                                <th>
                                    منطقه
                                </th>
                                <th style="width: 50px;">
                                    پرسنل
                                </th>
                                <th style="width: 30px;">
                                    از
                                </th>
                                <th style="width: 30px;">
                                    به
                                </th>
                            </tr>
                        </table>
                        <div id="itemPlaceHolder" runat="server">
                        </div>
                        <div id="pager">
                            <asp:DataPager ID="DataPager1" runat="server" PagedControlID="lstArchive" PageSize="15">
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
                                <td style="width: 40px;">
                                    <%#Container.DataItemIndex + 1%>
                                </td>
                                <td style="width: 70px;">
                                    <%# Public.ToPersianDate(Eval("SubmitDate"))%>
                                </td>
                                <td style="width: 80px;">
                                    <%#Eval("SchoolCode")%>
                                </td>
                                <td style="width: 200px;">
                                    <%#Eval("SchoolName")%>
                                </td>
                                <td>
                                    <%#Eval("AreaName")%>
                                </td>
                                <td style="width: 50px;">
                                    <%# Eval("EmployeeType").ToString() == "C" ? "ثابت" : "متغیر"%>
                                </td>
                                <td style="width: 30px;">
                                    <%#Eval("FormerCount")%>
                                </td>
                                <td style="width: 30px;">
                                    <%#Eval("NextCount")%>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <table class="list-item-odd">
                            <tr>
                                <td style="width: 40px;">
                                    <%#Container.DataItemIndex + 1%>
                                </td>
                                <td style="width: 70px;">
                                    <%# Public.ToPersianDate(Eval("SubmitDate"))%>
                                </td>
                                <td style="width: 80px;">
                                    <%#Eval("SchoolCode")%>
                                </td>
                                <td style="width: 200px;">
                                    <%#Eval("SchoolName")%>
                                </td>
                                <td>
                                    <%#Eval("AreaName")%>
                                </td>
                                <td style="width: 50px;">
                                    <%# Eval("EmployeeType").ToString() == "C" ? "ثابت" : "متغیر"%>
                                </td>
                                <td style="width: 30px;">
                                    <%#Eval("FormerCount")%>
                                </td>
                                <td style="width: 30px;">
                                    <%#Eval("NextCount")%>
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
                SelectCountMethod="GetEmployeesArchiveCount" SelectMethod="LoadEmployeesArchives"
                TypeName="Paging" EnableViewState="False">
                <SelectParameters>
                    <asp:QueryStringParameter Name="DateFrom" Type="DateTime" />
                    <asp:QueryStringParameter Name="DateTo" Type="DateTime" />
                    <asp:QueryStringParameter Name="AreaCode" Type="Int32" />
                    <asp:QueryStringParameter Name="SchoolCode" Type="Int32" />
                    <asp:QueryStringParameter Name="EmployeeType" Type="String" />
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

        function getSchool() {
            if ($get('<%=txtAreaCode.ClientID %>', document).value != '' &&
                $get('<%=txtSchoolCode.ClientID %>', document).value != '') {
                $.ajax({
                    type: 'POST',
                    data: '{ sCode:' + $get('<%=txtSchoolCode.ClientID %>', document).value + ', aCode:' + $get('<%=txtAreaCode.ClientID %>', document).value + '}',
                    url: '../Utility.asmx/GetSchool',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    success: function (response) {
                        if (response.d != null) {
                            $get('<%=txtSchoolName.ClientID %>', document).value = response.d;
                        }
                        else {
                            $get('<%=txtSchoolCode.ClientID %>', document).value = '';
                            $get('<%=txtSchoolName.ClientID %>', document).value = '';
                            $get('<%=txtSchoolCode.ClientID %>', document).focus();
                        }
                    }
                });
            }
            else {
                $get('<%=txtSchoolCode.ClientID %>', document).value = '';
                $get('<%=txtSchoolName.ClientID %>', document).value = '';
            }
        }

    </script>
</asp:Content>

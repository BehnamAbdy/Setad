<%@ Page Title="تفکیک آموزشگاه" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="DisIntegration.aspx.cs" Inherits="School_DisIntegration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <div class="page-title">
        تفکیک آموزشگاه
    </div>
    <asp:UpdatePanel runat="server" ID="pnl">
        <ContentTemplate>
            <table style="margin-right: 160px;">
                <tr>
                    <td class="fieldName">
                        منطقه آموزشی :
                    </td>
                    <td>
                        <asp:TextBox ID="txtAreaCode" runat="server" CssClass="textbox-search" MaxLength="6"
                            onchange="javascript:getArea()" Width="70px" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                        <input type="button" id="btnAreaSearch" class="button-search" tabindex="-1" onclick="javascript:return areaList()" />
                        <asp:TextBox ID="txtAreaName" runat="server" CssClass="label-color" Width="300px"
                            Enabled="false" />
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        کد آموزشگاه :
                    </td>
                    <td>
                        <asp:TextBox ID="txtSchoolCode" runat="server" CssClass="textbox-search" AutoPostBack="true"
                            Width="70px" OnTextChanged="txtSchoolCode_TextChanged" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                        <asp:Button ID="btnSchoolCodeSearch" runat="server" CssClass="button-search" OnClientClick="javascript:return schoolsList()"
                            TabIndex="-1" />
                        <asp:TextBox ID="txtSchoolName" runat="server" CssClass="label-color" Width="300px"
                            Enabled="false"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        جنسیت :
                    </td>
                    <td>
                        <asp:TextBox ID="txtGender" runat="server" CssClass="label-color" Width="300px" Enabled="false"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        مقطع آموزشگاه :
                    </td>
                    <td>
                        <asp:TextBox ID="txtLevel" runat="server" CssClass="label-color" Width="300px" Enabled="false"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <br />
            <div class="button-box" style="padding-left: 20px; text-align: left;">
                <asp:Button UseSubmitBehavior="false" ID="btnAdd" runat="server" CssClass="button"
                    Text="آموزشگاه جدید" OnClick="btnAdd_Click" OnClientClick="javascript:hasSchool()" />
            </div>
            <br />
            <div class="grid-container" style="height: 150px;">
                <ComponentArt:Grid ID="grdSchools" runat="server" CssClass="Grid" EnableViewState="true"
                    AutoFocusSearchBox="False" SearchTextCssClass="GridHeaderText" CallbackCachingEnabled="true"
                    SearchOnKeyPress="true" SearchBoxCssClass="GridFooterText" ShowFooter="false"
                    HeaderCssClass="GridHeader" AllowPaging="false" PageSize="5" FooterCssClass="GridFooter"
                    AllowMultipleSelect="false" GroupByCssClass="GroupByCell" GroupByTextCssClass="GroupByText"
                    PagerStyle="Slider" PagerTextCssClass="GridFooterText" PagerButtonWidth="41"
                    PagerButtonHeight="22" SliderHeight="20" SliderWidth="150" HeaderHeight="27"
                    SliderGripWidth="9" SliderPopupOffsetX="20" SliderPopupClientTemplateId="SliderTemplate"
                    AllowHorizontalScrolling="true" GroupingPageSize="5" PreExpandOnGroup="true"
                    ImagesBaseUrl="../App_Themes/Default/images/ArtImage/" PagerImagesFolderUrl="../App_Themes/Default/images/ArtImage/pager/"
                    TreeLineImagesFolderUrl="../App_Themes/Default/images/ArtImage/lines/" TreeLineImageWidth="22"
                    TreeLineImageHeight="19" IndentCellWidth="22" LoadingPanelPosition="MiddleCenter"
                    GroupingNotificationText="" AllowColumnResizing="True" AllowTextSelection="True"
                    FillContainer="True" SearchText="جستجو:" ScrollBar="Auto" ScrollBarCssClass="ScrollBar"
                    ScrollButtonHeight="17" ScrollButtonWidth="16" ScrollGripCssClass="ScrollGrip"
                    ScrollImagesFolderUrl="../App_Themes/Default/images/ArtImage/scroller/" ScrollTopBottomImageHeight="2"
                    ScrollTopBottomImagesEnabled="True" ScrollTopBottomImageWidth="16" ScrollBarWidth="16"
                    ShowHeader="False" ShowSearchBox="True">
                    <Levels>
                        <ComponentArt:GridLevel ShowTableHeading="false" ShowSelectorCells="false" RowCssClass="Row"
                            ColumnReorderIndicatorImageUrl="reorder.gif" DataCellCssClass="DataCell" HeadingCellCssClass="HeadingCell"
                            HeadingCellHoverCssClass="HeadingCellHover" HoverRowCssClass="HoverRow" HeadingCellActiveCssClass="HeadingCellActive"
                            HeadingRowCssClass="HeadingRow" HeadingTextCssClass="HeadingCellText" SelectedRowCssClass="SelectedRow"
                            GroupHeadingCssClass="GroupHeading" SortAscendingImageUrl="asc.gif" SortDescendingImageUrl="desc.gif"
                            SortImageWidth="10" SortImageHeight="19">
                            <Columns>
                                <ComponentArt:GridColumn DataField="" HeadingText="ردیف" Align="Center" Width="35"
                                    DataCellClientTemplateId="RowIndex" />
                                <ComponentArt:GridColumn DataField="SchoolCode" HeadingText="کد آموزشگاه" Align="Center"
                                    Width="70" />
                                <ComponentArt:GridColumn DataField="SchoolName" HeadingText="نام آموزشگاه" Align="Center"
                                    Width="180" />
                                <ComponentArt:GridColumn DataField="LevelName" HeadingText="مقطع آموزشگاه" Align="Center"
                                    Width="100" />
                                <ComponentArt:GridColumn DataField="AreaName" HeadingText="منطقه آموزشی" Align="Center"
                                    Width="120" />
                                <ComponentArt:GridColumn DataField="SubmitDate" HeadingText="تاریخ" DataCellServerTemplateId="SubmitDate"
                                    Align="Center" Width="80" />
                            </Columns>
                        </ComponentArt:GridLevel>
                    </Levels>
                    <ClientTemplates>
                        <ComponentArt:ClientTemplate ID="RowIndex">
                            ## DataItem.Index + 1 ##
                        </ComponentArt:ClientTemplate>
                    </ClientTemplates>
                    <ServerTemplates>
                        <ComponentArt:GridServerTemplate ID="SubmitDate">
                            <Template>
                                <%#Public.ToPersianDate((DateTime)Container.DataItem["SubmitDate"])%>
                            </Template>
                        </ComponentArt:GridServerTemplate>
                    </ServerTemplates>
                </ComponentArt:Grid>
            </div>
            <asp:HiddenField ID="hdnDisIntegID" runat="server" />
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
                    $get('<%=hdnSchoolId.ClientID %>', document).value = school[0].Data[1];
                    $get('<%=txtSchoolCode.ClientID %>', document).value = school[0].Data[2];
                    return true;
                }
            }
            return false;
        }

        function hasSchool() {
            if ($get('<%=hdnSchoolId.ClientID %>', document).value == '' ||
                $get('<%=txtSchoolCode.ClientID %>', document).value == '') {
                return false;
            }
            return true;
        }

    </script>
</asp:Content>

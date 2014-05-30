<%@ Page Title="تعریف شهرستان ها" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Cities.aspx.cs" Inherits="BaseInfo_Cities" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <div class="page-title">
        شهرستان
    </div>
    <asp:UpdatePanel ID="pnl" runat="server">
        <ContentTemplate>
            <table style="margin-right: 150px;">
                <tr>
                    <td class="fieldName">
                        استان :
                    </td>
                    <td>
                        <asp:TextBox ID="txtProvinceCode" runat="server" CssClass="textbox-search" MaxLength="3"
                            Enabled="false" onchange="javascript:getProvince(this.value)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                        <input type="button" id="btnProvinceSearch" class="button-search" onclick="provinceList()"
                            tabindex="-1" disabled="disabled" />
                        <asp:TextBox ID="txtProvinceName" runat="server" CssClass="label-color" Width="280px"
                            Enabled="false"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        نام شهرستان :
                    </td>
                    <td>
                        <asp:TextBox ID="txtCityName" runat="server" CssClass="textbox" Width="250px" BackColor="White"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <div class="button-box">
                <asp:Button UseSubmitBehavior="false" ID="btnSave" runat="server" CssClass="button"
                    Text="ذخیره" OnClick="btnSave_Click" />
                <asp:Button ID="btnEdit" runat="server" CssClass="button" OnClick="btnEdit_Click"
                    Text="ویرایش" CausesValidation="false" OnClientClick="javascript:return isItemSelected(grdCities)" />
                <asp:Label ID="lblMessage" runat="server" CssClass="lbl-msg" EnableViewState="false" />
            </div>
            <div class="grid-container" style="height: 300px;">
                <ComponentArt:Grid ID="grdCities" runat="server" CssClass="Grid" EnableViewState="true"
                    AutoFocusSearchBox="False" SearchTextCssClass="GridHeaderText" CallbackCachingEnabled="true"
                    SearchOnKeyPress="true" SearchBoxCssClass="GridFooterText" ShowFooter="false"
                    HeaderCssClass="GridHeader" AllowPaging="false" PageSize="12" FooterCssClass="GridFooter"
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
                        <ComponentArt:GridLevel ShowTableHeading="false" DataKeyField="CityID" ShowSelectorCells="false"
                            RowCssClass="Row" ColumnReorderIndicatorImageUrl="reorder.gif" DataCellCssClass="DataCell"
                            HeadingCellCssClass="HeadingCell" HeadingCellHoverCssClass="HeadingCellHover"
                            HoverRowCssClass="HoverRow" HeadingCellActiveCssClass="HeadingCellActive" HeadingRowCssClass="HeadingRow"
                            HeadingTextCssClass="HeadingCellText" SelectedRowCssClass="SelectedRow" GroupHeadingCssClass="GroupHeading"
                            SortAscendingImageUrl="asc.gif" SortDescendingImageUrl="desc.gif" SortImageWidth="10"
                            SortImageHeight="19">
                            <Columns>
                                <ComponentArt:GridColumn HeadingText="ردیف" Align="Center" Width="35" DataCellClientTemplateId="RowIndex" />
                                <ComponentArt:GridColumn DataField="CityID" HeadingText="کد شهرستان" Align="Right"
                                    Width="100" />
                                <ComponentArt:GridColumn DataField="Name" HeadingText="نام شهرستان" Align="Right"
                                    Width="180" />
                            </Columns>
                        </ComponentArt:GridLevel>
                    </Levels>
                    <ClientTemplates>
                        <ComponentArt:ClientTemplate ID="RowIndex">
                            ## DataItem.Index + 1 ##
                        </ComponentArt:ClientTemplate>
                        <ComponentArt:ClientTemplate ID="LoadingFeedbackTemplate">
                            <table cellspacing="0" cellpadding="0" border="1" style="background: #ECE9D8">
                                <tr>
                                    <td style="font-size: 14px; font-family: Tahoma">
                                        در حال بارگذاری اطلاعات...&nbsp;
                                    </td>
                                    <td>
                                        <img src="../App_Themes/Default/images/ArtImage/spinner.gif" width="16" height="16"
                                            border="0" alt="" />
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:ClientTemplate>
                    </ClientTemplates>
                </ComponentArt:Grid>
            </div>
            <asp:HiddenField ID="hdnCityCode" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script src="../Scripts/javascript.js" type="text/javascript"></script>
    <script type="text/javascript">

        function provinceList() {
            var province = showProvincesList();
            if (province != null) {
                $get('<%=txtProvinceCode.ClientID %>', document).value = province[0].Data[0];
                $get('<%=txtProvinceName.ClientID %>', document).value = province[0].Data[1];
            }
        }

        function getProvince(code) {
            var province = BaseInfo_Cities.GetProvince(code);

            if (province.value != null) {
                $get('<%=txtProvinceName.ClientID %>', document).value = province.value;
            }
            else {
                $get('<%=txtProvinceCode.ClientID %>', document).value = '';
                $get('<%=txtProvinceName.ClientID %>', document).value = '';
                alert(codeNotFound);
            }

            $get('<%=hdnCityCode.ClientID %>', document).value = '';
            $get('<%=txtCityName.ClientID %>', document).value = '';
        }

    </script>
</asp:Content>

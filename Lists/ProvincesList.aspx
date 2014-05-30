<%@ Page Title="لیست استان ها" Language="C#" MasterPageFile="~/List.master" AutoEventWireup="true"
    CodeFile="ProvincesList.aspx.cs" Inherits="Lists_ProvincesList" %>

<%@ OutputCache Location="ServerAndClient" Duration="60" VaryByParam="none" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <script src="../Scripts/javascript.js" type="text/javascript"></script>
    <script type="text/javascript">

        function select() {
            getSelectedItems(grdProvinces);
        }

    </script>
    <div id="container">
        <div class="title-container">
            <span style="margin-right: 5px; width: 25px;">جستجو</span>
            <hr style="width: 230px;" />
        </div>
        <table>
            <tr>
                <td class="fieldName">
                    کد :
                </td>
                <td>
                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender" runat="server"
                        FilterType="Numbers" TargetControlID="txtCityCode" />
                    <asp:TextBox ID="txtCityCode" runat="server" CssClass="textbox" MaxLength="3" Width="40px"
                        onblur="filterGridBy(grdProvinces, 'Code', this.value);"></asp:TextBox>
                </td>
                <td class="fieldName">
                    نام :
                </td>
                <td>
                    <asp:TextBox ID="txtProvinceName" runat="server" CssClass="textbox" onblur="filterGrid(grdProvinces, 'ProvinceName', this.value)"></asp:TextBox>
                </td>
            </tr>
        </table>
        <hr />
        <div style="height: 315px; margin: 5px 4px 5px 5px;">
            <ComponentArt:Grid ID="grdProvinces" runat="server" CssClass="Grid" EnableViewState="true"
                AutoFocusSearchBox="False" SearchTextCssClass="GridHeaderText" CallbackCachingEnabled="true"
                SearchOnKeyPress="true" SearchBoxCssClass="GridFooterText" ShowFooter="false"
                HeaderCssClass="GridHeader" AllowPaging="false" PageSize="14" FooterCssClass="GridFooter"
                AllowMultipleSelect="false" GroupByCssClass="GroupByCell" GroupByTextCssClass="GroupByText"
                PagerStyle="Slider" PagerTextCssClass="GridFooterText" PagerButtonWidth="41"
                PagerButtonHeight="22" SliderHeight="20" SliderWidth="150" HeaderHeight="27"
                SliderGripWidth="9" SliderPopupOffsetX="20" SliderPopupClientTemplateId="SliderTemplate"
                AllowHorizontalScrolling="false" GroupingPageSize="5" PreExpandOnGroup="true"
                ImagesBaseUrl="../App_Themes/Default/images/ArtImage/" PagerImagesFolderUrl="../App_Themes/Default/images/ArtImage/pager/"
                TreeLineImagesFolderUrl="../App_Themes/Default/images/ArtImage/lines/" TreeLineImageWidth="22"
                TreeLineImageHeight="19" IndentCellWidth="22" LoadingPanelPosition="MiddleCenter"
                GroupingNotificationText="" AllowColumnResizing="True" AllowTextSelection="True"
                FillContainer="True" SearchText="جستجو:" ScrollBar="Auto" ScrollBarCssClass="ScrollBar"
                ScrollButtonHeight="17" ScrollButtonWidth="16" ScrollGripCssClass="ScrollGrip"
                ScrollImagesFolderUrl="../App_Themes/Default/images/ArtImage/scroller/" ScrollTopBottomImageHeight="2"
                ScrollTopBottomImagesEnabled="True" ScrollTopBottomImageWidth="16" ScrollBarWidth="16"
                ShowHeader="False" ShowSearchBox="True">
                <ClientEvents>
                    <ItemDoubleClick EventHandler="select" />
                </ClientEvents>
                <Levels>
                    <ComponentArt:GridLevel DataKeyField="ProvinceID" ShowTableHeading="false" ShowSelectorCells="false"
                        RowCssClass="Row" ColumnReorderIndicatorImageUrl="reorder.gif" DataCellCssClass="DataCell"
                        HeadingCellCssClass="HeadingCell" HeadingCellHoverCssClass="HeadingCellHover"
                        HeadingCellActiveCssClass="HeadingCellActive" HeadingRowCssClass="HeadingRow"
                        HeadingTextCssClass="HeadingCellText" SelectedRowCssClass="SelectedRow" GroupHeadingCssClass="GroupHeading"
                        SortAscendingImageUrl="asc.gif" SortDescendingImageUrl="desc.gif" SortImageWidth="10"
                        SortImageHeight="19">
                        <Columns>
                            <ComponentArt:GridColumn DataField="ProvinceID" HeadingText="کد" Align="Right" Width="80" />
                            <ComponentArt:GridColumn DataField="ProvinceName" HeadingText="نام استان" Align="Right"
                                Width="180" />
                        </Columns>
                    </ComponentArt:GridLevel>
                </Levels>
            </ComponentArt:Grid>
        </div>
        <div>
            <input type="button" id="btnSelect" value="انتخاب" class="button" onclick="getSelectedItems(grdProvinces)" />
            <%-- <asp:Button UseSubmitBehavior="false" ID="Button2" runat="server" Text="جدید" CssClass="button" />
                <asp:Button UseSubmitBehavior="false" ID="Button3" runat="server" Text="ویرایش" CssClass="button" />
                <asp:Button UseSubmitBehavior="false" ID="Button4" runat="server" Text="حذف" CssClass="button" />--%>
            <input type="button" id="btnBack" value="بازگشت" class="button" onclick="closeform()" />
        </div>
    </div>
</asp:Content>

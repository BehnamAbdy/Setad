<%@ Page Title="لیست شهرستان ها" Language="C#" MasterPageFile="~/List.master" AutoEventWireup="true"
    CodeFile="CitiesList.aspx.cs" Inherits="Lists_CitiesList" %>

<%@ OutputCache Location="ServerAndClient" Duration="300" VaryByParam="pId" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <script src="../Scripts/jquery-1.4.3.min.js" type="text/javascript"></script>
    <script src="../Scripts/javascript.js" type="text/javascript"></script>
    <script type="text/javascript">
        function select() {
            getSelectedItems(grdCities);
        }

        $(document).ready(function () {
            $('#lst-mst-wrapper').css({ 'height': '460px', 'width': '380px' });
            $('#lst-mst-topbar').text('لیست شهرستان ها');
        });
    </script>
    <table>
        <tr>
            <td class="fieldName">
                کد :
            </td>
            <td>
                <asp:TextBox ID="txtCityCode" runat="server" CssClass="textbox" MaxLength="3" Width="40px"
                    onblur="filterGridBy(grdCities, 'CityID', this.value);" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
            </td>
            <td class="fieldName">
                نام :
            </td>
            <td>
                <asp:TextBox ID="txtCityName" runat="server" CssClass="textbox" onblur="javascript:filterGrid(grdCities, 'Name', this.value)"></asp:TextBox>
            </td>
        </tr>
    </table>
    <div class="grid-container" style="height: 335px;">
        <ComponentArt:Grid ID="grdCities" runat="server" CssClass="Grid" EnableViewState="true"
            AutoFocusSearchBox="False" SearchTextCssClass="GridHeaderText" CallbackCachingEnabled="true"
            SearchOnKeyPress="true" SearchBoxCssClass="GridFooterText" ShowFooter="false"
            HeaderCssClass="GridHeader" AllowPaging="false" PageSize="15" FooterCssClass="GridFooter"
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
                <ComponentArt:GridLevel ShowTableHeading="false" ShowSelectorCells="false" RowCssClass="Row"
                    DataKeyField="CityID" ColumnReorderIndicatorImageUrl="reorder.gif" DataCellCssClass="DataCell"
                    HeadingCellCssClass="HeadingCell" HeadingCellHoverCssClass="HeadingCellHover"
                    HoverRowCssClass="HoverRow" HeadingCellActiveCssClass="HeadingCellActive" HeadingRowCssClass="HeadingRow"
                    HeadingTextCssClass="HeadingCellText" SelectedRowCssClass="SelectedRow" GroupHeadingCssClass="GroupHeading"
                    SortAscendingImageUrl="asc.gif" SortDescendingImageUrl="desc.gif" SortImageWidth="10"
                    SortImageHeight="19">
                    <Columns>
                        <ComponentArt:GridColumn DataField="CityID" HeadingText="کد" Align="Right" Width="80" />
                        <ComponentArt:GridColumn DataField="Name" HeadingText="نام شهر" Align="Right" Width="180" />
                    </Columns>
                </ComponentArt:GridLevel>
            </Levels>
        </ComponentArt:Grid>
    </div>
    <div class="button-box">
        <input type="button" id="btnSelect" value="انتخاب" class="button" onclick="javascript:getSelectedItems(grdCities)" />
        <input type="button" id="btnBack" value="برگشت" class="button" onclick="javascript:closeform()" />
    </div>
</asp:Content>

<%@ Page Title="لیست کالاها" Language="C#" MasterPageFile="~/List.master" AutoEventWireup="true"
    CodeFile="StuffsList.aspx.cs" Inherits="Lists_StuffsList" %>

<%@ OutputCache Location="ServerAndClient" Duration="60" VaryByParam="rId;ck" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <table>
        <tr>
            <td class="fieldName" style="width: 80px;">
                کد :
            </td>
            <td>
                <asp:TextBox ID="txtAreaCode" runat="server" CssClass="textbox" MaxLength="6" Width="60px"
                    onblur="filterGridBy(grdStuffs, 'StuffID', this.value);" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
            </td>
            <td class="fieldName">
                نام کالا :
            </td>
            <td>
                <asp:TextBox ID="txtAreaName" runat="server" CssClass="textbox" onblur="filterGrid(grdStuffs, 'StuffName', this.value)"
                    Width="140px"></asp:TextBox>
            </td>
        </tr>
    </table>
    <div class="grid-container" style="height: 370px;">
        <ComponentArt:Grid ID="grdStuffs" runat="server" CssClass="Grid" EnableViewState="true"
            AutoFocusSearchBox="False" SearchTextCssClass="GridHeaderText" CallbackCachingEnabled="true"
            SearchOnKeyPress="true" SearchBoxCssClass="GridFooterText" ShowFooter="false"
            HeaderCssClass="GridHeader" AllowPaging="false" PageSize="20" FooterCssClass="GridFooter"
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
            <ClientEvents>
                <ItemDoubleClick EventHandler="select" />
            </ClientEvents>
            <Levels>
                <ComponentArt:GridLevel ShowTableHeading="false" DataKeyField="StuffID" ShowSelectorCells="false"
                    RowCssClass="Row" ColumnReorderIndicatorImageUrl="reorder.gif" DataCellCssClass="DataCell"
                    HeadingCellCssClass="HeadingCell" HeadingCellHoverCssClass="HeadingCellHover"
                    HoverRowCssClass="HoverRow" HeadingCellActiveCssClass="HeadingCellActive" HeadingRowCssClass="HeadingRow"
                    HeadingTextCssClass="HeadingCellText" SelectedRowCssClass="SelectedRow" GroupHeadingCssClass="GroupHeading"
                    SortAscendingImageUrl="asc.gif" SortDescendingImageUrl="desc.gif" SortImageWidth="10"
                    SortImageHeight="19">
                    <Columns>
                        <ComponentArt:GridColumn DataField="StuffID" HeadingText="کد" Align="Right" Width="60" />
                        <ComponentArt:GridColumn DataField="StuffName" HeadingText="نام کالا" Align="Right"
                            Width="150" />
                    </Columns>
                </ComponentArt:GridLevel>
            </Levels>
        </ComponentArt:Grid>
    </div>
    <div class="button-box">
        <input type="button" id="btnSelect" value="انتخاب" class="button" onclick="getSelectedItems(grdStuffs)" />
        <input type="button" id="btnBack" value="بازگشت" class="button" onclick="closeform()" />
    </div>
    <script src="../Scripts/jquery-1.4.3.min.js" type="text/javascript"></script>
    <script src="../Scripts/javascript.js" type="text/javascript"></script>
    <script type="text/javascript">

        function select() {
            getSelectedItems(grdStuffs);
        }

        $(document).ready(function () {
            $('#lst-mst-wrapper').css({ 'height': '500px', 'width': '450px' });
            $('#lst-mst-topbar').text('لیست کالاها');
        });

    </script>
</asp:Content>

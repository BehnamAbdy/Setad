<%@ Page Title="لیست آموزشگاه ها" Language="C#" MasterPageFile="~/List.master" AutoEventWireup="true"
    CodeFile="SchoolsList.aspx.cs" Inherits="Lists_SchoolsList" %>

<%@ OutputCache Location="Client" Duration="180" VaryByParam="*" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <table style="margin-right: 50px;">
        <tr>
            <td class="fieldName" style="width: 80px;">
                کد :
            </td>
            <td>
                <asp:TextBox ID="txtCityCode" runat="server" CssClass="textbox" MaxLength="6" Width="60px"
                    onblur="filterGridBy(grdSchools, 'SchoolCode', this.value);" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
            </td>
            <td class="fieldName">
                نام آموزشگاه :
            </td>
            <td>
                <asp:TextBox ID="txtProvinceName" runat="server" CssClass="textbox" Width="140px"
                    onblur="filterGrid(grdSchools, 'SchoolName', this.value)"></asp:TextBox>
            </td>
        </tr>
    </table>
    <div class="grid-container" style="height: 480px;">
        <ComponentArt:Grid ID="grdSchools" runat="server" CssClass="Grid" EnableViewState="true"
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
                <ComponentArt:GridLevel DataKeyField="SchoolID" ShowTableHeading="false" ShowSelectorCells="false"
                    RowCssClass="Row" ColumnReorderIndicatorImageUrl="reorder.gif" DataCellCssClass="DataCell"
                    HeadingCellCssClass="HeadingCell" HeadingCellHoverCssClass="HeadingCellHover"
                    HoverRowCssClass="HoverRow" HeadingCellActiveCssClass="HeadingCellActive" HeadingRowCssClass="HeadingRow"
                    HeadingTextCssClass="HeadingCellText" SelectedRowCssClass="SelectedRow" GroupHeadingCssClass="GroupHeading"
                    SortAscendingImageUrl="asc.gif" SortDescendingImageUrl="desc.gif" SortImageWidth="10"
                    SortImageHeight="19">
                    <Columns>
                        <ComponentArt:GridColumn DataField="SchoolID" Visible="false" />
                        <ComponentArt:GridColumn DataField="" HeadingText="ردیف" Align="Center" Width="35"
                            DataCellClientTemplateId="RowIndex" />
                        <ComponentArt:GridColumn DataField="SchoolCode" HeadingText="کد آموزشگاه" Align="Center"
                            Width="70" />
                        <ComponentArt:GridColumn DataField="SchoolName" HeadingText="نام آموزشگاه" Align="Center"
                            Width="180" />
                        <ComponentArt:GridColumn DataField="LevelName" HeadingText="مقطع آموزشگاه" Align="Center"
                            Width="100" />
                        <ComponentArt:GridColumn DataField="AreaName" HeadingText="منطقه آموزشی" Align="Center"
                            Width="130" />
                        <ComponentArt:GridColumn DataField="Name" HeadingText="شهرستان" Align="Center" Width="120" />
                        <ComponentArt:GridColumn DataField="Phone" HeadingText="تلفن" Align="Center" Width="90" />
                    </Columns>
                </ComponentArt:GridLevel>
            </Levels>
            <ClientTemplates>
                <ComponentArt:ClientTemplate ID="RowIndex">
                    ## DataItem.Index + 1 ##
                </ComponentArt:ClientTemplate>
            </ClientTemplates>
        </ComponentArt:Grid>
    </div>
    <div class="button-box">
        <input type="button" id="btnSelect" value="انتخاب" class="button" onclick="javascript:getSelectedItems(grdSchools)" />
        <input type="button" id="btnBack" value="بازگشت" class="button" onclick="javascript:closeform()" />
    </div>
    <script src="../Scripts/jquery-1.4.3.min.js" type="text/javascript"></script>
    <script src="../Scripts/javascript.js" type="text/javascript"></script>
    <script type="text/javascript">

        function select() {
            getSelectedItems(grdSchools);
        }

        $(document).ready(function () {
            $('#lst-mst-wrapper').css('height', '600px');
            $('#lst-mst-topbar').text('لیست آموزشگاه ها');
        });

    </script>
</asp:Content>

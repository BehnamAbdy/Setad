<%@ Page Title="لیست کاربران" Language="C#" MasterPageFile="~/List.master" AutoEventWireup="true"
    CodeFile="UsersList.aspx.cs" Inherits="Lists_UsersList" %>

<%@ OutputCache Location="ServerAndClient" Duration="60" VaryByParam="none" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <table>
        <tr>
            <td class="fieldName" style="width: 50px;">
                نام :
            </td>
            <td>
                <asp:TextBox ID="txtName" runat="server" CssClass="textbox" Width="130px" onblur="filterGridBy(grdUsers, 'FirstName', this.value);"></asp:TextBox>
            </td>
            <td class="fieldName" style="width: 90px;">
                نام خانوادگی :
            </td>
            <td>
                <asp:TextBox ID="txtFamily" runat="server" CssClass="textbox" Width="130px" onblur="filterGridBy(grdUsers, 'LastName', this.value);"></asp:TextBox>
            </td>
        </tr>
    </table>
    <div class="grid-container" style="height: 315px;">
        <ComponentArt:Grid ID="grdUsers" runat="server" CssClass="Grid" EnableViewState="true"
            AutoFocusSearchBox="False" SearchTextCssClass="GridHeaderText" CallbackCachingEnabled="true"
            SearchOnKeyPress="true" SearchBoxCssClass="GridFooterText" ShowFooter="false"
            HeaderCssClass="GridHeader" AllowPaging="false" PageSize="13" FooterCssClass="GridFooter"
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
                <ComponentArt:GridLevel ShowTableHeading="false" DataKeyField="UserID" ShowSelectorCells="false"
                    RowCssClass="Row" ColumnReorderIndicatorImageUrl="reorder.gif" DataCellCssClass="DataCell"
                    HeadingCellCssClass="HeadingCell" HeadingCellHoverCssClass="HeadingCellHover"
                    HoverRowCssClass="HoverRow" HeadingCellActiveCssClass="HeadingCellActive" HeadingRowCssClass="HeadingRow"
                    HeadingTextCssClass="HeadingCellText" SelectedRowCssClass="SelectedRow" GroupHeadingCssClass="GroupHeading"
                    SortAscendingImageUrl="asc.gif" SortDescendingImageUrl="desc.gif" SortImageWidth="10"
                    SortImageHeight="19">
                    <Columns>
                        <ComponentArt:GridColumn DataField="" HeadingText="ردیف" Align="Center" Width="35"
                            DataCellClientTemplateId="RowIndex" />
                        <ComponentArt:GridColumn DataField="UserID" Visible="false" />
                        <ComponentArt:GridColumn DataField="UserName" HeadingText="نام کاربری" Align="Right"
                            Width="100" />
                        <ComponentArt:GridColumn DataField="FirstName" HeadingText="نام" Align="Right" Width="160" />
                        <ComponentArt:GridColumn DataField="LastName" HeadingText="نام خانوادگی" Align="Right" />
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
        <input type="button" id="btnSelect" value="انتخاب" class="button" onclick="getSelectedItems(grdUsers)" />
        <input type="button" id="btnBack" value="برگشت" class="button" onclick="closeform()" />
    </div>
    <script src="../Scripts/jquery-1.4.3.min.js" type="text/javascript"></script>
    <script src="../Scripts/javascript.js" type="text/javascript"></script>
    <script type="text/javascript">

        function select() {
            getSelectedItems(grdUsers);
        }

        $(document).ready(function () {
            $('#lst-mst-wrapper').css({ 'height': '440px', 'width': '450px' });
            $('#lst-mst-topbar').text('لیست کاربران');
        });

    </script>
</asp:Content>

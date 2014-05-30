<%@ Page Title="منطقه آموزشی" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Areas.aspx.cs" Inherits="BaseInfo_Areas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <div class="page-title">
        منطقه آموزشی
    </div>
    <asp:UpdatePanel ID="pnl" runat="server">
        <ContentTemplate>
            <table style="margin-right: 150px;">
                <tr>
                    <td class="fieldName">
                        کد آموزشی :
                    </td>
                    <td>
                        <asp:TextBox ID="txtAreaCode" runat="server" CssClass="textbox-search" MaxLength="6"
                            onkeypress="javascript:return isNumberKey(event)" AutoPostBack="True" OnTextChanged="txtAreaCode_TextChanged"></asp:TextBox>
                        <asp:Button ID="btnAreaSearch" runat="server" CssClass="button-search" OnClientClick="javascript:areaList()"
                            TabIndex="-1" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtAreaCode"
                            ErrorMessage="*"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        نام منطقه آموزشی :
                    </td>
                    <td>
                        <asp:TextBox ID="txtAreaName" runat="server" CssClass="textbox" Width="313px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtAreaName"
                            ErrorMessage="*"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        مدیر منطقه :
                    </td>
                    <td>
                        <input type="button" id="btnChiefSearch" class="button-search" onclick="getChief()" />
                        <asp:TextBox ID="txtChiefName" runat="server" CssClass="label-color" Width="280px"
                            ReadOnly="true"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <div class="button-box">
                <asp:Button ID="btnAdd" runat="server" CssClass="button" OnClick="btnAdd_Click" Text="ذخیره" />
                <asp:Button ID="btnEdit" runat="server" CssClass="button" OnClick="btnEdit_Click"
                    Text="ویرایش" CausesValidation="false" OnClientClick="javascript:return isItemSelected(grdAreas)" />
                <asp:Label ID="lblMessage" runat="server" CssClass="lbl-msg" EnableViewState="false" />
            </div>
            <div class="grid-container" style="height: 300px;">
                <ComponentArt:Grid ID="grdAreas" runat="server" CssClass="Grid" EnableViewState="true"
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
                        <ComponentArt:GridLevel ShowTableHeading="false" DataKeyField="AreaCode" ShowSelectorCells="false"
                            RowCssClass="Row" ColumnReorderIndicatorImageUrl="reorder.gif" DataCellCssClass="DataCell"
                            HeadingCellCssClass="HeadingCell" HeadingCellHoverCssClass="HeadingCellHover"
                            HoverRowCssClass="HoverRow" HeadingCellActiveCssClass="HeadingCellActive" HeadingRowCssClass="HeadingRow"
                            HeadingTextCssClass="HeadingCellText" SelectedRowCssClass="SelectedRow" GroupHeadingCssClass="GroupHeading"
                            SortAscendingImageUrl="asc.gif" SortDescendingImageUrl="desc.gif" SortImageWidth="10"
                            SortImageHeight="19">
                            <Columns>
                                <ComponentArt:GridColumn HeadingText="ردیف" Align="Center" Width="35" DataCellClientTemplateId="RowIndex" />
                                <ComponentArt:GridColumn DataField="AreaCode" HeadingText="کد منطقه" Align="Right"
                                    Width="100" />
                                <ComponentArt:GridColumn DataField="AreaName" HeadingText="نام منطقه" Align="Right"
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
            <asp:HiddenField ID="hdnUserID" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script src="../Scripts/javascript.js" type="text/javascript"></script>
    <script type="text/javascript">

        function areaList() {
            var area = showAreasList();
            if (area != null) {
                $get('<%=txtAreaCode.ClientID %>', document).value = area[0].Data[0];
                $get('<%=txtAreaName.ClientID %>', document).value = area[0].Data[1];
                $get('<%=txtChiefName.ClientID %>', document).value = area[0].Data[2];
                $get('<%=hdnUserID.ClientID %>', document).value = area[0].Data[3];
            }
        }

        function getChief() {
            var chief = showUserslist();

            if (chief != null) {
                $get('<%=hdnUserID.ClientID %>', document).value = chief[0].Data[1];
                $get('<%=txtChiefName.ClientID %>', document).value = chief[0].Data[2];
            }
        }

    </script>
</asp:Content>

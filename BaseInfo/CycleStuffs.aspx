<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="CycleStuffs.aspx.cs"
    Inherits="BaseInfo_CycleStuffs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <div class="page-title">
        <span id="spnTitle" runat="server"></span>
    </div>
    <asp:UpdatePanel ID="pnl" runat="server">
        <ContentTemplate>
            <table style="margin-right: 150px;">
                <tr>
                    <td class="fieldName" style="width: 90px; direction: rtl">
                        دوره:
                    </td>
                    <td>
                        <asp:DropDownList ID="drpCycles" runat="server" CssClass="dropdown-large" DataValueField="CycleID"
                            DataTextField="CycleName" AutoPostBack="true" OnSelectedIndexChanged="drpCycles_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:CompareValidator runat="server" ErrorMessage="*" ControlToValidate="drpCycles"
                            ValueToCompare="0" Operator="GreaterThan">
                        </asp:CompareValidator>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName" style="width: 90px;">
                        کالا :
                    </td>
                    <td>
                        <input type="button" id="btnSearch" class="button-search" tabindex="-1" onclick="javascript:stuffsList()" />
                        <asp:TextBox ID="txtStuffName" runat="server" CssClass="label-color" Width="188px"
                            ReadOnly="true" />
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        نمایش در لیست :
                    </td>
                    <td>
                        <div style="background-color: #c0c0e0; height: 20px; width: 250px;">
                            <asp:CheckBox ID="chkAvailable" runat="server" CssClass="checkbox" Checked="true" />
                        </div>
                    </td>
                </tr>
                <tr id="pnlDaily" runat="server" >
                    <td class="fieldName">
                        سهمیه روزانه :
                    </td>
                    <td>
                        <div style="background-color: #c0c0e0; height: 20px; width: 250px;">
                            <asp:CheckBox ID="chkIsDaily" runat="server" CssClass="checkbox" Checked="false" />
                        </div>
                    </td>
                </tr>
            </table>
            <div class="button-box">
                <asp:Button UseSubmitBehavior="false" ID="btnSave" runat="server" CssClass="button"
                    Text="ذخیره" OnClick="btnSave_Click" />
                <asp:Button ID="btnDelete" runat="server" CssClass="button" Text="حذف" OnClientClick="javascript:return isItemSelected(grdStuffs)"
                    CausesValidation="false" OnClick="btnDelete_Click" />
                <asp:Label ID="lblMessage" runat="server" CssClass="lbl-msg" EnableViewState="false" />
            </div>
            <div class="grid-container" style="height: 300px;">
                <ComponentArt:Grid ID="grdStuffs" runat="server" CssClass="Grid" EnableViewState="true"
                    AutoFocusSearchBox="False" SearchTextCssClass="GridHeaderText" CallbackCachingEnabled="true"
                    SearchOnKeyPress="true" SearchBoxCssClass="GridFooterText" ShowFooter="false"
                    HeaderCssClass="GridHeader" AllowPaging="false" PageSize="11" FooterCssClass="GridFooter"
                    AllowMultipleSelect="false" GroupByCssClass="GroupByCell" GroupByTextCssClass="GroupByText"
                    PagerStyle="Slider" PagerTextCssClass="GridFooterText" PagerButtonWidth="41"
                    PagerButtonHeight="22" SliderHeight="20" SliderWidth="150" HeaderHeight="27"
                    SliderGripWidth="9" SliderPopupOffsetX="20" SliderPopupClientTemplateId="SliderTemplate"
                    AllowHorizontalScrolling="false" GroupingPageSize="5" PreExpandOnGroup="true"
                    ImagesBaseUrl="../App_Themes/Default/images/ArtImage/" PagerImagesFolderUrl="../App_Themes/Default/images/ArtImage/pager/"
                    TreeLineImagesFolderUrl="../App_Themes/Default/images/ArtImage/lines/" TreeLineImageWidth="22"
                    TreeLineImageHeight="19" IndentCellWidth="22" LoadingPanelPosition="MiddleCenter"
                    GroupingNotificationText="" AllowColumnResizing="True" AllowTextSelection="True"
                    FillContainer="True" ScrollBar="Auto" ScrollBarCssClass="ScrollBar" ScrollButtonHeight="17"
                    ScrollButtonWidth="16" ScrollGripCssClass="ScrollGrip" ScrollImagesFolderUrl="../App_Themes/Default/images/ArtImage/scroller/"
                    ScrollTopBottomImageHeight="2" ScrollTopBottomImagesEnabled="True" ScrollTopBottomImageWidth="16"
                    ScrollBarWidth="16" ShowHeader="False" ShowSearchBox="True">
                    <Levels>
                        <ComponentArt:GridLevel ShowTableHeading="false" DataKeyField="StuffID" ShowSelectorCells="false"
                            RowCssClass="Row" ColumnReorderIndicatorImageUrl="reorder.gif" DataCellCssClass="DataCell"
                            HeadingCellCssClass="HeadingCell" HeadingCellHoverCssClass="HeadingCellHover"
                            HoverRowCssClass="HoverRow" HeadingCellActiveCssClass="HeadingCellActive" HeadingRowCssClass="HeadingRow"
                            HeadingTextCssClass="HeadingCellText" SelectedRowCssClass="SelectedRow" GroupHeadingCssClass="GroupHeading"
                            SortAscendingImageUrl="asc.gif" SortDescendingImageUrl="desc.gif" SortImageWidth="10"
                            SortImageHeight="19">
                            <Columns>
                                <ComponentArt:GridColumn DataField="StuffID" HeadingText="کد کالا" Align="Center"
                                    Width="80" />
                                <ComponentArt:GridColumn DataField="StuffName" HeadingText="نام کالا" Align="Center"
                                    Width="200" />
                                <ComponentArt:GridColumn DataField="Available" HeadingText="نمایش در لیست" Align="Center"
                                    Width="100" />
                                <ComponentArt:GridColumn Width="40" DataCellServerTemplateId="edit" Align="Center"
                                    HeadingText="ویرایش" />
                            </Columns>
                        </ComponentArt:GridLevel>
                    </Levels>
                    <ClientTemplates>
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
                    <ServerTemplates>
                        <ComponentArt:GridServerTemplate ID="edit">
                            <Template>
                                <asp:ImageButton ID="btnEdit" runat="server" CommandArgument='<%#Container.DataItem["StuffID"]%>'
                                    CausesValidation="false" OnClick="btnEdit_Click" ImageUrl="~/App_Themes/Default/images/edit.png"
                                    ToolTip="ویرایش" />
                            </Template>
                        </ComponentArt:GridServerTemplate>
                    </ServerTemplates>
                </ComponentArt:Grid>
            </div>
            <asp:HiddenField ID="hdnStuffId" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script src="../Scripts/jquery-1.4.3.min.js" type="text/javascript"></script>
    <script src="../Scripts/javascript.js" type="text/javascript"></script>
    <script type="text/javascript">

        function stuffsList() {
            var stuff = showdialog('../Lists/StuffsList.aspx?st=' + getQueryString('mode', null), null, 525, 480, false);
            if (stuff != null) {
                $get('<%=hdnStuffId.ClientID %>', document).value = stuff[0].Data[0];
                document.getElementById('txtStuffName').value = stuff[0].Data[1];
            }
        }

        function isStuffSelected() {
            if ($get('<%=hdnStuffId.ClientID %>', document).value == '') {
                $get('<%=lblMessage.ClientID %>', document).innerHTML = 'کالا را انتخاب کنید';
                return false;
            }
            return true;
        }

        function getQueryString(key, query) {
            if (!query)
                query = window.location.search;
            var re = new RegExp('[?|&]' + key + '=(.*?)&');
            var matches = re.exec(query + '&');
            if (!matches || matches.length < 2)
                return '';
            return decodeURIComponent(matches[1].replace('+', ' '));
        }
        
    </script>
</asp:Content>

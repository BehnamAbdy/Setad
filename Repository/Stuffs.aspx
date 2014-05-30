<%@ Page Title="مدیریت کالاهای انبار" Language="C#" MasterPageFile="~/Site.master"
    AutoEventWireup="true" CodeFile="Stuffs.aspx.cs" Inherits="Repository_Stuffs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <script src="../Scripts/javascript.js" type="text/javascript"></script>
    <div class="page-title">
        کالاهای انبار
    </div>
    <asp:UpdatePanel ID="pnl" runat="server">
        <ContentTemplate>
            <table style="margin-right: 150px;">
                <tr>
                    <td class="fieldName">
                        نوع کالا :
                    </td>
                    <td>
                        <asp:DropDownList ID="drpStuffTypes" runat="server" CssClass="dropdown-large" DataValueField="StuffTypeID"
                            DataTextField="TypeName" AutoPostBack="True" OnSelectedIndexChanged="drpStuffTypes_SelectedIndexChanged"
                            CausesValidation="false">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        نام کالا :
                    </td>
                    <td>
                        <asp:TextBox ID="txtGoodName" runat="server" CssClass="textbox-large"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
                            ControlToValidate="txtGoodName"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        واحد کلان کالا :
                    </td>
                    <td>
                        <asp:DropDownList ID="drpFirstUnit" runat="server" CssClass="dropdown-large" DataValueField="UnitID"
                            DataTextField="UnitName">
                        </asp:DropDownList>
                        <asp:CompareValidator ID="CompareValidator3" runat="server" ErrorMessage="*" ControlToValidate="drpFirstUnit"
                            ValueToCompare="0" Operator="GreaterThan" Type="Integer">*</asp:CompareValidator>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        ضریب تبدیل اول :
                    </td>
                    <td>
                        <asp:TextBox ID="txtFirstConversionCoefficient" runat="server" CssClass="textbox-large"
                            MaxLength="3" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*"
                            ControlToValidate="txtFirstConversionCoefficient"></asp:RequiredFieldValidator>
                    </td>
                    <tr>
                        <td class="fieldName">
                            واحد میانی کالا :
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSecondUnit" runat="server" CssClass="dropdown-large" DataValueField="UnitID"
                                DataTextField="UnitName">
                            </asp:DropDownList>
                            <asp:CompareValidator ID="CompareValidator2" runat="server" ErrorMessage="*" ControlToValidate="drpSecondUnit"
                                ValueToCompare="0" Operator="GreaterThan" Type="Integer">*</asp:CompareValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            ضریب تبدیل دوم :
                        </td>
                        <td>
                            <asp:TextBox ID="txtSecondConversionCoefficient" runat="server" CssClass="textbox-large"
                                MaxLength="3" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="*"
                                ControlToValidate="txtSecondConversionCoefficient"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            واحد خرد کالا :
                        </td>
                        <td>
                            <asp:DropDownList ID="drpThirdUnit" runat="server" CssClass="dropdown-large" DataValueField="UnitID"
                                DataTextField="UnitName">
                            </asp:DropDownList>
                            <asp:CompareValidator ID="CompareValidator4" runat="server" ErrorMessage="*" ControlToValidate="drpThirdUnit"
                                ValueToCompare="0" Operator="GreaterThan" Type="Integer">*</asp:CompareValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            شرکت سازنده :
                        </td>
                        <td>
                            <asp:TextBox ID="txtProducerCompany" runat="server" CssClass="textbox-large"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            کشور سازنده :
                        </td>
                        <td>
                            <asp:TextBox ID="txtProducerCountry" runat="server" CssClass="textbox-large"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            حجم(سانتی متر مربع) :
                        </td>
                        <td>
                            <asp:TextBox ID="txtCapacity" runat="server" CssClass="textbox-large" MaxLength="3"
                                onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            رنگ :
                        </td>
                        <td>
                            <asp:TextBox ID="txtColor" runat="server" CssClass="textbox-large"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            سایز(سانتی متر) :
                        </td>
                        <td>
                            <asp:TextBox ID="txtSize" runat="server" CssClass="textbox-large" MaxLength="3" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            توضیحات :
                        </td>
                        <td>
                            <asp:TextBox ID="txtTechnicalInfo" runat="server" CssClass="textbox-large" MaxLength="100"
                                TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
            </table>
            <div class="button-box">
                <asp:Button UseSubmitBehavior="false" ID="btnSave" runat="server" CssClass="button"
                    Text="ذخیره" OnClick="btnSave_Click" />
                <asp:Label ID="lblMessage" runat="server" CssClass="lbl-msg" EnableViewState="false" />
            </div>
            <div class="grid-container" style="height: 300px;">
                <ComponentArt:Grid ID="grdStuffs" runat="server" CssClass="Grid" EnableViewState="true"
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
                        <ComponentArt:GridLevel ShowTableHeading="false" DataKeyField="StuffID" ShowSelectorCells="false"
                            RowCssClass="Row" ColumnReorderIndicatorImageUrl="reorder.gif" DataCellCssClass="DataCell"
                            HeadingCellCssClass="HeadingCell" HeadingCellHoverCssClass="HeadingCellHover"
                            HoverRowCssClass="HoverRow" HeadingCellActiveCssClass="HeadingCellActive" HeadingRowCssClass="HeadingRow"
                            HeadingTextCssClass="HeadingCellText" SelectedRowCssClass="SelectedRow" GroupHeadingCssClass="GroupHeading"
                            SortAscendingImageUrl="asc.gif" SortDescendingImageUrl="desc.gif" SortImageWidth="10"
                            SortImageHeight="19">
                            <Columns>
                                <ComponentArt:GridColumn DataField="" HeadingText="ردیف" Align="Center" Width="35"
                                    DataCellClientTemplateId="RowIndex" />
                                <ComponentArt:GridColumn DataField="StuffID" HeadingText="کد" Align="Right" Width="60" />
                                <ComponentArt:GridColumn DataField="StuffName" HeadingText="نام کالا" Align="Right"
                                    Width="150" />
                                <ComponentArt:GridColumn Width="40" DataCellServerTemplateId="edit" Align="Center"
                                    HeadingText="ویرایش" />
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
</asp:Content>

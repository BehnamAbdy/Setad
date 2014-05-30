<%@ Page Title="فرم تعریف آموزشگاه" Language="C#" MasterPageFile="~/Site.master"
    AutoEventWireup="true" CodeFile="School.aspx.cs" Inherits="School_School" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <div class="page-title">
        فرم تعریف آموزشگاه
    </div>
    <asp:UpdatePanel ID="pnl" runat="server">
        <ContentTemplate>
            <div style="margin-right: 115px;">
                <table>
                    <tr>
                        <td class="fieldName">
                            کد آموزشگاه :
                        </td>
                        <td>
                            <asp:TextBox ID="txtSchoolCode" runat="server" CssClass="textbox-search" AutoPostBack="true"
                                Width="145px" OnTextChanged="txtSchoolCode_TextChanged" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                            <asp:Button ID="btnSchoolCodeSearch" runat="server" CssClass="button-search" OnClientClick="javascript:return schoolsList()"
                                TabIndex="-1" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtSchoolCode"
                                ErrorMessage="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            نام آموزشگاه :
                        </td>
                        <td>
                            <asp:TextBox ID="txtSchoolName" runat="server" CssClass="textbox-large"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtSchoolName"
                                ErrorMessage="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            مقطع آموزشگاه :
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSchoolLevel" runat="server" CssClass="dropdown-large" DataTextField="LevelName"
                                DataValueField="LevelID" AutoPostBack="True" OnSelectedIndexChanged="drpSchoolLevel_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="*" ControlToValidate="drpSchoolLevel"
                                ValueToCompare="0" Operator="GreaterThan" Type="Integer">*</asp:CompareValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            نوع آموزشگاه :
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSchoolKind" runat="server" CssClass="dropdown-large" DataTextField="SchoolKindName"
                                DataValueField="SchoolKindID">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            جنسیت :
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGender" runat="server" CssClass="dropdown-large" AutoPostBack="True"
                                OnSelectedIndexChanged="drpGender_SelectedIndexChanged">
                                <asp:ListItem Text="پسرانه"></asp:ListItem>
                                <asp:ListItem Text="دخترانه"></asp:ListItem>
                                <asp:ListItem Text="مختلط" Selected="true"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            منطقه آموزشی :
                        </td>
                        <td>
                            <asp:TextBox ID="txtAreaCode" runat="server" CssClass="textbox-search" MaxLength="6"
                                onchange="getArea(this.value)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                            <asp:Button ID="btnAreaSearch" runat="server" CssClass="button-search" OnClientClick="javascript:return areaList()"
                                TabIndex="-1" />
                            <asp:TextBox ID="txtAreaName" runat="server" CssClass="label-color" Width="230px"
                                ReadOnly="true"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtAreaCode"
                                ErrorMessage="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            استان :
                        </td>
                        <td>
                            <asp:TextBox ID="txtProvinceCode" runat="server" CssClass="textbox-search" MaxLength="3"
                                ReadOnly="true" onchange="getProvince(this.value)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                            <input type="button" id="btnProvinceSearch" class="button-search" onclick="provinceList()"
                                tabindex="-1" disabled="disabled" />
                            <asp:TextBox ID="txtProvinceName" runat="server" CssClass="label-color" Width="230px"
                                ReadOnly="true"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*"
                                ControlToValidate="txtProvinceName"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            شهرستان :
                        </td>
                        <td>
                            <asp:TextBox ID="txtCityCode" runat="server" CssClass="textbox-search" MaxLength="3"
                                onchange="getCity(this.value)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                            <input type="button" id="btnCitySearch" class="button-search" onclick="cityList()"
                                tabindex="-1" />
                            <asp:TextBox ID="txtCityName" runat="server" CssClass="label-color" Width="230px"
                                ReadOnly="true"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="*"
                                ControlToValidate="txtCityCode"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td class="fieldName">
                            دهستان :
                        </td>
                        <td>
                            <asp:TextBox ID="txtVillage" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                        <td class="fieldName">
                            روستا - خیابان :
                        </td>
                        <td>
                            <asp:TextBox ID="txtStreet" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            کوی - کوچه :
                        </td>
                        <td>
                            <asp:TextBox ID="txtAlly" runat="server" CssClass="textbox"></asp:TextBox>
                        </td>
                        <td class="fieldName">
                            پلاک :
                        </td>
                        <td>
                            <asp:TextBox ID="txtPelak" runat="server" CssClass="textbox" MaxLength="5" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            کد پستی :
                        </td>
                        <td>
                            <asp:TextBox ID="txtPostCode" runat="server" CssClass="textbox" MaxLength="10" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                        </td>
                        <td class="fieldName">
                            تلفن :
                        </td>
                        <td>
                            <asp:TextBox ID="txtPhone" runat="server" CssClass="textbox" MaxLength="15" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            تعداد کارکنان ثابت :
                        </td>
                        <td>
                            <asp:TextBox ID="txtFixed" runat="server" CssClass="textbox" MaxLength="4" onchange="getEmployeesCount()"
                                onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="*"
                                ControlToValidate="txtFixed"></asp:RequiredFieldValidator>
                        </td>
                        <td class="fieldName">
                            تعداد کارکنان متغیر :
                        </td>
                        <td>
                            <asp:TextBox ID="txtChangable" runat="server" CssClass="textbox" MaxLength="4" onchange="getEmployeesCount()"
                                onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ErrorMessage="*"
                                ControlToValidate="txtChangable"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td class="fieldName">
                            نام مدیر :
                        </td>
                        <td>
                            <asp:TextBox ID="txtManagerName" runat="server" CssClass="textbox-large"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ErrorMessage="*"
                                ControlToValidate="txtManagerName"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="fieldName">
                            نام معاون :
                        </td>
                        <td>
                            <asp:TextBox ID="txtAssistanceName" runat="server" CssClass="textbox-large"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
            <br />
            <asp:GridView ID="grdSubLevels" runat="server" AutoGenerateColumns="False" GridLines="None"
                HorizontalAlign="Center" CellPadding="4" ForeColor="#333333" Font-Size="16px"
                Width="600px" DataKeyNames="SubLevelID">
                <RowStyle Wrap="True" BackColor="#F7F6F3" ForeColor="#333333" />
                <Columns>
                    <asp:TemplateField HeaderText="پایه مقطع آموزشی">
                        <ItemTemplate>
                            <span style="font-size: 14px;">
                                <%#Eval("SubLevelName")%>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="تعداد پسر">
                        <ItemTemplate>
                            <asp:TextBox ID="txtBoysCount" runat="server" CssClass="textbox" Enabled='<%# ActivateByGender("M") %>'
                                Text='<%# Eval("BoysCount") %>' Width="60px" MaxLength="4" onchange="javascript:getStudentsCount()"
                                onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvBoys" runat="server" Enabled='<%# ActivateByGender("M") %>'
                                ErrorMessage="*" ControlToValidate="txtBoysCount"></asp:RequiredFieldValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="تعداد دختر">
                        <ItemTemplate>
                            <asp:TextBox ID="txtGirlsCount" runat="server" CssClass="textbox" Enabled='<%# ActivateByGender("F") %>'
                                Text='<%# Eval("GirlsCount") %>' Width="60px" MaxLength="4" onchange="javascript:getStudentsCount()"
                                onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvGirls" runat="server" Enabled='<%# ActivateByGender("F") %>'
                                ErrorMessage="*" ControlToValidate="txtGirlsCount"></asp:RequiredFieldValidator>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <EmptyDataTemplate>
                    <div style="text-align: center; width: 700px; direction: ltr; color: #bb4462; background-color: #ffffc7;">
                        .آیتمی وجود ندارد</div>
                </EmptyDataTemplate>
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <EditRowStyle BackColor="#999999" />
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            </asp:GridView>
            <div class="button-box">
                <asp:Button UseSubmitBehavior="false" ID="btnSave" runat="server" CssClass="button"
                    Text="ذخیره" OnClick="btnSave_Click" />
                <asp:Button UseSubmitBehavior="false" ID="btnEdit" runat="server" CssClass="button"
                    Text="ویرایش" OnClick="btnEdit_Click" Visible="false" />
                <asp:Label ID="lblMessage" runat="server" CssClass="lbl-msg" EnableViewState="false" />
            </div>
            <asp:HiddenField ID="hdnSchoolID" runat="server" Value="0" />
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

        function schoolsList() {
            var school = showSchoolsList();
            if (school != null) {
                $get('<%=hdnSchoolID.ClientID %>', document).value = school[0].Data[1];
                $get('<%=txtSchoolCode.ClientID %>', document).value = school[0].Data[2];
                return true;
            }
            return false;
        }

        function areaList() {
            var area = showAreasList();
            if (area != null) {
                $get('<%=txtAreaCode.ClientID %>', document).value = area[0].Data[0];
                $get('<%=txtAreaName.ClientID %>', document).value = area[0].Data[1];
            }
            return false;
        }

        function provinceList() {
            var province = showProvincesList();
            if (province != null) {
                $get('<%=txtProvinceCode.ClientID %>', document).value = province[0].Data[0];
                $get('<%=txtProvinceName.ClientID %>', document).value = province[0].Data[1];
            }
        }

        function cityList() {
            if ($get('<%=txtProvinceCode.ClientID %>', document).value != '') {
                var city = showCitiesList($get('<%=txtProvinceCode.ClientID %>', document).value);
                if (city != null) {
                    $get('<%=txtCityCode.ClientID %>', document).value = city[0].Data[0];
                    $get('<%=txtCityName.ClientID %>', document).value = city[0].Data[1];
                }
            }
            else {
                alert('ابتدا استان مورد نظرتان را انتخاب کنید');
            }
        }

        function getArea(code) {
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

        function getProvince(code) {
            $.ajax({
                type: 'POST',
                data: '{ code:' + code + '}',
                url: '../Utility.asmx/GetProvince',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (response) {
                    if (response.d != null) {
                        $get('<%=txtProvinceName.ClientID %>', document).value = response.d;
                    }
                    else {
                        $get('<%=txtProvinceCode.ClientID %>', document).value = '';
                        $get('<%=txtProvinceName.ClientID %>', document).value = '';
                        $get('<%=txtProvinceCode.ClientID %>', document).focus();
                    }
                }
            });

            $get('<%=txtCityCode.ClientID %>', document).value = '';
            $get('<%=txtCityName.ClientID %>', document).value = '';
        }

        function getCity(cCode) {
            if ($get('<%=txtProvinceCode.ClientID %>', document).value != '') {
                $.ajax({
                    type: 'POST',
                    data: '{ pCode:' + $get('<%=txtProvinceCode.ClientID %>', document).value + ',cCode:' + cCode + '}',
                    url: '../Utility.asmx/GetCity',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    success: function (response) {
                        if (response.d != null) {
                            $get('<%=txtCityName.ClientID %>', document).value = response.d;
                        }
                        else {
                            $get('<%=txtCityCode.ClientID %>', document).value = '';
                            $get('<%=txtCityName.ClientID %>', document).value = '';
                            $get('<%=txtCityCode.ClientID %>', document).focus();
                        }
                    }
                });
            }
            else {
                $get('<%=txtCityCode.ClientID %>', document).value = '';
                $get('<%=txtCityName.ClientID %>', document).value = '';
                alert('ابتدا استان مورد نظرتان را انتخاب کنید');
            }
        }
        
    </script>
</asp:Content>

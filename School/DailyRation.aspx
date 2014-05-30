<%@ Page Title="شیر روزانه آموزشگاه" Language="C#" MasterPageFile="~/Site.master"
    AutoEventWireup="true" CodeFile="DailyRation.aspx.cs" Inherits="School_DailyRation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <div class="page-title">
        شیر روزانه آموزشگاه
    </div>
    <asp:UpdatePanel ID="pnl" runat="server">
        <ContentTemplate>
            <table style="margin-right: 120px;">
                <tr>
                    <td class="fieldName">
                        کد آموزشگاه :
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="txtSchoolCode" runat="server" CssClass="textbox-search" AutoPostBack="True"
                            OnTextChanged="txtSchoolCode_TextChanged" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                        <asp:Button ID="btnSchoolCodeSearch" runat="server" CssClass="button-search" OnClientClick="javascript:return schoolsList()"
                            TabIndex="-1" />
                        <asp:TextBox ID="txtSchoolName" runat="server" CssClass="label-color" Width="286px"
                            ReadOnly="true"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtSchoolCode"
                            ErrorMessage="*"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
            <table style="margin-right: 120px;">
                <tr>
                    <td class="fieldName">
                        مقطع :
                    </td>
                    <td class="fieldName" style="text-align: center;">
                        <asp:Label ID="lblLevel" runat="server" CssClass="label-gray"></asp:Label>
                    </td>
                    <td class="fieldName">
                        جنسیت :
                    </td>
                    <td class="fieldName" style="text-align: center;">
                        <asp:Label ID="lblGender" runat="server" CssClass="label-gray" Width="149px"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        ماه :
                    </td>
                    <td>
                        <asp:DropDownList ID="drpMonths" runat="server" CssClass="dropdown" AutoPostBack="true"
                            Enabled="false" Width="157px" OnSelectedIndexChanged="drpMonths_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                    <td class="fieldName">
                        سال تحصیلی :
                    </td>
                    <td class="fieldName" style="text-align: center;">
                        <asp:Label ID="lblCycle" runat="server" Text="13.. _ 13.." Style="direction: ltr;
                            color: #3b76ee; font: bold 16px Arial;"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        پایه :
                    </td>
                    <td>
                        <asp:DropDownList ID="drpSublevel" runat="server" CssClass="dropdown" Enabled="false"
                            AutoPostBack="True" DataValueField="SchoolSubLevelID" DataTextField="SubLevelName"
                            Width="157px" OnSelectedIndexChanged="drpSublevel_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                    <td class="fieldName">
                        تعداد دانش آموزان :
                    </td>
                    <td class="fieldName" style="text-align: center;">
                        <asp:Label ID="lblStudentCount" runat="server" CssClass="label-gray"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="fieldName">
                        سهمیه :
                    </td>
                    <td>
                        <asp:DropDownList ID="drpStuff" runat="server" CssClass="textbox" DataTextField="StuffName"
                            DataValueField="CycleFoodID" Enabled="false" Width="157px" AutoPostBack="True"
                            OnSelectedIndexChanged="drpStuff_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                    <td colspan="2">
                        <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="drpStuff"
                            ValueToCompare="0" Operator="GreaterThan" Type="Integer" ErrorMessage="سهمیه را انتخاب کنید"></asp:CompareValidator>
                    </td>
                </tr>
            </table>
            <br />
            <div class="ration-list">
                <span id="td_1" runat="server" class="day">1 </span><span id="td_2" runat="server"
                    class="day">2 </span><span id="td_3" runat="server" class="day">3 </span><span id="td_4"
                        runat="server" class="day">4 </span><span id="td_5" runat="server" class="day">5
                </span><span id="td_6" runat="server" class="day">6 </span><span id="td_7" runat="server"
                    class="day">7 </span><span id="td_8" runat="server" class="day">8 </span><span id="td_9"
                        runat="server" class="day">9 </span><span id="td_10" runat="server" class="day">10
                </span>
            </div>
            <div class="ration-list">
                <asp:TextBox ID="txt_1" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_2" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_3" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_4" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_5" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_6" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_7" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_8" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_9" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_10" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
            </div>
            <br />
            <div class="ration-list">
                <span id="td_11" runat="server" class="day">11</span> <span id="td_12" runat="server"
                    class="day">12</span> <span id="td_13" runat="server" class="day">13</span>
                <span id="td_14" runat="server" class="day">14</span> <span id="td_15" runat="server"
                    class="day">15</span> <span id="td_16" runat="server" class="day">16</span>
                <span id="td_17" runat="server" class="day">17</span> <span id="td_18" runat="server"
                    class="day">18</span> <span id="td_19" runat="server" class="day">19</span>
                <span id="td_20" runat="server" class="day">20</span>
            </div>
            <div class="ration-list">
                <asp:TextBox ID="txt_11" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_12" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_13" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_14" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_15" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_16" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_17" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_18" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_19" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_20" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
            </div>
            <br />
            <div class="ration-list">
                <span id="td_21" runat="server" class="day">21</span> <span id="td_22" runat="server"
                    class="day">22</span> <span id="td_23" runat="server" class="day">23</span>
                <span id="td_24" runat="server" class="day">24</span> <span id="td_25" runat="server"
                    class="day">25</span> <span id="td_26" runat="server" class="day">26</span>
                <span id="td_27" runat="server" class="day">27</span> <span id="td_28" runat="server"
                    class="day">28</span> <span id="td_29" runat="server" class="day">29</span>
                <span id="td_30" runat="server" class="day">30</span>
            </div>
            <div class="ration-list">
                <asp:TextBox ID="txt_21" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_22" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_23" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_24" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_25" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_26" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_27" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_28" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_29" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <asp:TextBox ID="txt_30" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
            </div>
            <br />
            <div class="ration-list">
                <span id="td_31" runat="server" class="day">31</span>
            </div>
            <div class="ration-list">
                <asp:TextBox ID="txt_31" runat="server" CssClass="num" MaxLength="4" ReadOnly="true"
                    onchange="javascript:checkRationValue(this)" onkeypress="javascript:return isNumberKey(event)"></asp:TextBox>
                <table style="float: left;">
                    <tr>
                        <td class="fieldName">
                            جمع کل :
                        </td>
                        <td>
                            <asp:TextBox ID="txtSum" runat="server" CssClass="textbox" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
            <br />
            <div class="button-box">
                <asp:Button UseSubmitBehavior="false" ID="btnSave" runat="server" CssClass="button"
                    Text="ذخیره" OnClick="btnSave_Click" />
                <asp:Label ID="lblMessage" runat="server" CssClass="lbl-msg" EnableViewState="false" />
            </div>
            <asp:HiddenField ID="hdnSchoolId" runat="server" />
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
                $get('<%=hdnSchoolId.ClientID %>', document).value = school[0].Data[1];
                $get('<%=txtSchoolCode.ClientID %>', document).value = school[0].Data[2];
                return true;
            }
            return false;
        }

        function setSum() {
            var sum = 0;
            if ($get('<%=txt_1.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_1.ClientID %>', document).value);
            }
            if ($get('<%=txt_2.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_2.ClientID %>', document).value);
            }
            if ($get('<%=txt_3.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_3.ClientID %>', document).value);
            }
            if ($get('<%=txt_4.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_4.ClientID %>', document).value);
            }
            if ($get('<%=txt_5.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_5.ClientID %>', document).value);
            }
            if ($get('<%=txt_6.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_6.ClientID %>', document).value);
            }
            if ($get('<%=txt_7.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_7.ClientID %>', document).value);
            }
            if ($get('<%=txt_8.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_8.ClientID %>', document).value);
            }
            if ($get('<%=txt_9.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_9.ClientID %>', document).value);
            }
            if ($get('<%=txt_10.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_10.ClientID %>', document).value);
            }
            if ($get('<%=txt_11.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_11.ClientID %>', document).value);
            }
            if ($get('<%=txt_12.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_12.ClientID %>', document).value);
            }
            if ($get('<%=txt_13.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_13.ClientID %>', document).value);
            }
            if ($get('<%=txt_14.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_14.ClientID %>', document).value);
            }
            if ($get('<%=txt_15.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_15.ClientID %>', document).value);
            }
            if ($get('<%=txt_16.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_16.ClientID %>', document).value);
            }
            if ($get('<%=txt_17.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_17.ClientID %>', document).value);
            }
            if ($get('<%=txt_18.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_18.ClientID %>', document).value);
            }
            if ($get('<%=txt_19.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_19.ClientID %>', document).value);
            }
            if ($get('<%=txt_20.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_20.ClientID %>', document).value);
            }
            if ($get('<%=txt_21.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_21.ClientID %>', document).value);
            }
            if ($get('<%=txt_22.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_22.ClientID %>', document).value);
            }
            if ($get('<%=txt_23.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_23.ClientID %>', document).value);
            }
            if ($get('<%=txt_24.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_24.ClientID %>', document).value);
            }
            if ($get('<%=txt_25.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_25.ClientID %>', document).value);
            }
            if ($get('<%=txt_26.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_26.ClientID %>', document).value);
            }
            if ($get('<%=txt_27.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_27.ClientID %>', document).value);
            }
            if ($get('<%=txt_28.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_28.ClientID %>', document).value);
            }
            if ($get('<%=txt_29.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_29.ClientID %>', document).value);
            }
            if ($get('<%=txt_30.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_30.ClientID %>', document).value);
            }
            if ($get('<%=txt_31.ClientID %>', document).value != '') {
                sum += parseInt($get('<%=txt_31.ClientID %>', document).value);
            }
            $get('<%=txtSum.ClientID %>', document).value = sum;
        }

        function checkRationValue(txt) {
            if (parseInt(txt.value) > parseInt($get('<%=lblStudentCount.ClientID %>', document).innerHTML)) {
                window.alert('مقدار وارد شده از تعداد آمار بزرگتر است!');
                txt.focus();
                txt.value = '';
            }
            else {
                setSum();
            }
        }

    </script>
</asp:Content>

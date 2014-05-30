<%@ Page Language="C#" MasterPageFile="~/List.master" AutoEventWireup="true" CodeFile="CalanderMonthly.aspx.cs"
    Inherits="BaseInfo_CalanderMonthly" Title="تنظیم تقویم کاری" %>

<%@ OutputCache Location="None" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <asp:ScriptManager ID="scm" runat="server" />
    <asp:UpdatePanel ID="pnl" runat="server">
        <ContentTemplate>
            <table style="background-color: #ede8e1; margin-right: 7px; width: 97%;">
                <tr>
                    <td class="fieldName" style="width: 70px;">
                        سال :
                    </td>
                    <td>
                        <asp:Label ID="lblYear" runat="server" Font-Size="17px" ForeColor="#38385e"></asp:Label>
                    </td>
                    <td class="fieldName">
                        ماه :
                    </td>
                    <td style="padding-top: 4px;">
                        <asp:DropDownList ID="drpMonth" runat="server" CssClass="dropdown" AutoPostBack="True"
                            OnSelectedIndexChanged="drpMonth_SelectedIndexChanged">
                            <asp:ListItem Value="1" Text="فروردین"></asp:ListItem>
                            <asp:ListItem Value="2" Text="اردیبهشت"></asp:ListItem>
                            <asp:ListItem Value="3" Text="خرداد"></asp:ListItem>
                            <asp:ListItem Value="4" Text="تیر"></asp:ListItem>
                            <asp:ListItem Value="5" Text="مرداد"></asp:ListItem>
                            <asp:ListItem Value="6" Text="شهریور"></asp:ListItem>
                            <asp:ListItem Value="7" Text="مهر"></asp:ListItem>
                            <asp:ListItem Value="8" Text="آبان"></asp:ListItem>
                            <asp:ListItem Value="9" Text="آذر"></asp:ListItem>
                            <asp:ListItem Value="10" Text="دی"></asp:ListItem>
                            <asp:ListItem Value="11" Text="بهمن"></asp:ListItem>
                            <asp:ListItem Value="12" Text="اسفند"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
            <div class="button-box">
            </div>
            <div class="week">
                <div class="day" style="background-color: #cdcddd;">
                    شنبه
                </div>
                <div class="day" style="background-color: #cdcddd;">
                    یکشنبه
                </div>
                <div class="day" style="background-color: #cdcddd;">
                    دوشنبه
                </div>
                <div class="day" style="background-color: #cdcddd;">
                    سه شنبه
                </div>
                <div class="day" style="background-color: #cdcddd;">
                    چهارشنبه
                </div>
                <div class="day" style="background-color: #cdcddd;">
                    پنج شنبه
                </div>
                <div class="day" style="background-color: #cdcddd; border-left: solid 1px #9a9a9a;">
                    جمعه
                </div>
            </div>
            <div class="week">
                <div id="d1" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d2" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d3" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d4" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d5" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d6" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d7" runat="server" class="day" onclick="javascript:pickDay(this, true)"
                    style="border-left: solid 1px #9a9a9a;">
                </div>
            </div>
            <div class="week">
                <div id="d8" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d9" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d10" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d11" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d12" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d13" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d14" runat="server" class="day" onclick="javascript:pickDay(this, true)"
                    style="border-left: solid 1px #9a9a9a;">
                </div>
            </div>
            <div class="week">
                <div id="d15" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d16" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d17" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d18" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d19" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d20" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d21" runat="server" class="day" onclick="javascript:pickDay(this, true)"
                    style="border-left: solid 1px #9a9a9a;">
                </div>
            </div>
            <div class="week">
                <div id="d22" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d23" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d24" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d25" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d26" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d27" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d28" runat="server" class="day" onclick="javascript:pickDay(this, true)"
                    style="border-left: solid 1px #9a9a9a;">
                </div>
            </div>
            <div class="week">
                <div id="d29" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d30" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d31" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d32" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d33" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d34" runat="server" class="day" onclick="javascript:pickDay(this, false)">
                </div>
                <div id="d35" runat="server" class="day" onclick="javascript:pickDay(this, true)"
                    style="border-left: solid 1px #9a9a9a;">
                </div>
            </div>
            <div class="week" style="border-bottom: solid 1px #9a9a9a;">
                <div id="d36" runat="server" class="day" onclick="pickDay(this)">
                </div>
                <div id="d37" runat="server" class="day" onclick="pickDay(this)">
                </div>
                <div id="d38" runat="server" class="day" onclick="pickDay(this)">
                </div>
                <div id="d39" runat="server" class="day" onclick="pickDay(this)">
                </div>
                <div id="d40" runat="server" class="day" onclick="pickDay(this)">
                </div>
                <div id="d41" runat="server" class="day" onclick="pickDay(this)">
                </div>
                <div id="d42" runat="server" class="day" onclick="pickDay(this)" style="border-left: solid 1px #9a9a9a;">
                </div>
            </div>
            <div class="button-box">
                <asp:HiddenField ID="hdnDay" runat="server" />
                <asp:Button UseSubmitBehavior="false" ID="btnSave" runat="server" CssClass="button"
                    Text="خارج از محدوده" OnClick="btnSave_Click" />
                <input type="button" id="btnBack" class="button" value="بازگشت" onclick="self.close();" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <script src="../Scripts/jquery-1.4.3.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="../Scripts/javascript.js"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            $('#lst-mst-wrapper').css({ 'height': '370px', 'width': '500px' });
            $('#lst-mst-topbar').text('تنظیم تقویم کاری');
        });

        function pickDay(day, isFriday) {
            resetFonts();
            day.style.fontSize = '20px';
            day.style.fontWeight = 'bold';
            var btn = $get('<%=btnSave.ClientID %>', document);
            var hdnDay = $get('<%=hdnDay.ClientID %>', document);

            if (isFriday) {
                hdnDay.value = '';
                btn.value = 'خارج از محدوده';
                btn.disabled = true;
                return;
            }
            else if (day.style.backgroundColor == '#f2f200' || day.style.backgroundColor == 'rgb(242, 242, 0)') {
                hdnDay.value = '1_' + (day.innerHTML.length == 1 ? '0' + day.innerHTML : day.innerHTML);
                btn.value = 'روز کاری';
                btn.disabled = false;
            }
            else {
                hdnDay.value = '2_' + (day.innerHTML.length == 1 ? '0' + day.innerHTML : day.innerHTML);
                btn.value = 'روز تعطیل';
                btn.disabled = false;
            }
        }

        function resetFonts() {
            var day = $get('<%=d1.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d2.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d3.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d4.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d5.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d6.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d7.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d8.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d9.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d10.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d11.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d12.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d13.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d14.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d15.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d16.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d17.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d18.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d19.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d20.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d21.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d22.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d23.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d24.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d25.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d26.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d27.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d28.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d29.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d30.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d31.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d32.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d33.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d34.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d35.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d36.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d37.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d38.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d39.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d40.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d41.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';

            day = $get('<%=d42.ClientID %>', document);
            day.style.fontSize = '13px';
            day.style.fontWeight = 'normal';
        }
    </script>
</asp:Content>

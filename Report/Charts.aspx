<%@ Page Title="گزارش نموداری" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="Charts.aspx.cs" Inherits="Report_Charts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="Server">
    <div class="page-title">
    </div>
    <asp:UpdatePanel ID="pnl" runat="server">
        <ContentTemplate>
            <table style="margin: 0 auto;">
                <tr>
                    <td class="fieldName" style="width: 60px;">
                        دوره :
                    </td>
                    <td>
                        <asp:DropDownList ID="drpCycle" runat="server" DataValueField="CycleID" DataTextField="CycleName"
                            CssClass="dropdown" AutoPostBack="True" OnSelectedIndexChanged="drpCycle_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                    <td class="fieldName" style="width: 60px;">
                        ماه :
                    </td>
                    <td>
                        <asp:DropDownList ID="drpMonths" runat="server" CssClass="dropdown" onchange="javascript:resetChart()">
                        </asp:DropDownList>
                    </td>
                    <td class="fieldName" style="width: 60px;">
                        کالا :
                    </td>
                    <td>
                        <asp:DropDownList ID="drpStuffs" runat="server" CssClass="dropdown" DataTextField="StuffName"
                            DataValueField="CycleStuffID" Width="160px" onchange="javascript:chart()">
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div id="chart" style="direction: ltr; height: 400px;">
    </div>
    <script src="../Scripts/javascript.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-1.4.3.min.js" type="text/javascript"></script>
    <script src="../Scripts/highcharts.js" type="text/javascript"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            switch (getQueryString(window.location.search, 'mode')) {
                case '1':
                    $('.page-title').text('نمودار تغذیه مناطق');
                    break;

                case '2':
                    $('.page-title').text('نمودار نوشت افزار مناطق');
                    break;

                case '3':
                    $('.page-title').text('نمودار پوشاک مناطق');
                    break;
            }
            $('<img src="../App_Themes/Default/images/chart.png" style="position: absolute; bottom: 2px; left: 3px;"/>').appendTo('.page-title');
        });

        function chart() {
            if ($get('<%=drpStuffs.ClientID%>').selectedIndex == 0) {
                return;
            }

            $('#chart').html('<img src="../App_Themes/Default/images/loading.gif" style="margin: 20px 0 0 47%;" />');
            var year = 0;
            var month = 0;
            if ($get('<%=drpMonths.ClientID%>').selectedIndex > 0) {
                var ym = $get('<%=drpMonths.ClientID%>').value.split('|');
                year = ym[0];
                month = ym[1];
            }

            $.ajax({
                type: 'GET',
                data: ({ mode: getQueryString(window.location.search, 'mode'), cs: $get('<%=drpStuffs.ClientID%>').value, y: year, m: month }),
                url: '../Report/Charts.aspx',
                dataType: 'json',
                cache: false,
                success: function (result) {
                    var categories = new Array();
                    var name = 'نمودار';

                    var data = new Array();
                    for (index in result) {
                        categories.push(result[index].AreaName);
                        data.push(
                        {
                            name: result[index].AreaName,
                            y: result[index].Count,
                            color: get_random_color()
                        }
                        );
                    }

                    if (categories.length > 20) {
                        $('#wrapper').css('width', '1200px');
                    }
                    else {
                        $('#wrapper').css('width', '830px');
                    }

                    var chart = new Highcharts.Chart({
                        chart: {
                            renderTo: 'chart',
                            type: 'column'
                        },
                        title: {
                            text: ''
                            //text: mode == 1 ? 'آمار پرونده های متخلف شهرستانها' : 'آمار پرونده های سایر شهرستانها'
                        },
                        //                        subtitle: {
                        //                            text: 'Click the columns to view versions. Click again to view brands.'
                        //                        },
                        xAxis: {
                            categories: categories
                        },
                        yAxis: {
                            title: {
                                text: ''
                            },
                            labels: {
                                formatter: function () {
                                    return this.value;
                                }
                            }
                        },
                        plotOptions: {
                            column: {
                                cursor: 'pointer',
                                dataLabels: {
                                    enabled: true,
                                    color: '#4d4d4d',
                                    style: {
                                        fontWeight: 'bold'
                                    },
                                    formatter: function () {
                                        return this.y;
                                    }
                                }
                            }
                        },
                        tooltip: {
                            formatter: function () {
                                if (this.point.name) { // the pie chart
                                    s = '' +
									this.point.name + ': ' + this.y;
                                }
                                else {
                                    var point = this.point,
								s = this.x + ':<b>' + this.y + '% </b><br/>';
                                }
                                return s;
                            }
                        },
                        credits: {
                            enabled: false
                        },
                        series: [{
                            name: name,
                            data: data,
                            color: 'white'
                        }
                        //                        , {
                        //                            type: 'pie',
                        //                            name: 'Total consumption',
                        //                            data: data,
                        //                            center: [100, 50],
                        //                            size: 100,
                        //                            showInLegend: true,
                        //                            dataLabels: {
                        //                                enabled: false
                        //                            }
                        //                        }
                        ],
                        exporting: {
                            enabled: false
                        }
                    });
                }
            });
        }

        function get_random_color() {
            var letters = '0123456789ABCDEF'.split('');
            var color = '#';
            for (var i = 0; i < 6; i++) {
                color += letters[Math.round(Math.random() * 15)];
            }
            return color;
        }

        function resetChart() {
            $get('<%=drpStuffs.ClientID%>').selectedIndex = 0;
            $('#chart').empty();
        }
        //        function get_random_color() {
        //            function c() {
        //                return Math.floor(Math.random() * 256).toString(16)
        //            }
        //            return "#" + c() + c() + c();
        //        }
    </script>
</asp:Content>

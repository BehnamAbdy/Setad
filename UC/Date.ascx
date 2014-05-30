<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Date.ascx.cs" Inherits="UC_Date" %>

<script type="text/javascript" language="javascript">

    function DateValidate(control) {
        if (control.value == '____' + control.value.charAt(4) + '__' + control.value.charAt(4) + '__')
            return;
        control.value = control.value.replace(/\_/g, '0');
        var str = control.value.split(control.value.charAt(4));
        var day = parseFloat(str[2]);
        var month = parseFloat(str[1]);
        var year = parseFloat(str[0]);
        if (day > 31 || day < 1 || month > 12 || month < 1 || year > 1500 || year < 1300) {
            alert('تاریخ معتبر نمی باشد');
            control.value = '____' + control.value.charAt(4) + '__' + control.value.charAt(4) + '__';
            control.focus();
        }
    }

</script>

<asp:TextBox ID="txtDate" runat="server" dir="ltr" CssClass="textbox" onblur="DateValidate(this);"
    CausesValidation="true"></asp:TextBox>
<ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender" runat="server" Mask="9999/99/99"
    UserDateFormat="YearMonthDay" MaskType="None" TargetControlID="txtDate" ClearTextOnInvalid="false"
    ClearMaskOnLostFocus="false">
</ajaxToolkit:MaskedEditExtender>
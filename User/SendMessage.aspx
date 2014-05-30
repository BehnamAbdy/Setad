<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SendMessage.aspx.cs" Inherits="User_SendMessage"
    ValidateRequest="false" %>

<%@ OutputCache Location="Client" Duration="200" VaryByParam="none" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" dir="rtl">
<head runat="server">
    <title></title>
    <script src="../Scripts/jquery-1.4.3.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.cleditor.min.js" type="text/javascript"></script>
    <link href="../Styles/jquery.cleditor.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-ui.custom.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.cookie.js" type="text/javascript"></script>
    <link href="../Scripts/src/skin/ui.dynatree.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/src/jquery.dynatree.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">

        parent.document.getElementById('myiframe').style.height = '490px';

        $(document).ready(function () {
            loadTree(0);
            $('#txtEditor').cleditor();
        });

        function loadTree(mode) {
            document.getElementById('hdnUserInRoles').value = '';
            $('#snipper').css('visibility', 'visible');
            $.ajax({
                type: 'GET',
                data: { trv: mode },
                url: '../User/SendMessage.aspx',
                dataType: 'json',
                cache: true,
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    $('#tree').html('');
                    $("<div id='trvUsers' style='border:none;'></div>").appendTo('#tree');
                    var dv = $("<div id='trvUsers'></div>");
                    $(dv).dynatree({
                        checkbox: true,
                        selectMode: 3,
                        children: result,
                        onSelect: function (select, node) {
                            var selKeys = $.map(node.tree.getSelectedNodes(), function (node) {
                                return node.data.key;
                            });
                            document.getElementById('hdnUserInRoles').value = selKeys.join(',');
                        },
                        onDblClick: function (node, event) {
                            node.toggleSelect();
                        },
                        onKeydown: function (node, event) {
                            if (event.which == 32) {
                                node.toggleSelect();
                                return false;
                            }
                        },
                        cookieId: "dynatree-Cb3",
                        idPrefix: "dynatree-Cb3-"
                    });
                    $(dv).appendTo('#tree');
                    $('#snipper').css('visibility', 'hidden');
                }
            });
            $('.lbl-msg').text('');
        }

        function sendMsg() {
            var body = document.getElementById('txtEditor').value;
            if (body == '') {
                $('.lbl-msg').text('متن آیتم نوشته نشده');
                return;
            }

            var subject = document.getElementById('txtSubject').value;
            if (subject == '') {
                $('.lbl-msg').text('عنوان پیام نوشته نشده');
                return;
            }

            var userInRoles = document.getElementById('hdnUserInRoles').value;
            if (userInRoles == '') {
                $('.lbl-msg').text('گیرنده پیام انتخاب نشده');
                return;
            }

            $.ajax({
                type: 'POST',
                data: ({ sbj: subject, bdy: body, ids: userInRoles }),
                url: '../User/SendMessage.aspx',
                dataType: 'json',
                success: function (result) {
                    if (result == '1') {
                        $('.lbl-msg').text('پیام فرستاده شد');
                    }
                }
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="float: right;">
        <input id="rbnAreas" type="radio" name="rbn" checked="checked" onclick="javascript:loadTree(0)"
            style="margin-right: 10px;" />
        <label style="color: #4395bc; font: bold 13px Arial;">
            مدیران</label><input id="rbnSchools" name="rbn" type="radio" onclick="javascript:loadTree(1)"
                style="margin-right: 10px;" />
        <label style="color: #4395bc; font: bold 13px Arial;">
            مدیران آموزشگاه ها</label>&nbsp;<img id="snipper" src="../App_Themes/Default/images/ajax-loader.gif" />
        <div id="tree">
        </div>
    </div>
    <div style="float: left; margin: 26px 0 10px 0; width: 560px;">
        <textarea id="txtEditor" name="txtEditor" cols="60" rows="10"></textarea>
    </div>
    <table class="clear" style="margin-right: 270px;">
        <tr>
            <td class="fieldName" style="width: 60px;">
                عنوان پیام :
            </td>
            <td>
                <input type="text" id="txtSubject" class="textbox-large" />
                &nbsp;
                <input type="button" id="btnSend" onclick="javascript:sendMsg()" class="button" value="ثبت" />
                &nbsp;
                <label class="lbl-msg">
                </label>
            </td>
        </tr>
    </table>
    <input type="hidden" id="hdnEditorContent" />
    <input type="hidden" id="hdnUserInRoles" />
    </form>
</body>
</html>

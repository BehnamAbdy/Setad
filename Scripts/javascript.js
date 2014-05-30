function isItemSelected(a)
{ var b = a.getSelectedItems(); if (b.length == 0) { alert("ركوردي انتخاب نشده است"); return false } return true }

function getSelectedItems(a) { if (isItemSelected(a)) { window.returnValue = a.getSelectedItems(); closeform() } }

function getSelectedItem(a) { if (isItemSelected(a)) { return a.getSelectedItems() } return null }

function isNumberKey(a) { var b = a.which ? a.which : event.keyCode; if (b > 31 && (b < 48 || b > 57)) { return false } return true }

function showdialog(a, b, c, d, e) { e = e ? "yes" : "no"; if (window.navigator.appVersion.split(";")[1].split(" ")[2] == "6.0") { c = c + 60; d = d + 20 } return window.showModalDialog(a, b, "dialogHeight:" + c + "px;dialogWidth:" + d + "px;help:off;center:yes;resizeable:no;status:no;scroll:" + e + ";") }

function confirmBox(a, b) { if (isItemSelected(a)) { return confirm(b) } return false }

function filterGrid(a, b, c) { a.Filter("DataItem.GetMember('" + b + "').Value.indexOf('" + c + "') >= 0"); a.render() }

function filterGridBy(a, b, c) { if (c != "") { a.Filter("DataItem.getMember('" + b + "').get_value() == " + c); a.render(); a.unSelectAll() } }

function showUserslist() { return showdialog("../Lists/UsersList.aspx", null, 465, 480, false) }

function showAreasList() { return showdialog("../Lists/AreasList.aspx", null, 460, 480, false) }

function showSchoolsList() { return showdialog("../Lists/SchoolsList.aspx", null, 620, 580, false) }

function showAreaSchoolsList(a) { return showdialog("../Lists/SchoolsList.aspx?ac=" + a, null, 620, 580, false) }

function showProvincesList() { return showdialog("../Lists/ProvincesList.aspx", null, 520, 350, false) }

function showCitiesList(a) { return showdialog("../Lists/CitiesList.aspx?pId=" + a, null, 480, 400, false) }

function showCityPartsList(a) { return showdialog("../Lists/CityPartsList.aspx?cId=" + a, null, 520, 350, false) }

function showGoodsList(a) { return showdialog("../Lists/GoodsList.aspx?ck=" + a, null, 525, 480, false) }

function showRepositoryGoodsList(a) { return showdialog("../Lists/GoodsList.aspx?rId=" + a, null, 525, 480, false) }

function goodTypeList(a) { return showdialog("../Lists/GoodTypesList.aspx", null, 500, 550, false) }

function showCustomersList() { return showdialog("../Lists/CustomersList.aspx", null, 470, 380, false) }

function showSchoolKindsList() { return showdialog("../Lists/SchoolKindsList.aspx", null, 520, 370, false) }

function showLevelsList() { return showdialog("../Lists/LevelsList.aspx", null, 520, 370, false) }

function showSubLevelsList(a) { return showdialog("../Lists/SubLevelsList.aspx?LevCode=" + a, null, 520, 370, false) }

function showUnitsList() { return showdialog("../Lists/UnitsList.aspx", null, 520, 370, false) }

function showRepositoryTypesList() { return showdialog("../Lists/RepositoryTypesList.aspx", null, 500, 420, false) }

function showRepositoriesList() { return showdialog("../Lists/ReporitoriesList.aspx", null, 500, 550, false) }

function showReceptsList(a) { return showdialog("../Lists/ReceptsList.aspx?type=" + a, null, 500, 530, false) }

function closeform() { window.open("", "_self"); window.close(); return false }

function getQueryString(a, b) { var c = new Array, d = new Array; a = a.substring(1, a.length); a = a.split("&"); for (i = 0; i < a.length; i++) { d[i] = a[i].split("=")[0]; c[i] = a[i].split("=")[1] } for (i = 0; i < d.length; i++) { if (d[i] == b) { return c[i] } } return null } var codeNotFound = "کد مورد نظر یافت نشد"; var deleteConfirmationMessagse = "آیا رکورد انتخاب شده حذف گردد؟";

function objectPosition(obj) { var curleft = 0; var curtop = 0; if (obj.offsetParent) { do { curleft += obj.offsetLeft; curtop += obj.offsetTop; } while (obj = obj.offsetParent); } return [curleft, curtop]; }
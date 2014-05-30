var editorContentHolder;
window.onload = function()
{
    editFrame.document.designMode = "on"; 
    editFrame.document.oncontextmenu = function()
    { 
        showContextMenu();
        return false;
    }
    editFrame.document.onfocusout = getValue;
}

function getValue()
{
    var eValue = editFrame.document.body.innerHTML;
    document.getElementById(editorContentHolder).value = eValue;
}

function showContextMenu()
{
    var conDiv  = document.getElementById("contextDiv");
    if(conDiv)
    {
        document.body.removeChild(conDiv);
    }
    conDiv = document.createElement("div");
    conDiv.id= "contextDiv";
    conDiv.className = "contextDivStyle";
    editFrame.focus();
    var isDisabled = "";
    var selectedRegion = editFrame.document.selection.createRange().text;
    if(selectedRegion == "")
        isDisabled = "disabled";
    var innerStr = "<table width='100%' bgcolor='#fffcd1' cellpadding='0' cellspacing='0'>";
    innerStr += "<tr><td><img src='editorimages/Cut.gif' align='absmiddle' /></td><td align='left'><input type='button' class='contextItemStyle' value='Cut' onclick=\"doAction('Cut');hideContextDiv()\" " + isDisabled + "></td></tr>";
    innerStr += "<tr><td><img src='editorimages/Copy.gif' align='absmiddle' /></td><td align='left'><input type='button' class='contextItemStyle' value='Copy' onclick=\"doAction('Copy');hideContextDiv()\" " + isDisabled + "></td></tr>";
    innerStr += "<tr><td><img src='editorimages/Paste.gif' align='absmiddle' /></td><td align='left'><input type='button' class='contextItemStyle' value='Paste' onclick=\"doAction('Paste');hideContextDiv()\"></td></tr>";
    innerStr += "<tr><td><img src='editorimages/SelectAll.gif' align='absmiddle' /></td><td align='left'><input type='button' class='contextItemStyle' value='Select All' onclick=\"doAction('SelectAll');hideContextDiv()\"></td></tr>";
    innerStr += "<tr><td height='2' colspan='2'><hr size='1' disabled></td></tr>";
    innerStr += "<tr><td><img src='editorimages/Delete2.gif' align='absmiddle' /></td><td align='left'><input type='button' class='contextItemStyle' value='Delete' onclick=\"doAction('Delete');hideContextDiv()\" " + isDisabled + "></td></tr>";
    innerStr += "<tr><td><img src='editorimages/RemoveFormat.gif' align='absmiddle' /></td><td align='left'><input type='button' class='contextItemStyle' value='Remove Format' onclick=\"doAction('RemoveFormat');hideContextDiv()\" " + isDisabled + "></td></tr>";
    innerStr += "</table>";
    conDiv.innerHTML = innerStr;
    document.body.appendChild(conDiv);
    conDiv.style.display = "block";
    conDiv.style.top = editFrame.window.event.clientY;
    conDiv.style.left = editFrame.window.event.clientX;
    editFrame.document.body.onclick=hideContextDiv;
}

function doContextAction(ctrlid)
{
    document.getElementById(ctrlid).click();
}

function hideContextDiv()
{
    var conDiv = document.getElementById("contextDiv");
    if(conDiv)
        conDiv.style.display = "none";
}

var popDivId;
var htmlContent;
var indentValue = 30;
var colorArray = new Array(8);
colorArray[0] = new Array("#cc66cc","#cc99cc","#cccccc","#ccffcc","#ccff33","#cccc33","#000000","#cc3333");
colorArray[1] = new Array("#006699","#009999","#00cc99","#00ff99","#00ff00","#00cc00","#009900","#006600");
colorArray[2] = new Array("#ff00ff","#ff33ff","#ff66ff","#ff99ff","#ffccff","#ffffff","#ffff66","#ffcc66");
colorArray[3] = new Array("#0066cc","#0099cc","#00cccc","#00ffcc","#00ff33","#00cc33","#009933","#006633");
colorArray[4] = new Array("#40e0d0","#556b2f","#708090","#00008b","#191970","#4b0082","#8b008b","#b8860b");
colorArray[5] = new Array("#98fb98","#7888f4","#7fffd4","#87cefa","#dda0dd","#ffc0cb","#f5deb3","#ffefd3");
colorArray[6] = new Array("#ba55d3","#fa8072","#e0ffff","#cd5c5c","#dc143c","#ffe4c4","#f8f8ff","#a9a9a9");
colorArray[7] = new Array("#8a2be2","#9932cc","#bc8f8f","#6495ed","#48d1cc","#2e8b57","#5f9ea0","#afeeee");

function showColorDiv(colorTo)
{
    popDivId = document.getElementById('popDiv');
    if(popDivId)
    {
        document.body.removeChild(popDivId);
    }
    popDivId = document.createElement('div');
    popDivId.id = "popDiv";
    var tableStr = "<table cellspacing='0' bgcolor='#fffcd1' cellpadding='0' width='120' height='120'>";
    tableStr += "<tr><td colspan='8' align='right' style='padding-bottom:2px; padding-right:2px; padding-top:2px;'><img style='cursor:hand' src='../App_Themes/Default/images/editorimages/close.gif' onclick=\"popDivId.style.display='none'\" /></td></tr>";
    for(i=0;i<8;i++)
    {   
        tableStr += "<tr>";
        var padLeft = " padding-left:2px";
        for(j=0;j<8;j++)
        {
            tableStr += "<td align='center' style='padding-bottom:2px; padding-right:2px;" + padLeft + "'><input type='button' class='colortd' id='td" + i + "_" + j + "' onclick=setColor('"+ colorTo +"',"+ i + "," + j + ") style='width:15px; height:15px; background-color:"+ colorArray[i][j] +";' /></td>";
            padLeft = "";
        }
        tableStr += "</tr>";
    }
    tableStr += "</table>";
    popDivId.className = "innerDivStyle";
    popDivId.style.top = window.event.clientY+10;
    popDivId.style.left = window.event.clientX-10;
    popDivId.style.position = "absolute";
    popDivId.innerHTML = tableStr;
    document.body.appendChild(popDivId);
    editFrame.document.body.onclick=hidePopupDiv;
}

function hidePopupDiv()
{
   var conDiv = document.getElementById("popDiv");
    if(conDiv)
        conDiv.style.display = "none";  
}

function doActionWithValues(command,value1,value2)
{
    editFrame.focus();
    editFrame.document.execCommand(command,value1,value2);
}

function setColor(colorTo,x,y)
{
    editFrame.focus();
    editFrame.document.execCommand(colorTo,true, colorArray[x][y]);
    popDivId.style.display = "none";
}

function showFontDiv()
{
    popDivId = document.getElementById('popDiv');
    if(popDivId)
    {
        document.body.removeChild(popDivId);
    }
    popDivId = document.createElement('div');
    popDivId.id = "popDiv";
    var tablestr = "<table width='125' bgcolor='#fffcd1'>";
    tablestr += "<tr><td align='right' style='padding-bottom:2px; padding-right:2px; padding-top:2px;'><img style='cursor:hand' src='../App_Themes/Default/images/editorimages/close.gif' onclick=\"popDivId.style.display='none'\" /></td></tr>";
    tablestr += "<tr><td><input type='button' class='arialtd' onclick=\"setFont('Arial')\" value='Arial' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='tmrtd' onclick=\"setFont('Times New Roman')\" value='Times New Roman' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='verdanatd' onclick=\"setFont('Verdana')\" value='Verdana' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='comictd' onclick=\"setFont('Comic Sans MS')\" value='Comic Sans MS' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='cnewtd' onclick=\"setFont('Courier New')\" value='Courier New' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='impacttd' onclick=\"setFont('Impact')\" value='Impact' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='geotd' onclick=\"setFont('Georgia')\" value='Georgia' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='tahomatd' onclick=\"setFont('Tahoma')\" value='Tahoma' /></td></tr>";
    
    tablestr += "</table>";
    popDivId.className = "innerDivStyle";
    popDivId.style.top = window.event.clientY+10;
    popDivId.style.left = window.event.clientX-10;
    popDivId.style.display="block";
    popDivId.style.position = "absolute";
    popDivId.innerHTML = tablestr;
    document.body.appendChild(popDivId);
    editFrame.document.body.onclick=hidePopupDiv;
}

function setFont(fontName)
{
    editFrame.focus();
    editFrame.document.execCommand("FontName", false, fontName);
    popDivId.style.display = "none";
}

function showFontSizeDiv()
{
    popDivId = document.getElementById("popDiv");
    if(popDivId)
    {
       document.body.removeChild(popDivId);
    }
    popDivId = document.createElement('div');
    popDivId.id = "popDiv";
    var tablestr = "<table width='150' bgcolor='#fffcd1'>";
    tablestr += "<tr><td align='right'><img style='cursor:hand' src='../App_Themes/Default/images/editorimages/close.gif' onclick=\"popDivId.style.display='none'\" /></td></tr>";
    tablestr += "<tr><td><input type='button' class='smallertd' onclick=\"setFontSize('1')\" value='Smaller' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='smalltd' onclick=\"setFontSize('2')\" value='Small' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='mediumtd' onclick=\"setFontSize('3')\" value='Medium' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='largetd' onclick=\"setFontSize('4')\" value='Large' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='largertd' onclick=\"setFontSize('5')\" value='Larger' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='xlargetd' onclick=\"setFontSize('6')\" value='X-Large' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='xxlargetd' onclick=\"setFontSize('7')\" value='XX-Large' /></td></tr>";
    tablestr += "</table>";
    popDivId.className = "innerDivStyle";
    popDivId.style.top = window.event.clientY+10;
    popDivId.style.left = window.event.clientX-10;
    popDivId.style.display="block";
    popDivId.style.position = "absolute";
    popDivId.innerHTML = tablestr;
    document.body.appendChild(popDivId);
    editFrame.document.body.onclick=hidePopupDiv;
}

function setFontSize(fontSize)
{
    editFrame.focus();
    editFrame.document.execCommand("FontSize", false, fontSize);
    popDivId.style.display = "none";
}

function showHeadingDiv()
{
   popDivId = document.getElementById("popDiv");
    if(popDivId)
    {
       document.body.removeChild(popDivId);
    }
    popDivId = document.createElement('div');
    popDivId.id = "popDiv";
    var tablestr = "<table width='90' bgcolor='#fffcd1'>";
    tablestr += "<tr><td align='right'><img style='cursor:hand' src='../App_Themes/Default/images/editorimages/close.gif' onclick=\"popDivId.style.display='none'\" /></td></tr>";
    tablestr += "<tr><td><input type='button' class='smalltd' onclick=\"setHeading('Heading 1')\" value='Heading 1' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='smalltd' onclick=\"setHeading('Heading 2')\" value='Heading 2' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='smalltd' onclick=\"setHeading('Heading 3')\" value='Heading 3' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='smalltd' onclick=\"setHeading('Heading 4')\" value='Heading 4' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='smalltd' onclick=\"setHeading('Heading 5')\" value='Heading 5' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='smalltd' onclick=\"setHeading('Heading 6')\" value='Heading 6' /></td></tr>";
    tablestr += "</table>";
    popDivId.className = "innerDivStyle";
    popDivId.style.top = window.event.clientY+10;
    popDivId.style.left = window.event.clientX-10;
    popDivId.style.display="block";
    popDivId.style.position = "absolute";
    popDivId.innerHTML = tablestr;
    document.body.appendChild(popDivId);
    editFrame.document.body.onclick=hidePopupDiv; 
}

function setHeading(htype)
{
    editFrame.focus();
    editFrame.document.execCommand("FormatBlock", false, htype);
    popDivId.style.display = "none";
}


function showDateDiv()
{
    popDivId = document.getElementById("popDiv");
    if(popDivId)
    {
       document.body.removeChild(popDivId);
    }
    popDivId = document.createElement('div');
    popDivId.id = "popDiv";
    var tablestr = "<table width='150' bgcolor='#fffcd1'>";
    tablestr += "<tr><td align='right'><img style='cursor:hand' src='../App_Themes/Default/images/editorimages/close.gif' onclick=\"popDivId.style.display='none'\" /></td></tr>";
    tablestr += "<tr><td><input type='button' class='itemStyle' onclick=\"setDate('1')\" value='dd/mm/yyyy' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='itemStyle' onclick=\"setDate('2')\" value='mm/dd/yyyy' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='itemStyle' onclick=\"setDate('3')\" value='MMMM dd, yyyy' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='itemStyle' onclick=\"setDate('4')\" value='ddd MMMM dd, yyyy' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='itemStyle' onclick=\"setDate('5')\" value='dddd MMMM dd, yyyy' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='itemStyle' onclick=\"setDate('6')\" value='ddd MMM dd, yyyy' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='itemStyle' onclick=\"setDate('7')\" value='MMMM dd' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='itemStyle' onclick=\"setDate('8')\" value='dd MMMM' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='itemStyle' onclick=\"setDate('9')\" value='MMMM, yyyy' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='itemStyle' onclick=\"setDate('10')\" value='yyyy-MM-dd' /></td></tr>";
    tablestr += "</table>";
    popDivId.className = "innerDivStyle";
    popDivId.style.top = window.event.clientY+10;
    popDivId.style.left = window.event.clientX-10;
    popDivId.style.display="block";
    popDivId.style.position = "absolute";
    popDivId.innerHTML = tablestr;
    document.body.appendChild(popDivId);
    editFrame.document.body.onclick=hidePopupDiv;
}

function setDate(val)
{
    editFrame.focus();
    var selectedRegion = editFrame.document.selection.createRange();
    var now = new Date();
    var dateArr = now.toString().split(" ");
    var weekday = dateArr[0];
    var weekdayStr;
    switch(weekday)
    {
        case "Sun": weekdayStr = "Sunday"; break;
        case "Mon": weekdayStr = "Monday"; break;
        case "Tue": weekdayStr = "Tuesday"; break;
        case "Wed": weekdayStr = "Wednesday"; break;
        case "Thu": weekdayStr = "Thursday"; break;
        case "Fri": weekdayStr = "Friday"; break;
        case "Sat": weekdayStr = "Saturday"; break;
    }
    var month = dateArr[1];
    var monthStr;
    switch(month)
    {
        case "Jan": monthStr="January"; break;
        case "Feb": monthStr="February"; break;
        case "Mar": monthStr="March"; break;
        case "Apr": monthStr="April"; break;
        case "May": monthStr="May"; break;
        case "Jun": monthStr="June"; break;
        case "Jul": monthStr="July"; break;
        case "Aug": monthStr="August"; break;
        case "Sep": monthStr="September"; break;
        case "Oct": monthStr="October"; break;
        case "Nov": monthStr="November"; break;
        case "Dec": monthStr="December"; break;
    }
    var monthNum = now.getMonth() + 1;
    var monthNumStr =monthNum;
    if(parseInt(monthNum) < 10)
        monthNumStr = "0" + monthNum;
    var dateStr = dateArr[2];
    if(parseInt(dateStr) <10)
        dateStr = "0" + dateStr;
    var yearStr = dateArr[5];
    var resultStr="";
    switch(parseInt(val))
    {
        case 1:
            resultStr = dateStr + "/" + monthNumStr + "/" + yearStr;        
            break;
        case 2:
            resultStr = monthNumStr + "/" + dateStr + "/" + yearStr; 
            break;
        case 3:
            resultStr = monthStr + " " + dateStr + ", " + yearStr;
            break;
        case 4:
            resultStr = weekday + " " + monthStr + " " + dateStr + ", " + yearStr;
            break;
        case 5:
            resultStr = weekdayStr + " " + monthStr + " " + dateStr + ", " + yearStr;
            break;
        case 6:
            resultStr = weekday + " " + month + " " + dateStr + ", " + yearStr;
            break;
        case 7:
            resultStr = monthStr + " " + dateStr;
            break;
        case 8:
            resultStr = dateStr + " " + monthStr;
            break;
        case 9:
            resultStr = monthStr + ", " + yearStr;
            break;
        case 10:
             resultStr = yearStr + "-" + monthNumStr + "-" + dateStr;        
            break;
    }
    selectedRegion.pasteHTML(resultStr);
    popDivId.style.display = "none";
}

function showTimeDiv()
{
    popDivId = document.getElementById("popDiv");
    if(popDivId)
    {
       document.body.removeChild(popDivId);
    }
    popDivId = document.createElement('div');
    popDivId.id = "popDiv";
    var tablestr = "<table width='150' bgcolor='#fffcd1'>";
    tablestr += "<tr><td align='right'><img style='cursor:hand' src='../App_Themes/Default/images/editorimages/close.gif' onclick=\"popDivId.style.display='none'\" /></td></tr>";
    tablestr += "<tr><td><input type='button' class='itemStyle' onclick=\"setTime('1')\" value='hh:mm tt' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='itemStyle' onclick=\"setTime('2')\" value='hh:mm:ss tt' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='itemStyle' onclick=\"setTime('3')\" value='HH:mm' /></td></tr>";
    tablestr += "<tr><td><input type='button' class='itemStyle' onclick=\"setTime('4')\" value='HH:mm:ss' /></td></tr>";
    tablestr += "</table>";
    popDivId.className = "innerDivStyle";
    popDivId.style.top = window.event.clientY+10;
    popDivId.style.left = window.event.clientX-10;
    popDivId.style.display="block";
    popDivId.style.position = "absolute";
    popDivId.innerHTML = tablestr;
    document.body.appendChild(popDivId);
    editFrame.document.body.onclick=hidePopupDiv;
}

function setTime(val)
{
    editFrame.focus();
    var selectedRegion = editFrame.document.selection.createRange();
    var now = new Date();
    var dateArr = now.toString().split(" ");
    var timeStr = dateArr[3].split(":");
    var hourStr = timeStr[0];
    var hourStr12 = hourStr;
    var hourInt = parseInt(hourStr);
    var apm = "AM";
    if(hourInt >= 12)
    {
        apm = "PM";
        if(hourInt >12)
        {
            hourInt = hourInt - 12;
            hourStr = hourInt.toString();
            if(hourInt < 10)
                hourStr = "0" + hourStr; 
        }
    }
    var minStr = timeStr[1];
    var secStr = timeStr[2];
    var resultStr="";
    switch(parseInt(val))
    {
        case 1:
            resultStr = hourStr + ":" + minStr + " " + apm;        
            break;
        case 2:
            resultStr = hourStr + ":" + minStr + ":" + secStr + " " + apm;
            break;
        case 3:
            resultStr = hourStr12 + ":" + minStr;  
            break;
        case 4:
            resultStr = hourStr12 + ":" + minStr + ":" + secStr;
            break;
    }
    selectedRegion.pasteHTML(resultStr);
    popDivId.style.display = "none";
}

function doAction(action)
{
    editFrame.focus();
    if(action == "ClearAll")
    {
        editFrame.document.execCommand("SelectAll",true);
        action = "Delete";
    }
    editFrame.document.execCommand(action,true);
} 

function showDialog(link,w, h)//380, 125
{
    if(link == "li")
        link = "InsertLinks.htm";
    else if(link == "sc")
        link = "SpecialChars.htm";
    var returnedTxt= showModalDialog(link,"", "dialogWidth:"+ w +"px; dialogHeight:"+ h +"px; status:no; center:yes");
    editFrame.focus();
    if(returnedTxt)
    {
        var theRange = editFrame.document.selection.createRange();
        theRange.pasteHTML(returnedTxt);
    }
}

var cWnd;
function showInsertPopup(opt, w, h) // w=350, h=340
{
    var srcfile = "imgWindow.aspx";
    if(opt == "im")
        srcfile = "imgWindow.aspx";
    else if(opt == "fl")
        srcfile = "flashWindow.aspx";
    else if(opt == "me")
        srcfile = "mediaWindow.aspx";
    else if(opt == "tm")
        srcfile = "templateWindow.aspx";
    else if(opt == "st")
        srcfile = "saveWindow.aspx";
    else if(opt == "tb")
        srcfile = "InsertTable.htm";
    var left = (window.screen.width - parseInt(w))/2;
    var top = (window.screen.height - parseInt(h))/2;
    if(cWnd)
        cWnd.close();
    cWnd = window.open(srcfile,"", "width="+ w +",height="+ h +",location=no, status=no, top="+ top +", left="+ left);
}

function doHtmlAction(atype)
{
    editFrame.focus();
    var selectedRegion = editFrame.document.selection.createRange();
    if(atype == "lower")
        selectedRegion.text = selectedRegion.text.toLowerCase();
    else if(atype == "upper")
        selectedRegion.text = selectedRegion.text.toUpperCase();
}

function setZoom(zoomSize)
{
    var zoomValue = editFrame.document.body.style.zoom;
    if(zoomValue)
    {
        var zoomInt = parseInt(zoomValue);
        if(zoomSize == "p")
        {
            if(zoomInt <= 4)
                editFrame.document.body.style.zoom = zoomInt + 1;
        }
        if(zoomSize == "m")
        {
            if(zoomInt >= 1)
            editFrame.document.body.style.zoom = zoomInt - 1;
        }
    }
    else
    {
        editFrame.document.body.style.zoom = 2;
    }
}

function showPreview(atype)
{
    htmlContent = editFrame.document.body.innerHTML;
    if(atype == "s")
    {
        document.getElementById('textContent').value = htmlContent;
        document.getElementById('frameDiv').style.display = "none";
        document.getElementById('txtDiv').style.display = "block";
        document.getElementById('btnDesign').removeAttribute("disabled");
        document.getElementById('btnOutput').removeAttribute("disabled");
        document.getElementById('btnPreview').disabled = "disabled";
    }
    else if(atype == "d")
    {
        var txtcontent = document.getElementById("textContent")
        if(txtcontent)
        {   
            htmlContent = txtcontent.value;      
            document.getElementById('frameDiv').style.display = "block";
            document.getElementById('txtDiv').style.display = "none";
            document.getElementById('btnDesign').disabled = "disabled";
            document.getElementById('btnOutput').removeAttribute("disabled");
            document.getElementById('btnPreview').removeAttribute("disabled");
            editFrame.document.body.innerHTML = htmlContent;
        }
    }
    else if(atype == "o")
    {
        var win = window.open("","");
        win.document.write(htmlContent);
    }
}


function setText(iframe, txtId)
{
    iframe.contentWindow.document.body.innerHTML = document.getElementById(txtId).value;
    editorContentHolder = txtId;
}
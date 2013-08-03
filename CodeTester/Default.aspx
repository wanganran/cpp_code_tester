<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CodeTester._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <link href="Style.css" type="text/css" rel="Stylesheet" />
    <title>C/Cpp Code Tester</title>
    <script type="text/javascript" language="javascript">
    function insertAtCursor(obj, txt) {
  obj.focus();
  //IE support
  if (document.selection) {
    sel = document.selection.createRange();
    sel.text = txt;
  }
  //MOZILLA/NETSCAPE support
  else {
    var startPos = obj.selectionStart;
    var scrollTop = obj.scrollTop;
    var endPos = obj.selectionEnd;
    obj.value = obj.value.substring(0, startPos) + txt + obj.value.substring(endPos, obj.value.length);
    startPos += txt.length;
    obj.setSelectionRange(startPos, startPos);
    obj.scrollTop = scrollTop;
  }
}
function getCaretPos(ctrl) {
    var caretPos = 0;
    if (document.selection) {
    // IE Support
    var range = document.selection.createRange();
    // We'll use this as a 'dummy'
    var stored_range = range.duplicate();
    // Select all text
    stored_range.moveToElementText( ctrl );
    // Now move 'dummy' end point to end point of original range
    stored_range.setEndPoint( 'EndToEnd', range );
    // Now we can calculate start and end points
    ctrl.selectionStart = stored_range.text.length - range.text.length;
    ctrl.selectionEnd = ctrl.selectionStart + range.text.length;
    caretPos = ctrl.selectionStart;
    } else if (ctrl.selectionStart || ctrl.selectionStart == '0')
    // Firefox support
        caretPos = ctrl.selectionStart;
    return (caretPos);
}
function getCurrentLineBlanks(obj) {
  var pos = getCaretPos(obj);
  var str = obj.value;
  var i = pos-1;
  while (i>=0) {
    if (str.charAt(i) == '\n')
      break;
    i--;
  }
  i++;
  var blanks = "";
  while (i < str.length) {
    var c = str.charAt(i);
    if (c == ' ' || c == '\t')
      blanks += c;
    else
      break;
    i++;
  }
  return blanks;
}
function load(){
document.getElementById('code').onkeydown=function(eve)
{
      if (eve.target != this) return;
      if (eve.keyCode == 13)
        last_blanks = getCurrentLineBlanks(this);
      else if (eve.keyCode == 9) {
        eve.preventDefault();
        insertAtCursor(this, "  ");
        this.returnValue = false;
      }
}
document.getElementById('code').onkeyup=function(eve){
      if (eve.target == this && eve.keyCode == 13)
          insertAtCursor(this, last_blanks);
          }
          }
          
    </script>
    
</head>
<body onload="load()">
    <form id="form1" action="result.aspx" method="post">
    <div>
        <h1><span>C/Cpp</span> Code Tester</h1>
        <div style="width:100%">
        <div id="left">
            <h2>Code language:</h2>
            <select id="lang" name="lang" size="2" enableviewstate="true">
            <option>C</option>
            <option selected="selected">Cpp</option>
            </select>
            <h2>Time limit</h2>
            <select id="timelimit" name="timelimit" size="3" enableviewstate="true">
            <option>0.2s</option>
            <option selected="selected">1s</option>
            <option>5s</option>
            </select>
        </div>
        <div id="main">
            <h2>Enter code here:</h2>
            <textarea cols="*" rows="15" name="code" id="code" enableviewstate="true">#include &lt;cstdio&gt;
using namespace std;
int main()
{
    printf("hello world");
    return 0;
}</textarea>
            <h2>Input data:</h2>
            <textarea rows="5" name="input" enableviewstate="true"></textarea>
            <input type="submit" value="Run" />
            
        </div>
        </div>
    </div>
    </form>
</body>
</html>

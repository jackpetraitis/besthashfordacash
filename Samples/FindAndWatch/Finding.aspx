<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Finding.aspx.cs" Inherits="Example._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Finding</title>
    <link href="style.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="msg" runat="server" Text="Keyword:"></asp:Label>
        <asp:TextBox ID="keyword" runat="server">ipod</asp:TextBox>
        <asp:Button ID="findItem" runat="server" Text="Query" onclick="findItem_Click" />
    </div>
    </form>
    <div>
        <asp:Table ID="result" runat="server">
        </asp:Table>
        
    </div>
    <asp:Label ID="errorMsg" runat="server" Text=""></asp:Label>
</body>
</html>

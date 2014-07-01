<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddToWatchList.aspx.cs" Inherits="Example.AddToWatchList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Add To Watch</title>
    <link href="style.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <asp:Label ID="watchCount" runat="server" Text=""></asp:Label>    
    <div>
        <asp:Table ID="watchList" runat="server">
        </asp:Table>
    </div>
    <asp:Label ID="errorMsg" runat="server" Text=""></asp:Label>
    
</body>
</html>

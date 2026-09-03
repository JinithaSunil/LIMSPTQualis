<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewPO.aspx.cs" Inherits="LIMS.ViewPO" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Button ID="btnExportToPDF" runat="server" Text="Export to PDF" OnClick="btnExportToPDF_Click" Visible="false" />
        <asp:Button ID="btnExportToExcel" runat="server" Text="Export Report" OnClick="btnExportToExcel_Click" Visible="false" />
        <div id="divPO" runat="server">
        </div>
    </form>
</body>
</html>

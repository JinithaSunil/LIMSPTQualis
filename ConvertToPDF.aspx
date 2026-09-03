<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConvertToPDF.aspx.cs" Inherits="LIMS.ConvertToPDF" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Button ID="btnExport" runat="server" Text="Export" OnClick="ExportToPDF" />
        </div>
    </form>
</body>
</html>

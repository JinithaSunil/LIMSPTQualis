<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HTMLToPDF.aspx.cs" Async="true" Inherits="LIMS.HTMLToPDF" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>HTML to PDF using PuppeteerSharp</title>

    <style>
        body {
            font-family: Arial;
            margin:40px;
        }

        h1{
            color:#0066cc;
        }

        table{
            border-collapse:collapse;
            width:100%;
        }

        table td,table th{
            border:1px solid #000;
            padding:8px;
        }

        th{
            background:#efefef;
        }
    </style>

</head>
<body>
<form id="form1" runat="server">
    <asp:Button ID="btnGenerate" runat="server" Text="PDF" OnClick="btnGenerate_Click" />
    <div id="divPO" runat="server">
    </div>
</form>
</body>
</html>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestReportFood.aspx.cs" Inherits="LIMS.TestReportFood" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        body{
            font-family: Calibri;
            font-size:12px;
        }
        @media print {
            #noprint {
                visibility: hidden;
            }
        }

        #pageFooter {
            display: table-footer-group;
            float: right;
            margin-right:5%;
        }

            #pageFooter:after {
                counter-increment: page;
                content: counter(page) " of " counter(page);    
                
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="divTestResult" runat="server">
        </div>
        <div id="noprint">
            <asp:Button ID="btnExportToExcel" runat="server" Text="Export Report" OnClick="btnExportToExcel_Click" />
        </div>
        <div id="pageFooter">Page </div>
    </form>
</body>
</html>

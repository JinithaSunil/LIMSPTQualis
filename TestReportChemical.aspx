<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestReportChemical.aspx.cs" Inherits="LIMS.TestReportChemical" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        @media print {
            #noprint {
                visibility: hidden;
            }
        }

        #pageFooter {
            display: table-footer-group;
            float:right;
            margin-right:5%;
        }

            #pageFooter:after {
                counter-increment: page;
                content: counter(page) " of "  counter(page);      
                
            }

    </style>
</head>
<body>
    <form id="form1" runat="server">
          <div id="pageFooter">Page </div>
        <div id="divTestResult" runat="server">
        </div>
        <div id="noprint">
            <asp:Button ID="btnExportToExcel" runat="server" Text="Export Report" OnClick="btnExportToExcel_Click" />
        </div>
      
    </form>
</body>
</html>

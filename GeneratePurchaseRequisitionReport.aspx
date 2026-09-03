<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GeneratePurchaseRequisitionReport.aspx.cs" Inherits="LIMS.GeneratePurchaseRequisitionReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        @font-face {
            font-family: 'SignatureFont';
            src: url('assets/fonts/GreatVibes-Regular.ttf') format('truetype');
        }

        .signature-font {
            font-family: 'SignatureFont', cursive;
        }

        .signature-image {
            max-width: 90%;
            max-height: 65px;
            width: auto;
            height: auto;
            object-fit: contain;
        }
    </style>
    <style>
        .strikeRow {
            text-decoration: line-through;
            color: red; /* optional */
            opacity: 0.7; /* optional faded look */
        }

        .a4-page {
            min-height: 26cm;
            margin: auto;
            padding: 20mm;
            background: white; /* The page color */
            width: 26cm; /* A4 width */
            display: block;
            margin: 0 auto; /* Center the page horizontally */
            margin-bottom: 0.5cm; /* Space between multiple pages in a long scroll */
            box-shadow: 0 0 0.5cm rgba(0, 0, 0, 0.5); /* Adds a subtle shadow for depth */
            box-sizing: border-box; /* Ensures padding/border are included in the specified width/height */
            padding: 1cm 1.5cm; /* Example internal margins/padding for content */
        }

        @media print {
            .a4-page {
                background: white; /* The page color */
                width: 21cm; /* A4 width */
                display: block;
                margin: 0 auto; /* Center the page horizontally */
                margin-bottom: 0.5cm; /* Space between multiple pages in a long scroll */
                box-shadow: none !important; /* Adds a subtle shadow for depth */
                box-sizing: border-box; /* Ensures padding/border are included in the specified width/height */
                padding: 1cm 1.5cm; /* Example internal margins/padding for content */
            }

            .no-print {
                display: none;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="divPR" runat="server">
        </div>
    </form>

</body>
</html>

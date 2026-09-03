<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewDocuments.aspx.cs" Inherits="LIMS.ViewDocuments" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="css/CRF_style.css" media="all" />
    <style>
        @media print {
             .a4-page {
                page-break-after: always;
                box-shadow: none;
            }


            .no-print {
                display: none;
            }

            body {
                margin: 0;
            }
        }
         .a4-page {
            background: white;
            width: 23cm;
            display: block;
            margin: 0 auto;
            margin-bottom: 0.5cm;
            box-shadow: 0 0 0.5cm rgba(0, 0, 0, 0.5);
            box-sizing: border-box;
            padding: 1cm 1.5cm;
            position: relative;
            min-height: 29.7cm;
            padding-bottom: 3.5cm;
        }

        .table {
            border-collapse: collapse !important;
        }
        @font-face {
	font-family: 'Broadway BT';
	font-style: normal;
	font-weight: normal;
	src: local('Broadway BT'), url('assets/fonts/Broadway.woff') format('woff');
}

    </style>
</head>
<body>
   <div class="no-print" >
    <button onclick="window.print()" style="color: #b34788">Print <i class="fa-solid fa-print"></i></button>
</div>
    <div id="divDocument" runat="server" style="background-color: white">

        <%--<iframe runat="server" id="document" frameborder="0" height="1000" width="100%"></iframe>--%>
    </div>
</body>
</html>

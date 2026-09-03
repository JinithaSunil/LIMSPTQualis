<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewQuotation.aspx.cs" Inherits="LIMS.ViewQuotation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
        <style>
    .a4-page {
        background: white; /* The page color */
        width: 21cm; /* A4 width */
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
<body style="background-color: white">
    <form id="form1" runat="server">
       <div class="no-print" >
    <button onclick="window.print()" style="color: #b34788">Print <i class="fa-solid fa-print"></i></button>
</div>
                 <div id="divQuotation" runat="server">
        </div>
               
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="generatePurchaseInvoice.aspx.cs" Inherits="LIMS.generatePurchaseInvoice" %>

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
     .footer-table td {
         border: none !important; /* remove borders */
         text-align: right; /* right align text */
         padding: 6px 10px;        
         font-weight: bold;
     }
 </style>
</head>
<body>
    <form id="form1" runat="server">
         <div id="divVoucher" runat="server">
    </div>
    </form>
</body>
</html>

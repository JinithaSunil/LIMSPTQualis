<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="Viewer.aspx.cs" Inherits="LIMS.Viewer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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

}
        </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:Button ID="btnPrint" runat="server" Text="Print" OnClick="btnPrint_Click" />

<%--<asp:Button ID="btnPDF" runat="server" Text="Download PDF" OnClientClick="downloadPDF(); return false;" />--%>
     
<iframe id="frameHtml" runat="server" width="100%" height="600px">

</iframe>
         
<script src="https://cdnjs.cloudflare.com/ajax/libs/html2pdf.js/0.9.2/html2pdf.bundle.min.js"></script>

<script>
    function printIframe() {
        var iframe = document.getElementById("frameHtml");
        iframe.contentWindow.focus();
        iframe.contentWindow.print();
    }

    function downloadPDF() {
        var iframe = document.getElementById("frameHtml");
        var element = iframe.contentDocument.body;

        html2pdf().from(element).save("Report.pdf");
    }
</script>

</asp:Content>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SendMail.aspx.cs" Inherits="LIMS.SendMail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="LocalHostSMTP" runat="server" OnClick="LocalHostSMTP_Click" Text="Send"/>
    <asp:Button ID="btnSendEnquiryReceived" runat="server" Text="EnquiryReceived" OnClick="btnSendEnquiryReceived_Click" />
        <asp:Button ID="btnSendCRFSent" runat="server" Text="CRFReceived" OnClick="btnSendCRFSent_Click" />
        <asp:Label ID="lblMsg" runat="server"></asp:Label>
    </div>
    </form>
</body>
</html>

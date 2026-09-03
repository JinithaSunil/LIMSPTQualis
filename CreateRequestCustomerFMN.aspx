<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="CreateRequestCustomerFMN.aspx.cs" Inherits="LIMS.CreateRequestCustomerFMN" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <title>Test Applcation Form</title>
</head>
<body>
    <div id="divRequest" runat="server">
    </div>
<%--    <table style='width: 100%' class='tab1'>
        <thead>
            <tr>
                <td><b>Sl.<br>
                    No.</b></td>
                <td><b>Test Parameters Required for<br />
                    Analysis / Quotation<br />
                    Reference</b></td>
                <td><b>Test Sub Parameters</b></td>
                <td><b>Test Methods</b></td>
                <td><b>*Any<br />
                    Specification<br />
                    to be<br />
                    mentioned?</b></td>
            </tr>
        </thead>
        <asp:Repeater ID="rptParent" runat="server" OnItemDataBound="rptParent_ItemDataBound">

            <ItemTemplate>
                <tr style="text-align: left">
                    <td>
                        <asp:Label ID="lblTestParameter" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "TestParameter") %>'></asp:Label>
                        <asp:Repeater ID="rptChild" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblTestParameter" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "TestSubParameter") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblTestSubParameter" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "TestMethod") %>'></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblTestMethod" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Specification") %>'></asp:Label>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </td>
                    <td>
                        <asp:Label ID="lblTestMethod" runat="server" Text='<%# Eval("TestMethod") %>'></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Specification") %>'></asp:Label>
                    </td>
                </tr>
            </ItemTemplate>

        </asp:Repeater>

    </table>--%>
</body>
</html>



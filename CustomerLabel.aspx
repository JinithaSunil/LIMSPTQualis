<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomerLabel.aspx.cs" Inherits="LIMS.CustomerLabel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        @media print {
            .no-print {
                display: none;
            }

            body {
                margin: 0;
            }
        }

        .page {
            width: 210mm;
            padding: 10mm;
            display: grid;
            grid-template-columns: 1fr 1fr;
            grid-template-rows: auto auto;
            gap: 20px;
        }

        .label {
             border: 1px solid #ddd;
    border-top: 4px solid #b02871; /* colored top line */
    padding: 15px;
    box-shadow: 0 0 5px #ccc;
    font-family: Arial;
    height: 180px;
        }

        .header-line {
            height: 4px;
            background: #b02871;
            margin-bottom: 10px;
        }

        .bold {
            font-weight: bold;
            font-size: 16px;
        }

        .row {
            margin-bottom: 5px;
            font-size: 13px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
		<div class="no-print" >
    <button onclick="window.print()" style="color: #b34788">Print <i class="fa-solid fa-print"></i></button>
</div>
            <div class="page">
                <asp:Repeater ID="rptPage" runat="server">
                    <ItemTemplate>

                        <div class="labels-container">

                            <div class="label">
                                <div class="bold"><%# Eval("companyname") %></div>
                                <br />
                                <hr />
                                <div class="row"><b>Invoice#:</b> <%# Eval("InvoiceNumber") %></div>
                                <div class="row"><b>Contact:</b> <%# Eval("ContactPerson") %></div>
                                <div class="row"><b>Mobile:</b> <%# Eval("Mobile") %></div>
                                <div class="row"><b>Email:</b> <%# Eval("Email") %></div>
                                <div class="row"><b>Address:</b> <%# Eval("Address") %></div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

            </div>
    </form>
</body>
</html>

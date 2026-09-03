<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="K3LForElectricandElectronicProduct.aspx.cs" Inherits="LIMS.K3LForElectricandElectronicProduct" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Test Results Form</title>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" rel="stylesheet" />

    <style type="text/css">
        body {
            font-family: "Calibri", "Segoe UI", sans-serif;
            /*font-size: 14px;*/
        }

        .form-container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
        }

        .clause-table {
            border-collapse: collapse;
            width: 100%;
            margin-bottom: 20px;
            border: 2px solid #333;
        }

        .header-row {
            color: black;
            font-weight: bold;
        }

        .sub-header-row {
            background-color: #d1d0d0;
            font-weight: bold;
        }

        .clause-table td, .clause-table th {
            border: 1px solid black;
            padding: 8px;
            text-align: left;
            vertical-align: top;
        }

        .clause-number {
            width: 10%;
            font-weight: bold;
        }

        .requirement {
            width: 45%;
        }

        .result {
            width: 25%;
        }

        .remark {
            width: 20%;
        }

        /*.checkbox-group label {
            display: block;
            margin-bottom: 5px;
        }*/

        .checkbox-group {
            display: flex;
            flex-direction: column;
            gap: 10px; /* spacing between rows */
        }

        .checkbox-row {
            display: flex;
            gap: 20px; /* spacing between items in a row */
        }

        .checkbox-item {
            display: flex;
            align-items: center;
            white-space: nowrap;
        }

        .checkbox-item {
            display: flex;
            align-items: center;
            white-space: nowrap;
            font-size: 18px; /* Increase font size */
        }

            .checkbox-item input[type="checkbox"] {
                width: 20px; /* Increase checkbox width */
                height: 20px; /* Increase checkbox height */
                margin-right: 8px; /* Space between checkbox and label */
            }

        .btn-container {
            text-align: center;
            margin-top: 20px;
        }

        .btn {
            padding: 10px 20px;
            margin: 5px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
        }

        .btn-save {
            /*background-color: #4CAF50;*/
            background-color: #b34788 !important;
            color: white;
        }



        .btn-report {
            background-color: #b34788 !important;
            color: black;
        }

        .btn:hover {
            opacity: 0.8;
        }



        .table-responsive {
            width: 100%;
            overflow-x: auto;
            margin-bottom: 20px;
        }

        table {
            width: 100%;
            min-width: 800px; /* More room for fields */
            border-collapse: collapse;
            background-color: #fff;
        }

        table, th, td {
            border: 2px solid #333;
            text-align: left;
        }

        td {
            padding: 8px 10px;
            text-align: left;
            vertical-align: middle;
            text-align: left;
        }

        .info-table td:first-child,
        .info-table td:nth-child(3) {
            width: 20%;
        }

        input[type="text"], input[type="number"], input[type="date"],
        .form-control {
            width: 100%;
            max-width: 100%;
            padding: 6px 8px;
            border: 1px solid #ccc;
            border-radius: 4px;
            display: block;
            box-sizing: border-box;
        }

        .center-text {
            text-align: center;
        }

        .form-actions {
            margin-top: 20px;
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <div class="form-container">

            <div class="header">
                <h2>Test Result Entry Form </h2>
                <asp:Label ID="lblstandardID" runat="server" Visible="false"></asp:Label>
                <asp:Label ID="lblReportID" runat="server" Visible="false"></asp:Label>
                <asp:Label ID="lblSampleID" runat="server" Visible="false"></asp:Label>
            </div>
            <div class="table-responsive">
                <table class="info-table">
                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Work Order</td>
                        <td>
                            <asp:TextBox ID="txtworkorder" runat="server" CssClass="form-control" />
                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Product</td>
                        <td>
                            <asp:TextBox ID="txtproduct" runat="server" CssClass="form-control" />
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Brand</td>
                        <td>
                            <asp:TextBox ID="txtbrand" runat="server" CssClass="form-control" />
                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Model</td>
                        <td>
                            <asp:TextBox ID="txtmodel" runat="server" CssClass="form-control" />
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Standard</td>
                        <td>
                            <asp:TextBox ID="txtstandard" runat="server" CssClass="form-control" />
                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Test Method No.</td>
                        <td>
                            <asp:TextBox ID="txttestMethodNo" runat="server" CssClass="form-control" />
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Temp</td>
                        <td>
                            <asp:TextBox ID="txttemp" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvTemp" runat="server"
                                ControlToValidate="txttemp" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Temperature is required"
                                Display="Dynamic"
                                ForeColor="Red"
                                Text="Required" />
                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Humidity (%)</td>
                        <td>
                            <asp:TextBox ID="txthumidity" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvHumidity" runat="server"
                                ControlToValidate="txthumidity" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Humidity is required"
                                Display="Dynamic"
                                ForeColor="Red"
                                Text="Required" />
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Test Starting Date</td>
                        <td>
                            <asp:TextBox ID="txtstartingdate" runat="server" CssClass="form-control" TextMode="Date" />
                            <asp:RequiredFieldValidator ID="rfvStartingDate" runat="server"
                                ControlToValidate="txtstartingdate" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Starting Date is required"
                                Display="Dynamic"
                                ForeColor="Red"
                                Text="Required" />
                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Test Completion Date</td>
                        <td>
                            <asp:TextBox ID="txtcompletiondate" runat="server" CssClass="form-control" TextMode="Date" />
                            <asp:RequiredFieldValidator ID="rfvCompletionDate" runat="server"
                                ControlToValidate="txtcompletiondate" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Completion Date is required"
                                Display="Dynamic"
                                ForeColor="Red"
                                Text="Required" />
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Equipment</td>
                        <td colspan="3">
                            <asp:CheckBoxList ID="chkEquipmentList" runat="server" RepeatColumns="1" RepeatDirection="Horizontal">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                </table>
            </div>
<div>

</div>
            
            <br />
            <br />
            <br />
            <div id="clauseContainer" runat="server" style="margin-bottom: 100px;"></div>
            <hr />
            <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                   <%-- <h2><u>TABLE: Power Input Deviation</u></h2>--%>
                    <div class="table-responsive">
                        <table class="table table-bordered">
                            <thead>
                                <tr>
                                    <th>Name Of Goods</th>
                                    <th>Item Description</th>
                                    <th>Type of Testing</th>
                                    <th>Condition</th>
                                    <th>TestMethod</th>
                                    <th>Result</th>
                               

                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtnameofgoods" runat="server" CssClass="form-control" placeholder="Input deviation from/on" />
                                        <asp:RequiredFieldValidator ID="rfvtxtnameofgoods" runat="server"
                                            ControlToValidate="txtnameofgoods" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtitemdescription" runat="server" CssClass="form-control" placeholder="Item Description" />
                                        <asp:RequiredFieldValidator ID="rfvitemdescription" runat="server"
                                            ControlToValidate="txtitemdescription" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txttypeoftesting" runat="server" CssClass="form-control" placeholder="TYPES OF TESTING" />
                                        <asp:RequiredFieldValidator ID="rfvtypeoftesting" runat="server"
                                            ControlToValidate="txttypeoftesting" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>

                                    <td>
                                        <asp:TextBox ID="txtCondition" runat="server" CssClass="form-control" placeholder="Condition" />
                                        <asp:RequiredFieldValidator ID="rfvCondition" runat="server"
                                            ControlToValidate="txtCondition" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txttestmethod" runat="server" CssClass="form-control" placeholder="TestMethod" />
                                        <asp:RequiredFieldValidator ID="rfvtestmethod" runat="server"
                                            ControlToValidate="txttestmethod" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtresult" runat="server" CssClass="form-control" placeholder="Result" />
                                        <asp:RequiredFieldValidator ID="rfvresult" runat="server"
                                            ControlToValidate="txtresult" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>

                                    <td>
                                        <asp:Button ID="btnAdd" runat="server" CssClass="btn btn-primary" Text="ADD"
                                            Style="background-color: #2196F3;" OnClick="btnAdd_Click"
                                            CausesValidation="true" />
                                    </td>
                                </tr>

                            </tbody>
                        </table>
                    </div>
                    <hr />
                  <asp:GridView ID="gvTests" runat="server" AutoGenerateColumns="False" OnRowDeleting="gvTests_RowDeleting">
    <Columns>
        <asp:TemplateField HeaderText="No.">
            <ItemTemplate>
                <%# (gvTests.PageIndex * gvTests.PageSize) + Container.DataItemIndex + 1 %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField DataField="NameofGoods" HeaderText="Name of Goods" />
        <asp:BoundField DataField="ItemDescription" HeaderText="Item Description" />
        <asp:BoundField DataField="TypeOfTesting" HeaderText="Type of Testing" />
        <asp:BoundField DataField="Condition" HeaderText="Condition" />
        <asp:BoundField DataField="TestMethod" HeaderText="Test Method" />
        <asp:BoundField DataField="Results" HeaderText="Results" />

        <asp:TemplateField>
            <ItemTemplate>
                <asp:LinkButton ID="lnkDelete" runat="server" Style="color: red;" CommandName="Delete"
                    OnClientClick="return confirm('Are you sure you want to delete this item?');"
                    CssClass="text-danger" ToolTip="Delete">
                    <i class="fas fa-trash-alt"></i>
                </asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>




                </ContentTemplate>
            </asp:UpdatePanel>
 
   
            <div class="btn-container">
                <asp:Button ID="btnSave" runat="server" Text="Save Results" OnClick="btnSave_Click" CssClass="btn btn-save" CausesValidation="true" ValidationGroup="DefaultDataUpperDetails" />
                <%--<asp:Button ID="btnGenerateReport" runat="server" Text="Generate Report" OnClick="btnExport_Click" CssClass="btn btn-report" Visible="false" />--%>
            </div>

        </div>

    </form>
 
        

   
</body>
</html>

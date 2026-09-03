<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StainlessSteelFlatWear.aspx.cs" Inherits="LIMS.StainlessSteelFlatWear" %>

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
                <h2>Test Result Entry Form :
                    <asp:Label ID="lblReportName" runat="server"></asp:Label>
                </h2>
                <asp:Label ID="lblstandardID" runat="server" Visible="false"></asp:Label>
                <asp:Label ID="lblReportID" runat="server" Visible="false"></asp:Label>
                <asp:Label ID="lblSampleID" runat="server" Visible="false"></asp:Label>
            </div>
            <div class="table-responsive">
                <table class="info-table">

                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Usage groups</td>
                        <td>
                            <asp:TextBox ID="txtusageofgroups" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server"
                                ControlToValidate="txtusageofgroups" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Usage groups  required" Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>

                        <td style="background-color: #d1d0d0; font-weight: bold;">Model</td>
                        <td>
                            <asp:TextBox ID="txtmodel" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server"
                                ControlToValidate="txtmodel" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Model/Type is required" Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>
                    </tr>


                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Type Of Products</td>
                        <td>
                            <asp:TextBox ID="txtTypeOfProducts_header" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server"
                                ControlToValidate="txtTypeOfProducts_header" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Type Of Products Date is required" Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>

                        <td style="background-color: #d1d0d0; font-weight: bold;">Standard no.</td>
                        <td>
                            <asp:TextBox ID="txtstandard" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server"
                                ControlToValidate="txtstandard" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Starting Date is required" Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>
                    </tr>

                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Brand</td>
                        <td>
                            <asp:TextBox ID="txtBrand" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server"
                                ControlToValidate="txtBrand" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Starting Date is required" Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Job Order No.</td>
                        <td>
                            <asp:TextBox ID="txtworkorder" runat="server" CssClass="form-control" />
                        </td>
                    </tr>

                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Test Date</td>
                        <td>
                            <asp:TextBox ID="txtstartingdate" runat="server" CssClass="form-control" TextMode="Date" />
                            <asp:RequiredFieldValidator ID="rfvStartingDate" runat="server"
                                ControlToValidate="txtstartingdate" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Starting Date is required" Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>

                        <td style="background-color: #d1d0d0; font-weight: bold;">Test Completion Date</td>
                        <td>
                            <asp:TextBox ID="txtcompletiondate" runat="server" CssClass="form-control" TextMode="Date" />
                            <asp:RequiredFieldValidator ID="rfvCompletionDate" runat="server"
                                ControlToValidate="txtcompletiondate" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Completion Date is required"
                                Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>
                    </tr>


                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Sample Code</td>
                        <td>
                            <asp:TextBox ID="txtsampleCode" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server"
                                ControlToValidate="txtsampleCode" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test sample code is required" Display="Dynamic" ForeColor="Red" Text="Required" />

                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Test Equipment No.</td>
                        <td>
                            <asp:TextBox ID="txtTestEquipmentNo" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server"
                                ControlToValidate="txtTestEquipmentNo" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test EquipmentNo  is required" Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>
                    </tr>



                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Test Method </td>

                    </tr>


                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Test Category </td>
                        <td>
                            <asp:TextBox ID="txtTestCategory" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server"
                                ControlToValidate="txtTestCategory" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Method is required" Display="Dynamic" ForeColor="Red" Text="Required" />

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
              <table class="table table-bordered" style="margin-bottom: 0;">
    <tr>
        <td style="width: 50%;">
            <table style="width: 100%; border: none;">
                <tr>
                    <td style="border: none; width: 40%;">Test method No.</td>
                    <td style="border: none;">
                        : <asp:TextBox ID="txttestmethod" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server"
                            ControlToValidate="txttestmethod" ValidationGroup="DefaultDataUpperDetails"
                            ErrorMessage="Test Method is required" Display="Dynamic" ForeColor="Red" Text="Required" />
                    </td>
                </tr>
                <tr>
                    <td style="border: none;">Serial No.</td>
                    <td style="border: none;">
                        : <asp:TextBox ID="txtSerialNo" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvSerialNo" runat="server"
                            ControlToValidate="txtSerialNo"
                            ErrorMessage="Required" ForeColor="Red" Display="Dynamic"
                            ValidationGroup="EquipmentDetails" />
                    </td>
                </tr>
                <tr>
                    <td style="border: none;">Room Temp.</td>
                    <td style="border: none;">
                        : <asp:TextBox ID="txttemp" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvRoomTempHumidity" runat="server"
                            ControlToValidate="txttemp"
                            ErrorMessage="Required" ForeColor="Red" Display="Dynamic"
                            ValidationGroup="EquipmentDetails" />
                    </td>
                </tr>
                <tr>
                    <td style="border: none;">Humidity</td>
                    <td style="border: none;">
                        : <asp:TextBox ID="txthumidity" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator15" runat="server"
                            ControlToValidate="txthumidity"
                            ErrorMessage="Required" ForeColor="Red" Display="Dynamic"
                            ValidationGroup="EquipmentDetails" />
                    </td>
                </tr>
                <tr>
                    <td style="border: none;">Place / Date testing</td>
                    <td style="border: none;">
                        : Qualis 1, <asp:TextBox ID="txtPlaceDateTesting" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvPlaceDateTesting" runat="server"
                            ControlToValidate="txtPlaceDateTesting"
                            ErrorMessage="Required" ForeColor="Red" Display="Dynamic"
                            ValidationGroup="EquipmentDetails" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
                <br />
                <!-- Footer Row: Examiner / Checked / Approval / Decision -->
                <table class="table table-bordered" style="margin-top: 0;">
                    <tr style="height: 80px; vertical-align: bottom;">
                        <td style="width: 25%;">Examiner:
            <asp:TextBox ID="txtExaminer" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvExaminer" runat="server"
                                ControlToValidate="txtExaminer"
                                ErrorMessage="Required" ForeColor="Red" Display="Dynamic"
                                ValidationGroup="EquipmentDetails">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 25%;">Checked:
            <asp:TextBox ID="txtChecked" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvChecked" runat="server"
                                ControlToValidate="txtChecked"
                                ErrorMessage="Required" ForeColor="Red" Display="Dynamic"
                                ValidationGroup="EquipmentDetails">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 25%;">Approval:
            <asp:TextBox ID="txtApproval" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvApproval" runat="server"
                                ControlToValidate="txtApproval"
                                ErrorMessage="Required" ForeColor="Red" Display="Dynamic"
                                ValidationGroup="EquipmentDetails">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 25%; vertical-align: top;">
                            <b>Decision:</b>
                            <asp:TextBox ID="txtDecisionFinal" runat="server" CssClass="form-control" />
                        </td>
                    </tr>
                </table>

            </div>
            <br />
            <br />
            <br />



            <div id="clauseContainer" runat="server" style="margin-bottom: 100px;"></div>


            <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <h2><u>MEASUREMENT RESULTS TABLE</u></h2>

                    <!-- Section Header matching image -->
                    <table class="table table-bordered" style="margin-bottom: 0;">
                        <tr style="background-color: #d9d9d9;">
                            <td style="width: 60px;"><b>8.2</b></td>
                            <td colspan="4"><b>Table: Product Thickness Measurement</b></td>
                        </tr>
                        <tr style="background-color: #d9d9d9;">
                            <td><b>8.2.1</b></td>
                            <td colspan="4"><b>Tableware</b></td>
                        </tr>
                    </table>

                    <div class="table-responsive">
                        <table>
                            <thead>
                                <td>Group</td>
                                <td>Types of products</td>
                                <td>Measurement results (mm)</td>
                                <td>Min thickness requirement (mm)</td>
                                <td>Decision</td>
                            </thead>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtGroup" runat="server" CssClass="form-control" placeholder="Group" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ValidationGroup="ObjectDetails"
                                        ControlToValidate="txtGroup" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </td>

                                <td>
                                    <asp:TextBox ID="txtTypeOfProducts" runat="server" CssClass="form-control" placeholder="Types of Products" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ValidationGroup="ObjectDetails"
                                        ControlToValidate="txtTypeOfProducts" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </td>

                                <td>
                                    <asp:TextBox ID="txtMeasurementResults" runat="server" CssClass="form-control" placeholder="Measurement Results (mm)" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ValidationGroup="ObjectDetails"
                                        ControlToValidate="txtMeasurementResults" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </td>

                                <td>
                                    <asp:TextBox ID="txtMinThicknessRequirement" runat="server" CssClass="form-control" placeholder="Min Thickness Requirement (mm)" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ValidationGroup="ObjectDetails"
                                        ControlToValidate="txtMinThicknessRequirement" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </td>

                                <td>
                                    <asp:TextBox ID="txtDecision" runat="server" CssClass="form-control" placeholder="Decision" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ValidationGroup="ObjectDetails"
                                        ControlToValidate="txtDecision" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </td>

                                <td>
                                    <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-primary" Text="ADD"
                                        Style="background-color: #2196F3;" OnClick="btnAdd_Click"
                                        CausesValidation="true" ValidationGroup="ObjectDetails" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <hr />

                    <asp:GridView ID="gvTests" runat="server" AutoGenerateColumns="False" OnRowDeleting="gvTests_RowDeleting"
                        CssClass="table table-bordered" Style="margin-bottom: 0;">
                        <Columns>
                            <asp:TemplateField HeaderText="No.">
                                <ItemTemplate>
                                    <%# (gvTests.PageIndex * gvTests.PageSize) + Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="Group" HeaderText="Group" />
                            <asp:BoundField DataField="TypeOfProducts" HeaderText="Types of Products" />
                            <asp:BoundField DataField="MeasurementResults" HeaderText="Measurement Results (mm)" />
                            <asp:BoundField DataField="MinThicknessRequirement" HeaderText="Min Thickness Requirement (mm)" />
                            <asp:BoundField DataField="Decision" HeaderText="Decision" />

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

                    <!-- Additional Information row matching image -->
                    <table class="table table-bordered" style="margin-top: 0;">
                        <tr>
                            <td><b>Additional information:</b></td>
                            <td>
                             <asp:TextBox ID="txtAdditionalinfo" runat="server" CssClass="form-control" placeholder="Additional information:" />
 <asp:RequiredFieldValidator ID="RequiredFieldValidator16" runat="server" ValidationGroup="ObjectDetails"
     ControlToValidate="txtAdditionalinfo" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
 </asp:RequiredFieldValidator>
                                </td>
                        </tr>
                    </table>

                </ContentTemplate>
            </asp:UpdatePanel>

           <%-------------------------------------------------------------------- Table2--------------------------------------------------------------------------%>

            
      
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <h2><u>MEASUREMENT RESULTS TABLE</u></h2>

                    <!-- Section Header matching image -->
                    <table class="table table-bordered" style="margin-bottom: 0;">
                        <tr style="background-color: #d9d9d9;">
                            <td style="width: 60px;"><b>8.2</b></td>
                            <td colspan="4"><b>Table: Product Thickness Measurement</b></td>
                        </tr>
                        <tr style="background-color: #d9d9d9;">
                            <td><b>8.2.1</b></td>
                            <td colspan="4"><b>Tableware</b></td>
                        </tr>
                    </table>

                    <div class="table-responsive">
                        <table>
                            <thead>
                                <td>Group</td>
                                <td>Types of products</td>
                                <td>Measurement results (mm)</td>
                                <td>Min thickness requirement (mm)</td>
                                <td>Decision</td>
                            </thead>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtGroupcookingequi" runat="server" CssClass="form-control" placeholder="Group" />
                                  <asp:RequiredFieldValidator ID="RequiredFieldValidator17" runat="server" ValidationGroup="ObjectDetailscookingequ"
                                        ControlToValidate="txtGroupcookingequi" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </td>

                                <td>
                                    <asp:TextBox ID="txtTypeOfProductscookingequi" runat="server" CssClass="form-control" placeholder="Types of Products" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator18" runat="server" ValidationGroup="ObjectDetailscookingequ"
                                        ControlToValidate="txtTypeOfProductscookingequi" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </td>

                                <td>
                                    <asp:TextBox ID="txtMeasurementResultscookingequi" runat="server" CssClass="form-control" placeholder="Measurement Results (mm)" />
                                   <asp:RequiredFieldValidator ID="RequiredFieldValidator19" runat="server" ValidationGroup="ObjectDetailscookingequ"
                                        ControlToValidate="txtMeasurementResultscookingequi" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </td>

                                <td>
                                    <asp:TextBox ID="txtMinThicknessRequirementcookingequi" runat="server" CssClass="form-control" placeholder="Min Thickness Requirement (mm)" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator20" runat="server" ValidationGroup="ObjectDetailscookingequ"
                                        ControlToValidate="txtMinThicknessRequirementcookingequi" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </td>

                                <td>
                                    <asp:TextBox ID="txtDecisioncookingequi" runat="server" CssClass="form-control" placeholder="Decision" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator21" runat="server" ValidationGroup="ObjectDetailscookingequ"
                                        ControlToValidate="txtDecisioncookingequi" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </td>

                                <td>
                                    <asp:Button ID="btnsubmitcookingequi" runat="server" CssClass="btn btn-primary" Text="ADD"
                                        Style="background-color: #2196F3;" OnClick="btnAddcookingequ_Click"
                                        CausesValidation="true" ValidationGroup="ObjectDetailscookingequ" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <hr />

                    <asp:GridView ID="gvcooking" runat="server" AutoGenerateColumns="False" OnRowDeleting="gvcooking_RowDeleting"
                        CssClass="table table-bordered" Style="margin-bottom: 0;">
                        <Columns>
                            <asp:TemplateField HeaderText="No.">
                                <ItemTemplate>
                                    <%# (gvTests.PageIndex * gvTests.PageSize) + Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="Group_cookingequ" HeaderText="Group" />
                            <asp:BoundField DataField="TypeOfProducts_cookingequ" HeaderText="Types of Products" />
                            <asp:BoundField DataField="MeasurementResults_cookingequ" HeaderText="Measurement Results (mm)" />
                            <asp:BoundField DataField="MinThicknessRequirement_cookingequ" HeaderText="Min Thickness Requirement (mm)" />
                            <asp:BoundField DataField="Decision_cookingequ" HeaderText="Decision" />

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

                    <!-- Additional Information row matching image -->
                    <table class="table table-bordered" style="margin-top: 0;">
                        <tr>
                            <td><b>Additional information:</b></td>
                            <td>
                             <asp:TextBox ID="txtAdditionalinfocookingequi" runat="server" CssClass="form-control" placeholder="Additional information:" />
 <asp:RequiredFieldValidator ID="RequiredFieldValidator22" runat="server" ValidationGroup="ObjectDetails"
     ControlToValidate="txtAdditionalinfo" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
 </asp:RequiredFieldValidator>
                                </td>
                        </tr>
                    </table>

                </ContentTemplate>
            </asp:UpdatePanel>

            <div class="btn-container">
                <asp:Button ID="btnSave" runat="server" Text="Save Results" OnClick="btnSave_Click" CssClass="btn btn-save" CausesValidation="true" ValidationGroup="DefaultDataUpperDetails" />
            </div>

        </div>

    </form>
</body>
</html>

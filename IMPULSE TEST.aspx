<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="IMPULSE TEST.aspx.cs" Inherits="LIMS.IMPULSE_TEST" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">


    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 20px;
            background-color: #f5f5f5;
        }

        .card {
            background: white;
            border-radius: 3px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            margin-bottom: 30px;
        }

        .violet {
            border: 0;
            height: 4px;
            background-color: #b34788;
            margin: 0;
        }

        .header {
            padding: 15px 20px;
            border-bottom: 1px solid #e0e0e0;
        }

            .header h2 {
                margin: 0;
                color: #333;
                font-size: 18px;
                font-weight: bold;
            }

        .body {
            padding: 20px;
        }

        .table-responsive {
            overflow-x: auto;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            background-color: white;
        }

        table, th, td {
            border: 1px solid #ddd;
        }

        th {
            background-color: #f2f2f2;
            color: #333;
            padding: 10px;
            text-align: center;
            font-weight: bold;
        }

        td {
            padding: 8px;
            text-align: center;
        }

        .info-table td:first-child {
            background-color: #f2f2f2;
            width: 25%;
            font-weight: bold;
        }

        .test-table th {
            background-color: #e0e0e0;
            vertical-align: middle;
        }

        input[type="text"], input[type="number"], input[type="date"] {
            width: 90%;
            padding: 6px;
            border: 1px solid #ddd;
            border-radius: 3px;
        }

        .btn-primary {
            background-color: #b34788;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 3px;
            cursor: pointer;
            font-weight: bold;
        }

            .btn-primary:hover {
                background-color: #933375;
            }

        .form-actions {
            margin-top: 20px;
            text-align: center;
        }

        /* Make header cells and input aligned */
        .tests-section th, .test-row td {
            vertical-align: middle;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="updt" runat="server">
        <ContentTemplate>
            <div class="row clearfix">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="card">
                        <hr class="violet">
                        <div class="header">
                            <h2>VOLTAGE 4 HOURS</h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td><b>Type</b></td>
                                        <td>
                                            <asp:TextBox ID="txtcableType" runat="server" CssClass="form-control" /></td>
                                        <%--<asp:RequiredFieldValidator ID="rfvCableType" runat="server" ControlToValidate="txtcableType" ErrorMessage="Type is required" CssClass="text-danger" Display="Dynamic" />--%>
                                        <td><b>Ref.Spec.</b></td>
                                        <td>
                                            <asp:TextBox ID="txtrefSpec" runat="server" CssClass="form-control" /></td>
                                        <%-- <asp:RequiredFieldValidator ID="rfvRefSpec" runat="server" ControlToValidate="txtrefSpec" ErrorMessage="Ref.Spec. is required" CssClass="text-danger" Display="Dynamic" />--%>
                                    </tr>
                                    <tr>
                                        <td><b>Size</b></td>
                                        <td>
                                            <asp:TextBox ID="txtcableSize" runat="server" CssClass="form-control" /></td>
                                        <%-- <asp:RequiredFieldValidator ID="rfvCableSize" runat="server" ControlToValidate="txtcableSize" ErrorMessage="Size is required" CssClass="text-danger" Display="Dynamic" />--%>
                                        <td><b>Test Method No.</b></td>
                                        <td>
                                            <asp:TextBox ID="txttestMethodNo" runat="server" CssClass="form-control" /></td>
                                        <%-- <asp:RequiredFieldValidator ID="rfvTestMethodNo" runat="server" ControlToValidate="txttestMethodNo" ErrorMessage="Test Method No. is required" CssClass="text-danger" Display="Dynamic" />--%>
                                    </tr>
                                    <tr>
                                        <td><b>Voltage</b></td>
                                        <td>
                                            <asp:TextBox ID="voltage" runat="server" CssClass="form-control" /></td>
                                        <%-- <asp:RequiredFieldValidator ID="rfvVoltage" runat="server" ControlToValidate="voltage" ErrorMessage="Voltage is required" CssClass="text-danger" Display="Dynamic" />--%>
                                        <td><b>Job Order No.</b></td>
                                        <td>
                                            <asp:TextBox ID="txtjobOrderNo" runat="server" CssClass="form-control" /></td>
                                        <%-- <asp:RequiredFieldValidator ID="rfvJobOrderNo" runat="server" ControlToValidate="txtjobOrderNo" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />--%>
                                    </tr>
                                    <tr>
                                        <td><b>Date</b></td>
                                        <td>
                                            <asp:TextBox ID="txtdate" runat="server" CssClass="form-control" TextMode="Date" /></td>
                                        <%--<asp:RequiredFieldValidator ID="rfvDate" runat="server" ControlToValidate="txtdate" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />--%>
                                        <td><b>Work Sheet No.</b></td>
                                        <td>
                                            <asp:TextBox ID="txtWorksheetNo" runat="server" CssClass="form-control" /></td>
                                        <%--   <asp:RequiredFieldValidator ID="rfvWorksheetNo" runat="server" ControlToValidate="txtWorksheetNo" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />--%>
                                    </tr>
                                    <tr>
                                        <td><b>Room No. </b></td>
                                        <td>
                                            <asp:TextBox ID="txtRoomNo" runat="server" CssClass="form-control" /></td>
                                        <%-- <asp:RequiredFieldValidator ID="rfvRoomNo" runat="server" ControlToValidate="txtRoomNo" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />--%>
                                        <td><b>Room Humidity</b></td>
                                        <td>
                                            <asp:TextBox ID="txtRoomhumidity" runat="server" CssClass="form-control" /></td>
                                        <%--<asp:RequiredFieldValidator ID="rfvroomHumidity" runat="server" ControlToValidate="txtRoomhumidity" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />--%>
                                    </tr>
                                    <tr>
                                        <td><b>Room Temp.</b></td>
                                        <td>
                                            <asp:TextBox ID="txtRoomTemp" runat="server" CssClass="form-control" /></td>
                                        <%-- <asp:RequiredFieldValidator ID="rfvJRoomTemp" runat="server" ControlToValidate="txtRoomTemp" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />--%>
                                        <td><b>Equipment No.</b></td>
                                        <td>
                                            <asp:TextBox ID="txtEquipment" runat="server" CssClass="form-control" /></td>
                                        <%--   <asp:RequiredFieldValidator ID="rfvEquipment" runat="server" ControlToValidate="txtEquipment" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />--%>
                                    </tr>
                                </table>
                            </div>

                            <div class="table-responsive tests-section" style="margin-top: 20px;">
                                <table class="table table-bordered">

                                    <%--   <caption>
                                        <asp:DropDownList ID="ddlType" runat="server" CssClass="form-control">
                                            <asp:ListItem>Select </asp:ListItem>
                                            <asp:ListItem>10 Positive Impluse Test followed by a Voltage test</asp:ListItem>
                                            <asp:ListItem>10 Negative Impluse Test followed by a Voltage test</asp:ListItem>
                                        </asp:DropDownList></caption>--%>
                                    <thead>
                                        <tr>
                                            <th>Item Test</th>
                                            <th>Unit</th>
                                            <th>Specified</th>
                                            <th>Measured</th>
                                            <th>Remark</th>
                                        </tr>
                                    </thead>

                                    <tr>
                                        <td>Impulse test at Withstand Voltage</td>
                                        <td>
                                            <asp:TextBox ID="txtUnit_Imp" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtSpecified_Imp" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtMeasured_Imp" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtRemark_Imp" runat="server"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td>Peak Voltage</td>
                                        <td>
                                            <asp:TextBox ID="txtUnit_Peak" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtSpecified_Peak" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtMeasured_Peak" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtRemark_Peak" runat="server"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td>Front wave time</td>
                                        <td>
                                            <asp:TextBox ID="txtUnit_Front" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtSpecified_Front" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtMeasured_Front" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtRemark_Front" runat="server"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td>Tail wave time</td>
                                        <td>
                                            <asp:TextBox ID="txtUnit_Tail" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtSpecified_Tail" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtMeasured_Tail" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtRemark_Tail" runat="server"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td>Conductor temperature</td>
                                        <td>
                                            <asp:TextBox ID="txtUnit_Con" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtSpecified_Con" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtMeasured_Con" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtRemark_Con" runat="server"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td>Test Result</td>
                                        <td>
                                            <asp:TextBox ID="txtUnit_TResult" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtSpecified_TResult" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtMeasured_TResult" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtRemark_TResult" runat="server"></asp:TextBox></td>
                                    </tr>
                                </table>

                                <%--                                <asp:Button ID="btnAdd" runat="server" Text="+" OnClick="btnAdd_Click" />--%>
                                <div class="form-actions">
                                    <asp:Button ID="btnAdd" runat="server" CssClass="btn btn-success " Text="+" OnClick="btnAdd_Click" />
                                </div>

                                <%--  <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" OnRowCommand="GridView1_RowCommand">
                                    <Columns>

                                        <asp:BoundField DataField="ItemTest" HeaderText="Item Test" />
                                        <asp:BoundField DataField="Unit" HeaderText="Unit" />
                                        <asp:BoundField DataField="Specified" HeaderText="Specified" />
                                        <asp:BoundField DataField="Measured" HeaderText="Measured" />
                                        <asp:BoundField DataField="Remark" HeaderText="Remark" />
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandName="EditRow" CommandArgument='<%# Container.DataItemIndex %>' />

                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>--%>



                                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" OnRowCommand="GridView1_RowCommand">
                                    <Columns>

                                        <asp:BoundField DataField="ItemTest" HeaderText="Item Test" ReadOnly="true" />
                                        <asp:BoundField DataField="Unit" HeaderText="Unit" ReadOnly="true" />

                                        <asp:TemplateField HeaderText="Specified">
                                            <ItemTemplate>
                                                <%# Eval("Specified") %>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtSpecified" runat="server" Text='<%# Eval("Specified") %>'></asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Measured">
                                            <ItemTemplate>
                                                <%# Eval("Measured") %>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtMeasured" runat="server" Text='<%# Eval("Measured") %>'></asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Remark">
                                            <ItemTemplate>
                                                <%# Eval("Remark") %>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtRemark" runat="server" Text='<%# Eval("Remark") %>'></asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandName="EditRow" CommandArgument='<%# Container.DataItemIndex %>'    />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Button ID="btnUpdate" runat="server" Text="Update" CommandName="UpdateRow" CommandArgument='<%# Container.DataItemIndex %>' />
                                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CommandName="CancelEdit" />
                                            </EditItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>
                                </table>
                                      
                            </div>

                            <%--<tr>--%>

                            <%--  <td>Impulse test at Withstand Voltage</td>
                                        <td>
                                            <asp:TextBox ID="txtUnit7" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtSpecified7" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtMeasured7" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtRemark7" runat="server"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td>Peak Voltage</td>
                                        <td>
                                            <asp:TextBox ID="txtUnit8" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtSpecified8" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtMeasured8" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtRemark8" runat="server"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td>Front wave time</td>
                                        <td>
                                            <asp:TextBox ID="txtUnit9" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtSpecified9" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtMeasured9" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtRemark9" runat="server"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td>Tail wave time</td>
                                        <td>
                                            <asp:TextBox ID="txtUnit10" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtSpecified10" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtMeasured10" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtRemark10" runat="server"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td>Conductor temperature</td>
                                        <td>
                                            <asp:TextBox ID="txtUnit11" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtSpecified11" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtMeasured11" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtRemark11" runat="server"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td>Test Result</td>


                                        <td>
                                            <asp:TextBox ID="txtUnit12" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtSpecified12" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtMeasured12" runat="server"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txtRemark12" runat="server"></asp:TextBox></td>
                                    </tr>--%>
                                </table>

                            <div class="form-actions">
                                <asp:Button ID="btnCancelAll" runat="server" Text="Cancel" OnClick="btnCancelAll_Click" CssClass="btn btn-danger" />
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnSaveData" runat="server" CssClass="btn-primary" Text="Save Data" />
                            </div>
                        </div>
                    </div>
                </div>
        </ContentTemplate>

    </asp:UpdatePanel>

</asp:Content>

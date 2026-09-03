<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="DirectionOfLayandLength.aspx.cs" Inherits="LIMS.DirectionOfLay_Length" %>



<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
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
            padding: 5px;
            text-align: center;
        }

        .info-table td:first-child {
            background-color: #068001;
            width: 25%;
            font-weight: bold;
        }

        .test-table th {
            background-color: #e0e0e0;
            vertical-align: middle;
        }

        input[type="text"], input[type="number"], input[type="date"] {
            width: 100%;
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

        .invalid-input {
            border: 2px solid #ff9999 !important;
            background-color: #fff0f0 !important;
        }

        .table-bordered th,
        .table-bordered td {
            border: 1px solid #ddd;
            padding: 8px;
            text-align: center;
        }

        .table-bordered th {
            background-color: #f2f2f2;
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
                            <h2>DIRECTION OF LAY & LAY LENGTH</h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
     <tr>
         <td><b>Type</b></td>
         <td>
             <asp:TextBox ID="cableType" runat="server" CssClass="form-control" readonly="true"/>
           
         </td>
         <td><b>Ref.Spec.</b></td>
         <td>
             <asp:TextBox ID="refSpec" runat="server" CssClass="form-control" readonly="true"/>
            
         </td>
     </tr>
     <tr>
         <td><b>Size</b></td>
         <td>
             <asp:TextBox ID="cableSize" runat="server" CssClass="form-control" readonly="true"/>
           
         </td>
         <td><b>Test Method No.</b></td>
         <td>
             <asp:TextBox ID="testMethodNo" runat="server" CssClass="form-control" readonly="true"/>
           
         </td>
     </tr>
     <tr>
         <td><b>Voltage</b></td>
         <td>
             <asp:TextBox ID="voltage" runat="server" CssClass="form-control" readonly="true"/>
             
         </td>
         <td><b>Job Order No.</b></td>
         <td>
             <asp:TextBox ID="jobOrderNo" runat="server" CssClass="form-control" readonly="true"/>
          
         </td>
     </tr>
     <tr>
         <td><b>Date</b></td>
         <td>
             <asp:TextBox ID="Date" runat="server" CssClass="form-control" TextMode="Date" Style="text-align: center;" />
             <asp:RequiredFieldValidator ID="rfvDate" runat="server"
                 ControlToValidate="Date"
                 ErrorMessage="Date is required"
                 Display="Dynamic" ForeColor="Red" />
         </td>
         <td><b>Work Sheet No.</b></td>
         <td>
             <asp:TextBox ID="worksheetno" runat="server" CssClass="form-control" readonly="true"/>
            
         </td>
     </tr>
     <tr>
         <td><b>Room No.</b></td>
         <td>
             <asp:TextBox ID="roomno" runat="server" CssClass="form-control" Style="text-align: center;" readonly="true"/>
           
         </td>
         <td><b>Room Humidity (%)</b></td>
         <td>
             <asp:TextBox ID="roomhumidity" runat="server" CssClass="form-control" Style="text-align: center;" />
             <asp:RequiredFieldValidator ID="rfvRoomHumidity" runat="server"
                 ControlToValidate="roomhumidity"
                 ErrorMessage="Room Humidity is required"
                 Display="Dynamic" ForeColor="Red" />
         </td>
     </tr>
     <tr>
         <td><b>Room Temp. (°C)</b></td>
         <td>
             <asp:TextBox ID="roomtemp" runat="server" CssClass="form-control" Style="text-align: center;" />
             <asp:RequiredFieldValidator ID="rfvRoomTemp" runat="server"
                 ControlToValidate="roomtemp"
                 ErrorMessage="Room Temperature is required"
                 Display="Dynamic" ForeColor="Red" />
         </td>
         <td><b>Equipment No.</b></td>
         <td>
             <asp:TextBox ID="equipmentno" runat="server" CssClass="form-control" readonly="true"/>
          
         </td>
     </tr>
 </table>
                            </div>
                            <br /><div class="table-responsive">
    <!-- ===== TABLE 1 ===== -->
    <table class="table table-bordered text-center">
        <thead>
            <tr>
                <th>Layer:</th>
                <th colspan="7">Conductor diameter (mm)</th>
                <th colspan="3">Lay ratios of stranded conductors (x D)</th>
                <th>Direction of lay</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>Test results:</td>
                <td>
                    <asp:TextBox ID="txtCondDia1_1" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvCondDia1_1" runat="server" ControlToValidate="txtCondDia1_1"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtCondDia2_1" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvCondDia2_1" runat="server" ControlToValidate="txtCondDia2_1"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtCondDia3_1" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvCondDia3_1" runat="server" ControlToValidate="txtCondDia3_1"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtCondDia4_1" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvCondDia4_1" runat="server" ControlToValidate="txtCondDia4_1"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtCondDia5_1" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvCondDia5_1" runat="server" ControlToValidate="txtCondDia5_1"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtCondDia6_1" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvCondDia6_1" runat="server" ControlToValidate="txtCondDia6_1"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtConDiaNom_1" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvConDiaNom_1" runat="server" ControlToValidate="txtConDiaNom_1"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtLayRatioMin1_1" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvLayRatioMin1_1" runat="server" ControlToValidate="txtLayRatioMin1_1"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtLayRatioMax1_1" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvLayRatioMax1_1" runat="server" ControlToValidate="txtLayRatioMax1_1"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtLayRatioNom_1" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvLayRatioNom_1" runat="server" ControlToValidate="txtLayRatioNom_1"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtDirectionOfLay1_1" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvDirectionOfLay1_1" runat="server" ControlToValidate="txtDirectionOfLay1_1"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td>Standard requirement:</td>
                <td colspan="3">
                    <asp:TextBox ID="txtStdMin_1" runat="server" CssClass="form-control" Placeholder="Min:" />
                    <asp:RequiredFieldValidator ID="rfvStdMin_1" runat="server" ControlToValidate="txtStdMin_1"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td colspan="3">
                    <asp:TextBox ID="txtStdMax_1" runat="server" CssClass="form-control" Placeholder="Max:" />
                    <asp:RequiredFieldValidator ID="rfvStdMax_1" runat="server" ControlToValidate="txtStdMax_1"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtStdNom_1" runat="server" CssClass="form-control" Placeholder="Nom:" />
                    <asp:RequiredFieldValidator ID="rfvStdNom_1" runat="server" ControlToValidate="txtStdNom_1"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtStdLayMin_1" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvStdLayMin_1" runat="server" ControlToValidate="txtStdLayMin_1"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtStdLayMax_1" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvStdLayMax_1" runat="server" ControlToValidate="txtStdLayMax_1"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtStdLayNom_1" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvStdLayNom_1" runat="server" ControlToValidate="txtStdLayNom_1"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtStdDirectionOfLay_1" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvStdDirectionOfLay_1" runat="server" ControlToValidate="txtStdDirectionOfLay_1"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
            </tr>
        </tbody>
    </table>

    <!-- ===== TABLE 2 ===== -->
    <table class="table table-bordered text-center">
        <thead>
            <tr>
                <th>Layer:</th>
                <th colspan="7">Conductor diameter (mm)</th>
                <th colspan="3">Lay ratios of stranded conductors (x D)</th>
                <th>Direction of lay</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>Test results:</td>
                <td>
                    <asp:TextBox ID="txtCondDia1_2" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvCondDia1_2" runat="server" ControlToValidate="txtCondDia1_2"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtCondDia2_2" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvCondDia2_2" runat="server" ControlToValidate="txtCondDia2_2"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtCondDia3_2" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvCondDia3_2" runat="server" ControlToValidate="txtCondDia3_2"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtCondDia4_2" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvCondDia4_2" runat="server" ControlToValidate="txtCondDia4_2"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtCondDia5_2" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvCondDia5_2" runat="server" ControlToValidate="txtCondDia5_2"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtCondDia6_2" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvCondDia6_2" runat="server" ControlToValidate="txtCondDia6_2"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtConDiaNom_2" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvConDiaNom_2" runat="server" ControlToValidate="txtConDiaNom_2"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtLayRatioMin1_2" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvLayRatioMin1_2" runat="server" ControlToValidate="txtLayRatioMin1_2"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtLayRatioMax1_2" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvLayRatioMax1_2" runat="server" ControlToValidate="txtLayRatioMax1_2"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtLayRatioNom_2" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvLayRatioNom_2" runat="server" ControlToValidate="txtLayRatioNom_2"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtDirectionOfLay1_2" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvDirectionOfLay1_2" runat="server" ControlToValidate="txtDirectionOfLay1_2"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td>Standard requirement:</td>
                <td colspan="3">
                    <asp:TextBox ID="txtStdMin_2" runat="server" CssClass="form-control" Placeholder="Min:" />
                    <asp:RequiredFieldValidator ID="rfvStdMin_2" runat="server" ControlToValidate="txtStdMin_2"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td colspan="3">
                    <asp:TextBox ID="txtStdMax_2" runat="server" CssClass="form-control" Placeholder="Max:" />
                    <asp:RequiredFieldValidator ID="rfvStdMax_2" runat="server" ControlToValidate="txtStdMax_2"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtStdNom_2" runat="server" CssClass="form-control" Placeholder="Nom:" />
                    <asp:RequiredFieldValidator ID="rfvStdNom_2" runat="server" ControlToValidate="txtStdNom_2"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtStdLayMin_2" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvStdLayMin_2" runat="server" ControlToValidate="txtStdLayMin_2"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtStdLayMax_2" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvStdLayMax_2" runat="server" ControlToValidate="txtStdLayMax_2"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtStdLayNom_2" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvStdLayNom_2" runat="server" ControlToValidate="txtStdLayNom_2"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
                <td>
                    <asp:TextBox ID="txtStdDirectionOfLay_2" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvStdDirectionOfLay_2" runat="server" ControlToValidate="txtStdDirectionOfLay_2"
                        ErrorMessage="Value is Required" ForeColor="Red" Display="Dynamic" />
                </td>
            </tr>
        </tbody>
    </table>
</div>


                            <div>
                            </div>
                        </div>
                    </div>
                    <div class="form-actions">
                        <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" />
                        <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClear_Click" />
                        <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" Visible="false" OnClick="btnUpdate_Click" />
                    </div>
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>
</asp:Content>

<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/PTQualisMasterFull.Master" CodeBehind="CalibrationRecord.aspx.cs" Inherits="LIMS.CalibrationRecord" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
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
            background-color: #d1d0d0;
            font-weight: bold;
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

        .btn {
            background-color: #b34788 !important;
            color: #fff !important; /* white text for contrast */
            border: 1px solid #9e3c76 !important;
        }
    </style>

    <script type="text/javascript">
        window.addEventListener('DOMContentLoaded', function () {
            var inputs = document.querySelectorAll('input[type=text], textarea');
            inputs.forEach(function (input) {
                input.addEventListener('input', function () {
                    if (this.value.trim() !== "")
                        this.style.borderColor = "";
                    else
                        this.style.borderColor = "red";
                });
            });
        });
    </script>


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
                            <h2>Test Record </h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                              <table>
  <table>
    <tr>
        <td><b>Analysis Starting Date</b></td>
        <td>
            <asp:TextBox ID="txtAnalysisStart" runat="server" CssClass="form-control" TextMode="Date" Style="text-align: center;" oninput="clearRedBorder(this)" />
        </td>
        <td><b>Analysis Completion Date</b></td>
        <td colspan="2">
            <asp:TextBox ID="txtCompleteDate" runat="server" CssClass="form-control" TextMode="Date" Style="text-align: center;" oninput="clearRedBorder(this)" />
        </td>

        
    </tr>
    <tr>
        <td><b>Certificate No.</b></td>
        <td>
            <asp:TextBox ID="txtCertificateNo" runat="server" CssClass="form-control" />
        </td>
        <td><b>Calibration Date</b></td>
        <td colspan="2">
            <asp:TextBox ID="txtCalibrationDate" runat="server" CssClass="form-control" TextMode="Date" />
        </td>
    </tr>
    <tr>
        <td><b>Tool Name</b></td>
        <td>
            <asp:TextBox ID="txtToolName" runat="server" CssClass="form-control" Text="(MOBILE PHONE)*" />
        </td>
        <td><b>Calibration Location</b></td>
        <td colspan="2">
            <asp:TextBox ID="txtCalibrationLocation" runat="server" CssClass="form-control" />
        </td>
    </tr>
    <tr>
        <td><b>No. Series</b></td>
        <td>
            <asp:TextBox ID="txtSeriesNo" runat="server" CssClass="form-control" />
        </td>
        <td><b>Calibration Method</b></td>
        <td colspan="2">
            <asp:TextBox ID="txtCalibrationMethod" runat="server" CssClass="form-control" Text="TSQ-CAL-IK-080" />
        </td>
    </tr>
    <tr>
        <td><b>Model / Type</b></td>
        <td>
            <asp:TextBox ID="txtModelType" runat="server" CssClass="form-control" />
        </td>
        <td><b>Ref.</b></td>
        <td colspan="2">
            <asp:TextBox ID="txtReference" runat="server" CssClass="form-control" Text="BS EN 837-1:1998" />
        </td>
    </tr>
    <tr>
        <td><b>Type</b></td>
        <td>
            <asp:TextBox ID="txtType" runat="server" CssClass="form-control" />
        </td>
        <td><b>Room Conditions</b></td>
        <td><b>Beginning</b></td>
        <td><b>End</b></td>
    </tr>
    <tr>
        <td><b>Capacity / Resolution</b></td>
        <td>
            <asp:TextBox ID="txtCapacity" runat="server" CssClass="form-control" />
        </td>
        <td><b>Room Temperature</b></td>
        <td>
            <asp:TextBox ID="txtRoomTempBegin" runat="server" CssClass="form-control" Placeholder="°C" />
        </td>
        <td>
            <asp:TextBox ID="txtRoomTempEnd" runat="server" CssClass="form-control" Placeholder="°C" />
        </td>
    </tr>
    <tr>
        <td><b>Resolution</b></td>
        <td>
            <asp:TextBox ID="txtResolution" runat="server" CssClass="form-control" />
        </td>
        <td><b>Humidity</b></td>
        <td>
            <asp:TextBox ID="txtHumidityBegin" runat="server" CssClass="form-control" Placeholder="%RH" />
        </td>
        <td>
            <asp:TextBox ID="txtHumidityEnd" runat="server" CssClass="form-control" Placeholder="%RH" />
        </td>
    </tr>
    <tr>
        <td><b>Accuracy</b></td>
        <td  colspan="4">
            <asp:TextBox ID="txtAccuracy" runat="server" CssClass="form-control" />
        </td>
   
    </tr>
    <tr>
        <td><b>Owner</b></td>
        <td  colspan="4">
            <asp:TextBox ID="txtOwner" runat="server" CssClass="form-control" />
        </td>
    </tr>
    <tr>
        <td><b>Address</b></td>
        <td  colspan="4">
            <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
        </td>
       
    </tr>
      <tr>
    <td><b>Receipt Date</b></td>
    <td colspan="4">
        <asp:TextBox ID="txtReceiptDate" 
                     runat="server" 
                     CssClass="form-control" 
                     TextMode="Date"/>
    </td>

</tr>

</table>
                            </div>


                            <table border="1" width="100%" cellpadding="5" cellspacing="0">
                                <tr>
                                    <td><b>Standards Used</b></td>
                                    <td>
                                        <asp:TextBox ID="txtStandardsUsed" runat="server"
                                            CssClass="form-control"
                                            Text="Digital Pressure Calibrator"
                                            Style="text-align: center" />
                                    </td>

                                    <td><b>Equipment Brand</b></td>
                                    <td>
                                        <asp:TextBox ID="txtEquipmentBrand" runat="server"
                                            CssClass="form-control"
                                            Text="Additel"
                                            Style="text-align: center" />
                                    </td>
                                </tr>

                                <tr>
                                    <td><b>No. Series</b></td>
                                    <td>
                                        <asp:TextBox ID="txtNoSeries" runat="server"
                                            CssClass="form-control"
                                            Style="text-align: center" />
                                    </td>

                                    <td><b>Traced to SI</b></td>
                                    <td>
                                        <asp:TextBox ID="txtTracedToSI" runat="server"
                                            CssClass="form-control"
                                            Text="NVLAP LAB CODE 600214-0"
                                            Style="text-align: center" />
                                    </td>
                                </tr>

                                <tr>
                                    <td><b>Uncertainty</b></td>
                                    <td>
                                        <asp:TextBox ID="txtUncertainty" runat="server"
                                            CssClass="form-control"
                                            Style="text-align: center" />
                                    </td>

                                    <td><b>Calibrated By</b></td>
                                    <td>
                                        <asp:TextBox ID="txtCalibratedBy" runat="server"
                                            CssClass="form-control"
                                            Style="text-align: center" />
                                    </td>
                                </tr>

                                <tr>
                                    <td><b>Checked By</b></td>
                                    <td>
                                        <asp:TextBox ID="txtCheckedBy" runat="server"
                                            CssClass="form-control"
                                            Style="text-align: center" />
                                    </td>

                                    <td colspan="2"></td>
                                </tr>
                            </table>
                            <%--   <asp:Label ID="lblMessage" runat="server" CssClass="alert" Visible="false"></asp:Label>--%>

                            <div class="form-actions">
                                <div style="margin-bottom: 20px;">
                                    <asp:Button ID="btnDownlaodNew" runat="server" CssClass="btn btn-primary"
                                        Text="Download" OnClick="btnDownload_Click" Visible="false" />

                                </div>
                                <asp:Button ID="btnDownloadExisting" runat="server"
                                    Text="Download Existing File"
                                    CssClass="btn btn-primary"
                                    Visible="false"
                                    OnClick="btnDownloadExisting_Click" />
                            </div>

                            <!-- Step 2: Upload Filled Excel -->
                            <div style="margin-bottom: 15px;">
                                <label><strong>Step 2: Select your filled Excel file</strong></label>
                                <asp:FileUpload ID="fileUpload" runat="server" CssClass="form-control"
                                    Accept=".xlsx,.xls" />
                            </div>

                            <!-- Step 3: Preview  Save -->
                            <div style="display: flex; align-items: center; gap: 10px; margin-top: 15px;">
                                <asp:Button ID="btnPreview" runat="server" CssClass="btn btn-primary"
                                    Text="Preview PDF" OnClick="btnPreview_Click" />

                                <asp:Button ID="btnConfirmUpload" runat="server" CssClass="btn btn-primary"
                                    Text="Save Excel File" Visible="false" OnClick="btnConfirmUpload_Click" />

                                <%-- <small style="color: #666; margin-left: 10px;">Preview to check or Save directly
                                </small>--%>
                            </div>

                            <asp:Button ID="btnUpdateUpload" runat="server" Text="Update" CssClass="btn btn-primary"
                                OnClick="btnUpdateUpload_Click" Visible="false" />

                        </div>
                    </div>
                    <br />


                    <div>
                    </div>
                </div>
            </div>

            </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnDownlaodNew" />
            <asp:PostBackTrigger ControlID="btnPreview" />
            <asp:PostBackTrigger ControlID="btnConfirmUpload" />
            <asp:PostBackTrigger ControlID="btnUpdateUpload" />
            <asp:PostBackTrigger ControlID="btnDownloadExisting" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>

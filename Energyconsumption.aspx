<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Energyconsumption.aspx.cs" Inherits="LIMS.Energyconsumption" %>



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
          <div id="google_translate_element"></div>
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
                 
                </table>
            </div>
            <!-- Add this in your form -->
           <div>
    <asp:Label ID="lblImageUpload" runat="server" Text="Upload Lab Analysis Images:"></asp:Label>
    <asp:FileUpload ID="fuLabAnalysis" runat="server" AllowMultiple="true" />
    <asp:Button ID="btnUploadImages" runat="server" Text="Upload Images" OnClick="btnUploadImages_Click" />
</div>

            <br />
            <br />
            <br />
          
            <hr />

<div class="measurement-section">
            <div class="section-title">Measurement Results</div>
            
            <table class="measurement-table">
                <thead>
                    <tr class="header-main">
                        <th colspan="2">
                            Measurement Results<br />
                            <span style="font-style: italic; font-weight: normal; font-size: 11px;">Measurement Results</span>
                        </th>
                    </tr>
                    <tr>
                        <th>
                            Parameter<br />
                            <span style="font-style: italic; font-weight: normal; font-size: 11px;">Parameters</span>
                        </th>
                        <th>
                            Results<br />
                            <span style="font-style: italic; font-weight: normal; font-size: 11px;">Result</span>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td class="param-label">Screen size [cm2]</td>
                        <td class="result-value">
                            <asp:TextBox ID="txtScreenSize" runat="server" CssClass="input-box" AutoPostBack="false"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="param-label">L<sub>ratio</sub> [%]</td>
                        <td class="result-value">
                            <asp:TextBox ID="txtLratio" runat="server" CssClass="input-box" AutoPostBack="false"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="param-label">P<sub>standby_passive</sub> [Watt]</td>
                        <td class="result-value">
                            <asp:TextBox ID="txtPstandby" runat="server" CssClass="input-box" AutoPostBack="false"></asp:TextBox>
                        </td>
                    </tr>
                    <tr class="calculated-row">
                        <td class="param-label">EC [Wh/cm2/year]</td>
                        <td class="result-value">
                            <asp:TextBox ID="txtEC" runat="server" CssClass="input-box" ReadOnly="true" BackColor="#fff9e6"></asp:TextBox>
                        </td>
                    </tr>
                </tbody>
            </table>
            
            <div style="text-align: center;">
                <asp:Button ID="btnCalculateEC" runat="server" Text="Calculate EC" CssClass="btn-save" OnClick="btnCalculateEC_Click" />
                <asp:Button ID="Button1" runat="server" Text="Save & Generate Report" CssClass="btn-save" OnClick="btnSave_Click" />
            </div>
            
            <asp:Label ID="Label1" runat="server" ForeColor="Green" Font-Bold="true"></asp:Label>
        </div> <!-- Star Category Table (Static) -->
        <div class="measurement-section">
            <div class="section-title">Star Category</div>
            
            <table class="measurement-table">
                <thead>
                    <tr class="header-main">
                        <th colspan="2">
                            Star category<br />
                            <span style="font-style: italic; font-weight: normal; font-size: 11px;">Star category</span>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td style="text-align: center; font-weight: bold;">**</td>
                        <td>≤ 29.78</td>
                    </tr>
                    <tr>
                        <td style="text-align: center; font-weight: bold;">***</td>
                        <td>≤ 25.27</td>
                    </tr>
                    <tr>
                        <td style="text-align: center; font-weight: bold;">****</td>
                        <td>≤ 20.33</td>
                    </tr>
                    <tr>
                        <td style="text-align: center; font-weight: bold;">*****</td>
                        <td>≤ 15.24</td>
                    </tr>
                    <tr>
                        <td style="text-align: center; font-weight: bold;">******</td>
                        <td>≤ 10.59</td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!-- Conclusion Section -->
        <div class="measurement-section">
            <div class="section-title">Conclusion</div>
            
            <table class="measurement-table">
                <thead>
                    <tr class="header-main">
                        <th>
                            Conclusion<br />
                            <span style="font-style: italic; font-weight: normal; font-size: 11px;">Conclusion</span>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td style="text-align: center; padding: 20px; font-size: 14px;">
                            <asp:Label ID="lblConclusion" runat="server" Text="Three Stars" Font-Bold="true" Font-Italic="true"></asp:Label>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
          <%--  <div style="text-align: center;">
                <asp:Button ID="btnSaveMeasurement" runat="server" Text="Save Measurement Results" 
                            CssClass="btn-save" OnClick="btnSaveMeasurement_Click" />
            </div>--%>
            
            <asp:Label ID="lblMessage" runat="server" ForeColor="Green" Font-Bold="true"></asp:Label>
        </div>
  

            <div class="btn-container">
                <asp:Button ID="btnSave" runat="server" Text="Save Results" OnClick="btnSave_Click" CssClass="btn btn-save" CausesValidation="true" ValidationGroup="DefaultDataUpperDetails" />
               <%-- <asp:Button ID="btnGenerateReport" runat="server" Text="Generate Report" OnClick="btnExport_Click" CssClass="btn btn-report" Visible="false" />--%>
            </div>

          <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
 <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <div class="table-responsive">
            <table class="table table-bordered">
                <tbody>
                    <tr>
                        <td style="width: 40%;">Testing Officer Name</td>
                        <td style="width: 60%;">
                            <asp:TextBox ID="txtTestingOfficer" runat="server" CssClass="form-control" placeholder="Testing Officer Name" />
                        </td>
                    </tr>
                    <tr>
                        <td>Name of Examining Officer</td>
                        <td>
                            <asp:TextBox ID="txtExaminingOfficer" runat="server" CssClass="form-control" placeholder="Name of Examining Officer" />
                        </td>
                    </tr>
                    <tr>
                        <td>Name of Person in Charge of Testing Laboratory<br/>(Head of Lab)</td>
                        <td>
                            <asp:TextBox ID="txtHeadOfLab" runat="server" CssClass="form-control" placeholder="Name of Person in Charge" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="background-color: #d3d3d3; font-weight: bold;">PRODUCT SPECIFICATIONS</td>
                    </tr>
                   
                    <tr>
                        <td>Version or Serial Number</td>
                        <td>
                            <asp:TextBox ID="txtVersion" runat="server" CssClass="form-control" placeholder="Version or Serial Number" />
                        </td>
                    </tr>
                    <tr>
                        <td>Voltage</td>
                        <td>
                            <asp:TextBox ID="txtVoltage" runat="server" CssClass="form-control" placeholder="Voltage" />
                        </td>
                    </tr>
                    <tr>
                        <td>Frequency</td>
                        <td>
                            <asp:TextBox ID="txtFrequency" runat="server" CssClass="form-control" placeholder="Frequency" />
                        </td>
                    </tr>
                    <tr>
                        <td>Rated Power</td>
                        <td>
                            <asp:TextBox ID="txtRatedPower" runat="server" CssClass="form-control" placeholder="Rated Power" />
                        </td>
                    </tr>
                    <tr>
                        <td>Rated Current</td>
                        <td>
                            <asp:TextBox ID="txtRatedCurrent" runat="server" CssClass="form-control" placeholder="Rated Current" />
                        </td>
                    </tr>
                    <tr>
                        <td>Firmware or Software Version</td>
                        <td>
                            <asp:TextBox ID="txtFirmware" runat="server" CssClass="form-control" placeholder="Firmware or Software Version" />
                        </td>
                    </tr>
                    <tr>
                        <td>HDMI Port Features</td>
                        <td>
                            <asp:TextBox ID="txtHDMIPort" runat="server" CssClass="form-control" placeholder="HDMI Port Features" />
                        </td>
                    </tr>
                    <tr>
                        <td>USB Port Features</td>
                        <td>
                            <asp:TextBox ID="txtUSBPort" runat="server" CssClass="form-control" placeholder="USB Port Features" />
                        </td>
                    </tr>
                    <tr>
                        <td>AV Input Features</td>
                        <td>
                            <asp:TextBox ID="txtAVInput" runat="server" CssClass="form-control" placeholder="AV Input Features" />
                        </td>
                    </tr>
                    <tr>
                        <td>Internet Network Features</td>
                        <td>
                            <asp:TextBox ID="txtInternetNetwork" runat="server" CssClass="form-control" placeholder="Internet Network Features" />
                        </td>
                    </tr>
                    <tr>
                        <td>MDD (Motion-Based Dimming) Feature</td>
                        <td>
                            <asp:TextBox ID="txtMDD" runat="server" CssClass="form-control" placeholder="MDD Feature" />
                        </td>
                    </tr>
                    <tr>
                        <td>ABC (Automatic Brightness Control) feature</td>
                        <td>
                            <asp:TextBox ID="txtABC" runat="server" CssClass="form-control" placeholder="ABC Feature" />
                        </td>
                    </tr>
                    <tr>
                        <td>Retail Mode Features</td>
                        <td>
                            <asp:TextBox ID="txtRetailMode" runat="server" CssClass="form-control" placeholder="Retail Mode Features" />
                        </td>
                    </tr>
                    <tr>
                        <td>Video Signal Types</td>
                        <td>
                            <asp:TextBox ID="txtVideoSignal" runat="server" CssClass="form-control" placeholder="Video Signal Types" />
                        </td>
                    </tr>
                    <tr>
                        <td>Battery Component Availability</td>
                        <td>
                            <asp:TextBox ID="txtBattery" runat="server" CssClass="form-control" placeholder="Battery Component" />
                        </td>
                    </tr>
                    <tr>
                        <td>Quick Start Feature</td>
                        <td>
                            <asp:TextBox ID="txtQuickStart" runat="server" CssClass="form-control" placeholder="Quick Start Feature" />
                        </td>
                    </tr>
                    <tr>
    <td>Satellite Features</td>
    <td>
        <asp:TextBox ID="txtSatellite" runat="server" CssClass="form-control" placeholder="Satellite Features" />
    </td>
</tr>
                </tbody>
            </table>


            <br /><br />
            <table border="1" cellpadding="8" cellspacing="0" style="border-collapse: collapse; width: 100%;">
    <thead>
        <tr style="background-color: #f2f2f2;">
            <th style="width: 60%;">Testing Conditions</th>
            <th style="width: 40%;">Results</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>Test voltage [V]</td>
            <td><input type="text" id="txtTestVoltage" style="width: 100%; padding: 4px;" value="220.62"></td>
        </tr>
        <tr>
            <td>Test frequency [Hz]</td>
            <td><input type="text" id="txtTestFrequency" style="width: 100%; padding: 4px;" value="50,007"></td>
        </tr>
        <tr>
            <td>Total Harmonic Distortion in Supply Voltage [%]</td>
            <td><input type="text" id="txtHarmonicDistortion" style="width: 100%; padding: 4px;" value="0.52"></td>
        </tr>
        <tr>
            <td>Test room temperature [°C]</td>
            <td><input type="text" id="txtTemperature" style="width: 100%; padding: 4px;" value="22.1"></td>
        </tr>
        <tr>
            <td>Test setup description</td>
            <td><textarea id="txtSetupDescription" style="width: 100%; padding: 4px; height: 60px;">- The test room uses a darkroom with lighting conditions silk. - The test table is used with a surface base using black cloth.</textarea></td>
        </tr>
        <tr>
            <td>Selected input terminal in UUT</td>
            <td><input type="text" id="txtInputTerminal" style="width: 100%; padding: 4px;" value="USB Port"></td>
        </tr>
        <tr>
            <td>The video signal selected to measure power consumption in on mode</td>
            <td><input type="text" id="txtVideoSignalPower" style="width: 100%; padding: 4px;" value="SDR Broadcast Content Video Signal"></td>
        </tr>
        <tr>
            <td>Resolution and speed in selected frame</td>
            <td><input type="text" id="txtResolution" style="width: 100%; padding: 4px;" value="HD SDHz"></td>
        </tr>
        <tr>
            <td>Settings changed from default to fill the entire display area</td>
            <td><input type="text" id="txtDisplaySettings" style="width: 100%; padding: 4px;" value="16:9 signal source Law 16:9"></td>
        </tr>
        <tr>
            <td>The video signal selected to measure the peak luminance ratio</td>
            <td><input type="text" id="txtPeakLuminanceSignal" style="width: 100%; padding: 4px;" value="L20PeakLumMotion"></td>
        </tr>
    </tbody>
</table>
            <br /><br />
              <table class="table table-bordered">
                <tbody>
                    <tr>
                        <td colspan="3" style="background-color: #d3d3d3; font-weight: bold; text-align: center;">TEST RESULTS</td>
                    </tr>
                    
                    <!-- ON MODE Header -->
                    <tr style="background-color: #e8f4f8;">
                        <td colspan="3" style="font-weight: bold; background-color: #d3d3d3;">ON MODE</td>
                    </tr>
                    <tr style="background-color: #f0f8ff;">
                        <td style="width: 40%; font-weight: bold;"></td>
                        <td style="width: 20%; font-weight: bold; text-align: center;">SDR On Mode</td>
                        <td style="width: 20%; font-weight: bold; text-align: center;">HDR On Mode</td>                       
                    </tr>
                    
                    <!-- MDD Features -->
                    <tr>
                        <td>MDD Features</td>
                        <td>
                            <asp:TextBox ID="txtMDDSDR" runat="server" CssClass="form-control" Text="ns a)" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtMDDHDR" runat="server" CssClass="form-control" Text="-" />
                        </td>
                   
                    </tr>
                    
                    <!-- ABC Features -->
                    <tr>
                        <td>ABC Features</td>
                        <td>
                            <asp:TextBox ID="txtABCSDR" runat="server" CssClass="form-control" Text="ns a)" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtABCHDR" runat="server" CssClass="form-control" Text="-" />
                        </td>
                       
                    </tr>
                    
                    <!-- Lighting level applied -->
                    <tr>
                        <td>Lighting level applied [Lux]</td>
                        <td>
                            <asp:TextBox ID="txtLightingSDR" runat="server" CssClass="form-control" Text="ns a)" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtLightingHDR" runat="server" CssClass="form-control" Text="-" />
                        </td>
                       
                    </tr>
                    
                    <!-- Power consumption on mode -->
                    <tr>
                        <td>Power consumption on mode (P<sub>on</sub>) [Watt]</td>
                        <td>
                            <asp:TextBox ID="txtPowerSDR" runat="server" CssClass="form-control" Text="34.95 b)" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtPowerHDR" runat="server" CssClass="form-control" Text="-" />
                        </td>
                       
                    </tr>
                    
                    <!-- PEAK LUMINANCE Header -->
                    <tr style="background-color: #e8f4f8;">
                        <td colspan="4" style="font-weight: bold; background-color: #d3d3d3;">PEAK LUMINANCE</td>
                    </tr>
                    
                    <!-- Peak Luminance Sub-header -->
                    <tr>
                       
                        <td style="font-weight: bold; text-align: center;">Luminance [Cd/m²]</td>
                        <td style="font-weight: bold; text-align: center;">Mode</td>
                      
                    </tr>
                    
                    <!-- Default image settings -->
                    <tr>
                        <td>Default image settings</td>
                        <td>
                            <asp:TextBox ID="txtDefaultLuminance" runat="server" CssClass="form-control" Text="171.6" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtDefaultMode" runat="server" CssClass="form-control" Text="Standard" />
                        </td>
                      
                    </tr>
                    
                    <!-- Brightest preset image settings -->
                    <tr>
                        <td>Brightest preset image settings</td>
                        <td>
                            <asp:TextBox ID="txtBrightestLuminance" runat="server" CssClass="form-control" Text="228.8" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtBrightestMode" runat="server" CssClass="form-control" Text="Vivid" />
                        </td>
                       
                    </tr>
                    
                    <!-- Actual image settings -->
                    <tr>
                        <td>Actual image settings</td>
                        <td>
                            <asp:TextBox ID="txtActualLuminance" runat="server" CssClass="form-control" Text="-" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtActualMode" runat="server" CssClass="form-control" Text="-" />
                        </td>
                      
                    </tr>
                    
                    <!-- Peak luminance ratio -->
                    <tr>
                        <td>Peak luminance ratio [%]</td>
                        <td colspan="2">
                            <asp:TextBox ID="txtPeakRatio" runat="server" CssClass="form-control" Text="75.0" />
                        </td>
                       
                    </tr>
                    
                    <!-- PARTIAL MODE Header -->
                    <tr style="background-color: #e8f4f8;">
                        <td colspan="4" style="font-weight: bold; background-color: #d3d3d3;">PARTIAL MODE</td>
                    </tr>
                    
                    <!-- Standby-passive header -->
                    <tr style="background-color: #f0f8ff;">
                        <td colspan="4" style="font-weight: bold;">Standby-passive power consumption</td>
                    </tr>
             
                    <!-- Standby-passive (default) -->
                    <tr>
                        <td>P<sub>standby-passive</sub> (default or quick start feature disabled) [Watt]</td>
                        <td colspan="3">
                            <asp:TextBox ID="txtStandbyDefault" runat="server" CssClass="form-control" Text="0.31 c)" />
                        </td>
                    </tr>
                    
                    <!-- Standby-passive (Quick Start enabled) -->
                    <tr>
                        <td>P<sub>standby-passive</sub> (Quick Start feature enabled) [Watt]</td>
                        <td colspan="3">
                            <asp:TextBox ID="txtStandbyQuickStart" runat="server" CssClass="form-control" Text="ns a)" />
                        </td>
                    </tr>
                    
                    <!-- Standby-passive, low -->
                    <tr>
                        <td>Standby-passive power consumption, low [Watt]</td>
                        <td colspan="3">
                            <asp:TextBox ID="txtStandbyLow" runat="server" CssClass="form-control" Text="ns a)" />
                        </td>
                    </tr>
                    
                    <!-- Power consumption Off mode -->
                    <tr>
                        <td>Power consumption Off mode [Watt]</td>
                        <td colspan="3">
                            <asp:TextBox ID="txtOffMode" runat="server" CssClass="form-control" Text="ns a)" />
                        </td>
                    </tr>
                </tbody>
            </table>
            <br /><br />
              <table class="table table-bordered">
                <tbody>
                    <tr>
                        <td colspan="2" style="background-color: #d3d3d3; font-weight: bold; text-align: center;">ENERGY CONSUMPTION</td>
                    </tr>
                    
                    <!-- Screen size -->
                    <tr>
                        <td style="width: 60%;">Screen size (cm²)</td>
                        <td style="width: 40%;">
                            <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control" Text="4977.89" />
                        </td>
                    </tr>
                    
                    <!-- Energy Consumption On Mode -->
                    <tr>    
                        <td>Energy Consumption On Mode (E<sub>on</sub>) [Wh/year]</td>
                        <td>
                            <asp:TextBox ID="txtEnergyOnMode" runat="server" CssClass="form-control" Text="89297.25" />
                        </td>
                    </tr>
                    
                    <!-- Standby-passive Energy Consumption -->
                    <tr>
                        <td>Standby-passive Energy Consumption [Wh/year]</td>
                        <td>
                            <asp:TextBox ID="txtStandbyEnergy" runat="server" CssClass="form-control" Text="1923.55" />
                        </td>
                    </tr>
                    
                    <!-- Total Energy Consumption -->
                    <tr>
                        <td>Total Energy Consumption (EC) [Wh/cm²/year]</td>
                        <td>
                            <asp:TextBox ID="txtTotalEnergy" runat="server" CssClass="form-control" Text="18.33" />
                        </td>
                    </tr>
                    
                    <!-- Additional Information Header -->
                    <tr>
                        <td colspan="2" style="background-color: #f2f2f2; font-weight: bold; padding: 10px;">
                            Additional information:
                        </td>
                    </tr>
                    
                    <!-- Additional Information Items -->
                    <tr>
                        <td colspan="2" style="padding: 15px;">
                            <div style="margin-bottom: 10px;">
                                <strong>a)</strong> na or not available means not included in the text due to unavailability on the UUT
                            </div>
                            <div style="margin-bottom: 10px;">
                                <strong>b)</strong> On mode power consumption testing using EOI video signals with sound volume at 8% of 100% sound level and default (standard) picture settings
                            </div>
                            <div>
                                <strong>c)</strong> The average power was 15 minutes stable after 1 hour of measurement.
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>

        </div>
        
        <div style="margin-top: 20px;">
            <asp:Button ID="btnSaveSpecs" runat="server" CssClass="btn btn-success" Text="Save Specifications" 
                OnClick="btnSaveSpecs_Click" Style="background-color: #4CAF50;" />
        </div>
    </ContentTemplate>
</asp:UpdatePanel>

       
    </form>
</body>
      <script src="js/demo.js"></script>
  <script type="text/javascript">
      function googleTranslateElementInit() {
          new google.translate.TranslateElement(
              { pageLanguage: 'en' },
              'google_translate_element'
          );
      }
  </script>

  <script src="//translate.google.com/translate_a/element.js?cb=googleTranslateElementInit"></script>
</html>

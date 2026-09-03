<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DimensionalCheck.aspx.cs" Inherits="LIMS.DimensionalCheck" %>

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

        table, td {
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

    <style>
        .full-grid table {
            width: 100% !important;
            table-layout: fixed !important; /* forces full stretch */
            border: 1px solid #000;
        }

        .full-grid img {
            width: 100% !important;
            height: 300px;
            object-fit: cover;
        }

        /* Form styling */
        .form-control {
            display: block;
            width: 100%;
            padding: 8px 12px;
            font-size: 14px;
            line-height: 1.5;
            color: #495057;
            background-color: #fff;
            background-clip: padding-box;
            border: 1px solid #ced4da;
            border-radius: 4px;
            transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
        }

            .form-control:focus {
                border-color: #80bdff;
                outline: 0;
                box-shadow: 0 0 0 0.2rem rgba(0,123,255,0.25);
            }

        .btn-primary {
            background-color: #d0d0d0;
            border-color: #d0d0d0;
            color: black;
            padding: 8px 16px;
            border-radius: 4px;
            cursor: pointer;
        }

            .btn-primary:hover {
                background-color: #0056b3;
                border-color: #0056b3;
            }

        /* GridView styling */
        .measurement-grid {
            border-collapse: collapse;
            margin: 20px 0;
            font-family: Arial, sans-serif;
        }

            .measurement-grid th {
                background-color: #d0d0d0;
                color: black;
                padding: 12px;
                text-align: left;
                border: 1px solid #000;
                font-weight: bold;
            }

            .measurement-grid td {
                padding: 10px;
                border: 1px solid #000;
                vertical-align: middle;
            }

        .image-cell {
            display: flex;
            align-items: center;
            gap: 15px;
            min-height: 70px;
        }

        .measurement-thumb {
            width: 200px;
            height: 200px;
            object-fit: cover;
            border: 2px solid #dee2e6;
            border-radius: 4px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }

        .image-info {
            flex: 1;
        }

        .image-text {
            font-weight: bold;
            color: #333;
            word-break: break-word;
        }

        .result-text, .standard-text {
            font-size: 14px;
            color: #495057;
        }

        .delete-btn {
            color: #dc3545;
            text-decoration: none;
            padding: 5px 10px;
            border: 1px solid #dc3545;
            border-radius: 4px;
            background: white;
            transition: all 0.3s;
            display: inline-block;
        }

            .delete-btn:hover {
                background: #dc3545;
                color: white;
                text-decoration: none;
            }

        /* Alert messages */
        .alert {
            padding: 12px 20px;
            border-radius: 4px;
            margin-bottom: 15px;
            border: 1px solid transparent;
        }

        .alert-success {
            color: #155724;
            background-color: #d4edda;
            border-color: #c3e6cb;
        }

        .alert-danger {
            color: #721c24;
            background-color: #f8d7da;
            border-color: #f5c6cb;
        }

        .alert-info {
            color: #0c5460;
            background-color: #d1ecf1;
            border-color: #bee5eb;
        }
    </style>

    <style>
        /* Lightbox Modal Styles */
        .image-lightbox {
            display: none;
            position: fixed;
            z-index: 9999;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0, 0, 0, 0.9);
            animation: fadeIn 0.3s;
        }

        @keyframes fadeIn {
            from {
                opacity: 0;
            }

            to {
                opacity: 1;
            }
        }

        .lightbox-content {
            margin: auto;
            display: block;
            max-width: 90%;
            max-height: 90%;
            margin-top: 5%;
        }

        #lightbox-image {
            animation: zoomIn 0.3s;
        }

        @keyframes zoomIn {
            from {
                transform: scale(0.9);
            }

            to {
                transform: scale(1);
            }
        }

        .lightbox-caption {
            text-align: center;
            color: #ccc;
            padding: 10px 0;
            height: 30px;
            font-size: 16px;
        }

        .lightbox-close {
            position: absolute;
            top: 20px;
            right: 35px;
            color: #f1f1f1;
            font-size: 40px;
            font-weight: bold;
            transition: 0.3s;
            cursor: pointer;
        }

            .lightbox-close:hover,
            .lightbox-close:focus {
                color: #bbb;
                text-decoration: none;
            }

        /* Navigation buttons */
        .lightbox-prev, .lightbox-next {
            cursor: pointer;
            position: absolute;
            top: 50%;
            width: auto;
            padding: 16px;
            margin-top: -50px;
            color: white;
            font-weight: bold;
            font-size: 30px;
            transition: 0.6s ease;
            user-select: none;
            background-color: rgba(0, 0, 0, 0.5);
            border-radius: 3px;
        }

        .lightbox-prev {
            left: 20px;
        }

        .lightbox-next {
            right: 20px;
        }

            .lightbox-prev:hover, .lightbox-next:hover {
                background-color: rgba(0, 0, 0, 0.8);
            }

        /* Make thumbnail clickable */
        .clickable-thumb {
            cursor: pointer;
            transition: transform 0.2s;
        }

            .clickable-thumb:hover {
                transform: scale(1.05);
                box-shadow: 0 4px 8px rgba(0,0,0,0.2);
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
                        <td style="background-color: #d1d0d0; font-weight: bold;">Test category</td>
                        <td colspan="3">
                            <asp:TextBox ID="txtTestCategory" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server"
                                ControlToValidate="txtTestCategory" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Starting Date is required" Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>
                    </tr>


                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Type</td>
                        <td>
                            <asp:TextBox ID="txtType" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server"
                                ControlToValidate="txtType" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Starting Date is required" Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>

                        <td style="background-color: #d1d0d0; font-weight: bold;">Test Method</td>
                        <td>
                            <asp:TextBox ID="txtstandard" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server"
                                ControlToValidate="txtstandard" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Starting Date is required" Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>
                    </tr>

                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Ranking</td>
                        <td>
                            <asp:TextBox ID="txtRanking" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server"
                                ControlToValidate="txtRanking" ValidationGroup="DefaultDataUpperDetails"
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

                        <td style="background-color: #d1d0d0; font-weight: bold;">Request No.</td>
                        <td>
                            <asp:TextBox ID="txtRequestNo" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvCompletionDate" runat="server"
                                ControlToValidate="txtRequestNo" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Completion Date is required"
                                Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>
                    </tr>

                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Room temp.(°C)</td>
                        <td>
                            <asp:TextBox ID="txttemp" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvTemp" runat="server"
                                ControlToValidate="txttemp" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Temperature is required" Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Humidity (%)</td>
                        <td>
                            <asp:TextBox ID="txthumidity" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvHumidity" runat="server"
                                ControlToValidate="txthumidity" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Humidity is required" Display="Dynamic" ForeColor="Red" Text="Required" />
                        </td>
                    </tr>

                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Sample Code</td>
                        <td>
                            <asp:TextBox ID="txtsampleCode" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server"
                                ControlToValidate="txtsampleCode" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Starting Date is required" Display="Dynamic" ForeColor="Red" Text="Required" />

                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Test Equipment No.</td>
                        <td>
                            <asp:TextBox ID="txtTestEquipmentNo" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server"
                                ControlToValidate="txtTestEquipmentNo" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Starting Date is required" Display="Dynamic" ForeColor="Red" Text="Required" />
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


            <div id="clauseContainer" runat="server" style="margin-bottom: 50px;"></div>

            <!-- image content -->

            <div style="background: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0; border: 1px solid #ddd;">
                <h3 style="color: #b34788; margin-bottom: 15px;">Upload Sample Images</h3>

                <!-- Message Display -->
                <asp:Label ID="lblImageMessage" runat="server"
                    Style="display: none; padding: 10px; margin: 10px 0; border-radius: 3px; font-weight: bold;"
                    Text=""></asp:Label>

                <!-- Upload Form -->
                <table style="width: 100%;">
                    <tr>
                        <td style="width: 40%;">
                            <asp:FileUpload ID="fileUpload" runat="server" Width="100%" />
                        </td>
                        <td style="width: 50%;">
                            <asp:TextBox ID="txtRemarks" runat="server" placeholder="sample Text"></asp:TextBox>
                        </td>

                        <td style="width: 10%; padding-left: 10px;">
                            <asp:Button ID="btnUpload" runat="server" Text="Upload"
                                OnClick="btnUpload_Click"
                                Style="background-color: #b34788; color: white; padding: 8px 20px; border: none; border-radius: 3px; cursor: pointer;" />
                        </td>
                    </tr>
                </table>

                <div style="margin-top: 5px; font-size: 12px; color: #666;">
                    <i class="fas fa-info-circle"></i>Allowed: JPG, PNG, GIF, BMP | Max: 5MB
                </div>

                <!-- Image Preview Section -->
                <div id="divImages" runat="server" style="margin-top: 50px;">
                    <h4 style="color: #666; border-bottom: 1px solid #ddd; padding-bottom: 5px;">Uploaded Images</h4>

                    <asp:GridView ID="gvImages" runat="server" AutoGenerateColumns="false"
                        CssClass="image-grid" Width="100%" GridLines="None"
                        OnRowCommand="gvImages_RowCommand" DataKeyNames="ID" CellPadding="5">

                        <Columns>
                            <asp:TemplateField HeaderText="No.">
                                <HeaderStyle Width="5%"
                                    BackColor="#f8f9fa"
                                    Font-Bold="true"
                                    BorderStyle="Solid"
                                    BorderWidth="2px"
                                    BorderColor="#333" />
                                <ItemStyle Width="5%" />
                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Image">
                                <HeaderStyle Width="40%"
                                    BackColor="#f8f9fa"
                                    Font-Bold="true"
                                    BorderStyle="Solid"
                                    BorderWidth="2px"
                                    BorderColor="#333" />
                                <ItemStyle Width="40%" />
                                <ItemTemplate>
                                    <img src='<%# "SampleImages/" + Eval("filename") %>'
                                        style="width: 100%; object-fit: cover; border: 2px solid black;" />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="SampleText" HeaderText="File Name">
                                <HeaderStyle Width="20%" BackColor="#f8f9fa"
                                    Font-Bold="true"
                                    BorderStyle="Solid"
                                    BorderWidth="2px"
                                    BorderColor="#333" />
                                <ItemStyle Width="20%" />
                            </asp:BoundField>

                            <asp:BoundField DataField="createdDate" HeaderText="Upload Date"
                                DataFormatString="{0:dd-MMM-yyyy HH:mm}">
                                <HeaderStyle Width="20%" BackColor="#f8f9fa"
                                    Font-Bold="true"
                                    BorderStyle="Solid"
                                    BorderWidth="2px"
                                    BorderColor="#333" />
                                <ItemStyle Width="20%" />
                            </asp:BoundField>

                            <asp:TemplateField HeaderText="Actions">
                                <HeaderStyle Width="15%" BackColor="#f8f9fa"
                                    Font-Bold="true"
                                    BorderStyle="Solid"
                                    BorderWidth="2px"
                                    BorderColor="#333" />
                                <ItemStyle Width="15%" />
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkDelete" runat="server"
                                        CommandName="DeleteImage"
                                        CommandArgument='<%# Eval("ID") + "|" + Eval("filename") %>'
                                        OnClientClick="return confirm('Delete this image?');"
                                        Style="color: #dc3545; text-decoration: none;">
                                     <i class="fas fa-trash"></i> Delete
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>

                        <HeaderStyle BackColor="#f8f9fa" Font-Bold="true" />
                        <RowStyle BorderColor="#333" />
                        <AlternatingRowStyle BackColor="#f9f9f9" />
                    </asp:GridView>

                </div>


                <%--new code update--%>
                <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

                <!-- APPENDIX A Form Section -->
                <div id="divManualImageData" runat="server" style="margin-top: 50px;">
                    <h4 style="color: #666; border-bottom: 1px solid #ddd; padding-bottom: 5px;">APPENDIX A</h4>

                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <!-- Form for adding measurements -->
                            <table style="width: 100%; margin-bottom: 20px;">
                                <tr>
                                    <td style="width: 30%; vertical-align: top;">
                                        <asp:Label ID="label1" runat="server" Text="The Measured Part" Font-Bold="true"></asp:Label><br />
                                        <asp:DropDownList ID="ddlImageName" runat="server" Width="95%" CssClass="form-control"></asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rfvImageName" runat="server" ValidationGroup="ObjectDetails"
                                            ControlToValidate="ddlImageName" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>

                                    <td style="width: 30%; vertical-align: top;">
                                        <asp:Label ID="Label2" runat="server" Text="Results measurement (mm)" Font-Bold="true"></asp:Label><br />
                                        <asp:TextBox ID="txtResultsMeasurement" runat="server" Width="95%" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ValidationGroup="ObjectDetails"
                                            ControlToValidate="txtResultsMeasurement" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>

                                    <td style="width: 30%; vertical-align: top;">
                                        <asp:Label ID="Label3" runat="server" Text="Standard requirements (mm)" Font-Bold="true"></asp:Label><br />
                                        <asp:TextBox ID="txtStandardRequirements" runat="server" Width="95%" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ValidationGroup="ObjectDetails"
                                            ControlToValidate="txtStandardRequirements" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>

                                    <td style="width: 10%; vertical-align: bottom;">
                                        <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-primary" Text="ADD"
                                            Style="background-color: #2196F3; padding: 8px 20px;"
                                            CausesValidation="true" ValidationGroup="ObjectDetails"
                                            OnClick="btnSubmit_Click" />
                                    </td>
                                </tr>
                            </table>

                            <!-- Measurement Data GridView -->
                            <div id="divMeasurementGrid" runat="server" style="margin-top: 30px;" visible="false">
                                <h4 style="color: #666; border-bottom: 1px solid #ddd; padding-bottom: 5px;">Measurement Data</h4>

                                <asp:GridView ID="gvMeasurements" runat="server" AutoGenerateColumns="false"
                                    CssClass="measurement-grid" Width="100%" GridLines="Both"
                                    OnRowCommand="gvMeasurements_RowCommand" OnRowDataBound="gvMeasurements_RowDataBound"
                                    DataKeyNames="ImageName" CellPadding="5">

                                    <Columns>

                                        <asp:TemplateField HeaderText="The Measured Part" HeaderStyle-Width="45%">
                                            <ItemTemplate>
                                                <div class="image-cell">
                                                    <a href='<%# "SampleImages/" + Eval("FileName") %>'
                                                        target="_blank"
                                                        title="Click to view full size">
                                                        <img src='<%# "SampleImages/" + Eval("FileName") %>'
                                                            alt='<%# Eval("DisplayText") %>'
                                                            class="measurement-thumb" />
                                                    </a>
                                                    <div class="image-info">
                                                        <asp:Label ID="lblDisplayText" runat="server"
                                                            Text='<%# Eval("DisplayText") %>' CssClass="image-text" />
                                                        <div style="margin-top: 5px;">
                                                            <a href='<%# "SampleImages/" + Eval("FileName") %>'
                                                                target="_blank"
                                                                style="font-size: 12px; color: #071fdd; text-decoration: none;">
                                                                <i class="fas fa-external-link-alt"></i>View Full Size
                                                            </a>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Results measurement (mm)" HeaderStyle-Width="25%">
                                            <ItemTemplate>
                                                <asp:Label ID="lblMeasurementResult" runat="server"
                                                    Text='<%# Eval("MeasurementResult") %>' CssClass="result-text" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Standard requirements (mm)" HeaderStyle-Width="25%">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStandardRequirement" runat="server"
                                                    Text='<%# Eval("StandardRequirement") %>' CssClass="standard-text" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Action" HeaderStyle-Width="5%">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkDelete" runat="server"
                                                    CommandName="DeleteRow"
                                                    CommandArgument='<%# Container.DataItemIndex %>'
                                                    OnClientClick="return confirm('Delete this measurement?');"
                                                    CssClass="delete-btn">
                                                    <i class="fas fa-trash"></i>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>

                                    <EmptyDataTemplate>
                                        <div style="text-align: center; padding: 20px; color: #666;">
                                            No measurement data added yet. Use the form above to add measurements.
                                        </div>
                                    </EmptyDataTemplate>
                                </asp:GridView>

                                <!-- Status message label -->
                                <asp:Label ID="lblGridMessage" runat="server" Visible="false"
                                    CssClass="alert alert-info" Style="display: block; margin-top: 10px;"></asp:Label>
                            </div>

                            <!-- Lightbox Modal -->
                            <div id="imageLightbox" class="image-lightbox">
                                <span class="lightbox-close" onclick="closeLightbox()">&times;</span>

                                <!-- Navigation buttons -->
                                <a class="lightbox-prev" onclick="changeImage(-1)">&#10094;</a>
                                <a class="lightbox-next" onclick="changeImage(1)">&#10095;</a>

                                <div class="lightbox-content">
                                    <img id="lightbox-image" src="" alt="Full size image" />
                                    <div class="lightbox-caption">
                                        <span id="lightbox-caption"></span>
                                        <a id="download-link" href="" download style="margin-left: 20px; color: #4CAF50; text-decoration: none;">
                                            <i class="fas fa-download"></i>
                                        </a>
                                    </div>
                                </div>


                            </div>

                        </ContentTemplate>

                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnSubmit" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="gvMeasurements" EventName="RowCommand" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>

            </div>

            <div class="btn-container">
                <asp:Button ID="btnSave" runat="server" Text="Save Results" OnClick="btnSave_Click" CssClass="btn btn-save" CausesValidation="true" ValidationGroup="DefaultDataUpperDetails" />
            </div>

        </div>
    </form>
</body>

<script type="text/javascript">
    var currentImages = [];
    var currentIndex = 0;

    // Initialize images array from GridView
    function initImages() {
        currentImages = [];
        // Collect all images from the grid
        $('.measurement-thumb').each(function () {
            var imgSrc = $(this).attr('src');
            var imgAlt = $(this).attr('alt');
            currentImages.push({
                src: imgSrc,
                alt: imgAlt
            });
        });
    }

    // Open lightbox with specific image
    function openLightbox(index) {
        initImages();
        currentIndex = index;
        showImage(currentIndex);
        document.getElementById("imageLightbox").style.display = "block";
        document.body.style.overflow = "hidden"; // Prevent scrolling
    }

    // Show specific image in lightbox
    function showImage(index) {
        if (currentImages.length === 0) return;

        if (index >= currentImages.length) currentIndex = 0;
        if (index < 0) currentIndex = currentImages.length - 1;

        var image = currentImages[currentIndex];
        var lightboxImg = document.getElementById("lightbox-image");
        var caption = document.getElementById("lightbox-caption");
        var downloadLink = document.getElementById("download-link");

        lightboxImg.src = image.src;
        caption.innerHTML = image.alt;
        downloadLink.href = image.src;
        downloadLink.download = image.src.split('/').pop();
    }

    // Navigate to next/previous image
    function changeImage(direction) {
        currentIndex += direction;
        showImage(currentIndex);
    }

    // Close lightbox
    function closeLightbox() {
        document.getElementById("imageLightbox").style.display = "none";
        document.body.style.overflow = "auto"; // Restore scrolling
    }

    // Close with ESC key
    document.addEventListener('keydown', function (event) {
        if (event.key === 'Escape') {
            closeLightbox();
        } else if (event.key === 'ArrowRight') {
            changeImage(1);
        } else if (event.key === 'ArrowLeft') {
            changeImage(-1);
        }
    });

    // Close when clicking outside image
    document.getElementById('imageLightbox').addEventListener('click', function (e) {
        if (e.target.id === 'imageLightbox') {
            closeLightbox();
        }
    });

    // Simple open in new tab function (alternative)
    function openImageInNewTab(imageSrc) {
        window.open(imageSrc, '_blank');
    }
</script>
</html>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="AddTestParameters.aspx.cs" Inherits="LIMS.AddTestParameters" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .form-control {
            width: 100%; /* Ensures it takes full width of its parent */
        }

        .auto-style1 {
            display: block;
            font-size: 14px;
            line-height: 1.42857143;
            color: #555;
            border-radius: 4px;
            -webkit-box-shadow: inset 0 1px 1px rgba(0, 0, 0, .075);
            box-shadow: inset 0 1px 1px rgba(0, 0, 0, .075);
            -webkit-transition: border-color ease-in-out .15s, -webkit-box-shadow ease-in-out .15s;
            -o-transition: border-color ease-in-out .15s, box-shadow ease-in-out .15s;
            transition: border-color ease-in-out .15s, box-shadow ease-in-out .15s;
            border: 1px solid #ccc;
            padding: 6px 12px;
            background-color: #fff;
            background-image: none;
        }

        .required-field::after {
            content: "*";
            color: red;
            margin-left: 4px;
        }

        .add-uom-btn {
            background-color: #b34788;
            border-color: #b34788;
            color: white;
            border-radius: 3px;
            padding: 5px 10px;
            font-size: 12px;
            margin-top: 5px;
        }

            .add-uom-btn:hover {
                background-color: #822c97;
                border-color: #1e7e34;
            }

        .uom-input-group {
            display: flex;
            align-items: flex-end;
            gap: 10px;
        }

        .uom-dropdown-wrapper {
            flex: 1;
        }

        /* UOM Form Panel Styles */
        .uom-form-panel {
            border: 2px solid #b34788;
            border-radius: 8px;
            padding: 20px;
            margin: 20px 0;
            background-color: #f8f9fa;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }

        .uom-form-header {
            background-color: #b34788;
            color: white;
            padding: 10px 15px;
            margin: -20px -20px 15px -20px;
            border-radius: 6px 6px 0 0;
            font-weight: bold;
            font-size: 16px;
        }

        .uom-form-buttons {
            margin-top: 15px;
            text-align: center;
        }

        .btn-success {
            background-color: #28a745;
            border-color: #28a745;
            color: white;
            padding: 8px 20px;
            border-radius: 4px;
            margin-right: 10px;
        }

        .btn-secondary {
            background-color: #6c757d;
            border-color: #6c757d;
            color: white;
            padding: 8px 20px;
            border-radius: 4px;
        }

        .error-message {
            color: red;
            font-size: 12px;
            margin-top: 5px;
        }

        .success-message {
            color: green;
            font-size: 14px;
            font-weight: bold;
            margin-bottom: 15px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet"> 
                <div class="header">
                    <h2>ADD TEST PARAMETER</h2>
                </div>
                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <!-- Basic Parameter Information -->
                            <div class="row clearfix">
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label class="required-field">Test Parameter:</label>
                                            <asp:TextBox ID="txtTestParameter" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTestParameter" Display="Dynamic"
                                                ErrorMessage="Please enter TestParameter" CssClass="error-message" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label class="required-field">Department:</label>
                                            <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlDepartment_SelectedIndexChanged">
                                                <asp:ListItem Text="--Select Department--" Value="" />
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlDepartment" Display="Dynamic"
                                                ErrorMessage="Please select a Department" CssClass="error-message" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Requirement:</label>
                                            <div class="uom-input-group">
                                                <div class="requirement-dropdown-wrapper">
                                                    <asp:DropDownList ID="ddlrequirement" runat="server" CssClass="form-control">
                                                        <asp:ListItem Text="--Select Requirement--" Value="" />
                                                    </asp:DropDownList>
                                                </div>
                                                <asp:Button ID="btnShowAddRequirement" runat="server" Text="+"
                                                    CssClass="add-uom-btn" OnClick="btnShowAddRequirement_Click"
                                                    CausesValidation="false" ToolTip="Add New Requirement" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <asp:Panel ID="pnlAddRequirement" runat="server" Visible="false" CssClass="uom-form-panel">
                                <div class="uom-form-header">
                                    Add New Requirement
                                </div>

                                <asp:Label ID="lblRequirementMessage" runat="server" CssClass="success-message" Visible="false" />

                                <div class="row clearfix">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <label class="required-field">Requirement:</label>
                                                <asp:TextBox ID="txtNewRequirement" runat="server" CssClass="form-control"
                                                    placeholder="Enter Requirement Name" TextMode="MultiLine" Rows="3" />
                                                <asp:RequiredFieldValidator ID="rfvRequirement" runat="server"
                                                    ControlToValidate="txtNewRequirement" Display="Dynamic"
                                                    ErrorMessage="Requirement is required" CssClass="error-message"
                                                    ValidationGroup="RequirementValidation" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <label class="required-field">Department:</label>
                                                <asp:DropDownList ID="ddlRequirementDepartment" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="--Select Department--" Value="" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvRequirementDepartment" runat="server"
                                                    ControlToValidate="ddlRequirementDepartment" Display="Dynamic"
                                                    ErrorMessage="Department is required" CssClass="error-message"
                                                    ValidationGroup="RequirementValidation" />
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="uom-form-buttons">
                                    <asp:Button ID="btnSaveRequirement" runat="server" Text="Save Requirement"
                                        CssClass="btn btn-success" OnClick="btnSaveRequirement_Click"
                                        ValidationGroup="RequirementValidation" />
                                    <asp:Button ID="btnCancelRequirement" runat="server" Text="Cancel"
                                        CssClass="btn btn-secondary" OnClick="btnCancelRequirement_Click"
                                        CausesValidation="false" />
                                </div>
                            </asp:Panel>
                            <!-- TM and RID -->
                            <div class="row clearfix">
                                <div class="col-sm-4" style="margin-bottom: -5px!important;">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label class="required-field">UOM:</label>
                                            <div class="uom-input-group">
                                                <div class="uom-dropdown-wrapper">
                                                    <asp:DropDownList ID="ddlUOM" runat="server" CssClass="form-control">
                                                        <asp:ListItem Text="--Select UOM--" Value="" />
                                                    </asp:DropDownList>
                                                    <asp:HiddenField ID="hdnUOMId" runat="server" />
                                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlUOM" Display="Dynamic"
                                                        ErrorMessage="Please select a UOM" CssClass="error-message" ValidationGroup="MainValidation" />
                                                </div>
                                                <asp:Button ID="btnShowAddUOM" runat="server" Text="+"
                                                    CssClass="add-uom-btn" OnClick="btnShowAddUOM_Click"
                                                    CausesValidation="false" ToolTip="Add New UOM" />
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-4" style="margin-bottom: -5px!important;">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Test Method:</label>
                                            <div class="uom-input-group">
                                                <div class="testmethod-dropdown-wrapper">
                                                    <asp:DropDownList ID="ddlTestMethod" runat="server" CssClass="form-control">
                                                        <asp:ListItem Text="--Select Test Method--" Value="" />
                                                    </asp:DropDownList>
                                                </div>
                                                <asp:Button ID="btnShowAddTestMethod" runat="server" Text="+"
                                                    CssClass="add-uom-btn" OnClick="btnShowAddTestMethod_Click"
                                                    CausesValidation="false" ToolTip="Add New Test Method" />
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-4" style="margin-bottom: -5px!important;">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Specification:</label>
                                            <asp:TextBox ID="txtSpecification" AutoPostBack="true" OnTextChanged="txtSpecification_TextChanged" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>
                                </div>

                            </div>

                            <!-- PanelS (Initially Hidden) -->
                            <asp:Panel ID="pnlAddUOM" runat="server" Visible="false" CssClass="uom-form-panel">
                                <div class="uom-form-header">
                                    Add New Unit of Measurement (UOM)
                                </div>

                                <asp:Label ID="lblUOMMessage" runat="server" CssClass="success-message" Visible="false" />

                                <div class="row clearfix">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <label class="required-field">UOM Name:</label>
                                                <asp:TextBox ID="txtNewUOMName" runat="server" CssClass="form-control"
                                                    placeholder="Enter UOM Name (e.g., Milligrams)" />
                                                <asp:RequiredFieldValidator ID="rfvUOMName" runat="server"
                                                    ControlToValidate="txtNewUOMName" Display="Dynamic"
                                                    ErrorMessage="UOM Name is required" CssClass="error-message"
                                                    ValidationGroup="UOMValidation" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <label>UOM Code:</label>
                                                <asp:TextBox ID="txtNewUOMCode" runat="server" CssClass="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="uom-form-buttons">
                                    <asp:Button ID="btnSaveUOM" runat="server" Text="Save UOM"
                                        CssClass="btn btn-success" OnClick="btnSaveUOM_Click"
                                        ValidationGroup="UOMValidation" />
                                    <asp:Button ID="btnCancelUOM" runat="server" Text="Cancel"
                                        CssClass="btn btn-secondary" OnClick="btnCancelUOM_Click"
                                        CausesValidation="false" />
                                </div>
                            </asp:Panel>
                            <asp:Panel ID="pnlAddTestMethod" runat="server" Visible="false" CssClass="uom-form-panel">
                                <div class="uom-form-header">
                                    Add New Test Method
                                </div>

                                <asp:Label ID="lblTestMethodMessage" runat="server" CssClass="success-message" Visible="false" />

                                <div class="row clearfix">
                                    <div class="col-sm-12">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <label class="required-field">Test Method:</label>
                                                <asp:TextBox ID="txtNewTestMethod" runat="server" CssClass="form-control"
                                                    placeholder="Enter Test Method Name" TextMode="MultiLine" Rows="3" />
                                                <asp:RequiredFieldValidator ID="rfvTestMethod" runat="server"
                                                    ControlToValidate="txtNewTestMethod" Display="Dynamic"
                                                    ErrorMessage="Test Method is required" CssClass="error-message"
                                                    ValidationGroup="TestMethodValidation" />
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="uom-form-buttons">
                                    <asp:Button ID="btnSaveTestMethod" runat="server" Text="Save Test Method"
                                        CssClass="btn btn-success" OnClick="btnSaveTestMethod_Click"
                                        ValidationGroup="TestMethodValidation" />
                                    <asp:Button ID="btnCancelTestMethod" runat="server" Text="Cancel"
                                        CssClass="btn btn-secondary" OnClick="btnCancelTestMethod_Click"
                                        CausesValidation="false" />
                                </div>
                            </asp:Panel>

                            <!-- Food Department Fields (initially hidden) -->
                            <div id="divFoodFields" class="row clearfix" runat="server" style="display: none">
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>LOD:</label>
                                            <asp:TextBox ID="txtLOD" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>LOQ:</label>
                                            <asp:TextBox ID="txtLOQ" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>LOR:</label>
                                            <asp:TextBox ID="txtLOR" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- Chemical Department Fields (initially hidden) -->
                            <div id="divChemicalFields" class="row clearfix" runat="server" style="display: none">
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>MDL:</label>
                                            <asp:TextBox ID="txtMDL" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>CAS No:</label>
                                            <asp:TextBox ID="txtCASNo" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>
                                </div>
                             
                            </div>

                            <!-- Additional Information -->
                            <div class="row clearfix">

                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Position:</label>
                                            <asp:TextBox ID="txtPosition" runat="server" CssClass="form-control" TextMode="Number" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Clause:</label>
                                            <asp:TextBox ID="txtClause" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Specification Value:</label>
                                            <asp:TextBox ID="txtSpecificationValue" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- Buttons -->
                            <div class="row clearfix">
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary"
                                            OnClick="btnSubmit_Click" ValidationGroup="MainValidation" />
                                    </div>
                                </div>

                                <div class="col-sm-2">
                                    <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-warning"
                                        OnClick="btnClear_Click" CausesValidation="False" UseSubmitBehavior="false" />
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="TestSubParameterEdit.aspx.cs" Inherits="LIMS.TestSubParameterEdit" %>

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
    </style>
   
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <h2>EDIT TEST SUB-PARAMETER</h2>
                </div>
                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <!-- Basic Parameter Information -->
                            <div class="row clearfix">
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label class="required-field">Test Sub-Parameter:</label>
                                            <asp:TextBox ID="txtTestsubParameter" runat="server" CssClass="form-control" required="required" />
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
                                            <label class="required-field">Test Parameter:</label>
                                            <asp:DropDownList ID="ddltp" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddltp_SelectedIndexChanged">
                                                <asp:ListItem Text="--Select Test Parameter--" Value="" />
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddltp" Display="Dynamic"
                                                ErrorMessage="Please select a Test Parameter" CssClass="error-message" />
                                        </div>
                                    </div>
                                </div>

                            </div>

                            <!-- TM and RID -->
                            <div class="row clearfix">
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label class="required-field">UOM:</label>
                                            <asp:DropDownList ID="ddlUOM" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="--Select UOM--" Value="" />
                                            </asp:DropDownList>
                                            <asp:HiddenField ID="hdnUOMId" runat="server" />
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlUOM" Display="Dynamic"
                                                ErrorMessage="Please select a UOM" CssClass="error-message" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Test Method:</label>
                                            <asp:DropDownList ID="ddlTestMethod" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="--Select Test Method--" Value="" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Specification:</label>
                                            <asp:TextBox ID="txtSpecification" Autopostback="true" OnTextChanged="txtSpecification_TextChanged" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>
                                </div>

                            </div>


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
                                            <label>Requirement :</label>
                                            <asp:DropDownList ID="ddlrequirement" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="--Select Requirement--" Value="" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>


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
                                        <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btn btn-primary" OnClick="btnUpdate_Click" />
                                    </div>
                                </div>
                                
                                <div class="col-sm-2">
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-warning"
                                        OnClick="btnCancel_Click" CausesValidation="False" UseSubmitBehavior="false" />
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

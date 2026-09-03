<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="EditClauses.aspx.cs" Inherits="LIMS.EditClauses" %>


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
                    <h2>EDIT Clauses</h2>
                </div>
                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <!-- Basic Parameter Information -->
                            <div class="row clearfix">
                                <asp:Label ID="lblClausesID" runat="server"></asp:Label>
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
                                            <label class="required-field">Standard:</label>
                                            <asp:DropDownList ID="ddlStandards" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="--Select Standard--" Value="" />
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlStandards" Display="Dynamic"
                                                ErrorMessage="Please select a Standard" CssClass="error-message" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label class="required-field">Product:</label>
                                            <asp:DropDownList ID="ddlProduct" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="--Select Product--" Value="" />
                                            </asp:DropDownList>
                                            <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="ddlProduct" Display="Dynamic"
                                                ErrorMessage="Please select a Product" CssClass="error-message" />--%>
                                        </div>
                                    </div>
                                </div>
                            </div>


                            <div class="row clearfix">
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Clause Number:</label>
                                            <asp:TextBox ID="txtClauseNumber" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Clause Title:</label>
                                            <asp:TextBox ID="txtClauseTitle" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Clause Title BH:</label>
                                            <asp:TextBox ID="txtClauseTitleBH" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="row clearfix">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Requirement:</label>
                                            <asp:TextBox ID="txtRequirement" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Requirement BH:</label>
                                            <asp:TextBox ID="txtRequirementBH" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>
                                </div>


                            </div>


                            <div class="row clearfix">
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Result Options:</label>
                                            <asp:TextBox ID="txtResultOptions" runat="server" CssClass="form-control" placeholder="eg: YES,NO" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Result Options BH:</label>
                                            <asp:TextBox ID="txtResultOptionsBH" runat="server" CssClass="form-control" placeholder="eg: Ya,Tidak" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label class="required-field">Row Type:</label>
                                            <asp:DropDownList ID="ddlIsHeader" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="Normal" Value="0" />
                                                <asp:ListItem Text="Header" Value="1" />
                                            </asp:DropDownList>

                                        </div>
                                    </div>
                                </div>
                            </div>


                        </ContentTemplate>
                    </asp:UpdatePanel>

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

                </div>
            </div>
        </div>
    </div>
</asp:Content>

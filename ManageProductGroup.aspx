<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ManageProductGroup.aspx.cs" Inherits="LIMS.ManageProductGroup" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" />
    <style>
        .form-control {
            width: 100%;
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

        .header {
            display: flex;
            justify-content: space-between; /* Pushes h2 to left and button to right */
            align-items: center; /* Vertically centers items */
            padding: 10px 0;
        }

        .refresh-btn {
            padding: 8px 12px;
            background: #b34788 !important;
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            transition: background-color 0.3s;
            display: flex;
            align-items: center;
            gap: 5px;
        }

            .refresh-btn:hover {
                background: #2980b9;
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

        /* Grid Styles */
        .parameter-grid {
            margin-top: 20px;
            width: 100%;
            border-collapse: collapse;
        }

            .parameter-grid th {
                background-color: #b34788;
                color: white;
                font-weight: bold;
                padding: 10px 8px;
                text-align: left;
                border: 1px solid #ddd;
            }

            .parameter-grid td {
                padding: 8px;
                border: 1px solid #ddd;
                vertical-align: middle;
            }

            .parameter-grid tr:nth-child(even) {
                background-color: #f9f9f9;
            }

            .parameter-grid tr:hover {
                background-color: #f5f5f5;
            }

        .grid-edit-row {
            background-color: #fff3cd !important;
        }

        .btn-edit {
            background-color: #007bff;
            color: white;
            border: none;
            padding: 4px 8px;
            border-radius: 3px;
            font-size: 14px;
            margin-right: -1px;
            cursor: pointer;
        }

        .btn-delete {
            background-color: #dc3545;
            color: white;
            border: none;
            padding: 4px 8px;
            border-radius: 3px;
            font-size: 14px;
            cursor: pointer;
            margin-left: 30px;
        }

        .btn-update {
            background-color: #28a745;
            color: white;
            border: none;
            padding: 4px 8px;
            border-radius: 3px;
            font-size: 12px;
            margin-right: 3px;
            cursor: pointer;
        }

        .btn-cancel {
            background-color: #6c757d;
            color: white;
            border: none;
            padding: 4px 8px;
            border-radius: 3px;
            font-size: 12px;
            cursor: pointer;
        }

        .hierarchy-indent-1 {
            padding-left: 20px;
        }

        .hierarchy-indent-2 {
            padding-left: 40px;
        }

        .grid-container {
            overflow-x: auto;
        }

        .no-data-message {
            text-align: center;
            color: #666;
            font-style: italic;
            padding: 20px;
        }

        /* Grid edit controls */
        .grid-edit-control {
            width: 100%;
            padding: 4px;
            border: 1px solid #ccc;
            border-radius: 3px;
            font-size: 14px;
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
                    <h2>MANAGE GROUPS AND PARAMETERS</h2>
                </div>

                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>

                            <div class="row clearfix">
                                <div class="col-sm-8">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label class="required-field">Product Sub Group:</label>
                                            <asp:DropDownList ID="ddlProductSubGroup" runat="server" CssClass="form-control"
                                                AutoPostBack="true" OnSelectedIndexChanged="ddlProductSubGroup_SelectedIndexChanged">
                                                <asp:ListItem Text="--Select Product Sub Group--" Value="" />
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlProductSubGroup"
                                                Display="Dynamic" ErrorMessage="Please select a Product Sub Group"
                                                CssClass="error-message" ValidationGroup="MainValidation" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Department:</label>
                                            <asp:TextBox ID="txtdepartment" runat="server" CssClass="form-control" Text="Softline" ReadOnly="true" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="btn-group" role="group" style="margin-top: 22px; margin-bottom: -3px;">
                                        <a href="ManageGroupSubGroup.aspx" class="btn btn-primary mb-3 mb-md-0"><i class='bx bxs-plus-square'></i>ADD NEW GROUP</a>
                                    </div>
                                </div>
                            </div>

                            <div class="row clearfix">
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label class="required-field">Test Parameter:</label>
                                            <asp:DropDownList ID="ddlTestParameter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlTestParameter_SelectedIndexChanged" CssClass="form-control">
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlTestParameter"
                                                Display="Dynamic" ErrorMessage="Please select a Test Parameter"
                                                CssClass="error-message" ValidationGroup="MainValidation" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Test Sub Parameter:</label>
                                            <asp:DropDownList ID="ddlTestSubParameter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlTestSubParameter_SelectedIndexChanged" CssClass="form-control">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Test Sub Sub Parameter:</label>
                                            <asp:DropDownList ID="ddlTestSubSubParameter" runat="server" CssClass="form-control">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>

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

                            <div class="grid-container">
                                <asp:GridView ID="gvParameters" runat="server"
                                    CssClass="parameter-grid"
                                    AutoGenerateColumns="false"
                                    DataKeyNames="SubGroupID,TestParameterID,TestSubParameterID,TestSubSubParameterID,TestParameter,TestSubParameter,TestSubSubParameter"
                                    OnRowEditing="gvParameters_RowEditing"
                                    OnRowUpdating="gvParameters_RowUpdating"
                                    OnRowCancelingEdit="gvParameters_RowCancelingEdit"
                                    OnRowDeleting="gvParameters_RowDeleting"
                                    OnPageIndexChanging="gvParameters_PageIndexChanging"
                                    AllowPaging="true"
                                    PageSize="10"
                                    EmptyDataText="No parameters found."
                                    ShowHeaderWhenEmpty="true">
                                    <PagerStyle HorizontalAlign="Center" CssClass="grid-pager" />
                                    <Columns>
                                        <asp:BoundField DataField="SubGroupName" HeaderText="Sub Group" ReadOnly="true" />

                                        <asp:TemplateField HeaderText="Test Parameter">
                                            <ItemTemplate>
                                                <asp:Label runat="server" Text='<%# Eval("TestParameter") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:DropDownList ID="ddlEditTestParameter" runat="server" AutoPostBack="true"
                                                    OnSelectedIndexChanged="ddlEditTestParameter_SelectedIndexChanged" CssClass="grid-edit-control">
                                                </asp:DropDownList>
                                            </EditItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Test Sub Parameter">
                                            <ItemTemplate>
                                                <asp:Label runat="server" Text='<%# Eval("TestSubParameter") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:DropDownList ID="ddlEditTestSubParameter" runat="server" AutoPostBack="true"
                                                    OnSelectedIndexChanged="ddlEditTestSubParameter_SelectedIndexChanged" CssClass="grid-edit-control">
                                                </asp:DropDownList>
                                            </EditItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Test Sub Sub Parameter">
                                            <ItemTemplate>
                                                <asp:Label runat="server" Text='<%# Eval("TestSubSubParameter") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:DropDownList ID="ddlEditTestSubSubParameter" runat="server" CssClass="grid-edit-control">
                                                </asp:DropDownList>
                                            </EditItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="DepartmentName" HeaderText="Department" Visible="false" ReadOnly="true" />

                                        <asp:TemplateField HeaderText="Actions" HeaderStyle-Width="135px" ItemStyle-Width="135px">
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" CommandName="Edit" CssClass="btn-edit">
                                                <i class="fas fa-edit"></i> Edit
                                                </asp:LinkButton>

                                                <asp:LinkButton runat="server" CommandName="Delete" CssClass="btn-delete"
                                                    OnClientClick="return confirm('Are you sure you want to delete this parameter?');">
                                                <i class="fas fa-trash-alt"></i> 
                                                </asp:LinkButton>
                                            </ItemTemplate>

                                            <EditItemTemplate>
                                                <asp:LinkButton runat="server" CommandName="Update" CssClass="btn-update" Text="Update" />
                                                <asp:LinkButton runat="server" CommandName="Cancel" CssClass="btn-cancel" Text="Cancel" />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <div id="nodata" class="no-data-message">
                                            No parameters found for the selected Product Sub Group. Please add parameters using the form above.
                                        </div>
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" EnableEventValidation="false" AutoEventWireup="true" CodeBehind="PurchaseItemAdd.aspx.cs" Inherits="LIMS.PurchaseItemAdd" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- JavaScript moved here to the head section -->

    <style>
        .rbl1 [type="radio"]:not(:checked) + label {
            padding-left: 26px;
            height: 25px;
            line-height: 25px;
            font-size: 11px;
            font-weight: normal;
            vertical-align: top;
        }

        .rbl1 [type="radio"]:checked + label {
            padding-left: 26px;
            height: 25px;
            line-height: 25px;
            font-size: 11px;
            font-weight: bold;
            vertical-align: top;
        }

        .form-control {
            width: 100%; /* Ensures it takes full width of its parent */
        }

        #ddlVAT {
            width: 70%; /* Adjust this value as needed for your design */
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
                    <h2>ITEM MANAGER</h2>
                </div>
                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <!-- Puchase group and sub group -->
                            <div class="row clearfix">
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Purchase Group</label>
                                            <asp:DropDownList ID="ddlPurchaseGroup" runat="server" CssClass="form-control" Height="30px" Font-Size="12px" OnSelectedIndexChanged="ddlPurchaseGroup_SelectedIndexChanged" AutoPostBack="true">
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" InitialValue="" ErrorMessage="Please select a PurchaseGroup." ControlToValidate="ddlPurchaseGroup"
                                                Display="Dynamic" CssClass="text-danger">
                                            </asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Purchase Sub Group</label>
                                            <asp:DropDownList ID="ddlSubGroup" runat="server" CssClass="form-control" Height="30px" Font-Size="12px">
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" InitialValue="" ErrorMessage="Please select a PurchaseSubGroup." ControlToValidate="ddlSubGroup" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                            <asp:TextBox ID="txtType" runat="server" CssClass="form-control" placeholder="Type" Visible="false"></asp:TextBox>
                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" Enabled="True" TargetControlID="txtType" FilterType="UppercaseLetters,LowercaseLetters,Custom" ValidChars="1234567890 "></ajaxToolkit:FilteredTextBoxExtender>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Item code</label>
                                            <asp:TextBox ID="txtItemCode" runat="server" CssClass="form-control" OnTextChanged="txtItemCode_TextChanged" AutoPostBack="true" MaxLength="6"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="required" ControlToValidate="txtItemCode" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Item Name</label>
                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender4" runat="server" Enabled="True" TargetControlID="txtItemCode" FilterType="UppercaseLetters,LowercaseLetters,Custom" ValidChars="1234567890 "></ajaxToolkit:FilteredTextBoxExtender>
                                            <asp:TextBox ID="txtItemDescription" runat="server" CssClass="form-control"></asp:TextBox>
                                            <%--  <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtItemDescription" ValidationExpression="^[\s\S]{8,}$" ForeColor="Red" Display="Dynamic" ErrorMessage="Description too short"></asp:RegularExpressionValidator>--%>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="required" ControlToValidate="txtItemDescription" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                </div>
                            </div>


                            <%--/************Item Name and description***********/--%>
                            <div class="row clearfix">
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Account Code</label>
                                            <asp:DropDownList ID="ddlCOA" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlCOA_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>                                           
                                            <asp:RequiredFieldValidator
                                                ID="RequiredFieldValidator4"
                                                runat="server"
                                                ErrorMessage="Please select COA"
                                                ControlToValidate="ddlCOA"
                                                InitialValue=""
                                                Display="Dynamic"
                                                CssClass="text-danger">
                                            </asp:RequiredFieldValidator>
                                            <asp:TextBox ID="txtModel" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                        </div>
                                         <asp:Label ID="lblCOA" runat="server" Text=""></asp:Label>
                                    </div>
                                </div>
                                <div class="col-sm-9">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Item Description</label>
                                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" MaxLength="8000"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="required" ControlToValidate="txtDescription" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row clearfix">
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>VAT</label>
                                            <asp:DropDownList ID="ddlVAT" runat="server" CssClass="form-control" AutoPostBack="true"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator15" runat="server" ErrorMessage="required" ControlToValidate="ddlVAT" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>HSN</label>
                                            <asp:TextBox ID="txtHSN" runat="server" CssClass="form-control"></asp:TextBox>
                                            <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ErrorMessage="required" ControlToValidate="txtHSN" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>--%>
                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender6" runat="server" Enabled="True" TargetControlID="txtHSN" FilterType="UppercaseLetters,LowercaseLetters,Custom" ValidChars="1234567890"></ajaxToolkit:FilteredTextBoxExtender>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Item Type</label>
                                            <asp:DropDownList ID="ddlStoreType" runat="server" CssClass="form-control"></asp:DropDownList>
                                            <asp:RequiredFieldValidator
                                                ID="RequiredFieldValidator13"
                                                runat="server"
                                                ErrorMessage="Please select a store type"
                                                ControlToValidate="ddlStoreType"
                                                InitialValue=""
                                                Display="Dynamic"
                                                CssClass="text-danger">
                                            </asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>UOM</label>
                                            <div class="uom-input-group">
                                                <div class="uom-dropdown-wrapper">
                                                    <asp:DropDownList ID="ddlUnit" CssClass="form-control" runat="server" Height="30px" Font-Size="12px"></asp:DropDownList>
                                                    <asp:RequiredFieldValidator
                                                        ID="RequiredFieldValidator14"
                                                        runat="server"
                                                        ErrorMessage="Please select a UOM"
                                                        ControlToValidate="ddlUnit"
                                                        InitialValue=""
                                                        Display="Dynamic"
                                                        CssClass="text-danger">
                                                    </asp:RequiredFieldValidator>
                                                </div>
                                                <asp:Button ID="btnShowAddUOM" runat="server" Text="+"
                                                    CssClass="add-uom-btn" OnClick="btnShowAddUOM_Click"
                                                    CausesValidation="false" ToolTip="Add New UOM" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                          

                            <%--/************Store Type**********/--%>
                            <div class="row clearfix">
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Current Stock</label>
                                            <asp:TextBox ID="txtStock" runat="server" CssClass="form-control"></asp:TextBox>
                                            <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtStock" ErrorMessage="required" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>--%>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Min Stock</label>
                                            <asp:TextBox ID="txtMinStock" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Max Stock</label>
                                            <asp:TextBox ID="txtMaxStock" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Lead Time</label>
                                            <asp:TextBox ID="txtLeadTime" runat="server" CssClass="form-control"></asp:TextBox>
                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender8" runat="server" Enabled="True" TargetControlID="txtLeadTime" FilterType="Numbers" FilterMode="ValidChars"></ajaxToolkit:FilteredTextBoxExtender>
                                            <asp:TextBox ID="txtRate" runat="server" Text="0" CssClass="form-control" Visible="false"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

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
                            <!--submit & clear-->
                            <div class="row clearfix">
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <label>&nbsp;</label>
                                        <asp:Button ID="btnAdd" runat="server" Text="Create" class="btn btn-primary" OnClick="btnAdd_Click" />
                                    </div>
                                </div>

                                <div class="col-sm-2">
                                    <label>&nbsp;</label>
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" CssClass="btn btn-primary" />
                                </div>
                                <div>
                                    <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btn btn-primary" class="btnUpdate_Click" OnClick="btnUpdate_Click" />
                                    <asp:Button ID="btnPrevious" runat="server" Text=" << " CssClass="btn btn-primary" ToolTip="Previous" OnClick="btnPrevious_Click" Visible="false" />
                                    <asp:Button ID="btnNext" runat="server" Text=" >> " CssClass="btn btn-primary" ToolTip="Next" OnClick="btnNext_Click" Visible="false" />
                                </div>
                            </div>

                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btnAdd" />
                            <asp:PostBackTrigger ControlID="btnUpdate" />
                            <asp:PostBackTrigger ControlID="btnPrevious" />
                            <asp:PostBackTrigger ControlID="btnNext" />
                        </Triggers>

                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>



</asp:Content>

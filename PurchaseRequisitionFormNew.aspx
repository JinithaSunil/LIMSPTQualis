<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="PurchaseRequisitionFormNew.aspx.cs" Inherits="LIMS.PurchaseRequisitionFormNew" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

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
            width: 100%;
        }

        #ddlVAT {
            width: 70%;
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
    </style>
    <style>
        /* =========================================
       APPROVAL PANEL
       ========================================= */

        .approval-panel {
            margin: 20px 0 25px 0;
            padding: 20px 25px;
            border: 1px solid #e5e5e5;
            border-left: 4px solid #b52b7f;
            border-radius: 5px;
            background: #fcf9fd;
        }

        .approval-title {
            font-size: 18px;
            font-weight: 600;
            color: #b52b7f;
            margin-bottom: 5px;
        }

        .approval-description {
            font-size: 13px;
            color: #555;
            margin-bottom: 18px;
        }


        /* =========================================
       HIERARCHY
       ========================================= */

        .approval-hierarchy-title {
            text-align: center;
            font-weight: 600;
            font-size: 14px;
            margin-bottom: 12px;
        }

        .approval-hierarchy {
            display: flex;
            align-items: center;
            justify-content: center;
            flex-wrap: wrap;
            gap: 8px;
            margin-bottom: 20px;
        }

        .approval-level-box {
            min-width: 190px;
            max-width: 240px;
            padding: 12px 15px;
            background: white;
            border: 1px solid #ddd;
            border-radius: 7px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.08);
        }

        .approval-level-number {
            font-size: 13px;
            font-weight: 600;
            color: #333;
            margin-bottom: 5px;
        }

        .approval-level-name {
            font-size: 12px;
            color: #b52b7f;
            font-weight: 500;
        }

        .approval-arrow {
            font-size: 25px;
            color: #777;
            padding: 0 5px;
        }


        /* =========================================
       RADIO BUTTON LIST
       ========================================= */

        .approval-selection {
            border-top: 1px solid #e2e2e2;
            padding-top: 18px;
            margin-top: 10px;
        }

        .approval-selection-title {
            font-size: 14px;
            font-weight: 600;
            margin-bottom: 12px;
            color: #333;
        }

        .required {
            color: red;
        }

        .rblApproval {
            display: flex;
            flex-wrap: wrap;
            gap: 12px 30px;
        }

            .rblApproval input[type="radio"] {
                width: 18px;
                height: 18px;
                margin-right: 7px;
                vertical-align: middle;
                accent-color: #b52b7f;
            }

            .rblApproval label {
                font-size: 13px;
                color: #333;
                cursor: pointer;
                vertical-align: middle;
            }


        /* =========================================
       INFORMATION BOX
       ========================================= */

        .approval-info {
            margin-top: 18px;
            padding: 10px 15px;
            border: 1px solid #b8d4f0;
            background: #f4f9ff;
            color: #315b82;
            border-radius: 4px;
            font-size: 12px;
            display: inline-flex;
            align-items: center;
            gap: 10px;
        }

        .approval-info-icon {
            width: 20px;
            height: 20px;
            border-radius: 50%;
            border: 1px solid #5b9bd5;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
        }


        /* =========================================
       MOBILE
       ========================================= */

        @media (max-width: 768px) {

            .approval-hierarchy {
                display: block;
            }

            .approval-level-box {
                max-width: none;
                margin-bottom: 5px;
            }

            .approval-arrow {
                text-align: center;
                height: 25px;
            }

            .rblApproval {
                display: block;
            }

                .rblApproval label {
                    display: inline-block;
                    margin-bottom: 12px;
                }
        }
    </style>
    <script type="text/javascript">
        function setContextKey(sender, args) {

            // RadioButtonList renders as multiple <input type="radio">
            var rbl = document.getElementsByName('<%= rblStoreType.UniqueID %>');
            var selectedValue = '';

            for (var i = 0; i < rbl.length; i++) {
                if (rbl[i].checked) {
                    selectedValue = rbl[i].value;
                    break;
                }
            }

            sender.set_contextKey(selectedValue);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <h2>PURCHASE REQUISITION</h2>
                </div>
                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <div class="row clearfix">
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <asp:RadioButtonList ID="radLocalOrImport" runat="server" Font-Size="10px" RepeatDirection="Horizontal" CssClass="form-control rbl1">
                                            <asp:ListItem Selected="True">Lokal</asp:ListItem>
                                            <asp:ListItem>Import</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                           
                                <div class="col-sm-10">
                                    <div class="form-group">
                                        <label for="lblStoreType">Item Type</label>
                                        <asp:RadioButtonList ID="rblStoreType" runat="server" Font-Size="10px" AutoPostBack="true" RepeatDirection="Horizontal" CssClass="form-control rbl1">
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                            </div>

                            <asp:DropDownList ID="ddlPRType" runat="server" Visible="false" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlPRType_SelectedIndexChanged">
                            </asp:DropDownList>

                            <asp:DropDownList ID="ddlProject" runat="server" Visible="false" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlProject_SelectedIndexChanged">
                            </asp:DropDownList>

                            <!-- ========================================================= -->
                            <!-- HIGHEST LEVEL OF APPROVAL -->
                            <!-- ========================================================= -->

                            <div class="row clearfix">

                                <div class="col-sm-12">

                                    <div class="approval-panel">

                                        <!-- Heading -->
                                        <div class="approval-title">
                                            HIGHEST LEVEL OF APPROVAL

                                        </div>

                                        <div class="approval-description">
                                            Select the highest organizational level required
                                                        for approval.

                                        </div>


                                        <!-- ================================================= -->
                                        <!-- APPROVAL HIERARCHY DISPLAY -->
                                        <!-- ================================================= -->

                                        <div class="approval-hierarchy-title">
                                            Approval Hierarchy (From Your Position Upwards)

                                        </div>

                                        <div class="approval-hierarchy">

                                            <asp:Repeater
                                                ID="rptApprovalHierarchy"
                                                runat="server">

                                                <ItemTemplate>
                                                    <asp:Panel
                                                        ID="pnlArrow"
                                                        runat="server"
                                                        Visible='<%# Convert.ToInt32(Eval("ApprovalLevel")) > 0 %>'>

                                                        <div class="approval-arrow">
                                                            ↑
                                                        </div>

                                                    </asp:Panel>
                                                    <div class="approval-level-box">

                                                        <div class="approval-level-number">
                                                            <%# Eval("ApprovalLevel").ToString() == "0"
                                                            ? "User"
                                                            : "Level " + Eval("ApprovalLevel") %>
                                                        </div>

                                                        <div class="approval-level-name">
                                                            <%# Eval("Name") %>
                                                        </div>

                                                    </div>



                                                </ItemTemplate>

                                            </asp:Repeater>

                                        </div>


                                        <!-- ================================================= -->
                                        <!-- HIGHEST APPROVAL RADIO BUTTON LIST -->
                                        <!-- ================================================= -->

                                        <div class="approval-selection">

                                            <div class="approval-selection-title">
                                                Select Highest Approval Level

                                                        <span class="required">*</span>
                                            </div>


                                            <asp:RadioButtonList
                                                ID="rblHighestApproval"
                                                runat="server"
                                                RepeatDirection="Horizontal"
                                                RepeatLayout="Flow"
                                                CssClass="rblApproval">
                                            </asp:RadioButtonList>


                                            <asp:RequiredFieldValidator
                                                ID="rfvHighestApproval"
                                                runat="server"
                                                ControlToValidate="rblHighestApproval"
                                                InitialValue=""
                                                ErrorMessage="Please select the highest approval level."
                                                ForeColor="Red"
                                                Display="Dynamic"
                                                ValidationGroup="PRSave">
                                                    </asp:RequiredFieldValidator>

                                        </div>


                                        <!-- ================================================= -->
                                        <!-- INFORMATION -->
                                        <!-- ================================================= -->

                                        <div class="approval-info">

                                            <span class="approval-info-icon">i
                                                    </span>

                                            <span>The approval will be routed from your position
                                                        up to and including the selected level.
                                                    </span>

                                        </div>

                                    </div>

                                </div>

                            </div>

                            <div class="row clearfix">
                                <div class="col-sm-12">
                                    <div id="divGrid" runat="server" class="table-responsive">
                                        <asp:Label ID="lblMessage" runat="server" ForeColor="Green" />
                                        <asp:GridView ID="gvPR" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered" ShowFooter="true"
                                            OnRowDataBound="gvPR_RowDataBound">
                                            <Columns>
                                                <asp:TemplateField HeaderText="#">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSrNo" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Button ID="btnAdd" runat="server" Text="+" CssClass="btn btn-primary" OnClick="btnAdd_Click" />
                                                    </FooterTemplate>
                                                    <FooterStyle HorizontalAlign="Right" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Item Code">
                                                    <HeaderTemplate>
                                                        <asp:HyperLink ID="hypCode" runat="server" Text="Item Code" NavigateUrl="~/CodeList.aspx?Type=PR"></asp:HyperLink>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtItemCode" runat="server" CssClass="form-control" Text='<%# Bind("ItemCode") %>' OnTextChanged="txtCode_TextChanged" AutoPostBack="true">
                                                        </asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Item Description">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control"
                                                            Text='<%# Bind("ItemDescription") %>' ReadOnly="false" OnTextChanged="txtDescription_TextChanged" AutoPostBack="true">
                                                        </asp:TextBox>
                                                        <asp:AutoCompleteExtender
                                                            ID="AutoCompleteExtender2"
                                                            runat="server"
                                                            ServiceMethod="GetItem"
                                                            TargetControlID="txtDescription"
                                                            MinimumPrefixLength="1"
                                                            CompletionInterval="10"
                                                            CompletionSetCount="10"
                                                            EnableCaching="false"
                                                            UseContextKey="true"
                                                            OnClientPopulating="setContextKey">
                                                        </asp:AutoCompleteExtender>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Product Category">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtCategory" runat="server" CssClass="form-control"
                                                            Text='<%# Bind("ProductCategory") %>' ReadOnly="true">
                                                        </asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="UOM">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtUOM" runat="server" CssClass="form-control"
                                                            Text='<%# Bind("UOM") %>' ReadOnly="true">
                                                        </asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Qty">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtQty" runat="server" CssClass="form-control"
                                                            Text='<%# Bind("Qty") %>'>
                                                        </asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="RequiredOn">
                                                    <ItemTemplate>
                                                        <input type="date" id="txtRequiredOn" runat="server" class="form-control" style="width: 90px;"
                                                            value='<%# Bind("RequiredOn") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Stock Available" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtStock" runat="server" CssClass="form-control"
                                                            Text="0" ReadOnly="true">
                                                        </asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Purpose">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtPurpose" runat="server" CssClass="form-control"
                                                            Text='<%# Bind("Purpose") %>'>
                                                        </asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Remarks">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control" Text='<%# Bind("Remarks") %>'></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Button ID="btnRemove" runat="server" Text="X" CssClass="btn btn-danger btn-sm" OnClick="btnRemove_Click" CommandArgument='<%# Container.DataItemIndex %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                            <hr />

                            <h4>Supporting Documents</h4>

                            <div class="row clearfix">

                                <div class="col-md-3">
                                    <asp:DropDownList ID="ddlDocumentType"
                                        runat="server"
                                        CssClass="form-control">
                                        <asp:ListItem Text="Select Document Type" Value="" />
                                        <asp:ListItem>Reference Document</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-3">
                                    <asp:TextBox ID="txtDocDescription"
                                        runat="server"
                                        CssClass="form-control"
                                        placeholder="Description"></asp:TextBox>
                                </div>

                                <div class="col-md-4">
                                    <asp:FileUpload ID="fuDocument" runat="server" CssClass="form-control" />
                                </div>

                                <div class="col-md-2">
                                    <asp:Button ID="btnUpload"
                                        runat="server"
                                        Text="Upload"
                                        CssClass="btn btn-primary"
                                        OnClick="btnUpload_Click" />
                                </div>

                            </div>

                            <br />

                            <asp:GridView ID="gvDocuments"
                                runat="server"
                                CssClass="table table-bordered"
                                AutoGenerateColumns="false">

                                <Columns>

                                    <asp:BoundField DataField="DocumentType" HeaderText="Type" />

                                    <asp:BoundField DataField="Description" HeaderText="Description" />

                                    <asp:BoundField DataField="FileName" HeaderText="File Name" />

                                    <asp:BoundField DataField="Size" HeaderText="Size (KB)" />

                                    <asp:TemplateField HeaderText="Delete">
                                        <ItemTemplate>
                                            <asp:Button ID="btnDeleteDoc"
                                                runat="server"
                                                Text="Delete"
                                                CssClass="btn btn-danger btn-sm"
                                                CommandArgument='<%# Container.DataItemIndex %>'
                                                OnClick="btnDeleteDoc_Click" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                </Columns>

                            </asp:GridView>
                            <div class="row clearfix mt-3">
                                <div class="col-sm-12">
                                    <div class="form-group">
                                        <asp:Button ID="BtnSave" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                                        <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-warning" OnClick="btnClear_Click" />
                                    </div>
                                </div>
                            </div>


                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btnUpload" />
                        </Triggers>
                    </asp:UpdatePanel>

                </div>
            </div>
        </div>
    </div>
</asp:Content>

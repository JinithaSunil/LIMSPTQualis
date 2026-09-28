<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="POMasterList.aspx.cs" Inherits="LIMS.POMasterList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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

        .card {
            margin-bottom: 30px;
            border: none;
            border-radius: 5px;
            box-shadow: 0 0.46875rem 2.1875rem rgba(90,97,105,0.1), 0 0.9375rem 1.40625rem rgba(90,97,105,0.1), 0 0.25rem 0.53125rem rgba(90,97,105,0.12), 0 0.125rem 0.1875rem rgba(90,97,105,0.1);
        }

        .violet {
            border-top: 3px solid #9c27b0;
            margin: 0;
        }

        .header {
            padding: 20px;
            margin-bottom: 20px;
        }

            .header h2 {
                margin: 0;
                color: #444;
                font-size: 24px;
            }

        .body {
            padding: 20px;
        }

        .btn-group {
            margin-bottom: 20px;
            margin-right: 10px;
        }

        .btn-secondary {
            color: #fff;
            background-color: #6c757d;
            border-color: #6c757d;
            margin-right: 5px;
        }

            .btn-secondary:hover {
                background-color: #5a6268;
                border-color: #545b62;
            }

        .btn-primary.btn-xs {
            padding: 1px 5px;
            font-size: 12px;
            line-height: 1.5;
            border-radius: 3px;
            background-color: #2196F3;
            border-color: #2196F3;
        }

        .btn-danger.btn-xs {
            padding: 1px 5px;
            font-size: 12px;
            line-height: 1.5;
            border-radius: 3px;
        }

        .table {
            margin-bottom: 0;
        }

        .table-hover tbody tr:hover {
            background-color: rgba(0,0,0,.075);
        }

        .table-responsive {
            min-height: .01%;
            overflow-x: auto;
        }

        .hyperlink-column a {
            color: #b34788;
            text-decoration: none;
        }

            .hyperlink-column a:hover {
                text-decoration: underline;
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
                    <h2>PO APPROVAL LIST</h2>
                </div>
                <div class="body">
                     <div class="row">
     <div class="col-4">
         <asp:RadioButtonList ID="radChoice" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="radChoice_SelectedIndexChanged" AutoPostBack="true">
             <asp:ListItem Value="0" Selected="True">PendingApproval</asp:ListItem>
             <asp:ListItem Value="1">Approved PO-Pending Delivery</asp:ListItem>
         </asp:RadioButtonList>
     </div>
 </div>
                    <div class="row clearfix">
                        <div class="col-12">
                            <div class="table-responsive">
                                <asp:GridView ID="gvPO" runat="server" class="table table-bordered table-striped table-hover" DataKeyNames="PurchaseOrderID"
                                    AutoGenerateColumns="false" OnRowCommand="gvPO_RowCommand" OnRowDataBound="gvPO_RowDataBound">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sl#">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle Width="1%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Enq#" ItemStyle-CssClass="hyperlink-column">
                                            <ItemTemplate>
                                                <asp:HyperLink ID="hypEnquiryID" runat="server" NavigateUrl='<%# Eval("EnquiryID","QuoteAnalysis.aspx?ID={0}") %>' Target="_blank" Text='<%# Eval("EnquiryID").ToString().ToUpper() %>'></asp:HyperLink>
                                            </ItemTemplate>
                                            <ItemStyle Width="2%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="PO#" ItemStyle-CssClass="hyperlink-column">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPONumber" runat="server" Text='<%# Eval("PONumber").ToString() %>' Visible="false"></asp:Label>
                                                <asp:Label ID="lblReportFileName" runat="server" Text='<%# Eval("ReportFileName").ToString() %>' Visible="false"></asp:Label>
                                                <asp:HyperLink ID="hypPO" runat="server" Text='<%# Eval("PONumber").ToString() %>' Target="_blank" NavigateUrl='<%# Eval("PurchaseOrderID","GeneratePO.aspx?POID={0}") %>'></asp:HyperLink>
                                                <asp:Label ID="lblPOStatusID" runat="server" Text='<%# Eval("POStatusID").ToString() %>' Visible="false"></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="10%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="PO Date" ItemStyle-CssClass="hyperlink-column">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPODate" runat="server"  Text='<%# Eval("PODate","{0:dd MMM yyyy}").ToString() %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="8%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Vendor Name">
                                            <ItemTemplate>
                                                <asp:Label ID="lblVendorName" runat="server" Text='<%# Eval("VendorName").ToString() %>'></asp:Label>
                                                <asp:Label ID="lblVendorID" runat="server" Text='<%# Eval("VendorID").ToString() %>' Visible="false"></asp:Label>
                                                <asp:Label ID="lblStoreType" runat="server" Text='<%# Eval("StoreType").ToString() %>' Visible="false"></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="10%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Special Instruction">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSpecialInstruction" runat="server" Text='<%# Eval("SpecialInstructions").ToString() %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="10%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Remarks">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control"></asp:TextBox>
                                            </ItemTemplate>
                                            <ItemStyle Width="8%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Approve">
                                            <ItemTemplate>
                                                <asp:Button ID="btnApprove" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' CommandName="Approve" Text="Approve" Font-Size="Small" runat="server" class="btn btn-success btn-xs" Height="30px" />
                                            </ItemTemplate>
                                            <ItemStyle Width="3%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Reject">
                                            <ItemTemplate>
                                                <div class="row">
                                                    <div class="col-12">
                                                        <asp:Button ID="btnReject" CommandArgument='<%# DataBinder.Eval(Container,"RowIndex") %>' CommandName="Reject" Text="Reject" Font-Size="Small" runat="server" class="btn btn-danger btn-xs" Height="30px" /><br />
                                                        <br />
                                                        <asp:DropDownList ID="ddlRevertType" runat="server" CssClass="form-control" Height="35px" OnSelectedIndexChanged="ddlRevertType_SelectedIndexChanged" AutoPostBack="true">
                                                            <asp:ListItem Value="0">Select Type</asp:ListItem>
                                                            <asp:ListItem Value="3">Amend Terms</asp:ListItem>
                                                            <asp:ListItem Value="2">Float Enuiry</asp:ListItem>
                                                            <asp:ListItem Value="1">Quote Received</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                            <ItemStyle Width="3%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Accept Reason">
                                            <ItemTemplate>
                                                <asp:Label ID="lblAcceptReason" runat="server"></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="8%"></ItemStyle>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                    
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" Async="true" AutoEventWireup="true" CodeBehind="AccountsReceivable.aspx.cs" Inherits="LIMS.AccountsReceivable" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   
     <style>

        .cb label {
            height: 43px;
        }

        .rbWidth label {
            height: 40px;
        }

        [type="radio"]:not(:checked) + label {
            padding-left: 26px;
            height: 43px;
            line-height: 25px;
            font-size: 13px;
            font-weight: normal;
        }

        [type="radio"]:checked + label {
            padding-left: 26px;
            height: 43px;
            line-height: 25px;
            font-size: 13px;
            font-weight: normal;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <!-- Basic Examples -->
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-12">
                            <h2>ACCOUNTS RECEIVABLE</h2>
                        </div>
                    </div>
                    <br />
                </div>
                <div class="body">
                    <div class="row">
                        <div class="col-sm-12">
                            <%--                            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <fieldset>--%>
                            <div class="row">
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <b>Customers</b>
                                        <div class="form-line">
                                            <asp:DropDownList ID="ddlCustomers" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlCustomers_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <b>Invoice List</b>
                                            <%--<asp:DropDownList ID="ddlInvoice" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlInvoice_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>--%>
                                            <asp:CheckBoxList ID="chkInvoice" runat="server" CssClass="cb" OnSelectedIndexChanged="chkInvoice_SelectedIndexChanged" AutoPostBack="true"></asp:CheckBoxList>
                                        <asp:HiddenField ID="hdInvoiceNumber" runat="server" />
                                        </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <b>Currency</b>
                                        <div class="form-line">
                                            <asp:DropDownList ID="ddlCurrency" runat="server" CssClass="form-control">
                                                <asp:ListItem>IDR</asp:ListItem>
                                                <asp:ListItem>USD</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <b>Amount</b>
                                        <div class="form-line">
                                            <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" Placeholder="Amount"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <b>Invoice Date</b>
                                        <div class="form-line">
                                            <asp:TextBox ID="txtInvoiceDate" runat="server" CssClass="form-control" Placeholder="yyyy-mm-dd" TextMode="Date"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <b>Due Date</b>
                                        <div class="form-line">
                                            <asp:TextBox ID="txtDueDate" runat="server" CssClass="form-control" Placeholder="yyyy-mm-dd" TextMode="Date"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>


                                <div class="col-sm-3">
                                    <asp:Button ID="btnAddInvoice" runat="server" CssClass="btn btn-primary" Text="Add Invoice" OnClick="btnAddInvoice_Click" />
                                </div>
                            </div>
                            <asp:RadioButtonList ID="radFilter" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="radFilter_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem Value="1" Selected="True">Invoiced - Unpaid</asp:ListItem>
                                <asp:ListItem Value="2">Invoiced - Paid</asp:ListItem>
                                <asp:ListItem Value="3">UnInvoiced</asp:ListItem>
                            </asp:RadioButtonList>
                            <asp:GridView ID="gvInvoices" runat="server" OnRowDataBound="gvInvoices_RowDataBound" OnRowCommand="gvInvoices_RowCommand"
                                DataKeyNames="InvoiceARId" class="table table-bordered table-striped table-hover js-basic-example dataTable" AllowSorting="true" OnSorting="gvInvoices_Sorting"
                                AutoGenerateColumns="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="Sl.No">
                                        <ItemTemplate>
                                            <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                            <%--<asp:Label ID="lblInvoiceID" runat="server" Text='<%#Eval("InvoiceID") %>' Visible="false"></asp:Label>
                                            <asp:Label ID="lblCustomerID" runat="server" Text='<%#Eval("CompanyID") %>' Visible="false"></asp:Label>--%>
                                        </ItemTemplate>
                                        <ItemStyle Width="10%"></ItemStyle>
                                    </asp:TemplateField>
                                   <%-- <asp:TemplateField HeaderText="InvoiceDate" SortExpression="TotalAmount">
                                        <ItemTemplate>
                                            <asp:Label ID="lblInvoiceDate" runat="server" Text='<%#Eval("InvoiceDate", "{0:dd MMM yyyy}") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="15%" />
                                    </asp:TemplateField>--%>
                                    <asp:BoundField DataField="CompanyName" HeaderText="Customer Name" SortExpression="CompanyName" ItemStyle-Width="20%" />
                                    <asp:TemplateField HeaderText="Invoice#" SortExpression="InvoiceNumber">
                                        <ItemTemplate>
                                            <asp:Label ID="lblInvoiceID" runat="server" Visible="false" Text='<%#Eval("InvoiceID") %>'></asp:Label>
                                            <asp:Label ID="lblInvoiceNumber" runat="server"></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="20%" />
                                    </asp:TemplateField>
                                   
                                    <asp:TemplateField HeaderText="TotalAmount" SortExpression="TotalAmount">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTotalAmount" runat="server" Text='<%#Eval("TotalAmount") %>'></asp:Label>
                                            
                                        </ItemTemplate>
                                        <ItemStyle Width="15%" />
                                    </asp:TemplateField>
                                   <%-- <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Center" SortExpression="Status">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkStatus" runat="server" CssClass="badge bg-primary"
                                                Text='<%# Eval("Status") == DBNull.Value ? "UnInvoiced" : "Invoiced" %>'
                                                Style='<%# Eval("Status") == DBNull.Value ? "background-color:red;color:white": "background-color:green;color:white" %>'
                                                CommandName="SelectInvoice" CommandArgument='<%# Eval("InvoiceID") %>'>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Width="30px"></ItemStyle>
                                    </asp:TemplateField>--%>
                                    <asp:TemplateField HeaderText="Paid Amt." ItemStyle-Width="20%" ItemStyle-Font-Bold="true">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkPayment" runat="server"
                                                Text='<%#Eval("AmountPaid") %>' CommandName="Payments" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>'>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Width="30px"></ItemStyle>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:HiddenField ID="hdInvoiceID" runat="server" />
                            <asp:HiddenField ID="hdAmountPaid" runat="server" />
                            <asp:Button ID="btnReceipt" runat="server" Text="Receive Payment" CssClass="btn btn-primary" OnClick="btnReceipt_Click" />
                            <br />
                            <%--   </fieldset>
                                </ContentTemplate>
                            </asp:UpdatePanel>--%>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
</asp:Content>

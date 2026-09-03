<%--<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="QuotationList.aspx.cs" Inherits="LIMS.QuotationList" %>--%>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ManagementReportList.aspx.cs" Inherits="LIMS.ManagementReportList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />

    <%-- <link href="css/all.main.css" rel="stylesheet" />--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <!-- Basic Examples -->
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-6">
                            <h2>MANAGEMENT REPORT LIST</h2>
                        </div>
                        <div class="col-sm-4">
                            <asp:TextBox ID="txtSearch" runat="server" class="form-control" placeholder="Search Quotation" AutoPostBack="true"></asp:TextBox>
                        </div>
                        <div class="col-sm-1">
                            <asp:ImageButton ID="imgSearch" runat="server" ImageUrl="images/search.png" />
                        </div>
                    </div>

                </div>
                <div class="body">
                    <div class="table-responsive">
                        <asp:Panel ID="pnlView" runat="server">
                            <asp:RadioButtonList ID="radInv" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="radInv_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem Selected="True">All</asp:ListItem>
                                <asp:ListItem>UnInvoiced</asp:ListItem>
                                <asp:ListItem>Invoiced</asp:ListItem>
                            </asp:RadioButtonList>
                            <br />
                            Records:
                            <asp:Label ID="lblRecords" runat="server" Text="0"></asp:Label>
                            <br />
                            <br />
                            <asp:GridView ID="gvQuotation"
                                runat="server"
                                AutoGenerateColumns="false"
                                class="table table-bordered table-striped table-hover js-basic-example dataTable"
                                AllowPaging="true"
                                PageSize="10"
                                DataKeyNames="EnquiryId"
                                OnPageIndexChanging="gvQuotation_PageIndexChanging"
                                OnRowCommand="gvQuotation_RowCommand"
                                OnRowDataBound="gvQuotation_RowDataBound">

                                <Columns>
                                    <asp:TemplateField HeaderText="Request No." SortExpression="RequestNo">
                                        <ItemTemplate>
                                            <asp:Label ID="lblEnquiryId" runat="server"
                                                Text='<%# Eval("EnquiryId") == DBNull.Value ? "" : "R/" + Eval("RequestNo").ToString() %>'>
                                            </asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="5%"></ItemStyle>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Quotation Number">
                                        <HeaderTemplate>
                                            Quotation#<br />
                                            <asp:TextBox ID="txtQuotationNumber" runat="server" OnTextChanged="txtQuotationNumber_TextChanged" AutoPostBack="true" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:HyperLink
                                                ID="hypInquiry"
                                                runat="server"
                                                Text='<%# Eval("QuotationNumber").ToString().ToUpper() %>'
                                                Target="_blank"
                                                NavigateUrl='<%# Eval("QuotationMasterID", "GenerateQuotation.aspx?ID={0}&r=" + Eval("RevisionCount")) %>'>
                                            </asp:HyperLink>
                                        </ItemTemplate>
                                        <ItemStyle Width="20%"></ItemStyle>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Customer" SortExpression="CustomerName">
                                        <HeaderTemplate>
                                            Customer<br />
                                            <asp:TextBox ID="txtCustomerName" runat="server" OnTextChanged="txtCustomerName_TextChanged" AutoPostBack="true" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblCustomerName" runat="server" Text='<%# Eval("CustomerName").ToString().ToUpper()  %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="20%"></ItemStyle>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Total Amount" SortExpression="GrandTotal">
                                        <ItemTemplate>
                                            <asp:Label ID="lblGrandTotal" runat="server" Text='<%# Eval("GrandTotal").ToString()  %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="15%"></ItemStyle>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Date" SortExpression="FormattedDate">
                                        <HeaderTemplate>
                                            Date<br />
                                            <asp:TextBox ID="txtDate" runat="server" TextMode="Date" OnTextChanged="txtDate_TextChanged" AutoPostBack="true" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblformattedDate" runat="server" Text='<%# Eval("FormattedDate").ToString()  %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="10%"></ItemStyle>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Status" SortExpression="QuotationStatus">
                                        <HeaderTemplate>
                                            QuotationStatus<br />
                                            <asp:TextBox ID="txtQuotationStatus" runat="server" OnTextChanged="txtQuotationStatus_TextChanged" AutoPostBack="true" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblQuotationStatus" runat="server" Text='<%# Eval("QuotationStatus").ToString().ToUpper()  %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="10%"></ItemStyle>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Action">
                                        <HeaderTemplate>
                                            Action
                                            <br />
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:LinkButton ID="btnExport" runat="server" ToolTip="Download"
                                                            OnClick="btnExport_Click" CssClass="btn btn-success">
                                                            <i class="ri-arrow-down-line"></i>
                                                        </asp:LinkButton>
                                                    </td>
                                                    <td>
                                                        <asp:LinkButton ID="btnClear" runat="server" ToolTip="Clear Fliters"
                                                            CommandName="ClearFilter" CssClass="btn btn-primary btn-sm">
                                                            <i class="fa fa-square-xmark"></i>
                                                        </asp:LinkButton>
                                                    </td>
                                                </tr>
                                            </table>


                                            <asp:Button ID="btnFilter" runat="server" Text="Filter" Visible="false"
                                                CommandName="Filter" CssClass="btn btn-primary btn-sm" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                          
                                            <asp:LinkButton ID="lnkMRgenerate" runat="server" CommandName="MRGenerate" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'
                                                ToolTip="Generate MR">
                                                <i class="fa fa-file-text-o" style="font-size:medium; margin-right: 12px;"></i>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Width="6%"></ItemStyle>
                                    </asp:TemplateField>
                                </Columns>

                                <PagerStyle BackColor="#b34788" ForeColor="White" HorizontalAlign="Center" />
                                <FooterStyle BackColor="White" ForeColor="#333333" />
                                <RowStyle BackColor="White" ForeColor="#333333" Height="50px" HorizontalAlign="Left" VerticalAlign="Middle" />
                                <SelectedRowStyle BackColor="#339966" ForeColor="White" />
                                <SortedAscendingCellStyle BackColor="#F7F7F7" />
                                <SortedAscendingHeaderStyle BackColor="#487575" />
                                <SortedDescendingCellStyle BackColor="#E5E5E5" />
                                <SortedDescendingHeaderStyle BackColor="#275353" />
                            </asp:GridView>

                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

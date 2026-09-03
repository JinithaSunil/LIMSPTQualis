<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeBehind="QuotationList.aspx.cs" Inherits="LIMS.QuotationList" %>

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
                            <h2>QUOTATION LIST</h2>
                        </div>
                        <div class="col-sm-4">
                            <asp:TextBox ID="txtSearch" runat="server" class="form-control" OnTextChanged="txtSearch_TextChanged" placeholder="Search Quotation" AutoPostBack="true"></asp:TextBox>
                        </div>
                        <div class="col-sm-1">
                            <asp:ImageButton ID="imgSearch" runat="server" ImageUrl="images/search.png" OnClick="imgSearch_Click" />
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
                                DataKeyNames="QuotationMasterID,RevisionCount"
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
                                            <asp:HiddenField ID="hdQuotationMasterID" runat="server" Value='<%#Eval("QuotationMasterID") %>' />
                                            <asp:HiddenField ID="hdRevisionCount" runat="server" Value='<%#Eval("RevisionCount") %>' />
                                            <asp:HiddenField ID="hdDepartmentID" runat="server" Value='<%#Eval("DepartmentID") %>' />
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
                                            <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit"
                                                CommandArgument='<%# Eval("QuotationMasterID") %>'
                                                ToolTip="Edit">
                                                <i class="fas fa-edit" style="font-size:medium; margin-right: 12px;"></i>  
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="lnkSendQuote" runat="server" CommandName="SendQuote" CommandArgument='<%# Eval("QuotationMasterID") %>'
                                                ToolTip="Send Quote">
                                                <i class="fa-solid fa-paper-plane" style="font-size:medium; margin-right: 12px;"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="lnkAcceptQuote" runat="server" CommandName="QuoteAccepted" CommandArgument='<%# Eval("QuotationMasterID") %>'
                                                ToolTip="Quote Accepted">
                                                <i class="fa-regular fa-square-check" style="font-size:medium; margin-right: 12px;"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="lnkRejectQuote" runat="server" CommandName="QuoteRejected" CommandArgument='<%# Eval("QuotationMasterID") %>'
                                                ToolTip="Quote Rejected">
                                                <i class="fa fa-square-xmark" style="font-size:medium; margin-right: 12px;"></i>
                                            </asp:LinkButton>

                                            <!-- PO Controls Start -->
                                            <asp:FileUpload ID="fpPO" runat="server" Width="70%" CssClass="btn btn-primary" />
                                            <asp:TextBox ID="txtPONo" runat="server" CssClass="form-control" placeholder="PO#"></asp:TextBox>
                                            <asp:TextBox ID="txtPODate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                            <asp:Button ID="btnUploadRow" runat="server" Text="Upload PO"
                                                CommandName="POReceived"
                                                CommandArgument='<%# Eval("QuotationMasterID") %>'
                                                CssClass="btn btn-success btn-sm" /><br /><br />
                                            <!-- PO Controls End -->

                                            <asp:LinkButton ID="lnkSOgenerate" runat="server" CommandName="SalesOrderGenerate" CommandArgument='<%# Eval("QuotationMasterID") %>'
                                                ToolTip="SO Generate">
                                                <i class="fa fa-file-text-o" style="font-size:medium; margin-right: 12px;"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="lnkProformaInvoice" runat="server" Visible="false" CommandName="ProformaInvoice" CommandArgument='<%# Eval("QuotationMasterID") %>'
                                                ToolTip="Generate PI">
                                                <i class="fa fa-file-lines" style="font-size:medium; margin-right: 12px;"></i> 
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="lnkInvoice" runat="server" CommandName="InvoiceGenerate" CommandArgument='<%# Eval("QuotationMasterID") %>'
                                                ToolTip="Generate Invoice">
                                                <i class="fa fa-file-lines" style="font-size:medium; margin-right: 12px;"></i> 
                                            </asp:LinkButton>

                                            <asp:LinkButton ID="lnkCoverLetter" runat="server" CommandName="CoverLetter" Visible="false" CommandArgument='<%# Eval("QuotationMasterID") %>'
                                                ToolTip="Cover Letter">
                                                <i class="fa fa-eye" style="font-size:medium; margin-right: 12px;"></i> 
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Width="6%"></ItemStyle>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="View">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkShowSO" runat="server" CommandName="SOShow" CommandArgument='<%# Eval("QuotationMasterID") %>'
                                                ToolTip="SO Show">🛒
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="lnkShowInvoice" runat="server" Visible="false" CommandName="InvoiceShow" CommandArgument='<%# Eval("QuotationMasterID") %>'
                                                ToolTip="Invoice Show">🧾
                                                <i class="fa fa-eye" style="font-size:medium; margin-right: 12px;"></i> 
                                            </asp:LinkButton>
                                        </ItemTemplate>
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
                            <asp:Label ID="lblNoRecords" runat="server" Text="No Data!" Font-Bold="True" Font-Size="X-Large" ForeColor="Red" Visible="False"></asp:Label>

                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

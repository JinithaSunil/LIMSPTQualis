<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" EnableEventValidation="false" AutoEventWireup="true" CodeBehind="CustomerInquiryView.aspx.cs" Inherits="LIMS.CustomerInquiryView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="http://www.jqueryscript.net/css/jquerysctipttop.css" rel="stylesheet" type="text/css">
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
        <ContentTemplate>
            <fieldset>
                <!-- Basic Examples -->
                <div class="row clearfix">
                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <div class="card">
                            <hr class="violet">
                            <div class="header">


                                <div class="row">
                                    <div class="col-sm-6">
                                        <h2>ENQUIRY LIST</h2>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:TextBox ID="txtSearch" runat="server" class="form-control" placeholder="Search Customer" OnTextChanged="txtSearch_TextChanged" AutoPostBack="true"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-1">
                                        <asp:ImageButton ID="imgSearch" runat="server" ImageUrl="images/search.png" OnClick="imgSearch_Click" />
                                    </div>
                                </div>

                            </div>
                            <div class="body">
                                <asp:RadioButtonList ID="radOption" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="radOption_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Selected="True">Lab</asp:ListItem>
                                    <asp:ListItem>Certification</asp:ListItem>
                                </asp:RadioButtonList>
                                <div class="table-responsive">
                                    <asp:Panel ID="pnlView" runat="server">
                                        <asp:GridView ID="gvInquiries" runat="server" AutoGenerateColumns="False" Width="100%"
                                            DataKeyNames="CustomerMasterID,InquiryID" class="table table-bordered table-striped table-hover js-basic-example dataTable"
                                            OnPageIndexChanging="gvInquiries_PageIndexChanging" AllowPaging="true" PageSize="5"
                                            OnRowDataBound="gvInquiries_RowDataBound" AllowSorting="true" OnSorting="gvInquiries_Sorting"
                                            OnRowCommand="gvInquiries_RowCommand">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Request No." ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <asp:LinkButton ID="lnkSortValue1" runat="server"
                                                            Text="Request#"
                                                            CommandName="Sort"
                                                            CommandArgument="RequestNo" />
                                                        |
                                                            <asp:LinkButton ID="lnkSortValue2" runat="server"
                                                                Text="Division"
                                                                CommandName="Sort"
                                                                CommandArgument="DepartmentName" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:HyperLink ID="hypInquiry" runat="server" Text='<%# "R/" + Eval("RequestNo").ToString().ToUpper() %>' Target="_blank" NavigateUrl='<%# Eval("InquiryID", "EnquiryStatus.aspx?ID={0}") %>'></asp:HyperLink>
                                                        <asp:Label ID="lblDivision" runat="server" Text='<%#Eval("DepartmentName") %>'></asp:Label>
                                                        <asp:Label ID="lblReportFormat" Text='<%# Eval("ReportFormat") %>' runat="server" Visible="false" />
                                                        <asp:HiddenField ID="hdQuotationStatus" runat="server" Value='<%#Eval("QuotationStatus") %>' />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="5%"></ItemStyle>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="InquiryID" Visible="false" />
                                                <asp:TemplateField HeaderText="Customer" SortExpression="CustomerName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCustomer" runat="server" Text='<%# Eval("CustomerName").ToString().ToUpper()  %>' Target="_blank" NavigateUrl='<%# Eval("CustomerMasterID", "CustomerProfile.aspx?ID={0}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="20%"></ItemStyle>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Inquiry Details">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblInquiryDetail" runat="server" Text='<%# Limit(System.Web.HttpUtility.HtmlDecode(Eval("InquiryDetail").ToString()),40) %>' ToolTip='<%# System.Web.HttpUtility.HtmlDecode(Eval("InquiryDetail").ToString()) %>'></asp:Label>
                                                        <asp:LinkButton ID="ReadMoreLinkButton" runat="server"
                                                            Text="Read More"
                                                            Visible='<%# SetVisibility(Eval("InquiryDetail"), 40) %>'
                                                            OnClick="ReadMoreLinkButton_Click">
                                                        </asp:LinkButton>
                                                        <asp:HiddenField ID="hdEnquiryDept" runat="server" Value='<%#Eval("EnquiryDept") %>' />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="20%"></ItemStyle>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Created" SortExpression="Created">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCreated" runat="server" Text='<%# Eval("Created", "{0:dd MMM yyyy}")  %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="18%"></ItemStyle>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="TAF/CRA" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <span>
                                                            <asp:Button
                                                                ID="btnTAF"
                                                                runat="server"
                                                                Text="TAF"
                                                                CssClass="btn btn-primary" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'
                                                                CommandName="TAF" />
                                                            <br />
                                                            <asp:HyperLink ID="hypCRF" runat="server" Target="_blank" Visible="false"></asp:HyperLink><br />
                                                            <asp:HyperLink ID="hypCRA" runat="server" Target="_blank" Visible="false"></asp:HyperLink><br />
                                                        </span>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="20%"></ItemStyle>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <span>
                                                            <asp:Button ID="btnQuotationGenerate" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' CommandName="QuotationGenerate" Text="Quotation Generate" runat="server" class="btn btn-primary btn-xs" Height="30px" Visible="false" />
                                                            <asp:DropDownList ID="ddlCRFList" runat="server" Width="80%" Height="30px" Visible="false"></asp:DropDownList>
                                                            <asp:Button ID="btnSendCRF" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' CommandName="SendCRF" Text="Send TAF" runat="server" class="btn btn-primary btn-xs" Height="30px" Visible="false" />
                                                            <asp:Button ID="btnCloneCRF" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' CommandName="CloneCRF" Text="Clone TAF" runat="server" class="btn btn-primary btn-xs" Height="30px" Visible="false" />
                                                            <asp:Button ID="btnAcceptCRA" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' CommandName="AcceptCRA" Text="Accept CRA" runat="server" CssClass="btn btn-primary btn-xs" Height="30px" Visible="false" />
                                                        </span>
                                                        <asp:PlaceHolder
                                                            ID="PlaceHolderCRFCloneList"
                                                            runat="server"></asp:PlaceHolder>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="20%"></ItemStyle>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Enquiry Status" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStatusName" Text='<%# Eval("StatusName") %>' runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <FooterStyle BackColor="White" ForeColor="#333333" />
                                            <PagerStyle BackColor="#b34788" ForeColor="White" HorizontalAlign="Center" />
                                            <RowStyle BackColor="White" ForeColor="#333333" Height="50px" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <SelectedRowStyle BackColor="#339966" ForeColor="White" />
                                            <SortedAscendingCellStyle BackColor="#F7F7F7" />
                                            <SortedAscendingHeaderStyle BackColor="#487575" />
                                            <SortedDescendingCellStyle BackColor="#E5E5E5" />
                                            <SortedDescendingHeaderStyle BackColor="#275353" />
                                        </asp:GridView>
                                        <asp:Label ID="lblNoRecords" runat="server" Text="No Data!" Font-Bold="True" Font-Size="X-Large" ForeColor="Red" Visible="False"></asp:Label>
                                        <br />
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="gvInquiries" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

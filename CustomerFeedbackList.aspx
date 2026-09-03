<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="CustomerFeedbackList.aspx.cs" Inherits="LIMS.CustomerFeedbackList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <!-- Task Info -->
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <div class="card"><hr class="violet"><div class="header">
                        
                            <h2>CUSTOMER FEEDBACK LIST</h2>                          
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                   <asp:GridView ID="gvCustomerFeedbackList" runat="server" AutoGenerateColumns="false" class="table table-bordered table-striped table-hover js-basic-example dataTable" BorderWidth="1px" CellPadding="4" Width="100%" OnRowDataBound="gvCustomerFeedbackList_RowDataBound" DataKeyNames="InquiryID">
                                    <Columns>
                                        <asp:TemplateField HeaderText="#" ItemStyle-Width="100%">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle Width="5%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="CustomerName" HeaderText="Customer" />
                                        <asp:TemplateField HeaderText="Request No.">
                                            <ItemTemplate>
                                            <asp:HyperLink ID="hypInquiry" runat="server" Text='<%# "R/" + Eval("RequestNo").ToString().ToUpper() %>' Target="_blank" NavigateUrl='<%# Eval("InquiryID", "CustomerFeedbackStatus.aspx?ID={0}") %>'></asp:HyperLink>
                                        </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Overall Satifaction">
                                            <ItemTemplate>
                                                <asp:Label ID="lblfeedbackweightage" Text='<%# Eval("average") %>' runat="server" /><span>%</span>
                                                <div class="progress">
                                                    <div id="divProgress" runat="server" class="progress-bar bg-blue" role="progressbar" aria-valuenow="80" aria-valuemin="0" aria-valuemax="100" style="width: 80%"></div>
                                                </div>
                                            </ItemTemplate>
                                            <ItemStyle Width="25%"></ItemStyle>
                                        </asp:TemplateField>
                                    </Columns>

                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- #END# Task Info -->
</asp:Content>

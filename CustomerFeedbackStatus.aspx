<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="CustomerFeedbackStatus.aspx.cs" Inherits="LIMS.CustomerFeedbackStatus" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="row clearfix">
        <!-- Task Info -->
        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="card"><hr class="violet"><div class="header">
                
                    <h2>CUSTOMER FEEDBACK</h2>
                </div>
                <div class="body">
                    <div class="table-responsive">
                        <div class="table table-hover dashboard-task-infos">
                            <div class="row clearfix" style="text-align:center; padding-left:5%;padding-top:2%">    
                                   <asp:ListView ID="LstDetails" runat="server">
                                            <ItemTemplate>
                                            <div class="col-lg-5 col-md-5 col-sm-5 col-xs-12">
                                                <div class="card"><hr class="violet"><div class="header">
                                                    
                                                        <h2>
                                                            <asp:Label ID="lblContactName" runat="server" Text='<%# Eval("CustomerName") %>' Font-Bold="True" Font-Size="Large"></asp:Label>
                                                        </h2>                                                       
                                                    </div>
                                                    <div class="body" style="text-align:left">
                                                        EnquiryID: <asp:Label ID="lblEnquiryID" runat="server" Text='<%# "R/" + Eval("RequestNo") %>' Font-Bold="True" ></asp:Label><br /><br />
                                                        Department: <asp:Label ID="lblDepartment" runat="server" Text='<%# Eval("DepartmentName") %>' Font-Bold="True" ></asp:Label><br /><br />
                                                        Category: <asp:Label ID="lblCategory" runat="server" Text='<%# Eval("ProductGroupName") %>' Font-Bold="True" ></asp:Label><br /><br />
                                                        Sub Category: <asp:Label ID="lblSubCategory" runat="server" Text='<%# Eval("SubGroupName") %>' Font-Bold="True" ></asp:Label><br /><br />
                                                        Lab Head:   <asp:Label ID="lblSupervisor" runat="server" Text='<%# Eval("SupervisorName") %>' Font-Bold="True" ></asp:Label><br /><br />
                                                        Lab Analyst:   <asp:Label ID="lblAnalyst" runat="server" Text='<%# Eval("EmployeeName") %>' Font-Bold="True" ></asp:Label><br /><br />
                                                        Enquiry Received: <asp:Label ID="lblEnquiryDate" runat="server" Text='<%# Eval("EnquiryDate", "{0:dd MMM yyyy}") %>' Font-Bold="True" ></asp:Label><br /><br />
                                                        Report Generated: <asp:Label ID="lblReportDate" runat="server" Text='<%# Eval("ReportDate", "{0:dd MMM yyyy}") %>' Font-Bold="True" ></asp:Label><br /><br />
                                                    </div>
                                                </div>
                                        </div>   
                                        </ItemTemplate>
                                        </asp:ListView>     
                                 <div class="col-lg-5 col-md-5 col-sm-5 col-xs-12">
                                                <div class="card"><hr class="violet"><div class="header">
                                                    
                                                        <h2>
                                                            Customer Satisfaction
                                                        </h2>                                                       
                                                    </div>
                                                    <div class="body">
                                    <asp:TextBox ID="txtKnob" runat="server" class="knob" data-linecap="round" value="0" data-width="125" data-height="125" data-thickness="0.25" ReadOnly="true"
                                        data-fgcolor="#F44336"></asp:TextBox>
                                    <asp:Label ID="lblAverage" runat="server" Font-Size="14px"></asp:Label>
                                </div>
                                                </div>
                                    </div>
                            </div>
                            <div class="row clearfix">
                                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                                <asp:GridView ID="gvCustomerFeedback" runat="server" AutoGenerateColumns="false" class="table table-bordered table-striped table-hover js-basic-example dataTable" BorderWidth="1px" CellPadding="4" Width="100%" OnRowDataBound="gvCustomerFeedback_RowDataBound" DataKeyNames="FeedbackID">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sl.No">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle Width="5%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="feedbackname" HeaderText="Description" />
                                        <asp:TemplateField HeaderText="Feedback">
                                            <ItemTemplate>
                                                <asp:Label ID="lblfeedbackvalue" Text='<%# Eval("feedbackvalue") %>' runat="server"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Feeback Weightage">
                                            <ItemTemplate>
                                                <asp:Label ID="lblfeedbackweightage" Text='<%# Eval("feedbackweightage") + "0% " %>' runat="server" />
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
                </div>
            </div>
            <!-- #END# Task Info -->
            </div>
        </div>
        <!-- Jquery Knob Plugin Js -->
    <script src="plugins/jquery-knob/jquery.knob.min.js"></script>
    <script src="js/pages/charts/jquery-knob.js"></script>
</asp:Content>

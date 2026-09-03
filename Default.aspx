<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LIMS.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="card">
        <div class="header bg-violet">
            <h2 style="color:white !important">DASHBOARD</h2>
        </div>
        <div class="body">
            <div id="divAdmin1" runat="server" visible="false" class="row">
                <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12">
                    <div id="divPeriod" runat="server" class="title"></div>
                    <asp:Button ID="btnToday" runat="server" Text="TODAY" class="btn bg-red waves-effect" OnClick="btnToday_Click" />&nbsp;
                    <asp:Button ID="btnWeek" runat="server" Text="WEEK" class="btn bg-pink waves-effect" OnClick="btnWeek_Click" />&nbsp;
                    <asp:Button ID="btnMonth" runat="server" Text="MONTH" class="btn bg-purple waves-effect" OnClick="btnMonth_Click" />&nbsp;
                    <asp:Button ID="btnAll" runat="server" Text="ALL" class="btn bg-primary waves-effect" OnClick="btnAll_Click" />&nbsp;<br />
                </div>
            </div>
            <br />
            <div id="divAdmin" runat="server" visible="false" class="row">
                <div class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
                    <div class="info-box-2 bg-lightpink hover-zoom-effect">
                        <div class="icon">
                            <i class="material-icons">help_outline</i>
                        </div>
                        <div class="content">
                            <div class="text">
                                INQUIRIES
                            </div>
                            <asp:HyperLink ID="hypNewEnquiries" runat="server" class="number" NavigateUrl="~/CustomerInquiryView.aspx"></asp:HyperLink>
                        </div>
                    </div>

                </div>
                <div class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
                    <div class="info-box-2 bg-indigo hover-zoom-effect">
                        <div class="icon">
                            <i class="material-icons">face</i>
                        </div>
                        <div class="content">
                            <div class="text">CUSTOMERS</div>
                            <asp:HyperLink ID="hypNewCustomers" runat="server" class="number"></asp:HyperLink>
                        </div>
                    </div>

                </div>
                <div class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
                    <div class="info-box-2 bg-purple hover-zoom-effect">
                        <div class="icon">
                            <i class="material-icons">bookmark</i>
                        </div>
                        <div class="content">
                            <div class="text">ASSIGNED</div>
                            <asp:HyperLink ID="hypAssigned" runat="server" class="number"></asp:HyperLink>
                        </div>
                    </div>
                </div>
                <div class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
                    <div class="info-box-2 bg-deep-purple hover-zoom-effect">
                        <div class="icon">
                            <i class="material-icons">equalizer</i>
                        </div>
                        <div class="content">
                            <div class="text">COMPLETED</div>
                            <asp:HyperLink ID="hypCompleted" runat="server" class="number"></asp:HyperLink>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Widgets -->
            <div class="row clearfix">
                 <div id="divCRF" runat="server" class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
                    <div class="info-box-2 bg-lightpink hover-zoom-effect">
                        <div class="icon">
                            <i class="material-icons">insert_comment</i>
                        </div>
                        <div class="content">
                            <div class="text">TAF</div>
                            <asp:HyperLink ID="hypCRF" runat="server" class="number" NavigateUrl="~/CustomerCRFList.aspx"></asp:HyperLink>
                        </div>
                    </div>
                </div>
              <%--  <div id="divCRFApproved" runat="server" class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
                    <div class="info-box-2 bg-lightgreen hover-zoom-effect">
                        <div class="icon">
                            <i class="material-icons">insert_comment</i>
                        </div>
                        <div class="content">
                            <div class="text">Approved CRF</div>
                            <asp:HyperLink ID="hypApprovedCRF" runat="server" class="number" NavigateUrl="~/CustomerCRFList.aspx"></asp:HyperLink>
                        </div>
                    </div>
                </div>--%>
                <div id="divSamples" runat="server" class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
                    <div class="info-box-2 bg-lightblue hover-zoom-effect">
                        <div class="icon">
                            <i class="material-icons">book</i>
                        </div>
                        <div class="content">
                            <div class="text">ALL SAMPLES</div>
                            <asp:HyperLink ID="hypSamples" runat="server" class="number" NavigateUrl="~/AllSamples.aspx"></asp:HyperLink>
                        </div>
                    </div>
                </div>
                 <div id="divAllocation" runat="server" class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
                    <div class="info-box-2 bg-cyan hover-zoom-effect">
                        <div class="icon">
                            <i class="material-icons">assignment_ind</i>
                        </div>
                        <div class="content">
                            <div class="text">ALLOCATION</div>
                            <asp:HyperLink ID="hypAllocate" runat="server" class="number" NavigateUrl="~/AllSamples.aspx"></asp:HyperLink>
                        </div>
                    </div>
                </div>
                <div id="divWorkList" runat="server" class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
                    <div class="info-box-2 bg-lightblue hover-zoom-effect">
                        <div class="icon">
                            <i class="material-icons">assignment</i>
                        </div>
                        <div class="content">
                            <div class="text">WORK LIST</div>
                            <asp:HyperLink ID="hypWorkList" runat="server" class="number"></asp:HyperLink>
                        </div>
                    </div>
                </div>
                <div id="divGenReport" runat="server" class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
                     <div class="info-box-2 bg-lighterpink hover-zoom-effect">
                        <div class="icon">
                            <i class="material-icons">assignment</i>
                        </div>
                        <div class="content">
                            <div class="text">GEN. REPORT</div>
                            <asp:HyperLink ID="hypGenReport" runat="server" class="number"></asp:HyperLink>
                        </div>
                    </div>
                </div>
                <div id="divReview" runat="server" class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
                     <div class="info-box-2 bg-light-orange hover-zoom-effect">
                        <div class="icon">
                            <i class="material-icons">assignment_turned_in</i>
                        </div>
                        <div class="content">
                            <div class="text">REVIEW</div>
                            <asp:HyperLink ID="hypReview" runat="server" class="number"></asp:HyperLink>
                        </div>
                    </div>
                </div>
            </div>
            <!-- #END# Widgets -->

           
            <!-- CPU Usage -->
            <div class="row clearfix" style="visibility: hidden">
                <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
                    <div class="card"><hr class="violet"><div class="header">

                        <div class="body">
                            <div id="real_time_chart" class="dashboard-flot-chart"></div>
                        </div>
                    </div>
                </div>
            </div>
            <!-- #END# CPU Usage -->
        </div>
    </div>
        </div>
    <asp:HiddenField ID="hdPeriod" runat="server" />
    <asp:HiddenField ID="hdType" runat="server" />
    <asp:HiddenField ID="hdRoleID" runat="server" />
</asp:Content>

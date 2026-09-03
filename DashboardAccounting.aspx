<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="DashboardAccounting.aspx.cs" Inherits="LIMS.DashboardAccounting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="card">
        <div class="header bg-violet">
            <h2 style="color: white !important">ACCOUNTS DASHBOARD</h2>
        </div>
        <div class="body">
            <div id="divAdmin1" runat="server" class="row">
                <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12">
                    <div id="divPeriod" runat="server" class="title"></div>
                    <asp:Button ID="btnToday" runat="server" Text="TODAY" class="btn bg-red waves-effect" />&nbsp;
                 <asp:Button ID="btnWeek" runat="server" Text="WEEK" class="btn bg-pink waves-effect" />&nbsp;
                 <asp:Button ID="btnMonth" runat="server" Text="MONTH" class="btn bg-purple waves-effect" />&nbsp;
                 <asp:Button ID="btnAll" runat="server" Text="ALL" class="btn bg-primary waves-effect" />&nbsp;<br />
                </div>
            </div>
            <br />
            <div id="divAdmin" runat="server" class="row">
                <div class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
                    <div class="info-box-2 bg-lightpink hover-zoom-effect" style="margin-top: 0px !important;">
                       
                        <div class="content">
                            <div class="text">
                                ACCTS. PAYABLE
                            </div>
                            <asp:HyperLink ID="hypAP" runat="server" class="number" NavigateUrl="~/AccountsPayable.aspx"></asp:HyperLink>
                        </div>
                    </div>

                </div>
                <div class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
                    <div class="info-box-2 bg-indigo hover-zoom-effect">
                       
                        <div class="content">
                            <div class="text">ACCTS. RECEIVABLE</div>
                            <asp:HyperLink ID="hypAR" runat="server" class="number" NavigateUrl="~/AccountsReceivable.aspx"></asp:HyperLink>
                        </div>
                    </div>

                </div>
                <div class="col-lg-3 col-md-3 col-sm-6 col-xs-12">
                    <div class="info-box-2 bg-purple hover-zoom-effect">
                       
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



        </div>
    </div>
    <asp:HiddenField ID="hdPeriod" runat="server" />
    <asp:HiddenField ID="hdType" runat="server" />
    <asp:HiddenField ID="hdRoleID" runat="server" />
</asp:Content>

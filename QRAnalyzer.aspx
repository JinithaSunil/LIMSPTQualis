<%@ Page Title="" Language="C#" MasterPageFile="~/HemetekLIMSNoMenu.Master" AutoEventWireup="true" CodeBehind="QRAnalyzer.aspx.cs" Inherits="LIMS.QRAnalyzer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <fieldset>
                <div class="row clearfix">
                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <div class="card"><hr class="violet"><div class="header">                            
                                
                            </div>
                            <div class="body">
                                <div class="row clearfix">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtSampleID" runat="server" CssClass="form-control" placeholder="SampleID" OnTextChanged="txtSampleID_TextChanged" AutoPostBack="true"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row clearfix">
                                <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12" id="cardCustomer" runat="server" >
                                    <div class="card"><hr class="violet"><div class="header">
                                        <div class="header bg-red">
                                            <h2>
                                                <span style="font-size: 24px; font-weight: bold">&#9312;</span> CUSTOMER DETAILS <small></small>
                                            </h2>
                                        </div>
                                        <div class="body">
                                            <b>Customer Name: </b>
                                            <asp:Label ID="lblCustomerName" runat="server"></asp:Label><br />
                                            <br />
                                            <b>Address: </b>
                                            <asp:Label ID="lblCustomerAddress" runat="server"></asp:Label><br />
                                        </div>
                                    </div>
                                </div>
                                    </div>
                                <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12" id="cardSample" runat="server" >
                                    <div class="card"><hr class="violet"><div class="header">
                                        <div class="header bg-cyan">
                                            <h2>
                                                <span style="font-size: 24px; font-weight: bold">&#9313;</span> SAMPLE DETAILS <small></small>
                                            </h2>
                                        </div>
                                        <div class="body">
                                            <b>Category: </b>
                                            <asp:Label ID="lblCategory" runat="server"></asp:Label><br /><br />
                                            <b>Sub Category: </b>
                                            <asp:Label ID="lblSubCategory" runat="server"></asp:Label><br /><br />
                                            <b>Sample Received: </b>
                                            <asp:Label ID="lblReceivedDate" runat="server"></asp:Label><br /><br />
                                            <b>Destroy Date: </b>
                                            <asp:Label ID="lblDestroyDate" runat="server"></asp:Label><br /><br />
                                            <b>Analysis Start Date: </b>
                                            <asp:Label ID="lblAnalysisStart" runat="server"></asp:Label><br /><br />
                                            <b>Analysis End Date: </b>
                                            <asp:Label ID="lblAnalysisEnd" runat="server"></asp:Label><br /><br />
                                            <b>Manufactured Date: </b>
                                            <asp:Label ID="lblManufacturedDate" runat="server"></asp:Label><br /><br />
                                        </div>
                                    </div>
                                </div>
                                    </div>
                                <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12" id="cardAllocation" runat="server" >
                                    <div class="card"><hr class="violet"><div class="header">
                                        <div class="header bg-green">
                                            <h2>
                                                <span style="font-size: 24px; font-weight: bold">&#9314;</span> ALLOCATION DETAILS <small></small>
                                            </h2>
                                        </div>
                                        <div class="body">
                                            <b>Department: </b>
                                            <asp:Label ID="lblDepartment" runat="server"></asp:Label><br /><br />
                                            <b>Technical Manager: </b>
                                            <asp:Label ID="lblManager" runat="server"></asp:Label><br /><br />
                                            <b>Lab Technician: </b>
                                            <asp:Label ID="lblTechnician" runat="server"></asp:Label><br /><br />
                                        </div>
                                    </div>
                                </div>     
                                    </div>                           
                            </div>
                        </div>
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

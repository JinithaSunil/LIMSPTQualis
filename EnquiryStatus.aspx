<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="EnquiryStatus.aspx.cs" Inherits="LIMS.EnquiryStatus" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card"><hr class="violet"><div class="header">
                    <div class="row">
                        <div class="col-sm-6">
                            <h2>ENQUIRY STATUS</h2>
                        </div>
                    </div>
                </div>
                <div class="body">
                    <div id="divNoSample" runat="server" visible="false">No Sample Details Available as yet</div>
                    <div id="divSample" runat="server">
                    <div class="row clearfix">
                        <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12" id="cardEnquiry" runat="server" visible="false">
                            <div class="card"><hr class="violet"><div class="header">
                                <div class="header bg-red">
                                    <h2>
                                        <span style="font-size: 24px; font-weight: bold">&#9312;</span> ENQUIRY -
                                        <asp:Label ID="lblEnquiryID" runat="server"></asp:Label>
                                        <small></small>
                                    </h2>
                                </div>
                                <div class="body">
                                    <b>Inquiry Status: </b>
                                    <asp:Label ID="lblInquiryStatus" runat="server"></asp:Label><br />
                                    <br />
                                    <b>Inquiry Details: </b>
                                    <br />
                                    <asp:Label ID="lblInquiryDetail" runat="server"></asp:Label>
                                    <asp:LinkButton ID="ReadMoreLinkButton" runat="server"
                                        Text="Read More"
                                        OnClick="ReadMoreLinkButton_Click">
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                            </div>
                        <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12" id="cardCRF" runat="server" visible="false">
                            <div class="card"><hr class="violet"><div class="header">
                                <div class="header bg-cyan">
                                    <h2>
                                        <span style="font-size: 24px; font-weight: bold">&#9313;</span> TAF DETAILS <small></small>
                                    </h2>
                                </div>
                                <div class="body">
                                    <b>CRF Status: </b>
                                    <asp:Label ID="lblCRFStatus" runat="server"></asp:Label><br />
                                    <br />
                                    <b>CRF Document: </b>
                                    <asp:HyperLink ID="hypCRF" runat="server" Text="View TAF" Target="_blank" Visible="false"></asp:HyperLink>
                                </div>
                            </div>
                        </div>
						</div>
                        <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12" id="cardSample" runat="server" visible="false">
                            <div class="card"><hr class="violet">
							<div class="header">
                                <div class="header bg-green">
                                    <h2>
                                        <span style="font-size: 24px; font-weight: bold">&#9314;</span> SAMPLE DETAILS <small></small>
                                    </h2>
                                </div>
								
                                <div class="body">
                                    <b>Sample Status:</b><asp:Label ID="lblSampleStatus" runat="server"></asp:Label><br />
                                    <br />
                                    <b>Sample Verification:</b><asp:HyperLink ID="hypSampleVerification" runat="server" Text="Verification Checklist" Target="_blank" Visible="false"></asp:HyperLink>
                                </div>
                            </div>
                        </div>
                    </div>
                            
                        </div>
                    <div class="row">
                        <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12" id="cardAllocation" runat="server" visible="false">
                            <div class="card"><hr class="violet"><div class="header">
                                <div class="header bg-orange">
                                    <h2>
                                        <span style="font-size: 24px; font-weight: bold">&#9315;</span> ALLOCATION FORM <small></small>
                                    </h2>
                                </div>
                                <div class="body">
                                    <b>Allocated To: </b>
                                    <asp:Label ID="lblAllocationStatus" runat="server"></asp:Label><br />
                                    <br />
                                    <b>Allocation Form: </b>
                                    <asp:HyperLink ID="hypTestAllocation" runat="server" Text="View Allocation Form" Target="_blank" Visible="false"></asp:HyperLink>
                                </div>
                            </div>
                        </div>
                            </div>
                        <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12" id="cardTesting" runat="server" visible="false">
                            <div class="card"><hr class="violet"><div class="header">
                                <div class="header bg-blue-grey">
                                    <h2>
                                        <span style="font-size: 24px; font-weight: bold">&#9316;</span> TEST RESULTS <small></small>
                                    </h2>
                                </div>
                                <div class="body">
                                    <b>Testing Status: </b>
                                    <asp:Label ID="lblTestingStatus" runat="server"></asp:Label><br />
                                    <br />
                                    <b>Test Result: </b>
                                    <asp:HyperLink ID="hypTestResults" runat="server" Text="View Test Results" Target="_blank" Visible="false"></asp:HyperLink> 
                                    <br />
                                    <asp:HyperLink ID="hypTestResults1" runat="server" Text="View Test Result English PDF" Target="_blank" Visible="false"></asp:HyperLink>
                                    <br />
                                    <asp:HyperLink ID="hypTestResults2" runat="server" Text="View Test Result BHASA PDF" Target="_blank" Visible="false"></asp:HyperLink>
                                    <br />
                                    <asp:HyperLink ID="hypTestResults3" runat="server" Text="View Test Result English Draft" Target="_blank" Visible="false"></asp:HyperLink>
                                    <br />
                                    <asp:HyperLink ID="hypTestResults4" runat="server" Text="View Test Result BHASA Draft" Target="_blank" Visible="false"></asp:HyperLink>
                                </div>
                            </div>
                        </div>
                            </div>
                        <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12" id="cardFeedback" runat="server" visible="false">
                            <div class="card"><hr class="violet"><div class="header">
                                <div class="header bg-pink">
                                    <h2>
                                        <span style="font-size: 24px; font-weight: bold">&#9317;</span> CUSTOMER FEEDBACK<small></small>
                                    </h2>
                                </div>
                                <div class="body">
                                    <b>Feedback Status: </b>
                                    <asp:Label ID="lblFeedbackStatus" runat="server"></asp:Label><br />
                                    <br />
                                    <b>Customer Feedback: </b>
                                    <asp:HyperLink ID="hypCustomerFeedback" runat="server" Text="Customer Feedback" Target="_blank" Visible="false"></asp:HyperLink>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                        </div>
            </div>
        </div>
    </div>
    </div>
    <!-- #END# Basic Example -->
</asp:Content>

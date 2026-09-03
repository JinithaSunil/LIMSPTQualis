<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="SampleStatus.aspx.cs" Inherits="LIMS.SampleStatus" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function showModal() {
            $("#myModal").modal('show');
        }

        function ShowPopup() {
            $("#btnShowPopup").click();
        }
    </script>
    <style>
        @media screen and (min-width: 768px) {
            .modal-dialog {
                width: 300px; /* New width for default modal */
            }

            .modal-sm {
                width: 150px; /* New width for small modal */
            }
        }

        @media screen and (min-width: 992px) {
            .modal-lg {
                width: 40%; /* New width for large modal */
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="container-fluid">
        <button type="button" style="display: none;" id="btnShowPopup" class="btn btn-primary btn-lg"
            data-toggle="modal" data-target="#basic">
            Launch demo modal
        </button>
    </div>
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-6">
                            <h2>SAMPLE STATUS</h2>
                        </div>
                    </div>
                </div>
                <div class="body">
                    <div class="row">
                        <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12" id="cardEnquiry" runat="server" visible="false">
                            <div class="card">
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
                        <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12" id="cardCRF" runat="server" visible="false">
                            <div class="card">
                                <div class="header bg-cyan">
                                    <h2>
                                        <span style="font-size: 24px; font-weight: bold">&#9313;</span> TEST REQUEST DETAILS <small></small>
                                    </h2>
                                </div>
                                <div class="body">
                                    <b>Test Request Status: </b>
                                    <asp:Label ID="lblCRFStatus" runat="server"></asp:Label><br />
                                    <br />
                                    <b>Test Request Form: </b>
                                    <asp:HyperLink ID="hypCRF" runat="server" Text="View" Target="_blank" Visible="false"></asp:HyperLink><br />
                                    <br />
                                    <b>Test Request Review: </b>
                                    <asp:HyperLink ID="hypCRFReview" runat="server" Text="View" Target="_blank" Visible="false"></asp:HyperLink>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12" id="cardSample" runat="server" visible="false">
                            <div class="card">
                                <div class="header bg-green">
                                    <h2>
                                        <span style="font-size: 24px; font-weight: bold">&#9314;</span> SAMPLE DETAILS <small></small>
                                    </h2>
                                </div>
                                <div class="body">
                                    <b>Sample Status:</b><asp:Label ID="lblSampleStatus" runat="server"></asp:Label><br />
                                    <br />
                                    <b>Sample Verification:</b><asp:HyperLink ID="hypSampleVerification" runat="server" Text="Verification Checklist" Target="_blank" Visible="false"></asp:HyperLink>
                                    <br />
                                    <br />
                                    <b>Sample Forwarding Memo:</b><asp:HyperLink ID="hypSampleMemo" runat="server" Text="View" Target="_blank" Visible="false"></asp:HyperLink>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12" id="cardWO" runat="server" visible="false">
                            <div class="card">
                                <div class="header bg-pink">
                                    <h2>
                                        <span style="font-size: 24px; font-weight: bold">&#9315;</span> WORK ORDER<small></small>
                                    </h2>
                                </div>
                                <div class="body">
                                    <b>SampleID: </b>
                                    <asp:Label ID="lblSampleID" runat="server"></asp:Label><br />
                                    <br />
                                    <b>Work Order: </b>
                                    <asp:HyperLink ID="hypWO" runat="server" Text="View Work Order" NavigateUrl="" Target="_blank" Visible="true"></asp:HyperLink>
                              
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12" id="Div1" runat="server">
                            <div class="card">
                                <div class="header bg-pink">
                                    <h2>
                                        <span style="font-size: 24px; font-weight: bold">&#9316;</span> SAMPLE IMAGES<small></small>
                                    </h2>
                                </div>
                                <div class="body">
                                    
                                    <label>Image Name: </label>
                                    <asp:TextBox ID="txtSampleName" runat="server" CssClass="form-control"></asp:TextBox>
                                    <asp:Label ID="lblInquiryID" runat="server" Visible="false"></asp:Label>

                                    <asp:FileUpload ID="fpSampleImage" runat="server" Width="100%" Style="margin-top: 23px;" EnableViewState="true" />

                                    <asp:Button ID="btnUploadImage" runat="server" Text="Upload Image" OnClick="btnUploadImage_Click" CssClass="btn btn-info" Style="margin-top: 20px;" />
                                    <br />
                                    <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
                                    <br />
                                    <asp:GridView ID="GridView1" runat="server"
                                        AutoGenerateColumns="false"
                                        CssClass="gridview"
                                        DataKeyNames="ID, filename"
                                        OnRowCommand="GridView1_RowCommand"
                                        OnRowDataBound="GridView1_RowDataBound" class="table table-bordered table-striped table-hover js-basic-example dataTable">
                                        <Columns>
                                            <asp:TemplateField HeaderText="#">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRowNumber" runat="server"
                                                        Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Image">
                                                <ItemTemplate>
                                                    <asp:Image ID="imgSample" runat="server"
                                                        CssClass="img-thumbnail"
                                                        AlternateText='<%# Eval("SampleText") %>' />
                                                    <br />
                                                    <asp:Label ID="lblFileName" runat="server"
                                                        Text='<%# Eval("filename") %>' Visible="false"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="SampleText" HeaderText="Sample Name" />

                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <asp:Button ID="btnDelete" runat="server"
                                                        Text="Delete"
                                                        CssClass="delete-button"
                                                        CommandName="DeleteRow"
                                                        CommandArgument='<%# Container.DisplayIndex %>'
                                                        OnClientClick="return confirm('Are you sure you want to delete this image?');" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12" id="cardAllocation" runat="server" visible="false">
                            <div class="card">
                                <div class="header bg-orange">
                                    <h2>
                                        <span style="font-size: 24px; font-weight: bold">&#9317;</span> ALLOCATION FORM <small></small>
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

                        <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12" id="cardTesting" runat="server" visible="false">
                            <div class="card">
                                <div class="header bg-blue-grey">
                                    <h2>
                                        <span style="font-size: 24px; font-weight: bold">&#9318;</span> TEST RESULTS <small></small>
                                    </h2>
                                </div>
                                <div class="body">
                                    <b>Testing Status: </b>
                                    <asp:Label ID="lblTestingStatus" runat="server"></asp:Label><br />
                                    <br />
                                    <b>Test Report: </b>
                                    <asp:HyperLink ID="hypTestResults" runat="server" Text="View" Target="_blank" Visible="false"></asp:HyperLink>/
                                    <asp:HyperLink ID="hypTestResultsNN" runat="server" Text="Delete" Target="_blank" Visible="false"></asp:HyperLink><br />
                                    <br />
                                    <b>Test Review List </b>
                                    <asp:PlaceHolder
                                        ID="PlaceHolderReviewList"
                                        runat="server"></asp:PlaceHolder>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12" id="cardReport" runat="server" visible="false">
                            <div class="card">
                                <div class="header bg-pink">
                                    <h2>
                                        <span style="font-size: 24px; font-weight: bold">&#9319;</span> TEST REPORT<small></small>
                                    </h2>
                                </div>
                                <div class="body">
                                    <b>Test Report Status: </b>
                                    <asp:Label ID="lblReportStatus" runat="server"></asp:Label><br />
                                    <br />
                                    <b>Customer Feedback: </b>
                                    <asp:HyperLink ID="hypTestReport" runat="server" Text="Test Report(System Generated)" Target="_blank" Visible="false"></asp:HyperLink>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-4" id="divChat" runat="server" visible="true">
                            <div class="card">
                                <div class="header bg-red">
                                    <h2>
                                        <span style="font-size: 24px; font-weight: bold">&#9320;</span> CHAT HISTORY <small></small>
                                    </h2>
                                </div>
                                <div class="body" style="text-align: center">
                                    <asp:Button ID="btnChat" runat="server" Text="View Chat" CssClass="btn btn-danger" OnClick="btnChat_Click" />
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4" id="cardTestReportUpload" runat="server" visible="false">
                            <div class="card">
                                <div class="header bg-cyan">
                                    <h2>
                                        <span style="font-size: 24px; font-weight: bold">&#9321;</span> REPORT UPLOADS <small></small>
                                    </h2>
                                </div>
                                <div class="body">
                                    <asp:PlaceHolder
                                        ID="PlaceHolderReportList"
                                        runat="server"></asp:PlaceHolder>
                                    <asp:TextBox ID="txtTestReportNo" runat="server" placeholder="Test Report No." CssClass="form-control"></asp:TextBox><br />
                                    <table>
                                        <tr>
                                            <td>English Report:</td>
                                            <td>
                                                <asp:FileUpload ID="fpTestReportUploadEN" runat="server" CssClass="btn btn-info" Width="90%" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Bahasa Report:</td>
                                            <td>
                                                <asp:FileUpload ID="fpTestReportUploadBH" runat="server" CssClass="btn btn-info" Width="90%" />
                                            </td>
                                        </tr>

                                        <tr></tr>

                                        <tr>
                                            <td></td>
                                            <td>
                                                <asp:Button ID="btnUpload" runat="server" Text="Upload Report" OnClick="btnUpload_Click" CssClass="btn btn-info" Style="margin-top: 10px;" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12" id="cardFeedback" runat="server" visible="true">
                            <div class="card">
                                <div class="header bg-green">
                                    <h2>
                                        <span style="font-size: 24px; font-weight: bold">&#9322;</span> CUSTOMER FEEDBACK<small></small>
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
                    <div class="modal fade" id="myModal" role="dialog" data-backdrop="static" data-keyboard="false">
                        <div class="modal-dialog modal-lg">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                                    <h4 class="modal-title">
                                        <center>Chat</center>
                                    </h4>
                                </div>
                                <div class="modal-body" style="margin: 20px">

                                    <div class="row">
                                        <div class="col-4">
                                            <asp:TextBox ID="txtMsg" class="form-control" AutoComplete="off" placeholder="Messagge" AutoPostBack="true" ReadOnly="true" Height="290px" Width="490px" TextMode="MultiLine" runat="server"></asp:TextBox>
                                            <asp:Timer ID="tmMsg" runat="server" OnTick="tmMsg_Tick"></asp:Timer>
                                        </div>
                                    </div>
                                    <br />
                                    <div class="row">
                                        <div style="align-items: center">
                                            <asp:TextBox ID="txtSend" class="form-control" AutoComplete="off" placeholder="Type a Message" runat="server" TextMode="MultiLine" Width="100%"></asp:TextBox>
                                        </div>
                                    </div>
                                    <br />
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- #END# Basic Example -->
    </div>
</asp:Content>

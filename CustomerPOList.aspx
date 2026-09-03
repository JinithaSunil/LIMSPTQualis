<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="CustomerPOList.aspx.cs" Inherits="LIMS.CustomerPOList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <!-- Basic Examples -->
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-12">
                            <h2>CUSTOMER PO LIST</h2>
                        </div>
                    </div>
                    <br />
                </div>
                <div class="body">
                    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <fieldset>

                                <br />
                                <div class="row">
                                    <div class="col-sm-12">
                                        <asp:GridView ID="gvCustomerPO" runat="server" OnRowDataBound="gvCustomerPO_RowDataBound" OnRowCommand="gvCustomerPO_RowCommand" class="table table-bordered table-striped table-hover js-basic-example dataTable" AutoGenerateColumns="false">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Quotation#">
                                                    <ItemTemplate>
                                                        <asp:HyperLink
                                                            ID="hypInquiry"
                                                            runat="server"
                                                            Text='<%# Eval("QuotationNumber").ToString().ToUpper() %>'
                                                            Target="_blank"
                                                            NavigateUrl='<%# Eval("QuotationMasterID", "GenerateQuotationCalib.aspx?ID={0}&r=" + Eval("RevisionCount")) %>'>
                                                        </asp:HyperLink>
                                                        <asp:Label ID="lblCalibrationStatusID" runat="server" Text='<%#Eval("CalibrationStatusID") %>' Visible="false"></asp:Label>
                                                        <asp:HiddenField ID="hdDepartmentID" runat="server" Value='<%#Eval("DepartmentID") %>' />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="5%"></ItemStyle>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Customer" SortExpression="CustomerName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCustomer" runat="server" Text='<%# Eval("CustomerName").ToString().ToUpper()  %>' Target="_blank" NavigateUrl='<%# Eval("CustomerMasterID", "CustomerProfile.aspx?ID={0}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="20%"></ItemStyle>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Documents">
                                                    <ItemTemplate>
                                                        <asp:HyperLink ID="hypPO"
                                                            runat="server"
                                                            Text='<%# "CustPO-" + Eval("PONumber").ToString().ToUpper() %>'
                                                            Target="_blank"
                                                            NavigateUrl='<%# "CustomerPO/" + Eval("POFile") %>'>
                                                        </asp:HyperLink><br />
                                                        <asp:HyperLink ID="hypWO" runat="server" Text='<%# "WO-" + Eval("QuotationMasterID") %>' Target="_blank" NavigateUrl='<%#"WorkOrder/WO-" + Eval("QuotationMasterID") + ".htm" %>'></asp:HyperLink><br />
                                                        <asp:HyperLink ID="hypSR" runat="server" Text='<%# Eval("SampleReturn") %>' Target="_blank" NavigateUrl='<%#"SampleReturn/" + Eval("SampleReturn") + ".htm" %>'></asp:HyperLink><br />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="10%"></ItemStyle>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="ServiceDueDate" HeaderText="Service Due Date" DataFormatString="{0:dd-MM-yyyy}" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="CalibrationStatus" HeaderText="Status" />
                                                <asp:TemplateField HeaderText="Action">
                                                    <ItemTemplate>
                                                        <asp:Button ID="btnVerifyPO" runat="server" Text="Verify PO" CssClass="btn btn-primary" CommandName="VerifyPO" CommandArgument='<%# Eval("QuotationMasterID") %>'
                                                            ToolTip="Verify PO"></asp:Button>
                                                        <br />
                                                        <asp:Label ID="lblServiceDue" runat="server" Text="Service Due Date:" Visible="false"></asp:Label><br />
                                                        <asp:TextBox ID="txtDueDate" runat="server" CssClass="form-control" TextMode="Date" Visible="false"></asp:TextBox>
                                                        <asp:Button ID="btnPOVerified" runat="server" Text="Verified" CssClass="btn btn-primary" CommandName="VerifiedPO" CommandArgument='<%# Eval("QuotationMasterID") %>'></asp:Button>
                                                        <asp:Button ID="btnAcceptForCalibration" runat="server" Text="Accept For Calibration" CssClass="btn btn-success waves-effect waves-light" CommandName="AcceptForCalibration" CommandArgument='<%# Eval("QuotationMasterID") %>'></asp:Button>&nbsp;
                                                        <asp:Button ID="btnRejectForCalibration" runat="server" Text="Reject For Calibration" CssClass="btn btn-danger waves-effect waves-light" CommandName="RejectForCalibration" CommandArgument='<%# Eval("QuotationMasterID") %>'></asp:Button>
                                                        <asp:Button ID="btnToolsReceivedByCS" runat="server" Text="Tools Received by CS" CssClass="btn btn-info waves-effect waves-light" CommandName="ToolsReceivedByCS" CommandArgument='<%# Eval("QuotationMasterID") %>'></asp:Button>
                                                        <asp:Button ID="btnSPK" runat="server" Text="SPK" CssClass="btn btn-info waves-effect waves-light" CommandName="SPK" CommandArgument='<%# Eval("QuotationMasterID") %>'></asp:Button>
                                                        <asp:Button ID="btnCalibAccept" runat="server" Text="Accept for Calibration" CssClass="btn btn-success waves-effect waves-light" CommandName="AcceptCalib" CommandArgument='<%# Eval("QuotationMasterID") %>'></asp:Button>&nbsp;&nbsp;
                                                        <asp:Button ID="btnCalibOnTime" runat="server" Text="Calibration On Time" CssClass="btn btn-success waves-effect waves-light" CommandName="CalibOnTime" CommandArgument='<%# Eval("QuotationMasterID") %>'></asp:Button>&nbsp;&nbsp;
                                                        <asp:Button ID="btnCalibDelay" runat="server" Text="Calibration Delay" CssClass="btn btn-danger waves-effect waves-light" CommandName="CalibDelay" CommandArgument='<%# Eval("QuotationMasterID") %>'></asp:Button>
                                                        <asp:Button ID="btnCalibDelayCustomerAccept" runat="server" Text="Customer Accepted Calibration Delay" CssClass="btn btn-success waves-effect waves-light" CommandName="CalibCustomerDelayAccepted" CommandArgument='<%# Eval("QuotationMasterID") %>'></asp:Button>
                                                        <asp:Button ID="btnCalibDelayCustomerReject" runat="server" Text="Customer Rejected Calibration Delay" CssClass="btn btn-danger waves-effect waves-light" CommandName="CalibCustomerDelayRejected" CommandArgument='<%# Eval("QuotationMasterID") %>'></asp:Button>
                                                        <asp:Button ID="btnCalibReject" runat="server" Text="Reject for Calibration" CssClass="btn btn-danger waves-effect waves-light" CommandName="RejectCalib" CommandArgument='<%# Eval("QuotationMasterID") %>'></asp:Button>
                                                        <asp:Button ID="btnAllocate" runat="server" Text="Allocate" CssClass="btn btn-primary waves-effect waves-light" CommandName="Allocate" CommandArgument='<%# Eval("QuotationMasterID") %>'></asp:Button>
                                                        <asp:Button ID="btnCalibResult" runat="server" Text="Calibration Result" CssClass="btn btn-primary waves-effect waves-light" CommandName="CalibResult" CommandArgument='<%# Eval("QuotationMasterID") %>'></asp:Button>
                                                        <asp:Button ID="btnGenerateMR" runat="server" Text="Generate MR" CssClass="btn btn-primary waves-effect waves-light" CommandName="GenerateMR" CommandArgument='<%# Eval("QuotationMasterID") %>'></asp:Button>
                                                        <asp:Button ID="btnReturnTool" runat="server" Text="Return Tool" CssClass="btn btn-primary waves-effect waves-light" CommandName="ReturnTool" CommandArgument='<%# Eval("QuotationMasterID") %>'></asp:Button>
                                                        <asp:Button ID="btnSubContractCustomerAccept" runat="server" Text="Customer Accepted Sub Contracting" CssClass="btn btn-success waves-effect waves-light" CommandName="CustomerAcceptedSubContracting" CommandArgument='<%# Eval("QuotationMasterID") %>'></asp:Button>
                                                        <asp:Button ID="btnSubContractCustomerReject" runat="server" Text="Customer Rejected Sub Contracting" CssClass="btn btn-danger waves-effect waves-light" CommandName="CustomerRejectedSubContracting" CommandArgument='<%# Eval("QuotationMasterID") %>'></asp:Button>
                                                        <asp:Button ID="btnChooseSubContractor" runat="server" Text="Choose Sub Contractor" CssClass="btn btn-primary" CommandName="ChooseSubContractor" CommandArgument='<%# Eval("QuotationMasterID") %>' />
                                                        <asp:DropDownList ID="ddlSubContractors" runat="server" CssClass="form-control"></asp:DropDownList>
                                                        <asp:Button ID="btnAddSubContractor" runat="server" CssClass="btn btn-primary" Text="Add" CommandName="AddSubContractor"  CommandArgument='<%# Eval("QuotationMasterID") %>' />
                                                        <asp:Button ID="btnSendImages" runat="server" CssClass="btn btn-primary" Text="Send Images" CommandName="SendImages"  CommandArgument='<%# Eval("QuotationMasterID") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <br />
                                    </div>
                                </div>
                            </fieldset>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

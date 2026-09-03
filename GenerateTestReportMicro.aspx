<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="GenerateTestReportMicro.aspx.cs" Inherits="LIMS.GenerateTestReportMicro" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .ajax__calendar_container td {
            padding: 0 !important;
            margin: 0 !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <fieldset>
                <div class="row clearfix">
                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <div class="card">
                            <hr class="violet">
                            <div class="header">

                                <h3>Test Report Food Laboratory</h3>
                            </div>
                            <div class="body">
                                <table style="width: 100%">
                                    <tr>
                                        <td style="width: 20%"><b>Issued To:</b></td>
                                        <td style="width: 30%">
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtCustomerName" class="form-control" AutoComplete="off" placeholder="Customer Name" runat="server" disabled></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                        <td style="width: 20%; padding-left:5%"><b>Signatory:</b></td>
                                        <td style="width: 30%">
                                            <div class="form-group" id="divSignatory" runat="server" visible="false">
                                                <div class="form-line">
                                                    <asp:DropDownList ID="ddlSignatory" runat="server" CssClass="form-control"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width:20%"><b>ACCREDITATION NO:</b></td>
                                        <td style="width:30%">
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtUlrNo" CssClass="form-control" AutoComplete="off" Text="LP-708-IDN" placeholder="ACCREDITATION No." runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                        <td style="padding-left:5%"><b>REPORT TYPE:</b></td>
                                        <td>
                                            <div class="form-group" id="divNABL" runat="server" visible="false">
                                                <div class="form-line">
                                                    <asp:DropDownList ID="ddlReportType" runat="server" CssClass="form-control">
                                                        <asp:ListItem>Select Type</asp:ListItem>
                                                        <asp:ListItem>ACCREDITATION </asp:ListItem>
                                                        <asp:ListItem>NON-ACCREDITATION</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <table style="width: 100%">
                                    <tr>
                                         <td style="width: 20%"><b>Test Report No:</b></td>
                                         <td style="width: 30%">
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtTestReportNo" AutoComplete="off" CssClass="form-control" placeholder="Test Report No." runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                         <td style="width: 20%; padding-left:5%"><b>Issue Date:</b></td>
                                         <td style="width: 30%">
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtIssueDate" AutoComplete="off" placeholder="Issue Date" runat="server" CssClass="form-control"></asp:TextBox>
                                                    <ajaxToolkit:CalendarExtender ID="CalendarExtender5" runat="server" TargetControlID="txtIssueDate" Format="dd/MM/yyyy"></ajaxToolkit:CalendarExtender>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 20%;"><b>Report Type:</b></td>
                                        <td style="width: 30%;" >
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:Label ID="lblReportID" runat="server" Visible="false"></asp:Label>
                                                    <asp:DropDownList ID="ddlReportFormat" runat="server" CssClass="form-control" Width="400"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Discipline:</b></td>
                                        <td>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtDiscipline" class="form-control" AutoComplete="off" placeholder="Discipline" runat="server" disabled></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                        <td style="width: 20%; padding-left:5%"><b>Group:</b></td>
                                        <td>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtGroup" class="form-control" AutoComplete="off" placeholder="Group" runat="server" disabled></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>

                                    </tr>
                                    <tr>
                                        <td><b>Contact Person:</b></td>
                                        <td>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtContactPerson" class="form-control" AutoComplete="off" placeholder="Contact Person" runat="server" disabled></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                        <td style="width: 20%; padding-left:5%"><b>Contact Details:</b></td>
                                        <td>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtContactDetail" class="form-control" AutoComplete="off" placeholder="Contact Detail" runat="server" disabled></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                                <br />

                                <table style="width: 100%" class="table table-bordered table-striped table-hover js-basic-example dataTable">
                                    <tr>
                                        <td colspan="4" align="center">
                                            <h4>Sample Test Details</h4>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Sample Registration Date:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtSampleRegisterDate" class="form-control" AutoComplete="off" placeholder="Sample Register Date" runat="server" disabled></asp:TextBox>
                                            </div>
                                        </td>
                                        <td><b>Sample Id:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtSampleId" class="form-control" AutoComplete="off" placeholder="Sample ID" runat="server" disabled></asp:TextBox>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Analysis Starting Date:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtAnalysisStartingDate" AutoComplete="off" placeholder="Analysis Starting Date" runat="server" BorderColor="YellowGreen"></asp:TextBox>
                                                <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtAnalysisStartingDate" Format="dd/MM/yyyy"></ajaxToolkit:CalendarExtender>
                                            </div>
                                        </td>
                                        <td><b>Analysis Completion Date:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtAnalysisCompleteDate" AutoComplete="off" placeholder="Analysis Complete Date" runat="server" BorderColor="YellowGreen"></asp:TextBox>
                                                <ajaxToolkit:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtAnalysisCompleteDate" Format="dd/MM/yyyy"></ajaxToolkit:CalendarExtender>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Name Of Product:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtNameOfProduct" AutoComplete="off" placeholder="Name Of Product" runat="server" BorderColor="YellowGreen"></asp:TextBox>
                                            </div>
                                        </td>
                                        <td><b>Date of Manufacture:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtDateOfMfg" AutoComplete="off" placeholder="Date of Mfg." runat="server" BorderColor="YellowGreen"></asp:TextBox>
                                                <ajaxToolkit:CalendarExtender ID="CalendarExtender3" runat="server" TargetControlID="txtDateOfMfg" Format="dd/MM/yyyy"></ajaxToolkit:CalendarExtender>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Sample Quantity:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtSampleQuality" AutoComplete="off" placeholder="Sample Quantity" runat="server" BorderColor="YellowGreen"></asp:TextBox>
                                            </div>
                                        </td>
                                        <td><b>Date of Expiry:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtDateOfExp" AutoComplete="off" placeholder="Date of Exp." runat="server" BorderColor="YellowGreen"></asp:TextBox>
                                                <ajaxToolkit:CalendarExtender ID="CalendarExtender4" runat="server" TargetControlID="txtDateOfExp" Format="dd/MM/yyyy"></ajaxToolkit:CalendarExtender>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Sample Received Date:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtSampleReceivedDate" class="form-control" AutoComplete="off" placeholder="Sample Received Date" runat="server" disabled></asp:TextBox>
                                            </div>
                                        </td>
                                        <td><b>Batch/Lot No.:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtBatchNo" AutoComplete="off" class="form-control" placeholder="Batch No." runat="server" disabled></asp:TextBox>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Test Required:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtTestRequired" AutoComplete="off" placeholder="Test Required" runat="server" BorderColor="YellowGreen"></asp:TextBox>
                                            </div>
                                        </td>
                                        <td><b>Sample Condition:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtSampleCondition" AutoComplete="off" placeholder="Sample Condition" runat="server" BorderColor="YellowGreen"></asp:TextBox>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Sampling:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtSampling" AutoComplete="off" placeholder="Sampling" runat="server" BorderColor="YellowGreen"></asp:TextBox>
                                            </div>
                                        </td>
                                        <td><b>Sampling Method:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtSamplingMethod" AutoComplete="off" placeholder="Sampling Method" runat="server" BorderColor="YellowGreen"></asp:TextBox>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Customer Reference:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtCustomerReference" placeholder="Customer Reference" runat="server" BorderColor="YellowGreen"></asp:TextBox>
                                            </div>
                                        </td>
                                        <td><b>Remarks:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtRemarks" AutoComplete="off" placeholder="Remarks" runat="server" BorderColor="YellowGreen"></asp:TextBox>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <div style="overflow: scroll">
                                    <table>
                                        <tr>
                                            <th>Sample Name</th>
                                            <th>Batch#</th>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblSampleName" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblBatchNo" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:GridView ID="gvResults" runat="server" CssClass="table table-striped" AutoGenerateColumns="false"
                                        OnRowEditing="gvResults_RowEditing" OnRowUpdating="gvResults_RowUpdating"
                                        OnRowCancelingEdit="gvResults_RowCancelingEdit" DataKeyNames="CRFSampleId" 
                                        OnRowCommand="gvResults_RowCommand" OnRowDataBound="gvResults_RowDataBound">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="1%">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="TestMethod" HeaderText="Test Method" ReadOnly="true" ItemStyle-Width="10%" />
                                            <asp:TemplateField HeaderText="TestParameter">
                <ItemTemplate>
                    <%# Eval("TestParameter") + " " + Eval("TestSubParameter") %>
                </ItemTemplate>
            </asp:TemplateField>
                                            <%--<asp:BoundField DataField="TestParameter" HeaderText="Test Parameter" ReadOnly="true" ItemStyle-Width="10%" />--%>
                                            <asp:BoundField DataField="AnalysisStartingDate" HeaderText="Start Date" DataFormatString="{0:dd/MM/yyyy}" ReadOnly="true" ItemStyle-Width="10%" />
                                            <asp:BoundField DataField="AnalysisCompletionDate" HeaderText="Completion Date" DataFormatString="{0:dd/MM/yyyy}" ReadOnly="true" ItemStyle-Width="10%" />
                                            <asp:BoundField DataField="TestResult" HeaderText="Test Result" ReadOnly="true" ItemStyle-Width="10%" />
                                            <asp:BoundField DataField="UnitOfMeasurement1" HeaderText="Unit Of Measurement" ReadOnly="true" ItemStyle-Width="7%" ControlStyle-Width="100%" />
                                            <asp:BoundField DataField="specification1" HeaderText="Specification" ReadOnly="true" ItemStyle-Width="10%" ControlStyle-Width="100%" />
                                            <asp:BoundField DataField="Conclusion" HeaderText="Conclusion" ReadOnly="true" ItemStyle-Width="10%" />

                                            <asp:TemplateField HeaderText="Status" ItemStyle-Width="10%">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblStatusName" Text='<%# Eval("StatusName") %>' runat="server"></asp:Label>
                                                    <asp:ImageButton ID="editButton" runat="server" CommandName="Edit" Text="Edit"
                                                        ToolTip="Edit" ImageUrl="~/assets/img/edit.png" Width="30px" />
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:DropDownList ID="ddlStatus" runat="server">
                                                        <asp:ListItem Value="1006">Result Accepted</asp:ListItem>
                                                        <asp:ListItem Value="1005">Re-Test</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:TextBox ID="txtComments" runat="server"></asp:TextBox>
                                                    <asp:ImageButton ID="BtnUpdate" runat="server" CommandName="Update" Text="Update"
                                                        ImageUrl="~/assets/img/accept1.png" Width="30px" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' />
                                                    <asp:ImageButton ID="BtnCancel" runat="server" CommandName="Cancel" Text="Cancel"
                                                        ImageUrl="~/assets/img/cross.png" Width="30px" />
                                                </EditItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                                <table style="width: 100%">
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="btnSave" runat="server" class="btn bg-yellow m-t-15 waves-effect" Text="Re-Test" OnClick="btnSave_Click" Visible="false" />
                                            <asp:Button ID="btnSubmit" runat="server" class="btn bg-teal m-t-15 waves-effect" Text="Accept" OnClick="btnSubmit_Click" Visible="false" />
                                            <asp:Label ID="lblmsg" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
                <asp:HiddenField ID="hdReportID" runat="server" />
                <asp:HiddenField ID="hdEnquiryID" runat="server" />
                <asp:HiddenField ID="hdDepartmentID" runat="server" />
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

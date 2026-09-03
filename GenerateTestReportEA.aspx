<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="GenerateTestReportEA.aspx.cs" Inherits="LIMS.GenerateTestReportEA" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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

                                <h3>Test Report Electrical Accessories Testing Laboratory</h3>
                            </div>
                            <div class="body">
                                <table style="width: 100%">
                                    <tr>
                                        <td><b>Issued To:</b></td>
                                        <td style="padding-right: 20px">
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtCustomerName" class="form-control" AutoComplete="off" placeholder="Customer Name" runat="server" disabled></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>

                                        <td style="padding-left: 20px"><b>ULR No</b></td>
                                        <td>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtUlrNo" AutoComplete="off" placeholder="ULR No." runat="server" BorderColor="YellowGreen"></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                        <td>
                                            <div class="form-group" id="divSignatory" runat="server">
                                                <asp:DropDownList ID="ddlSignatory" runat="server" CssClass="form-control"></asp:DropDownList>
                                            </div>
                                        </td>
                                        <td>
                                            <div class="form-group" id="divNABL" runat="server" visible="false">
                                                <asp:DropDownList ID="ddlReportType" runat="server" CssClass="form-control">
                                                    <asp:ListItem>Select Type</asp:ListItem>
                                                    <asp:ListItem>NABL</asp:ListItem>
                                                    <asp:ListItem>NON NABL</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <table style="width: 100%">
                                    <tr>
                                        <td><b>Test Report No:</b></td>
                                        <td>
                                            <div class="form-group">
                                                <asp:TextBox ID="txtTestReportNo" AutoComplete="off" placeholder="Test Report No." runat="server" BorderColor="YellowGreen"></asp:TextBox>
                                            </div>
                                        </td>
                                        <td><b>Issue Date:</b></td>
                                        <td>
                                            <div class="form-group">
                                                <asp:TextBox ID="txtIssueDate" AutoComplete="off" placeholder="Issue Date" runat="server" BorderColor="YellowGreen"></asp:TextBox>
                                                <ajaxToolkit:CalendarExtender ID="CalendarExtender5" runat="server" TargetControlID="txtIssueDate" Format="dd/MM/yyyy"></ajaxToolkit:CalendarExtender>
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
                                        <td><b>Contact Person:</b></td>
                                        <td>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtContactPerson" class="form-control" AutoComplete="off" placeholder="Contact Person" runat="server" disabled></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Group:</b></td>
                                        <td>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtGroup" class="form-control" AutoComplete="off" placeholder="Group" runat="server" disabled></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                        <td><b>Contact Details:</b></td>
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

                                <table style="width: 100%" border="1">
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
                                                <asp:TextBox ID="txtDateOfMfg" class="form-control" AutoComplete="off" placeholder="Date of Mfg." runat="server"></asp:TextBox>
                                                <ajaxToolkit:CalendarExtender ID="CalendarExtender3" runat="server" TargetControlID="txtDateOfMfg" Format="dd/MM/yyyy"></ajaxToolkit:CalendarExtender>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Sample Quality:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtSampleQuality" AutoComplete="off" placeholder="Sample Quality" runat="server" BorderColor="YellowGreen"></asp:TextBox>
                                            </div>
                                        </td>
                                        <td><b>Date of Expiry:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtDateOfExp" class="form-control" AutoComplete="off" placeholder="Date of Exp." runat="server"></asp:TextBox>
                                                <ajaxToolkit:CalendarExtender ID="CalendarExtender4" runat="server" TargetControlID="txtDateOfExp" Format="dd/MM/yyyy"></ajaxToolkit:CalendarExtender>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Sample Received Date:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtSampleReceivedDate" AutoComplete="off" placeholder="Sample Received Date" runat="server" BorderColor="YellowGreen"></asp:TextBox>
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
                                                <asp:TextBox ID="txtTestRequired" AutoComplete="off" Text="As per TAF" placeholder="Test Required" runat="server" BorderColor="YellowGreen"></asp:TextBox>
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
                                        <td><b>Remarks:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtRemarks" AutoComplete="off" placeholder="Remarks" runat="server" BorderColor="YellowGreen"></asp:TextBox>
                                            </div>
                                        </td>
                                        <%--<td><b>Final Result:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:DropDownList ID="ddlResult" runat="server">
                                                </asp:DropDownList>
                                            </div>
                                        </td>--%>
                                    </tr>
                                </table>
                                <br />

                                <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
                                    >
                                    <Columns>
                                        <asp:TemplateField HeaderText="#" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="sampleidseries" HeaderText="Sample Series" />

                                        <asp:TemplateField HeaderText="Report Name">
                                            <ItemTemplate>
                                                <asp:HyperLink ID="hypReportName" runat="server"
                                                    Text='<%# Eval("ReportName") %>'
                                                    NavigateUrl='<%# "~/Reports/" + Eval("ReportURL") %>'
                                                    Target="_blank"
                                                    CssClass="report-link" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        
                                        <asp:TemplateField HeaderText="Status" ItemStyle-Width="20%">
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
                                                <asp:TextBox ID="txtComments" runat="server" placeholder="Comments"></asp:TextBox>
                                                <asp:ImageButton ID="BtnUpdate" runat="server" CommandName="Update" Text="Update"
                                                    ImageUrl="~/assets/img/accept1.png" Width="30px" />
                                                <asp:ImageButton ID="BtnCancel" runat="server" CommandName="Cancel" Text="Cancel"
                                                    ImageUrl="~/assets/img/cross.png" Width="30px" />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                
                                <br />
                                <table width="100%">
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="btnSubmit" runat="server" class="btn bg-teal m-t-15 waves-effect" Text="Save" OnClick="btnSubmit_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:HiddenField ID="hdReportID" runat="server" />
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="GenerateTestReport_ToyAndBabyWear.aspx.cs" Inherits="LIMS.GenerateTestReport_ToyAndBabyWear" %>

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

                                <h3>Test Report Toys and Baby Wear Laboratory</h3>
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
                                        <td><b>Final Result:</b></td>
                                        <td style="padding-left: 70px; padding-top: 25px;">
                                            <div class="form-group">
                                                <asp:DropDownList ID="ddlResult" runat="server">
                                                </asp:DropDownList>
                                            </div>
                                        </td>

                                    </tr>
                                </table>
                                <br />

                                <asp:GridView ID="gvResultsNoEdit" runat="server" CssClass="table table-striped" AutoGenerateColumns="false">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="1%">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Test Item" SortExpression="TestParameter">
                                            <ItemTemplate>
                                                <%# Eval("TestParameter") + " -> " + Eval("TestSubParameter") + (String.IsNullOrEmpty(Eval("TestSubSubParameter").ToString()) ? "" : " -> " + Eval("TestSubSubParameter")) %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="TestMethodPrint" HeaderText="Test Method" ReadOnly="true" ItemStyle-Width="12%" />
                                        <asp:BoundField DataField="TestResult" HeaderText="Test Result" ReadOnly="true" ItemStyle-Width="10%" />
                                        <asp:BoundField DataField="UnitOfMeasurement" HeaderText="Unit Of Measurement" ItemStyle-Width="7%" ControlStyle-Width="10%" />
                                        <asp:BoundField DataField="specification" HeaderText="Specification" ItemStyle-Width="10%" ControlStyle-Width="10%" />
                                        <asp:BoundField DataField="ColourCodeId1" HeaderText="Colour Code" ItemStyle-Width="10%" ControlStyle-Width="10%" />
                                        <asp:BoundField DataField="EquipmentName" HeaderText="Equipment" ItemStyle-Width="10%" ControlStyle-Width="10%" />
                                        <asp:BoundField DataField="Conclusion" HeaderText="Conclusion" ReadOnly="true" ItemStyle-Width="10%" />
                                    </Columns>
                                </asp:GridView>

                                <asp:GridView ID="gvResults" runat="server" CssClass="table table-striped"
                                    AutoGenerateColumns="False"
                                    DataKeyNames="TestResultID"
                                    OnRowEditing="gvResults_RowEditing"
                                    OnRowCancelingEdit="gvResults_RowCancelingEdit"
                                    OnRowUpdating="gvResults_RowUpdating"
                                    OnRowDataBound="gvResults_RowDataBound">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="1%">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowNumber" runat="server" Text='<%# Container.DataItemIndex + 1 %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Test Item">
                                            <ItemTemplate>
                                                <%# Eval("TestParameter") + " -> " + Eval("TestSubParameter") + 
                                                (String.IsNullOrEmpty(Eval("TestSubSubParameter").ToString()) ? "" : " -> " + Eval("TestSubSubParameter")) %>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="TestMethodPrint" HeaderText="Test Method" ReadOnly="true" />
                                        <asp:BoundField DataField="TestResult" HeaderText="Test Result" ReadOnly="true" />
                                        <asp:BoundField DataField="UnitOfMeasurement" HeaderText="Unit Of Measurement" ReadOnly="true" />
                                        <asp:BoundField DataField="specification" HeaderText="Specification" ReadOnly="true" />
                                        <asp:BoundField DataField="ColourCodeId1" HeaderText="Colour Code" ReadOnly="true" />
                                        <asp:BoundField DataField="EquipmentName" HeaderText="Equipment" ReadOnly="true" />


                                        <asp:TemplateField HeaderText="Conclusion">
                                            <ItemTemplate>
                                                <%# Eval("Conclusion") %>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:DropDownList ID="ddlConclusion" runat="server">
                                                    <asp:ListItem Text="PASS" Value="PASS" />
                                                    <asp:ListItem Text="FAIL" Value="FAIL" />
                                                </asp:DropDownList>
                                            </EditItemTemplate>
                                        </asp:TemplateField>


                                        <asp:CommandField ShowEditButton="True" ButtonType="Button" />
                                    </Columns>
                                </asp:GridView>

                                <asp:GridView ID="gvResultsA01Gen" runat="server" CssClass="table table-striped" AutoGenerateColumns="false">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="1%">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Test Item" SortExpression="TestParameter">
                                            <ItemTemplate>
                                                <%# Eval("TestParameter") + " -> " + Eval("TestSubParameter") + 
                                                    (String.IsNullOrEmpty(Eval("TestSubSubParameter").ToString()) ? "" : " -> " + Eval("TestSubSubParameter")) %>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="TestMethodPrint" HeaderText="Test Method" ReadOnly="true" ItemStyle-Width="15%" />
                                        <asp:BoundField DataField="TestResult" HeaderText="Test Result" ReadOnly="true" ItemStyle-Width="10%" />
                                        <asp:BoundField DataField="UnitOfMeasurement" HeaderText="Unit Of Measurement" ItemStyle-Width="7%" ControlStyle-Width="10%" />
                                        <asp:BoundField DataField="specification" HeaderText="Specification" ItemStyle-Width="10%" ControlStyle-Width="10%" />
                                        <asp:BoundField DataField="EquipmentName" HeaderText="Equipment" ItemStyle-Width="10%" ControlStyle-Width="10%" />
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


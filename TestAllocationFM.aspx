<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="TestAllocationFM.aspx.cs" Inherits="LIMS.TestAllocationFM" %>

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

                                <h3>Test Allocation</h3>
                            </div>
                            <div class="body">
                                <table style="width: 100%">
                                    <tr>
                                        <td style="width: 20%">
                                            <div class="form-group"><b>Sample ID No:</b></div>
                                        </td>
                                        <td style="padding-right: 20px; width: 80%">
                                            <div class="form-group">
                                                <asp:Label ID="lblSampleID" class="form-control" AutoComplete="off" runat="server" disabled></asp:Label>
                                            </div>
                                        </td>
                                    </tr>
                                     <tr>
                                        <td style="width: 20%">
                                            <div class="form-group"><b>Sample Name:</b></div>
                                        </td>
                                        <td style="padding-right: 20px; width: 80%">
                                            <div class="form-group">
                                                <asp:Label ID="lblSampleName" class="form-control" AutoComplete="off" runat="server" disabled></asp:Label>
                                            </div>
                                        </td>
                                    </tr>
                                     <tr>
                                        <td style="width: 20%">
                                            <div class="form-group"><b>Batch#:</b></div>
                                        </td>
                                        <td style="padding-right: 20px; width: 80%">
                                            <div class="form-group">
                                                <asp:Label ID="lblBatchNo" class="form-control" AutoComplete="off" runat="server" disabled></asp:Label>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="form-group"><b>Sample Received Date:</b></div>
                                        </td>
                                        <td style="padding-right: 20px">
                                            <div class="form-group">
                                                <asp:Label ID="lblSampleReceivedDate" class="form-control" AutoComplete="off" placeholder="Customer Name" runat="server" disabled></asp:Label>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="form-group"><b>Product category if any :</b></div>
                                        </td>
                                        <td style="padding-right: 20px">
                                            <div class="form-group">
                                                <asp:Label ID="lblProductCategory" class="form-control" AutoComplete="off" placeholder="Customer Name" runat="server" disabled></asp:Label>
                                            </div>
                                        </td>
                                    </tr>
                                     <tr>
                                        <td>
                                            <div class="form-group"><b>Requirement if any :</b></div>
                                        </td>
                                        <td style="padding-right: 20px">
                                            <div class="form-group">
                                                <asp:Label ID="lblRequirement" class="form-control" AutoComplete="off" runat="server" disabled></asp:Label>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="form-group"><b>Allocated By:</b></div>
                                        </td>
                                        <td style="padding-right: 20px">
                                            <div class="form-group">
                                                <asp:Label ID="lblAllocatedBy" class="form-control" runat="server"></asp:Label>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="form-group"><b>Tests Requested:</b></div>
                                        </td>
                                        <td style="padding-right: 20px">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtTestsRequired" AutoComplete="off" placeholder="" Text="As per TAF" runat="server"></asp:TextBox>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                                <table>
                                    <tr>
                                        <td colspan="2">
                                            <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="false" DataKeyNames="CRFSampleId"
                                                CssClass="table table-striped table-bordered table-hover table-checkable order-column"
                                                BorderWidth="3px" CellPadding="4" Width="200%" OnRowCommand="gvResults_RowCommand"
                                                OnRowUpdating="gvResults_RowUpdating" OnRowEditing="gvResults_RowEditing"
                                                OnRowCancelingEdit="gvResults_RowCancelingEdit" OnRowDataBound="gvResults_RowDataBound">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="70">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                        </ItemTemplate>
                                                        <ItemStyle Width="70px"></ItemStyle>
                                                    </asp:TemplateField>
                                                     <asp:TemplateField HeaderText="Test Item" SortExpression="TestItem">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblTestParameter" runat="server" Text='<%# Eval("TestItem") %>'></asp:Label>
                                                        </ItemTemplate>
                                                          <EditItemTemplate>
                                                            <asp:TextBox ID="txtTestParameter" runat="server" ReadOnly="true" Text='<%# Bind("TestParameter") %>'></asp:TextBox>
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>
                                                   <asp:TemplateField HeaderText="TestMethod" SortExpression="TestMethod">
                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="txtTestMethod" runat="server" ReadOnly="true" Text='<%# Bind("TestMethod") %>'></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblTestMethod" runat="server" Text='<%# Bind("TestMethod") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>                                                   
                                                    <asp:TemplateField HeaderText="UnitOfMeasurement" SortExpression="UnitOfMeasurement">
                                                        <EditItemTemplate>
                                                            <asp:DropDownList ID="ddlUOM" runat="server"></asp:DropDownList>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblUOM" runat="server" Text='<%# Bind("UnitOfMeasurement") %>'></asp:Label>
                                                            <asp:Label ID="lblSupervisor" runat="server" Text='<%# Bind("Supervisor") %>' Visible="false"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="specification" HeaderText="Specification" />                                                     
                                                    <asp:BoundField DataField="EmployeeName" HeaderText="Allocated To" />
                                                    <asp:CommandField ShowEditButton="true" ButtonType="Image" EditImageUrl="~/assets/img/edit.png" ControlStyle-Width="30px" UpdateImageUrl="~/assets/img/accept1.png" CancelImageUrl="~/assets/img/cross.png" />

                                                </Columns>
                                                <FooterStyle BackColor="White" ForeColor="#333333" />
                                                <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="Black" />
                                                <PagerStyle BackColor="#b34788" ForeColor="White" HorizontalAlign="Center" />
                                                <RowStyle BackColor="White" ForeColor="#333333" />
                                                <SelectedRowStyle BackColor="#ffe6e6" Font-Bold="True" ForeColor="Black" />
                                                <SortedAscendingCellStyle BackColor="#F7F7F7" />
                                                <SortedAscendingHeaderStyle BackColor="#487575" />
                                                <SortedDescendingCellStyle BackColor="#E5E5E5" />
                                                <SortedDescendingHeaderStyle BackColor="#275353" />
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                <ContentTemplate>
                                                    <table width="90%">
                                                        <tr>
                                                            <td>&nbsp;</td>
                                                            <td align="center">
                                                                <asp:Button ID="btnAllocate" runat="server" class="btn bg-teal m-t-15 waves-effect" Text="Allocate" OnClick="btnAllocate_Click" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                </table>
                                <br />
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

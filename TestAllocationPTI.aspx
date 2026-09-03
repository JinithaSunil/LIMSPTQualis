<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="TestAllocationPTI.aspx.cs" Inherits="LIMS.TestAllocationPTI" %>

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
                            
                                <h3>Test Allocation</h3>
                            </div>
                            <div class="body">
                                <table style="width: 100%">
                                    <tr>
                                        <td style="width:20%"><div class="form-group"><b>Sample ID No:</b></div></td>
                                        <td style="padding-right: 20px;width:80%">
                                            <div class="form-group">
                                                <asp:Label ID="lblSampleID" class="form-control" AutoComplete="off" placeholder="Customer Name" runat="server" disabled></asp:Label>
                                            </div>
                                        </td>                                       
                                    </tr>
                                    <tr>
                                        <td><div class="form-group"><b>Sample Received Date:</b></div></td>
                                        <td style="padding-right: 20px">
                                            <div class="form-group">
                                                <asp:Label ID="lblSampleReceivedDate" class="form-control" AutoComplete="off" placeholder="Customer Name" runat="server" disabled></asp:Label>
                                            </div>
                                        </td>                                       
                                    </tr>
                                    <tr>
                                        <td><div class="form-group"><b>Product category if any :</b></div></td>
                                        <td style="padding-right: 20px">
                                            <div class="form-group">
                                                <asp:Label ID="lblProductCategory" class="form-control" AutoComplete="off" placeholder="Customer Name" runat="server" disabled></asp:Label>
                                            </div>
                                        </td>                                        
                                    </tr>
                                    <tr>
                                        <td><div class="form-group"><b>Sample Allotted to:</b></div></td>
                                        <td style="padding-right: 20px">
                                            <div class="form-group">
                                                <asp:Label ID="lblAllocatedTo" AutoComplete="off" placeholder="" runat="server" ></asp:Label>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><div class="form-group"><b>Allocated By:</b></div></td>
                                        <td style="padding-right: 20px">
                                            <div class="form-group">
                                                <asp:Label ID="lblAllocatedBy" class="form-control" runat="server"></asp:Label>
                                            </div>
                                        </td>                                        
                                    </tr>
                                    <tr>
                                        <td><div class="form-group"><b>Tests Requested:</b></div></td>
                                        <td style="padding-right: 20px">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtTestsRequired" AutoComplete="off" placeholder="" runat="server" ></asp:TextBox>
                                            </div>
                                        </td>
                                    </tr>
                                    </table>
                                <table>
                                    <tr>
                                        <td colspan="2">
                                            <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="false" DataKeyNames="CRFSampleId" CssClass="table table-striped table-bordered table-hover table-checkable order-column" BorderWidth="3px" CellPadding="4" Width="200%" OnRowCommand="gvResults_RowCommand" OnRowUpdating="gvResults_RowUpdating" OnRowEditing="gvResults_RowEditing" OnRowCancelingEdit="gvResults_RowCancelingEdit">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="70">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                        </ItemTemplate>
                                                        <ItemStyle Width="70px"></ItemStyle>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="SampleName" HeaderText="Sample Name / Description" ReadOnly="true" />
                                                    <asp:BoundField DataField="BatchNo" HeaderText="Batch No" ReadOnly="true" />
                                                    <asp:BoundField DataField="TestMethod" HeaderText="Test Method" />
                                                    <asp:BoundField DataField="TestParameter" HeaderText="Test Parameter"/>
                                                    <asp:BoundField DataField="UnitOfMeasurement" HeaderText="Unit Of Measurement" />
                                                    <asp:BoundField DataField="specification" HeaderText="Specification" />
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

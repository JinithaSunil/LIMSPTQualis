<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="AllocateMultiple_ToyAndBabyWear.aspx.cs" Inherits="LIMS.AllocateMultiple_ToyAndBabyWear" %>

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

                                <h3>Allocate Multiple</h3>
                            </div>
                            <div class="body">
                                <div class="row clearfix">
                                    <div class="col-lg-4">
                                        <asp:DropDownList ID="ddlAllocate" runat="server" class="form-control"></asp:DropDownList>
                                    </div>
                                    <div class="col-lg-4">
                                        <asp:Button ID="btnAllocateAll" runat="server" Text="Allocate" CssClass="btn btn-primary" OnClick="btnAllocate_Click" />
                                    </div>
                                </div>
                                <div class="row clearfix">
                                    <div class="form-group">
                                        <asp:GridView ID="gvSampleList" runat="server" AutoGenerateColumns="false" DataKeyNames="ID,InquiryID,SampleID2,CloneCRFSampleID,CRFSampleID" class="table table-bordered table-striped table-hover js-basic-example dataTable" OnRowDataBound="gvSampleList_RowDataBound" OnRowCommand="gvSampleList_RowCommand">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="70">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="30px"></ItemStyle>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="TestItem" HeaderText="Test Item" />
                                                <asp:BoundField DataField="TestMethod" HeaderText="TestMethod" />
                                                <asp:TemplateField HeaderText="Allocation">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAllocatedTo" runat="server" Text='<%#Eval("EmployeeName") %>' Visible="false"></asp:Label>
                                                        <asp:DropDownList ID="ddlAllocatedTo" runat="server" class="form-control"></asp:DropDownList>
                                                        <asp:Label ID="lblEmployeeID" runat="server" Visible="false" Text='<%# Eval("EmployeeID") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="150px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="View" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:HyperLink ID="hypAllocation" runat="server" Target="_blank">- Allocation</asp:HyperLink><br />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="120px" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Status">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStatusName" Text='<%# Eval("StatusName") %>' runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="120px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action">
                                                    <ItemTemplate>
                                                        <asp:Button ID="btnAllocate" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' CommandName="Save" Text="Allocate" runat="server" class="btn btn-primary btn-xs" Height="30px" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <FooterStyle BackColor="White" ForeColor="#333333" />
                                            <PagerStyle BackColor="#b34788" ForeColor="White" HorizontalAlign="Center" />
                                            <RowStyle BackColor="White" ForeColor="#333333" Height="50px" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <SelectedRowStyle BackColor="#339966" ForeColor="White" />
                                            <SortedAscendingCellStyle BackColor="#F7F7F7" />
                                            <SortedAscendingHeaderStyle BackColor="#487575" />
                                            <SortedDescendingCellStyle BackColor="#E5E5E5" />
                                            <SortedDescendingHeaderStyle BackColor="#275353" />
                                        </asp:GridView>
                                        <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="Submit" CssClass="btn btn-primary" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Label ID="lblSampleID" runat="server" Visible="false"></asp:Label>
    <asp:Label ID="lblSampleUID" runat="server" Visible="false"></asp:Label>
    <asp:Label ID="lblCloneCRFSampleID" runat="server" Visible="false"></asp:Label>
</asp:Content>



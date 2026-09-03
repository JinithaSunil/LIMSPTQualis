<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeBehind="CustomerCRFList.aspx.cs" Inherits="LIMS.CustomerCRFList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">

    <!-- Basic Examples -->
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card"><hr class="violet"><div class="header">
                

                    <div class="row">
                        <div class="col-sm-6">
                            <h2>TAF LIST</h2>
                        </div>
                        <div class="col-sm-4">
                            <asp:TextBox ID="txtSearch" runat="server" class="form-control" placeholder="Search Customer" OnTextChanged="txtSearch_TextChanged" AutoPostBack="true"></asp:TextBox>
                        </div>
                        <div class="col-sm-1">
                            <asp:ImageButton ID="imgSearch" runat="server" ImageUrl="images/search.png" OnClick="imgSearch_Click" />
                        </div>
                    </div>

                </div>
                <div class="body">
                    <div class="table-responsive">
                        Records: <span id="spanCount" runat="server"></span>
                        <asp:Panel ID="pnlView" runat="server">
                            <asp:GridView ID="gvCRFList" runat="server" AutoGenerateColumns="False" Width="100%"
                                DataKeyNames="CustomerMasterID,InquiryID,CloneCRFSampleID" class="table table-bordered table-striped table-hover js-basic-example dataTable"
                                OnPageIndexChanging="gvCRFList_PageIndexChanging" AllowPaging="true" PageSize="5"
                                OnRowDataBound="gvCRFList_RowDataBound" AllowSorting="true" OnSorting="gvCRFList_Sorting"
                                OnRowCommand="gvCRFList_RowCommand">
                                <Columns>
                                     <asp:TemplateField HeaderText="Clone#">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCloneCRFSampleID" Text='<%# Eval("CloneCRFSampleID") %>' runat="server" />
                                        </ItemTemplate>
                                        <ItemStyle Width="5%"></ItemStyle>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Request No.">
                                        <ItemTemplate>
                                            <asp:Label ID="lblInquiryID" Text='<%# "R/" + Eval("RequestNo") %>' runat="server" />     
                                            <asp:Label ID="lblDivision" runat="server" Text='<%#Eval("DepartmentName") %>'></asp:Label>
                                              <asp:Label ID="lblWO" Text='<%# Eval("WorkOrderNumber") %>' runat="server" Visible="false" />                                        
                                            <asp:Label ID="lblSampleID" Text='<%# Eval("SampleID") %>' runat="server" Visible="false" />                                        
                                        </ItemTemplate>
                                        <ItemStyle Width="5%"></ItemStyle>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="InquiryID" Visible="false" />
                                    <asp:TemplateField HeaderText="Customer" SortExpression="CustomerName">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCustomer" runat="server" Text='<%# Eval("CustomerName").ToString().ToUpper()  %>' Target="_blank" NavigateUrl='<%# Eval("CustomerMasterID", "CustomerProfile.aspx?ID={0}") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="25%"></ItemStyle>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Enquiry Details">
                                        <ItemTemplate>
                                            <asp:Label ID="lblInquiryDetail" runat="server" Text='<%# System.Web.HttpUtility.HtmlDecode(Eval("InquiryDetail").ToString()) %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle Width="20%"></ItemStyle>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                            <span>
                                                <asp:HyperLink ID="hypCRF" runat="server" Text="Customer TAF" Target="_blank" Visible="false"></asp:HyperLink><br />
                                               <asp:HyperLink ID="hypCloneCRF" runat="server" Text="Final TAF" Target="_blank" Visible="false"></asp:HyperLink><br />
                                                <asp:Button ID="btnApproveCRF" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' CommandName="CRFApprove" Text="Review" runat="server" class="btn btn-success waves-effect" Height="30px" /><br /><br />
                                                <asp:HyperLink ID="hypWO" runat="server" Text="WO" Target="_blank" Visible="false"></asp:HyperLink><br />
                                            </span>
                                        </ItemTemplate>
                                        <ItemStyle Width="15%"></ItemStyle>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Sample Received">
                                        <ItemTemplate>
                                            <span>
                                                <div class="btn-group" role="group">
                                                    <asp:Button ID="btnSampleReceivedOn" runat="server" CssClass="btn btn-default" Text="Yes" CommandName="On" CommandArgument='<%# DataBinder.Eval(Container,"RowIndex") %>' Visible="false" />
                                                    <asp:Button ID="btnSampleReceivedOff" runat="server" CssClass="btn btn-danger" Text="No" CommandName="Off" CommandArgument='<%# DataBinder.Eval(Container,"RowIndex") %>' Visible="false" />
                                                </div>
                                                <asp:Button ID="btnSampleManagement" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' CommandName="SampleManagement" Text="Sample Mangmt" runat="server" class="btn btn-primary btn-xs" Height="30px" Visible="false" />
                                            </span>
                                        </ItemTemplate>
                                        <ItemStyle Width="13%"></ItemStyle>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Status">
                                        <ItemTemplate>
                                            <asp:Label ID="lblStatusName" Text='<%# Eval("StatusName") %>' runat="server"></asp:Label>
                                            <asp:Label ID="lblCRFStatus" Text='<%# Eval("CRFStatus") %>' runat="server"></asp:Label>
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
                            <asp:Label ID="lblNoRecords" runat="server" Text="No Data!" Font-Bold="True" Font-Size="X-Large" ForeColor="Red" Visible="False"></asp:Label>
                            <br />
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

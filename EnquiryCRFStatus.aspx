<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="EnquiryCRFStatus.aspx.cs" Inherits="LIMS.EnquiryCRFStatus" %>

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
                            
                                <h3>REQUEST v/s TAF</h3>
                            </div>
                            <div class="body">
                                <div class="row clearfix">
                                    <div class="form-group" style="overflow-x: scroll" onscroll="true">
                                        <asp:GridView ID="gvEnquiryList" runat="server" AutoGenerateColumns="false"
                                            AllowSorting="true" DataKeyNames="InquiryID"
                                            class="table table-bordered table-striped table-hover js-basic-example dataTable"
                                            OnSorting="gvEnquiryList_Sorting" OnRowEditing="gvEnquiryList_RowEditing" OnRowDataBound="gvEnquiryList_RowDataBound"
                                            OnRowUpdating="gvEnquiryList_RowUpdating" OnRowCancelingEdit="gvEnquiryList_RowCancelingEdit">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl.No">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="30px"></ItemStyle>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Request No." SortExpression="RequestNo">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEnquiry" runat="server" Text='<%# "R/" + Eval("RequestNo").ToString().ToUpper() %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="5%"></ItemStyle>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Dept" SortExpression="DepartmentName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="DepartmentName" runat="server" Text='<%# Eval("DepartmentName")%>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlDepartment_SelectedIndexChanged"></asp:DropDownList>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="SubGroup" SortExpression="SubGroupName">
                                                    <ItemTemplate>
                                                        <asp:Label ID="SubGroupName" runat="server" Text='<%# Eval("SubGroupName")%>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:DropDownList ID="ddlSubGroupName" runat="server" CssClass="form-control"></asp:DropDownList>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="CustomerName" HeaderText="Customer" ControlStyle-Width="20%" ReadOnly="true" SortExpression="CustomerName" />
                                                <asp:BoundField DataField="StatusName" HeaderText="Status" ControlStyle-Width="20%" ReadOnly="true" SortExpression="StatusName" />
                                                <asp:BoundField DataField="created" HeaderText="Enq.Date" DataFormatString="{0:dd, MMM yy}" ReadOnly="true" ControlStyle-Width="20%" SortExpression="created" />
                                                <asp:TemplateField HeaderText="Request Details">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblInquiryDetail" runat="server" Text='<%# Limit(System.Web.HttpUtility.HtmlDecode(Eval("InquiryDetail").ToString()),40) %>' ToolTip='<%# System.Web.HttpUtility.HtmlDecode(Eval("InquiryDetail").ToString()) %>'></asp:Label>
                                                        <asp:LinkButton ID="ReadMoreLinkButton" runat="server"
                                                            Text="Read More"
                                                            Visible='<%# SetVisibility(Eval("InquiryDetail"), 40) %>'
                                                            OnClick="ReadMoreLinkButton_Click">
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="20%"></ItemStyle>
                                                </asp:TemplateField>
                                                <asp:CommandField ShowEditButton="false" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="EditPurchasegroupAndSubgroup.aspx.cs" Inherits="LIMS.EditPurchasegroupAndSubgroup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
     <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <h2>EDIT PURCHASE GROUP & SUBGROUP</h2>
                </div>

                <div class="body">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <!-- Purchase Group Section -->
        <div class="row clearfix">
            <div class="col-md-12">
                <h4>Purchase Group Details</h4>
                <hr />
            </div>
        </div>

        <div class="row clearfix">
            <div class="col-sm-4" style="display:none;">
                <div class="form-group">
                    <div class="form-line">
                        <label for="txtPurchaseGroupID">Purchase Group ID</label>
                        <asp:TextBox ID="txtPurchaseGroupID" runat="server" CssClass="form-control" ReadOnly="true" Enabled="false"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="col-sm-6">
                <div class="form-group">
                    <div class="form-line">
                        <label for="txtPurchaseGroupName">Purchase Group Name <span style="color:red">*</span></label>
                        <asp:TextBox ID="txtPurchaseGroupName" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="col-sm-6">
                <div class="form-group">
                    <div class="form-line">
                        <label for="ddlItemType">Item Type <span style="color:red">*</span></label>
                        <asp:DropDownList runat="server" ID="ddlItemType" Height="35px" CssClass="form-control"></asp:DropDownList>
                    </div>
                </div>
            </div>
        </div>

        <div class="row clearfix">
            <div class="col-md-12 text-center">
                <asp:Button ID="btnUpdateGroup" runat="server" Text="Update Purchase Group" CssClass="btn btn-primary" OnClick="btnUpdateGroup_Click" />
                <asp:Button ID="btnCancelGroup" runat="server" Text="Cancel" CssClass="btn btn-default" OnClick="btnCancelGroup_Click" />
            </div>
        </div>

        <br /><br />

        <!-- Purchase SubGroup Section -->
        <div class="row clearfix">
            <div class="col-md-12">
                <h4>Purchase SubGroups</h4>
                <hr />
            </div>
        </div>

        <div class="row clearfix">
            <div class="col-md-12">
                <asp:GridView ID="gvSubGroups" runat="server" AutoGenerateColumns="False" 
                    CssClass="table table-striped table-hover table-bordered" Width="100%"
                    OnRowEditing="gvSubGroups_RowEditing" 
                    OnRowCancelingEdit="gvSubGroups_RowCancelingEdit"
                    OnRowUpdating="gvSubGroups_RowUpdating"
                    OnRowDeleting="gvSubGroups_RowDeleting"
                    DataKeyNames="PurchaseSubGroupID">
                    <Columns>
                        <asp:TemplateField HeaderText="Sl#" HeaderStyle-Width="5%">
                            <ItemTemplate>
                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="SubGroup ID" HeaderStyle-Width="10%" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblSubGroupID" runat="server" Text='<%# Eval("PurchaseSubGroupID") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="SubGroup Name" HeaderStyle-Width="30%">
                            <ItemTemplate>
                                <asp:Label ID="lblSubGroupName" runat="server" Text='<%# Eval("PurchaseSubGroupName") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtSubGroupName" runat="server" Text='<%# Bind("PurchaseSubGroupName") %>' CssClass="form-control"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Item Type" HeaderStyle-Width="25%">
                            <ItemTemplate>
                                <asp:Label ID="lblItemType" runat="server" Text='<%# Eval("ItemType") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Actions" HeaderStyle-Width="20%">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" CssClass="btn btn-info btn-sm">
                                    <i class="material-icons">edit</i> Edit
                                </asp:LinkButton>
                               <%-- <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" CssClass="btn btn-danger btn-sm"
                                    OnClientClick="return confirm('Are you sure you want to delete this subgroup?');">
                                    <i class="material-icons">delete</i> Delete
                                </asp:LinkButton>--%>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:LinkButton ID="btnUpdate" runat="server" CommandName="Update" CssClass="btn btn-success btn-sm">
                                    <i class="material-icons">save</i> Update
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" CssClass="btn btn-default btn-sm">
                                    <i class="material-icons">cancel</i> Cancel
                                </asp:LinkButton>
                            </EditItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <br />

        <!-- Add New SubGroup Section -->
    <%--    <div class="row clearfix">
            <div class="col-md-12">
                <h5>Add New SubGroup</h5>
            </div>
        </div>

        <div class="row clearfix">
            <div class="col-sm-6">
                <div class="form-group">
                    <div class="form-line">
                        <label for="txtNewSubGroupName">SubGroup Name <span style="color:red">*</span></label>
                        <asp:TextBox ID="txtNewSubGroupName" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="col-sm-6">
                <br />
                <asp:Button ID="btnAddSubGroup" runat="server" Text="Add SubGroup" CssClass="btn btn-success" OnClick="btnAddSubGroup_Click" />
            </div>
        </div>--%>

    </ContentTemplate>
</asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
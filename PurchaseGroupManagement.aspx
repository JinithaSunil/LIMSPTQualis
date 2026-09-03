<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="PurchaseGroupManagement.aspx.cs" Inherits="LIMS.PurchaseGroupManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <div class="card">
        <div class="header">
            <h2>PURCHASE GROUP MANAGEMENT</h2>
        </div>

        <div class="body">
            <asp:UpdatePanel ID="upMain" runat="server">
                <ContentTemplate>

                    <!-- ADD PURCHASE GROUP -->
                    <div class="row">
                        <div class="col-md-6">
                            <label>Purchase Group</label>
                            <asp:TextBox ID="txtPurchaseGroup" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>

                        <div class="col-md-6">
                            <label>Item Type</label>
                            <asp:DropDownList ID="ddlItemType" runat="server" CssClass="form-control"></asp:DropDownList>
                        </div>
                    </div>

                    <br />

                    <div class="row">
                        <div class="col-md-12 text-right">
                            <asp:Button ID="btnSaveGroup"
                                runat="server"
                                Text="Save Purchase Group"
                                CssClass="btn btn-primary"
                                OnClick="btnSaveGroup_Click" />
                        </div>
                    </div>

                    <hr />

                    <!-- ADD SUB GROUP -->
                    <div class="row">
                        <div class="col-md-6">
                            <label>Select Purchase Group</label>
                            <asp:DropDownList ID="ddlPurchaseGroup"
                                runat="server"
                                CssClass="form-control"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlPurchaseGroup_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>

                        <div class="col-md-6">
                            <label>Purchase Sub Group</label>
                            <asp:TextBox ID="txtSubGroup"
                                runat="server"
                                CssClass="form-control">
                            </asp:TextBox>
                        </div>
                    </div>

                    <br />

                    <div class="row">
                        <div class="col-md-12 text-right">
                            <asp:Button ID="btnSaveSubGroup"
                                runat="server"
                                Text="Save Sub Group"
                                CssClass="btn btn-secondary"
                                OnClick="btnSaveSubGroup_Click" />
                        </div>
                    </div>

                    <hr />

                    <!-- GRID -->
                    <asp:GridView ID="gvPurchase"
                        runat="server"
                        CssClass="table table-striped table-hover"
                        AutoGenerateColumns="False"
                        DataKeyNames="PurchaseGroupID,PurchaseSubGroupID,GroupIsDisabled"
                        OnRowCommand="gvPurchase_RowCommand">

                        <Columns>

                            <asp:BoundField DataField="PurchaseGroupName" HeaderText="Purchase Group" />
                            <asp:BoundField DataField="PurchaseSubGroupName" HeaderText="Sub Group" />
                            <asp:BoundField DataField="ItemType" HeaderText="Item Type" />

                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <asp:Label ID="lblStatus" runat="server"
                                        Text='<%# Convert.ToInt32(Eval("GroupIsDisabled")) == 0 ? "Active" : "Inactive" %>'
                                        CssClass='<%# Convert.ToInt32(Eval("GroupIsDisabled")) == 0 ? "badge bg-success" : "badge bg-danger" %>'>
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnToggle"
                                        runat="server"
                                        Text="Toggle"
                                        CommandName="Toggle"
                                        CommandArgument='<%# Container.DataItemIndex %>'
                                        CssClass="btn btn-warning btn-sm">
                                    </asp:LinkButton>

                                    <asp:HyperLink ID="hypEdit"
                                        runat="server"
                                        NavigateUrl='<%# "EditPurchasegroupAndSubgroup.aspx?PurchaseGroupID=" + Eval("PurchaseGroupID") + "&PurchaseSubGroupID=" + Eval("PurchaseSubGroupID") %>'
                                        CssClass="btn btn-info btn-sm">
                                        Edit
                                    </asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="MaterialRequestFormEdit.aspx.cs" Inherits="LIMS.MaterialRequestFormEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="page-wrapper">
        <div class="page-content">

            <div class="row">
                <div class="col-xl-12 mx-auto">
                    <div class="card">

                        <div class="header bg-violet">
                            <h2 style="color: white !important">MATERIAL REQUISITION EDIT</h2>
                        </div>

                        <div class="body">

                            <asp:Label ID="lblPageID" runat="server"></asp:Label>
                            <asp:Label ID="lblMRNumber" runat="server" Visible="false"></asp:Label>
                            <asp:Label ID="lblMRMID" runat="server" Visible="false"></asp:Label>

                            <div class="row mt-4 mb-4">

                                <div class="col-md-2">
                                    <label for="input1" class="form-label">Item Category </label>
                                    <asp:DropDownList ID="ddlCategory" class="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-2">
                                    <label for="input1" class="form-label">Item Code </label>
                                    <asp:DropDownList ID="ddlItemCode" class="form-control" runat="server"
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlItemCode_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-2">
                                    <label for="input1" class="form-label">Item Name</label>
                                    <asp:TextBox ID="txtItemName" runat="server" class="form-control" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-md-2">
                                    <label for="input1" class="form-label">Current Stock</label>
                                    <asp:TextBox ID="txtCurrentStock" runat="server" class="form-control" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-md-2">
                                    <label for="input1" class="form-label">Min. Stock</label>
                                    <asp:TextBox ID="txtMinStock" runat="server" class="form-control" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-md-2">
                                    <label for="input1" class="form-label">UOM</label>
                                    <asp:DropDownList ID="ddlUOM" CssClass="form-control" runat="server">
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="row mt-4 mb-4">
                                <div class="col-md-2">
                                    <label for="input1" class="form-label">Require Qty.</label>
                                    <asp:TextBox ID="txtRequireQty" runat="server" class="form-control"></asp:TextBox>
                                    <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtRequireQty" ValidationGroup="outsideGrid"
                                    ErrorMessage="Current Stock is required." Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>--%>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtRequireQty" ValidationGroup="outsideGrid"
                                        ErrorMessage="Invalid input. Please enter a positive number." ValidationExpression="^\d*\.?\d+$"
                                        Display="Dynamic" CssClass="text-danger"></asp:RegularExpressionValidator>
                                </div>

                                <div class="col-md-2">
                                    <label for="input1" class="form-label">Require Date</label>
                                    <asp:TextBox ID="txtRequiredDate" runat="server" class="form-control" TextMode="Date"></asp:TextBox>
                                </div>

                                <div class="col-md-2">
                                    <a id="MyAnchor" onserverclick="btnAdd_Click" runat="server" class="btn btn-primary" style="margin-top: 20px;"><i class="fa fa-plus"></i></a>
                                    <%--<asp:Button ID="btnAdd" runat="server" Text="ADD" class="btn btn-primary" Style="margin-top: 28px;" OnClick="btnAdd_Click" />--%>
                                </div>
                            </div>

                            <div class="row mt-4 mb-4">

                                <asp:GridView ID="GridView1" runat="server" CssClass="table table-bordered" HeaderStyle-CssClass="table-header-bg"
                                    OnRowEditing="GridView1_RowEditing" OnRowUpdating="GridView1_RowUpdating" OnRowCancelingEdit="GridView1_RowCancelingEdit"
                                    OnRowDeleting="GridView1_RowDeleting" AutoGenerateColumns="False" DataKeyNames="MR_ItemCode">
                                    <Columns>
                                        <asp:TemplateField HeaderText="#">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="MR_ItemCode" HeaderText="Item Code" SortExpression="PoBalanceQty" ReadOnly="true" />
                                        <asp:BoundField DataField="MR_ItemName" HeaderText="Item Name" SortExpression="PoBasicPrice" ReadOnly="true" />
                                        <asp:BoundField DataField="MR_CurrentStock" HeaderText="Current Stock" SortExpression="PoGSTPercentage" ReadOnly="true" />
                                        <asp:BoundField DataField="MR_MinStock" HeaderText="Min. Stock" SortExpression="PoGSTPercentage" ReadOnly="true" />
                                        <asp:BoundField DataField="MR_UOM" HeaderText="UOM" SortExpression="PoGSTPercentage" ReadOnly="true" />

                                        <asp:TemplateField HeaderText="Require Qty.">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRequireQty" runat="server" Text='<%# Bind("MR_EmployeeRequireItemQty") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtRequireQty" runat="server" Text='<%# Bind("MR_EmployeeRequireItemQty") %>'></asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Require Date">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRequireDate" runat="server" Text='<%# Bind("MR_RequireDate") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtRequireDate" runat="server" Text='<%# Bind("MR_RequireDate") %>' TextMode="Date"></asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateField>

                                        <asp:CommandField ShowEditButton="True" />
                                        <asp:CommandField ShowDeleteButton="True" />
                                    </Columns>
                                </asp:GridView>

                            </div>

                            <div class="row g-3 mt-3 mb-3">
                                <div class="col-md-4">
                                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" class="btn btn-primary" OnClick="btnSubmit_Click" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!--end row-->
    </div>
</asp:Content>

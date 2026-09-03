<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="MaterialRequestForm.aspx.cs" Inherits="LIMS.MaterialRequestForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">

    <%--<div class="page-wrapper">
        <div class="page-content">--%>
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <div class="header bg-violet">
                    <h2 style="color: white !important">MATERIAL REQUISITION</h2>
                </div>

                <%--<hr class="violet">
                <div class="header">
                    <h2>Material Request</h2>
                </div>--%>

                <div class="body">

                    <div class="row mt-4 mb-4">

                        <div class="col-md-2">
                            <label for="input1" class="form-label">Item Category </label>
                            <asp:DropDownList ID="ddlCategory" class="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvddlCategory" runat="server" ControlToValidate="ddlCategory"
                                InitialValue="" ErrorMessage="Please select an Item Category." Display="Dynamic" CssClass="text-danger">
                            </asp:RequiredFieldValidator>
                        </div>

                        <div class="col-md-2">
                            <label for="input1" class="form-label">Item Code </label>
                            <asp:DropDownList ID="ddlItemCode" class="form-control" runat="server"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlItemCode_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvItemCode" runat="server" ControlToValidate="ddlItemCode"
                                InitialValue="" ErrorMessage="Please select an Item Code." Display="Dynamic" CssClass="text-danger">
                            </asp:RequiredFieldValidator>

                        </div>

                        <div class="col-md-6">
                            <label for="input1" class="form-label">Item Name</label>
                            <asp:TextBox ID="txtItemName" runat="server" class="form-control" ReadOnly="true"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvItemName" runat="server" ControlToValidate="txtItemName"
                                ErrorMessage="Item Name is required." Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                        </div>


                    </div>

                    <div class="row mt-4 mb-4">
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

                        <div class="col-md-2">
                            <label for="input1" class="form-label">Require Qty.</label>
                            <asp:TextBox ID="txtRequireQty" runat="server" class="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtRequireQty"
                                ErrorMessage="Current Stock is required." Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtRequireQty"
                                ErrorMessage="Invalid input. Please enter a positive number." ValidationExpression="^\d*\.?\d+$"
                                Display="Dynamic" CssClass="text-danger"></asp:RegularExpressionValidator>
                        </div>

                        <div class="col-md-2">
                            <label for="input1" class="form-label">Require Date</label>
                            <asp:TextBox ID="txtRequiredDate" runat="server" class="form-control" TextMode="Date"></asp:TextBox>
                        </div>

                        <div class="col-md-2">
                            <a id="MyAnchor" onserverclick="btnAdd_Click" runat="server" class="btn btn-primary" style="margin-top: 21px;"><i class="fa fa-plus" aria-hidden="true"></i></a>
                        </div>
                    </div>

                    <div class="row clearfix mt-4 mb-4">
                        <div class="col-sm-12">
                            <div id="divGrid" runat="server" class="table-responsive">
                                <asp:GridView ID="GridView1" runat="server" CssClass="table table-bordered" HeaderStyle-CssClass="table-header-bg"
                                    OnRowEditing="GridView1_RowEditing" OnRowUpdating="GridView1_RowUpdating" OnRowCancelingEdit="GridView1_RowCancelingEdit"
                                    OnRowDeleting="GridView1_RowDeleting" AutoGenerateColumns="False">
                                    <Columns>
                                        <asp:TemplateField HeaderText="#">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="ItemCode" HeaderText="Item Code" SortExpression="PoBalanceQty" ReadOnly="true" />
                                        <asp:BoundField DataField="ItemName" HeaderText="Item Name" SortExpression="PoBasicPrice" ReadOnly="true" />
                                        <asp:BoundField DataField="CurrentStock" HeaderText="Current Stock" SortExpression="PoGSTPercentage" ReadOnly="true" />
                                        <asp:BoundField DataField="MinStock" HeaderText="Min. Stock" SortExpression="PoGSTPercentage" ReadOnly="true" />
                                        <asp:BoundField DataField="UOM" HeaderText="UOM" SortExpression="PoGSTPercentage" ReadOnly="true" />

                                        <asp:TemplateField HeaderText="Require Qty." ItemStyle-Width="100px">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRequireQty" runat="server" Text='<%# Bind("RequireQty") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtRequireQty" runat="server" Text='<%# Bind("RequireQty") %>' ControlStyle-Width="100px"></asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Require Date">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRequireDate" runat="server" Text='<%# Bind("RequireDate") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtRequireDate" runat="server" Text='<%# Bind("RequireDate") %>' TextMode="Date"></asp:TextBox>
                                            </EditItemTemplate>
                                        </asp:TemplateField>

                                        <asp:CommandField ShowEditButton="True" />
                                        <asp:CommandField ShowDeleteButton="True" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>

                    <div class="row g-3 mt-3 mb-3">
                        <div class="col-md-4">
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>

    <%-- </div>       
    </div>--%>
</asp:Content>

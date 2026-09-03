<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="InventoryEdit.aspx.cs" Inherits="LIMS.InventoryEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="page-wrapper">
        <div class="page-content">
            <!--breadcrumb-->
            <%--<div class="page-breadcrumb d-none d-sm-flex align-items-center mb-3">
            <div class="breadcrumb-title pe-3">Inventory</div>
            <div class="ps-3">
                <nav aria-label="breadcrumb">
                    <ol class="breadcrumb mb-0 p-0">
                        <li class="breadcrumb-item">
                            <a href="Dashboard.aspx">
                                <i class="bx bx-home-alt"></i>
                            </a>
                        </li>
                        <li class="breadcrumb-item">
                            <a href="InventoryShow.aspx">Inventory Show
                            </a>
                        </li>
                        <li class="breadcrumb-item active" aria-current="page">Inventory EDIT</li>
                    </ol>
                </nav>
            </div>
        </div>--%>
            <!--end breadcrumb-->
            <div class="row">
                <div class="col-xl-12 mx-auto">
                    <div class="card">
                        <div class="header bg-violet">
                            <h2 style="color: white !important">INVENTORY MANAGEMENT EDIT</h2>
                        </div>
                        <%--<div class="header">
                            <h2>INVENTORY MANAGEMENT EDIT</h2>
                        </div>--%>

                        <div class="body">
                            <asp:Label ID="lblPageID" runat="server" Text=""></asp:Label>
                            <div class="row mt-4 mb-4">

                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Item Code </label>
                                    <asp:DropDownList ID="ddlItemCode" CssClass="form-control" runat="server">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvItemCode" runat="server" ControlToValidate="ddlItemCode"
                                        InitialValue="" ErrorMessage="Please select an Item Code." Display="Dynamic" CssClass="text-danger">
                                    </asp:RequiredFieldValidator>
                                </div>

                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Item Name</label>
                                    <asp:TextBox ID="txtItemName" runat="server" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtItemName"
                                        ErrorMessage="Current Stock is required." Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>

                                <div class="col-md-2">
                                    <label for="input1" class="form-label">HSN Code</label>
                                    <asp:TextBox ID="txtHSNCode" runat="server" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtHSNCode"
                                        ErrorMessage="Current Stock is required." Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                </div>

                                <div class="col-md-2">
                                    <label for="input1" class="form-label">UOM</label>
                                    <asp:DropDownList ID="ddlUOM" CssClass="form-control" runat="server">
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-2">
                                    <label for="input1" class="form-label">TAX</label>
                                    <asp:DropDownList ID="ddlVAT" CssClass="form-control" runat="server">
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Rack Type</label>
                                    <asp:DropDownList ID="ddlRackType" CssClass="form-control" runat="server" AutoPostBack="true"
                                        OnSelectedIndexChanged="ddlRackType_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvRackType" runat="server" ControlToValidate="ddlRackType"
                                        InitialValue="" ErrorMessage="Please select a Rack Type." Display="Dynamic" CssClass="text-danger">
                                    </asp:RequiredFieldValidator>
                                </div>

                                <div class="col-md-2">
                                    <label for="input1" class="form-label">Min. Stock</label>
                                    <asp:TextBox ID="txtMinStock" runat="server" class="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtMinStock"
                                        ErrorMessage="Current Stock is required." Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtMinStock"
                                        ErrorMessage="Invalid input. Please enter a positive number." ValidationExpression="^\d*\.?\d+$"
                                        Display="Dynamic" CssClass="text-danger"></asp:RegularExpressionValidator>
                                </div>

                                <div class="col-md-2">
                                    <label for="txtCurrentStock" class="form-label">Current Stock</label>
                                    <asp:TextBox ID="txtCurrentStock" runat="server" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvCurrentStock" runat="server" ControlToValidate="txtCurrentStock"
                                        ErrorMessage="Current Stock is required." Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regexCurrentStock" runat="server" ControlToValidate="txtCurrentStock"
                                        ErrorMessage="Invalid input. Please enter a positive number." ValidationExpression="^\d*\.?\d+$"
                                        Display="Dynamic" CssClass="text-danger"></asp:RegularExpressionValidator>
                                </div>

                            </div>

                            <hr />
                            <div class="row g-3 mt-2 mb-2">
                                <div class="col-md-12">
                                    <label for="input1" class="form-label">Row Series Select</label>
                                    <asp:CheckBoxList ID="chkRowSeries" runat="server" CssClass="custom-checkboxbindDB" RepeatDirection="Vertical">
                                    </asp:CheckBoxList>
                                </div>
                            </div>
                            <hr />


                            <div class="row g-3 mt-3 mb-3">
                                <div class="col-md-4">
                                    <asp:Button ID="btnUpdate" runat="server" Text="UPDATE" class="btn btn-primary" OnClick="btnUpdate_Click" />
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

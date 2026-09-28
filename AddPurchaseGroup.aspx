<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="AddPurchaseGroup.aspx.cs" Inherits="LIMS.AddPurchaseGroup" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .form-control {
            width: 100%; /* Ensures it takes full width of its parent */
        }

        #ddlVAT {
            width: 70%; /* Adjust this value as needed for your design */
        }

        .pagination-ys {
            text-align: center !important;
        }

            .pagination-ys table {
                margin: 0 auto !important;
            }

            .pagination-ys td {
                text-align: center !important;
            }

        .auto-style1 {
            display: block;
            font-size: 14px;
            line-height: 1.42857143;
            color: #555;
            border-radius: 4px;
            -webkit-box-shadow: inset 0 1px 1px rgba(0, 0, 0, .075);
            box-shadow: inset 0 1px 1px rgba(0, 0, 0, .075);
            -webkit-transition: border-color ease-in-out .15s, -webkit-box-shadow ease-in-out .15s;
            -o-transition: border-color ease-in-out .15s, box-shadow ease-in-out .15s;
            transition: border-color ease-in-out .15s, box-shadow ease-in-out .15s;
            border: 1px solid #ccc;
            padding: 6px 12px;
            background-color: #fff;
            background-image: none;
        }

        .table-striped tbody tr:nth-of-type(odd) {
            background-color: rgba(0, 0, 0, 0.05);
        }

        .table th, .table td {
            padding: 8px;
            text-align: left;
            border: 1px solid #ddd;
        }

        .table-hover tbody tr:hover {
            background-color: rgba(0, 0, 0, 0.075);
        }

        .padding-0 {
            padding: 0;
        }

        .table-striped dt-responsive {
            width: 100% !important;
        }
    </style>
</asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <h2>PURCHASE GROUPS / PURCHASE SUB GROUPS</h2>
                </div>


                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <div class="row" style="padding: 10px">
                                <div class="col-md-6">
                                    <div class="card">
                                        <div class="header">
                                            <h5>ADD PURCHASE GROUP</h5>
                                        </div>
                                        <div class="body">
                                            <!-- Purchase group and sub group -->
                                            <div class="row">
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <div class="form-line">
                                                            <label for="txtpurchasegroup">Purchase Group</label>
                                                            <asp:TextBox ID="txtpurchasegroup" runat="server" CssClass="form-control"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <div class="form-line">
                                                            <label for="ddlitemtype">Item Type</label>
                                                            <asp:DropDownList runat="server" ID="ddlitemtype" Height="35px" CssClass="form-control"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col-sm-12" style="text-align: center">
                                                    <asp:Button ID="btnadd" runat="server" Text="Add-PurchaseGroup" CssClass="btn btn-primary" OnClick="btnadd_Click" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-6">
    <div class="card">
        <div class="header">
            <h5>ADD PURCHASE SUB GROUP</h5>
        </div>
        <div class="body">
            <div class="row">
                <div class="col-sm-6">
                    <div class="form-group">
                        <div class="form-line">
                            <label for="ddlpurchaseGroup">Purchase Group</label>
                            <asp:DropDownList runat="server" ID="ddlpurchaseGroup" Height="35px" CssClass="form-control" OnSelectedIndexChanged="ddlpurchaseGroup_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div class="col-md-6" id="divitemtype" runat="server" visible="false">
                    <div class="form-group">
                        <div class="form-line">
                            <label for="ddlitemtypefrosubgroup">Item Type</label>
                            <asp:DropDownList runat="server" ID="ddlitemtypefrosubgroup" Height="35px" CssClass="form-control"></asp:DropDownList>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <div class="form-group">
                        <div class="form-line">
                            <label for="txtpurchasesubgroup">Purchase SubGroup</label>
                            <asp:TextBox ID="txtpurchasesubgroup" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12" style="text-align: center">
                    <asp:Button ID="btnAddsubgroup" runat="server" Text="Add-PurchaseSubGroup" CssClass="btn btn-secondary" OnClick="btnAddsubgroup_Click" />
                </div>
            </div>
        </div>

    </div>
</div>
                            
                            </div>
                            </div>



                            <%--/************Item Name and description***********/--%>
                            <div class="row" style="padding: 10px; visibility:hidden" >
                                    <div class="col-md-6">
        <div class="card">
            <div class="header">
                <h5>DISABLE/ENABLE PURCHASE GROUP</h5>
            </div>
            <div class="body">

                <!-- NEW ROW: Disable Purchase Group -->
                <div class="row">
                    <div class="col-sm-12">
                        <div class="form-group">
                            <div class="form-line">
                                <label for="ddlDisablePurchaseGroup">Select Purchase Group to Disable/Enable</label>
                                <asp:DropDownList runat="server" ID="ddlDisablePurchaseGroup" Height="35px" CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12" style="text-align: center">
                        <asp:Button ID="btnDisablePurchaseGroup" runat="server" Text="Disable/EnableGroup" CssClass="btn btn-warning" OnClick="btnDisablePurchaseGroup_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
                                
                                <!-- NEW ROW: Disable Purchase SubGroup -->
                                                                <div class="col-md-6">
                                    <div class="card">
                                        <div class="header">
                                            <h5>DISABLE/ENABLE PURCHASE SUB GROUP</h5>
                                        </div>
                                        <div class="body">
                                <div class="row" style="padding: 10px; margin-bottom: 15px;">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <label for="ddlDisableSelectGroup">Select Purchase Group</label>
                                                <asp:DropDownList runat="server" ID="ddlDisableSelectGroup" Height="35px" CssClass="form-control" OnSelectedIndexChanged="ddlDisableSelectGroup_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <label for="ddlDisablePurchaseSubGroup">Select SubGroup to Disable/Enable</label>
                                                <asp:DropDownList runat="server" ID="ddlDisablePurchaseSubGroup" Height="35px" CssClass="form-control"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                    </div>
                                            <div class="row">
                                    <div class="col-md-12" style="text-align:center">
                    <asp:Button ID="btnDisablePurchaseSubGroup" runat="server" Text="Disable/Enable SubGroup" CssClass="btn btn-danger" OnClick="btnDisablePurchaseSubGroup_Click" />
                                    </div>
                                </div>
                                </div>
                                            </div>
                                        </div>
                                <br />
                                <br />

                                <div class="form-row">
                                    <div class="row clearfix">
                                        <div class="col-md-12">
                                            <asp:GridView ID="gvPurchaseGroup" runat="server" OnSorting="gvPurchaseGroup_Sorting" AllowSorting="true"
                                                AllowPaging="true" PageSize="10" OnPageIndexChanging="gvPurchaseGroup_PageIndexChanging" AutoGenerateColumns="False"
                                                class="table table-striped table-hover dt-responsive display" Width="100%">

                                                <Columns>
                                                    <asp:TemplateField HeaderText="Purchase Group Name">
                                                        <HeaderTemplate>
                                                            <span>Purchase Group Name</span>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <%# Eval("PurchaseGroupName") %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Purchase SubGroup Name">
                                                        <HeaderTemplate>
                                                            <span>Purchase SubGroup Name</span>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <%# Eval("PurchaseSubGroupName") %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Item Type">
                                                        <HeaderTemplate>
                                                            <span>Item Type</span>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <%# Eval("ItemType") %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Actions" HeaderStyle-Width="10%">
                                                        <ItemTemplate>
                                                            <asp:HyperLink ID="hypEdit" runat="server"
                                                                NavigateUrl='<%# "EditPurchasegroupAndSubgroup.aspx?PurchaseGroupID=" + Eval("PurchaseGroupID") + "&PurchaseSubGroupID=" + Eval("PurchaseSubGroupID") %>'
                                                                CssClass="btn btn-info btn-sm">
                                            <i class="material-icons">edit</i> Edit
                                                            </asp:HyperLink>
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:TemplateField>

                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

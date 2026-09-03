<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ItemInward.aspx.cs" Inherits="LIMS.ItemInward" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .table-bordered {
            border: 1px solid #dee2e6;
        }

            .table-bordered th,
            .table-bordered td {
                border: 1px solid #dee2e6;
                padding: 8px; /* Adjust padding as needed */
            }

        .table-header-bg {
            background-color: #d3d3d3; /* Set your desired background color */
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="page-wrapper">
        <div class="page-content">

            <div class="row">
                <div class="col-xl-12 mx-auto">

                    <div class="card">
                        <div class="header bg-violet">
                            <h2 style="color: white !important">ITEM TRANSACTION ADD</h2>
                        </div>

                        <div class="body">
                            <asp:Label ID="lblPONumber" runat="server" ForeColor="White"></asp:Label>

                            <asp:Label ID="lblPoNo" runat="server" ForeColor="White"></asp:Label>

                            <asp:Label ID="lblorderQty" runat="server" ForeColor="White"></asp:Label>
                            <asp:Label ID="lblreceiveQty" runat="server" ForeColor="White"></asp:Label>
                            <asp:Label ID="lblbalanceQty" runat="server" ForeColor="White"></asp:Label>
                            <div class="row">
                                <div class="col-md-2">
                                    <label for="input1" class="form-label">ReceivedDate</label>
                                    <asp:TextBox ID="txtReceivedDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Vendor </label>
                                    <asp:DropDownList ID="ddlVendor" CssClass="form-control" runat="server" OnSelectedIndexChanged="ddlVendor_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-3">
                                    <label for="input1" class="form-label">PO Number </label>
                                    <asp:DropDownList ID="ddlPoNo" CssClass="form-control" runat="server">
                                    </asp:DropDownList>

                                    <asp:RequiredFieldValidator ID="rfvPoNo" runat="server" ControlToValidate="ddlPoNo"
                                        InitialValue="" ErrorMessage="Please select a PoNo." Display="Dynamic" CssClass="text-danger">
                                    </asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-1">
                                    <br />
                                    <asp:Button ID="btnAdd" runat="server" Text="+" OnClick="btnAdd_Click" CssClass="btn btn-primary" />
                                </div>

                            </div>
                            <div class="row">
                                <div class="col-sm-3">
                                    <div class="form-group" style="margin-left: 20px;">
                                        <label>Document Name/Number: </label>
                                        <asp:TextBox ID="txtDocumentName" runat="server" Text="" CssClass="form-control" placeholder="SURAT JALAN"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Document Upload</label>
                                    <asp:FileUpload ID="fpFile" runat="server" AllowMultiple="True" CssClass="form-control" />
                                    <asp:GridView ID="gvAttachments"
                                        runat="server"
                                        AutoGenerateColumns="False"
                                        OnRowDeleting="gvAttachments_RowDeleting">
                                        <Columns>
                                            <asp:BoundField DataField="DocumentName"
                                                HeaderText="Document Name" />

                                            <asp:BoundField DataField="OriginalFileName"
                                                HeaderText="File Name" />

                                            <asp:ButtonField Text="Delete"
                                                CommandName="Delete" />

                                        </Columns>

                                    </asp:GridView>
                                </div>
                                <div class="col-md-1">
                                    <br />
                                    <asp:Button ID="btnAddDocument" runat="server" Text="+" CssClass="btn btn-primary" OnClick="btnAddDocument_Click" />
                                </div>
                            </div>

                            <hr />
                            <div class="row">
                                <div class="col-md-12">
                                    PO Details
                                </div>
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="table-responsive">
                                            <asp:GridView ID="GridView1" runat="server" DataKeyNames="PODetailsID" AutoGenerateColumns="False"
                                                AutoGenerateEditButton="true" OnRowEditing="GridView1_RowEditing"
                                                OnRowCancelingEdit="GridView1_RowCancelingEdit" OnRowUpdating="GridView1_RowUpdating" CssClass="table table-bordered"
                                                HeaderStyle-CssClass="table-header-bg">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Item">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblItemCode" runat="server" Text='<%#Eval("ItemCode") %>'></asp:Label>
                                                            <asp:HiddenField ID="hdItemID" runat="server" Value='<%#Eval("ItemID") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="ItemName" HeaderText="Item Name" SortExpression="ItemName" ReadOnly="true" />
                                                    <asp:BoundField DataField="Quantity" HeaderText="Order Qty." SortExpression="PoOrderQty" ReadOnly="true" />
                                                    <asp:BoundField DataField="POReceivedQty" HeaderText="Prev. Received Qty." SortExpression="PoReceivedQty" ReadOnly="true" />
                                                    <asp:TemplateField HeaderText="Received Qty">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblReceivedQty" runat="server" Text='<%#Eval("ReceivedQty") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="txtReceivedQty" runat="server"></asp:TextBox>
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="EmployeeName" HeaderText="Requested By" SortExpression="EmployeeName" ReadOnly="true" />
                                                </Columns>
                                            </asp:GridView>

                                        </div>
                                        <asp:Button ID="btnSaveInward" runat="server" Text="Save" OnClick="btnSaveInward_Click" CssClass="btn btn-primary" />
                                        <asp:HiddenField ID="hdInwardID" runat="server" />
                                        <hr />
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
                <!--end row-->
            </div>
</asp:Content>

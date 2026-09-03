<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="PaymentRequisition.aspx.cs" Inherits="LIMS.PaymentRequisition" %>

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
                            <h2 style="color: white !important">Payment Requisition</h2>
                        </div>

                        <div class="body">

                            <div class="row mt-4 mb-4">
                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Payment Requisition Type </label>
                                    <asp:RadioButtonList ID="radPRType" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem>Expense Payment</asp:ListItem>
                                        <asp:ListItem>Cash Advance</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <div class="col-md-2">
                                    <label for="input1" class="form-label">Requisition From</label>
                                    <asp:TextBox ID="txtRequisitionFrom" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Requisition Number</label>
                                    <asp:TextBox ID="txtRequisitionNumber" runat="server" Enabled="false" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-2">
                                    <label for="input1" class="form-label">Requisition Date</label>
                                    <asp:TextBox ID="txtRequisitionDate" runat="server" Enabled="false" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                </div>
                                <div class="col-md-2">
                                    <label for="input1" class="form-label">Currency</label>
                                    <asp:DropDownList ID="ddlCurrency" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-3">
                                    <div class="form-group" style="margin-left: 20px;">
                                        <label>Document Name/Number: </label>
                                        <asp:TextBox ID="txtDocumentName" runat="server" placeholder="Enter Document Name" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <label for="input1" class="form-label">Document Upload</label>
                                    <asp:FileUpload ID="fpFile" runat="server" AllowMultiple="True" CssClass="form-control" EnableViewState="true" />

                                </div>
                            </div>
                            <hr />
                            <h5>Requisition Details</h5>
                            <asp:ScriptManager ID="scrpt" runat="server"></asp:ScriptManager>
                            <asp:UpdatePanel ID="uptd" runat="server">
                                <ContentTemplate>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvPaymentRequisition" runat="server" CssClass="table" AutoGenerateColumns="false" ShowFooter="true" OnRowDataBound="gvPaymentRequisition_RowDataBound">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Nomor" ItemStyle-Width="70">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="30px"></ItemStyle>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="ItemDescription" HeaderText="Nama & Ukuran Barang" />
                                                <asp:BoundField DataField="Quantity" HeaderText="Jumlah Permintaan" />
                                                <asp:BoundField DataField="RequirementDate" HeaderText="Tgl. Dibutuhkan" DataFormatString="{0:dd-MM-yyyy}" />
                                                <asp:BoundField DataField="UnitPrice" HeaderText="Biaya" />
                                                <asp:BoundField DataField="TotalPrice" HeaderText="Total Biaya" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" />

                                            </Columns>
                                        </asp:GridView>
                                        <asp:HiddenField ID="hdGrandTotal" runat="server" />
                                    </div>
                                    <div class="row">
                                        <div class="col-md-3">Nama & Ukuran Barang</div>
                                        <div class="col-md-2">Jumlah Permintaan</div>
                                        <div class="col-md-2">Tgl. Dibutuhkan</div>
                                        <div class="col-md-2">Biaya</div>
                                        <div class="col-md-2">Total Biaya</div>

                                    </div>
                                    <div class="row">
                                        <div class="col-md-3">
                                            <asp:TextBox ID="txtItemDescription" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-md-2">
                                            <asp:TextBox ID="txtQuantity" runat="server" CssClass="form-control" OnTextChanged="txtQuantity_TextChanged" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                        <div class="col-md-2">
                                            <asp:TextBox ID="txtRequirementDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        </div>
                                        <div class="col-md-2">
                                            <asp:TextBox ID="txtUnitPrice" runat="server" CssClass="form-control" OnTextChanged="txtUnitPrice_TextChanged" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                        <div class="col-md-2">
                                            <asp:TextBox ID="txtTotalPrice" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                        </div>

                                        <div class="col-md-1">
                                            <asp:Button ID="btnAdd" runat="server" Text="+" CssClass="btn btn-primary btn-sm" OnClick="btnAdd_Click" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12">
                                            Remarks<br />
                                            <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5"></asp:TextBox>

                                        </div>
                                    </div>
                                    <asp:Button ID="btnSavePaymentRequest" runat="server" Text="Save" OnClick="btnSavePaymentRequest_Click" CssClass="btn btn-primary" />
                                    <asp:HiddenField ID="hdPaymentRequisitionID" runat="server" />
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:PostBackTrigger ControlID="btnSavePaymentRequest" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                        <hr />


                    </div>
                </div>
            </div>
        </div>
        <!--end row-->
    </div>
</asp:Content>

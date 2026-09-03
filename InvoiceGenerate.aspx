<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeBehind="InvoiceGenerate.aspx.cs" Inherits="LIMS.InvoiceGenerate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .a4-page {
            page-break-after: always;
            box-shadow: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <!-- Basic Examples -->
    <div class="row">
        <div class="col-lg-9 col-md-12 col-sm-12 col-xs-12" style="margin-left: 10%">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-6">
                            <h2>GENERATE INVOICE</h2>
                        </div>
                    </div>
                </div>
                <div class="body">
                    <div class="row">
                        <div class='table-responsive'>
                            <table style="margin-right: 5px; page-break-after: always; width: 100%">
                                <thead>
                                    <tr>
                                        <td colspan="2">
                                            <table style='font-family: "Arial MT", Arial, sans-serif; font-size: 12.5px; width: 100%'>
                                                <tr>
                                                    <td>
                                                        <table class="table">
                                                            <tr>
                                                                <td><b>Inv No.</b></td>
                                                                <td>:</td>
                                                                <td>
                                                                    <asp:Label ID="lblInvoiceNo" runat="server"></asp:Label>
                                                                    <asp:HiddenField ID="hdQuotationID" runat="server" Value='<%#Eval("QuotationID") %>' />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td><b>Tanggal Faktur</b></td>
                                                                <td>:</td>
                                                                <td colspan="5">
                                                                    <asp:TextBox ID="txtInvoiceDate" runat="server" CssClass="form-control" Text='<%# String.Format("{0:yyyy-MM-dd}", Eval("Date")) %>' TextMode="Date" OnTextChanged="txtInvoiceDate_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td><b>Due Date</b></td>
                                                                <td>:</td>
                                                                <td colspan="5">
                                                                    <asp:TextBox ID="txtDueDate" runat="server" CssClass="form-control" Text='<%# String.Format("{0:yyyy-MM-dd}", Eval("DueDate")) %>' TextMode="Date"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td><b>Currency</b></td>
                                                                <td>:</td>
                                                                <td>
                                                                    <asp:HiddenField ID="hdCurrency" runat="server" Value='<%#Eval("CurrencyName") %>' />
                                                                    <asp:DropDownList ID="ddlCurrency" runat="server" CssClass="form-control"
                                                                        AutoPostBack="true">
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td><b>Payment Terms</b></td>
                                                                <td>:</td>
                                                                <td>
                                                                    <asp:HiddenField ID="hdPaymentTerm" runat="server" Value='<%#Eval("PaymentTerm") %>' />
                                                                    <asp:DropDownList ID="ddlPaymentTerm" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlPaymentTerm_SelectedIndexChanged" AutoPostBack="true">
                                                                        <asp:ListItem Text="--Select Payment Term--" Value="" />
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td><b>PIC Marketing</b></td>
                                                                <td>:</td>
                                                                <td colspan="5">
                                                                    <asp:TextBox ID="txtPIC" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td><b>PO Number</b></td>
                                                                <td>:</td>
                                                                <td colspan="5">
                                                                    <asp:Label ID="lblPONumber" runat="server"></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td style="width: 10%">&nbsp;</td>
                                                    <td colspan="2">
                                                        <table class="table">
                                                            <tr>
                                                                <td><b>Bill To:</b></td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblCompanyName" runat="server" Font-Bold="true"></asp:Label>
                                                                    <asp:HiddenField ID="hdCustomerMasterID" runat="server" Value='<%#Eval("CustomerMasterID") %>' />
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td><b>Address<br />
                                                                </b>
                                                                    <asp:Label ID="lblAddressLine1" runat="server"></asp:Label>
                                                                    <br />
                                                                    <asp:Label ID="lblAddressLine2" runat="server"></asp:Label>
                                                                <tr>
                                                                    <td><b>NPWP/NIK Pembeli:</b>
                                                                        <asp:Label ID="lblNPWP" runat="server"></asp:Label></td>
                                                                </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3" style="width: 50%">
                                                        <table class="table">
                                                            <tr>
                                                                <td style="width: 20%"><b>Kode Transaksi</b></td>
                                                                <td style="width: 5%">:</td>
                                                                <td style="width: 20%" colspan="2">
                                                                    <asp:DropDownList ID="ddlTransactionCode" runat="server" CssClass="form-control">
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td style="width: 10%"></td>
                                                                <td style="width: 20%"><b>Jenis ID Pembeli</b></td>
                                                                <td style="width: 5%">:</td>
                                                                <td style="width: 20%" colspan="2">
                                                                    <asp:DropDownList ID="ddlBuyerType" runat="server" CssClass="form-control">
                                                                        <asp:ListItem>Select</asp:ListItem>
                                                                        <asp:ListItem>NPWP</asp:ListItem>
                                                                        <asp:ListItem>Paspor</asp:ListItem>
                                                                        <asp:ListItem>Lainnya</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td colspan="2" style="margin-top: 250px">
                                            <table style='width: 100%; margin-top: 10px; font-family: "Arial MT", Arial, sans-serif; font-size: 12.5px' class='tab'>
                                                <tr>
                                                    <td colspan="7">
                                                        <asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="false" DataKeyNames="quotationdetailID"
                                                            OnRowDataBound="gvItems_RowDataBound"
                                                            OnRowEditing="gvItems_RowEditing" OnRowUpdating="gvItems_RowUpdating" OnRowCancelingEdit="gvItems_RowCancelingEdit"
                                                            CssClass="table table-bordered table-hover" ShowFooter="true">
                                                            <Columns>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton runat="server" CommandName="Edit" Text="Edit" />
                                                                        |
                                                                        <asp:LinkButton runat="server" CommandName="Delete" Text="Delete" />
                                                                    </ItemTemplate>
                                                                    <EditItemTemplate>
                                                                        <asp:LinkButton runat="server" CommandName="Update" Text="Update" />
                                                                        |
                                                                        <asp:LinkButton runat="server" CommandName="Cancel" Text="Cancel" />
                                                                    </EditItemTemplate>

                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="COAAccount">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblAccountCode" runat="server" Text='<%#Eval("AccountCode") %>'></asp:Label><br />
                                                                        <asp:Label ID="lblAccountName" runat="server"></asp:Label>
                                                                        <asp:HiddenField ID="hdItemType" runat="server" Value='<%#Eval("ItemType") %>' />
                                                                        <asp:HiddenField ID="hdDepartmentID" runat="server" Value='<%#Eval("DepartmentID") %>' />
                                                                        <asp:HiddenField ID="hdquotationdetailID" runat="server" Value='<%#Eval("quotationdetailID") %>' />
                                                                    </ItemTemplate>
                                                                    <EditItemTemplate>
                                                                        <asp:DropDownList ID="ddlCOA" runat="server" CssClass="form-control"></asp:DropDownList>
                                                                        <asp:HiddenField ID="hdAccountCode" runat="server" Value='<%#Eval("AccountCode") %>' />
                                                                    </EditItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="RequestNo" HeaderText="REQUEST NO." ReadOnly="true" />
                                                                <asp:BoundField DataField="WorkOrderNumber" HeaderText="WO NO." ReadOnly="true" />
                                                                <asp:TemplateField HeaderText="DESCRIPTION">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblSampleName" runat="server" Text='<%#Eval("TestItems") %>' Font-Bold="true"></asp:Label><br />
                                                                        <asp:Label ID="lblsampleDescription" runat="server" Text='<%#Eval("sampleDescription") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="TestItems" HeaderText="DESCRIPTION" ReadOnly="true" />
                                                                <asp:TemplateField HeaderText="QTY">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblQuantity" runat="server" Text='<%#Eval("Quantity") %>' Font-Bold="true"></asp:Label><br />
                                                                    </ItemTemplate>
                                                                    <EditItemTemplate>
                                                                        <asp:TextBox ID="txtQuantity" runat="server" Width="50%" Text='<%#Eval("Quantity") %>' OnTextChanged="txtQuantity_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                    </EditItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Price">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblPrice" runat="server" Text='<%#Eval("Price") %>' Font-Bold="true"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <EditItemTemplate>
                                                                        <asp:Label ID="lblPrice" runat="server" Text='<%#Eval("Price") %>' Font-Bold="true"></asp:Label>
                                                                    </EditItemTemplate>
                                                                    <FooterTemplate>
                                                                        <div style="text-align: right; font-weight: bold; float: right">
                                                                            <table>
                                                                                <tr>
                                                                                    <td>Subtotal:</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>Discount:</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>DPP(11/12):</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>VAT (11%):</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>Grand Total:</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>Down Payment:</td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>Balance Due:</td>
                                                                                </tr>
                                                                            </table>
                                                                            <br />
                                                                        </div>
                                                                    </FooterTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Total Amount">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotalAmount" runat="server" Text='<%#Eval("TotalAmount") %>' Font-Bold="true"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <EditItemTemplate>
                                                                        <asp:Label ID="lblTotalAmount" runat="server" Text='<%#Eval("TotalAmount") %>' Font-Bold="true"></asp:Label>
                                                                    </EditItemTemplate>
                                                                    <FooterTemplate>
                                                                        <div style="text-align: right">
                                                                            <table>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:Label ID="lblSubtotal" runat="server" />
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:TextBox ID="txtDiscount" runat="server"
                                                                                            AutoPostBack="true"
                                                                                            OnTextChanged="txtDiscount_TextChanged"
                                                                                            Width="80px" Style="text-align:right;"/></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:Label ID="lblDPP" runat="server" /></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:Label ID="lblVat" runat="server" /></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:Label ID="lblGrandTotal" runat="server" /></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <!-- 🔹 Down Payment -->
                                                                                    <td style="text-align:right">
                                                                                        <asp:TextBox ID="txtDownPayment" runat="server"
                                                                                            AutoPostBack="true"
                                                                                            OnTextChanged="txtDownPayment_TextChanged"
                                                                                            Width="80px" Style="text-align:right;" /></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:Label ID="lblBalance" runat="server" /></td>
                                                                                </tr>
                                                                            </table>


                                                                            <!-- Discount editable -->


                                                                        </div>
                                                                    </FooterTemplate>
                                                                </asp:TemplateField>

                                                            </Columns>
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="7">
                                                        <hr />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div class="row">
                        <table width="100%">
                            <tr>
                                <td colspan="7" style="text-align: center">
                                    <asp:Button ID="btnSave" runat="server" Text="Save Invoice" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                                    &nbsp;&nbsp;&nbsp;
                                    <asp:Button ID="btnClear" runat="server" Text="Cancel" CssClass="btn btn-primary" OnClick="btnCancel_Click" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

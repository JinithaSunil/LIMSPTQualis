<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="RequisitionPaymentsNew.aspx.cs" Inherits="LIMS.RequisitionPaymentsNew" %>

<%@ Register Src="~/UserControls/JournalEntry.ascx" TagPrefix="uc" TagName="JournalEntry" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">

    <div class="card">

        <div class="card-header bg-primary text-white">
            <h4>Payment Processing</h4>
        </div>

        <div class="card-body">

            <!-- PR Information -->

            <div class="row">
                <div class="card-body">
                    <div class="row">
                        <div class="col-sm-3">
                            <b>Payment Date</b>
                            <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-sm-9">
                            <b>Reference</b>
                            <asp:TextBox ID="txtReference" runat="server" CssClass="form-control" Placeholder="Reference"></asp:TextBox>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-4">
                            <b>Payee Type</b>
                            <asp:DropDownList ID="ddlPayeeType" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlPayeeType_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem>Employee</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-sm-4">
                            <b>Pay To</b>
                            <asp:DropDownList ID="ddlPayee" runat="server" CssClass="form-control"></asp:DropDownList>
                            <asp:HiddenField ID="hdPayeeID" runat="server" />
                        </div>
                        <div class="col-sm-4">
                            <b>Payment Requisition</b>
                            <asp:DropDownList ID="ddlPaymentRequisition" runat="server" CssClass="form-control"></asp:DropDownList>
                            <asp:HiddenField ID="hdPaymentRequisitionID" runat="server" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-2">
                            <b>Total Amount</b>
                            <asp:TextBox ID="txtTotalAmount" runat="server" CssClass="form-control" Enabled="false" Placeholder="Total Amount" OnTextChanged="txtAmount_TextChanged"></asp:TextBox>
                        </div>
                        <div class="col-sm-2">
                            <b>Amount Paid</b>
                            <asp:TextBox ID="txtAmountPaid" runat="server" CssClass="form-control" Enabled="false" Placeholder="Previous Paid"></asp:TextBox>
                        </div>
                        <div class="col-sm-2">
                            <b>Pay Amount</b>
                            <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" Placeholder="Amount" OnTextChanged="txtAmount_TextChanged" AutoPostBack="true"></asp:TextBox>
                        </div>
                        <div class="col-sm-2">
                            <b>Difference</b>
                            <asp:TextBox ID="txtDifference"
                                runat="server"
                                CssClass="form-control"
                                Enabled="false"></asp:TextBox>
                        </div>
                        <div class="col-sm-2">
                            <b>Balance Amount</b>
                            <asp:TextBox ID="txtBalance" runat="server" CssClass="form-control" Enabled="false" Placeholder="Current Payment"></asp:TextBox>
                        </div>
                    </div>

                </div>
            </div>
             <div class="row">
     <div class="col-sm-6">
         <b>Payment Mode</b>
         <table>
             <tr>
                 <td colspan="2">
                     <asp:RadioButtonList ID="radPaymentMode" runat="server" RepeatDirection="Horizontal" CssClass="form-check" OnSelectedIndexChanged="radPaymentMode_SelectedIndexChanged" AutoPostBack="true">
                         <asp:ListItem>CASH</asp:ListItem>
                         <asp:ListItem>CHEQUE</asp:ListItem>
                         <asp:ListItem>BANK</asp:ListItem>
                     </asp:RadioButtonList>
                 </td>
             </tr>
         </table>
         <table id="tblCheque" runat="server" visible="false" class="table" style="text-align: center">
             <tr>
                 <td>
                     <b>Bank / Account#</b>
                     <asp:TextBox ID="txtBankAccount" runat="server" CssClass="form-control"></asp:TextBox>
                 </td>
            
                 <td>
                     <b>Cheque#</b>
                     <asp:TextBox ID="txtChequeNo" runat="server" CssClass="form-control"></asp:TextBox>
                 </td>
            
                 <td>
                     <b>Cheque Dt.</b><asp:TextBox ID="txtChequeDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                 </td>
             </tr>
         </table>
         <table id="tblBank" runat="server" visible="false" class="table" style="text-align: center">
             <tr>
                 <td>
                     <b>Bank Name</b>
                     <asp:TextBox ID="txtBankName" runat="server" CssClass="form-control"></asp:TextBox>
                 </td>
                 <td>
                     <b>Bank / Account#</b>
                     <asp:TextBox ID="txtBankAccount1" runat="server" CssClass="form-control"></asp:TextBox>
                 </td>
                 <td>
                     <b>Transfer Dt.</b><asp:TextBox ID="txtTransferDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                 </td>
             </tr>
         </table>
         <br />

     </div>
   
     <div class="col-sm-6">
         <b>Notes</b>
         <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4"></asp:TextBox>
     </div>
     <div class="col-sm-3">
         <br />
         <asp:Button ID="btnAdd" runat="server" CssClass="btn btn-primary" Text="Add Payment" OnClick="btnAdd_Click" />
     </div>
 </div>
            <hr />
            <uc:JournalEntry runat="server" ID="JournalEntry1" />

            <!-- PR Items -->

            <h5>Payment Requisition Items</h5>

            <asp:GridView ID="gvItems"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="table table-bordered table-hover"
                DataKeyNames="PaymentRequisitionDetailsID" OnRowCommand="gvItems_RowCommand">

                <Columns>


                    <asp:BoundField DataField="ItemDescription" HeaderText="Item" />
                    <asp:BoundField DataField="Quantity" HeaderText="Quantity" />
                    <asp:BoundField DataField="UnitPrice" HeaderText="Unit Price" />
                    <asp:BoundField DataField="TotalPrice" HeaderText="TotalPrice" DataFormatString="{0:N2}" />

                    <%--                    <asp:BoundField
                        DataField="Status"
                        HeaderText="Status" />--%>
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnProcess"
                                runat="server"
                                Text="Process"
                                CssClass="btn btn-primary btn-sm"
                                CommandName="Process"
                                CommandArgument='<%# Eval("PaymentRequisitionDetailsID") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>

            </asp:GridView>

            <hr />

            <!-- Selected Item -->

            <div class="alert alert-info">

                <b>Selected Item :</b>

                <asp:Label ID="lblDescription"
                    runat="server" />

                &nbsp;&nbsp;&nbsp;

            Gross :

            <asp:Label ID="lblGross"
                runat="server" />

            </div>

            <!-- Accounting -->

            <h5>Accounting Distribution</h5>

            <asp:GridView ID="gvAccounting"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="table table-striped">

                <Columns>

                    <asp:TemplateField HeaderText="Dr/Cr">

                        <ItemTemplate>

                            <asp:DropDownList
                                ID="ddlType"
                                runat="server"
                                CssClass="form-select">

                                <asp:ListItem>Dr</asp:ListItem>
                                <asp:ListItem>Cr</asp:ListItem>

                            </asp:DropDownList>

                        </ItemTemplate>

                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Account">

                        <ItemTemplate>

                            <asp:DropDownList
                                ID="ddlAccount"
                                runat="server"
                                CssClass="form-select">
                            </asp:DropDownList>

                        </ItemTemplate>

                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Debit">

                        <ItemTemplate>

                            <asp:TextBox
                                ID="txtDebit"
                                runat="server"
                                CssClass="form-control text-end" />

                        </ItemTemplate>

                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Credit">

                        <ItemTemplate>

                            <asp:TextBox
                                ID="txtCredit"
                                runat="server"
                                CssClass="form-control text-end" />

                        </ItemTemplate>

                    </asp:TemplateField>

                    <asp:TemplateField>

                        <ItemTemplate>

                            <asp:LinkButton
                                ID="btnDelete"
                                runat="server"
                                CssClass="btn btn-danger btn-sm">

                            Delete

                            </asp:LinkButton>

                        </ItemTemplate>

                    </asp:TemplateField>

                </Columns>

            </asp:GridView>

            <div class="text-end">

                <asp:Button ID="btnAddDebit"
                    runat="server"
                    Text="+ Debit"
                    CssClass="btn btn-success" />

                <asp:Button ID="btnAddCredit"
                    runat="server"
                    Text="+ Credit"
                    CssClass="btn btn-warning" />

            </div>

            <hr />

            <div class="row">

                <div class="col-md-3">
                    Debit Total

                <asp:Label ID="lblDebit"
                    runat="server"
                    CssClass="fw-bold" />

                </div>

                <div class="col-md-3">
                    Credit Total

                <asp:Label ID="lblCredit"
                    runat="server"
                    CssClass="fw-bold" />

                </div>

                <div class="col-md-3">
                    Difference

                <asp:Label ID="lblDifference"
                    runat="server"
                    CssClass="fw-bold text-danger" />

                </div>

                <div class="col-md-3 text-end">

                    <asp:Button ID="btnSaveItem"
                        runat="server"
                        CssClass="btn btn-primary"
                        Text="Save Item" />

                    <asp:Button ID="btnNext"
                        runat="server"
                        CssClass="btn btn-secondary"
                        Text="Next Item" />

                </div>

            </div>

        </div>

    </div>
</asp:Content>

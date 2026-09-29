<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeBehind="RequisitionPayments.aspx.cs" Inherits="LIMS.RequisitionPayments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function calculatePRRow(element) {

            var row = $(element).closest('tr');

            var totalAmount =
                parseFloat(row.find('.total-amount').val()) || 0;

            var amountPaid =
                parseFloat(row.find('.amount-paid').val()) || 0;

            var debitAmount =
                parseFloat(row.find('.debit-amount').val()) || 0;

            var remaining =
                totalAmount - amountPaid;

            var difference =
                remaining - debitAmount;
            alert(totalAmount + ":" + amountPaid + ":" + debitAmount + ":" + remaining);
            // No difference
            if (Math.abs(difference) < 0.01) {
                difference = 0;
            }

            // Credit cannot be negative
            var creditAmount =
                difference > 0 ? difference : 0;

            row.find('.credit-amount').val(
                creditAmount.toFixed(2)
            );
        }

        function selectAllPRs(source) {

            var checkboxes = document.querySelectorAll(
            '#<%= gvPaymentRequisitions.ClientID %> input.chkPR'
            );

            for (var i = 0; i < checkboxes.length; i++) {
                checkboxes[i].checked = source.checked;
            }
        }

    </script>
    <style>
        .subheader {
            color: #b34788;
        }

        .alert {
            line-height: 1.375rem;
        }

        .alert-success1 {
            background-color: #e4f7d6;
            border-color: #e4f7d6;
            color: #56ca00 !important;
        }

        .alert-danger1 {
            background-color: #ffe2e3;
            border-color: #ffe2e3;
            color: #ff4c51 !important;
        }

        .alert-warning1 {
            background-color: #fff3d6;
            border-color: #fff3d6;
            color: #ffb400 !important;
        }
    </style>
    <script>
        function AllowDecimal(evt, obj) {
            var charCode = evt.which ? evt.which : evt.keyCode;

            // Allow Backspace, Tab, Delete
            if (charCode == 8 || charCode == 9 || charCode == 46)
                return true;

            // Allow only one decimal point
            if (charCode == 46) {
                if (obj.value.indexOf('.') !== -1)
                    return false;
                return true;
            }

            // Allow digits only
            if (charCode >= 48 && charCode <= 57)
                return true;

            return false;
        }

        function ValidateDecimal(obj) {
            // Remove invalid characters
            obj.value = obj.value.replace(/[^0-9.]/g, '');

            // Allow only one decimal point
            var parts = obj.value.split('.');
            if (parts.length > 2) {
                obj.value = parts[0] + '.' + parts.slice(1).join('');
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <!-- Basic Examples -->
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-12">
                            <h2>PAYMENTS</h2>
                        </div>
                    </div>
                    <br />
                </div>
                <div class="body">
                    <div class="card" style="padding: 15px">
                        <div class="card-header subheader">
                            <h5>Payment Information</h5>
                        </div>
                        <div class="card-body">
                            <div class="row">
                                <asp:RadioButtonList ID="radOption" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="radOption_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Selected="True">Single</asp:ListItem>
                                    <asp:ListItem>Multiple</asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
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
                                <div class="col-sm-3">
                                    <b>Pay To</b>
                                    <asp:DropDownList ID="ddlPayee" runat="server" OnSelectedIndexChanged="ddlPayee_SelectedIndexChanged" AutoPostBack="true" CssClass="form-control"></asp:DropDownList>
                                    <asp:HiddenField ID="hdPayeeID" runat="server" />
                                </div>
                                <div class="col-sm-3">
                                    <b>Payment Requisition</b>
                                    <asp:DropDownList ID="ddlPaymentRequisition" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlPaymentRequisition_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    <asp:HiddenField ID="hdPaymentRequisitionID" runat="server" />
                                </div>
                                <div class="col-sm-2">
                                    <br />
                                    <asp:Button ID="btnAdd" runat="server" Text="+" OnClick="btnAdd_Click" CssClass="btn btn-primary" />
                                    <asp:Button ID="btnAddAll" runat="server" Text="+ All" OnClick="btnAddAll_Click" CssClass="btn btn-primary" />
                                    <asp:Button ID="btnClear" runat="server" Text="X" OnClick="btnClear_Click" CssClass="btn btn-primary" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-2">
                                    <b>Total Amount</b>
                                    <asp:TextBox ID="txtTotalAmount" runat="server" CssClass="form-control" Enabled="false" Placeholder="Total Amount" OnTextChanged="txtTotalAmount_TextChanged"></asp:TextBox>
                                </div>
                                <div class="col-sm-2">
                                    <b>Amount Paid</b>
                                    <asp:TextBox ID="txtAmountPaid" runat="server" CssClass="form-control" Enabled="false" Placeholder="Previous Paid"></asp:TextBox>
                                </div>
                            </div>

                            <div class="row">
                                <asp:GridView
                                    ID="gvPaymentRequisitions"
                                    runat="server"
                                    CssClass="table table-bordered table-hover"
                                    DataKeyNames="PaymentRequisitionID,TotalAmount,AmountPaid"
                                    AutoGenerateColumns="false"
                                    ShowFooter="True"
                                    OnRowDataBound="gvPaymentRequisitions_RowDataBound">

                                    <Columns>

                                        <asp:TemplateField
                                            HeaderText="Select"
                                            HeaderStyle-Width="60px">

                                            <HeaderTemplate>
                                                <asp:CheckBox
                                                    ID="chkSelectAll"
                                                    runat="server"
                                                    AutoPostBack="true"
                                                    OnCheckedChanged="chkSelectAll_CheckedChanged" />
                                            </HeaderTemplate>

                                            <ItemTemplate>

                                                <asp:CheckBox
                                                    ID="chkPR"
                                                    runat="server"
                                                    CssClass="chkPR"
                                                    AutoPostBack="true"
                                                    OnCheckedChanged="chkPR_CheckedChanged" />

                                                <asp:Label
                                                    ID="lblRowNumber"
                                                    runat="server"
                                                    Text='<%# Container.DataItemIndex + 1 %>' />

                                                <asp:HiddenField
                                                    ID="hfPaymentRequisitionID"
                                                    runat="server"
                                                    Value='<%# Eval("PaymentRequisitionID") %>' />

                                            </ItemTemplate>

                                        </asp:TemplateField>


                                        <asp:BoundField
                                            DataField="RequisitionNumber"
                                            HeaderText="Requisition Number" />
                                        <asp:TemplateField HeaderText="Total Amt.">
                                            <ItemTemplate>
                                                <asp:Label
                                                    ID="lblTotalAmount"
                                                    runat="server"
                                                    CssClass="total-amount"
                                                    Text='<%# Eval("TotalAmount") == DBNull.Value ? "0" : Eval("TotalAmount", "{0:0.00}") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Amt. Paid">
                                            <ItemTemplate>
                                                <asp:Label
                                                    ID="lblAmountPaid"
                                                    runat="server"
                                                    CssClass="amount-paid"
                                                    Text='<%# Eval("AmountPaid") == DBNull.Value ? "0" : Eval("AmountPaid", "{0:0.00}") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Debit" ItemStyle-BackColor="lightPink" HeaderStyle-HorizontalAlign="Center">

                                            <ItemTemplate>

                                                <div class="row g-2">

                                                    <div class="col-md-4">




                                                        <label class="form-label">
                                                            Pay Amount
                                                        </label>

                                                        <asp:TextBox
                                                            ID="txtDebitPayAmount"
                                                            runat="server"
                                                            CssClass="form-control form-control-sm debit-amount"
                                                            onkeyup="calculatePRRow(this)"
                                                            onchange="calculatePRRow(this)">
                                                            </asp:TextBox>
                                                    </div>


                                                    <div class="col-md-4">

                                                        <label class="form-label">
                                                            Debit COA
                                                        </label>

                                                        <asp:DropDownList
                                                            ID="ddlDebitCOA"
                                                            runat="server"
                                                            CssClass="form-control form-control-sm">
                                                        </asp:DropDownList>

                                                    </div>
                                                    <div class="col-md-4">
                                                        <label class="form-label">
                                                            Bank
                                                        </label>

                                                        <asp:DropDownList
                                                            ID="ddlBank"
                                                            runat="server"
                                                            CssClass="form-control form-control-sm">
                                                        </asp:DropDownList>

                                                    </div>

                                                </div>

                                            </ItemTemplate>

                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Credit" ItemStyle-BackColor="lightGreen">
                                            <ItemTemplate>

                                                <div class="row g-2">

                                                    <div class="col-md-6">

                                                        <label class="form-label">
                                                            Pay Amount
                                                        </label>

                                                        <asp:TextBox
                                                            ID="txtCreditPayAmount"
                                                            runat="server"
                                                            CssClass="form-control form-control-sm credit-amount"
                                                            Text='<%# Eval("TotalAmount", "{0:0.00}") %>'
                                                            onkeyup="calculatePRRow(this)"
                                                            onchange="calculatePRRow(this)">
                                                        </asp:TextBox>

                                                    </div>


                                                    <div class="col-md-6">

                                                        <label class="form-label">
                                                            Credit COA
                                                        </label>

                                                        <asp:DropDownList
                                                            ID="ddlCreditCOA"
                                                            runat="server"
                                                            CssClass="form-control form-control-sm">
                                                        </asp:DropDownList>

                                                    </div>

                                                </div>

                                            </ItemTemplate>

                                        </asp:TemplateField>



                                    </Columns>

                                </asp:GridView>
                                <asp:HiddenField ID="hdTotal" runat="server" />
                            </div>
                        </div>
                    </div>

                    <br />

                </div>
                <asp:Label
                    ID="Label1"
                    runat="server"
                    Visible="false"
                    CssClass="alert"
                    Style="display: block;">
                </asp:Label>
                <div class="row-clearfix">
                    <div class="col-sm-12">
                        <asp:Label ID="lblMessage" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="row-clearfix">

                    <div class="col-sm-3">
                        <br />
                        <asp:Button ID="btnSave"
                            runat="server"
                            Text="SAVE TEST"
                            CssClass="btn btn-danger"
                            CausesValidation="false"
                            UseSubmitBehavior="false"
                            OnClick="btnSave_Click1" Visible="true" />

                    </div>
                    <br>
                </div>


                <div class="row">
                    <div class="col-12">
                        <asp:GridView ID="gvPayments" runat="server" AutoGenerateColumns="false" Font-Size="12px" class="table table-bordered table-striped table-hover js-basic-example dataTable">
                            <Columns>
                                <asp:TemplateField HeaderText="Voucher#">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="hypVoucher" runat="server" NavigateUrl='<%# "generateVoucher.aspx?o=2&x=" + Eval("paymentID") %>' Text='<%#Eval("VoucherNumber")%>' Target="_blank"></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="PaymentDate" HeaderText="Date" DataFormatString="{0:dd.MM.yyyy}" />
                                <asp:BoundField DataField="PayeeType" HeaderText="Payee Type" />
                                <asp:BoundField DataField="AccountName" HeaderText="Paid From" />
                                <asp:BoundField DataField="ReferenceNo" HeaderText="Description" />
                                <asp:BoundField DataField="PayeeName" HeaderText="Payee" />
                                <asp:BoundField DataField="PaymentAmount" HeaderText="Amount" />

                            </Columns>
                        </asp:GridView>
                        <asp:HiddenField ID="hdCOAAccountCode" runat="server" />
                        <asp:HiddenField ID="hdBankAccount" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>
        function calculateJournalTotals() {

            var totalDebit = 0;
            var totalCredit = 0;

            $('.journal-debit').each(function () {

                var value = parseFloat($(this).val());

                if (!isNaN(value)) {
                    totalDebit += value;
                }
            });

            $('.journal-credit').each(function () {

                var value = parseFloat($(this).val());

                if (!isNaN(value)) {
                    totalCredit += value;
                }
            });


            $('#lblJournalDebit').text(
                totalDebit.toLocaleString(
                    'en-US',
                    {
                        minimumFractionDigits: 2,
                        maximumFractionDigits: 2
                    }
                )
            );

            $('#lblJournalCredit').text(
                totalCredit.toLocaleString(
                    'en-US',
                    {
                        minimumFractionDigits: 2,
                        maximumFractionDigits: 2
                    }
                )
            );

            $('#lblJournalDifference').text(
                difference.toLocaleString(
                    'en-US',
                    {
                        minimumFractionDigits: 2,
                        maximumFractionDigits: 2
                    }
                )
            );

            if (Math.abs(difference) < 0.01) {

                $('#lblJournalDifference')
                    .removeClass('text-danger')
                    .addClass('text-success');

            }
            else {

                $('#lblJournalDifference')
                    .removeClass('text-success')
                    .addClass('text-danger');
            }
        }
    </script>
</asp:Content>

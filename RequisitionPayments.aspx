<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeBehind="RequisitionPayments.aspx.cs" Inherits="LIMS.RequisitionPayments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

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
                                <div class="col-sm-6">
                                    <div class="card" style="padding: 15px; height:150px">
                                        <div class="card-header subheader">
                                            <h5>Debit</h5>
                                        </div>
                                        <div class="card-body">
                                            <div class="col-sm-4">
                                                <b>Pay Amount</b>
                                                <asp:TextBox ID="txtPayAmount" runat="server" CssClass="form-control" Placeholder="Amount" OnTextChanged="txtPayAmount_TextChanged" AutoPostBack="true"></asp:TextBox>
                                            </div>
                                            <div class="col-sm-4">
                                                <b>Bank Account</b>
                                                <asp:DropDownList ID="ddlBank" runat="server" CssClass="form-control"></asp:DropDownList>
                                            </div>
                                            <div class="col-sm-4">
                                                <b>COA Account</b>
                                                <asp:DropDownList ID="ddlCOADr" runat="server" CssClass="form-control"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="card" style="padding: 15px;height:150px">
                                        <div class="card-header subheader">
                                            <h5>Credit</h5>
                                        </div>
                                        <div class="card-body">
                                            <div class="col-sm-6">
                                                <b>Balance Amount</b>
                                                <asp:TextBox ID="txtBalanceAmount" runat="server" CssClass="form-control" Enabled="false" Placeholder="Current Payment"></asp:TextBox>
                                            </div>
                                            <div class="col-sm-6">
                                                <b>COA Account</b>
                                                <asp:DropDownList ID="ddlCOACr" runat="server" CssClass="form-control"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-2">
                                    <br />
                                    <asp:Button ID="btnSaveDraft" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSaveDraft_Click" Visible="true" />
                                </div>
                            </div>
                            <div class="row">
                                <asp:GridView ID="gvPaymentRequisitions" runat="server" CssClass="table" DataKeyNames="PaymentRequisitionID,TotalAmount,AmountPaid"
                                    AutoGenerateColumns="false" ShowFooter="True">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Select" HeaderStyle-Width="60px">
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="chkSelectAll" runat="server" OnCheckedChanged="chkSelectAll_CheckedChanged" AutoPostBack="true" />
                                            </HeaderTemplate>

                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkPR" runat="server"
                                                    CssClass="chkPR"
                                                    AutoPostBack="true"
                                                    OnCheckedChanged="chkPR_CheckedChanged" />
                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                <asp:HiddenField ID="hfPaymentRequisitionID" runat="server" Value='<%# Eval("PaymentRequisitionID") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="RequisitionNumber"
                                            HeaderText="RequisitionNumber" />

                                        <asp:BoundField DataField="TotalAmount"
                                            HeaderText="TotalAmount"
                                            DataFormatString="{0:N2}" />
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
                        <%--                        <asp:GridView ID="gvPaymentRequisition"
                            runat="server"
                            AutoGenerateColumns="False"
                            DataKeyNames="PaymentRequisitionDetailsID"
                            OnRowCommand="gvPaymentRequisition_RowCommand"
                            OnRowDataBound="gvPaymentRequisition_RowDataBound"
                            class="table table-bordered table-striped table-hover js-basic-example dataTable">
                            <Columns>

                                <asp:TemplateField HeaderText="Sl#">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                        <asp:HiddenField ID="hfPaymentRequisitionDetailsID" runat="server" Value='<%# Eval("PaymentRequisitionDetailsID") %>' />
                                    </ItemTemplate>
                                    <ItemStyle Width="2%"></ItemStyle>
                                </asp:TemplateField>

                                <asp:BoundField DataField="ItemDescription"
                                    HeaderText="Description" />

                                <asp:BoundField DataField="Quantity"
                                    HeaderText="Qty" />

                                <asp:BoundField DataField="TotalPrice"
                                    HeaderText="Amount"
                                    DataFormatString="{0:N2}" />
                                <asp:TemplateField HeaderText="Journal">
                                    <ItemTemplate>

                                        <!-- JOURNAL CONTAINER -->
                                        <div class="journal-container">

                                            <!-- SHOW / HIDE BUTTON -->
                                            <asp:LinkButton
                                                ID="lnkJournal"
                                                runat="server"
                                                Text="View Journal"
                                                CommandName="Journal"
                                                CommandArgument='<%# Eval("PaymentRequisitionDetailsID") %>'
                                                CssClass="btn btn-sm"
                                                Style="background: #b34788; color: white;">
                                            </asp:LinkButton>

                                            <!-- JOURNAL PANEL -->
                                            <asp:Panel
                                                ID="pnlLineJournal"
                                                runat="server"
                                                Visible="false"
                                                CssClass="journal-panel mt-2">

                                                <div class="card">

                                                    <div class="card-header d-flex justify-content-between align-items-center">

                                                        <b>Journal Entries</b>

                                                        <!-- ADD JOURNAL ENTRY BUTTON -->
                                                        <asp:Button
                                                            ID="btnAddJournal"
                                                            runat="server"
                                                            Text="+ Add Journal Entry"
                                                            CssClass="btn btn-sm btn-success"
                                                            CommandName="AddJournal"
                                                            CommandArgument='<%# Eval("PaymentRequisitionDetailsID") %>' />

                                                    </div>

                                                    <div class="card-body">

                                                        <!-- JOURNAL GRID -->
                                                        <asp:GridView
                                                            ID="gvLineJournal"
                                                            runat="server"
                                                            AutoGenerateColumns="False"
                                                            CssClass="table table-bordered table-sm"
                                                            OnRowCommand="gvLineJournal_RowCommand"
                                                            OnRowDataBound="gvLineJournal_RowDataBound">

                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Seq">
                                                                    <ItemTemplate>
                                                                        <asp:Label
                                                                            ID="lblSequence"
                                                                            runat="server"
                                                                            Text='<%# Eval("SequenceNo") %>' />
                                                                    </ItemTemplate>

                                                                    <ItemStyle Width="5%" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Account">

                                                                    <ItemTemplate>

                                                                        <asp:DropDownList
                                                                            ID="ddlAccount"
                                                                            runat="server"
                                                                            CssClass="form-control form-control-sm">
                                                                        </asp:DropDownList>

                                                                        <asp:HiddenField
                                                                            ID="hfJournalLineID"
                                                                            runat="server"
                                                                            Value='<%# Eval("LineID") %>' />
                                                                        <asp:HiddenField ID="hfPaymentRequisitionDetailsID" runat="server" Value='<%# Eval("SourceID") %>' />
                                                                    </ItemTemplate>

                                                                    <ItemStyle Width="25%" />

                                                                </asp:TemplateField>


                                                                <asp:TemplateField HeaderText="Description">

                                                                    <ItemTemplate>

                                                                        <asp:TextBox
                                                                            ID="txtJournalDescription"
                                                                            runat="server"
                                                                            CssClass="form-control form-control-sm"
                                                                            Text='<%# Eval("LineDescription") %>' />

                                                                    </ItemTemplate>

                                                                    <ItemStyle Width="25%" />

                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Debit">

                                                                    <ItemTemplate>

                                                                        <asp:TextBox
                                                                            ID="txtDebit"
                                                                            runat="server"
                                                                            CssClass="form-control form-control-sm text-end journal-debit"
                                                                            Text='<%# Eval("Debit", "{0:0.00}") %>'
                                                                            onkeyup="calculateJournalTotals(this)"
                                                                            onchange="calculateJournalTotals(this)" />

                                                                    </ItemTemplate>

                                                                    <ItemStyle Width="15%" />

                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Credit">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox
                                                                            ID="txtCredit"
                                                                            runat="server"
                                                                            CssClass="form-control form-control-sm text-end journal-credit"
                                                                            Text='<%# Eval("Credit", "{0:0.00}") %>'
                                                                            onkeyup="calculateJournalTotals(this)"
                                                                            onchange="calculateJournalTotals(this)" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="15%" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Action">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkSaveJournal"
                                                                            runat="server"
                                                                            Text="Save Journal"
                                                                            CssClass="btn btn-sm btn-primary"
                                                                            CommandArgument='<%# Eval("SourceID") %>'
                                                                            OnClick="lnkSaveJournal_Click" />
                                                                        <asp:LinkButton
                                                                            ID="lnkDeleteJournal"
                                                                            runat="server"
                                                                            Text="Delete"
                                                                            CssClass="btn btn-sm btn-danger"
                                                                            CommandName="DeleteJournal"
                                                                            CommandArgument='<%# Eval("LineID") %>'
                                                                            OnClientClick="return confirm('Delete this journal entry?');">
                                                                        </asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="10%" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>

                                                        <!-- TOTALS -->
                                                        <div class="row mt-2 p-2"
                                                            style="background: #f8f8f8; border-radius: 4px;">

                                                            <div class="col-md-4">
                                                                <b>Debit:</b>

                                                                <asp:Label
                                                                    ID="lblLineDebit"
                                                                    runat="server"
                                                                    Text="0.00" />
                                                            </div>

                                                            <div class="col-md-4">
                                                                <b>Credit:</b>

                                                                <asp:Label
                                                                    ID="lblLineCredit"
                                                                    runat="server"
                                                                    Text="0.00" />
                                                            </div>

                                                            <div class="col-md-4">
                                                                <b>Difference:</b>

                                                                <asp:Label
                                                                    ID="lblLineDifference"
                                                                    runat="server"
                                                                    Text="0.00"
                                                                    ForeColor="Green" />
                                                            </div>

                                                        </div>

                                                    </div>

                                                    <asp:Button
                                                        ID="btnSaveJournal"
                                                        runat="server"
                                                        Text="Save Journal"
                                                        CssClass="btn btn-primary"
                                                        OnClick="btnSaveJournal_Click" Visible="false" />
                                                </div>

                                            </asp:Panel>

                                        </div>

                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>--%>
                    </div>
                </div>
                <div class="row-clearfix">
                    <div class="col-sm-6">
                        <b>Notes</b>
                        <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    </div>

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

            var payAmount =
                parseFloat($('#<%= txtPayAmount.ClientID %>').val()) || 0;

            var difference =
                payAmount - totalDebit;

            $('#<%= txtBalanceAmount.ClientID %>').val(
                difference.toFixed(2)
            );

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

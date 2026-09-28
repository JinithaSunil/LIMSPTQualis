<%@ Page Title="" Language="C#" MasterPageFile="~/CRF.Master" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeBehind="EnquiryForm.aspx.cs" Inherits="LIMS.EnquiryForm" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .rectangle {
            width: 100%;
            height: 100%;
            border-color: #555;
            border: 1px solid;
            padding: 10px;
        }

        .custom-card {
            background-color: #f9f9f9;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
            border-radius: 10px;
            max-width: 800px;
            width: 90%;
            overflow: hidden;
            margin-top: 30px;
            font-family: 'Segoe UI', sans-serif;
        }

        input[type="checkbox"] + label {
            display: inline-flex;
            align-items: center;
            white-space: nowrap;
            gap: 5px;
            margin-left: 5px;
        }

        .card-header {
            color: white;
            font-size: 20px;
            font-weight: bold;
            padding: 20px;
            text-align: center;
        }

        .btn {
            background-color: #B74181;
            color: white;
        }
        /* New color background */
        .magenta-bg {
            background-color: #B74181; /* extracted from your screenshot */
        }

        .rbl1 input[type="radio"] {
            margin-left: 0px;
            margin-right: 120px;
        }
    </style>
    <script>
        function isDecimal(evt, input) {
            var charCode = evt.which ? evt.which : evt.keyCode;
            var charStr = String.fromCharCode(charCode);

            // Allow numbers
            if (/[0-9]/.test(charStr)) {
                return true;
            }

            // Allow one decimal point
            if (charStr === '.' && input.value.indexOf('.') === -1) {
                return true;
            }

            return false;
        }
</script>
    
    <script type="text/javascript">


        function preventPrefixEdit(event, textbox) {
            var value = textbox.value;
            var cursorPos = textbox.selectionStart;

            // Get currency symbol length (check for multi-char symbols like "Rp")
            var symbolLength = 0;
            if (value.startsWith('$') || value.startsWith('€') || value.startsWith('£') ||
                value.startsWith('¥') || value.startsWith('₹')) {
                symbolLength = 1;
            } else if (value.startsWith('Rp')) {
                symbolLength = 2;
            }

            // Prevent cursor movement or editing before symbol
            if (cursorPos < symbolLength) {
                // Allow only navigation keys
                var allowedKeys = [9, 35, 36, 37, 39, 46]; // Tab, End, Home, Left, Right, Delete
                if (allowedKeys.indexOf(event.keyCode) === -1) {
                    event.preventDefault();
                    textbox.setSelectionRange(symbolLength, symbolLength);
                    return false;
                }
            }

            return true;
        }

        function handlePaste(event, textbox) {
            event.preventDefault();
            var pastedText = (event.clipboardData || window.clipboardData).getData('text');
            var value = textbox.value;
            var cursorPos = textbox.selectionStart;

            // Get symbol length
            var symbolLength = 0;
            if (value.startsWith('Rp')) symbolLength = 2;
            else if (value.match(/^[$€£¥₹]/)) symbolLength = 1;

            // Ensure paste happens after symbol
            if (cursorPos < symbolLength) {
                cursorPos = symbolLength;
            }

            // Insert pasted text after symbol
            var newValue = value.substring(0, cursorPos) + pastedText + value.substring(textbox.selectionEnd);
            textbox.value = newValue;
            textbox.setSelectionRange(cursorPos + pastedText.length, cursorPos + pastedText.length);

            return false;
        }

        function CalculateTotals() {
            var grid = document.getElementById("<%# gvEnquiry.ClientID%>");
            for (var i = 0; i < grid.rows.length - 1; i++) {
                var txtAmountReceive = $("input[id*=txtQty]")
                if (txtAmountReceive[i].value != '') {
                    alert(txtAmountReceive[i].value);
                }
            }
        }


        // Function to validate if the date selected is in the past or future
        function validatePastDate() {
            var txtFollowUpDate = document.getElementById('<%= txtValidityDate.ClientID %>');
            var selectedDate = new Date(txtFollowUpDate.value);
            var today = new Date();

            // Clear the time portion of the date for accurate comparison
            today.setHours(0, 0, 0, 0);
            selectedDate.setHours(0, 0, 0, 0);

            // Check if the selected date is in the past
            if (selectedDate < today) {
                alert('Please select a date that is today or in the future.');
                txtFollowUpDate.value = ''; // Clear the invalid date
            }
        }

        // Function to set the minimum date to today's date
        function setMinDate() {
            var txtFollowUpDate = document.getElementById('<%= txtValidityDate.ClientID %>');
            var today = new Date();

            // Format today's date as YYYY-MM-DD
            var minDate = today.toISOString().split('T')[0];  // "2025-04-09"

            // Set the min date attribute to prevent selecting past dates
            txtFollowUpDate.setAttribute("min", minDate);
        }

    </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">


    <div class="bg-container-contact100" style="background-image: url(assets/img/bg-01.jpg);">
        <div class="contact100-header flex-sb-m"></div>
    </div>
    <div>
        <div class="container-contact100">
            <div style="background-color: white">
                <form runat="server" class="contact100-form validate-form" data-dpmaxz-eid="1">
                    <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>

                            <div id="divCRF" class="row clearfix" runat="server">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <div class="card"></div>
                                    <div id="divVendor" runat="server">
                                        <div class="header" style="text-align: center">
                                           <span style="float:left">
                                                        <img src="assets/img/logo.png" height='80' alt="PT Qualis" /></span>                                                    
                                                        <span style="text-align:center"><h2>ENQUIRY FORM</h2></span>
                                                    
                                        </div><br /><br /><br />
                                        <div class="body">
                                            <br />
                                            <div class="rectangle">
                                                <div class="row clearfix">
                                                    <div class="col-sm-12" style="text-align: center">
                                                        <h3><b>Vendor</b></h3>
                                                    </div>
                                                </div>
                                                <br />
                                                <div class="row clearfix">
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <label for="txtVendorName">Vendor Name</label>
                                                                <asp:TextBox ID="txtVendorName" CssClass="form-control" AutoComplete="off" placeholder="Vendor Name" runat="server" disabled></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <label for="txtVendorCode">Vendor Code</label>
                                                                <asp:TextBox ID="txtVendorCode" CssClass="form-control" AutoComplete="off" placeholder="Vendor Code" runat="server" disabled></asp:TextBox>
                                                                <asp:Label ID="lblVendorID" runat="server" Visible="false"></asp:Label>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <label for="txtDealer">Dealer</label>
                                                                <asp:TextBox ID="txtDealer" CssClass="form-control" AutoComplete="off" placeholder="Dealer" runat="server" disabled></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <br />
                                                <div class="row clearfix">
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <label for="txtYearofEstablishment">Year of Establishment</label>
                                                                <asp:TextBox ID="txtYearofEstablishment" CssClass="form-control" AutoComplete="off" runat="server" disabled></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <label for="txtTypeofEstablishment">Type of Establishment</label>
                                                                <asp:TextBox ID="txtTypeofEstablishment" CssClass="form-control" AutoComplete="off" runat="server" disabled></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <label for="txtTypeofBusiness">Type of Business</label>
                                                                <asp:TextBox ID="txtTypeofBusiness" CssClass="form-control" AutoComplete="off" runat="server" disabled></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="rectangle">
                                                <div class="row clearfix">
                                                    <div class="col-sm-12" style="text-align: center">
                                                        <h3><b>Enquiry</b></h3>
                                                        <br />
                                                    </div>
                                                </div>
                                                <div class="col-sm-12">
                                                    <div class="form-group">
                                                        <asp:CheckBox ID="chkNA" runat="server" Text="Not Available" AutoPostBack="true" OnCheckedChanged="chkNA_CheckedChanged" />
                                                    </div>
                                                </div>
                                                <br />

                                                <div class="row clearfix">
                                                    <div class="col-md-4" id="divQuoteCurrency" runat="server">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <label for="ddlQuoteCurrency">Quote Currecy</label>
                                                                <asp:DropDownList ID="ddlQuoteCurrency" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="ddlQuoteCurrency_SelectedIndexChanged">
                                                                    <%-- <asp:ListItem Value="IDR" Selected="True">Indonesia Rupiah</asp:ListItem>--%>
                                                                </asp:DropDownList>

                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <label for="txtPaymentTerms">Payment Terms</label>
                                                                <asp:DropDownList ID="ddlPaymentTerms" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlPaymentTerms_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                <asp:TextBox ID="txtPaymentTerms" runat="server" CssClass="form-control" Visible="false" placeholder="Enter Partial Payment Details"></asp:TextBox>

                                                            </div>

                                                        </div>
                                                    </div>
                                                    <div class="col-md-4" id="divContactPerson" runat="server">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <label for="ddlContactPerson">ContactPerson</label>
                                                                <asp:DropDownList ID="ddlContactPerson" CssClass="form-control" runat="server" AutoPostBack="true" OnTextChanged="ddlContactPerson_SelectedIndexChanged"></asp:DropDownList>

                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row clearfix">
                                                    <div class="col-md-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <label for="txtValidityDate">Validity Date</label>
                                                                <asp:TextBox ID="txtValidityDate" runat="server" CssClass="form-control" AutoComplete="false" TextMode="Date" onfocus="setMinDate()" onblur="validatePastDate()" AutoPostBack="true" OnTextChanged="txtValidityDate_TextChanged"></asp:TextBox>
                                                                <%-- <ajaxToolkit:CalendarExtender ID="CalendarExtender3" runat="server" TargetControlID="txtValidityDate" Format="dd/MMM/yyyy"></ajaxToolkit:CalendarExtender>--%>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <label for="txtQuotationNumber">Quotation Number</label>
                                                                <asp:TextBox ID="txtQuotationNumber" CssClass="form-control" runat="server" AutoPostBack="true" OnTextChanged="txtQuotationNumber_TextChanged"></asp:TextBox>


                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-12 col-md-12 col-12" style="padding: 10px; height: 100%;">

                                                        <asp:GridView ID="gvEnquiry" runat="server" class="table table-striped table-bordered table-hover table-checkable order-column table-responsive"
                                                            AutoGenerateColumns="false" OnRowDataBound="gvEnquiry_RowDataBound"
                                                            ShowFooter="true">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Sl#">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="1%"></ItemStyle>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Item">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblVendorQuotationID" runat="server" Text='<%# Eval("VendorQuotationID").ToString() %>' Visible="false"></asp:Label>
                                                                        <asp:Label ID="lblVendorQuotationDetailsID" runat="server" Text='<%# Eval("VendorQuotationDetailsID").ToString() %>' Visible="false"></asp:Label>
                                                                        <asp:Label ID="lblItemDescription" runat="server" Text='<%# Eval("ItemDescription") + " " + Eval("Specification") + "  "+  Eval("ModelNo") %>'></asp:Label><br />
                                                                        <asp:Label ID="lblUserRemarksOrig" runat="server" Text='<%# Eval("UserRemarks").ToString() %>' Visible="false"></asp:Label>
                                                                        <asp:Label ID="lblUserRemarks" runat="server" Text=""></asp:Label>
                                                                        <asp:TextBox ID="txtSpecification" runat="server" placeholder="Enter Brand / Specification" TextMode="MultiLine" Rows="2"></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="5%"></ItemStyle>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="ItemCode">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval("ItemCode").ToString() %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="5%"></ItemStyle>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Unit">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblUOM" runat="server" Text='<%# Eval("UOMName").ToString() %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="3%"></ItemStyle>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Qty">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblOrderedQty" runat="server" Text='<%# Eval("OrderedQty").ToString().ToUpper() %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="3%"></ItemStyle>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Remarks" Visible="false">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblRemarks" runat="server" class="form-control" Text='<%# Eval("Remarks").ToString() %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="10%"></ItemStyle>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Rate Per Unit">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="txtRate" runat="server" Text='<%#Eval("QuoteRate") %>' class="form-control" Width="100px" Font-Size="12px"
                                                                            OnTextChanged="txtRate_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                        <asp:HiddenField ID="hfCurrencyCode" runat="server" />
                                                                        <asp:RequiredFieldValidator ID="rfvRate" runat="server" ControlToValidate="txtRate"
                                                                            ErrorMessage="Rate is required" ForeColor="Red" Display="Dynamic" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="5%"></ItemStyle>
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Delivery (in Days)">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="txtDeliveryTime" runat="server" Text='<%#Eval("DeliveryTime") %>' CssClass="form-control" AutoPostBack="True" Width="80px" Font-Size="12px"></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="rfvDelivery" runat="server" ControlToValidate="txtDeliveryTime"
                                                                            ErrorMessage="Delivery time required" ForeColor="Red" Display="Dynamic" />
                                                                        <ajaxToolkit:FilteredTextBoxExtender ID="feDeliveryTime" runat="server" Enabled="true" TargetControlID="txtDeliveryTime" FilterType="Numbers" ValidChars="0123456789" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="3%"></ItemStyle>
                                                                </asp:TemplateField>
                                                                <%--  <asp:TemplateField HeaderText="Tax(%)">
                                                                        <ItemTemplate>
                                                                            <asp:DropDownList ID="ddlGST" class="form-control" runat="server" Font-Size="12px" OnSelectedIndexChanged="ddlGST_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                        </ItemTemplate>
                                                                        <ItemStyle Width="7%"></ItemStyle>
                                                                    </asp:TemplateField>--%>
                                                                <%--  <asp:TemplateField HeaderText="Tax(%)">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="ddlVAT" class="form-control" runat="server" Font-Size="12px" OnSelectedIndexChanged="ddlVAT_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                    <asp:RequiredFieldValidator ID="rfvVAT" runat="server" ControlToValidate="ddlVAT"
                                                                        InitialValue="" ErrorMessage="Please select VAT" ForeColor="Red" Display="Dynamic" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="12%"></ItemStyle>
                                                            </asp:TemplateField>--%>
                                                                <asp:TemplateField HeaderText="Discount(%)">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="txtDiscount" class="form-control" runat="server" Font-Size="12px" AutoPostBack="true" onkeypress="return isDecimal(event, this);" OnTextChanged="txtDiscount_TextChanged"></asp:TextBox>

                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="12%"></ItemStyle>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Discount Applied" ItemStyle-HorizontalAlign="Right" SortExpression="DiscountApplied" Visible="true">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblDiscountApplied" runat="server" Text="0.00" CssClass="text-right"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="8%" HorizontalAlign="Right" />
                                                                </asp:TemplateField>


                                                                <asp:TemplateField HeaderText="Total" ItemStyle-HorizontalAlign="Center">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotalAmount" runat="server"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="5%"></ItemStyle>
                                                                    <%-- --%>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Remarks">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="txtRemarks1" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="10%"></ItemStyle>

                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                        <div class="row" style="margin-top: 10px;">
                                                            <div class="col-lg-12">

                                                                <table class="table table-bordered" style="height: 100%">

                                                                    <!-- Sub Total -->
                                                                    <tr>
                                                                        <td style="width: 70%; text-align: right; font-weight: bold;">Sub Total:
                                                                        </td>
                                                                        <td style="width: 30%; text-align: right;">
                                                                            <asp:Label ID="lblSubTotal"
                                                                                runat="server"
                                                                                Font-Weight="Bold">
                                                                            </asp:Label>
                                                                        </td>
                                                                    </tr>

                                                                    <!-- Additional Charge -->
                                                                    <tr>
                                                                        <td style="text-align: right; font-weight: bold;">
                                                                            <asp:TextBox ID="txtAdditionalChargeDescription"
                                                                                runat="server"
                                                                                CssClass="form-control"
                                                                                placeholder="Additional charge (e.g. Pick up & Delivery)"
                                                                                Style="width: 60%; display: inline-block; text-align:right; font-size:12px !important">
                                                                            </asp:TextBox>
                                                                        </td>

                                                                        <td style="text-align: center;">
                                                                            <asp:TextBox ID="txtAdditionalCharge"
                                                                                runat="server"
                                                                                CssClass="form-control"
                                                                                Text="0.00"
                                                                                Style="text-align: right;" OnTextChanged="txtAdditionalCharge_TextChanged" AutoPostBack="true">
                                                                            </asp:TextBox>
                                                                        </td>
                                                                    </tr>

                                                                    <!-- Total Before VAT -->
                                                                    <tr>
                                                                        <td style="text-align: right; font-weight: bold;">Total:
                                                                        </td>

                                                                        <td style="text-align: right;">
                                                                            <asp:Label ID="lblTotalBeforeVAT"
                                                                                runat="server"
                                                                                Font-Weight="Bold">
                                                                            </asp:Label>
                                                                        </td>
                                                                    </tr>

                                                                    <!-- TAX -->
                                                                    <tr>
                                                                        <td style="text-align: right; font-weight: bold;">TAX:
                                                                        </td>

                                                                        <td>
                                                                            <table style="width: 100%">
                                                                                <tr>
                                                                                    <td style="width: 50%">
                                                                                        <asp:DropDownList ID="ddlVAT"
                                                                                            class="form-control"
                                                                                            runat="server"
                                                                                            Font-Size="12px"
                                                                                            OnSelectedIndexChanged="ddlVAT_SelectedIndexChanged"
                                                                                            AutoPostBack="true">
                                                                                        </asp:DropDownList>
                                                                                    </td>
                                                                                    <td style="width: 50%; text-align: right">
                                                                                        <asp:Label ID="lblVat" runat="server"></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>


                                                                        </td>
                                                                    </tr>

                                                                    <!-- Grand Total -->
                                                                    <tr>
                                                                        <td style="text-align: right; font-weight: bold; font-size: 16px;">Grand Total:
                                                                        </td>

                                                                        <td style="text-align: right;">
                                                                            <asp:Label ID="lblGrandTotal"
                                                                                runat="server"
                                                                                Font-Weight="Bold"
                                                                                Font-Size="16px">
                                                                            </asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="2">
                                                                            <b>Special Instruction</b>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="2">
                                                                            <asp:TextBox ID="txtSpecialInstruction" runat="server" CssClass="form-control" TextMode="MultiLine"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                </table>

                                                            </div>
                                                        </div>
                                                        <asp:HiddenField ID="hdQuotationTotal" runat="server" Value="0" />

                                                    </div>


                                                </div>

                                            </div>
                                            <br />
                                            <div class="rectangle">
                                                <div class="row clearfix">

                                                    <div class="col-sm-12" style="text-align: center">
                                                        <h3><b>Terms & Conditions</b></h3>
                                                        <br />

                                                    </div>
                                                </div>
                                                <br />
                                                <div class="row clearfix">
                                                    <div class="col-md-3 mb-3">
                                                        <label for="txtRemarks">Remarks</label>
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-3 mb-3">
                                                        <label for="txtDelivery">Delivery</label>
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtDelivery" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-md-3 mb-3">
                                                        <label for="fpAttach">Document</label>
                                                        <asp:FileUpload ID="fpDocument" runat="server" Width="100%" CssClass="btn btn-primary" />
                                                        <asp:Label ID="lblDocumentName" runat="server" CssClass="form-text text-muted"></asp:Label>
                                                    </div>
                                                    <div class="col-md-3 mb-3">
                                                        <label for="fpAttach">Certificate</label>
                                                        <asp:FileUpload ID="fpCertificate" runat="server" Width="100%" CssClass="btn btn-primary" />
                                                        <asp:Label ID="lblCertificateName" runat="server" CssClass="form-text text-muted"></asp:Label>
                                                    </div>
                                                </div>

                                            </div>
                                            <div class="row clearfix">
                                                <div class="col-sm-4">
                                                </div>
                                                <div class="col-sm-4">
                                                    <asp:Button ID="btnSave" runat="server" class="btn m-t-15" Text="Submit" OnClick="btnSave_Click" />
                                                </div>

                                            </div>

                                        </div>
                                    </div>
                                </div>


                                <div class="row clearfix" style="margin-top: 30px;">
                                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                        <div style="display: flex; justify-content: center; margin-bottom: 20px;">
                                            <div class="custom-card" id="divSubmited" runat="server" style="background: #fff; border: 2px solid #e0e0e0; border-radius: 8px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); max-width: 525px; width: 100%; overflow: hidden;">
                                                <div class="card-header magenta-bg">
                                                    <div class="card-header magenta-bg">
                                                        Quotation Already Submitted.
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="row clearfix" style="margin-top: 30px;">
                                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                        <div style="display: flex; justify-content: center; margin-bottom: 20px;">
                                            <div class="custom-card" id="divSuccess" runat="server" style="background: #fff; border: 2px solid #e0e0e0; border-radius: 8px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); max-width: 400px; width: 100%; overflow: hidden;">
                                                <div class="card-header magenta-bg">
                                                    <div class="card-header magenta-bg">
                                                        Quotation Submitted Successfully.
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="row clearfix" style="margin-top: 30px;">
                                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                        <div style="display: flex; justify-content: center; margin-bottom: 20px;">
                                            <div class="custom-card" id="divClosed" runat="server" style="background: #fff; border: 2px solid #e0e0e0; border-radius: 8px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); max-width: 400px; width: 100%; overflow: hidden;">
                                                <div class="card-header magenta-bg">
                                                    <div class="card-header magenta-bg">
                                                        Enquiry Closed. Quote cannot be submitted.
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            </div>

                            <br />


                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btnSave" />
                        </Triggers>
                    </asp:UpdatePanel>

                </form>
            </div>
        </div>
    </div>


</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ItemArrangementEdit.aspx.cs" Inherits="LIMS.ItemArrangementEdit" %>

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
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="page-wrapper">
        <div class="page-content">

            <div class="row">
                <div class="col-xl-12 mx-auto">

                    <div class="card">
                        <div class="header bg-violet">
                            <h2 style="color: white !important">ITEM STORE IN WAREHOUSE EDIT</h2>
                        </div>
                        <div class="body">
                            <asp:Label ID="lblMsg" runat="server" ForeColor="Red" Font-Size="Large"></asp:Label>
                            <asp:Label ID="lblPoID" runat="server"></asp:Label>
                            <asp:Label ID="lblPoNo" runat="server"></asp:Label>
                            <asp:Label ID="lblorderQty" runat="server" ForeColor="White"></asp:Label>
                            <asp:Label ID="lblreceiveQty" runat="server" ForeColor="White"></asp:Label>
                            <asp:Label ID="lblPreReceivedQty" runat="server"></asp:Label>
                            <asp:Label ID="lblbalanceQty" runat="server"></asp:Label>


                            <div class="row mt-4 mb-4">
                                <div class="col-md-3">
                                    <label for="input1" class="form-label">GRN Number </label>
                                    <asp:TextBox ID="txtGRNNumber" runat="server" class="form-control"></asp:TextBox>

                                    <ajaxToolkit:AutoCompleteExtender ID="autoCompleteExtender1" runat="server"
                                        TargetControlID="txtGRNNumber"
                                        MinimumPrefixLength="1"
                                        EnableCaching="true"
                                        CompletionInterval="100"
                                        CompletionSetCount="10"
                                        ServiceMethod="GetSuggestions">
                                    </ajaxToolkit:AutoCompleteExtender>

                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtGRNNumber"
                                        InitialValue="" ErrorMessage="PO Required" Display="Dynamic" CssClass="text-danger">
                                    </asp:RequiredFieldValidator>
                                </div>

                                <div class="col-md-1">
                                    <button type="button" onserverclick="btnSearch_Click" runat="server" class="btn btn-outline-primary" style="margin-top: 18px;">
                                        <i class='bx bx-search'></i>SEARCH</button>
                                </div>


                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Item Code</label>
                                    <asp:DropDownList ID="ddlPoItemCode" class="form-control" runat="server" AutoPostBack="true"
                                        OnSelectedIndexChanged="ddlPoItemCode_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Item Name</label>
                                    <asp:TextBox ID="txtItemName" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-md-2">
                                    <label for="input1" class="form-label">HSN Code</label>
                                    <asp:TextBox ID="txtHSNCode" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>

                            </div>

                            <hr />
                            Item Transaction Details

                                <div class="table-responsive">
                                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
                                        HeaderStyle-CssClass="table-header-bg">
                                        <Columns>
                                            <asp:BoundField DataField="GRN_PoNumber" HeaderText="PO No." SortExpression="PONo" />
                                            <asp:BoundField DataField="ItemCode" HeaderText="Item Code" SortExpression="ItemCode" />
                                            <asp:BoundField DataField="ItemName" HeaderText="Item Name" SortExpression="ItemName" />
                                            <asp:BoundField DataField="IT_OrderedQty" HeaderText="Order Qty." SortExpression="OrderQty" />
                                            <asp:BoundField DataField="IT_PreviousReceivedQty" HeaderText="Pre. Received Qty." SortExpression="PoReceivedQty" />
                                            <asp:BoundField DataField="IT_ReceivedQty" HeaderText="Received Qty." SortExpression="PoReceivedQty" />
                                            <asp:BoundField DataField="IT_BalanceQty" HeaderText="Balance Qty." SortExpression="PoBalanceQty" />
                                            <asp:BoundField DataField="IT_BasicPrice" HeaderText="Basic Price(₹)" SortExpression="PoBasicPrice" />
                                        </Columns>
                                    </asp:GridView>
                                </div>

                            <hr />

                            <div class="row mt-4 mb-4">

                                <div class="col-md-4">
                                    <label for="input1" class="form-label">Vendor</label>
                                    <asp:DropDownList ID="ddlVendor" class="form-control" runat="server">
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-2">
                                    <label for="input1" class="form-label">Current Stock</label>
                                    <asp:TextBox ID="txtCurrentStock" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-md-2">
                                    <label for="input1" class="form-label">Min. Stock</label>
                                    <asp:TextBox ID="txtMinStock" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>

                            </div>

                            <div class="row mt-4 mb-4">
                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Basic Price(₹)</label>
                                    <asp:TextBox ID="txtBasicPrice" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-md-3">
                                    <label for="input1" class="form-label">UOM</label>
                                    <asp:DropDownList ID="ddlUOM" CssClass="form-control" runat="server">
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-3">
                                    <label for="input1" class="form-label">TAX</label>
                                    <asp:DropDownList ID="ddlVAT" CssClass="form-control" runat="server">
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Delivery Date</label>
                                    <asp:TextBox ID="txtDeliveryDateTime" runat="server" class="form-control" TextMode="DateTimeLocal"></asp:TextBox>
                                </div>
                            </div>

                            <div class="row mt-4 mb-4">
                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Pre. Received Qty.</label>
                                    <asp:TextBox ID="txtPreReceivedQty" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Pre. Balance Qty.</label>
                                    <asp:TextBox ID="txtPreBalanceQty" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Pre. Accept Qty.</label>
                                    <asp:TextBox ID="txtPreAcceptQty" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Pre. Rejecte Qty.</label>
                                    <asp:TextBox ID="txtPreRejectedQty" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>

                            <div class="row mt-4 mb-4">
                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Received Qty.</label>
                                    <asp:TextBox ID="txtReceivedQty" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>

                                </div>

                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Balance Qty.</label>
                                    <asp:TextBox ID="txtBalanceQty" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>


                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Accept Qty.</label>
                                    <asp:TextBox ID="txtAcceptQty" runat="server" class="form-control"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="revAcceptQty" runat="server" ControlToValidate="txtAcceptQty"
                                        ErrorMessage="Please enter a valid positive number or decimal" ValidationExpression="^\d+(\.\d+)?$"
                                        ForeColor="Red" Display="Dynamic"></asp:RegularExpressionValidator>
                                </div>

                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Rejecte Qty.</label>
                                    <asp:TextBox ID="txtRejectQty" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtRejectQty"
                                        ErrorMessage="Please enter a valid positive number or decimal" ValidationExpression="^\d+(\.\d+)?$"
                                        ForeColor="Red" Display="Dynamic"></asp:RegularExpressionValidator>
                                </div>
                            </div>

                            <div class="row mt-4 mb-4">
                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Batch No.</label>
                                    <asp:TextBox ID="txtBatchNo" runat="server" class="form-control"></asp:TextBox>
                                </div>

                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Expiry Date</label>
                                    <asp:TextBox ID="txtExpiryDate" runat="server" class="form-control" TextMode="Date"></asp:TextBox>
                                </div>
                            </div>

                            <hr />
                            <div class="row g-3 mb-2">
                                <div class="col-md-3">
                                    <asp:DropDownList ID="ddlRackType" CssClass="form-control" runat="server">
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="row g-3 mt-2 mb-2">
                                <div class="col-md-4">
                                    <label for="input1" class="form-label">Item Store Location Allocate Show</label>
                                    <asp:CheckBoxList ID="chkRowSeries" runat="server" CssClass="custom-checkboxbindDB" RepeatDirection="Vertical">
                                    </asp:CheckBoxList>
                                </div>
                                <div class="col-md-4" id="Div1" runat="server">
                                    <asp:Repeater ID="Repeater1" runat="server" EnableViewState="true">
                                        <ItemTemplate>
                                            <asp:Label ID="lblRowTotal" runat="server" Text='<%# Eval("RackSerialId") %>' Visible="false"></asp:Label>
                                            <asp:CheckBox ID="chkRowTotal" runat="server" CssClass="custom-checkboxbindDB" Text='<%# Eval("RackSerialNumber") %>' RepeatDirection="Vertical" Checked="true" Enabled="false" />
                                            <asp:TextBox ID="txtBox" runat="server" CssClass="custom-textbox form-control"></asp:TextBox>
                                            <br />
                                        </ItemTemplate>
                                    </asp:Repeater>

                                </div>
                            </div>
                            <hr />

                            <asp:Label ID="msg" runat="server"></asp:Label>

                            <div class="row g-3 mt-3 mb-3">
                                <div class="col-md-4">
                                    <asp:Button ID="btnAdd" runat="server" Text="ADD" CssClass="btn btn-primary" OnClick="btnAdd_Click" />
                                </div>
                            </div>

                        </div>



                    </div>
                </div>



            </div>
        </div>
        <!--end row-->
    </div>


    <script src="https://code.jquery.com/jquery-3.6.0.min.js" integrity="sha384-KyZXEAg3QhqLMpG8r+8fhAXLR2fGW9KFElKu+5B4sUWJp6e8pc2O/nGwCqk4bL6W" crossorigin="anonymous"></script>
    <script>
        $(document).ready(function () {
            /* Attach an event listener to the txtReceivedQty field */
            $('#<%= txtAcceptQty.ClientID %>').on('input', function () {
            updateBalanceQty(); /* Call updateBalanceQty() function whenever the input changes */
        });
    });

        function updateBalanceQty() {

            var acceptQty = $('#<%= txtAcceptQty.ClientID %>').val();
        var poOrderedQty = $('#<%= lblorderQty.ClientID %>').text();
        var poReceivedQty = $('#<%= lblreceiveQty.ClientID %>').text();

        /* AJAX POST request */
        $.ajax({
            type: "POST",
            url: "ItemArrangementAdd.aspx/UpdateBalanceQty",
            data: JSON.stringify({ acceptQty: acceptQty, poOrderedQty: poOrderedQty, poReceivedQty: poReceivedQty }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                $('#<%= txtRejectQty.ClientID %>').val(response.d);
         <%--$('#<%= hdnBalanceQty.ClientID %>').val(updatedBalanceQty);     /* Store in hidden field */--%> 
                $('#<%= RegularExpressionValidator1.ClientID %>').focusout();   /* Trigger validation after updating txtBalanceQty */
            },
            error: function (xhr, textStatus, errorThrown) {
                console.log('Error:', errorThrown);
            }
        });
        }
    </script>
</asp:Content>

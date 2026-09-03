<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="MaterialRequestFormShow.aspx.cs" Inherits="LIMS.MaterialRequestFormShow" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .custom-checkboxbindDB input[type="checkbox"] {
            margin-right: 5px;
            opacity: 1;
            left: 1191px !important;
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
                            <h2 style="color: white !important">MATERIAL REQUISITION FOR RELEASE</h2>
                        </div>
                        <div class="body">

                            <asp:Label ID="lblPageID" runat="server"></asp:Label>
                            <asp:Label ID="lblMRNumber" runat="server"></asp:Label>
                            <asp:Label ID="lblMRMID" runat="server" Visible="false"></asp:Label>

                            

                            <div class="row mt-4 mb-4">
                                <div class="table-responsive">
                                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
                                        HeaderStyle-CssClass="gridview-header" OnRowDataBound="GridView1_RowDataBound" DataKeyNames="MR_ID">
                                        <Columns>
                                            <asp:TemplateField HeaderText="#">
                                                <ItemTemplate>
                                                    <%# Container.DataItemIndex + 1 %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="MR_NUMBER" HeaderText="MR No" />
                                            <%-- <asp:BoundField DataField="MRM_ProjectCode" HeaderText="Project Code" />--%>
                                            <asp:BoundField DataField="MR_ItemCode" HeaderText="Item Code" />
                                            <asp:BoundField DataField="MR_ItemName" HeaderText="Item Name" />
                                            <asp:BoundField DataField="MR_EmployeeRequireItemQty" HeaderText="Required Qty." />
                                            <asp:BoundField DataField="MR_ReceiveQty" HeaderText="Received Qty." />
                                            <asp:BoundField DataField="MR_BalanceQty" HeaderText="Balance Qty." />
                                            <asp:BoundField DataField="MR_UOM" HeaderText="UOM" />
                                            <asp:BoundField DataField="MR_RequireDate" HeaderText="Require Date" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                            <hr />
                            <%-- <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>--%>
                            <div class="row mt-4 mb-4">
                                <div class="col-md-2">
                                    <label for="input1" class="form-label">Item Code </label>
                                    <asp:DropDownList ID="ddlItemCode" CssClass="form-control" runat="server"
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlItemCode_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-2">
                                    <label for="input1" class="form-label">Pre.Item Send</label>
                                    <asp:TextBox ID="txtPreItemSend" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-md-2">
                                    <label for="input1" class="form-label">Pre.Balance</label>
                                    <asp:TextBox ID="txtPreBalanceQty" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-md-2">
                                    <label for="input1" class="form-label">UOM</label>
                                    <asp:DropDownList ID="ddlUOM" CssClass="form-control" runat="server">
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-2">
                                    <label for="input1" class="form-label">Item Qty</label>
                                    <asp:TextBox ID="txtItemQtySend" runat="server" CssClass="form-control" placeholder="Item Qty"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtItemQtySend" runat="server" ErrorMessage="Required" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regexValidator" runat="server" ControlToValidate="txtItemQtySend"
                                        ValidationExpression="^(?!0$)(?!0\.0*$)(\d+(?:\.\d{1,5})?)$" ErrorMessage="Please enter a valid positive decimal number greater than zero."
                                        Display="Dynamic" ForeColor="Red" SetFocusOnError="true">
                                    </asp:RegularExpressionValidator>
                                </div>

                                <div class="col-md-1">
                                    <label for="input1" class="form-label">Scan QR</label>
                                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>

                                <div class="col-md-1">
                                    <a id="btnFilter" onserverclick="btnFilterData_Click" runat="server" class="btn btn-primary" style="margin-top: 20px;">
                                        <i class="fa fa-plus"></i>
                                    </a>
                                </div>
                            </div>
                            <%--<hr />--%>

                            <div id="printableArea">
                                <div class="row mt-4 mb-4">
                                    <div class="table-responsive">
                                        <table id="example0" class="table table-bordered mb-0">
                                            <thead class="table-header">
                                                <tr>
                                                    <th>#</th>
                                                    <th>Item Codes</th>
                                                    <th>Current Stock</th>
                                                    <th>Min. Stock</th>
                                                    <th>Rack Type</th>
                                                    <th>Series No</th>
                                                    <th>Item In Rack</th>
                                                    <th>Batch No.</th>
                                                    <th>Expiry Date</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <!-- Data rows will be dynamically added here -->
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <%--<hr />--%>

                            <div class="row mt-4 mb-4">
                                <div class="table-responsive">
                                    <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
                                        HeaderStyle-CssClass="gridview-header" DataKeyNames="IS_ItemStoreId">
                                        <Columns>
                                            <asp:TemplateField HeaderText="#">
                                                <ItemTemplate>
                                                    <%# Container.DataItemIndex + 1 %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="ItemCode" HeaderText="Item Code" />
                                            <asp:BoundField DataField="InventoryCurrentStock" HeaderText="Current Stock" />
                                            <%--<asp:BoundField DataField="InventoryMinStock" HeaderText="Min.Stock" />--%>
                                            <asp:BoundField DataField="UOMName" HeaderText="UOM" />
                                            <asp:BoundField DataField="RackType" HeaderText="Rack" />
                                            <asp:BoundField DataField="RackSerialNumber" HeaderText="Se.No." />
                                            <asp:BoundField DataField="TotalItemInRack" HeaderText="Total Item" />
                                            <asp:BoundField DataField="CumulativeTotalItemInRack" HeaderText="Totals" />
                                            <asp:BoundField DataField="IT_BatchNumber" HeaderText="Batch No." />
                                            <asp:BoundField DataField="IT_ExpiryDateTime" HeaderText="Expiry Date" />

                                            <asp:TemplateField HeaderText="Item Release Note">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkIsQtyGo" runat="server" Checked='<%# Convert.ToBoolean(Eval("IsQtyGo")) %>' CssClass="custom-checkboxbindDB" RepeatDirection="Vertical"
                                                        OnCheckedChanged="chkIsQtyGo_CheckedChanged" AutoPostBack="true" />
                                                    <asp:TextBox ID="txtCustom" runat="server" Enabled="false" Width="100px"></asp:TextBox>
                                                    <%--<asp:TextBox ID="txtCustom" runat="server" Enabled="false" Text='<%# Eval("YourTextBoxField") %>'></asp:TextBox>--%>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkBothMatch" runat="server" Checked='<%# Convert.ToBoolean(Eval("BothMatch")) %>' Enabled="false" CssClass="custom-checkboxbindDB" RepeatDirection="Vertical" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>

                            <%--<hr />--%>

                            <div class="row mt-4 mb-4">
                                <div class="table-responsive">
                                    <asp:GridView ID="GridView3" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
                                        HeaderStyle-CssClass="gridview-header" DataKeyNames="IS_ItemStoreId">
                                        <Columns>
                                            <asp:TemplateField HeaderText="#">
                                                <ItemTemplate>
                                                    <%# Container.DataItemIndex + 1 %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="ItemCode" HeaderText="Item Code" />
                                            <asp:BoundField DataField="InventoryCurrentStock" HeaderText="Current Stock" />
                                            <%--<asp:BoundField DataField="InventoryMinStock" HeaderText="Min. Stock" />--%>
                                            <asp:BoundField DataField="UOMName" HeaderText="UOM" />
                                            <asp:BoundField DataField="RackType" HeaderText="Rack" />
                                            <asp:BoundField DataField="RackSerialNumber" HeaderText="Se.No." />
                                            <asp:BoundField DataField="TotalItemInRack" HeaderText="Total Item" />
                                            <asp:BoundField DataField="CumulativeTotalItemInRack" HeaderText="Totals" />
                                            <asp:BoundField DataField="IT_BatchNumber" HeaderText="Batch No." />
                                            <asp:BoundField DataField="IT_ExpiryDateTime" HeaderText="Expiry Date" />

                                            <asp:TemplateField HeaderText="Item Release Note">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkIsQtyGo" runat="server" Checked='<%# Convert.ToBoolean(Eval("IsQtyGo")) %>' CssClass="custom-checkboxbindDB" RepeatDirection="Vertical"
                                                        OnCheckedChanged="chkIsQtyGo_CheckedChanged" AutoPostBack="true" />
                                                    <asp:TextBox ID="txtCustom" runat="server" Enabled="false" Width="100px"></asp:TextBox>

                                                    <%--<asp:TextBox ID="txtCustom" runat="server" Enabled="false" Text='<%# Eval("YourTextBoxField") %>'></asp:TextBox>--%>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <%--<asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkIsQtyGo" runat="server" Checked='<%# Convert.ToBoolean(Eval("IsQtyGo")) %>' Enabled="true" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>--%>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>

                            <%--<hr />--%>

                            <div class="row g-3 mt-3 mb-3">
                                <div class="col-md-4">
                                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                                </div>
                            </div>
                            <%--</ContentTemplate>
                        </asp:UpdatePanel>--%>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>




    <script>
        $(document).ready(function () {
            $('#example0').hide();
            $('#<%= txtSearch.ClientID %>').on('input', function () {
                SearchData();
            });
        });

        function SearchData() {
            var TransactionId = $('#<%= txtSearch.ClientID %>').val();
            $.ajax({
                type: "POST",
                url: '<%= ResolveUrl("~/MaterialRequestFormShow.aspx/SearchData") %>',
                data: JSON.stringify({ transactionID: TransactionId }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    try {
                        var rawData = response.d;
                        var data = JSON.parse(rawData);

                        if (Array.isArray(data) && data.length > 0) {
                            var item = data[0];
                            $('#example0 tbody').empty();

                            $.each(data, function (index, row) {
                                $('#example0 tbody').append('<tr>' +
                                    '<td>' + (index + 1) + '</td>' +
                                    '<td>' + row.ItemCode + '</td>' +
                                    '<td>' + row.InventoryCurrentStock + " " + row.UOMName + '</td>' +
                                    '<td>' + row.InventoryMinStock + " " + row.UOMName + '</td>' +
                                    '<td>' + row.RackType + '</td>' +
                                    '<td>' + row.RackSerialNumber + '</td>' +
                                    '<td>' + row.TotalItemInRack + " " + row.UOMName + '</td>' +
                                    '<td>' + row.IT_BatchNumber + '</td>' +
                                    '<td>' + row.IT_ExpiryDateTime + '</td>' +
                                    '</tr>');
                            });
                            $('#example0').show();
                        } else {
                            $('#example0 tbody').empty();
                            $('#example0').hide();
                        }
                    } catch (e) {
                        alert("Error parsing response. Please try again.");
                    }
                },
                error: function (xhr, textStatus, errorThrown) {
                    alert("An error occurred while processing your request. Please try again.");
                }
            });
        }
    </script>

</asp:Content>

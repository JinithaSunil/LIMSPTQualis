<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PTQualisMaster.Master" CodeBehind="Quotation.aspx.cs" Inherits="LIMS.Quotation" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.ckeditor.com/4.25.1/lts/standard/ckeditor.js"></script>
    <!-- JavaScript moved here to the head section -->
    <script type="text/javascript">
        $(document).ready(function () {
            $("#<%= txtTestItem.ClientID %>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: '<%=ResolveUrl("~/Quotation.aspx/GetProducts") %>',
                        data: JSON.stringify({ prefix: request.term }),
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            response($.map(data.d, function (item) {
                                return {
                                    label: item.SubGroupName,
                                    value: item.SubGroupName,
                                    productGroupId: item.ProductGroupID,
                                    groupName: item.GroupName
                                };
                            }));
                        },
                        error: function (response) {
                            alert(response.responseText);
                        },
                        failure: function (response) {
                            alert(response.responseText);
                        }
                    });
                },
                minLength: 1,
                select: function (event, ui) {
                    $("#<%= hdnProductGroupID.ClientID %>").val(ui.item.productGroupId);
                }
            });
        });
    </script>
    <style>
        .form-control {
            width: 100%; /* Ensures it takes full width of its parent */
        }

        #ddlVAT {
            width: 70%; /* Adjust this value as needed for your design */
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
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <asp:Label ID="lblCustomerMasterId" runat="server" Text=""></asp:Label>

    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">

                    <h2>NEW QUOTATION</h2>
                </div>
                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <div class="row clearfix">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Customer:</label>
                                            <asp:DropDownList ID="ddlCustomer" runat="server" CssClass="form-control"
                                                AutoPostBack="true" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged">
                                                <asp:ListItem Text="--Select Company--" Value="" />
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlCustomer"
                                                ErrorMessage="Please select a company" CssClass="error-message" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Quotation Validity (Days):</label>
                                            <asp:TextBox ID="txtValidity" runat="server" CssClass="form-control" TextMode="Number" min="1" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Quotation Date:</label>
                                            <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" Enabled="false" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row clearfix">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Billing Address:</label>
                                            <asp:TextBox ID="txtBillingAddress" runat="server" CssClass="form-control" TextMode="MultiLine" ReadOnly />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Shipping Address:</label>
                                            <asp:TextBox ID="txtShippingAddress" runat="server" CssClass="form-control" TextMode="MultiLine" ReadOnly />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row clearfix">
                                <div class="col-sm-12">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Sample Name:</label>
                                            <asp:TextBox ID="txtTestItem" runat="server" CssClass="form-control" Width="100%" TextMode="SingleLine" AutoPostBack="true"></asp:TextBox>
                                            <asp:AutoCompleteExtender ServiceMethod="GetProducts" MinimumPrefixLength="1"
                                                CompletionInterval="10" EnableCaching="false" CompletionSetCount="1" TargetControlID="txtTestItem"
                                                ID="AutoCompleteExtender2" runat="server" FirstRowSelected="false">
                                            </asp:AutoCompleteExtender>
                                            <asp:HiddenField ID="hdnProductGroupID" runat="server" />
                                            <asp:HiddenField ID="hdServiceType" runat="server" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-12">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Sample Description:</label>
                                            <asp:TextBox ID="txtSampleDescription" runat="server" CssClass="form-control" Width="100%" TextMode="MultiLine" Rows="5" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Test Items:</label><br />
                                            <textarea id="txtTestItemsData" runat="server" rows="10" style="width: 400px"></textarea>


                                            <script>
                                                CKEDITOR.replace('<%= txtTestItemsData.ClientID %>', {
                                                    height: 400,
                                                    width: 400,
                                                    removeButtons: 'Image,Flash,Table,SpecialChar',
                                                    allowedContent: true
                                                });
                                            </script>
                                            <br />
                                            <%--<asp:Literal ID="litTestItemsData" runat="server" />--%>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Test Methods:</label><br />
                                            <textarea id="txtTestMethods" runat="server" rows="10" style="width: 400px"></textarea>
                                            <script>
                                                CKEDITOR.replace('<%= txtTestMethods.ClientID %>', {
                                                    height: 400,
                                                    width: 400,
                                                    removeButtons: 'Image,Flash,Table,SpecialChar',
                                                    allowedContent: true
                                                });
                                            </script>
                                            <br />
                                            <%--<asp:Literal ID="litTestMethods" runat="server" />--%>
                                        </div>
                                    </div>
                                </div>



                                <div class="col-sm-12">
                                    <asp:GridView ID="gvQuotationDetails1" runat="server"
                                        CssClass="table table-bordered table-striped table-hover"
                                        AutoGenerateColumns="false" Width="100%">
                                        <Columns>
                                            <%--<asp:BoundField DataField="SampleName" HeaderText="Product Name" />
                                            <asp:BoundField DataField="SizeOfSample" HeaderText="Product Quantity" />--%>
                                            <asp:BoundField DataField="TestParameter" HeaderText="Test Parameter" />
                                            <asp:BoundField DataField="TestSubParameter" HeaderText="Test Sub Parameter" />
                                            <asp:BoundField DataField="TestSubSubParameter" HeaderText="Test Sub Sub Parameter" />
                                        </Columns>
                                        <EmptyDataTemplate>
                                            <div class="alert">No records found.</div>
                                        </EmptyDataTemplate>
                                    </asp:GridView>

                                    <asp:GridView ID="gvQuotationDetails2" runat="server"
                                        CssClass="table table-bordered table-striped table-hover"
                                        AutoGenerateColumns="false" Width="100%">
                                        <Columns>
                                            <%--<asp:BoundField DataField="SampleName" HeaderText="Product Name" />
                                                <asp:BoundField DataField="SizeOfSample" HeaderText="Product Quantity" />--%>
                                            <asp:BoundField DataField="ReportName" HeaderText="Test Parameter" />
                                        </Columns>
                                        <EmptyDataTemplate>
                                            <div class="alert">No records found.</div>
                                        </EmptyDataTemplate>
                                    </asp:GridView>

                                </div>
                            </div>
                            <div class="row clearfix">
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Type:</label>
                                            <asp:DropDownList ID="ddlType" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlType_SelectedIndexChanged" AutoPostBack="true">
                                                <asp:ListItem>-- Select Type --</asp:ListItem>
                                                <asp:ListItem>TESTING</asp:ListItem>
                                                <asp:ListItem>CERTIFICATION / LSPRO</asp:ListItem>
                                                <asp:ListItem>PICK UP SAMPLE</asp:ListItem>
                                                <asp:ListItem>OTHERS</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:HiddenField ID="hdCOA" runat="server" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-1">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Quantity:</label>
                                            <asp:TextBox ID="txtQuantity" runat="server" CssClass="form-control" TextMode="Number"
                                                AutoPostBack="true" OnTextChanged="CalculateTotal" min="1" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:Label ID="lblPrice" runat="server" Text="Price (IDR):" Font-Bold="true"></asp:Label>
                                            <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" TextMode="Number"
                                                AutoPostBack="true" OnTextChanged="CalculateTotal" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:Label ID="lblTotalAmount" runat="server" Text="Total Amount (IDR):" Font-Bold="true"></asp:Label>
                                            <asp:TextBox ID="txtTotalAmount" runat="server" CssClass="form-control" Enabled="false" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-1">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>UOM:</label>
                                            <asp:DropDownList ID="ddlUOM" runat="server" CssClass="form-control" Font-Size="12px" AutoPostBack="true">
                                                <asp:ListItem Text="Nos." Value="1032" />
                                            </asp:DropDownList>
                                            <asp:HiddenField ID="hdnUOMId" runat="server" />

                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-3">  
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Currency:</label>
                                            <asp:DropDownList ID="ddlCurrency" runat="server" CssClass="form-control"
                                                AutoPostBack="true" OnSelectedIndexChanged="ddlCurrency_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-1">
                                    <label>&nbsp;</label>
                                    <asp:Button ID="btnPlus" runat="server" Text="+" CssClass="btn btn-primary" OnClick="btnPlus_Click" />
                                </div>

                            </div>

                            <div class="row clearfix">
                                <div>
                                    <div>
                                        <div>
                                            <div class="row clearfix">
                                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                                    <asp:HiddenField ID="hdDepartmentID" runat="server" />
                                                    <asp:GridView ID="gvQuotationDetails" runat="server"
                                                        CssClass="table table-bordered table-striped table-hover"
                                                        AutoGenerateColumns="false"
                                                        OnRowCommand="gvQuotationDetails_RowCommand"
                                                        OnRowDataBound="gvQuotation_RowDataBound"
                                                        Width="100%">
                                                        <Columns>
                                                            
                                                            <asp:BoundField DataField="ProductName" HeaderText="Product Name" />
                                                            <asp:BoundField DataField="SampleDescription" HeaderText="Sample Description" />
                                                            <asp:TemplateField HeaderText="Test Items">
                                                                <ItemTemplate>
                                                                    <div style="white-space: normal; line-height: 1.5;">
                                                                        <asp:Literal ID="litSampleName" runat="server" Text='<%# Eval("TestItems") %>' />
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Quantity" HeaderText="Product Quantity" />
                                                            <asp:BoundField DataField="UOM" HeaderText="UOM" />
                                                            <asp:BoundField DataField="Price" HeaderText="Product Price" DataFormatString="{0:N2}" />
                                                            <asp:BoundField DataField="TotalAmount" HeaderText="Total Amount" DataFormatString="{0:N2}" />
                                                            <asp:BoundField DataField="COAAccountCode" HeaderText="COA" />
                                                            <asp:ButtonField CommandName="Remove" Text="Remove" ButtonType="Button" ControlStyle-CssClass="btn btn-danger btn-sm" />
                                                            
                                                        </Columns>
                                                        <EmptyDataTemplate>
                                                            <div class="alert">No records found.</div>
                                                        </EmptyDataTemplate>
                                                    </asp:GridView>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>


                            <div class="row clearfix">

                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:Label ID="Label1" runat="server" Text="Sub Total:" Font-Bold="true"></asp:Label>
                                            <asp:TextBox ID="lblSubTotal" runat="server" CssClass="form-control" Enabled="false" />
                                        </div>
                                    </div>
                                </div>

                                <!-- Discount Row -->
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:Label ID="Label2" runat="server" Text="Discount:" Font-Bold="true"></asp:Label>
                                            <asp:TextBox ID="txtDiscount" runat="server" CssClass="form-control" TextMode="Number" Text="0" OnTextChanged="CalculateGrandTotal" AutoPostBack="true" />
                                        </div>
                                    </div>
                                </div>

                                <!-- Total Before Tax Row -->
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:Label ID="Label3" runat="server" Text="Total before Tax:" Font-Bold="true"></asp:Label>
                                            <asp:TextBox ID="lblTotalBeforeTax" runat="server" CssClass="form-control" Style="color: #555;" TextMode="Number" Text="0" Enabled="false" />
                                        </div>
                                    </div>
                                </div>
                                <!-- VAT Row -->
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:Label ID="Label4" runat="server" Text="VAT (%):" Font-Bold="true"></asp:Label>
                                            <asp:DropDownList ID="ddlVAT" runat="server" CssClass="form-control"
                                                AutoPostBack="true" OnSelectedIndexChanged="ddlVAT_SelectedIndexChanged">
                                            </asp:DropDownList>
                                            <asp:HiddenField ID="hdnVATId" runat="server" />
                                        </div>
                                    </div>
                                </div>
                                <!-- VAT Amount Row -->
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>
                                                VAT Amount:<asp:Label ID="lblVATAmount" runat="server" CssClass="form-control" Enabled="false" />
                                            </label>
                                        </div>
                                    </div>
                                </div>
                                <!-- Grand Total Row -->
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>
                                                <strong>Grand Total:<asp:Label ID="lblGrandTotal" runat="server" CssClass="form-control" Enabled="false" />
                                                </strong>
                                            </label>
                                        </div>
                                    </div>
                                </div>


                            </div>

                            <!-- Payment Terms, Currency, Notes, Add Button -->
                            <div class="row clearfix">
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Payment Terms:</label>
                                            <asp:DropDownList ID="ddlPaymentTerms" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="--Select Payment Term--" Value="" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Note:</label>
                                            <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>
                                </div>

                            </div>





                            <!--submit & clear-->
                            <div class="row clearfix">
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <label>&nbsp;</label>
                                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                                    </div>
                                </div>

                                <div class="col-sm-2">
                                    <label>&nbsp;</label>
                                    <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-warning" OnClick="btnClear_Click" />
                                </div>

                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>


    <%--</div>--%>
</asp:Content>





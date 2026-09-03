<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="QuotationEditPage.aspx.cs" Inherits="LIMS.QuotationEditPage1" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $("#<%= txtTestItem.ClientID %>").autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: '<%=ResolveUrl("~/QuotationEditPage.aspx/GetProducts") %>',
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

    <%-- <div runat="server" class="container">--%>
    <asp:Label ID="lblQuotationMasterID" runat="server" Text=""></asp:Label>
    <asp:Label ID="lblRevisionCount" runat="server" Text=""></asp:Label>

    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <h2>EDIT QUOTATION</h2>
                </div>
                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <!-- Customer, Validity, and Date Fields -->
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
                                            <asp:TextBox ID="txtValidity" runat="server" CssClass="form-control" TextMode="Number" />
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

                            <!-- Billing and Shipping Address -->
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

                            <!-- Product Name, Quantity, UOM, Price -->
                            <div class="row clearfix">
                                <div class="col-sm-12">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Sample Name:</label>
                                            <asp:TextBox ID="txtTestItem" runat="server" CssClass="form-control" Width="100%" TextMode="SingleLine" Rows="15" AutoPostBack="true"></asp:TextBox>
                                            <asp:AutoCompleteExtender ServiceMethod="GetProducts" MinimumPrefixLength="1"
                                                CompletionInterval="10" EnableCaching="false" CompletionSetCount="1" TargetControlID="txtTestItem"
                                                ID="AutoCompleteExtender2" runat="server" FirstRowSelected="false">
                                            </asp:AutoCompleteExtender>
                                            <asp:HiddenField ID="hdnProductGroupID" runat="server" />
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

                                <div class="col-sm-12">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Test Items:</label>
                                            <asp:TextBox ID="txtTestItemsData" runat="server" CssClass="form-control" Width="100%" TextMode="MultiLine" Rows="5" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>



                            <!-- Quantity, UOM, Price, VAT, Discount, Total Amount -->
                            <div class="row clearfix">

                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>UOM:</label>
                                            <asp:DropDownList ID="ddlUOM" runat="server" CssClass="form-control" AutoPostBack="true">
                                                <asp:ListItem Text="--Select UOM--" Value="" />
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
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Quantity:</label>
                                            <asp:TextBox ID="txtQuantity" runat="server" CssClass="form-control" TextMode="Number"
                                                AutoPostBack="true" OnTextChanged="CalculateTotal" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-1">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:Label ID="lblPrice" runat="server" Text="Price (IDR):"></asp:Label>
                                            <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" TextMode="Number"
                                                AutoPostBack="true" OnTextChanged="CalculateTotal" />
                                        </div>
                                    </div>
                                </div>



                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:Label ID="lblTotalAmount" runat="server" Text="Total Amount (IDR):"></asp:Label>
                                            <asp:TextBox ID="txtTotalAmount" runat="server" CssClass="form-control" Enabled="false" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-2">
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

                                                    <asp:GridView ID="gvQuotationDetails" runat="server"
                                                        CssClass="table table-bordered table-striped table-hover"
                                                        AutoGenerateColumns="false"
                                                        OnRowCommand="gvQuotationDetails_RowCommand"
                                                        Width="100%"
                                                        DataKeyNames="QuotationDetailId"
                                                        OnRowDataBound="gvQuotationDetails_RowDataBound">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Sample Name">
                                                                <ItemTemplate>
                                                                    <div style="white-space: normal; line-height: 1.5;">
                                                                    <asp:Literal ID="litSampleName" runat="server" Text='<%# FormatHtmlAsText(Eval("TestItems")) %>' />
                                                                        </div>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="SampleDescription" HeaderText="Sample Description" />
                                                            <asp:BoundField DataField="TestItemDetails" HeaderText="Item Description" />
                                                            <asp:BoundField DataField="Quantity" HeaderText="Product Quantity" />
                                                            <asp:BoundField DataField="UOMName" HeaderText="UOM" />
                                                            <asp:BoundField DataField="Price" HeaderText="Product Price" DataFormatString="{0:N2}" />
                                                            <asp:BoundField DataField="TotalAmount" HeaderText="Total Amount" DataFormatString="{0:N2}" />
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:Button ID="btnRemove" runat="server"
                                                                        Text="Remove"
                                                                        CommandName="Remove"
                                                                        CommandArgument='<%# Container.DataItemIndex %>'
                                                                        CssClass="btn btn-danger btn-sm" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <FooterStyle HorizontalAlign="Right" />
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
                                            <asp:Label ID="Label1" runat="server" Text="Sub Total:"></asp:Label>
                                            <asp:TextBox ID="lblSubTotal" runat="server" CssClass="form-control" Enabled="false" />
                                        </div>
                                    </div>
                                </div>

                                <!-- Discount Row -->
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:Label ID="Label2" runat="server" Text="Discount:"></asp:Label>
                                            <asp:TextBox ID="txtDiscount" runat="server" CssClass="form-control" TextMode="Number" Text="0" OnTextChanged="CalculateGrandTotal" AutoPostBack="true" />
                                        </div>
                                    </div>
                                </div>

                                <!-- Total Before Tax Row -->
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:Label ID="Label3" runat="server" Text="Total before Tax:"></asp:Label>
                                            <asp:TextBox ID="lblTotalBeforeTax" runat="server" CssClass="form-control" Enabled="false" />
                                        </div>
                                    </div>
                                </div>
                                <!-- VAT Row -->
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:Label ID="Label4" runat="server" Text="VAT (%):"></asp:Label>
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
                                            <asp:DropDownList ID="ddlPaymentTerms" runat="server" CssClass="form-control" AutoPostBack="true">
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
                                        <asp:Button ID="btnSubmit" runat="server" Text="Update" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
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

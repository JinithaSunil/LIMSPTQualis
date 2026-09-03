<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/HemetekLIMSNoMenu.Master" CodeBehind="QuotationCalib.aspx.cs" Inherits="LIMS.QuotationCalib" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- JavaScript moved here to the head section -->

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
                            <div class="row">
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Instrument Name:</label>
                                            <asp:DropDownList ID="ddlInstruments" runat="server" CssClass="form-control"></asp:DropDownList>
                                            <asp:TextBox ID="txtTestItem" runat="server" CssClass="form-control" Width="100%" Visible="false"></asp:TextBox>
                                            <asp:HiddenField ID="hdnProductGroupID" runat="server" />
                                            <asp:HiddenField ID="hdInstruments" runat="server" />
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Capacity:</label>
                                            <asp:TextBox ID="txtSampleDescription" runat="server" CssClass="form-control" Width="100%" AutoPostBack="true"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-4" style="visibility: hidden">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Test Items:</label>
                                            <asp:TextBox ID="txtTestItemsData" runat="server" CssClass="form-control" Width="100%"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
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
                            <div class="row">

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

                                <div class="col-sm-2">
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
                                            <asp:TextBox ID="lblTotalBeforeTax" runat="server" CssClass="form-control" Style="color: #555;" TextMode="Number" Text="0" Enabled="false" />
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
                                                        OnRowDataBound="gvQuotation_RowDataBound"
                                                        Width="100%">
                                                        <Columns>
                                                            <asp:BoundField DataField="ProductName" HeaderText="Nama Alat" />
                                                            <asp:BoundField DataField="SampleDescription" HeaderText="Kapasitas" />
                                                             <asp:BoundField DataField="Price" HeaderText="Harga Satuan" DataFormatString="{0:N2}" />
                                                            <asp:BoundField DataField="Quantity" HeaderText="Kuantitas" />                                                           
                                                            <asp:BoundField DataField="TotalAmount" HeaderText="Harga Kalibrasi" DataFormatString="{0:N2}" />
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




<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="SalesEnquiry.aspx.cs" Inherits="LIMS.SalesEnquiry" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
            width: 100%;
        }

        #ddlVAT {
            width: 70%;
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
   <%-- <form id="form1" runat="server">--%>
   
    <div runat="server" class="container">
         <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

        <div class="row clearfix">
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <div class="card">
                    <hr class="violet">
                    <div class="header">
                        <h2>SALES ENQUIRY</h2>
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
                                            </div>
                                        </div>
                                    </div>


                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <label>FollowUp Date</label>
                                                <asp:TextBox ID="txtfollowDate" runat="server" CssClass="form-control" TextMode="Date" />
                                            </div>
                                        </div>
                                    </div>


                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <label>Today's Date</label>
                                                <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" Enabled="false" />
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="row clearfix">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <label>Customer Name</label>
                                                <asp:TextBox ID="txtCustomerName" runat="server" CssClass="form-control" Enabled="false" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <label>Mobile Number</label>
                                                <asp:TextBox ID="txtMobileNumber" runat="server" CssClass="form-control" Enabled="false" />
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <label>Email</label>
                                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" Enabled="false" />
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


                                <div class="row clearfix">
                                    <div class="col-sm-12">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <label>Product Name:</label>
                                                <asp:TextBox ID="txtTestItem" runat="server" CssClass="form-control" Width="100%" TextMode="SingleLine" Rows="15" AutoPostBack="true"></asp:TextBox>
                                                <asp:AutoCompleteExtender ServiceMethod="GetProducts" MinimumPrefixLength="1"
                                                    CompletionInterval="10" EnableCaching="false" CompletionSetCount="1" TargetControlID="txtTestItem"
                                                    ID="AutoCompleteExtender2" runat="server" FirstRowSelected="false">
                                                </asp:AutoCompleteExtender>
                                                <asp:HiddenField ID="hdnProductGroupID" runat="server" />
                                            </div>
                                        </div>
                                    </div>
                                </div>


                                <div class="row clearfix">
                                    <div class="col-sm-12">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <label>Remarks:</label>
                                                <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control" Width="100%" TextMode="MultiLine" Rows="5"></asp:TextBox>
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
                                        <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-warning" />
                                    </div>

                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>


    </div>

<%--</form>--%>
</asp:Content>

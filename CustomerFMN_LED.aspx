<%@ Page Title="" Language="C#" MasterPageFile="~/CRF.Master" AutoEventWireup="true" CodeBehind="CustomerFMN_LED.aspx.cs" Inherits="LIMS.CustomerFMN_LED" %>

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

        .rbl1 input[type="radio"] {
            margin-left: 0px;
            margin-right: 120px;
        }
    </style>
    <style type="text/css">
        .table th {
            text-align: center !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="bg-container-contact100" style="background-image: url(assets/img/bg-01.jpg);">
        <div class="contact100-header flex-sb-m"></div>
    </div>
    <div>
        <div>
            <div class="container-contact100">
                <div style="background-color: white">
                    <form runat="server" class="contact100-form validate-form" data-dpmaxz-eid="1">
                        <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <fieldset>
                                    <div id="divCRF" runat="server" class="row clearfix">
                                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                            <div class="card">
                                                <div class="header" style="text-align: center">
                                                    <table>
                                                        <tr>
                                                            <td style="width: 10%">
                                                                <img src="assets/img/logo.png" height='80px' alt="PT Qualis" /></td>
                                                            <td style="text-align: center; width: 90%">
                                                                <h3>TEST APPLICATION FORM</h3>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <div class="body">
                                                    <br />
                                                    <div class="row clearfix">
                                                        <div class="col-sm-6">
                                                            <div class="form-group">
                                                                <div class="form-line disabled">
                                                                    <asp:Label ID="lblInquiry" runat="server" Font-Bold="true"></asp:Label>
                                                                    :
                                                                    <asp:TextBox ID="txtDateOfRequest" class="form-control" AutoComplete="off" Font-Bold="true" runat="server" Enabled="false"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>

                                                    </div>
                                                    <div class="rectangle">
                                                        <div class="row clearfix">
                                                            <div class="col-sm-12" style="text-align: center">
                                                                <h5><b>Applicant Details</b></h5>
                                                            </div>
                                                        </div>

                                                        <br />

                                                        <div class="row clearfix">
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div class="form-line">Company Name:                                                                     </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-10">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtCustomer" class="form-control disabled" AutoComplete="off" placeholder="CustomerName" runat="server" disabled></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div class="form-line">Address Line-1:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtAddressLine1" class="form-control" AutoComplete="off" placeholder="Address" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div class="form-line">Address Line-2:                                                                     </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtAddressLine2" class="form-control" AutoComplete="off" placeholder="Address" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div class="form-line">Pincode:                                                                     </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtPincode" class="form-control" AutoComplete="off" placeholder="Pin Code" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-1">
                                                                <div class="form-group">
                                                                    <div class="form-line">Country:                                                                     </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-3">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtCountry" class="form-control" AutoComplete="off" placeholder="Country" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-1">
                                                                <div class="form-group">
                                                                    <div class="form-line">VAT:                                                                     </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-3">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtVAT" class="form-control" AutoComplete="off" placeholder="VAT" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-6">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtShippingAgency" class="form-control" Visible="false" AutoComplete="off" placeholder="Shipping Agency" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-6">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtAirwayBillNo" class="form-control" Visible="false" AutoComplete="off" placeholder="AirWay Bill Number" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-12" style="text-align: center">
                                                                <h5><b>Contact Person</b></h5>
                                                                <br />
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-1">
                                                                <div class="form-group">
                                                                    <div class="form-line">Name:                                                                     </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-5">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtContactPerson" runat="server" CssClass="form-control" MaxLength="50" Visible="false"></asp:TextBox>
                                                                        <asp:DropDownList ID="ddlContactPerson" runat="server" AutoPostBack="true" Visible="true" class="form-control" OnSelectedIndexChanged="ddlContactPerson_SelectedIndexChanged"></asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-1">
                                                                <div class="form-group">
                                                                    <div class="form-line">Email:                                                                     </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-5">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtContactEmail" class="form-control" AutoComplete="off" placeholder="Email" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-1">
                                                                <div class="form-group">
                                                                    <div class="form-line">Mobile:                                                                     </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-5">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtMobile" class="form-control" AutoComplete="off" MaxLength="20" AutoPostBack="true" placeholder="Mobile" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-1">
                                                                <div class="form-group">
                                                                    <div class="form-line">Fax:                                                                     </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-5">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtFax" class="form-control" AutoComplete="off" MaxLength="20" AutoPostBack="true" placeholder="Fax" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                    </div>
                                                    <br />
                                                    <div class="rectangle">
                                                        <div class="row clearfix">
                                                            <div class="col-sm-12" style="text-align: center">
                                                                <b style="font-size: 14px">Buyer Details</b> &nbsp;
                                                            <asp:CheckBox ID="chkSameAsInvoice" runat="server" OnCheckedChanged="chkSameAsInvoice_CheckedChanged" AutoPostBack="true" />
                                                                Same as Applicant Details<br />
                                                                <br />
                                                            </div>
                                                        </div>
                                                        <br />
                                                        <div class="row clearfix">
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div class="form-line">Company Name:                                                                     </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-10">
                                                                <div class="form-group">
                                                                    <div class="form-line disabled">
                                                                        <asp:TextBox ID="txtCustomer1" class="form-control" AutoComplete="off" MaxLength="50" placeholder="Company Name on Report" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        Address Line-1:
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtAddressLine11" class="form-control" AutoComplete="off" MaxLength="50" placeholder="Address" runat="server"></asp:TextBox>

                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        Address Line-2: 
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtAddressLine21" class="form-control" AutoComplete="off" MaxLength="50" placeholder="Address" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        Pincode: 
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtPincode1" class="form-control" AutoComplete="off" MaxLength="10" placeholder="Pin Code" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        Country:      
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtCountry1" class="form-control" AutoComplete="off" MaxLength="10" placeholder="Country" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="rectangle">
                                                <div class="row clearfix">
                                                    <div class="col-md-12" style="text-align: center">
                                                        <h5><b>Additional Information</b></h5>
                                                    </div>
                                                </div>

                                                <br />
                                                <div class="row clearfix">
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Service Required:
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                               <asp:RadioButtonList ID="radServiceRequired" runat="server" RepeatDirection="Horizontal" CssClass="rbl1"></asp:RadioButtonList>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Service Delivered By: 
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="servicerequiredby" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row clearfix">
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Sample Return: 
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:RadioButtonList ID="radSampleReturn" runat="server" RepeatDirection="Horizontal" CssClass="rbl1">
                                                                    <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                                                    <asp:ListItem Value="NO">NO</asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Report Recipient: 
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:RadioButtonList ID="radReportRecipient" runat="server" RepeatDirection="Horizontal" CssClass="rbl1">
                                                                    <asp:ListItem Value="Applicant">Applicant</asp:ListItem>
                                                                    <asp:ListItem Value="Payer">Payer</asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row clearfix">
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Invoice Recipient: 
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-10">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:RadioButtonList ID="radInvoiceRecipient" runat="server" RepeatDirection="Horizontal" CssClass="rbl1" OnSelectedIndexChanged="radInvoiceRecipient_SelectedIndexChanged" AutoPostBack="true">
                                                                    <asp:ListItem Value="Applicant">Applicant</asp:ListItem>
                                                                    <asp:ListItem Value="Other">If other, provide the information:</asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row clearfix">
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">Payer's Name:                                                                     </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtPayerName" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">Contact Person:                                                                     </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtPayerContactPerson" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                </div>
                                                <div class="row clearfix">
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">Email Address:                                                                     </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtPayerEmail" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">Contact No.:                                                                    </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtPayerContactNo" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                </div>




                                                <div class="row clearfix">

                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Product Type /<br />
                                                                End Use:
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtProductType" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Item /
                                                                <br />
                                                                Style No.:
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtItemStyleNo" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row clearfix">
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                PO/Order No.
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtPO" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Supplier Name
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtSupplierName" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                </div>

                                                <div class="row clearfix">
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Article Number
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtArticleNo" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Letter No.
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtLotNo" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>


                                                </div>

                                                <%--<div class="row clearfix">
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Material/fiber type
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtMaterialFiberType" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Brand Product
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtBrandProduct" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>--%>

                                                <div class="row clearfix">
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Factory Name
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtFactoryName" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Factory Address
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtFactoryAddress" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row clearfix">

                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Country Of Origin:
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtOriginCountry" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Country Of Destination:
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtDesctinationCountry" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                            </div>
                                            <br />
                                            <div class="rectangle">
                                                <div class="row clearfix">
                                                    <div class="col-md-12" style="text-align: center">
                                                        <h5><b>Test Object</b></h5>
                                                    </div>
                                                </div>

                                                <br />

                                                <div class="row clearfix">
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Product name: 
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtProductName" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Product Brand: 
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtbrand" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                </div>


                                                <div class="row clearfix">
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">Product Color:                                                                     </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtProductColor" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Product specification:
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtProductSpecification" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row clearfix">
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">Product Quantity:                                                                     </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtQuantityofsample" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Product Serial No: 
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtProductSerialNo" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row clearfix">
                                                </div>


                                            </div>

                                            <br />
                                            <div class="rectangle">
                                                <div class="card">
                                                    <h5 style="text-align: center"><b>REQUISITION FOR TESTING (Test Required)</b></h5>
                                                    <br />
                                                </div>
                                                <div class="body">


                                                    <br />
                                                    <asp:GridView ID="gvCRF" runat="server" AutoGenerateColumns="False" class="table table-bordered table-striped table-hover js-basic-example dataTable" BorderWidth="1px" CellPadding="4" Width="100%" OnRowCommand="gvCRF_RowCommand">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="100">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20px"></ItemStyle>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Test Name" ItemStyle-Width="30%">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblReportID" runat="server" Text='<%#Eval("ReportID") %>' Visible="false"></asp:Label>
                                                                    <asp:CheckBox ID="chkTest" runat="server" Text='<%#Eval("ReportName") %>' Checked="true" OnCheckedChanged="chkTest_CheckedChanged" AutoPostBack="false" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Test Method" ItemStyle-Width="35%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtMethod" runat="server" Width="300"></asp:TextBox>
                                                                    <asp:Label ID="lblMethod" runat="server" Text='<%#Eval("TestMethodID") %>' Visible="false"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Remarks" ItemStyle-Width="35%">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtRemarks" runat="server" Width="300"></asp:TextBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:ButtonField ButtonType="Image" ImageUrl="~/assets/img/del.ico" ControlStyle-Width="15px" CommandName="del" />
                                                        </Columns>
                                                        <FooterStyle BackColor="White" ForeColor="#333333" />
                                                        <PagerStyle BackColor="#b34788"
                                                            ForeColor="White" HorizontalAlign="Center" />
                                                        <RowStyle BackColor="White" ForeColor="#333333"
                                                            Height="50px" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        <SelectedRowStyle BackColor="#339966"
                                                            Font-Bold="True" ForeColor="White" />
                                                        <SortedAscendingCellStyle BackColor="#F7F7F7" />
                                                        <SortedAscendingHeaderStyle BackColor="#487575" />
                                                        <SortedDescendingCellStyle BackColor="#E5E5E5" />
                                                        <SortedDescendingHeaderStyle BackColor="#275353" />
                                                    </asp:GridView>

                                                    <asp:GridView ID="gvTests" runat="server" DataKeyNames="ReportID" OnRowDataBound="gvTests_RowDataBound" OnRowCommand="gvTests_RowCommand" AutoGenerateColumns="false" class="table table-bordered table-striped table-hover js-basic-example dataTable">
                                                        <HeaderStyle HorizontalAlign="Center" />
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
                                                                <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Test Name" ItemStyle-Width="25%">
                                                                <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblReportID" runat="server" Text='<%#Eval("ReportID") %>' Visible="false"></asp:Label>
                                                                    <asp:CheckBox ID="chkTest" runat="server" Text='<%#Eval("ReportName") %>' OnCheckedChanged="chkTest_CheckedChanged" AutoPostBack="false" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Standard" ItemStyle-Width="25%" ItemStyle-HorizontalAlign="Center">
                                                                <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtStandardName" runat="server" Text='<%#Eval("StandardName") %>' Width="250" Style="text-align: center;"></asp:TextBox>
                                                                    <asp:Label ID="lblStandardID" runat="server" Text='<%#Eval("StandardID") %>' Visible="false"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Test Method" ItemStyle-Width="30%" ItemStyle-HorizontalAlign="Center">
                                                                <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtMethod" runat="server" Text='<%#Eval("TestMethodID") %>' Width="250" Style="text-align: center;"></asp:TextBox>
                                                                    <asp:Label ID="lblMethod" runat="server" Text='<%#Eval("TestMethodID") %>' Visible="false"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Part" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                                                <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtPart" runat="server" Text='<%#Eval("Part") %>' Style="text-align: center;"></asp:TextBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Requirement" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                                                                <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtRequirement" runat="server" Text='<%#Eval("Requirement") %>' Style="text-align: center;"></asp:TextBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Remarks" ItemStyle-Width="15%" ItemStyle-HorizontalAlign="Center">
                                                                <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtRemarks" runat="server" Text='<%#Eval("Remarks") %>' Style="text-align: center;"></asp:TextBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                        </Columns>
                                                    </asp:GridView>

                                                </div>
                                                <div class="row clearfix">
                                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                        <ContentTemplate>
                                                            <fieldset>
                                                                <table style="width: 90%">
                                                                    <tr>
                                                                        <td colspan="2" style="text-align: center; display: block; margin: auto; padding-left: 45%">
                                                                            <asp:Button ID="btnSubmit" runat="server" class="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </fieldset>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </div>
                                            </div>
                                        </div>
                                    </div>


                                    </div>
                                    <div class="row clearfix">
                                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                            <div id="divFail" runat="server" visible="false" class="card" style="text-align: center">
                                                <br />
                                                <br />
                                                <br />
                                                <h4>REQUEST FORM RECEIVED.<br />
                                                    <br />
                                                    For any changes contact Admin.</h4>
                                                <div style="margin-bottom: 20%"></div>
                                            </div>
                                        </div>
                                    </div>
                                    </div>
                                </fieldset>
                            </ContentTemplate>
                            <Triggers>
                                <%--<asp:PostBackTrigger ControlID="btnUploadImage" />--%>
                                <asp:PostBackTrigger ControlID="btnSubmit" />
                            </Triggers>
                        </asp:UpdatePanel>

                        <asp:HiddenField ID="hdInquiryId" runat="server" />
                        <asp:HiddenField ID="hdCustomerMasterID" runat="server" />
                    </form>
                </div>
            </div>
        </div>
    </div>
    <script src="js/jquery-3.2.1.min.js"></script>
    <script src="js/animsition.js"></script>
    <script src="js/popper.js"></script>
    <script src="js/bootstrap.min.js"></script>
    <script src="js/select2.min.js"></script>
    <script src="js/moment.min.js"></script>
    <script src="js/daterangepicker.js"></script>
    <script src="js/countdowntime.js"></script>
    <script src="js/main.js"></script>
    <script async="" src="LIMS/js/js"></script>
</asp:Content>


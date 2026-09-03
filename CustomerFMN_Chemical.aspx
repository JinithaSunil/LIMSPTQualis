<%@ Page Title="" Language="C#" MasterPageFile="~/CRF.Master" AutoEventWireup="true" CodeBehind="CustomerFMN_Chemical.aspx.cs" Inherits="LIMS.CustomerFMN_Chemical" %>

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
                                                                        <asp:TextBox ID="txtAddressLine1" class="form-control" AutoComplete="off" disabled placeholder="Address" runat="server"></asp:TextBox>
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
                                                                        <asp:TextBox ID="txtAddressLine2" class="form-control" AutoComplete="off" disabled placeholder="Address" runat="server"></asp:TextBox>
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
                                                                        <asp:TextBox ID="txtPincode" class="form-control" AutoComplete="off" disabled placeholder="Pin Code" runat="server"></asp:TextBox>
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
                                                                        <asp:TextBox ID="txtCountry" class="form-control" AutoComplete="off" disabled placeholder="Country" runat="server"></asp:TextBox>
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
                                                                        <asp:TextBox ID="txtVAT" class="form-control" AutoComplete="off" disabled placeholder="VAT" runat="server"></asp:TextBox>
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
                                                            <div class="form-line">Contact No.                                                                     </div>
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
                                                    <div class="col-md-12" style="text-align: center">
                                                        <h5><b>Other Details</b></h5>
                                                    </div>
                                                </div>
                                                <br />
                                                <div class="row clearfix">
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Product Type /<br />
                                                                End Use
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
                                                                Style No. 
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
                                                                Lot No.
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

                                                <div class="row clearfix">
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
                                                </div>

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
                                                    <%--<div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Buyer’s Name / Division
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtBuyersNameDivision" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>--%>
                                                </div>





                                                <div class="row clearfix">
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                Country Of Origin
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
                                                                Country Of Destination
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
                                                <div class="card">
                                                    <h5 style="text-align: center"><b>REQUISITION FOR TESTING (Sample Details & Test Required)</b></h5>
                                                    <br />
                                                </div>
                                                <div class="body">
                                                    <%--  <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                        <ContentTemplate>
                                                            <fieldset>--%>
                                                    <b>Sample Name:</b>
                                                    <asp:Label ID="lblSampleName" runat="server"></asp:Label><br />
                                                    <br />
                                                    <b>Batch#: </b>
                                                    <asp:Label ID="lblBatchNo" runat="server"></asp:Label><br />
                                                    <br />
                                                    <b>Sample Qty#: </b>
                                                    <asp:Label ID="lblSampleQty" runat="server"></asp:Label><br />
                                                    <br />
                                                    <b>Requirement: </b>
                                                    <asp:Label ID="lblRequirement" runat="server"></asp:Label><br />
                                                    <br />
                                                    <asp:GridView ID="gvCRF" runat="server" AutoGenerateColumns="False" class="table table-bordered table-striped table-hover js-basic-example dataTable" BorderWidth="1px" CellPadding="4" Width="100%" OnRowCommand="gvCRF_RowCommand">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="100">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20px"></ItemStyle>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Test Request">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblTestParameter" runat="server" Text='<%#Eval("TestParameter") %>'></asp:Label>
                                                                    <asp:Label ID="lblTestParameterID" runat="server" Visible="false" Text='<%#Eval("TestParameterID") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbl" runat="server" Text='<%# String.IsNullOrEmpty(Eval("TestSubSubParameter").ToString()) ? Eval("TestSubParameter") : Eval("TestSubParameter") + "-" + Eval("TestSubSubParameter") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="TestMethod" HeaderText="Test Method" />
                                                            <asp:BoundField DataField="UOMName" HeaderText="UOMName" />

                                                            <asp:BoundField DataField="CASNo" HeaderText="CAS No." />
                                                            <asp:BoundField DataField="MDL" HeaderText="M.D.L" />                                                            
                                                            <asp:BoundField DataField="Specification" HeaderText="Specification If Any" />
                                                            <%--<asp:BoundField DataField="Remarks" HeaderText="Remarks" />--%>
                                                            <asp:ButtonField ButtonType="Image" ImageUrl="~/assets/img/del.ico" ControlStyle-Width="15px" CommandName="del" />
                                                        </Columns>
                                                        <FooterStyle BackColor="White" ForeColor="#333333" />
                                                        <PagerStyle BackColor="#b34788" ForeColor="White" HorizontalAlign="Center" />
                                                        <RowStyle BackColor="White" ForeColor="#333333" Height="50px" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        <SelectedRowStyle BackColor="#339966" Font-Bold="True" ForeColor="White" />
                                                        <SortedAscendingCellStyle BackColor="#F7F7F7" />
                                                        <SortedAscendingHeaderStyle BackColor="#487575" />
                                                        <SortedDescendingCellStyle BackColor="#E5E5E5" />
                                                        <SortedDescendingHeaderStyle BackColor="#275353" />
                                                    </asp:GridView>

                                                    <%--</fieldset>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>--%>
                                                    <br />
                                                    <table style="width: 100%">
                                                        <tr>
                                                            <td style="width: 30%; text-align: left">
                                                                <div class="col-sm-12">
                                                                    <div class="form-group">
                                                                        <div class="form-line" style="text-align: center">
                                                                            Sample Name<br />
                                                                            <asp:TextBox ID="txtSampleName" class="form-control" AutoComplete="off" placeholder="Sample Name" runat="server"></asp:TextBox>
                                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtSampleName" ErrorMessage="Required!" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </td>

                                                            <td style="width: 30%; text-align: left">
                                                                <div class="col-sm-12">
                                                                    <div class="form-group">
                                                                        <div class="form-line" style="text-align: center">
                                                                            Batch#<br />
                                                                            <asp:TextBox ID="txtBatchNo" class="form-control" AutoComplete="off" placeholder="Batch No/ID" runat="server"></asp:TextBox>
                                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtBatchNo" ErrorMessage="Required!" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                            <td style="width: 30%; text-align: left">
                                                                <div class="col-sm-12">
                                                                    <div class="form-group">
                                                                        <div class="form-line" style="text-align: center">
                                                                            Sample Size<br />
                                                                            <asp:TextBox ID="txtSizeofSample" runat="server" class="form-control" AutoComplete="off" placeholder="Size / Nos of sample"></asp:TextBox>
                                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtSizeofSample" ErrorMessage="Required!" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2" style="width: 30%; text-align: left">
                                                                <div class="col-sm-12">
                                                                    <div class="form-group">
                                                                        <div class="form-line" style="text-align: center">
                                                                            Requirement<br />
                                                                            <asp:DropDownList ID="ddlRequirement" runat="server" CssClass="form-control"></asp:DropDownList>
                                                                            <asp:HiddenField ID="hdRequirement" runat="server" />
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: 30%; text-align: left">
                                                                <div class="col-sm-12">
                                                                    <div class="form-group">
                                                                        <div class="form-line" style="text-align: center">
                                                                            Test Parameter<br />
                                                                            <asp:TextBox ID="txtTestParameter" class="form-control" placeholder="Test Parameter" runat="server" OnTextChanged="txtTestParameter_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                            <asp:Label ID="lblTestParameterID" runat="server" Visible="false"></asp:Label>
                                                                            <asp:AutoCompleteExtender ServiceMethod="GetTestParameters" MinimumPrefixLength="1"
                                                                                CompletionInterval="10" EnableCaching="false" CompletionSetCount="1" TargetControlID="txtTestParameter"
                                                                                ID="AutoCompleteExtender2" runat="server" FirstRowSelected="false">
                                                                            </asp:AutoCompleteExtender>
                                                                            <%--<asp:DropDownList ID="ddlTestParameter" runat="server" CssClass="form-control"></asp:DropDownList>--%>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                            <td style="width: 30%; text-align: left">
                                                                <div class="col-sm-12">
                                                                    <div class="form-group">
                                                                        <div class="form-line" style="text-align: center">
                                                                            Sub Parameter<br />
                                                                            <asp:DropDownList ID="ddlTestSubParameter" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlTestSubParameter_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                            <asp:Label ID="lblTestSubParameterID" runat="server" Visible="false"></asp:Label>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                            <td style="width: 30%; text-align: left">
                                                                <div class="col-sm-12">
                                                                    <div class="form-group">
                                                                        <div class="form-line" style="text-align: center">
                                                                            Sub SubParameter<br />
                                                                            <asp:DropDownList ID="ddlTestSubSubParameter" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlTestSubSubParameter_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                            <asp:Label ID="lblTestSubSubParameter" runat="server" Visible="false"></asp:Label>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </td>

                                                        </tr>
                                                        <tr>
                                                            <td style="width: 30%; text-align: left">
                                                                <div class="col-sm-12">
                                                                    <div class="form-group">
                                                                        <div class="form-line" style="text-align: center">
                                                                            Test Method<br />
                                                                            <asp:DropDownList ID="ddlTestMethod" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlTestMethod_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                            <td style="width: 30%; text-align: left">
                                                                <div class="col-sm-12">
                                                                    <div class="form-group">
                                                                        <div class="form-line" style="text-align: center">
                                                                            Unit<br />
                                                                            <asp:DropDownList ID="ddlUOM" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlUOM_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: 30%; text-align: left">
                                                                <div class="col-sm-12">
                                                                    <div class="form-group">
                                                                        <div class="form-line" style="text-align: center">
                                                                            CAS Number<br />
                                                                            <asp:TextBox ID="txtCasNumber" runat="server" CssClass="form-control"></asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                            <td style="width: 30%; text-align: left">
                                                                <div class="col-sm-12">
                                                                    <div class="form-group">
                                                                        <div class="form-line" style="text-align: center">
                                                                            M.D.L<br />
                                                                            <asp:TextBox ID="txtMDL" runat="server" CssClass="form-control"></asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </td>                                                            
                                                        </tr>
                                                        <tr>
                                                            <td style="width: 30%; text-align: left">
                                                                <div class="col-sm-12">
                                                                    <div class="form-group">
                                                                        <div class="form-line">
                                                                            <asp:TextBox ID="txtSpecification" class="form-control" AutoComplete="off" placeholder="Specification If Any" runat="server"></asp:TextBox>

                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                            <td style="width: 30%; text-align: left">
                                                                <div class="col-sm-12">
                                                                    <div class="form-group">
                                                                        <div class="form-line">
                                                                            <asp:TextBox ID="txtRemarks" class="form-control" AutoComplete="off" placeholder="Remarks" runat="server"></asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                            <td style="width: 30%; text-align: center">
                                                                <div class="col-sm-12">
                                                                    <div class="form-group">
                                                                        <asp:Button ID="btnAdd" runat="server" class="btn bg-teal m-t-15 waves-effect" Style="background: #b34788; color: #fff; width: 40px;" Text=" + " OnClick="btnAdd_Click" />
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>




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

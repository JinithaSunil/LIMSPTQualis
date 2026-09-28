<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeBehind="CustomerFMN_IAT_Textile.aspx.cs" Inherits="LIMS.CustomerFMN_IAT_Textile" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body {
            background-color: #e5e5e5;
        }

        .container-contact100 {
            max-width: 970px;
            margin: 30px auto;
            background: #ffffff;
            border: 1px solid #ddd;
            box-shadow: 0 0 6px rgba(0,0,0,0.08);
            padding: 0;
        }

        .card {
            box-shadow: none;
            border: none;
            margin: 0;
        }

            .card > .header {
                padding: 30px 40px 10px 40px;
            }

                .card > .header table {
                    width: 100%;
                }

                .card > .header h3 {
                    font-size: 30px;
                    font-weight: 600;
                    color: #222;
                    margin: 0;
                    letter-spacing: 0.5px;
                }

                .card > .header h5 {
                    font-size: 14px;
                    font-weight: 400;
                    color: #444;
                    margin-top: 4px;
                }

            .card > .body {
                padding: 10px 40px 40px 40px;
            }

        .rectangle {
            width: 100%;
            border: 1px solid #000;
            border-radius: 0;
            padding: 20px 25px;
            margin-bottom: 22px;
        }

            .rectangle h5 {
                font-size: 15px;
                font-weight: 700;
                color: #000;
                margin: 0 0 18px 0;
            }

        .form-group {
            margin-bottom: 14px;
        }

        .form-line {
            font-size: 13.5px;
            color: #333;
            margin-bottom: 5px;
        }

        .form-control {
            height: 38px;
            border: 1px solid #b3b3b3;
            border-radius: 2px;
            padding: 6px 12px;
            font-size: 13.5px;
            color: #333;
            background-color: #fff;
            box-shadow: none;
        }

            .form-control[disabled],
            .form-control.disabled,
            .form-control:disabled {
                background-color: #ededed;
                color: #555;
                border: 1px solid #ccc;
            }

            .form-control::-webkit-input-placeholder {
                color: #999;
            }

            .form-control::placeholder {
                color: #999;
            }

        textarea.form-control {
            height: auto;
        }

        .rbl1 {
            font-size: 13.5px;
        }

            .rbl1 input[type="radio"] {
                margin-left: 0px;
                margin-right: 8px;
            }

            .rbl1 label {
                margin-right: 35px;
                font-weight: normal;
                color: #333;
            }

        input[type="checkbox"] {
            margin-right: 6px;
        }

        .chk-inline label {
            margin-right: 25px;
            font-weight: normal;
        }

        .btn.btn-primary {
            background-color: #337ab7;
            border-color: #2e6da4;
            padding: 8px 30px;
            font-size: 14px;
        }

        .officer-section-note {
            font-size: 12.5px;
            color: #888;
            font-style: italic;
            margin-bottom: 12px;
            text-align: center;
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
                    <div class="contact100-form validate-form">
                        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <fieldset>
                                    <div class="row clearfix">
                                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                            <div class="card">
                                                <div class="header" style="text-align: center">
                                                    <table>
                                                        <tr>
                                                            <td style="width: 10%">
                                                                <img src="assets/img/logo.png" height='80px' alt="PT Qualis" /></td>
                                                            <td style="text-align: center; width: 90%">
                                                                <h3>BOOKING FORM</h3>
                                                                <h5>Inspection Services</h5>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <div class="body">
                                                    <br />

                                                    <asp:HiddenField ID="hdInquiryId" runat="server" />
                                                    <asp:HiddenField ID="hdCustomerMasterID" runat="server" />
                                                    <asp:HiddenField ID="hdPersonID" runat="server" />
                                                    <asp:HiddenField ID="hdIsOfficerMode" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdAdditionalDetailsID" runat="server" />
                                                    <!-- ===================== Header ===================== -->
                                                    <div class="rectangle">
                                                        <div class="row clearfix">
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div>Commodity:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtCommodity" runat="server" CssClass="form-control" AutoComplete="off" placeholder="e.g. Pakaian Jadi"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div>Booking Form No.:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtBookingNo" runat="server" CssClass="form-control" AutoComplete="off"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div>Date:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtRequestDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div>Attn (PT Qualis Contact):</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtAttn" runat="server" CssClass="form-control" AutoComplete="off"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <br />

                                                    <!-- ===================== Applicant Details ===================== -->
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
                                                                    <div>Applicant (Company Name):</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-10">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtApplicantName" runat="server" CssClass="form-control" AutoComplete="off" placeholder="Applicant / Company Name"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div>Address:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-10">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="Address"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-1">
                                                                <div class="form-group">
                                                                    <div>Contact Person:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-5">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:DropDownList ID="ddlContactPerson" runat="server" CssClass="form-control" AutoPostBack="true"
                                                                            OnSelectedIndexChanged="ddlContactPerson_SelectedIndexChanged">
                                                                        </asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-1">
                                                                <div class="form-group">
                                                                    <div>Tel:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-5">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" AutoComplete="off" placeholder="Phone"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-1">
                                                                <div class="form-group">
                                                                    <div>Email:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-5">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" AutoComplete="off" placeholder="Email"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-1">
                                                                <div class="form-group">
                                                                    <div>Fax:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-5">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtFax" runat="server" CssClass="form-control" AutoComplete="off" placeholder="Fax"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-1">
                                                                <div class="form-group">
                                                                    <div>CC To:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-5">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtCC" runat="server" CssClass="form-control" AutoComplete="off" placeholder="CC Email(s)"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-1">
                                                                <div class="form-group">
                                                                    <div>Email:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-5">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtInvoiceEmail" runat="server" CssClass="form-control" AutoComplete="off" placeholder="Invoice Email"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div>Report Sent To:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div>
                                                                        <asp:RadioButtonList ID="radReportRecipient" runat="server" RepeatDirection="Horizontal" CssClass="rbl1">
                                                                            <asp:ListItem Value="Buyer">Buyer</asp:ListItem>
                                                                            <asp:ListItem Value="Applicant" Selected="True">Applicant</asp:ListItem>
                                                                        </asp:RadioButtonList>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div>Invoice To:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div>
                                                                        <asp:RadioButtonList ID="radInvoiceRecipient" runat="server" RepeatDirection="Horizontal" CssClass="rbl1">
                                                                            <asp:ListItem Value="Buyer">Buyer</asp:ListItem>
                                                                            <asp:ListItem Value="Applicant" Selected="True">Applicant</asp:ListItem>
                                                                            <asp:ListItem Value="Supplier">Supplier</asp:ListItem>
                                                                            <asp:ListItem Value="Factory">Factory</asp:ListItem>
                                                                        </asp:RadioButtonList>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <div class="row clearfix">
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div>Other Instructions:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-10">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtOtherInstructions" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="Please specify"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <br />

                                                    <!-- ===================== Buyer Details ===================== -->
                                                    <div class="rectangle">
                                                        <div class="row clearfix">
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div>Buyer Name:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtBuyerName" runat="server" CssClass="form-control" AutoComplete="off"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div>Country:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtBuyerCountry" runat="server" CssClass="form-control" AutoComplete="off"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <br />

                                                    <!-- ===================== Type of Services ===================== -->
                                                    <div class="rectangle">
                                                        <div class="row clearfix">
                                                            <div class="col-sm-12" style="text-align: center">
                                                                <h5><b>Type of Services</b></h5>
                                                            </div>
                                                        </div>
                                                        <br />
                                                        <div class="row clearfix">
                                                            <div class="col-sm-4">
                                                                <asp:CheckBox ID="chkIPI" runat="server" Text="Initial Production Inspection (IPI)" />
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <asp:CheckBox ID="chkDPI" runat="server" Text="During Production Inspection (DPI)" />
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <asp:CheckBox ID="chkFRI" runat="server" Text="Final Random Inspection (FRI)" />
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-4">
                                                                <asp:CheckBox ID="chkLoadingSupervision" runat="server" Text="Loading Supervision (LS)" />
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <asp:CheckBox ID="chkSampleSealing" runat="server" Text="Sample Sealing" />
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <asp:CheckBox ID="chkFabricInspection" runat="server" Text="Fabric Inspection" />
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-6">
                                                                <asp:CheckBox ID="chkReInspection" runat="server" Text="Re-Inspection (Previous Report No.:" />
                                                                <asp:TextBox ID="txtPreviousReportNo" runat="server" CssClass="form-control" Style="display: inline-block; width: 180px;"></asp:TextBox>
                                                                )
                                                           
                                                            </div>
                                                            <div class="col-sm-6">
                                                                <asp:CheckBox ID="chkOtherService" runat="server" Text="Others:" />
                                                                <asp:TextBox ID="txtOtherService" runat="server" CssClass="form-control" Style="display: inline-block; width: 300px;"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <br />

                                                    <!-- ===================== Other Details ===================== -->
                                                    <div class="rectangle">
                                                        <div class="row clearfix">
                                                            <div class="col-sm-12" style="text-align: center">
                                                                <h5><b>Other Details</b></h5>
                                                            </div>
                                                        </div>
                                                        <br />
                                                        <div class="row clearfix">
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div>Product Description:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-10">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtProductDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" AutoComplete="off"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div>Shipment Qty:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtShipmentQty" runat="server" CssClass="form-control" AutoComplete="off"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div>Item / Style No.:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtItemStyleNo" runat="server" CssClass="form-control" AutoComplete="off"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div>% Ready:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtPercentReady" runat="server" CssClass="form-control" AutoComplete="off" placeholder="e.g. 100%"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div>PO Number:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtPONumber" runat="server" CssClass="form-control" AutoComplete="off"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div>Shipping Date:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtShippingDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <br />

                                                    <!-- ===================== Reference Material ===================== -->
                                                    <div class="rectangle">
                                                        <div class="row clearfix">
                                                            <div class="col-sm-12" style="text-align: center">
                                                                <h5><b>Reference Material Provided to Inspection Office</b></h5>
                                                            </div>
                                                        </div>
                                                        <br />
                                                        <div class="row clearfix">
                                                            <div class="col-sm-12">
                                                                <asp:RadioButtonList ID="radReferenceProvided" runat="server" RepeatDirection="Horizontal" CssClass="rbl1" AutoPostBack="true" OnSelectedIndexChanged="radReferenceProvided_SelectedIndexChanged">
                                                                    <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                                                    <asp:ListItem Value="No" Selected="True">No</asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </div>
                                                        </div>
                                                        <asp:Panel ID="pnlReference" runat="server" Visible="false">
                                                            <div class="row clearfix">
                                                                <div class="col-sm-4">
                                                                    <asp:CheckBox ID="chkRefApprovedSamples" runat="server" Text="Approved Samples" />
                                                                </div>
                                                                <div class="col-sm-4">
                                                                    <asp:CheckBox ID="chkRefProductSpec" runat="server" Text="Product Specification / Drawing" />
                                                                </div>
                                                                <div class="col-sm-4">
                                                                    <asp:CheckBox ID="chkRefDigitalImages" runat="server" Text="Digital Images" />
                                                                </div>
                                                            </div>
                                                            <div class="row clearfix">
                                                                <div class="col-sm-4">
                                                                    <asp:CheckBox ID="chkRefSwatchCard" runat="server" Text="Swatch Card" />
                                                                </div>
                                                                <div class="col-sm-4">
                                                                    <asp:CheckBox ID="chkRefPOCopy" runat="server" Text="PO Copy" />
                                                                </div>
                                                                <div class="col-sm-4">
                                                                    <asp:CheckBox ID="chkRefPackingListInvoice" runat="server" Text="Packing List / Invoice Copy" />
                                                                </div>
                                                            </div>
                                                            <div class="row clearfix">
                                                                <div class="col-sm-4">
                                                                    <asp:CheckBox ID="chkRefLCCopy" runat="server" Text="LC Copy" />
                                                                </div>
                                                                <div class="col-sm-4">
                                                                    <asp:CheckBox ID="chkRefClientInstructionSheet" runat="server" Text="Client Instruction Sheet" />
                                                                </div>
                                                                <div class="col-sm-4">
                                                                    <asp:CheckBox ID="chkRefRoadMap" runat="server" Text="Road Map" />
                                                                </div>
                                                            </div>
                                                            <div class="row clearfix">
                                                                <div class="col-sm-4">
                                                                    <asp:CheckBox ID="chkRefCertificate" runat="server" Text="Certificate" />
                                                                </div>
                                                                <div class="col-sm-4">
                                                                    <asp:CheckBox ID="chkRefConstructionDataForm" runat="server" Text="Construction Data Form" />
                                                                </div>
                                                                <div class="col-sm-4">
                                                                    <asp:CheckBox ID="chkRefOther" runat="server" Text="Other:" />
                                                                </div>
                                                            </div>
                                                            <div class="row clearfix">
                                                                <div class="col-sm-12">
                                                                    <asp:TextBox ID="txtOtherReference" runat="server" CssClass="form-control" Style="display: inline-block; width: 400px;" placeholder="Other reference, e.g. Status Produksi"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </asp:Panel>
                                                    </div>

                                                    <br />

                                                    <!-- ===================== Officer-filled section ===================== -->
                                                    <%-- NEW: this whole block is wrapped in pnlOfficerSection. Its child controls are
                                                         force-disabled by the code-behind (SetControlsEnabled) whenever hdIsOfficerMode = "0",
                                                         i.e. on the customer's first pass through this form. Once a matching row exists in
                                                         CustomerCRFSampleDetails for this InquiryID, the code-behind flips the hidden field to
                                                         "1" and re-enables everything in here for the officer to fill in / amend. --%>
                                                    <asp:Panel ID="pnlOfficerSection" runat="server">
                                                    <div class="rectangle">
                                                        <div class="row clearfix">
                                                            <div class="col-sm-12" style="text-align: center">
                                                                <h5><b>Additional Requirement / Amendments</b> <i>(to be filled in by officer)</i></h5>
                                                            </div>
                                                        </div>
                                                        <asp:Label ID="lblOfficerLocked" runat="server" CssClass="officer-section-note"
                                                            Text="This section is locked until the customer's booking form has been submitted." />
                                                        <br />
                                                        <div class="row clearfix">
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div>Inspection Location:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-10">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtInspectionLocation" runat="server" CssClass="form-control" AutoComplete="off"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div>Factory / Supplier Name:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-10">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtFactorySupplierName" runat="server" CssClass="form-control" AutoComplete="off"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div>Address:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-10">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtFactoryAddress" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" AutoComplete="off"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-1">
                                                                <div class="form-group">
                                                                    <div>Contact Person:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-5">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtFactoryContactPerson" runat="server" CssClass="form-control" AutoComplete="off"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-1">
                                                                <div class="form-group">
                                                                    <div>Tel:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-5">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtFactoryPhone" runat="server" CssClass="form-control" AutoComplete="off"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-1">
                                                                <div class="form-group">
                                                                    <div>Fax:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-5">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtFactoryFax" runat="server" CssClass="form-control" AutoComplete="off"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-2">
                                                                <div class="form-group">
                                                                    <div>Requested Inspection Date:</div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtRequestedInspectionDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    </asp:Panel>

                                                </div>

                                                <div class="row clearfix">
                                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                        <ContentTemplate>
                                                            <fieldset>
                                                                <table style="width: 100%">
                                                                    <tr>
                                                                        <td colspan="2" style="text-align: center; display: block; margin: auto;">
                                                                            <asp:Button ID="btnSubmit" runat="server" class="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <div style="text-align: center">
                                                                    <asp:Label ID="lblMessage" runat="server" ForeColor="Red" Font-Bold="true"></asp:Label>
                                                                </div>
                                                            </fieldset>
                                                        </ContentTemplate>
                                                        <Triggers>
                                                            <asp:PostBackTrigger ControlID="btnSubmit" />
                                                        </Triggers>
                                                    </asp:UpdatePanel>
                                                </div>

                                            </div>
                                        </div>
                                    </div>
                                </fieldset>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

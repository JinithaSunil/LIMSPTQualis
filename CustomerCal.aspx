<%@ Page Language="C#" MasterPageFile="~/CRF.Master" AutoEventWireup="true" CodeBehind="CustomerCal.aspx.cs" Inherits="LIMS.CustomerCal" %>

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
            margin-left: 5px;
            margin-right: 15px;
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
                                                                <h3>CALIBRATION REQUEST APPLICATION</h3>
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

                                                </div>
                                            </div>
                                            <br />

                                            <br />
                                            <div class="rectangle">
                                                <div class="card">
                                                    <h5 style="text-align: center"><b>LIST OF CALIBRATION INSTRUMENTS</b></h5>
                                                    <br />
                                                </div>
                                                <div class="body">
                                                    <table>
                                                        <tr style="text-align: center">
                                                            <td><b>Nama Alat</b></td>
                                                            <td><b>Merek</b></td>
                                                            <td><b>Tipe</b></td>
                                                            <td><b>No. Seri</b></td>
                                                            <td><b>Range Kalibrasi</b></td>
                                                            <td><b>Jumlah</b></td>
                                                            <td><b>Cek Kesesuaian*</b></td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:DropDownList ID="ddlInstruments" runat="server" CssClass="form-control"></asp:DropDownList>
                                                                <asp:HiddenField ID="hdInstruments" runat="server" />
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtBrand" class="form-control" AutoComplete="off" runat="server"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtType" class="form-control" AutoComplete="off" runat="server"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtSerialNo" class="form-control" AutoComplete="off" runat="server"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtCalbRange" class="form-control" AutoComplete="off" runat="server"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtQty" class="form-control" AutoComplete="off" runat="server"></asp:TextBox>
                                                            </td>
                                                            <td style="text-align: center">
                                                                <asp:RadioButtonList ID="radCompliance" runat="server" RepeatDirection="Horizontal" CssClass="rbl1">
                                                                    <asp:ListItem>P</asp:ListItem>
                                                                    <asp:ListItem>S</asp:ListItem>
                                                                    <asp:ListItem>M</asp:ListItem>
                                                                    <asp:ListItem>C</asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Button ID="btnAddDetails" runat="server" Text="+" CssClass="btn btn-primary" OnClick="btnAddDetails_Click" /></td>
                                                        </tr>
                                                    </table>

                                                    <br />
                                                    <asp:GridView ID="gvInstruments" runat="server" AutoGenerateColumns="false" class="table table-bordered table-striped table-hover js-basic-example dataTable">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="NO." ItemStyle-Width="100">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                                    <asp:HiddenField ID="hdInstrumentID" runat="server" Value='<%#Eval("InstrumentID") %>' />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="100px"></ItemStyle>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="InstrumentName" runat="server" HeaderText="Nama Alat" />
                                                            <asp:BoundField DataField="Brand" runat="server" HeaderText="Merek" />
                                                            <asp:BoundField DataField="InstrumentType" runat="server" HeaderText="Tipe" />
                                                            <asp:BoundField DataField="SerialNo" runat="server" HeaderText="No. Seri" />
                                                            <asp:BoundField DataField="CalibrationRange" runat="server" HeaderText="Range Kalibrasi" />
                                                            <asp:BoundField DataField="Quantity" runat="server" HeaderText="Jumlah" />
                                                            <asp:BoundField DataField="ComplianceCheck" runat="server" HeaderText="Cek Kesesuaian*(P/S/M/C)" />
                                                        </Columns>
                                                    </asp:GridView>
                                                    <br />

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
                        <asp:Label ID="ErrorMessageLabel" runat="server" EnableViewState="false" />
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

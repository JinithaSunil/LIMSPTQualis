<%@ Page Language="C#" MasterPageFile="~/CRF.Master" AutoEventWireup="true" CodeBehind="CustomerFMN_LSSM.aspx.cs" Inherits="LIMS.CustomerFMN_LSSM" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .wizard-step {
            min-height: 500px;
        }

        .step-indicator {
            display: flex;
            justify-content: space-between;
            margin-bottom: 25px;
        }

        .step-item {
            flex: 1;
            text-align: center;
            position: relative;
        }

            .step-item .circle {
                width: 40px;
                height: 40px;
                border-radius: 50%;
                background: #e9ecef;
                line-height: 40px;
                margin: auto;
                font-weight: bold;
            }

            .step-item.active .circle {
                background: #696cff;
                color: #fff;
            }

            .step-item.completed .circle {
                background: #28c76f;
                color: #fff;
            }

        .step-title {
            margin-top: 8px;
            font-size: 14px;
            font-weight: 600;
        }

        .section-card {
            margin-bottom: 20px;
        }

        .required {
            color: red;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <!-- ===================== Page Header ===================== -->
    <div class="bg-container-contact100" style="background-image: url(assets/img/bg-01.jpg);">
        <div class="contact100-header flex-sb-m"></div>
    </div>
    <div>
        <form id="form" runat="server">
            <div class="container-contact100">
                <div style="background-color: white">
                    <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                            <fieldset>
                                <div id="divCRF" runat="server" class="row clearfix">
                                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                        <div class="card">
                                            <div class="row mb-4">
                                                <div class="col-md-8">
                                                    <h4 class="fw-bold mb-1">
                                                        <i class="bx bx-certification text-primary"></i>
                                                        Certification Application
                                                    </h4>
                                                    <small class="text-muted">PT Qualis ERP - Laboratory System & Standards Management (LSSM)
                                                    </small>
                                                </div>

                                                <div class="col-md-4 text-end">

                                                    <asp:Label ID="lblStatus"
                                                        runat="server"
                                                        CssClass="badge bg-label-warning fs-6"
                                                        Text="Draft"></asp:Label>

                                                </div>
                                            </div>

                                            <!-- ===================== Progress ===================== -->

                                            <div class="card shadow-sm mb-4">

                                                <div class="card-body">

                                                    <div class="d-flex justify-content-between align-items-center mb-2">

                                                        <span class="fw-semibold">Progress

                                                        </span>

                                                        <span>

                                                            <asp:Label ID="lblProgress"
                                                                runat="server"
                                                                Text="10%">
                                                            </asp:Label>

                                                        </span>

                                                    </div>

                                                    <div class="progress" style="height: 8px;">

                                                        <div id="wizardProgress"
                                                            runat="server"
                                                            class="progress-bar progress-bar-striped progress-bar-animated bg-primary"
                                                            role="progressbar"
                                                            style="width: 10%;">
                                                        </div>

                                                    </div>

                                                </div>

                                            </div>

                                            <!-- ===================== Wizard Steps ===================== -->

                                            <div class="card">

                                                <div class="card-body p-2">

                                                    <ul class="nav nav-pills nav-justified"
                                                        id="wizardTabs"
                                                        role="tablist">

                                                        <li class="nav-item">

                                                            <a class="nav-link active"
                                                                id="tabGeneral"
                                                                href="javascript:void(0);">

                                                                <i class="bx bx-building-house d-block fs-4"></i>

                                                                <small>General</small>

                                                            </a>

                                                        </li>

                                                        <li class="nav-item">

                                                            <a class="nav-link"
                                                                id="tabCertification"
                                                                href="javascript:void(0);">

                                                                <i class="bx bx-certification d-block fs-4"></i>

                                                                <small>Certification</small>

                                                            </a>

                                                        </li>

                                                        <li class="nav-item">

                                                            <a class="nav-link"
                                                                id="tabOrganization"
                                                                href="javascript:void(0);">

                                                                <i class="bx bx-group d-block fs-4"></i>

                                                                <small>Organization</small>

                                                            </a>

                                                        </li>

                                                        <li class="nav-item">

                                                            <a class="nav-link"
                                                                id="tabMultiSite"
                                                                href="javascript:void(0);">

                                                                <i class="bx bx-map d-block fs-4"></i>

                                                                <small>Multi Site</small>

                                                            </a>

                                                        </li>

                                                        <li class="nav-item">

                                                            <a class="nav-link"
                                                                id="tabDocuments"
                                                                href="javascript:void(0);">

                                                                <i class="bx bx-folder-open d-block fs-4"></i>

                                                                <small>Documents</small>

                                                            </a>

                                                        </li>

                                                        <li class="nav-item">

                                                            <a class="nav-link"
                                                                id="tabISO45001"
                                                                href="javascript:void(0);">

                                                                <i class="bx bx-shield-quarter d-block fs-4"></i>

                                                                <small>ISO 45001</small>

                                                            </a>

                                                        </li>

                                                        <li class="nav-item">

                                                            <a class="nav-link"
                                                                id="tabISO14001"
                                                                href="javascript:void(0);">

                                                                <i class="bx bx-leaf d-block fs-4"></i>

                                                                <small>ISO 14001</small>

                                                            </a>

                                                        </li>

                                                        <li class="nav-item">

                                                            <a class="nav-link"
                                                                id="tabTransfer"
                                                                href="javascript:void(0);">

                                                                <i class="bx bx-transfer d-block fs-4"></i>

                                                                <small>Transfer</small>

                                                            </a>

                                                        </li>

                                                        <li class="nav-item">

                                                            <a class="nav-link"
                                                                id="tabDeclaration"
                                                                href="javascript:void(0);">

                                                                <i class="bx bx-edit d-block fs-4"></i>

                                                                <small>Declaration</small>

                                                            </a>

                                                        </li>

                                                        <li class="nav-item">

                                                            <a class="nav-link"
                                                                id="tabReview"
                                                                href="javascript:void(0);">

                                                                <i class="bx bx-check-circle d-block fs-4"></i>

                                                                <small>Review</small>

                                                            </a>

                                                        </li>

                                                    </ul>

                                                </div>

                                            </div>

                                            <br />

                                            <!-- ===================== Wizard Content ===================== -->

                                            <div class="card shadow-sm">

                                                <div class="card-body">

                                                    <asp:MultiView ID="mvWizard"
                                                        runat="server"
                                                        ActiveViewIndex="0">

                                                        <asp:View ID="vwGeneral" runat="server">

                                                            <!-- ========================= -->
                                                            <!-- STEP 1 - GENERAL INFORMATION -->
                                                            <!-- ========================= -->

                                                           

                                                                <!-- Application Information -->

                                                                <div class="col-md-12">

                                                                    <div class="card border-primary mb-4">

                                                                        <div class="card-header bg-label-primary">

                                                                            <h5 class="mb-0">
                                                                                <i class="bx bx-file"></i>
                                                                                Application Information
                                                                            </h5>

                                                                        </div>

                                                                        <div class="card-body">

                                                                            <div class="row">

                                                                                <div class="col-md-3 mb-3">

                                                                                    <label>Application No</label>

                                                                                    <asp:TextBox ID="txtApplicationNo"
                                                                                        runat="server"
                                                                                        CssClass="form-control"
                                                                                        ReadOnly="true">
                                                                                    </asp:TextBox>

                                                                                </div>

                                                                                <div class="col-md-3 mb-3">

                                                                                    <label>Application Date</label>

                                                                                    <asp:TextBox ID="txtApplicationDate"
                                                                                        runat="server"
                                                                                        TextMode="Date"
                                                                                        CssClass="form-control">
                                                                                    </asp:TextBox>

                                                                                </div>

                                                                                <div class="col-md-3 mb-3">

                                                                                    <label>Status</label>

                                                                                    <asp:TextBox ID="txtStatus"
                                                                                        runat="server"
                                                                                        CssClass="form-control"
                                                                                        ReadOnly="true"
                                                                                        Text="Draft">
                                                                                    </asp:TextBox>

                                                                                </div>

                                                                                <div class="col-md-3 mb-3">

                                                                                    <label>Version</label>

                                                                                    <asp:TextBox ID="txtVersion"
                                                                                        runat="server"
                                                                                        CssClass="form-control"
                                                                                        ReadOnly="true"
                                                                                        Text="1.0">
                                                                                    </asp:TextBox>

                                                                                </div>

                                                                            </div>

                                                                        </div>

                                                                    </div>

                                                                </div>
                                                               
                                                                <!-- Customer -->

                                                                <div class="col-md-12">

                                                                    <div class="card border-info mb-4">

                                                                        <div class="card-header bg-label-info">

                                                                            <h5>

                                                                                <i class="bx bx-building-house"></i>

                                                                                Customer Information

                                                                            </h5>

                                                                        </div>

                                                                        <div class="card-body">

                                                                            <div class="row">

                                                                                <div class="col-md-6 mb-3">

                                                                                    <label>Customer <span class="text-danger">*</span></label>
                                                                                    <asp:TextBox ID="txtCustomerName" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>

                                                                                </div>

                                                                                <div class="col-md-6 mb-3">

                                                                                    <label>Agreement No</label>

                                                                                   <asp:TextBox ID="txtAgreement" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>

                                                                                </div>

                                                                                <div class="col-md-6 mb-3">

                                                                                    <label>Quotation No</label>

                                                                                    <asp:TextBox ID="txtQuotationNo" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>

                                                                                </div>

                                                                                <div class="col-md-6 mb-3">

                                                                                    <label>Contact Person</label>

                                                                                    <asp:DropDownList
                                                                                        ID="ddlContactPerson"
                                                                                        runat="server"
                                                                                        CssClass="form-select"
                                                                                        AutoPostBack="true"
                                                                                        OnSelectedIndexChanged="ddlContactPerson_SelectedIndexChanged">
                                                                                    </asp:DropDownList>

                                                                                </div>

                                                                            </div>

                                                                        </div>

                                                                    </div>

                                                                </div>

                                                                <!-- Contact -->

                                                                <div class="col-md-12">

                                                                    <div class="card border-success mb-4">

                                                                        <div class="card-header bg-label-success">

                                                                            <h5>

                                                                                <i class="bx bx-user"></i>

                                                                                Contact Details

                                                                            </h5>

                                                                        </div>

                                                                        <div class="card-body">

                                                                            <div class="row">

                                                                                <div class="col-md-3 mb-3">

                                                                                    <label>Designation</label>

                                                                                    <asp:TextBox
                                                                                        ID="txtDesignation"
                                                                                        runat="server"
                                                                                        CssClass="form-control">
                                                                                    </asp:TextBox>

                                                                                </div>

                                                                                <div class="col-md-3 mb-3">

                                                                                    <label>Department</label>

                                                                                    <asp:TextBox
                                                                                        ID="txtDepartment"
                                                                                        runat="server"
                                                                                        CssClass="form-control">
                                                                                    </asp:TextBox>

                                                                                </div>

                                                                                <div class="col-md-3 mb-3">

                                                                                    <label>Mobile</label>

                                                                                    <asp:TextBox
                                                                                        ID="txtMobile"
                                                                                        runat="server"
                                                                                        CssClass="form-control">
                                                                                    </asp:TextBox>

                                                                                </div>

                                                                                <div class="col-md-3 mb-3">

                                                                                    <label>Telephone</label>

                                                                                    <asp:TextBox
                                                                                        ID="txtPhone"
                                                                                        runat="server"
                                                                                        CssClass="form-control">
                                                                                    </asp:TextBox>

                                                                                </div>

                                                                                <div class="col-md-6 mb-3">

                                                                                    <label>Email</label>

                                                                                    <asp:TextBox
                                                                                        ID="txtEmail"
                                                                                        runat="server"
                                                                                        CssClass="form-control">
                                                                                    </asp:TextBox>

                                                                                </div>

                                                                                <div class="col-md-6 mb-3">

                                                                                    <label>Website</label>

                                                                                    <asp:TextBox
                                                                                        ID="txtWebsite"
                                                                                        runat="server"
                                                                                        CssClass="form-control">
                                                                                    </asp:TextBox>

                                                                                </div>

                                                                                <div class="col-md-12">

                                                                                    <label>Head Office Address</label>

                                                                                    <asp:TextBox
                                                                                        ID="txtAddress"
                                                                                        runat="server"
                                                                                        TextMode="MultiLine"
                                                                                        Rows="3"
                                                                                        CssClass="form-control">
                                                                                    </asp:TextBox>

                                                                                </div>

                                                                            </div>

                                                                        </div>

                                                                    </div>

                                                                </div>

                                                                <!-- Organization -->

                                                                <div class="col-md-12">

                                                                    <div class="card border-warning">

                                                                        <div class="card-header bg-label-warning">

                                                                            <h5>

                                                                                <i class="bx bx-buildings"></i>

                                                                                Organization Information

                                                                            </h5>

                                                                        </div>

                                                                        <div class="card-body">

                                                                            <div class="row">

                                                                                <div class="col-md-6 mb-3">

                                                                                    <label>Production Location</label>

                                                                                    <asp:TextBox
                                                                                        ID="txtProductionLocation"
                                                                                        runat="server"
                                                                                        CssClass="form-control">
                                                                                    </asp:TextBox>

                                                                                </div>

                                                                                <div class="col-md-3 mb-3">

                                                                                    <label>Organization Area (Sq.m)</label>

                                                                                    <asp:TextBox
                                                                                        ID="txtArea"
                                                                                        runat="server"
                                                                                        CssClass="form-control">
                                                                                    </asp:TextBox>

                                                                                </div>

                                                                                <div class="col-md-3 mb-3">

                                                                                    <label>Total Employees</label>

                                                                                    <asp:TextBox
                                                                                        ID="txtEmployees"
                                                                                        runat="server"
                                                                                        CssClass="form-control">
                                                                                    </asp:TextBox>

                                                                                </div>

                                                                                <div class="col-md-12">

                                                                                    <label>Business Process Description</label>

                                                                                    <asp:TextBox
                                                                                        ID="txtBusinessProcess"
                                                                                        runat="server"
                                                                                        TextMode="MultiLine"
                                                                                        Rows="5"
                                                                                        CssClass="form-control">
                                                                                    </asp:TextBox>

                                                                                </div>

                                                                            </div>

                                                                        </div>

                                                                    </div>

                                                                </div>

                                                            </div>

                                                            <hr />

                                                            <div class="d-flex justify-content-between">

                                                                <asp:Button
                                                                    ID="btnSaveDraft"
                                                                    runat="server"
                                                                    CssClass="btn btn-outline-secondary"
                                                                    Text="Save Draft"
                                                                    OnClick="btnSaveDraft_Click" />

                                                                <asp:Button
                                                                    ID="btnNext1"
                                                                    runat="server"
                                                                    CssClass="btn btn-primary"
                                                                    Text="Next →"
                                                                    OnClick="btnNext1_Click" />

                                                            </div>

                                                        </asp:View>

                                                        <asp:View ID="vwCertification" runat="server">

                                                            <!-- Step 2 -->

                                                        </asp:View>

                                                        <asp:View ID="vwOrganization" runat="server">
                                                        </asp:View>

                                                        <asp:View ID="vwMultiSite" runat="server">
                                                        </asp:View>

                                                        <asp:View ID="vwDocuments" runat="server">
                                                        </asp:View>

                                                        <asp:View ID="vwISO45001" runat="server">
                                                        </asp:View>

                                                        <asp:View ID="vwISO14001" runat="server">
                                                        </asp:View>

                                                        <asp:View ID="vwTransfer" runat="server">
                                                        </asp:View>

                                                        <asp:View ID="vwDeclaration" runat="server">
                                                        </asp:View>

                                                        <asp:View ID="vwReview" runat="server">
                                                        </asp:View>

                                                    </asp:MultiView>

                                                </div>

                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </fieldset>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </form>
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

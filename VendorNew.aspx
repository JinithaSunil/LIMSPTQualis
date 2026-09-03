<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="VendorNew.aspx.cs" Inherits="LIMS.VendorNew" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://code.jquery.com/jquery-1.11.2.min.js"></script>

    <%--<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />--%>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    <link href="css/style.css" rel="stylesheet">

    <style type="text/css">
        .ContentPlaceHolderBody_tvTables_0 {
            margin-left: 15px;
        }

        .ContentPlaceHolderBody_tvTablesn0CheckBox input[type="checkbox"] {
            margin-right: 8px;
            position: static !important; /* KEY FIX */
            left: auto !important;
            visibility: visible !important;
            margin-left: 6px !important;
        }

        #ContentPlaceHolderBody_tvTablesn1CheckBox input[type="checkbox"] {
            margin-right: 8px;
            position: static !important; /* KEY FIX */
            left: auto !important;
            visibility: visible !important;
            margin-left: 6px !important;
        }

        .ContentPlaceHolder1_tvTables_0 {
            color: Black;
            font-size: 14px;
            font-family: 'Open Sans', Arial, Helvetica, sans-serif;
            text-decoration: none;
            padding-left: 10px;
            color: rgba(118, 118, 118, 1.0);
            display: inline-block !important;
        }
        /* Add this CSS to fix the tab navigation issues */

        /* Ensure Bootstrap tabs work properly */
        .nav-tabs .nav-link.active {
            color: #495057;
            background-color: #fff;
            border-color: #dee2e6 #dee2e6 #fff;
            border-bottom: 2px solid #007bff !important; /* Blue line under active tab */
        }

        .nav-tabs .nav-link {
            border: 1px solid transparent;
            border-top-left-radius: 0.25rem;
            border-top-right-radius: 0.25rem;
            cursor: pointer;
        }

            .nav-tabs .nav-link:hover {
                border-color: #e9ecef #e9ecef #dee2e6;
            }

        /* Fix tab content display */
        .tab-content > .tab-pane {
            display: none;
        }

        .tab-content > .active {
            display: block !important;
        }

        .tab-content > .show {
            display: block !important;
        }

        /* Ensure proper tab switching */
        .tab-pane.fade {
            transition: opacity 0.15s linear;
            opacity: 0;
        }

            .tab-pane.fade.show {
                opacity: 1;
            }

        .tab-pane.active {
            opacity: 1;
        }

        /* Override any conflicting styles */
        .nav-tabs {
            border-bottom: 1px solid #dee2e6;
        }
            /* ADD this to your existing CSS - don't remove the previous CSS */

            /* Target and remove only the duplicate blue line */
            .nav-tabs .nav-link.active::after,
            .nav-tabs .nav-item .nav-link.active::after {
                content: none !important;
                display: none !important;
            }

            /* Remove any duplicate border from Bootstrap's default styling */
            .nav-tabs .nav-item.show .nav-link,
            .nav-tabs .nav-link.active {
                border-bottom-width: 2px !important; /* Ensure only one 2px line */
            }

            /* Remove focus-related duplicate lines */
            .nav-tabs .nav-link:focus::after,
            .nav-tabs .nav-link:focus {
                box-shadow: none !important;
                outline: none !important;
            }

            /* Remove any programmatically added duplicate styling */
            .nav-tabs > .nav-item > .nav-link.active:focus,
            .nav-tabs > .nav-item > .nav-link.active:hover {
                border-bottom: 2px solid #007bff !important; /* Keep only our line */
                border-bottom-width: 2px !important;
            }
    </style>
    <script type="text/javascript" src="//netdna.bootstrapcdn.com/bootstrap/3.0.0/js/bootstrap.min.js"></script>
    <script type="text/javascript">

</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="body">
                    <%--<asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>--%>
                    <!-- Verification Section -->
                    <div class="col-xl-12" id="divVerify" runat="server">
                        <div class="header">
                            <h2>VERIFICATION</h2>
                        </div>
                        <div class="content-body">
                            <div class="row">
                                <div class="col-lg-12 col-md-12 col-12">
                                    <div class="col-lg-12">
                                        <div class="form-row">
                                            <div class="form-line">
                                                <div class="col-md-12 mb-3" style="margin-top: 20px;">

                                                    <label for="txtReferenceNumber">Reference</label>
                                                    <asp:TextBox ID="txtReferenceNumber" class="form-control" runat="server" Text=""></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-row">
                                            <div class="col-md-4 mb-3">
                                            </div>
                                            <div class="col-md-4 mb-3">
                                                <asp:Button ID="btnSubmitReference" OnClick="btnSubmitReference_Click" runat="server" class="btn btn-primary" Style="margin-left: 100px;" Text="Verify" />
                                            </div>
                                            <div class="col-md-4 mb-3">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                    <div id="placeholder"></div>

                    <!-- Vendor Details Section -->
                    <div class="col-xl-12" id="divContent" runat="server">

                        <h3>VENDOR DETAILS</h3>

                        <%-- </header>--%>
                        <!-- Horizontal - start -->
                        <div class="row">
                            <div class="col-lg-12">
                                <ul class="nav nav-tabs primary" id="myTab1" role="tablist">
                                    <li class="nav-item">
                                        <a id="aguide" runat="server" class="nav-link " href="#guide" role="tab" data-toggle="tab">
                                            <i class="fa fa-building-o"></i>Guidelines
                                        </a>
                                    </li>
                                    <li class="nav-item">
                                        <a id="ahome" runat="server" class="nav-link disabled" href="#home" role="tab" data-toggle="tab">
                                            <i class="fa fa-briefcase"></i>Vendor
                                        </a>
                                    </li>
                                    <li class="nav-item">
                                        <a id="abank" runat="server" class="nav-link disabled" role="tab" href="#bank" data-toggle="tab">
                                            <i class="fa fa-bank"></i>Bank
                                        </a>
                                    </li>
                                    -
                                    <li class="nav-item">
                                        <a id="aperson" runat="server" class="nav-link disabled" role="tab" href="#person" data-toggle="tab">
                                            <i class="fa fa-user"></i>Contact Person
                                        </a>
                                    </li>
                                    <li class="nav-item">
                                        <a id="alocation" runat="server" class="nav-link disabled" role="tab" href="#location" data-toggle="tab">
                                            <i class="fa fa-map-marker"></i>Location
                                        </a>
                                    </li>
                                    <li class="nav-item">
                                        <a id="afiles" runat="server" class="nav-link disabled" role="tab" href="#files" data-toggle="tab">
                                            <i class="fa fa-upload"></i>File Upload
                                        </a>
                                    </li>
                                </ul>
                                <div class="tab-content primary" id="myTabContent1">
                                    <div id="divguide" runat="server" class="tab-pane fade">
                                        <div>
                                            <div class="row">
                                                <div class="col-12">
                                                    <div class="blog_post full_blog_post" style="margin-left: 17px;">
                                                        <h3>Registration Form - Fill-up Guidelines</h3>
                                                        <div class="blog-content">
                                                            <div>
                                                                <b>Kindly keep the documents ready for upload since the system will time-out if idle for more than 10 minutes for security reasons.</b>
                                                                <br />
                                                                <br />
                                                                Please find below Documents Required:-
                                                                                    <ol>
                                                                                        <li>PAN Card(Mandatory)</li>
                                                                                        <li>Cancelled Cheque Copy(Mandatory)</li>
                                                                                        <li>Company Registration(If applicable)</li>
                                                                                        <li>VAT Registration(If applicable)</li>
                                                                                        <li>PF Registration(If applicable)</li>
                                                                                        <li>MSME(If applicable)</li>
                                                                                        <li>ESIC(If applicable)</li>
                                                                                        <li>Company Logo(Optional)</li>
                                                                                    </ol>
                                                                <asp:Button ID="btnGuideLines" runat="server" CssClass="btn btn-primary"
                                                                    Text="Continue" OnClick="btnGuideLines_Click" OnClientClick="ShowVendor(); return true;" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>


                                    </div>
                                    <div id="divhome" runat="server" class="tab-pane fade">
                                        <div>
                                            <div class="row">
                                                <div class="col-lg-12 col-md-12 col-12">


                                                    <div class="form-row">
                                                        <div class="col-md-3 mb-3">
                                                            <label for="VendorCode">Vendor Code</label>
                                                            <span id="spUn" style="color: Red" runat="server">*</span>
                                                            <asp:TextBox ID="txtVendorCode" class="form-control" runat="server" disabled="true"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-5 mb-3">
                                                            <label for="txtVendorName">Vendor Name</label>
                                                            <span id="Span1" style="color: Red" runat="server">*</span>
                                                            <asp:TextBox ID="txtVendorName" class="form-control" runat="server"></asp:TextBox>
                                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender6" runat="server"
                                                                FilterType="LowercaseLetters, UppercaseLetters, Custom"
                                                                TargetControlID="txtVendorName"
                                                                ValidChars="& 1234567890.-()," />
                                                        </div>
                                                        <div class="col-md-4 mb-3">
                                                            <label for="txtCompanyName">Company Name</label>
                                                            <span id="Span14" style="color: Red" runat="server">*</span>
                                                            <asp:TextBox ID="txtCompanyName" class="form-control" runat="server"></asp:TextBox>
                                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender3" runat="server"
                                                                FilterType="LowercaseLetters, UppercaseLetters, Custom"
                                                                TargetControlID="txtCompanyName"
                                                                ValidChars="& 1234567890.-()," />
                                                        </div>
                                                    </div>


                                                    <div class="form-row">
                                                        <div class="col-md-4 mb-3">
                                                            <label for="lblNumberofEmployees">Company Size</label>
                                                            <span id="Span19" style="color: Red" runat="server">*</span>
                                                            <asp:DropDownList ID="ddlCompanySize" runat="server" CssClass="form-control"></asp:DropDownList>
                                                        </div>
                                                        <div class="col-md-4 mb-3">
                                                            <label for="ddlYearOfEstablishment">Year of Establishment</label>
                                                            <span id="Span3" style="color: Red" runat="server">*</span>
                                                            <asp:DropDownList ID="ddlYearOfEstablishment" runat="server" class="form-control"></asp:DropDownList>
                                                        </div>
                                                        <div class="col-md-4 mb-3">
                                                            <label for="ddlTypeofEstablishment">Type of Establishment</label>
                                                            <span id="Span27" style="color: Red" runat="server">*</span>
                                                            <asp:DropDownList ID="ddlTypeofEstablishment" runat="server" class="form-control"></asp:DropDownList>
                                                        </div>
                                                    </div>


                                                    <div class="form-row">
                                                        <div class="col-md-6 mb-3">
                                                            <label for="ddlTypeOfBussiness">Type of Business</label>
                                                            <span id="Span5" style="color: Red" runat="server">*</span>
                                                            <asp:DropDownList ID="ddlTypeOfBussiness" runat="server" class="form-control">
                                                                <asp:ListItem Value="">Select Type of Business</asp:ListItem>
                                                                <asp:ListItem Value="manufacturer">Manufacturer</asp:ListItem>
                                                                <asp:ListItem Value="trader">Trader</asp:ListItem>
                                                                <asp:ListItem Value="Agent">Agent</asp:ListItem>
                                                                <asp:ListItem Value="ServiceProvider">Service Provider</asp:ListItem>
                                                                <asp:ListItem Value="Agent">Agent</asp:ListItem>
                                                                <asp:ListItem Value="Contractor">Contractor</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                        <div class="col-md-6 mb-3">
                                                            <label for="ddlDealerCode">Dealer</label>
                                                            <span id="Span2" style="color: Red" runat="server">*</span>
                                                            <asp:DropDownList ID="ddlDealer" runat="server" class="form-control"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="form-row">
                                                        <div class="col-md-12 mb-3">
                                                            <button id="btnSelectCategory" runat="server" type="button" class="btn btn-primary" data-toggle="collapse" data-target="#demo" style="width: 100%">Select Distributor Category</button>
                                                            <div id="demo" class="collapse">
                                                                <table style="width: 100%">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:TreeView ID="tvTables" runat="server" ForeColor="#767676" Font-Size="14px" ShowCheckBoxes="All" OnClick="OnTreeClick(event)">
                                                                            </asp:TreeView>
                                                                        </td>
                                                                        <td>
                                                                            <asp:HiddenField ID="hdnTree" runat="server" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>

                                                        </div>
                                                    </div>
                                                    <div class="form-row">
                                                        <div class="col-md-3 mb-3" id="divRegistrationNumber" runat="server">
                                                            <label for="txtRegistrationNumber">Registration No.</label>
                                                            <span id="Span28" style="color: Red" runat="server">*</span>
                                                            <asp:TextBox ID="txtRegistrationNumber" class="form-control" runat="server" MaxLength="25"></asp:TextBox>
                                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender4" runat="server" Enabled="True" TargetControlID="txtRegistrationNumber" FilterType="UppercaseLetters,LowercaseLetters,Custom" ValidChars="1234567890 "></ajaxToolkit:FilteredTextBoxExtender>
                                                        </div>

                                                        <div class="col-md-3 mb-2">
                                                            <label>TAX</label>
                                                            <span id="Span30" style="color: black" runat="server">&nbsp;</span>
                                                            <asp:DropDownList ID="ddlVAT" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlVAT_SelectedIndexChanged">
                                                            </asp:DropDownList>
                                                        </div>

                                                        <div class="col-md-3 mb-2">
                                                            <label>Currency</label>
                                                            <span style="color: Red">*</span>
                                                            <asp:DropDownList ID="ddlQuoteCurrency" runat="server" CssClass="form-control">
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="form-row">
                                                        <div class="col-md-3 mb-3">
                                                            <label for="txtPF">PF No.</label>
                                                            <span id="Span15" style="color: black" runat="server">(if applicable)</span>
                                                            <asp:TextBox ID="txtPF" class="form-control" MaxLength="12" runat="server"></asp:TextBox>
                                                            <ajaxToolkit:FilteredTextBoxExtender ID="ftbAdhar" runat="server" Enabled="true" TargetControlID="txtPF" FilterType="Numbers" FilterMode="ValidChars" />

                                                        </div>
                                                        <div class="col-md-3 mb-3">
                                                            <label for="txtPan">NPWP</label>
                                                            <span id="Span32" style="color: Red" runat="server">*</span>
                                                            <asp:TextBox ID="txtPan" class="form-control" MaxLength="16" runat="server"></asp:TextBox>
                                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender_gpf_no" runat="server" Enabled="True" TargetControlID="txtPan" FilterType="UppercaseLetters,LowercaseLetters,Custom" ValidChars="1234567890 "></ajaxToolkit:FilteredTextBoxExtender>
                                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator8" runat="server" ControlToValidate="txtPan" ValidationExpression="^[\s\S]{10,}$" ForeColor="Red" Display="Dynamic" ErrorMessage="Pan Card Number not Valid"></asp:RegularExpressionValidator>
                                                        </div>
                                                        <div class="col-md-3 mb-3" id="div3" runat="server">
                                                            <label for="txtUANNumber">MSME Registration</label>
                                                            <span id="Span17" style="color: black" runat="server">(if applicable)</span>
                                                            <asp:TextBox ID="txtUANNumber" MaxLength="12" class="form-control" runat="server"></asp:TextBox>
                                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender7" runat="server" Enabled="True" TargetControlID="txtUANNumber" FilterType="UppercaseLetters,LowercaseLetters,Custom" ValidChars="1234567890 "></ajaxToolkit:FilteredTextBoxExtender>

                                                        </div>
                                                        <div class="col-md-3 mb-3">
                                                            <label for="txtESIC">ESIC</label>
                                                            <span id="Span36" style="color: black" runat="server">(if applicable)</span>
                                                            <asp:TextBox ID="txtESIC" class="form-control" MaxLength="17" runat="server"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-3 mb-3">
                                                            <label for="txtIEC">Import Export Code (IEC)</label>
                                                            <span id="Span4" style="color: black" runat="server">(if applicable)</span>
                                                            <asp:TextBox ID="txtIEC" class="form-control" MaxLength="12" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="form-row">
                                                        <div class="col-12 mb-1">
                                                            <label for="txtYearlyTurnover1">
                                                                Yearly Turnover (3 Year's Turnover Required)
                                                                  <span id="Span37" runat="server">*</span>
                                                            </label>
                                                        </div>
                                                    </div>

                                                    <div class="form-row">
                                                        <div class="col-md-4 mb-3">
                                                            <label for="txtYearlyTurnover1">
                                                            </label>
                                                            <asp:Label ID="lblYearlyTurnover1" runat="server" Text="1960"></asp:Label>
                                                            <asp:TextBox ID="txtYearlyTurnover1" class="form-control" runat="server"></asp:TextBox>
                                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender27" runat="server" FilterType="Numbers, Custom" ValidChars="." TargetControlID="txtYearlyTurnover1" />
                                                        </div>
                                                        <div class="col-md-4 mb-3">
                                                            <label for="txtYearlyTurnover2"></label>
                                                            <asp:Label ID="lblYearlyTurnover2" runat="server" Text="1961"></asp:Label>
                                                            <asp:TextBox ID="txtYearlyTurnover2" class="form-control" runat="server"></asp:TextBox>
                                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender28" runat="server" FilterType="Numbers, Custom" ValidChars="." TargetControlID="txtYearlyTurnover2" />
                                                        </div>
                                                        <div class="col-md-4 mb-3">
                                                            <label for="txtYearlyTurnover3"></label>
                                                            <asp:Label ID="lblYearlyTurnover3" runat="server" Text="1962"></asp:Label>
                                                            <asp:TextBox ID="txtYearlyTurnover3" class="form-control" runat="server"></asp:TextBox>
                                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender29" runat="server" FilterType="Numbers, Custom" ValidChars="." TargetControlID="txtYearlyTurnover3" />
                                                        </div>
                                                    </div>

                                                    <div class="form-row">
                                                        <div class="col-md-4 mb-3" id="divRequiredCredit" runat="server">
                                                            <label for="txtRequiredCredit">Required Credit</label>
                                                            <asp:TextBox ID="txtRequiredCredit" class="form-control" runat="server"></asp:TextBox>
                                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" FilterType="Numbers, Custom" ValidChars="." TargetControlID="txtRequiredCredit" />
                                                        </div>
                                                        <div class="col-md-4 mb-3" id="divApprovedCredit" runat="server">
                                                            <label for="txtApprovedCredit">Approved Credit</label>
                                                            <asp:TextBox ID="txtApprovedCredit" class="form-control" runat="server"></asp:TextBox>
                                                            <asp:RegularExpressionValidator ID="revApprovedCredit" runat="server"
                                                                ControlToValidate="txtApprovedCredit"
                                                                ErrorMessage="* Only numeric values allowed."
                                                                ValidationExpression="^\d+(\.\d{1,2})?$"
                                                                Display="Dynamic"
                                                                CssClass="text-danger" />
                                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender2" runat="server" FilterType="Numbers, Custom" ValidChars="." TargetControlID="txtApprovedCredit" />
                                                        </div>
                                                        <div class="col-md-4 mb-3">
                                                            <label for="txtWebsite">Website</label>
                                                            <asp:TextBox ID="txtWebsite" class="form-control" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>


                                                    <div class="form-row">
                                                        <div class="col-md-11 mb-3">
                                                        </div>
                                                        <div class="col-md-1 mb-3">
                                                            <asp:Button ID="btnSaveVendor" runat="server" Text="Next"
                                                                OnClick="btnVendor_Click" OnClientClick="ShowContactPerson(); return true;"
                                                                class="btn btn-primary" />

                                                            <asp:HiddenField ID="hdnNextVendor" runat="server" Value="1" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                    <div id="divbank" runat="server" class="tab-pane fade">
                                        <div>
                                            <div class="row">
                                                <div class="col-lg-12 col-md-12 col-12">
                                                    <div class="form-row">
                                                        <div class="col-md-3 mb-3">
                                                            <label for="txtAccountName">Account Name</label>
                                                            <span id="Span23" style="color: Red" runat="server">*</span>
                                                            <asp:TextBox ID="txtAccountName" class="form-control" runat="server" MaxLength="100"></asp:TextBox>
                                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender8" runat="server" FilterType="LowercaseLetters, UppercaseLetters, Custom" TargetControlID="txtAccountName" ValidChars=" " />
                                                        </div>
                                                        <div class="col-md-3 mb-3">
                                                            <label for="txtAccountNumber">Account Number</label>
                                                            <span id="Span20" style="color: Red" runat="server">*</span>
                                                            <asp:TextBox ID="txtAccountNumber" class="form-control" runat="server" MaxLength="20"></asp:TextBox>
                                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender19" runat="server" Enabled="true" TargetControlID="txtAccountNumber" FilterType="Numbers" FilterMode="ValidChars" />
                                                        </div>
                                                        <div class="col-md-3 mb-3">
                                                            <label for="txtBankName">Bank Name</label>
                                                            <span id="Span24" style="color: Red" runat="server">*</span>
                                                            <asp:TextBox ID="txtBankName" class="form-control" runat="server" MaxLength="20"></asp:TextBox>
                                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender9" runat="server" FilterType="LowercaseLetters, UppercaseLetters, Custom" TargetControlID="txtBankName" ValidChars=" " />
                                                        </div>
                                                        <div class="col-md-3 mb-3">
                                                            <label for="txtBankBranch">Bank Branch</label>
                                                            <span id="Span22" style="color: Red" runat="server">*</span>
                                                            <asp:TextBox ID="txtBankBranch" class="form-control" runat="server" MaxLength="20"></asp:TextBox>
                                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender10" runat="server" FilterType="LowercaseLetters, UppercaseLetters, Custom" TargetControlID="txtBankBranch" ValidChars=" " />
                                                        </div>
                                                        <div class="col-md-3 mb-3">
                                                            <label for="txtBankBranch">Branch Code</label>
                                                            <span id="Span26" style="color: Red" runat="server">*</span>
                                                            <asp:TextBox ID="txtBankBranchCode" class="form-control" runat="server" MaxLength="20"></asp:TextBox>
                                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender26" runat="server" FilterType="LowercaseLetters, UppercaseLetters, Custom" TargetControlID="txtBankBranchCode" ValidChars="1234567890 " />
                                                        </div>
                                                    </div>
                                                    <div class="form-row">

                                                        <div class="col-md-3 mb-3">
                                                            <label for="txtIFSCode">IFS Code</label>
                                                            <span id="Span21" style="color: Red" runat="server">*</span><br />
                                                            <asp:TextBox ID="txtIFSCode" class="form-control" runat="server" MaxLength="20"></asp:TextBox>
                                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender20" runat="server" Enabled="True" TargetControlID="txtIFSCode" FilterType="UppercaseLetters,LowercaseLetters,Custom" ValidChars="1234567890 "></ajaxToolkit:FilteredTextBoxExtender>
                                                        </div>
                                                        <div class="col-md-3 mb-3">
                                                            <label for="txtSwiftCode">Swift Code</label>
                                                            <%--<span id="Span26" style="color: Red" runat="server">*</span><br />--%>
                                                            <asp:TextBox ID="txtSwiftCode" class="form-control" runat="server" MaxLength="20"></asp:TextBox>
                                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender11" runat="server" FilterType="LowercaseLetters, UppercaseLetters,Custom" TargetControlID="txtSwiftCode" ValidChars="1234567890" />
                                                        </div>
                                                        <div class="col-md-3 mb-3" style="padding-top: 30px;">
                                                            <asp:Button ID="btnAddBank" OnClick="btnAddBank_Click" runat="server" class="btn btn-primary" Text="Add" />

                                                        </div>

                                                    </div>
                                                    <div class="form-row">
                                                        <div class="col-md-4 mb-3">
                                                        </div>
                                                        <div class="col-md-4 mb-3">
                                                        </div>
                                                    </div>
                                                    <div class="form-row">
                                                        <div class="col-md-12 mb-3">
                                                            <asp:GridView ID="gvBank" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered table-hover table-checkable order-column" BorderWidth="3px" CellP
                                                                ing="4" Width="90%" OnRowCommand="gvBank_RowCommand">
                                                                <Columns>
                                                                    <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="100">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                                        </ItemTemplate>
                                                                        <ItemStyle Width="100px"></ItemStyle>
                                                                    </asp:TemplateField>
                                                                    <asp:BoundField DataField="AccountName" HeaderText="Account Name" />
                                                                    <asp:BoundField DataField="BankName" HeaderText="Bank Name" />
                                                                    <asp:BoundField DataField="BankBranch" HeaderText="Bank Branch" />
                                                                    <asp:BoundField DataField="AccountNumber" HeaderText="Account Number" />
                                                                    <asp:BoundField DataField="IFSCode" HeaderText="IFS Code" />
                                                                    <asp:BoundField DataField="SwiftCode" HeaderText="Swift Code" />
                                                                    <%-- <asp:BoundField DataField="CancelCheque" HeaderText="Cancel Cheque" />--%>
                                                                    <asp:ButtonField ButtonType="Image" ImageUrl="~/assets/img/del.ico" ControlStyle-Width="35px" CommandName="del" />
                                                                    <%--<asp:ButtonField ButtonType="Image" ImageUrl="~/assets/img/cross.png" ControlStyle-Width="35px" CommandName="del" />--%>
                                                                </Columns>
                                                            </asp:GridView>
                                                        </div>
                                                    </div>
                                                    <div class="form-row">
                                                        <div class="col-md-10 mb-3">
                                                        </div>
                                                        <div class="col-md-1 mb-3">
                                                            <asp:Button ID="btnPreviousVendor" runat="server" class="btn btn-primary" Text="Previous" OnClientClick="ShowVendor();" OnClick="btnPreviousbank_Click" />
                                                        </div>
                                                        <div class="col-md-1 mb-3">
                                                            <asp:Button ID="btnSaveBank" OnClick="btnSaveBank_Click" runat="server" class="btn btn-primary" Text="Next" OnClientClick="ShowContactPerson();  return true;" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                    <div id="divperson" runat="server" class="tab-pane fade ">
                                        <div>
                                            <div class="form-row">
                                                <div class="col-md-3 mb-3">
                                                    <label for="ddlTitle">Title</label>
                                                    <span id="Span6" style="color: Red" runat="server">*</span>
                                                    <asp:DropDownList ID="ddlTitle" runat="server" Width="100%" class="form-control">
                                                        <asp:ListItem Value="">Select Title</asp:ListItem>
                                                        <asp:ListItem Value="mr">Mr.</asp:ListItem>
                                                        <asp:ListItem Value="mrs">Mrs.</asp:ListItem>
                                                        <asp:ListItem Value="miss">Miss</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-md-3 mb-3">
                                                    <label for="txtFirstName">First Name</label>
                                                    <span id="Span16" style="color: Red" runat="server">*</span><br />
                                                    <asp:TextBox ID="txtFirstName" class="form-control" runat="server"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender12" runat="server" FilterType="LowercaseLetters, UppercaseLetters, Custom" TargetControlID="txtFirstName" ValidChars=" " />
                                                </div>
                                                <div class="col-md-3 mb-3">
                                                    <label for="txtMiddleName">Middle Name</label>
                                                    <asp:TextBox ID="txtMiddleName" class="form-control" runat="server"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender13" runat="server" FilterType="LowercaseLetters, UppercaseLetters, Custom" TargetControlID="txtMiddleName" ValidChars=" " />
                                                </div>
                                                <div class="col-md-3 mb-3">
                                                    <label for="txtLastName">Last Name</label>
                                                    <span id="Span18" style="color: Red" runat="server">*</span><br />
                                                    <asp:TextBox ID="txtLastName" class="form-control" runat="server"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender14" runat="server" FilterType="LowercaseLetters, UppercaseLetters, Custom" TargetControlID="txtLastName" ValidChars=" " />
                                                </div>
                                            </div>
                                            <div class="form-row">
                                                <div class="col-md-4 mb-3">
                                                    <label for="txtDesignation">Designation</label>
                                                    <span id="Span34" style="color: black" runat="server">(if applicable)</span>
                                                    <asp:TextBox ID="txtDesignation" class="form-control" runat="server"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender15" runat="server" FilterType="LowercaseLetters, UppercaseLetters, Custom" TargetControlID="txtDesignation" ValidChars=" -&" />
                                                </div>
                                                <div class="col-md-4 mb-3">
                                                    <label for="txtDepartment">Department</label>
                                                    <span id="Span33" style="color: black" runat="server">(if applicable)</span>
                                                    <asp:TextBox ID="txtDepartment" class="form-control" runat="server"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender16" runat="server" FilterType="LowercaseLetters, UppercaseLetters, Custom" TargetControlID="txtDepartment" ValidChars=" " />
                                                </div>

                                                <div class="col-md-4 mb-3">
                                                    <lable for="ddlPersonAccountType">Select type</lable>

                                                    <%--<asp:TextBox ID="txtCustomerTitle" runat="server" CssClass="form-control" placeholder="Title"></asp:TextBox>--%>
                                                    <asp:DropDownList ID="ddlPersonAccountType" runat="server" class="form-control show-tick">
                                                        <asp:ListItem Value="0">Select Type</asp:ListItem>
                                                        <asp:ListItem Value="1">Primary</asp:ListItem>
                                                        <asp:ListItem Value="2">Secondary</asp:ListItem>
                                                    </asp:DropDownList>

                                                </div>

                                                <div class="col-md-4 mb-3" style="padding-top: 30px;">

                                                    <%--    <asp:Button ID="btnAddContactPerson" OnClick="btnAddContactPerson_Click" runat="server" class="btn btn-primary" Text="Add" />--%>
                                                    <asp:Button ID="btnAddContactPerson"
                                                        OnClick="btnAddContactPerson_Click"
                                                        runat="server"
                                                        class="btn btn-primary"
                                                        Text="Add" />

                                                </div>

                                            </div>
                                            <div class="form-row">
                                                <div class="col-md-4 mb-3">
                                                    <asp:Label ID="lblDuplicateContactPerson" runat="server" Visible="false" Text="Contact Person with same name added." ForeColor="Red"></asp:Label>
                                                </div>

                                            </div>
                                            <div class="form-row">
                                                <div class="col-md-12 mb-3">
                                                    <asp:GridView ID="gvPerson" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered table-hover table-checkable order-column" BorderWidth="3px" CellPadding="4" Width="90%" OnRowCommand="gvPerson_RowCommand">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="100">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="100px"></ItemStyle>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Title" HeaderText="Title" />
                                                            <asp:BoundField DataField="FirstName" HeaderText="First Name" />
                                                            <asp:BoundField DataField="MiddleName" HeaderText="Middle Name" />
                                                            <asp:BoundField DataField="LastName" HeaderText="Last Name" />
                                                            <asp:BoundField DataField="Designation" HeaderText="Designation" />
                                                            <asp:BoundField DataField="Department" HeaderText="Department" />
                                                            <asp:BoundField DataField="PersonAccountType" HeaderText="Contact Type" />
                                                            <asp:ButtonField ButtonType="Image" ImageUrl="~/assets/img/del.ico" ControlStyle-Width="35px" CommandName="del" />
                                                            <%-- <asp:ButtonField ButtonType="Image" ImageUrl="assets/img/cross.png" ControlStyle-Width="35px" CommandName="del" />--%>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                                <asp:HiddenField ID="hdnContactPerson" runat="server" Value="1" />
                                            </div>
                                            <div class="form-row" id="divSaveContactPerson" runat="server">
                                                <div class="col-md-10 mb-3">
                                                </div>
                                                <div class="col-md-1 mb-3">
                                                    <asp:Button ID="btnPreviousContactPerson" runat="server" class="btn btn-primary" Text="Previous" OnClientClick="ShowVendor(); return true;" OnClick="btnPreviousContactPerson_Click" />
                                                </div>
                                                <div class="col-md-1 mb-3">
                                                    <asp:Button ID="btnSaveContactPerson" OnClick="btnSaveContactPerson_Click" runat="server" class="btn btn-primary" Text="Next" OnClientClick="ShowLocation(); return true;" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div id="divlocation" runat="server" class="tab-pane fade">

                                        <div class="form-row">
                                            <div class="col-md-4 mb-3">
                                                <label for="ddlLocationType">Location Type</label>
                                                <span id="Span25" style="color: Red" runat="server">*</span><br />
                                                <asp:DropDownList ID="ddlLocationType" runat="server" Width="100%" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlLocationType_SelectedIndexChanged">
                                                    <asp:ListItem Value="">Select Location Type</asp:ListItem>
                                                    <asp:ListItem Value="registred">Registered Office</asp:ListItem>
                                                    <asp:ListItem Value="manufacture">Manufacturing Office</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-md-3 mb-3">
                                                <label for="ddlContactPerson">Contact Person</label>
                                                <span id="Span29" style="color: Red" runat="server">*</span><br />
                                                <asp:DropDownList ID="ddlContactPerson1" runat="server" Width="100%" class="form-control"></asp:DropDownList>
                                            </div>

                                            <div class="col-md-1 mb-3" style="padding-top: 30px;">
                                                <%--<asp:Button ID="btnAddMoreContactPerson" runat="server" class="btn btn-primary" Text=" + " ToolTip="Add More Contact Pesron" OnClientClick="ShowContactPerson();" />--%>
                                                <asp:Button ID="btnAddMoreContactPerson" runat="server"
                                                    CssClass="btn btn-primary"
                                                    Text=" + "
                                                    ToolTip="Add More Contact Person" OnClick="btnAddMoreContactPerson_Click"
                                                    OnClientClick="ShowContactPerson();return true;" />

                                            </div>
                                            <div class="col-md-4 mb-3" id="divLocationType" runat="server" visible="false" style="padding-left: 20px;">
                                                <label for="rblLocationType">Same as Registered Office Address?</label>
                                                <asp:RadioButtonList ID="rblLocationType" class="demo-radio-button" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="rblLocationType_SelectedIndexChanged">
                                                    <asp:ListItem Value="Yes">Yes &nbsp;&nbsp;&nbsp;&nbsp;</asp:ListItem>
                                                    <asp:ListItem Value="No">No</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>
                                        <div class="form-row">
                                            <div class="col-md-4 mb-3">
                                                <label for="ddlCountry">Country</label>
                                                <span id="Span13" style="color: Red" runat="server">*</span><br />
                                                <asp:DropDownList ID="ddlCountry" runat="server" AutoPostBack="true" Width="100%"
                                                    class="form-control" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged">
                                                    <asp:ListItem Value="">Select Country</asp:ListItem>

                                                </asp:DropDownList>
                                            </div>

                                            <div class="col-md-3 mb-3">
                                                <label for="txtMobile">Mobile</label>
                                                <span id="Span9" style="color: Red" runat="server">*</span><br />
                                                <table style="width: 100%">
                                                    <tr>
                                                        <td style="width: 20%">
                                                            <asp:TextBox ID="txtCountryCode" class="form-control" runat="server" Text="+62" disabled></asp:TextBox></td>
                                                        <td>
                                                            <asp:TextBox ID="txtMobile" class="form-control" runat="server" MaxLength="13"></asp:TextBox></td>
                                                    </tr>
                                                </table>
                                                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender24" runat="server" FilterType="Numbers" FilterMode="ValidChars" TargetControlID="txtMobile" />
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator9" runat="server" ControlToValidate="txtMobile" ValidationExpression="^[\s\S]{10,}$" ForeColor="Red" ErrorMessage="Enter 10 digits Mobile Number"></asp:RegularExpressionValidator>
                                            </div>
                                            <div class="col-md-4 mb-3">
                                                <label for="txtEmail">Email(multiple emails comma separated)</label><span id="Span7" style="color: Red" runat="server">*</span>
                                                <asp:TextBox ID="txtEmail" class="form-control" runat="server"></asp:TextBox>
                                                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender5" runat="server" FilterType="Numbers, LowercaseLetters, Custom" ValidChars=".@,-_" TargetControlID="txtEmail" />
                                            </div>
                                            <div class="col-md-4 mb-3">
                                                <label for="txtfax">Fax</label>
                                                <asp:TextBox ID="txtfax" class="form-control" runat="server"></asp:TextBox>
                                                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender30" runat="server" FilterType="Numbers, LowercaseLetters, Custom" ValidChars=".@,-_" TargetControlID="txtfax" />
                                            </div>
                                        </div>
                                        <div id="divAddress" runat="server">
                                            <div class="form-row">
                                                <div class="col-md-4 mb-3">
                                                    <label for="txtAddressLine1">Address Line1</label>
                                                    <span id="Span10" style="color: Red" runat="server">*</span><br />
                                                    <asp:TextBox ID="txtAddressLine1" class="form-control" runat="server"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender21" runat="server" FilterType="LowercaseLetters, UppercaseLetters, Numbers, Custom" ValidChars=",/#- " TargetControlID="txtAddressLine1" />
                                                </div>
                                                <div class="col-md-4 mb-3">
                                                    <label for="txtAddressLine2">Address Line2</label>
                                                    <asp:TextBox ID="txtAddressLine2" class="form-control" runat="server"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender22" runat="server" FilterType="LowercaseLetters, UppercaseLetters, Numbers, Custom" ValidChars=",/#- " TargetControlID="txtAddressLine2" />
                                                </div>
                                                <div class="col-md-4 mb-3">
                                                    <label for="txtAddressLine3">Address Line3</label>
                                                    <asp:TextBox ID="txtAddressLine3" class="form-control" runat="server"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender23" runat="server" FilterType="LowercaseLetters, UppercaseLetters, Numbers, Custom" ValidChars=",/#- " TargetControlID="txtAddressLine3" />
                                                </div>
                                            </div>

                                            <div class="form-row">
                                                <div class="col-md-4 mb-3">
                                                    <label for="txtCity">City</label>
                                                    <span id="Span11" style="color: Red" runat="server">*</span><br />
                                                    <asp:TextBox ID="txtCity" class="form-control" runat="server"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender17" runat="server" FilterType="LowercaseLetters, UppercaseLetters, Custom" ValidChars=" " TargetControlID="txtCity" />
                                                </div>
                                                <div class="col-md-4 mb-3">
                                                    <label for="txtState">State</label>
                                                    <span id="Span12" style="color: Red" runat="server">*</span><br />
                                                    <asp:TextBox ID="txtState" runat="server" CssClass="form-control">                                                  
                                                    </asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender18" runat="server" FilterType="LowercaseLetters, UppercaseLetters, Custom" ValidChars=" " TargetControlID="txtState" />
                                                </div>
                                                <div class="col-md-3 mb-3">
                                                    <label for="txtPincode">Pincode</label>
                                                    <asp:TextBox ID="txtPincode" class="form-control" runat="server"></asp:TextBox>
                                                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender25" runat="server" FilterType="Numbers, Custom" ValidChars=" " TargetControlID="txtPincode" />
                                                </div>
                                                <div class="col-md-1 mb-3" style="padding-top: 30px;">
                                                    <asp:Button ID="btnAddLocation" OnClick="btnAddLocation_Click" runat="server" class="btn btn-primary" Text="Add" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="form-row">
                                            <div class="col-md-4 mb-3">
                                                <asp:Label ID="lblDuplicateData" runat="server" Visible="false" Text="Registered Office Must be Only One." ForeColor="Red"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="form-row">
                                            <div class="col-md-4 mb-3">
                                                <asp:Label ID="lblSaveData" runat="server" Visible="false" Text="Add Manufacturing office address." ForeColor="Red"></asp:Label>
                                            </div>
                                        </div>
                                        <div class="form-row table-resposive">
                                            <div class="col-md-12 mb-3">
                                                <div>
                                                    <asp:GridView ID="gvLocation" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered table-hover table-checkable order-column" BorderWidth="3px" CellPadding="4" Width="100%" OnRowCommand="gvLocation_RowCommand">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="100">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="100px"></ItemStyle>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="LocationType" HeaderText="Location Type" />
                                                            <asp:BoundField DataField="ContactPersonName" HeaderText="Contact Person" />
                                                            <asp:BoundField DataField="Mobile" HeaderText="Mobile" />
                                                            <asp:BoundField DataField="Email" HeaderText="Email" />
                                                            <asp:BoundField DataField="Fax" HeaderText="Fax" />

                                                            <asp:BoundField DataField="AddressLine1" HeaderText="Address Line1" />
                                                            <asp:BoundField DataField="AddressLine2" HeaderText="Address Line2" />
                                                            <asp:BoundField DataField="AddressLine3" HeaderText="Address Line3" />
                                                            <asp:BoundField DataField="City" HeaderText="City" />
                                                            <asp:BoundField DataField="State" HeaderText="State" />
                                                            <asp:BoundField DataField="Country" HeaderText="Country" />
                                                            <asp:BoundField DataField="PersonAccountType" HeaderText="Account Type" />
                                                            <asp:ButtonField ButtonType="Image" ImageUrl="~/assets/img/del.ico" ControlStyle-Width="35px" CommandName="del" />
                                                            <%--  <asp:ButtonField ButtonType="Image" ImageUrl="~/assets/img/cross.png" ControlStyle-Width="35px" CommandName="del" />--%>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="form-row" id="divSaveLocation" runat="server">
                                            <div class="col-md-10 mb-3">
                                            </div>
                                            <div class="col-md-1 mb-3" style="padding-left: 5px;">
                                                <asp:Button ID="btnPreviousLocation" runat="server" class="btn btn-primary" Text="Previous" OnClientClick="ShowContactPerson();return true;" OnClick="btnPreviousLocation_Click" />
                                            </div>


                                            <div class="col-md-1 mb-3">
                                                <asp:Button ID="btnSaveLocation" OnClick="btnSaveLocation_Click" runat="server" class="btn btn-primary" Text="Next" />
                                            </div>
                                        </div>
                                    </div>
                                    <div id="divfiles" runat="server" class="tab-pane fade">
                                        <div>
                                            <div class="row">
                                                <div class="col-lg-12 col-md-12 col-12">


                                                    <div class="form-row">

                                                        <div class="col-md-12 mb-3">
                                                            <table style="width: 100%">
                                                                <tr>
                                                                    <td style="width: 22%; padding-left: 6%;">
                                                                        <asp:Label ID="lblPanCard" runat="server" Text="NPWP"></asp:Label>
                                                                        <span id="Span8" style="color: Red" runat="server">*</span></td>
                                                                    <td style="width: 20%">
                                                                        <asp:FileUpload ID="fpPanCard" runat="server" class="btn btn-primary" />
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" Display="Dynamic" ControlToValidate="fpPanCard" ErrorMessage="Please select a .pdf, .doc/.docx, .eps, .jpeg, .jpg, .png   file" ForeColor="Red" ValidationExpression="^([a-zA-Z].*|[1-9].*)\.(((p|P)(d|D)(f|F))|((d|D)(o|O)(c|C))|((d|D)(o|O)(c|C)(x|X))|((e|E)(p|P)(s|S))|((p|P)(n|N)(g|G))|((j|J)(p|P)(e|E)(g|G))|((j|J)(p|P)(g|G)))$"></asp:RegularExpressionValidator>
                                                                        <asp:CustomValidator ControlToValidate="fpPanCard" ClientValidationFunction="uploadPan" Display="Dynamic" ForeColor="Red" runat="server" ErrorMessage="File is too large, must select file under 1 Mb." ValidationGroup="vgFileUpload" />
                                                                    </td>
                                                                    <td style="padding-left: 30px; width: 10%;">
                                                                        <asp:Button ID="btnSavePan" OnClick="btnSavePan_Click" runat="server" class="btn btn-primary" Text="Save" /></td>
                                                                    <td>
                                                                        <asp:Label ID="lblSavePan" runat="server" Text="File Saved" ForeColor="Blue" Visible="false"></asp:Label></td>
                                                                </tr>
                                                            </table>
                                                        </div>

                                                        <div class="col-md-12 mb-3">
                                                            <table style="width: 100%">
                                                                <tr>
                                                                    <td style="width: 22%; padding-left: 6%;">
                                                                        <asp:Label ID="lblCancelledChq" runat="server" Text="Cancelled Check"></asp:Label>
                                                                        <%--<span id="Span35" style="color: Red" runat="server">*</span>--%>
                                                                    </td>
                                                                    <td style="width: 20%">
                                                                        <asp:FileUpload ID="fpCancelledChq" runat="server" class="btn btn-primary" />
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" Display="Dynamic" ControlToValidate="fpCancelledChq" ErrorMessage="Please select a .pdf, .doc/.docx, .eps, .jpeg, .jpg, .png   file" ForeColor="Red" ValidationExpression="^([a-zA-Z].*|[1-9].*)\.(((p|P)(d|D)(f|F))|((d|D)(o|O)(c|C))|((d|D)(o|O)(c|C)(x|X))|((e|E)(p|P)(s|S))|((p|P)(n|N)(g|G))|((j|J)(p|P)(e|E)(g|G))|((j|J)(p|P)(g|G)))$"></asp:RegularExpressionValidator>
                                                                        <asp:CustomValidator ControlToValidate="fpCancelledChq" ClientValidationFunction="uploadCheque" Display="Dynamic" ForeColor="Red" runat="server" ErrorMessage="File is too large, must select file under 1 Mb." ValidationGroup="vgFileUpload" />
                                                                    </td>
                                                                    <td style="padding-left: 30px; width: 10%;">
                                                                        <asp:Button ID="btnSaveCancelledChq" OnClick="btnSaveCancelledChq_Click" runat="server" class="btn btn-primary" Text="Save" /></td>
                                                                    <td>
                                                                        <asp:Label ID="lblSaveCancelledChq" runat="server" Text="File Saved" ForeColor="Blue" Visible="false"></asp:Label></td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                        <div class="col-md-12 mb-3">
                                                            <table style="width: 100%">
                                                                <tr>
                                                                    <td style="width: 22%; padding-left: 6%;">
                                                                        <asp:Label ID="lblRegistrationofCompany" runat="server" Text="Registration of Company"></asp:Label>
                                                                        <span id="Span31" style="color: Red" runat="server"></span>
                                                                    </td>
                                                                    <td style="width: 20%">
                                                                        <asp:FileUpload ID="fpRegistrationofCompany" runat="server" class="btn btn-primary" />
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator7" runat="server" Display="Dynamic" ControlToValidate="fpRegistrationofCompany" ErrorMessage="Please select a .pdf, .doc/.docx, .eps, .jpeg, .jpg, .png   file" ForeColor="Red" ValidationExpression="^([a-zA-Z].*|[1-9].*)\.(((p|P)(d|D)(f|F))|((d|D)(o|O)(c|C))|((d|D)(o|O)(c|C)(x|X))|((e|E)(p|P)(s|S))|((p|P)(n|N)(g|G))|((j|J)(p|P)(e|E)(g|G))|((j|J)(p|P)(g|G)))$"></asp:RegularExpressionValidator>
                                                                        <asp:CustomValidator ControlToValidate="fpRegistrationofCompany" ClientValidationFunction="uploadCompany" Display="Dynamic" ForeColor="Red" runat="server" ErrorMessage="File is too large, must select file under 1 Mb." ValidationGroup="vgFileUpload" />
                                                                    </td>
                                                                    <td style="padding-left: 30px; width: 10%;">
                                                                        <asp:Button ID="btnSaveRegistrationofCompany" OnClick="btnSaveRegistrationofCompany_Click" runat="server" class="btn btn-primary" Text="Save" /></td>
                                                                    <td>
                                                                        <asp:Label ID="lblSaveRegistrationofCompany" runat="server" Text="File Saved" ForeColor="Blue" Visible="false"></asp:Label></td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                        <div class="col-md-12 mb-3">
                                                            <table style="width: 100%">
                                                                <tr>
                                                                    <td style="width: 22%; padding-left: 6%;">
                                                                        <asp:Label ID="lblGST" runat="server" Text="VAT Registration"></asp:Label>
                                                                    </td>
                                                                    <td style="width: 20%">
                                                                        <asp:FileUpload ID="fpGST" runat="server" class="btn btn-primary" />
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" Display="Dynamic" ControlToValidate="fpGST" ErrorMessage="Please select a .pdf, .doc/.docx, .eps, .jpeg, .jpg, .png   file" ForeColor="Red" ValidationExpression="^([a-zA-Z].*|[1-9].*)\.(((p|P)(d|D)(f|F))|((d|D)(o|O)(c|C))|((d|D)(o|O)(c|C)(x|X))|((e|E)(p|P)(s|S))|((p|P)(n|N)(g|G))|((j|J)(p|P)(e|E)(g|G))|((j|J)(p|P)(g|G)))$"></asp:RegularExpressionValidator>

                                                                        <asp:CustomValidator ControlToValidate="fpGST" ClientValidationFunction="upload" Display="Dynamic" ForeColor="Red" runat="server" ErrorMessage="File is too large, must select file under 1 Mb." ValidationGroup="vgFileUpload" />
                                                                    </td>
                                                                    <td style="padding-left: 30px; width: 10%;">
                                                                        <asp:Button ID="btnSaveGST" OnClick="btnSaveGST_Click" runat="server" class="btn btn-primary" Text="Save" /></td>
                                                                    <td>
                                                                        <asp:Label ID="lblSaveGST" runat="server" Text="File Saved" ForeColor="Blue" Visible="false"></asp:Label></td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                        <div class="col-md-12 mb-3">
                                                            <table style="width: 100%">
                                                                <tr>
                                                                    <td style="width: 22%; padding-left: 6%;">
                                                                        <asp:Label ID="lblPFCard" runat="server" Text="PF Card"></asp:Label>
                                                                    </td>
                                                                    <td style="width: 20%">
                                                                        <asp:FileUpload ID="fpPFCard" runat="server" class="btn btn-primary" />
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" Display="Dynamic" ControlToValidate="fpPFCard" ErrorMessage="Please select a .pdf, .doc/.docx, .eps, .jpeg, .jpg, .png   file" ForeColor="Red" ValidationExpression="^([a-zA-Z].*|[1-9].*)\.(((p|P)(d|D)(f|F))|((d|D)(o|O)(c|C))|((d|D)(o|O)(c|C)(x|X))|((e|E)(p|P)(s|S))|((p|P)(n|N)(g|G))|((j|J)(p|P)(e|E)(g|G))|((j|J)(p|P)(g|G)))$"></asp:RegularExpressionValidator>
                                                                        <asp:CustomValidator ControlToValidate="fpPFCard" ClientValidationFunction="uploadPF" Display="Dynamic" ForeColor="Red" runat="server" ErrorMessage="File is too large, must select file under 1 Mb." ValidationGroup="vgFileUpload" />
                                                                    </td>
                                                                    <td style="padding-left: 30px; width: 10%;">
                                                                        <asp:Button ID="btnSavePF" OnClick="btnSavePF_Click" runat="server" class="btn btn-primary" Text="Save" /></td>
                                                                    <td>
                                                                        <asp:Label ID="lblSavePF" runat="server" Text="File Saved" ForeColor="Blue" Visible="false"></asp:Label></td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                        <div class="col-md-12 mb-3">
                                                            <table style="width: 100%">
                                                                <tr>
                                                                    <td style="width: 22%; padding-left: 6%;">
                                                                        <asp:Label ID="lblUANNumber" runat="server" Text="MSME Number"></asp:Label>
                                                                    </td>
                                                                    <td style="width: 20%">
                                                                        <asp:FileUpload ID="fpUANNumber" runat="server" class="btn btn-primary" />
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator11" runat="server" Display="Dynamic" ControlToValidate="fpUANNumber" ErrorMessage="Please select a .pdf, .doc/.docx, .eps, .jpeg, .jpg, .png   file" ForeColor="Red" ValidationExpression="^([a-zA-Z].*|[1-9].*)\.(((p|P)(d|D)(f|F))|((d|D)(o|O)(c|C))|((d|D)(o|O)(c|C)(x|X))|((e|E)(p|P)(s|S))|((p|P)(n|N)(g|G))|((j|J)(p|P)(e|E)(g|G))|((j|J)(p|P)(g|G)))$"></asp:RegularExpressionValidator>
                                                                        <asp:CustomValidator ControlToValidate="fpUANNumber" ClientValidationFunction="uploadUAN" Display="Dynamic" ForeColor="Red" runat="server" ErrorMessage="File is too large, must select file under 1 Mb." ValidationGroup="vgFileUpload" />
                                                                    </td>
                                                                    <td style="padding-left: 30px; width: 10%;">
                                                                        <asp:Button ID="btnSaveUANNumber" OnClick="btnSaveUANNumber_Click" runat="server" class="btn btn-primary" Text="Save" /></td>
                                                                    <td>
                                                                        <asp:Label ID="lblSaveUANNumber" runat="server" Text="File Saved" ForeColor="Blue" Visible="false"></asp:Label></td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                        <div class="col-md-12 mb-3">
                                                            <table style="width: 100%">
                                                                <tr>
                                                                    <td style="width: 22%; padding-left: 6%;">
                                                                        <asp:Label ID="lblESIC" runat="server" Text="ESIC Certificate"></asp:Label>
                                                                    </td>
                                                                    <td style="width: 20%">
                                                                        <asp:FileUpload ID="fpESIC" runat="server" class="btn btn-primary" />
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" Display="Dynamic" ControlToValidate="fpESIC" ErrorMessage="Please select a .pdf, .doc/.docx, .eps, .jpeg, .jpg, .png   file" ForeColor="Red" ValidationExpression="^([a-zA-Z].*|[1-9].*)\.(((p|P)(d|D)(f|F))|((d|D)(o|O)(c|C))|((d|D)(o|O)(c|C)(x|X))|((e|E)(p|P)(s|S))|((p|P)(n|N)(g|G))|((j|J)(p|P)(e|E)(g|G))|((j|J)(p|P)(g|G)))$"></asp:RegularExpressionValidator>
                                                                        <asp:CustomValidator ControlToValidate="fpESIC" ClientValidationFunction="uploadESIC" Display="Dynamic" ForeColor="Red" runat="server" ErrorMessage="File is too large, must select file under 1 Mb." ValidationGroup="vgFileUpload" />
                                                                    </td>
                                                                    <td style="padding-left: 30px; width: 10%;">
                                                                        <asp:Button ID="btnSaveESIC" OnClick="btnSaveESIC_Click" runat="server" class="btn btn-primary" Text="Save" /></td>
                                                                    <td>
                                                                        <asp:Label ID="lblSaveESIC" runat="server" Text="File Saved" ForeColor="Blue" Visible="false"></asp:Label></td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                        <div class="col-md-12 mb-3">
                                                            <table style="width: 100%">
                                                                <tr>
                                                                    <td style="width: 22%; padding-left: 6%;">
                                                                        <asp:Label ID="lblVendorLogo" runat="server" Text="Company Logo"></asp:Label>
                                                                    </td>
                                                                    <td style="width: 20%">
                                                                        <asp:FileUpload ID="fpVendorLogo" runat="server" class="btn btn-primary" />
                                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" Display="Dynamic" ControlToValidate="fpVendorLogo" ErrorMessage="Please select a .jpeg, .jpg, .png, .bmp   file" ForeColor="Red" ValidationExpression="^([a-zA-Z].*|[1-9].*)\.(((p|P)(n|N)(g|G))|((j|J)(p|P)(e|E)(g|G))|((j|J)(p|P)(g|G))|((b|B)(m|M)(p|P)))$"></asp:RegularExpressionValidator>
                                                                        <asp:CustomValidator ControlToValidate="fpVendorLogo" ClientValidationFunction="uploadLogo" Display="Dynamic" ForeColor="Red" runat="server" ErrorMessage="File is too large, must select file under 1 Mb." ValidationGroup="vgFileUpload" />
                                                                    </td>
                                                                    <td style="padding-left: 30px; width: 10%;">
                                                                        <asp:Button ID="btnSaveVendorLogo" OnClick="btnSaveVendorLogo_Click" runat="server" class="btn btn-primary" Text="Save" /></td>
                                                                    <td>
                                                                        <asp:Label ID="lblSaveVendorLogo" runat="server" Text="File Saved" ForeColor="Blue" Visible="false"></asp:Label></td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </div>

                                                    <div class="form-row">
                                                        <asp:CheckBox ID="chkAccept" runat="server"
                                                            Text="I have read and agree to the website terms and conditions"
                                                            OnCheckedChanged="chkAccept_CheckedChanged" AutoPostBack="true" Visible="true" />
                                                    </div>

                                                    <div class="form-row">
                                                        <div class="col-md-11 mb-3">
                                                            &nbsp;
                                                        </div>
                                                        <%--<div class="col-md-1 mb-3" style="padding-left: 5px;">
                                                            <asp:Button ID="Button1" runat="server" class="btn btn-primary" Text="Previous"
                                                                OnClientClick="ShowLocation();return true;"
                                                                OnClick="btnPreviousFile_Click" />
                                                        </div>--%>
                                                        <div class="col-md-1 mb-3">
                                                            <asp:Button ID="btnFinish" OnClick="btnFinish_Click" Visible="true" Enabled="false" runat="server" class="btn btn-primary" Text="Finish" />
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>

                            </div>
                            <br />
                        </div>

                    </div>
                </div>
            </div>

        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            //// Save current tab in localStorage when clicked
            //$('a[data-toggle="tab"]').on('show.bs.tab', function (e) {
            //    localStorage.setItem('activeTab', $(e.target).attr('href'));
            //});

            //// Restore tab from localStorage
            //var activeTab = localStorage.getItem('activeTab');
            //if (activeTab) {
            //    $('#myTab1 a[href="' + activeTab + '"]').tab('show');
            //}


            //function setActiveTab(tabId) {
            //    $('#myTab1 a[href="#' + tabId + '"]').tab('show');
            //    localStorage.setItem('activeTab', '#' + tabId); // Update local storage to persist
            //}
            // function ShowVendor() {
            //   $('a[href="#home"]').tab('show');

            //// Save selected checkbox values
            //$("#btnSaveContact").click(function () {
            //    var vals = "";
            //    $(":checkbox").each(function () {
            //        if ($(this).is(":checked")) {
            //            var val = $(this).val();
            //            vals += val + ",";
            //        }
            //    });
            //    $("#hf_checkedvalues").val(vals);
            //});
            //});
            //function ShowVendor() {
            //    $('a[href="#home"]').tab('show');
            //}


            $("#btnSaveContact").click(function () {
                var vals = "";
                $(":checkbox").each(function () {
                    if ($(this).is(":checked")) {
                        vals += $(this).val() + ",";
                    }
                });
                $("#hf_checkedvalues").val(vals);
            });
        });



        function ShowBank() {
            document.getElementById('<%= btnSaveVendor.ClientID %>').click();

            <%--var x = $('#<%= hdnNextVendor.ClientID %>').val();
            if (x == 0) {
                $('a[href="#bank"]').tab('show');
            }--%>
        }



        function ShowContactPerson() {
            $('a[href="#person"]').tab('show');
        }

        //function ShowContact() {
        //    $('a[href="#contact"]').tab('show');
        //}

        function ShowLocation() {
            $('#alocation').removeClass('disabled');      // Enable the tab
            $('a[href="#location"]').tab('show');         // Activate it
        }
        function ShowFiles() {
            $('#afiles').removeClass('disabled');      // Enable the tab
            $('a[href="#files"]').tab('show');         // Activate it
        }


        // Tree View Checkbox Handling
        function OnTreeClick(evt) {
            var src = window.event != window.undefined ? window.event.srcElement : evt.target;
            var isChkBoxClick = (src.tagName.toLowerCase() === "input" && src.type === "checkbox");

            if (isChkBoxClick) {
                if (src.checked === true) {
                    var nodeText = getNextSibling(src).innerText || getNextSibling(src).innerHTML;
                    var nodeValue = GetNodeValue(getNextSibling(src));
                    document.getElementById("<%= hdnTree.ClientID %>").value += nodeText + ",";
                } else {
                    var nodeText = getNextSibling(src).innerText || getNextSibling(src).innerHTML;
                    var nodeValue = GetNodeValue(getNextSibling(src));
                    // Handle uncheck if needed
                }

                var parentTable = GetParentByTagName("table", src);
                var nxtSibling = parentTable.nextSibling;
                if (nxtSibling && nxtSibling.nodeType === 1 && nxtSibling.tagName.toLowerCase() === "div") {
                    CheckUncheckChildren(nxtSibling, src.checked);
                }

                CheckUncheckParents(src, src.checked);
            }
        }

        function CheckUncheckChildren(childContainer, check) {
            var childChkBoxes = childContainer.getElementsByTagName("input");
            var childChkBoxCount = childChkBoxes.length;
            for (var i = 0; i < childChkBoxCount; i++) {
                childChkBoxes[i].checked = check;
                document.getElementById("<%= hdnTree.ClientID %>").value += childChkBoxes[i].nextSibling.innerHTML + ",";
            }
        }

        function CheckUncheckParents(srcChild, check) {
            var parentDiv = GetParentByTagName("div", srcChild);
            var parentNodeTable = parentDiv.previousSibling;
            if (parentNodeTable) {
                var checkUncheckSwitch;
                if (check) {
                    var isAllSiblingsChecked = AreAllSiblingsChecked(srcChild);
                    if (!isAllSiblingsChecked) return;
                    checkUncheckSwitch = true;
                } else {
                    checkUncheckSwitch = false;
                }

                var inpElemsInParentTable = parentNodeTable.getElementsByTagName("input");
                if (inpElemsInParentTable.length > 0) {
                    var parentNodeChkBox = inpElemsInParentTable[0];
                    parentNodeChkBox.checked = checkUncheckSwitch;
                    CheckUncheckParents(parentNodeChkBox, checkUncheckSwitch);
                }
            }
        }

        function AreAllSiblingsChecked(chkBox) {
            var parentDiv = GetParentByTagName("div", chkBox);
            var childCount = parentDiv.childNodes.length;
            for (var i = 0; i < childCount; i++) {
                if (parentDiv.childNodes[i].nodeType === 1 && parentDiv.childNodes[i].tagName.toLowerCase() === "table") {
                    var prevChkBox = parentDiv.childNodes[i].getElementsByTagName("input")[0];
                    if (!prevChkBox.checked) {
                        return false;
                    }
                }
            }
            return true;
        }

        function GetParentByTagName(parentTagName, childElementObj) {
            var parent = childElementObj.parentNode;
            while (parent && parent.tagName.toLowerCase() !== parentTagName.toLowerCase()) {
                parent = parent.parentNode;
            }
            return parent;
        }

        function getNextSibling(element) {
            var n = element;
            do {
                n = n.nextSibling;
            } while (n && n.nodeType !== 1);
            return n;
        }

        function GetNodeValue(node) {
            var nodeValue = "";
            var nodePath = node.href.substring(node.href.indexOf(",") + 2, node.href.length - 2);
            var nodeValues = nodePath.split("\\");
            if (nodeValues.length > 1)
                nodeValue = nodeValues[nodeValues.length - 1];
            else
                nodeValue = nodeValues[0].substr(1);
            return nodeValue;
        }
    </script>

</asp:Content>








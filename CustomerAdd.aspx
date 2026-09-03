<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeBehind="CustomerAdd.aspx.cs" Inherits="LIMS.CustomerAdd" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Sweet Alert Css -->
    <link href="../../plugins/sweetalert/sweetalert.css" rel="stylesheet" />

    <!-- Custom Css -->
    <link href="../../css/style.css" rel="stylesheet">
    <script type="text/javascript">
        function add_() {
            document.getElementById('<%=testadd.ClientID%>').click();
        }

    </script>
    <style>
        .required:after {
            content: " *";
            color: red;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:Button ID="testadd" runat="server" Text="Button" OnClick="testadd_Click" Visible="false" />
    <!-- Basic Validation -->
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">

                    <h2>ADD CUSTOMER</h2>
                </div>
                <div class="body">
                    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                    <!-- Advanced Form Example With Validation -->

                    <!-- Basic Example | Vertical Layout -->
                    <div class="row clearfix">
                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                            <div class="card">
                                <div class="header">

                                    <h2>CUSTOMER DETAILS</h2>
                                    <br />
                                    <div id="divAlert" runat="server" visible="false">
                                        <asp:Label ID="lblMsg" runat="server" Visible="false"></asp:Label>
                                    </div>
                                </div>
                                <div class="body">
                                    <div id="wizard_vertical">
                                        <h2>Customer Details</h2>
                                        <section id="secCustomer" runat="server">
                                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                <ContentTemplate>
                                                    <fieldset>
                                                        <div class="row clearfix" id="divGroup" runat="server">
                                                            <div class="col-sm-4">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">
                                                                        <span style="color: red">*</span>
                                                                        <asp:DropDownList ID="ddlGroupName" runat="server" Width="90%" Font-Size="Small" class="form-control show-tick" AutoPostBack="true" OnSelectedIndexChanged="ddlGroupName_SelectedIndexChanged"></asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div id="divbtnAddGroup" runat="server" class="col-sm-3">
                                                                <div class="form-group form-float">
                                                                    <asp:Button ID="btnAddGroup" runat="server" class="btn bg-violet m-t-15 waves-effect" Text="Add New Group" OnClick="btnAddGroup_Click" />&nbsp;&nbsp;&nbsp;&nbsp;                                                                                                
                                                                </div>
                                                            </div>
                                                            <div id="divGroupName" runat="server" class="col-sm-3" visible="false">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtGroupName" runat="server" AutoComplete="off" CssClass="form-control" placeholder="Group Name"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div id="divGroupAddBtn" runat="server" class="col-sm-3">
                                                                <div class="form-group form-float">
                                                                    <asp:Button ID="btnAdd" runat="server" class="btn bg-violet m-t-15 waves-effect" Text="Add" OnClick="btnAdd_Click" Visible="false" />&nbsp;&nbsp;&nbsp;&nbsp;                                                                                                
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-4">
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <span style="color: red;">*</span>
                                                                        <asp:DropDownList ID="ddlDepartment" runat="server" Width="90%" class="form-control"></asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-6">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">
                                                                        <span style="color: red;">*</span>
                                                                        <asp:TextBox ID="txtCompanyName" runat="server" AutoComplete="off" CssClass="form-control" placeholder="Company Name"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-md-4">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtVAT" runat="server" AutoComplete="off" CssClass="form-control" MaxLength="16" placeholder="NPWP" Style="margin-top: 16px;"></asp:TextBox>
                                                                        <asp:TextBox ID="txtTradingName" runat="server" AutoComplete="off" CssClass="form-control" placeholder="Trading Name" Visible="false"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-6" style="padding-left: 35px;">

                                                                <asp:Label ID="lblMsg2" runat="server" CssClass="form-control" Visible="false"></asp:Label>

                                                            </div>
                                                        </div>

                                                        <div class="col-md-2">
                                                            <div class="form-group form-float">
                                                                <div class="center-block">
                                                                    <asp:Button ID="btnCustSave" runat="server" class="btn bg-violet m-t-15 waves-effect" Text=" + " OnClick="btnCustSave_Click" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </fieldset>
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="btnCustSave" EventName="Click" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </section>


                                        <h2>Location Details</h2>
                                        <section id="secLocation" runat="server">
                                            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                                <ContentTemplate>
                                                    <fieldset>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-4">
                                                                <asp:CheckBox ID="chkAddressSame" runat="server" Text="Both Same" OnCheckedChanged="chkAddressSame_CheckedChanged" AutoPostBack="true" />
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-6">
                                                                <asp:RadioButtonList ID="radLocationType" runat="server" OnSelectedIndexChanged="radLocationType_SelectedIndexChanged" AutoPostBack="true" RepeatDirection="Horizontal">
                                                                    <asp:ListItem Selected="True" Value="1">Invoice Address</asp:ListItem>
                                                                    <asp:ListItem Value="2">Send Address</asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </div>
                                                        </div>

                                                        <div class="row clearfix">
                                                            <div class="col-sm-4">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtLocLine1" runat="server" AutoComplete="off" CssClass="form-control" placeholder="Address Line-1"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtLocLine2" runat="server" AutoComplete="off" CssClass="form-control" placeholder="Address Line-2"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-4">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtCity" runat="server" AutoComplete="off" CssClass="form-control" placeholder="City"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-4">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtPincode" runat="server" AutoComplete="off" CssClass="form-control" placeholder="Postal Code"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix form-float">
                                                            <div class="col-sm-4">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">

                                                                        <asp:DropDownList ID="ddlCountry" runat="server" Width="90%" class="form-control show-tick">

                                                                            <asp:ListItem Value="">Select Country</asp:ListItem>

                                                                            <asp:ListItem Value="AF">Afghanistan</asp:ListItem>

                                                                            <asp:ListItem Value="AL">Albania</asp:ListItem>

                                                                            <asp:ListItem Value="DZ">Algeria</asp:ListItem>

                                                                            <asp:ListItem Value="AS">American Samoa</asp:ListItem>

                                                                            <asp:ListItem Value="AD">Andorra</asp:ListItem>

                                                                            <asp:ListItem Value="AO">Angola</asp:ListItem>

                                                                            <asp:ListItem Value="AI">Anguilla</asp:ListItem>

                                                                            <asp:ListItem Value="AQ">Antarctica</asp:ListItem>

                                                                            <asp:ListItem Value="AG">Antigua And Barbuda</asp:ListItem>

                                                                            <asp:ListItem Value="AR">Argentina</asp:ListItem>

                                                                            <asp:ListItem Value="AM">Armenia</asp:ListItem>

                                                                            <asp:ListItem Value="AW">Aruba</asp:ListItem>

                                                                            <asp:ListItem Value="AU">Australia</asp:ListItem>

                                                                            <asp:ListItem Value="AT">Austria</asp:ListItem>

                                                                            <asp:ListItem Value="AZ">Azerbaijan</asp:ListItem>

                                                                            <asp:ListItem Value="BS">Bahamas</asp:ListItem>

                                                                            <asp:ListItem Value="BH">Bahrain</asp:ListItem>

                                                                            <asp:ListItem Value="BD">Bangladesh</asp:ListItem>

                                                                            <asp:ListItem Value="BB">Barbados</asp:ListItem>

                                                                            <asp:ListItem Value="BY">Belarus</asp:ListItem>

                                                                            <asp:ListItem Value="BE">Belgium</asp:ListItem>

                                                                            <asp:ListItem Value="BZ">Belize</asp:ListItem>

                                                                            <asp:ListItem Value="BJ">Benin</asp:ListItem>

                                                                            <asp:ListItem Value="BM">Bermuda</asp:ListItem>

                                                                            <asp:ListItem Value="BT">Bhutan</asp:ListItem>

                                                                            <asp:ListItem Value="BO">Bolivia</asp:ListItem>

                                                                            <asp:ListItem Value="BA">Bosnia And Herzegowina</asp:ListItem>

                                                                            <asp:ListItem Value="BW">Botswana</asp:ListItem>

                                                                            <asp:ListItem Value="BV">Bouvet Island</asp:ListItem>

                                                                            <asp:ListItem Value="BR">Brazil</asp:ListItem>

                                                                            <asp:ListItem Value="IO">British Indian Ocean Territory</asp:ListItem>

                                                                            <asp:ListItem Value="BN">Brunei Darussalam</asp:ListItem>

                                                                            <asp:ListItem Value="BG">Bulgaria</asp:ListItem>

                                                                            <asp:ListItem Value="BF">Burkina Faso</asp:ListItem>

                                                                            <asp:ListItem Value="BI">Burundi</asp:ListItem>

                                                                            <asp:ListItem Value="KH">Cambodia</asp:ListItem>

                                                                            <asp:ListItem Value="CM">Cameroon</asp:ListItem>

                                                                            <asp:ListItem Value="CA">Canada</asp:ListItem>

                                                                            <asp:ListItem Value="CV">Cape Verde</asp:ListItem>

                                                                            <asp:ListItem Value="KY">Cayman Islands</asp:ListItem>

                                                                            <asp:ListItem Value="CF">Central African Republic</asp:ListItem>

                                                                            <asp:ListItem Value="TD">Chad</asp:ListItem>

                                                                            <asp:ListItem Value="CL">Chile</asp:ListItem>

                                                                            <asp:ListItem Value="CN">China</asp:ListItem>

                                                                            <asp:ListItem Value="CX">Christmas Island</asp:ListItem>

                                                                            <asp:ListItem Value="CC">Cocos (Keeling) Islands</asp:ListItem>

                                                                            <asp:ListItem Value="CO">Colombia</asp:ListItem>

                                                                            <asp:ListItem Value="KM">Comoros</asp:ListItem>

                                                                            <asp:ListItem Value="CG">Congo</asp:ListItem>

                                                                            <asp:ListItem Value="CK">Cook Islands</asp:ListItem>

                                                                            <asp:ListItem Value="CR">Costa Rica</asp:ListItem>

                                                                            <asp:ListItem Value="CI">Cote D'Ivoire</asp:ListItem>

                                                                            <asp:ListItem Value="HR">Croatia (Local Name: Hrvatska)</asp:ListItem>

                                                                            <asp:ListItem Value="CU">Cuba</asp:ListItem>

                                                                            <asp:ListItem Value="CY">Cyprus</asp:ListItem>

                                                                            <asp:ListItem Value="CZ">Czech Republic</asp:ListItem>

                                                                            <asp:ListItem Value="DK">Denmark</asp:ListItem>

                                                                            <asp:ListItem Value="DJ">Djibouti</asp:ListItem>

                                                                            <asp:ListItem Value="DM">Dominica</asp:ListItem>

                                                                            <asp:ListItem Value="DO">Dominican Republic</asp:ListItem>

                                                                            <asp:ListItem Value="TP">East Timor</asp:ListItem>

                                                                            <asp:ListItem Value="EC">Ecuador</asp:ListItem>

                                                                            <asp:ListItem Value="EG">Egypt</asp:ListItem>

                                                                            <asp:ListItem Value="SV">El Salvador</asp:ListItem>

                                                                            <asp:ListItem Value="GQ">Equatorial Guinea</asp:ListItem>

                                                                            <asp:ListItem Value="ER">Eritrea</asp:ListItem>

                                                                            <asp:ListItem Value="EE">Estonia</asp:ListItem>

                                                                            <asp:ListItem Value="ET">Ethiopia</asp:ListItem>

                                                                            <asp:ListItem Value="FK">Falkland Islands (Malvinas)</asp:ListItem>

                                                                            <asp:ListItem Value="FO">Faroe Islands</asp:ListItem>

                                                                            <asp:ListItem Value="FJ">Fiji</asp:ListItem>

                                                                            <asp:ListItem Value="FI">Finland</asp:ListItem>

                                                                            <asp:ListItem Value="FR">France</asp:ListItem>

                                                                            <asp:ListItem Value="GF">French Guiana</asp:ListItem>

                                                                            <asp:ListItem Value="PF">French Polynesia</asp:ListItem>

                                                                            <asp:ListItem Value="TF">French Southern Territories</asp:ListItem>

                                                                            <asp:ListItem Value="GA">Gabon</asp:ListItem>

                                                                            <asp:ListItem Value="GM">Gambia</asp:ListItem>

                                                                            <asp:ListItem Value="GE">Georgia</asp:ListItem>

                                                                            <asp:ListItem Value="DE">Germany</asp:ListItem>

                                                                            <asp:ListItem Value="GH">Ghana</asp:ListItem>

                                                                            <asp:ListItem Value="GI">Gibraltar</asp:ListItem>

                                                                            <asp:ListItem Value="GR">Greece</asp:ListItem>

                                                                            <asp:ListItem Value="GL">Greenland</asp:ListItem>

                                                                            <asp:ListItem Value="GD">Grenada</asp:ListItem>

                                                                            <asp:ListItem Value="GP">Guadeloupe</asp:ListItem>

                                                                            <asp:ListItem Value="GU">Guam</asp:ListItem>

                                                                            <asp:ListItem Value="GT">Guatemala</asp:ListItem>

                                                                            <asp:ListItem Value="GN">Guinea</asp:ListItem>

                                                                            <asp:ListItem Value="GW">Guinea-Bissau</asp:ListItem>

                                                                            <asp:ListItem Value="GY">Guyana</asp:ListItem>

                                                                            <asp:ListItem Value="HT">Haiti</asp:ListItem>

                                                                            <asp:ListItem Value="HM">Heard And Mc Donald Islands</asp:ListItem>

                                                                            <asp:ListItem Value="VA">Holy See (Vatican City State)</asp:ListItem>

                                                                            <asp:ListItem Value="HN">Honduras</asp:ListItem>

                                                                            <asp:ListItem Value="HK">Hong Kong</asp:ListItem>

                                                                            <asp:ListItem Value="HU">Hungary</asp:ListItem>

                                                                            <asp:ListItem Value="IS">Icel And</asp:ListItem>

                                                                            <asp:ListItem Value="INDIA">India</asp:ListItem>

                                                                            <asp:ListItem Value="ID" Selected="True">Indonesia</asp:ListItem>

                                                                            <asp:ListItem Value="IR">Iran (Islamic Republic Of)</asp:ListItem>

                                                                            <asp:ListItem Value="IQ">Iraq</asp:ListItem>

                                                                            <asp:ListItem Value="IE">Ireland</asp:ListItem>

                                                                            <asp:ListItem Value="IL">Israel</asp:ListItem>

                                                                            <asp:ListItem Value="IT">Italy</asp:ListItem>

                                                                            <asp:ListItem Value="JM">Jamaica</asp:ListItem>

                                                                            <asp:ListItem Value="JP">Japan</asp:ListItem>

                                                                            <asp:ListItem Value="JO">Jordan</asp:ListItem>

                                                                            <asp:ListItem Value="KZ">Kazakhstan</asp:ListItem>

                                                                            <asp:ListItem Value="KE">Kenya</asp:ListItem>

                                                                            <asp:ListItem Value="KI">Kiribati</asp:ListItem>

                                                                            <asp:ListItem Value="KP">Korea, Dem People'S Republic</asp:ListItem>

                                                                            <asp:ListItem Value="KR">Korea, Republic Of</asp:ListItem>

                                                                            <asp:ListItem Value="KW">Kuwait</asp:ListItem>

                                                                            <asp:ListItem Value="KG">Kyrgyzstan</asp:ListItem>

                                                                            <asp:ListItem Value="LA">Lao People'S Dem Republic</asp:ListItem>

                                                                            <asp:ListItem Value="LV">Latvia</asp:ListItem>

                                                                            <asp:ListItem Value="LB">Lebanon</asp:ListItem>

                                                                            <asp:ListItem Value="LS">Lesotho</asp:ListItem>

                                                                            <asp:ListItem Value="LR">Liberia</asp:ListItem>

                                                                            <asp:ListItem Value="LY">Libyan Arab Jamahiriya</asp:ListItem>

                                                                            <asp:ListItem Value="LI">Liechtenstein</asp:ListItem>

                                                                            <asp:ListItem Value="LT">Lithuania</asp:ListItem>

                                                                            <asp:ListItem Value="LU">Luxembourg</asp:ListItem>

                                                                            <asp:ListItem Value="MO">Macau</asp:ListItem>

                                                                            <asp:ListItem Value="MK">Macedonia</asp:ListItem>

                                                                            <asp:ListItem Value="MG">Madagascar</asp:ListItem>

                                                                            <asp:ListItem Value="MW">Malawi</asp:ListItem>

                                                                            <asp:ListItem Value="MY">Malaysia</asp:ListItem>

                                                                            <asp:ListItem Value="MV">Maldives</asp:ListItem>

                                                                            <asp:ListItem Value="ML">Mali</asp:ListItem>

                                                                            <asp:ListItem Value="MT">Malta</asp:ListItem>

                                                                            <asp:ListItem Value="MH">Marshall Islands</asp:ListItem>

                                                                            <asp:ListItem Value="MQ">Martinique</asp:ListItem>

                                                                            <asp:ListItem Value="MR">Mauritania</asp:ListItem>

                                                                            <asp:ListItem Value="MU">Mauritius</asp:ListItem>

                                                                            <asp:ListItem Value="YT">Mayotte</asp:ListItem>

                                                                            <asp:ListItem Value="MX">Mexico</asp:ListItem>

                                                                            <asp:ListItem Value="FM">Micronesia, Federated States</asp:ListItem>

                                                                            <asp:ListItem Value="MD">Moldova, Republic Of</asp:ListItem>

                                                                            <asp:ListItem Value="MC">Monaco</asp:ListItem>

                                                                            <asp:ListItem Value="MN">Mongolia</asp:ListItem>

                                                                            <asp:ListItem Value="MS">Montserrat</asp:ListItem>

                                                                            <asp:ListItem Value="MA">Morocco</asp:ListItem>

                                                                            <asp:ListItem Value="MZ">Mozambique</asp:ListItem>

                                                                            <asp:ListItem Value="MM">Myanmar</asp:ListItem>

                                                                            <asp:ListItem Value="NA">Namibia</asp:ListItem>

                                                                            <asp:ListItem Value="NR">Nauru</asp:ListItem>

                                                                            <asp:ListItem Value="NP">Nepal</asp:ListItem>

                                                                            <asp:ListItem Value="NL">Netherlands</asp:ListItem>

                                                                            <asp:ListItem Value="AN">Netherlands Ant Illes</asp:ListItem>

                                                                            <asp:ListItem Value="NC">New Caledonia</asp:ListItem>

                                                                            <asp:ListItem Value="NZ">New Zealand</asp:ListItem>

                                                                            <asp:ListItem Value="NI">Nicaragua</asp:ListItem>

                                                                            <asp:ListItem Value="NE">Niger</asp:ListItem>

                                                                            <asp:ListItem Value="NG">Nigeria</asp:ListItem>

                                                                            <asp:ListItem Value="NU">Niue</asp:ListItem>

                                                                            <asp:ListItem Value="NF">Norfolk Island</asp:ListItem>

                                                                            <asp:ListItem Value="MP">Northern Mariana Islands</asp:ListItem>

                                                                            <asp:ListItem Value="NO">Norway</asp:ListItem>

                                                                            <asp:ListItem Value="OM">Oman</asp:ListItem>

                                                                            <asp:ListItem Value="PK">Pakistan</asp:ListItem>

                                                                            <asp:ListItem Value="PW">Palau</asp:ListItem>

                                                                            <asp:ListItem Value="PA">Panama</asp:ListItem>

                                                                            <asp:ListItem Value="PG">Papua New Guinea</asp:ListItem>

                                                                            <asp:ListItem Value="PY">Paraguay</asp:ListItem>

                                                                            <asp:ListItem Value="PE">Peru</asp:ListItem>

                                                                            <asp:ListItem Value="PH">Philippines</asp:ListItem>

                                                                            <asp:ListItem Value="PN">Pitcairn</asp:ListItem>

                                                                            <asp:ListItem Value="PL">Poland</asp:ListItem>

                                                                            <asp:ListItem Value="PT">Portugal</asp:ListItem>

                                                                            <asp:ListItem Value="PR">Puerto Rico</asp:ListItem>

                                                                            <asp:ListItem Value="QA">Qatar</asp:ListItem>

                                                                            <asp:ListItem Value="RE">Reunion</asp:ListItem>

                                                                            <asp:ListItem Value="RO">Romania</asp:ListItem>

                                                                            <asp:ListItem Value="RU">Russian Federation</asp:ListItem>

                                                                            <asp:ListItem Value="RW">Rwanda</asp:ListItem>

                                                                            <asp:ListItem Value="KN">Saint K Itts And Nevis</asp:ListItem>

                                                                            <asp:ListItem Value="LC">Saint Lucia</asp:ListItem>

                                                                            <asp:ListItem Value="VC">Saint Vincent, The Grenadines</asp:ListItem>

                                                                            <asp:ListItem Value="WS">Samoa</asp:ListItem>

                                                                            <asp:ListItem Value="SM">San Marino</asp:ListItem>

                                                                            <asp:ListItem Value="ST">Sao Tome And Principe</asp:ListItem>

                                                                            <asp:ListItem Value="SA">Saudi Arabia</asp:ListItem>

                                                                            <asp:ListItem Value="SN">Senegal</asp:ListItem>

                                                                            <asp:ListItem Value="SC">Seychelles</asp:ListItem>

                                                                            <asp:ListItem Value="SL">Sierra Leone</asp:ListItem>

                                                                            <asp:ListItem Value="SG">Singapore</asp:ListItem>

                                                                            <asp:ListItem Value="SK">Slovakia (Slovak Republic)</asp:ListItem>

                                                                            <asp:ListItem Value="SI">Slovenia</asp:ListItem>

                                                                            <asp:ListItem Value="SB">Solomon Islands</asp:ListItem>

                                                                            <asp:ListItem Value="SO">Somalia</asp:ListItem>

                                                                            <asp:ListItem Value="ZA">South Africa</asp:ListItem>

                                                                            <asp:ListItem Value="GS">South Georgia , S Sandwich Is.</asp:ListItem>

                                                                            <asp:ListItem Value="ES">Spain</asp:ListItem>

                                                                            <asp:ListItem Value="LK">Sri Lanka</asp:ListItem>

                                                                            <asp:ListItem Value="SH">St. Helena</asp:ListItem>

                                                                            <asp:ListItem Value="PM">St. Pierre And Miquelon</asp:ListItem>

                                                                            <asp:ListItem Value="SD">Sudan</asp:ListItem>

                                                                            <asp:ListItem Value="SR">Suriname</asp:ListItem>

                                                                            <asp:ListItem Value="SJ">Svalbard, Jan Mayen Islands</asp:ListItem>

                                                                            <asp:ListItem Value="SZ">Sw Aziland</asp:ListItem>

                                                                            <asp:ListItem Value="SE">Sweden</asp:ListItem>

                                                                            <asp:ListItem Value="CH">Switzerland</asp:ListItem>

                                                                            <asp:ListItem Value="SY">Syrian Arab Republic</asp:ListItem>

                                                                            <asp:ListItem Value="TW">Taiwan</asp:ListItem>

                                                                            <asp:ListItem Value="TJ">Tajikistan</asp:ListItem>

                                                                            <asp:ListItem Value="TZ">Tanzania, United Republic Of</asp:ListItem>

                                                                            <asp:ListItem Value="TH">Thailand</asp:ListItem>

                                                                            <asp:ListItem Value="TG">Togo</asp:ListItem>

                                                                            <asp:ListItem Value="TK">Tokelau</asp:ListItem>

                                                                            <asp:ListItem Value="TO">Tonga</asp:ListItem>

                                                                            <asp:ListItem Value="TT">Trinidad And Tobago</asp:ListItem>

                                                                            <asp:ListItem Value="TN">Tunisia</asp:ListItem>

                                                                            <asp:ListItem Value="TR">Turkey</asp:ListItem>

                                                                            <asp:ListItem Value="TM">Turkmenistan</asp:ListItem>

                                                                            <asp:ListItem Value="TC">Turks And Caicos Islands</asp:ListItem>

                                                                            <asp:ListItem Value="TV">Tuvalu</asp:ListItem>

                                                                            <asp:ListItem Value="UG">Uganda</asp:ListItem>

                                                                            <asp:ListItem Value="UA">Ukraine</asp:ListItem>

                                                                            <asp:ListItem Value="AE">United Arab Emirates</asp:ListItem>

                                                                            <asp:ListItem Value="GB">United Kingdom</asp:ListItem>

                                                                            <asp:ListItem Value="US">United States</asp:ListItem>

                                                                            <asp:ListItem Value="UM">United States Minor Is.</asp:ListItem>

                                                                            <asp:ListItem Value="UY">Uruguay</asp:ListItem>

                                                                            <asp:ListItem Value="UZ">Uzbekistan</asp:ListItem>

                                                                            <asp:ListItem Value="VU">Vanuatu</asp:ListItem>

                                                                            <asp:ListItem Value="VE">Venezuela</asp:ListItem>

                                                                            <asp:ListItem Value="VN">Viet Nam</asp:ListItem>

                                                                            <asp:ListItem Value="VG">Virgin Islands (British)</asp:ListItem>

                                                                            <asp:ListItem Value="VI">Virgin Islands (U.S.)</asp:ListItem>

                                                                            <asp:ListItem Value="WF">Wallis And Futuna Islands</asp:ListItem>

                                                                            <asp:ListItem Value="EH">Western Sahara</asp:ListItem>

                                                                            <asp:ListItem Value="YE">Yemen</asp:ListItem>

                                                                            <asp:ListItem Value="YU">Yugoslavia</asp:ListItem>

                                                                            <asp:ListItem Value="ZR">Zaire</asp:ListItem>

                                                                            <asp:ListItem Value="ZM">Zambia</asp:ListItem>

                                                                            <asp:ListItem Value="ZW">Zimbabwe</asp:ListItem>

                                                                        </asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                            </div>
                    <div class="col-sm-1"></div>
                    <div class="col-sm-1">
                        <asp:Button ID="btnAddToGrid" runat="server" class="btn bg-violet m-t-15 waves-effect" Text=" + " OnClick="btnlocation_Click" />
                    </div>
                </div>

                <asp:GridView ID="gvlocation" runat="server" AutoGenerateColumns="False" 
                    class="table table-bordered table-striped table-hover js-basic-example dataTable" 
                    BorderWidth="1px" CellPadding="4" Width="100%" 
                    OnRowCommand="gvlocation_RowCommand">
                    <Columns>
                        <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="5%">
                            <ItemTemplate>
                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="LocationType" HeaderText="Address Type" />
                        <asp:BoundField DataField="AddressLine1" HeaderText="Address Line 1" />
                        <asp:BoundField DataField="AddressLine2" HeaderText="Address Line 2" />
                        <asp:BoundField DataField="City" HeaderText="City" />
                        <asp:BoundField DataField="PostalCode" HeaderText="Postal Code" />
                        <asp:BoundField DataField="Country" HeaderText="Country" />
                        <asp:ButtonField ButtonType="Image" ImageUrl="~/assets/img/del.ico" 
                            ControlStyle-Width="15px" CommandName="del" />
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
                
                <div class="row clearfix">
                    <div class="col-md-6">
                        <div class="form-group form-float">
                            <asp:Button ID="btnLocSave" runat="server" class="btn bg-violet m-t-15 waves-effect" Text="Save" OnClick="btnLocSave_Click" Visible="false" />&nbsp;&nbsp;&nbsp;&nbsp;
                        </div>
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnAddToGrid" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnLocSave" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
</section>

                                        <h2>Contact Person</h2>
                                        <section id="secPerson" runat="server">
                                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                <ContentTemplate>
                                                    <fieldset>
                                                        <div class="row clearfix" id="div1" runat="server">
                                                            <div class="col-sm-2">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">
                                                                        <%--<asp:TextBox ID="txtCustomerTitle" runat="server" CssClass="form-control" placeholder="Title"></asp:TextBox>--%>
                                                                        <asp:DropDownList ID="ddlPersonTitle" runat="server" class="form-control show-tick">

                                                                            <asp:ListItem>Select Title</asp:ListItem>
                                                                            <asp:ListItem Value="Mr">Mr</asp:ListItem>
                                                                            <asp:ListItem Value="Mrs">Mrs</asp:ListItem>
                                                                            <asp:ListItem Value="Miss">Miss</asp:ListItem>
                                                                            <asp:ListItem Value="Ms">Ms</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-3">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtPersonFirstName" runat="server" AutoComplete="off" CssClass="form-control" placeholder="First Name"></asp:TextBox>
                                                                        <asp:TextBox ID="txtPersonMiddleName" runat="server" AutoComplete="off" CssClass="form-control" placeholder="Middle Name" Visible="false"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-3">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtPersonLastName" runat="server" AutoComplete="off" CssClass="form-control" placeholder="Last Name"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                        </div>
                                                        <div class="row clearfix" id="div2" runat="server">
                                                            <div class="col-sm-3">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtDesignation" runat="server" AutoComplete="off" CssClass="form-control" placeholder="Position"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-3">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtDepartment" runat="server" AutoComplete="off" CssClass="form-control" placeholder="Department"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-3">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">
                                                                        <%--<asp:TextBox ID="txtCustomerTitle" runat="server" CssClass="form-control" placeholder="Title"></asp:TextBox>--%>
                                                                        <asp:DropDownList ID="ddlPersonAccountType" runat="server" class="form-control show-tick">
                                                                            <asp:ListItem Value="0">Select Type</asp:ListItem>
                                                                            <asp:ListItem Value="1">Primary</asp:ListItem>
                                                                            <asp:ListItem Value="2">Secondary</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-1"></div>

                                                            <div class="col-sm-1">

                                                                <asp:Button ID="btnAddPerson" runat="server" class="btn bg-violet m-t-15 waves-effect" Text=" + " OnClick="btnAddPerson_Click" />
                                                            </div>
                                                        </div>


                                                        <asp:GridView ID="gvPerson" runat="server" AutoGenerateColumns="False" class="table table-bordered table-striped table-hover js-basic-example dataTable" BorderWidth="1px" CellPadding="4" Width="100%" OnRowCommand="gvPerson_RowCommand">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="100%">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="5%"></ItemStyle>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="Title" HeaderText="Title" />
                                                                <asp:BoundField DataField="FirstName" HeaderText="First Name" />
                                                                <asp:BoundField DataField="MiddleName" HeaderText="Middle Name" />
                                                                <asp:BoundField DataField="LastName" HeaderText="Last Name" />
                                                                <asp:BoundField DataField="Designation" HeaderText="Designation" />
                                                                <asp:BoundField DataField="Department" HeaderText="Department" />
                                                                <asp:BoundField DataField="PersonAccountType" HeaderText="Account Type" />
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
                                                        <div class="row clearfix">
                                                            <div class="col-md-6">
                                                                <div class="form-group form-float">
                                                                    <asp:Button ID="btnPersonSave" runat="server" class="btn bg-violet m-t-15 waves-effect" Text="Save" OnClick="btnPersonSave_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </fieldset>
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="btnPersonSave" EventName="Click" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </section>

                                        <h2>Contact Details</h2>
                                        <section id="secContact" runat="server">
                                            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                                <ContentTemplate>
                                                    <fieldset>
                                                        <div class="row clearfix">
                                                            <div class="col-sm-3">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">
                                                                        <asp:DropDownList ID="ddlContactPerson" runat="server" Width="90%" class="form-control show-tick" AutoPostBack="true" OnSelectedIndexChanged="ddlContactPerson_SelectedIndexChanged"></asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-3">
                                                                <asp:Label ID="lblContactExists" runat="server" class="error" for="name" Visible="false">Contact Details Exists.</asp:Label>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-md-4">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtMobile" runat="server" AutoComplete="off" CssClass="form-control" MaxLength="15" placeholder="Mobile"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-md-4">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtEmail" runat="server" AutoComplete="off" CssClass="form-control" placeholder="Email"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="col-md-4">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtPhone" runat="server" AutoComplete="off" CssClass="form-control" placeholder="Phone"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-md-4">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtFax" runat="server" AutoComplete="off" CssClass="form-control" placeholder="Fax"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-md-2" style="text-align: center">
                                                                <asp:Button ID="btnAddContact" runat="server" class="btn bg-violet m-t-15 waves-effect" Text=" + " OnClick="btnAddContact_Click" />
                                                            </div>
                                                        </div>


                                                        <asp:GridView ID="gvContact" runat="server" AutoGenerateColumns="False" class="table table-bordered table-striped table-hover js-basic-example dataTable" BorderWidth="1px" CellPadding="4" Width="100%" OnRowCommand="gvContact_RowCommand">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="100">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="100px"></ItemStyle>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="ContactPerson" HeaderText="Contact Person" />
                                                                <asp:BoundField DataField="ContactMobile" HeaderText="Mobile" />
                                                                <asp:BoundField DataField="ContactEmail" HeaderText="Email" />
                                                                <asp:BoundField DataField="ContactPhone" HeaderText="Phone" />
                                                                <asp:BoundField DataField="ContactFax" HeaderText="Fax" />
                                                                <asp:ButtonField ButtonType="Image" ImageUrl="~/assets/img/del.ico" ControlStyle-Width="15px" CommandName="del" HeaderText="Delete" />
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
                                                        <div class="row clearfix">
                                                            <div class="col-md-6">
                                                                <div class="form-group form-float">
                                                                    <asp:Button ID="btnContactSave" runat="server" class="btn bg-violet m-t-15 waves-effect" Text="Save" OnClick="btnContactSave_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </fieldset>
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="btnContactSave" EventName="Click" />
                                                </Triggers>
                                            </asp:UpdatePanel>

                                        </section>
                                        <div id="divError" runat="server" visible="false" class="alert alert-danger">
                                            <strong>Data Not Saved!</strong>
                                            <asp:Label ID="Label1" runat="server"></asp:Label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- #END# Basic Example | Vertical Layout -->
                </div>
            </div>
        </div>
    </div>


    <%-- <div class="loading" align="center">
        Loading. Please wait.<br />
        <br />
        <img src="images/loader.gif" alt="" />
    </div>--%>

    <script>
        function openTab(evt, tabName) {
            var i, tabcontent, tablinks;
            tabcontent = document.getElementsByClassName("tabcontent");
            for (i = 0; i < tabcontent.length; i++) {
                tabcontent[i].style.display = "none";
            }
            tablinks = document.getElementsByClassName("tablinks");
            for (i = 0; i < tablinks.length; i++) {
                tablinks[i].className = tablinks[i].className.replace(" active", "");
            }
            document.getElementById(tabName).style.display = "block";
            evt.currentTarget.className += " active";
        }

    </script>
</asp:Content>

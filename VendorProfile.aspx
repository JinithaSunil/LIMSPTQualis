<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="VendorProfile.aspx.cs" Inherits="LIMS.VendorProfile" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            var tab = document.getElementById('<%= hdnTAB.ClientID%>').value;
            $('#myTab1 a[href="' + tab + '"]').tab('show');
        });
    </script>
    <style>
/* Remove default spacing */
.aspNetTreeView table {
    border-collapse: collapse;
}

.aspNetTreeView td {
    padding: 0 !important;
}

/* Reduce space between expand icon and checkbox */
.aspNetTreeView input[type="checkbox"] {
    margin-left: -11px !important;
    margin-right: 4px !important;
    vertical-align: middle;
}

/* Align expand (+) icon */
.aspNetTreeView img {
    margin-right: 2px !important;
    vertical-align: middle;
}
/* Fix indentation alignment */
.aspNetTreeView span {
    margin-left: 2px !important;
}
       

        .tab-content > .tab-pane {
            display: none;
            position: absolute;
            width: 100%;
        }

            .tab-content > .tab-pane.active {
                display: block;
                position: relative;
            }

        .card .body {
            padding: 40px;
            color: #555;
        }

        body {
            font-family: Arial, sans-serif;
            background-color: #f5f5f5;
            padding: 20px;
        }

        .body {
            background: white;
            border-radius: 8px;
            padding: 20px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }

        .nav-tabs {
            border-bottom: 2px solid #dee2e6;
            margin-bottom: 20px;
        }

            .nav-tabs .nav-link {
                color: #495057;
                border: none;
                padding: 12px 20px;
                transition: all 0.3s;
            }

                .nav-tabs .nav-link:hover {
                    border: none;
                    color: #007bff;
                }

                .nav-tabs .nav-link.active {
                    color: #007bff;
                    border: none;
                    border-bottom: 3px solid #007bff;
                    background: transparent;
                }

                .nav-tabs .nav-link i {
                    margin-right: 5px;
                }

        .uprofile-image {
            text-align: center;
            margin-bottom: 20px;
        }

            .uprofile-image img {
                max-width: 200px;
                border-radius: 8px;
            }

        .uprofile-name {
            text-align: center;
            margin-bottom: 30px;
        }

            .uprofile-name h3 {
                font-size: 24px;
                font-weight: 600;
                margin-bottom: 5px;
            }

        .uprofile-status {
            display: inline-block;
            width: 10px;
            height: 10px;
            border-radius: 50%;
            margin-left: 8px;
        }

            .uprofile-status.online {
                background-color: #28a745;
            }

        .uprofile-title {
            color: #6c757d;
            font-size: 16px;
        }

        .tab-content {
            padding: 20px 0;
        }

        .border.rounded {
            border: 1px solid #dee2e6 !important;
        }

        .bg-light {
            background-color: #f8f9fa !important;
        }

        .form-label {
            margin-bottom: 5px;
            font-size: 14px;
        }

        .fw-bold {
            font-weight: 600 !important;
        }

        .form-control-plaintext {
            padding: 5px 0;
            color: #495057;
        }

        .btn-outline-primary {
            color: #007bff;
            border-color: #007bff;
        }

            .btn-outline-primary:hover {
                background-color: #007bff;
                color: white;
            }

        .list-unstyled {
            padding: 20px;
            background: #f8f9fa;
            border-radius: 8px;
            margin-bottom: 20px;
        }

            .list-unstyled li {
                margin-bottom: 15px;
                color: #495057;
            }

                .list-unstyled li i {
                    margin-right: 8px;
                    color: #007bff;
                }

        .form-control {
            margin-bottom: 10px;
        }

        #divDocuments, #divEditDocuments {
            color: darkblue;
        }

        .expand {
            padding: 20px;
            background: #f8f9fa;
            margin-top: 10px;
            border-radius: 8px;
        }

        .btn-primary {
            background-color: #007bff;
            border-color: #007bff;
        }

        .btn-sm {
            padding: 5px 15px;
            font-size: 14px;
        }

        .text-end {
            text-align: right;
        }
    </style>

    <script type="text/javascript" src="//netdna.bootstrapcdn.com/bootstrap/3.0.0/js/bootstrap.min.js"></script>
    <script type="text/javascript">

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

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:HiddenField ID="hdnTAB" runat="server" Value="#home" />
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <h2>Vendor Details</h2>
                </div>
                <div class="body">
                    <div class="body">
                        <div class="row">
                            <div class="col-lg-12">
                                <ul class="nav nav-tabs" id="myTab1" role="tablist">
                                    <li class="nav-item">
                                        <a class="nav-link active" href="#home" role="tab" data-toggle="tab">
                                            <i class="fa fa-building-o"></i>Profile
                                        </a>
                                    </li>
                                    <li class="nav-item">
                                        <a class="nav-link" role="tab" href="#address" data-toggle="tab">
                                            <i class="fa fa-newspaper-o"></i>Address 
                                        </a>
                                    </li>
                                    <li class="nav-item">
                                        <a class="nav-link" role="tab" href="#contact" data-toggle="tab">
                                            <i class="fa fa-user"></i>Contact
                                        </a>
                                    </li>
                                    <li class="nav-item">
                                        <a class="nav-link" role="tab" href="#documents" data-toggle="tab">
                                            <i class="fa fa-file-text"></i>Documents
                                        </a>
                                    </li>
                                    <li class="nav-item">
                                        <a class="nav-link" role="tab" href="#categories" data-toggle="tab">
                                            <i class="fa fa-file-text"></i>Categories
                                        </a>
                                    </li>
                                </ul>

                                <div class="tab-content" id="myTabContent5">
                                    <div class="tab-pane fade show active" id="home">
                                        <div class="row">
                                            <div class="col-lg-12 col-md-12 col-12">
                                                <div class="uprofile-image">
                                                    <img src="assets/img/qualis-2.png" class="img-fluid" />
                                                </div>
                                                <div class="uprofile-name">
                                                    <h3>
                                                        <a href="#">
                                                            <asp:Label ID="lblVendorName" runat="server"></asp:Label></a>
                                                        <span class="uprofile-status online"></span>
                                                    </h3>
                                                    <p class="uprofile-title">
                                                        <asp:Label ID="lblBusinessType" runat="server"></asp:Label>
                                                    </p>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="tab-pane fade" id="address">
                                        <div class="row">
                                            <div>
                                                <asp:ListView ID="liAddress" runat="server" OnItemDataBound="liAddress_ItemDataBound"
                                                    OnItemEditing="liAddress_ItemEditing" OnItemUpdating="liAddress_ItemUpdating"
                                                    OnItemCanceling="liAddress_ItemCanceling">
                                                    <ItemTemplate>
                                                        <div class="border rounded p-3 mb-3 bg-light">
                                                            <div class="row g-2">
                                                                <!-- Contact Person -->
                                                                <div class="col-md-6 mb-2">
                                                                    <label class="form-label fw-bold">Contact Person:</label>
                                                                    <div>
                                                                        <asp:Label ID="lblVendorContactPerson" runat="server" CssClass="form-control-plaintext"
                                                                            Text='<%# Eval("Title") + " " + Eval("FirstName") + " " + Eval("LastName") %>' />
                                                                    </div>
                                                                </div>

                                                                <!-- Address Type -->
                                                                <div class="col-md-6 mb-2">
                                                                    <label class="form-label fw-bold">Address Type:</label>
                                                                    <div>
                                                                        <asp:Label ID="lblAddressType" runat="server" CssClass="form-control-plaintext"
                                                                            Text='<%# Eval("LocationType") %>' />
                                                                    </div>
                                                                </div>

                                                                <!-- Address -->
                                                                <div class="col-md-12 mb-2">
                                                                    <label class="form-label fw-bold">Address:</label>
                                                                    <div>
                                                                        <asp:Label ID="lblAddress" runat="server" CssClass="form-control-plaintext"
                                                                            Text='<%# Eval("AddressLine1") + " " + Eval("AddressLine2") + " " + Eval("AddressLine3") %>' />
                                                                    </div>
                                                                </div>

                                                                <!-- City -->
                                                                <div class="col-md-6 mb-2">
                                                                    <label class="form-label fw-bold">City:</label>
                                                                    <div>
                                                                        <asp:Label ID="lblCity" runat="server" CssClass="form-control-plaintext"
                                                                            Text='<%# Eval("City") %>' />
                                                                    </div>
                                                                </div>

                                                                <!-- State -->
                                                                <div class="col-md-6 mb-2">
                                                                    <label class="form-label fw-bold">State:</label>
                                                                    <div>
                                                                        <asp:Label ID="lblState" runat="server" CssClass="form-control-plaintext"
                                                                            Text='<%# Eval("State") %>' />
                                                                    </div>
                                                                </div>

                                                                <!-- Country -->
                                                                <div class="col-md-6 mb-2">
                                                                    <label class="form-label fw-bold">Country:</label>
                                                                    <div>
                                                                        <asp:Label ID="lblCountry" runat="server" CssClass="form-control-plaintext"
                                                                            Text='<%# Eval("Country") %>' />
                                                                    </div>
                                                                </div>

                                                                <!-- Pincode -->
                                                                <div class="col-md-6 mb-2">
                                                                    <label class="form-label fw-bold">PinCode:</label>
                                                                    <div>
                                                                        <asp:Label ID="Label2" runat="server" CssClass="form-control-plaintext"
                                                                            Text='<%# Eval("PinCode") %>' />
                                                                    </div>
                                                                </div>

                                                                <!-- Edit Button -->
                                                                <div class="col-md-12 text-end">
                                                                    <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" CssClass="btn btn-outline-primary btn-sm">
                                                        <img src="assets/img/Edit.png" width="20px" class="me-1" />Edit
                                                                    </asp:LinkButton>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </ItemTemplate>

                                                    <EditItemTemplate>
                                                        <ul class="list-unstyled">
                                                            <li><i class='fa fa-home'></i>
                                                                <b>Contact Person: </b>
                                                                <asp:DropDownList ID="ddlContactPerson" runat="server" CssClass="form-control"></asp:DropDownList><br />
                                                                <b>Address Type: </b>
                                                                <asp:DropDownList ID="ddlAddressType" runat="server" CssClass="form-control">
                                                                    <asp:ListItem>Registered Office</asp:ListItem>
                                                                    <asp:ListItem>Manufacturing Office</asp:ListItem>
                                                                </asp:DropDownList><br />
                                                                <b>Address:</b>
                            <asp:TextBox ID="txtAddress1" runat="server" CssClass="form-control" Text='<%# Eval("AddressLine1")%>' placeholder="Enter Address Line 1"></asp:TextBox>
                                                                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender25" runat="server" FilterType="Numbers, LowercaseLetters, UppercaseLetters, Custom" ValidChars="#,-/ " TargetControlID="txtAddress1" />
                            <asp:TextBox ID="txtAddress2" runat="server" CssClass="form-control" Text='<%# Eval("AddressLine2")%>' placeholder="Enter Address Line 2"></asp:TextBox>
                                                                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender7" runat="server" FilterType="Numbers, LowercaseLetters, UppercaseLetters, Custom" ValidChars="#,-/ " TargetControlID="txtAddress2" />
                            <asp:TextBox ID="txtAddress3" runat="server" CssClass="form-control" Text='<%# Eval("AddressLine3")%>' placeholder="Enter Address Line 3"></asp:TextBox>
                                                                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender8" runat="server" FilterType="Numbers, LowercaseLetters, UppercaseLetters, Custom" ValidChars="#,-/ " TargetControlID="txtAddress3" />
                                                                <asp:Label ID="lblAddressID" runat="server" Text='<%# Eval("VendorLocationID") %>' Visible="false"></asp:Label>
                                                                <br />
                                                                <b>City: </b>
                            <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" Text='<%# Eval("City") %>' placeholder="Enter City"></asp:TextBox>
                                                                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender9" runat="server" FilterType="LowercaseLetters, UppercaseLetters, Custom" ValidChars=" " TargetControlID="txtCity" />
                                                                <br />
                                                                <b>State:</b>
                            <asp:TextBox ID="txtState" runat="server" CssClass="form-control" Text='<%# Eval("State") %>' placeholder="Enter State"></asp:TextBox>
                                                                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender11" runat="server" FilterType="LowercaseLetters, UppercaseLetters, Custom" ValidChars=" " TargetControlID="txtState" />
                                                                <b>Pincode:</b>
                            <asp:TextBox ID="txtPincode" runat="server" CssClass="form-control" Text='<%# Eval("Pincode") %>' placeholder="Enter Pincode"></asp:TextBox>
                                                                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender24" runat="server" FilterType="Numbers" FilterMode="ValidChars" TargetControlID="txtPincode" />
                                                                <br />
                                                                <b>Country: </b>
                                                                <asp:DropDownList ID="ddlCountry" CssClass="form-control" runat="server"></asp:DropDownList>
                                                                <asp:LinkButton ID="lnkUpdate" runat="server" CommandName="Update" Text="Update"><img src="assets/img/accept1.png" style="font-size:smaller" /></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkCancel" runat="server" CommandName="Cancel" Text="Cancel"><img src="assets/img/cross.png" style="font-size:smaller" /></asp:LinkButton>
                                                            </li>
                                                        </ul>
                                                    </EditItemTemplate>
                                                </asp:ListView>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="tab-pane fade" id="contact">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <asp:ListView ID="liContact" runat="server" OnItemDataBound="liContact_ItemDataBound"
                                                    OnItemEditing="liContact_ItemEditing" OnItemUpdating="liContact_ItemUpdating"
                                                    OnItemCanceling="liContact_ItemCanceling">
                                                    <ItemTemplate>
                                                        <ul class="list-unstyled">
                                                            <li><i class='fa fa-user'></i>
                                                                <asp:Label ID="lblContactPerson" runat="server" Text='<%# Eval("Title") + " " + Eval("FirstName") + " " + Eval("MiddleName") + " " + Eval("LastName") %>'></asp:Label></li>
                                                            <br />
                                                            <li><b>Designation:</b>
                                                                <asp:Label ID="lblDesignation" runat="server" Text='<%# Eval("Designation") %>'></asp:Label></li>
                                                            <br />
                                                            <li><b>Department:</b>
                                                                <asp:Label ID="lblDepartment" runat="server" Text='<%# Eval("Department") %>'></asp:Label></li>
                                                            <br />
                                                            <li><b>Mobile:</b>
                                                                <asp:Label ID="lblMobile" runat="server" Text='<%# Eval("ContactMobile") %>'></asp:Label></li>
                                                            <br />
                                                            <li><b>Email:</b>
                                                                <asp:Label ID="lblEmail" runat="server" Text='<%# Eval("ContactEmail") %>'></asp:Label>
                                                                <asp:LinkButton ID="lnkEditContact" runat="server" CommandName="Edit" Text="Edit"><img src="assets/img/Edit.png" width="25px" style="font-size:smaller" /></asp:LinkButton>
                                                            </li>
                                                            <li><b>Contact Type:</b>
                                                                <asp:Label ID="lblIsPrimaryContact" runat="server"
                                                                    Text='<%# Eval("PrimaryContact").ToString() == "Yes" ? "Primary" : "Secondary" %>' />
                                                            </li>
                                                        </ul>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <ul class="list-unstyled">
                                                            <li><i class='fa fa-user'></i>
                                                                <asp:DropDownList ID="ddlTitle" runat="server" CssClass="form-control">
                                                                    <asp:ListItem Value="">Select Title</asp:ListItem>
                                                                    <asp:ListItem Value="Mr.">Mr.</asp:ListItem>
                                                                    <asp:ListItem Value="Miss.">Miss.</asp:ListItem>
                                                                    <asp:ListItem Value="Mrs.">Mrs.</asp:ListItem>
                                                                </asp:DropDownList><br />
                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" Text='<%# Eval("FirstName") %>' placeholder="Enter First Name"></asp:TextBox><br />
                                                                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender2" runat="server" Enabled="True" TargetControlID="txtFirstName" FilterType="UppercaseLetters,LowercaseLetters,Custom" ValidChars=" "></ajaxToolkit:FilteredTextBoxExtender>
                            <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-control" Text='<%# Eval("MiddleName") %>' placeholder="Enter Middle Name"></asp:TextBox><br />
                                                                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" Enabled="True" TargetControlID="txtMiddleName" FilterType="UppercaseLetters,LowercaseLetters" ValidChars=" "></ajaxToolkit:FilteredTextBoxExtender>
                            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" Text='<%# Eval("LastName") %>' placeholder="Enter Last Name"></asp:TextBox><br />
                                                                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender3" runat="server" Enabled="True" TargetControlID="txtLastName" FilterType="UppercaseLetters,LowercaseLetters" ValidChars=" "></ajaxToolkit:FilteredTextBoxExtender>
                                                                <asp:Label ID="lblVendorContactPersonID" runat="server" CssClass="form-control" Text='<%# Eval("VendorContactPersonID") %>' Visible="false"></asp:Label>
                                                                <asp:Label ID="lblVendorContactID" runat="server" CssClass="form-control" Text='<%# Eval("VendorContactID") %>' Visible="false"></asp:Label><br />
                                                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("VendorID") %>' Visible="false"></asp:Label>
                                                            </li>
                                                            <li><b>Designation:</b>
                            <asp:TextBox ID="txtDesignation" runat="server" CssClass="form-control" Text='<%# Eval("Designation") %>' placeholder="Enter Designation"></asp:TextBox>
                                                                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender4" runat="server" Enabled="True" TargetControlID="txtDesignation" FilterType="UppercaseLetters,LowercaseLetters,Custom" ValidChars=" "></ajaxToolkit:FilteredTextBoxExtender>
                                                            </li>
                                                            <br />
                                                            <li><b>Department:</b>
                            <asp:TextBox ID="txtDepartment" runat="server" CssClass="form-control" Text='<%# Eval("Department") %>' placeholder="Enter Department"></asp:TextBox>
                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender5" runat="server" Enabled="True" TargetControlID="txtDepartment" FilterType="UppercaseLetters,LowercaseLetters,Custom" ValidChars=" "></ajaxToolkit:FilteredTextBoxExtender>
                                                            </li>
                                                            <br />
                                                            <li><b>Mobile:</b>
                            <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" Text='<%# Eval("ContactMobile") %>' MaxLength="14" placeholder="Enter Mobile Number"></asp:TextBox>
                                                                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender6" runat="server" Enabled="True" TargetControlID="txtMobile" FilterType="Numbers,Custom" ValidChars="+ "></ajaxToolkit:FilteredTextBoxExtender>
                                                            </li>
                                                            <br />
                                                            <li><b>Contact Type:</b>
                                                                <asp:DropDownList ID="ddlContactType" CssClass="form-control" runat="server">
                                                                    <asp:ListItem Text="Primary" Value="Yes" />
                                                                    <asp:ListItem Text="Secondary" Value="No" />
                                                                </asp:DropDownList>
                                                            </li>
                                                            <br />
                                                            <li><b>Email:</b>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" Text='<%# Eval("ContactEmail") %>' placeholder="Enter Email Address"></asp:TextBox>
                                                                <%--<ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender10" runat="server" FilterType="LowercaseLetters, UppercaseLetters,Custom" TargetControlID="txtEmail" ValidChars=".@,-_" />--%>
                                                                <asp:LinkButton ID="lnkUpdateContact" runat="server" CommandName="Update" Text="Update"><img src="assets/img/accept1.png" style="font-size:smaller" /></asp:LinkButton>
                                                                <asp:LinkButton ID="lnkCancelContact" runat="server" CommandName="Cancel" Text="Cancel"><img src="assets/img/cross.png" style="font-size:smaller" /></asp:LinkButton>
                                                            </li>
                                                        </ul>
                                                    </EditItemTemplate>
                                                </asp:ListView>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="tab-pane fade" id="documents">
                                        <div class="row">
                                            <div class="col-lg-12 col-md-12 col-12">
                                                <div class="form-row">
                                                    <div class="col-md-4">&nbsp;</div>
                                                    <div class="col-md-4">
                                                        <asp:LinkButton ID="lnkEditDocuments" runat="server" Text="Edit" OnClick="lnkEditDocuments_Click"><img src="assets/img/Edit.png" width="25px" style="font-size:smaller" /></asp:LinkButton>
                                                        <%--<asp:Button ID="btnUpdateDocuments" runat="server" Text="Update" OnClick="lnkUpdateDocuments_Click"></asp:Button>--%>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-lg-12 col-md-12 col-12" style="color: darkblue" id="divDocuments" runat="server">
                                                <asp:ListView ID="liVendorDocuments" runat="server">
                                                    <ItemTemplate>
                                                        <ul class="list-unstyled">
                                                            <li>
                                                                <asp:Label ID="lblSlNo" runat="server" Text='<%# Eval("SlNo") + "." %>'></asp:Label>
                                                                <asp:HyperLink ID="hyperlink1" runat="server" Text='<%# Eval("DocumentTypeName") %>' Target="_blank" NavigateUrl='<%# "~/VendorDocument/" + Eval("DocumentName") %>'></asp:HyperLink>
                                                            </li>
                                                        </ul>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                    </EditItemTemplate>
                                                </asp:ListView>
                                            </div>
                                            <div class="col-lg-12 col-md-12 col-12" id="divEditDocuments" runat="server">
                                                <div class="form-row">
                                                    <div class="col-md-4">
                                                        <b>Document Type</b>
                                                        <asp:DropDownList ID="ddlDocumentType" runat="server" CssClass="form-control"></asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <b>File</b>
                                                        <asp:FileUpload ID="fpUpload" runat="server" CssClass="btn btn-primary" Font-Size="Smaller" Width="100%" />
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator13" runat="server" Display="Dynamic" ControlToValidate="fpUpload" ErrorMessage="Please select a .pdf, .doc/.docx, .eps, .jpeg, .jpg, .png   file" ForeColor="Red" ValidationExpression="^([a-zA-Z].*|[1-9].*)\.(((p|P)(d|D)(f|F))|((d|D)(o|O)(c|C))|((d|D)(o|O)(c|C)(x|X))|((e|E)(p|P)(s|S))|((p|P)(n|N)(g|G))|((j|J)(p|P)(e|E)(g|G))|((j|J)(p|P)(g|G)))$"></asp:RegularExpressionValidator>
                                                    </div>
                                                    <div class="col-md-3" style="padding-top: 25px; padding-left: 30px">
                                                        <asp:LinkButton ID="lnkUpdateDocuments" runat="server" Text="Update" OnClick="lnkUpdateDocuments_Click"><img src="assets/img/accept1.png" width="25px" style="font-size:smaller" /></asp:LinkButton>
                                                        <asp:LinkButton ID="lnkCancelUpdate" runat="server" Text="Update" OnClick="lnkCancelUpdate_Click"><img src="assets/img/cross.png" width="25px" style="font-size:smaller" /></asp:LinkButton>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="tab-pane fade" id="categories">
                                        <div class="row">
                                            <div class="col-lg-12 col-md-12 col-12" style="color: darkblue">
                                                <button id="btnSelectCategory" runat="server" type="button" class="btn btn-primary" data-toggle="collapse" data-target="#demo" style="width: 100%">Distributor Category</button>
                                                <div id="demo" class="expand">
                                                    <table style="width: 100%">
                                                        <tr>
                                                            <td>
                                                                <asp:TreeView ID="tvTables" runat="server" ForeColor="#767676" Font-Size="14px"  CssClass="aspNetTreeView" ShowCheckBoxes="All" OnClick="OnTreeClick(event)">
                                                                </asp:TreeView>
                                                            </td>
                                                            <td>
                                                                <asp:HiddenField ID="hdnTree" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Button ID="btnSaveCategory" runat="server" CssClass="btn btn-primary" Text="Update Category" OnClick="btnSaveCategory_Click" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
                    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/4.6.2/js/bootstrap.bundle.min.js"></script>
                </div>


            </div>
        </div>
    </div>

</asp:Content>

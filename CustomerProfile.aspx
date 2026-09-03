<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" EnableEventValidation="false" AutoEventWireup="true" CodeBehind="CustomerProfile.aspx.cs" Inherits="LIMS.CustomerProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://code.jquery.com/jquery-1.11.2.min.js"></script>
    <script>
        $(document).ready(function () {
            $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {
                localStorage.setItem('activeTab', $(e.target).attr('href'));
            });
            var activeTab = localStorage.getItem('activeTab');
            if (activeTab) {
                $('#myTab a[href="' + activeTab + '"]').tab('show');
            }
        });
    </script>
    <style>
        .switch {
            position: relative;
            display: inline-block;
            width: 60px;
            height: 34px;
        }

            .switch input {
                opacity: 0;
                width: 0;
                height: 0;
            }

        .slider {
            position: absolute;
            cursor: pointer;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: #dc3545; /* red */
            transition: .4s;
        }

            .slider:before {
                position: absolute;
                content: "";
                height: 26px;
                width: 26px;
                left: 4px;
                bottom: 4px;
                background: white;
                transition: .4s;
            }

        input:checked + .slider {
            background: #28a745; /* green */
        }

            input:checked + .slider:before {
                transform: translateX(26px);
            }

        .slider.round {
            border-radius: 34px;
        }

            .slider.round:before {
                border-radius: 50%;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="row clearfix">
        <div class="col-xs-12 col-sm-3">
            <div class="card profile-card">
                <div class="profile-header">
                    <h3 style="color: white; text-align: center">
                        <asp:Label ID="lblCompany" runat="server"></asp:Label></h3>
                </div>
                <div class="profile-footer">
                    <ul>
                        <li>
                            <span>Enquiries</span>
                            <span>
                                <asp:Label ID="lblEnquiries" runat="server"></asp:Label></span>
                        </li>
                        <li>
                            <span>Samples</span>
                            <span>
                                <asp:Label ID="lblSamples" runat="server"></asp:Label></span>
                        </li>
                        <li>
                            <span>Completed</span>
                            <span>
                                <asp:Label ID="lblCompleted" runat="server"></asp:Label></span>
                        </li>
                    </ul>
                </div>
            </div>

        </div>
        <div class="col-xs-12 col-sm-9">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="body">
                        <div>
                            <ul class="nav nav-tabs" role="tablist" id="myTab">
                                <li role="presentation" class="active"><a href="#home" aria-controls="home" role="tab" data-toggle="tab"><span id="tabName" runat="server"></span></a></li>
                                <li role="presentation"><a href="#profile_settings" aria-controls="settings" role="tab" data-toggle="tab">Contact Details</a></li>
                            </ul>

                            <div class="tab-content">
                                <div role="tabpanel" class="tab-pane fade in active" id="home">
                                    <div class="panel panel-default panel-post">
                                        <ul id="ulCompany" runat="server" class="list-group" visible="false">
                                            <li id="liGroup" runat="server" class="list-group-item"><b>Group Name: </b>
                                                <asp:Label ID="lblGroupName" runat="server"></asp:Label>
                                                </></li>
                                            <li class="list-group-item"><b>Company Name: </b>
                                                <div class="row clearfix">
                                                    <div class="col-xs-12">
                                                        <asp:LinkButton ID="lnkEditCompanyName" runat="server" OnClick="lnkEditCompanyName_Click" Visible="false"><i class="material-icons pull-right">edit</i></asp:LinkButton>
                                                    </div>
                                                </div>
                                                <div class="row clearfix" id="divViewCustomerName" runat="server">
                                                    <div class="col-xs-6" id="divViewCompanyName">
                                                        <asp:Label ID="lblCompanyName" runat="server"></asp:Label>
                                                    </div>
                                                </div>
                                                <div class="row clearfix">
                                                    <div class="col-xs-6" id="divEditCompanyName" runat="server" visible="false">
                                                        <asp:TextBox ID="txtCompanyName" runat="server" CssClass="form-control" placeholder="Company Name"></asp:TextBox><br />
                                                        <asp:Button ID="btnSaveCustomerName" runat="server" Text="Save" OnClick="btnSaveCustomerName_Click" CssClass="btn btn-primary" />
                                                    </div>

                                                </div>
                                                </></li>

                                            <li class="list-group-item"><b>Address:</b><br />
                                                <div class="row clearfix">
                                                    <div class="col-xs-12">
                                                        <asp:LinkButton ID="btnEditCompanyAddress" runat="server" OnClick="btnEditCompanyAddress_Click"><i class="material-icons pull-right">edit</i></asp:LinkButton>
                                                    </div>
                                                </div>
                                                <div class="row clearfix">
                                                    <div class="col-xs-6">
                                                        <div id="divViewCompanyAddress" runat="server">
                                                            <asp:Label ID="lblCAddress1" runat="server"></asp:Label><br />
                                                            <asp:Label ID="lblCAddress2" runat="server"></asp:Label><br />
                                                            <%--<asp:Label ID="lblCAddress3" runat="server"></asp:Label>--%><br />
                                                            <asp:Label ID="lblCCity" runat="server"></asp:Label><br />
                                                            <%-- <asp:Label ID="lblCTaluka" runat="server"></asp:Label><br />
                                                        <asp:Label ID="lblCDistrict" runat="server"></asp:Label><br />
                                                        <asp:Label ID="lblCState" runat="server"></asp:Label><br />--%>
                                                            <asp:Label ID="lblCPinCode" runat="server"></asp:Label><br />
                                                            <asp:Label ID="lblCCountry" runat="server"></asp:Label><br />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row clearfix">
                                                    <div class="col-xs-12">
                                                        <div id="divEditCompanyAddress" runat="server">
                                                            <div class="form-group">
                                                                <label for="NameSurname" class="col-sm-3 control-label">Address Line-1</label>
                                                                <div class="col-sm-9">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtCAddress1" runat="server" CssClass="form-control"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label for="Email" class="col-sm-3 control-label">Address Line-2</label>
                                                                <div class="col-sm-9">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtCAddress2" runat="server" CssClass="form-control"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <%--   <div class="form-group">
                                                            <label for="Email" class="col-sm-3 control-label">Address Line-3</label>
                                                            <div class="col-sm-9">
                                                                <div class="form-line">
                                                                    <asp:TextBox ID="txtCAddress3" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>--%>
                                                            <div class="form-group">
                                                                <label for="Email" class="col-sm-3 control-label">City</label>
                                                                <div class="col-sm-9">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtCCity" runat="server" CssClass="form-control"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <%-- <div class="form-group">
                                                            <label for="Email" class="col-sm-3 control-label">Taluka</label>
                                                            <div class="col-sm-9">
                                                                <div class="form-line">
                                                                    <asp:TextBox ID="txtCTaluka" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <label for="Email" class="col-sm-3 control-label">District</label>
                                                            <div class="col-sm-9">
                                                                <div class="form-line">
                                                                    <asp:TextBox ID="txtCDistrict" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="form-group">
                                                            <label for="Email" class="col-sm-3 control-label">State</label>
                                                            <div class="col-sm-9">
                                                                <div class="form-line">
                                                                    <asp:TextBox ID="txtCState" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>--%>
                                                            <div class="form-group">
                                                                <label for="Email" class="col-sm-3 control-label">PinCode</label>
                                                                <div class="col-sm-9">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtCPinCode" runat="server" CssClass="form-control"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label for="Email" class="col-sm-3 control-label">Country</label>
                                                                <div class="col-sm-9">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtCCountry" runat="server" CssClass="form-control"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <div class="col-sm-9" style="text-align: center">
                                                                    <asp:Button ID="btnUpdateCAddress" runat="server" OnClick="btnUpdateCAddress_Click" Text="Save" CssClass="btn btn-primary" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </li>
                                        </ul>
                                        <ul id="ulIndividual" runat="server" class="list-group" visible="false">
                                            <li class="list-group-item"><b>Name: </b>
                                                <asp:Label ID="lblIndividualName" runat="server"></asp:Label>
                                                </></li>
                                            <li class="list-group-item"><b>Address</b><br />
                                                <div class="row clearfix">
                                                    <div class="col-xs-12">
                                                        <asp:LinkButton ID="btnEditIndividualAddress" runat="server" OnClick="btnEditIndividualAddress_Click"><i class="material-icons pull-right">edit</i></asp:LinkButton>
                                                    </div>
                                                </div>
                                                <div class="row clearfix">
                                                    <div class="col-xs-6">
                                                        <div id="divViewIndividualAddress" runat="server">
                                                            <asp:Label ID="lblIAddress1" runat="server"></asp:Label><br />
                                                            <asp:Label ID="lblIAddress2" runat="server"></asp:Label><br />
                                                            <asp:Label ID="lblIAddress3" runat="server"></asp:Label><br />
                                                            <asp:Label ID="lblICity" runat="server"></asp:Label><br />
                                                            <asp:Label ID="lblITaluka" runat="server"></asp:Label><br />
                                                            <asp:Label ID="lblIDistrict" runat="server"></asp:Label><br />
                                                            <asp:Label ID="lblIState" runat="server"></asp:Label><br />
                                                            <asp:Label ID="lblIPinCode" runat="server"></asp:Label><br />
                                                            <asp:Label ID="lblICountry" runat="server"></asp:Label><br />
                                                        </div>
                                                    </div>
                                                    <div class="col-xs-6">
                                                        <div id="divEditIndividualAddress" runat="server">
                                                            <div class="form-group">
                                                                <label for="NameSurname" class="col-sm-2 control-label">Address Line-1</label>
                                                                <div class="col-sm-10">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtIAddress1" runat="server" CssClass="form-control"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label for="Email" class="col-sm-2 control-label">Address Line-2</label>
                                                                <div class="col-sm-10">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtIAddress2" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label for="Email" class="col-sm-2 control-label">Address Line-3</label>
                                                                <div class="col-sm-10">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtIAddress3" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label for="Email" class="col-sm-3 control-label">City</label>
                                                                <div class="col-sm-9">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtICity" runat="server" CssClass="form-control"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label for="Email" class="col-sm-3 control-label">Taluka</label>
                                                                <div class="col-sm-9">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtITaluka" runat="server" CssClass="form-control"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label for="Email" class="col-sm-3 control-label">District</label>
                                                                <div class="col-sm-9">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtIDistrict" runat="server" CssClass="form-control"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label for="Email" class="col-sm-2 control-label">State</label>
                                                                <div class="col-sm-10">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtIState" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label for="Email" class="col-sm-2 control-label">PinCode</label>
                                                                <div class="col-sm-10">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtIPinCode" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label for="Email" class="col-sm-2 control-label">Country</label>
                                                                <div class="col-sm-10">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtICountry" runat="server"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <div class="col-sm-10">
                                                                    <div class="form-line">
                                                                        <asp:Button ID="btnUpdateIAddress" runat="server" OnClick="btnUpdateIAddress_Click" Text="Save" />
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </li>
                                        </ul>
                                    </div>
                                </div>
                                <div role="tabpanel" class="tab-pane fade in" id="profile_settings">
                                    <div class="panel panel-default panel-post">
                                        <div class="row clearfix">
                                            <div class="col-md-10">
                                                <div id="divBtnAdd" runat="server" class="col-md-2">
                                                    <div class="form-group form-float">
                                                        <div class="center-block">
                                                            <asp:Button ID="btnAddContact" runat="server" class="btn bg-teal m-t-15 waves-effect" Text=" + " OnClick="btnAddContact_Click" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row clearfix">
                                            <div id="divAddContact" runat="server" visible="false" style="padding: 5%">
                                                <h4>Contact Person</h4>
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
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-sm-3">
                                                                        <div class="form-group form-float">
                                                                            <div class="form-line">
                                                                                <asp:TextBox ID="txtPersonMiddleName" runat="server" AutoComplete="off" CssClass="form-control" placeholder="Middle Name"></asp:TextBox>
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
                                                                    <div class="col-sm-6">
                                                                        <div class="form-group form-float">
                                                                            <div class="form-line">
                                                                                <asp:TextBox ID="txtDesignation" runat="server" AutoComplete="off" CssClass="form-control" placeholder="Designation"></asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-sm-4">
                                                                        <div class="form-group form-float">
                                                                            <div class="form-line">
                                                                                <asp:TextBox ID="txtDepartment" runat="server" AutoComplete="off" CssClass="form-control" placeholder="Department"></asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div class="row clearfix">
                                                                    <div class="col-md-4">
                                                                        <div class="form-group form-float">
                                                                            <div class="form-line">
                                                                                <asp:TextBox ID="txtMobile" runat="server" AutoComplete="off" CssClass="form-control" placeholder="Mobile"></asp:TextBox>
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
                                                                </div>
                                                                <div class="row clearfix">
                                                                    <div class="col-md-6">
                                                                        <div class="form-group form-float">
                                                                            <asp:Button ID="btnContactSave" runat="server" class="btn bg-teal m-t-15 waves-effect" Text="Save" OnClick="btnContactSave_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
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
                                            </div>
                                            <div id="divError" runat="server" visible="false" class="alert alert-danger">
                                                <strong>Data Not Saved!</strong>
                                                <asp:Label ID="Label1" runat="server"></asp:Label>
                                            </div>
                                            <div id="divContactList" runat="server" visible="true" style="padding: 5%">
                                                <asp:ListView ID="liContacts" runat="server" OnItemDataBound="liContacts_ItemDataBound" OnItemEditing="liContacts_ItemEditing" OnItemUpdating="liContacts_ItemUpdating" OnItemCanceling="liContacts_ItemCanceling">
                                                    <ItemTemplate>
                                                        <div class="col-lg-5 col-md-5 col-sm-6 col-xs-12">
                                                            <div class="card">
                                                                <hr class="violet">
                                                                <div class="header">
                                                               
                                                                    <h2>
                                                                        <asp:Label ID="lblContactName" runat="server" Text='<%# Eval("Title") + " " + Eval("FirstName") + " " + Eval("MiddleName") + " " + Eval("LastName") %>' Font-Bold="True" Font-Size="Large"></asp:Label>
                                                                    </h2>
                                                                </div>
                                                                <div class="body">
                                                                    Mobile:
                                                            <asp:Label ID="lblMobile" runat="server" Text='<%# Eval("Mobile") %>' Font-Bold="True"></asp:Label><br />
                                                                    <br />
                                                                    Email:
                                                            <asp:Label ID="lblEmail" runat="server" Text='<%# Eval("Email") %>' Font-Bold="True"></asp:Label><br />
                                                                    <br />
                                                                    Phone:
                                                            <asp:Label ID="lblPhone" runat="server" Text='<%# Eval("Phone") %>' Font-Bold="True"></asp:Label><br />
                                                                    <br />
                                                                    <asp:Button ID="btnEditContact" runat="server" Text="Edit" CommandName="Edit" CssClass="btn btn-primary" />
                                                                    <asp:Button ID="btnToggle"
                                                                        runat="server"
                                                                        Text='<%# (Eval("IsActive") == DBNull.Value ? false : Convert.ToBoolean(Eval("IsActive")))
                                                                                        ? "Disable" : "Enable" %>'
                                                                        CssClass='<%# (Eval("IsActive") == DBNull.Value ? false : Convert.ToBoolean(Eval("IsActive")))
                                                                                        ? "btn btn-danger" : "btn btn-success" %>'
                                                                        CommandArgument='<%# Eval("PersonID") %>'
                                                                        OnClick="btnToggle_Click" />
                                                                </div>
                                                                <asp:HiddenField ID="hfPersonID"
                                                                    runat="server"
                                                                    Value='<%# Eval("PersonID") %>' />
                                                            </div>
                                                        </div>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <div class="col-lg-5 col-md-5 col-sm-6 col-xs-12">
                                                            <div class="card">
                                                                <hr class="violet">
                                                                <div class="header">

                                                                    <h2>
                                                                        <asp:Label ID="lblContactName" runat="server" Text='<%# Eval("Title") + " " + Eval("FirstName") + " " + Eval("MiddleName") + " " + Eval("LastName") %>' Font-Bold="True" Font-Size="Large"></asp:Label>
                                                                    </h2>
                                                                </div>
                                                                <div class="body">
                                                                    <div class="form-group">
                                                                        <label for="Email" class="col-sm-2 control-label">Mobile</label>
                                                                        <div class="col-sm-10">
                                                                            <div class="form-line">
                                                                                <asp:TextBox ID="txtMobile" runat="server" Text='<%#Eval("Mobile")%>' CssClass="form-control"></asp:TextBox>
                                                                                <asp:RequiredFieldValidator ID="rfv_txtMobile" runat="server" ForeColor="Red" Font-Bold="true"
                                                                                    Font-Size="18px" ToolTip="Please enter Mobile." ErrorMessage="*" ControlToValidate="txtMobile"
                                                                                    ValidationGroup="vgContactUpdate" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="form-group">
                                                                        <label for="Email" class="col-sm-2 control-label">Email</label>
                                                                        <div class="col-sm-10">
                                                                            <div class="form-line">
                                                                                <asp:TextBox ID="txtEmail" runat="server" Text='<%#Eval("Email")%>' CssClass="form-control"></asp:TextBox>
                                                                                <asp:RequiredFieldValidator ID="rfv_txtEmail" runat="server" ForeColor="Red" Font-Bold="true"
                                                                                    Font-Size="18px" ToolTip="Please enter Email." ErrorMessage="*" ControlToValidate="txtEmail"
                                                                                    ValidationGroup="vgContactUpdate" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="form-group">
                                                                        <label for="Email" class="col-sm-2 control-label">Phone</label>
                                                                        <div class="col-sm-10">
                                                                            <div class="form-line">
                                                                                <asp:TextBox ID="txtPhone" runat="server" Text='<%#Eval("Phone")%>' CssClass="form-control"></asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="form-group">
                                                                        <asp:Button ID="btnUpdate" runat="server" Text="Update" CommandName="Update" CommandArgument='<%# Eval("PersonID") %>' CausesValidation="true"
                                                                            ValidationGroup="vgContactUpdate" CssClass="btn btn-primary" />

                                                                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CommandName="Cancel" CssClass="btn btn-primary" /></td>               
                                                                    </div>
                                                    </EditItemTemplate>
                                                </asp:ListView>
                                            </div>
                                        </div>
                                        <asp:HiddenField ID="hdCustomerType" runat="server" />
                                        <asp:HiddenField ID="hdCustomerID" runat="server" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        </div>
</asp:Content>

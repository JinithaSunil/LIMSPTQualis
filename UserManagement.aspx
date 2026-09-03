<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="UserManagement.aspx.cs" Inherits="LIMS.UserManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   <link href="assets/css/core.css" rel="stylesheet">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">

                    <h2>USER MANAGEMENT</h2>
                </div>
                <div class="body">
                    <div class="row gy-12">
                        <div class="col-xl-12">
                            <div class="card mb-12">
                                <div class="card-header px-0 pt-0">
                                    <div class="nav-align-top">
                                        <ul class="nav nav-tabs" role="tablist">
                                            <li class="nav-item">
                                                <button
                                                    type="button"
                                                    class="nav-link active"
                                                    role="tab"
                                                    data-bs-toggle="tab"
                                                    data-bs-target="#navs-top-home"
                                                    aria-controls="navs-top-home"
                                                    aria-selected="true">
                                                    Reporting To
              
                                                </button>
                                            </li>
                                            <li class="nav-item">
                                                <button
                                                    type="button"
                                                    class="nav-link"
                                                    role="tab"
                                                    data-bs-toggle="tab"
                                                    data-bs-target="#navs-top-profile"
                                                    aria-controls="navs-top-profile"
                                                    aria-selected="false">
                                                    Employee Details
              
                                                </button>
                                            </li>
                                            <li class="nav-item">
                                                <button
                                                    type="button"
                                                    class="nav-link"
                                                    role="tab"
                                                    data-bs-toggle="tab"
                                                    data-bs-target="#navs-top-messages"
                                                    aria-controls="navs-top-messages"
                                                    aria-selected="false">
                                                    Login Details
              
                                                </button>
                                            </li>
                                        </ul>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="tab-content p-0">
                                        <div class="tab-pane fade show active" id="navs-top-home" role="tabpanel">
                                          <center><h2>Reporting</h2></center>
                                            <div class="row">
                                                <div class="col-md-6">
                                                    <label>Name</label>
                                                    <asp:TextBox ID="txtName" CssClass="form-control" runat="server" />
                                                </div>

                                                <div class="col-md-6">
                                                    <label>Reports To</label>
                                                    <asp:DropDownList ID="ddlReportsTo" CssClass="form-control" runat="server" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="tab-pane fade" id="navs-top-profile" role="tabpanel">
                                           <center> <h5>Employee Details</h5></center>
                                            <div class="col-md-6">
                                                <label>Department</label>
                                                <asp:DropDownList ID="ddlDepartment" CssClass="form-control" runat="server" />
                                            </div>

                                            <div class="col-md-6">
                                                <label>Designation</label>
                                                <asp:DropDownList ID="ddlDesignation" CssClass="form-control" runat="server" />
                                            </div>

                                            <div class="col-md-6 mt-2">
                                                <label>Email</label>
                                                <asp:TextBox ID="txtEmail" CssClass="form-control" runat="server" />
                                            </div>

                                            <div class="col-md-6 mt-2">
                                                <label>Mobile</label>
                                                <asp:TextBox ID="txtMobile" CssClass="form-control" runat="server" />
                                            </div>
                                        </div>
                                        <div class="tab-pane fade" id="navs-top-messages" role="tabpanel">
                                           <center><h5>Login Details</h5></center> 
                                            <div class="col-md-6">
                                                <label>Username</label>
                                                <asp:TextBox ID="txtUsername" CssClass="form-control" runat="server" />
                                            </div>

                                            <div class="col-md-6">
                                                <label>Password</label>
                                                <asp:TextBox ID="txtPassword" TextMode="Password" CssClass="form-control" runat="server" />
                                            </div>

                                            <div class="col-md-6 mt-2">
                                                <label>Role</label>
                                                <asp:DropDownList ID="ddlRole" CssClass="form-control" runat="server" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="text-end mt-3">
                                        <asp:Button ID="btnSave" Text="Save User" CssClass="btn btn-success"
                                            OnClick="btnSave_Click" runat="server" />
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                    <!-- Tabs -->
                </div>
            </div>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
</asp:Content>

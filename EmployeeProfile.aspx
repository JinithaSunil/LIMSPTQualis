<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="EmployeeProfile.aspx.cs" Inherits="LIMS.EmployeeProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="row clearfix">
        <div class="col-xs-12 col-sm-3">
            <div class="card profile-card">
                <div class="profile-header">
                    <h3 style="color: white; text-align: center">
                        <asp:Label ID="lblEmployee" runat="server"></asp:Label></h3>
                </div>
                <div class="profile-footer">
                </div>
            </div>

        </div>
        <div class="col-xs-12 col-sm-9">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="body">
                        <div>
                            <ul class="nav nav-tabs" role="tablist">
                                <li role="presentation" class="active"><a href="#home" aria-controls="home" role="tab" data-toggle="tab"><span id="tabName" runat="server"></span></a></li>
                                <li role="presentation"><a href="#profile_settings" aria-controls="settings" role="tab" data-toggle="tab">Contact Details</a></li>
                                <li role="presentation"><a href="#login" aria-controls="settings" role="tab" data-toggle="tab">Login Details</a></li>
                                <li id="liSignatureTab" runat="server" visible="false" role="presentation"><a href="#signatures" aria-controls="signatures" role="tab" data-toggle="tab">Signatures</a></li>
                            </ul>

                            <div class="tab-content">
                                <div role="tabpanel" class="tab-pane fade in active" id="home">
                                    <div class="panel panel-default panel-post">
                                        <ul id="ulEmployee" runat="server" class="list-group" visible="false">
                                            <li class="list-group-item"><b>Employee ID   :</b><asp:Label ID="lblEmployeeID" runat="server"></asp:Label>
                                                </></li>
                                            <li class="list-group-item"><b>Employee Name :</b><asp:Label ID="lblEmployeeName" runat="server"></asp:Label>
                                                </></li>
                                            <li class="list-group-item"><b>Department    :</b><asp:Label ID="lblDepartment" runat="server"></asp:Label>
                                                </></li>
                                            <li class="list-group-item"><b>Designation   :</b><asp:Label ID="lblDesignation" runat="server"></asp:Label>
                                                </></li>
                                        </ul>

                                    </div>
                                </div>
                                <div role="tabpanel" class="tab-pane fade in" id="profile_settings">
                                    <div class="panel panel-default panel-post">
                                        <div class="row clearfix">
                                            <asp:ListView ID="liContacts" runat="server">
                                                <ItemTemplate>
                                                    <div class="col-lg-5 col-md-5 col-sm-6 col-xs-12">
                                                        <div class="card">
                                                            <hr class="violet">
                                                            <div class="header">

                                                                <h2>
                                                                    <asp:Label ID="lblContactName" runat="server" Text='<%# Eval("EmployeeName") %>' Font-Bold="True" Font-Size="Large"></asp:Label>
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
                                                                <asp:Label ID="lblPhone" runat="server" Text='<%# Eval("ContactNumber") %>' Font-Bold="True"></asp:Label><br />
                                                                <br />
                                                            </div>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:ListView>
                                        </div>
                                    </div>
                                </div>
                                <div role="tabpanel" class="tab-pane fade in" id="login">
                                    <div class="panel panel-default panel-post">
                                        <div class="row clearfix">
                                            <div class="col-lg-5 col-md-5 col-sm-6 col-xs-12">
                                                <div class="card">
                                                    <hr class="violet">
                                                    <div class="header">

                                                        <h2>
                                                            <asp:Label ID="lblLogin" runat="server" Text="Login Details" Font-Bold="True" Font-Size="Large"></asp:Label>
                                                        </h2>
                                                    </div>
                                                    <div class="body">
                                                        User Name :
                                                        <asp:Label ID="lblUserName" runat="server" Font-Bold="True"></asp:Label><br />
                                                        <br />
                                                        Role :
                                                        <asp:Label ID="lblRole" runat="server" Font-Bold="True"></asp:Label><br />
                                                        <br />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div role="tabpanel" class="tab-pane fade" id="signatures" runat="server" ClientIDMode="Static">
                                    <div class="panel panel-default panel-post">
                                        <div class="row clearfix">

                                            <!-- Purchase Requisition -->
                                            <div class="col-lg-6 col-md-6 col-sm-12">
                                                <div class="card" style="text-align:center">
                                                    <hr class="violet" />
                                                    <div class="header">
                                                        <h4>PR Signature</h4>
                                                    </div>
                                                    <div class="body">

                                                        <asp:Image ID="imgPRSignature"
                                                            runat="server"
                                                            Width="250"
                                                            Height="100"
                                                            CssClass="img-thumbnail" />

                                                        <br />
                                                        <br />

                                                        <asp:FileUpload ID="fuPRSignature"
                                                            runat="server"
                                                            CssClass="form-control" />

                                                        <br />

                                                        <asp:Button ID="btnUploadPR"
                                                            runat="server"
                                                            Text="Upload"
                                                            CssClass="btn btn-primary"
                                                            OnClick="btnUploadPR_Click" />

                                                    </div>
                                                </div>
                                            </div>

                                            <!-- Purchase Order -->
                                            <div class="col-lg-6 col-md-6 col-sm-12">
                                                <div class="card" style="text-align:center">
                                                    <hr class="violet" />
                                                    <div class="header">
                                                        <h4>PO Signature</h4>
                                                    </div>
                                                    <div class="body">

                                                        <asp:Image ID="imgPOSignature"
                                                            runat="server"
                                                            Width="250"
                                                            Height="100"
                                                            CssClass="img-thumbnail" />

                                                        <br />
                                                        <br />

                                                        <asp:FileUpload ID="fuPOSignature"
                                                            runat="server"
                                                            CssClass="form-control" />

                                                        <br />

                                                        <asp:Button ID="btnUploadPO"
                                                            runat="server"
                                                            Text="Upload"
                                                            CssClass="btn btn-primary"
                                                            OnClick="btnUploadPO_Click" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
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

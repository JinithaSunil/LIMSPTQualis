<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="AddUser.aspx.cs" Inherits="LIMS.AddUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <!-- Basic Validation -->
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card"><hr class="violet"><div class="header">
                
                    <h2>ADD User</h2>
                </div>
                <div class="body">
                    <div class="row clearfix">
                        <div class="col-md-4">
                            <div class="form-group">
                                <div class="form-line">
                                    <asp:TextBox ID="txtUserName" runat="server" class="form-control" name="Name" required></asp:TextBox>
                                    <label class="form-label">User Name</label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <div class="form-line">
                                    <asp:TextBox ID="txtPassword" runat="server" class="form-control" name="Name" TextMode="Password" required></asp:TextBox>
                                    <label class="form-label">Password</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row clearfix">
                        <div class="col-sm-4">
                            <div class="form-group form-float">
                                <div class="form-line">
                                    <asp:DropDownList ID="ddlEmployees" runat="server" class="form-control show-tick"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-4">
                            <div class="form-group form-float">
                                <div class="form-line">
                                    <asp:DropDownList ID="ddlRoles" runat="server" class="form-control show-tick"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-4">
                            <div class="form-group form-float">
                                <div class="form-line">
                                    <asp:DropDownList ID="ddlStatus" runat="server" class="form-control show-tick">
                                        <asp:ListItem>Select Status</asp:ListItem>
                                        <asp:ListItem Value="1">Active</asp:ListItem>
                                        <asp:ListItem Value="0">InActive</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>

                    <asp:Button ID="btnSubmit" runat="server" class="btn btn-primary waves-effect" Text="SUBMIT" OnClick="btnSubmit_Click" />
                </div>
                <div id="divError" runat="server" visible="false" class="alert alert-danger">
                    <strong>Data Not Saved!</strong>
                    <asp:Label ID="lblMsg" runat="server"></asp:Label>
                </div>
            </div>


        </div>
        <!-- #END# Basic Validation -->
</asp:Content>

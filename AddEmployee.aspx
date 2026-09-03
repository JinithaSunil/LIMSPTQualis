<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="AddEmployee.aspx.cs" Inherits="LIMS.AddEmployee" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <!-- Basic Validation -->
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card"><hr class="violet"><div class="header">
                
                    <h2>ADD EMPLOYEE</h2>
                </div>
                <div class="body">
                    <div class="row clearfix">
                        <div class="col-md-4">
                            <div class="form-group">
                                <div class="form-line">
                                    <asp:TextBox ID="txtName" runat="server" class="form-control" name="Name" required></asp:TextBox>
                                    <label class="form-label">Name</label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <div class="form-line">
                                    <asp:TextBox ID="txtEmployeeID" runat="server" class="form-control" name="Name" required></asp:TextBox>
                                    <label class="form-label">EmployeeID</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row clearfix">
                        <div class="col-sm-4">
                            <div class="form-group form-float">
                                <div class="form-line">
                                    <asp:DropDownList ID="ddlDesignation" runat="server" class="form-control"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-4">
                            <div class="form-group form-float">
                                <div class="form-line">
                            <asp:DropDownList ID="ddlDepartment" runat="server" class="form-control"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row clearfix">
                        <div class="col-md-4">
                            <div class="form-group">
                                <div class="form-line">
                                    <asp:TextBox ID="txtEmail" runat="server" class="form-control" name="Email" required></asp:TextBox>
                                    <label class="form-label">Email</label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <div class="form-line">
                                    <asp:TextBox ID="txtMobile" runat="server" class="form-control" name="Mobile" required></asp:TextBox>
                                    <label class="form-label">Mobile</label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <div class="form-line">
                                    <asp:TextBox ID="txtPhone" runat="server" class="form-control" name="Phone" required></asp:TextBox>
                                    <label class="form-label">Contact Number</label>
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

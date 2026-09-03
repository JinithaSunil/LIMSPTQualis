<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="Employees.aspx.cs" Inherits="LIMS.Employees" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <fieldset>
                <div class="row clearfix" id="divAddEmployee" runat="server" visible="false">
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
                                    <div class="col-sm-4">
                                        <div class="form-group form-float">
                                            <div class="form-line">
                                                <asp:DropDownList ID="ddlSupervisor" runat="server" class="form-control"></asp:DropDownList>
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
                            <div id="div1" runat="server" visible="false" class="alert alert-danger">
                                <strong>Data Not Saved!</strong>
                                <asp:Label ID="Label1" runat="server"></asp:Label>
                            </div>
                        </div>

                    </div>
                </div>
                <div class="row clearfix">
                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <div class="card"><hr class="violet"><div class="header">
                            
                                <h3>Employees</h3>
                                <asp:Button ID="btnAddEmployee" runat="server" class="btn bg-teal m-t-15 waves-effect" Text=" + Add Employee" OnClick="btnAddEmployee_Click" />
                            </div>
                            <div class="body">
                                <div class="row clearfix">
                                    <div class="form-group">
                                        <asp:GridView ID="gvEmployees" runat="server" AutoGenerateColumns="false"
                                            class="table table-bordered table-striped table-hover js-basic-example dataTable"
                                            OnRowEditing="gvEmployees_RowEditing" OnRowUpdating="gvEmployees_RowUpdating"
                                            OnRowCancelingEdit="gvEmployees_RowCancelingEdit">
                                            <Columns>
                                               <%-- <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Button ID="btn_Edit" runat="server" Text="Edit" CommandName="Edit" />
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Button ID="btn_Update" runat="server" Text="Update" CommandName="Update" />
                                                        <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" CommandName="Cancel" />
                                                    </EditItemTemplate>
                                                </asp:TemplateField>--%>
                                                <asp:TemplateField HeaderText="Employee">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEmployee" runat="server" Text='<%#Eval("EmployeeName") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Designation" ControlStyle-Width="100%">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDesignation" runat="server" Text='<%#Eval("DesignationName") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:DropDownList ID="ddlDesignation" runat="server" class="form-control"></asp:DropDownList>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Department" ControlStyle-Width="100%">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDepartment" runat="server" Text='<%#Eval("EmployeeDepartmentName") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:DropDownList ID="ddlEmpDepartment" runat="server" class="form-control"></asp:DropDownList>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Phone" ControlStyle-Width="100%">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblcontactNumber" runat="server" Text='<%#Eval("contactNumber") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtPhone" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Mobile" ControlStyle-Width="100%">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMobile" runat="server" Text='<%#Eval("Mobile") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtMobile" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Email" ControlStyle-Width="100%">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEmail" runat="server" Text='<%#Eval("Email") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Role">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRole" runat="server" Text='<%#Eval("RoleName") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="divError" runat="server" visible="false" class="alert alert-danger">
                        <strong>Data Not Saved!</strong>
                        <asp:Label ID="lblMsg" runat="server"></asp:Label>
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

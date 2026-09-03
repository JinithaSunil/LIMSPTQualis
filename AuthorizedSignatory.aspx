<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="AuthorizedSignatory.aspx.cs" Inherits="LIMS.AuthorizedSignatory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <!-- Basic Validation -->
    <div class="row clearfix" id="divAddSingatory" runat="server" visible="false">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card"><hr class="violet"><div class="header">
                
                    <h2>ADD Signatory</h2>
                </div>
                <div class="body">
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
    </div>
    <div>
        <hr />
    </div>

    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card"><hr class="violet"><div class="header">
                
                    <h3>Authorized Signatories</h3>
                    <asp:Button ID="btnAddSignatory" runat="server" class="btn bg-teal m-t-15 waves-effect" Text=" + Add Signatory" OnClick="btnAddSignatory_Click" />
                </div>
                <div class="body">
                    <div class="row clearfix">
                        <div class="form-group">
                            <asp:GridView ID="gvSignatories" runat="server" AutoGenerateColumns="false" class="table table-bordered table-striped table-hover js-basic-example dataTable">
                                <Columns>
                                    <asp:BoundField DataField="AuthorizedSignatoryName" HeaderText="Signatory Name" />
                                    <asp:BoundField DataField="DesignationName" HeaderText="Designation" />
                                    <asp:BoundField DataField="AuthorizedSignatoryStatus" HeaderText="Status" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- #END# Basic Validation -->
</asp:Content>

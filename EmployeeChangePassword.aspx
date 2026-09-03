<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="EmployeeChangePassword.aspx.cs" Inherits="LIMS.EmployeeChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .preview-container {
            display: inline-block;
            position: relative;
            margin: 5px;
        }

        .preview-image {
            max-width: 100px;
            max-height: 100px;
        }

        .remove-btn {
            position: absolute;
            top: 1px;
            right: 1px;
            width: 20px; /* Set the desired width */
            height: 20px; /* Set the desired height */
            display: flex; /* Use flexbox for content alignment */
            justify-content: center; /* Center content horizontally */
            align-items: center; /* Center content vertically */
        }
    </style>
    <style>
        .gridview-header th {
            background-color: #e7f2ff;
            color: #333;
            font-weight: bold;
            border-color: #ccc;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="page-wrapper">
        <div class="page-content">
            <div class="row">
                <div class="col-xl-12 mx-auto">

                    <div class="card">
                        <div class="header bg-violet">
                            <h2 style="color: white !important">Change Password</h2>
                        </div>
                        <div class="body">
                            <asp:Label runat="server" ID="lblPageID"></asp:Label>

                            <div class="row mt-4 mb-4">
                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Current Password</label>
                                    <asp:TextBox ID="txtcurrentPassword" TextMode="Password" runat="server" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtcurrentPassword"
                                        Display="Dynamic" ErrorMessage="Current Password is required" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                            </div>

                            <div class="row mt-4 mb-4">
                                <div class="col-md-3">
                                    <label for="input1" class="form-label">New Password</label>
                                    <asp:TextBox ID="txtnewPassword" TextMode="Password" runat="server" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtnewPassword"
                                        Display="Dynamic" ErrorMessage="New Password is required" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                            </div>

                            <div class="row mt-4 mb-4">
                                <div class="col-md-3">
                                    <label for="input1" class="form-label">Re-enter New Password</label>
                                    <asp:TextBox ID="txtrenewPassword" TextMode="Password" runat="server" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtrenewPassword"
                                        Display="Dynamic" ErrorMessage="Please re-enter the new password" ForeColor="Red"></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtrenewPassword" ControlToCompare="txtnewPassword"
                                        Display="Dynamic" Type="String" Operator="Equal" ErrorMessage="New Password and Re-enter New Password must be same" ForeColor="Red"></asp:CompareValidator>
                                </div>
                            </div>


                            <div class="row g-3 mt-3 mb-3">
                                <div class="col-md-4" style="margin-left: 9px;">
                                    <asp:Button ID="btnChangePassword" runat="server" Text="Change Password" CssClass="btn btn-primary" OnClick="btnChangePassword_Click" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!--end row-->
    </div>

</asp:Content>

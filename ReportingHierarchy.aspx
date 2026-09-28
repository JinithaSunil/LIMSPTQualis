<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ReportingHierarchy.aspx.cs" Inherits="LIMS.ReportingHierarchy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .container {
            width: 900px;
            margin: 30px auto;
            font-family: Arial;
        }

        .form-row {
            margin-bottom: 18px;
        }

        .label {
            display: block;
            font-weight: bold;
            margin-bottom: 6px;
        }

        .dropdown {
            width: 400px;
            padding: 8px;
        }

        .button {
            padding: 8px 20px;
            margin-right: 8px;
        }

        .hierarchy {
            margin-top: 30px;
            width: 500px;
        }

        .hierarchy-level {
            padding: 10px;
            border: 1px solid #ddd;
            margin-bottom: 2px;
        }

        .level {
            display: inline-block;
            width: 80px;
            font-weight: bold;
        }

        .arrow {
            text-align: center;
            padding: 3px;
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
                            <h2 style="color: white !important">Reporting / Approval Hierarchy Maintenance</h2>
                        </div>

                        <div class="body">
                            <table>
                                <tr>
                                    <td>Employee:</td>
                                    <td>
                                        <asp:DropDownList
                                            ID="ddlEmployee"
                                            runat="server"
                                            CssClass="form-control"
                                            AutoPostBack="true"
                                            OnSelectedIndexChanged="ddlEmployee_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                 <tr>
     <td>&nbsp;</td>
 </tr>
                                <tr>
                                    <td>Level-1 Approver:</td>
                                    <td>
                                        <asp:DropDownList
                                            ID="ddlApprover"
                                            runat="server"
                                            CssClass="form-control">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button
                                            ID="btnSave"
                                            runat="server"
                                            Text="Save"
                                            CssClass="btn btn-primary"
                                            OnClick="btnSave_Click" />
                                    </td>
                                    <td>
                                        <asp:Button
                                            ID="btnClear"
                                            runat="server"
                                            Text="Clear"
                                            CssClass="btn btn-primary"
                                            OnClick="btnClear_Click" />
                                    </td>
                                </tr>
                            </table>



                            <asp:Label
                                ID="lblMessage"
                                runat="server">
                            </asp:Label>


                            <div class="hierarchy">

                                <h3>Reporting / Approval Hierarchy</h3>

                                <asp:GridView
                                    ID="gvHierarchy"
                                    runat="server"
                                    AutoGenerateColumns="false"
                                    Width="500px" class="table table-bordered table-striped table-hover js-basic-example dataTable">

                                    <Columns>

                                        <asp:BoundField
                                            DataField="Level"
                                            HeaderText="Level" />

                                        <asp:BoundField
                                            DataField="Name"
                                            HeaderText="Approver" />

                                    </Columns>

                                </asp:GridView>

                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

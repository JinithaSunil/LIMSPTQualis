<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PTQualisMasterFull.Master" CodeBehind="Electricaccessories.aspx.cs" Inherits="LIMS.Electricaccessories" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .table-responsive {
            width: 100%;
            overflow-x: auto;
            margin-bottom: 20px;
        }

        table {
            width: 100%;
            min-width: 800px; /* More room for fields */
            border-collapse: collapse;
            background-color: #fff;
        }

        table, th, td {
            border: 2px solid #333;
            text-align: left;
        }

        td {
            padding: 8px 10px;
            text-align: left;
            vertical-align: middle;
            text-align: left;
        }

        .info-table td:first-child,
        .info-table td:nth-child(3) {
            background-color: #d1d0d0;
            font-weight: bold;
            width: 20%;
        }

        input[type="text"], input[type="number"], input[type="date"],
        .form-control {
            width: 100%;
            max-width: 100%;
            padding: 6px 8px;
            border: 1px solid #ccc;
            border-radius: 4px;
            display: block;
            box-sizing: border-box;
        }

        .center-text {
            text-align: center;
        }

        .form-actions {
            margin-top: 20px;
            text-align: center;
        }

        .btn {
            background-color: #b34788 !important;
            color: #fff !important; /* white text for contrast */
            border: 1px solid #9e3c76 !important;
        }
    </style>

    <script type="text/javascript">
        window.addEventListener('DOMContentLoaded', function () {
            var inputs = document.querySelectorAll('input[type=text], textarea');
            inputs.forEach(function (input) {
                input.addEventListener('input', function () {
                    if (this.value.trim() !== "")
                        this.style.borderColor = "";
                    else
                        this.style.borderColor = "red";
                });
            });
        });
    </script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="updt" runat="server">
        <ContentTemplate>
            <div class="row clearfix">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="card">
                        <hr class="violet">
                        <div class="header">
                            <h2>Test Record </h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td><b>Analysis Starting Date</b></td>
                                        <td>
                                            <asp:TextBox ID="txtAnalysisStart" runat="server" CssClass="form-control" TextMode="Date" Style="text-align: center;" oninput="clearRedBorder(this)" />
                                        </td>
                                        <td><b>Analysis Completion Date</b></td>
                                        <td>
                                            <asp:TextBox ID="txtCompleteDate" runat="server" CssClass="form-control" TextMode="Date" Style="text-align: center;" oninput="clearRedBorder(this)" />
                                        </td>
                                    </tr>

                                    <tr>
                                        <%--<td><b>Brand</b></td>
                                        <td>
                                            <asp:TextBox ID="txtbrand" runat="server" CssClass="form-control" oninput="clearRedBorder(this)" style="text-align:center"/>

                                        </td>--%>
                                        <td><b>#R</b></td>
                                        <td>
                                            <asp:TextBox ID="txtR" runat="server" CssClass="form-control" oninput="clearRedBorder(this)" Style="text-align: center" />

                                        </td>
                                        <td><b>#WO</b></td>
                                        <td>
                                            <asp:TextBox ID="txtworkorder" runat="server" CssClass="form-control" oninput="clearRedBorder(this)" Style="text-align: center" />

                                        </td>
                                    </tr>

                                    <tr>
                                        <td><b>#Sample</b></td>
                                        <td>
                                            <asp:TextBox ID="txtsample" runat="server" CssClass="form-control" oninput="clearRedBorder(this)" Style="text-align: center" />

                                        </td>
                                        <td><b>PIC</b></td>
                                        <td>
                                            <asp:TextBox ID="txtpic" runat="server" CssClass="form-control" Style="text-align: center;" oninput="clearRedBorder(this)" />

                                        </td>
                                    </tr>

                                    <tr>
                                        <td><b>Start Date</b></td>
                                        <td>
                                            <asp:TextBox ID="txtstartdate" runat="server" CssClass="form-control" oninput="clearRedBorder(this)" Style="text-align: center" TextMode="Date" />

                                        </td>
                                        <td><b>Temperature</b></td>
                                        <td>
                                            <asp:TextBox ID="txttemperature" runat="server" CssClass="form-control"  Style="text-align: center;" oninput="clearRedBorder(this)" />

                                        </td>
                                    </tr>

                                    <tr>
                                        <td><b>Finish Date</b></td>
                                        <td>
                                            <asp:TextBox ID="txtfinishdate" runat="server" CssClass="form-control" oninput="clearRedBorder(this)" Style="text-align: center" TextMode="Date" />

                                        </td>
                                        <td><b>Equipment</b></td>
                                        <td>
                                            <asp:TextBox ID="txtequipment" runat="server" CssClass="form-control"  Style="text-align: center;" oninput="clearRedBorder(this)" />

                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Test Method No.</b></td>
                                        <td>
                                            <asp:TextBox ID="txttestmethod" runat="server" CssClass="form-control" oninput="clearRedBorder(this)" Style="text-align: center" />

                                        </td>
                                        <td><b>Calibration Due</b></td>
                                        <td>
                                            <asp:TextBox ID="txtcalibrationduedate" runat="server" CssClass="form-control" TextMode="Date" Style="text-align: center;" oninput="clearRedBorder(this)" />

                                        </td>
                                    </tr>

                                </table>

                            </div>

                            <%--   <asp:Label ID="lblMessage" runat="server" CssClass="alert" Visible="false"></asp:Label>--%>

                            <div class="form-actions">
                                <div style="margin-bottom: 20px;">
                                    <asp:Button ID="btnDownlaodNew" runat="server" CssClass="btn btn-primary"
                                        Text="Download" OnClick="btnDownload_Click" Visible="false" />

                                </div>
                                <asp:Button ID="btnDownloadExisting" runat="server"
                                    Text="Download Existing File"
                                    CssClass="btn btn-primary"
                                    Visible="false"
                                    OnClick="btnDownloadExisting_Click" />
                            </div>

                            <!-- Step 2: Upload Filled Excel -->
                            <div style="margin-bottom: 15px;">
                                <label><strong>Step 2: Select your filled Excel file</strong></label>
                                <asp:FileUpload ID="fileUpload" runat="server" CssClass="form-control"
                                    Accept=".xlsx,.xls" />
                            </div>

                            <!-- Step 3: Preview  Save -->
                            <div style="display: flex; align-items: center; gap: 10px; margin-top: 15px;">
                                <asp:Button ID="btnPreview" runat="server" CssClass="btn btn-primary"
                                    Text="Preview PDF" OnClick="btnPreview_Click" />

                                <asp:Button ID="btnConfirmUpload" runat="server" CssClass="btn btn-primary"
                                    Text="Save Excel File" Visible="false" OnClick="btnConfirmUpload_Click" />

                                <%-- <small style="color: #666; margin-left: 10px;">Preview to check or Save directly
                                </small>--%>
                            </div>

                            <asp:Button ID="btnUpdateUpload" runat="server" Text="Update" CssClass="btn btn-primary"
                                OnClick="btnUpdateUpload_Click" Visible="false" />

                        </div>
                    </div>
                    <br />


                    <div>
                    </div>
                </div>
            </div>

            </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnDownlaodNew" />
            <asp:PostBackTrigger ControlID="btnPreview" />
            <asp:PostBackTrigger ControlID="btnConfirmUpload" />
            <asp:PostBackTrigger ControlID="btnUpdateUpload" />
            <asp:PostBackTrigger ControlID="btnDownloadExisting" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>

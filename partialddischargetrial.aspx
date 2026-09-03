<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="partialddischargetrial.aspx.cs" Inherits="LIMSStaging.partialddischargetrial" %>





<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 20px;
            background-color: #f5f5f5;
        }

        .card {
            background: white;
            border-radius: 3px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            margin-bottom: 30px;
        }

        .violet {
            border: 0;
            height: 4px;
            background-color: #b34788;
            margin: 0;
        }

        .header {
            padding: 15px 20px;
            border-bottom: 1px solid #e0e0e0;
        }

            .header h2 {
                margin: 0;
                color: #333;
                font-size: 18px;
                font-weight: bold;
            }

        .body {
            padding: 20px;
        }

        .table-responsive {
            overflow-x: auto;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            background-color: white;
        }

        table, th, td {
            border: 1px solid #ddd;
        }

        th {
            background-color: #f2f2f2;
            color: #333;
            padding: 10px;
            text-align: center;
            font-weight: bold;
        }

        td {
            padding: 8px;
            text-align: center;
        }

        .info-table td:first-child {
            background-color: #f2f2f2;
            width: 25%;
            font-weight: bold;
        }

        .test-table th {
            background-color: #e0e0e0;
            vertical-align: middle;
        }

        input[type="text"], input[type="number"], input[type="date"] {
            width: 90%;
            padding: 6px;
            border: 1px solid #ddd;
            border-radius: 3px;
        }

        .btn-primary {
            background-color: #b34788;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 3px;
            cursor: pointer;
            font-weight: bold;
        }

            .btn-primary:hover {
                background-color: #933375;
            }

        .form-actions {
            margin-top: 20px;
            text-align: center;
        }

        /* Make header cells and input aligned */
        .tests-section th, .test-row td {
            vertical-align: middle;
        }
    </style>


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
                            <h2>PARTIAL DISCHARGE</h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td><b>Type</b></td>
                                        <td>
                                            <asp:TextBox ID="txtcableType" runat="server" CssClass="form-control" /></td>
                                        <td><b>Ref.Spec.</b></td>
                                        <td>
                                            <asp:TextBox ID="txtrefSpec" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Size</b></td>
                                        <td>
                                            <asp:TextBox ID="txtcableSize" runat="server" CssClass="form-control" /></td>
                                        <td><b>Test Method No.</b></td>
                                        <td>
                                            <asp:TextBox ID="txttestMethodNo" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Voltage</b></td>
                                        <td>
                                            <asp:TextBox ID="voltage" runat="server" CssClass="form-control" /></td>
                                        <td><b>Job Order No.</b></td>
                                        <td>
                                            <asp:TextBox ID="txtjobOrderNo" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Date</b></td>
                                        <td>
                                           <%-- <asp:TextBox ID="txtdate" runat="server" CssClass="form-control" TextMode="Date" /></td>--%>
                                        <td><b>Work Sheet No.</b></td>
                                        <td>
                                            <asp:TextBox ID="txtWorksheetNo" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Room No. </b></td>
                                        <td>
                                            <asp:TextBox ID="txtRoomNo" runat="server" CssClass="form-control" /></td>
                                        <td><b>Room Humidity</b></td>
                                        <td>
                                            <asp:TextBox ID="txtRoomhumidity" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Room Temp.</b></td>
                                        <td>
                                            <asp:TextBox ID="txtRoomTemp" runat="server" CssClass="form-control" /></td>
                                        <td><b>Equipment No.</b></td>
                                        <td>
                                            <asp:TextBox ID="txtEquipment" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                </table>
                            </div>

                          <div class="table-responsive tests-section" style="margin-top: 20px;">
                                <table class="table table-bordered">
                                 

                                    <table>
                                        <tr>
                                            <th>Date</th>
                                            <th>Item test</th>
                                            <th>Core</th>
                                            <th>Unit</th>
                                            <th>Voltage test 50 Hz</th>
                                            <th>Duration (s)</th>                                            
                                            <th>Maximum Partial discharge Level at</th>
                                        </tr>

 
                                        <tr>
                                            <td>  <asp:TextBox ID="txtdate" runat="server" CssClass="form-control" TextMode="Date" /></td>
                                            <td><asp:DropDownList ID="ddlParametertype" runat="server"></asp:DropDownList></td>
                                            <td><asp:DropDownList ID="ddlPhase" runat="server"></asp:DropDownList></td>
                                            <td><asp:DropDownList ID="ddlUnit" runat="server"></asp:DropDownList></td>
                                            <td><asp:TextBox ID="TextBox2" runat="server" CssClass="form-control" /></td>
                                            <td><asp:TextBox ID="TextBox3" runat="server" CssClass="form-control"  /></td>
                                            <td><asp:TextBox ID="TextBox4" runat="server" CssClass="form-control"  /></td>
                                            <td><asp:TextBox ID="TextBox5" runat="server" CssClass="form-control" TextMode="Date" /></td>
                                        </tr>                                                                      <%-- <tr>
                                            <td>YELLOW</td>
                                            <td>1.73</td>
                                            <td>6.3</td>
                                            <td>10</td>
                                            <td>2.3</td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td>BLUE</td>
                                            <td>1.73</td>
                                            <td>6.3</td>
                                            <td>10</td>
                                            <td>2.3</td>
                                            <td></td>
                                        </tr>--%>
                                      <%--  <tr>
                                            <td rowspan="3">31/12/24</td>
                                            <td rowspan="3">Partial discharge after heating cycles 20 times</td>
                                            <td>RED</td>
                                            <td>1.73</td>
                                            <td>6.3</td>
                                            <td>10</td>
                                            <td>2.5</td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td>YELLOW</td>
                                            <td>1.73</td>
                                            <td>6.3</td>
                                            <td>10</td>
                                            <td>2.5</td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td>BLUE</td>
                                            <td>1.73</td>
                                            <td>6.3</td>
                                            <td>10</td>
                                            <td>2.5</td>
                                            <td></td>
                                      <%--  </tr>
                                        <tr>
                                            <td colspan="6">Standard</td>
                                            <td colspan="2">MAX: 5</td>
                                        </tr>
                                    </table>
                                </table>

                            </div>--%>

                            <div class="form-actions">
                                <asp:Button ID="btnSaveData" runat="server" CssClass="btn-primary" Text="Save Data" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>
</asp:Content>

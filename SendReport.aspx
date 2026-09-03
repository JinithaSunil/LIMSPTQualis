<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="SendReport.aspx.cs" Inherits="LIMS.SendReport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">

        <ContentTemplate>
            <div class="card">
                <div class="header bg-violet">
                    <h2 style="color: white !important">Send Report/s</h2>
                </div>
                <div class="body">
                    <div class="row">
                        <table>
                            <tr>
                                <td>
                                    <div class="col-4">
                                        <asp:DropDownList ID="ddlYear" runat="server" class="form-control" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    </div>
                                </td>
                                <td>
                                    <div class="col-4">
                                        <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlMonth_SelectedIndexChanged" AutoPostBack="true">
                                            <asp:ListItem Value="0">Select Month</asp:ListItem>
                                            <asp:ListItem Value="1">January</asp:ListItem>
                                            <asp:ListItem Value="2">February</asp:ListItem>
                                            <asp:ListItem Value="3">March</asp:ListItem>
                                            <asp:ListItem Value="4">April</asp:ListItem>
                                            <asp:ListItem Value="5">May</asp:ListItem>
                                            <asp:ListItem Value="6">June</asp:ListItem>
                                            <asp:ListItem Value="7">July</asp:ListItem>
                                            <asp:ListItem Value="8">August</asp:ListItem>
                                            <asp:ListItem Value="9">September</asp:ListItem>
                                            <asp:ListItem Value="10">October</asp:ListItem>
                                            <asp:ListItem Value="11">November</asp:ListItem>
                                            <asp:ListItem Value="12">December</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>

                    <div class="row clearfix">
                        <center>                            
                            <div class="portlet-body" style="margin-top: 25px;">
                                <div class="tab-content">
                                    <div class="tab-pane active" id="tab_actions_pending">
                                        <asp:GridView ID="gvReportNewList" runat="server"
                                            OnRowDataBound="gvReportNewList_RowDataBound" DataKeyNames="ID" OnRowCommand="gvReportList_RowCommand"
                                            AutoGenerateColumns="false" class="table table-bordered table-striped table-hover js-basic-example dataTable"
                                            AllowPaging="true" PageSize="10" OnPageIndexChanging="gvReportNewList_PageIndexChanging">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl.No">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="30px"></ItemStyle>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="customerName" HeaderText="Customer Name" />
                                                <asp:BoundField DataField="LabID" HeaderText="SampleidSeries" />
                                                <asp:TemplateField HeaderText="Test Report">
                                                    <ItemTemplate>
                                                        <asp:Button ID="btnViewReport" runat="server" CssClass="btn btn-info btn-sm" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Status">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStatus" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnSend" runat="server" CssClass="btn bg-violet waves-effect" Text="Send" CommandName="SendReport" CommandArgument='<%#Eval("ID")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>

                                            <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" />
                                            <PagerStyle BackColor="#b34788" ForeColor="White" HorizontalAlign="Center" />
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </center>
                    </div>
                </div>
            </div>
        </ContentTemplate>
       
    </asp:UpdatePanel>
</asp:Content>
<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ActivityLog.aspx.cs" Inherits="LIMS.Activities" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .table {
            font-family: Segoe UI, Arial;
            font-size: 13px;
        }

            .table th {
                background-color: #f5f7fa;
                font-weight: 600;
            }

        .pager a {
            margin: 0 4px;
            padding: 6px 10px;
            border: 1px solid #ccc;
            text-decoration: none;
        }

        .pager span {
            background: #1f4ed8;
            color: #fff;
            padding: 6px 10px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <!-- Basic Examples -->
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-12">
                            <h2>ACTIVITY LOG</h2>
                        </div>
                    </div>
                    <br />
                </div>
                <div class="body">
                    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <fieldset>
                                <div class="row">
                                    <asp:GridView ID="gvActivities" runat="server"
                                        AutoGenerateColumns="false"
                                        AllowPaging="true"
                                        AllowCustomPaging="true"
                                        PageSize="10"
                                        CssClass="table table-bordered table-hover"
                                        OnPageIndexChanging="gvActivities_PageIndexChanging">
                                        <Columns>

                                            <asp:TemplateField HeaderText="Date & Time">
                                                <ItemTemplate>
                                                    <%# Eval("ActivityDate", "{0:dd MMM yyyy · hh:mm tt}") %>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="UserName" HeaderText="User" />


                                            <asp:TemplateField HeaderText="Details">
                                                <ItemTemplate>
                                                    <%# Eval("ModuleName") %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="ReferenceNo" HeaderText="Reference" />

                                            <asp:BoundField DataField="Amount"
                                                HeaderText="Amount"
                                                DataFormatString="{0:N2}"
                                                ItemStyle-HorizontalAlign="Right" />

                                        </Columns>

                                        <PagerStyle CssClass="pager" HorizontalAlign="Center" />

                                    </asp:GridView>

                                </div>
                            </fieldset>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

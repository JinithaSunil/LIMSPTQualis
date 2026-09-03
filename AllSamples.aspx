<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="AllSamples.aspx.cs" Inherits="LIMS.AllSamples" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">   
    </asp:Content>
    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <contenttemplate>
                <fieldset>
                    <div class="row clearfix">
                       
                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                            <div class="card"><hr class="violet"><div class="header">
                                    <h3>Sample List</h3>
                                </div>
                                <div class="body">
                                    <div class="row clearfix">
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
                                            <tr>
                                                <td colspan="2">&nbsp;</td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div class="row clearfix">
                                        <div class="form-group">
                                            <asp:GridView ID="gvSampleList" runat="server" AutoGenerateColumns="false"
                                                DataKeyNames="ID,CloneCRFSampleID" class="table table-bordered table-striped table-hover js-basic-example dataTable"
                                                AllowSorting="true" OnSorting="gvSampleList_Sorting" OnRowCommand="gvSampleList_RowCommand" OnRowDataBound="gvSampleList_RowDataBound">
                                                <columns>
                                                    <asp:TemplateField HeaderText="SampleID" SortExpression="SampleIdSeries">
                                                        <itemtemplate>
                                                            <asp:HyperLink ID="hypSampleID" runat="server" Text='<%# Eval("SampleIdSeries") %>' Target="_blank" NavigateUrl='<%# Eval("CloneCRFSampleID", "SampleStatus.aspx?ID={0}") %>'></asp:HyperLink>
                                                        </itemtemplate>
                                                        <itemstyle width="10%"></itemstyle>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="departmentname" HeaderText="Dept." SortExpression="departmentname" />
                                                    <asp:BoundField DataField="ReceivedDate" HeaderText="Received Date" DataFormatString="{0:dd, MMM yy}" />
                                                    <asp:TemplateField HeaderText="TAF" SortExpression="CRFID">
                                                        <itemtemplate>
                                                            <asp:HyperLink ID="hypInquiry" runat="server" Text='<%# "TAF-" + Eval("CloneCRFSampleID") %>' Target="_blank" NavigateUrl='<%#  "~/CustomerRequest/Request-" + Eval("CloneCRFSampleID") + Eval("InquiryId") + Eval("CustomerUID") + ".htm" %>'></asp:HyperLink>
                                                        </itemtemplate>
                                                        <itemstyle width="10%"></itemstyle>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="testreportNo" HeaderText="Test Report#" SortExpression="testreportNo" />
                                                    <asp:BoundField DataField="IssueDate" HeaderText="Report Issued Date" DataFormatString="{0:dd, MMM yy}" SortExpression="IssueDate" />
                                                    <asp:BoundField DataField="ReportName" HeaderText="Report" SortExpression="ReportName" />
                                                    <asp:BoundField DataField="CustomerName" HeaderText="Customer" SortExpression="CustomerName" />
                                                    <asp:BoundField DataField="StatusName" HeaderText="Status" />
                                                    <asp:TemplateField HeaderText="QR">
                                                        <itemtemplate>
                                                            <asp:ImageButton ID="imgQR" runat="server" CommandName="QR" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' ImageUrl='<%# "~/QR/" + Eval("ID") + ".jpg" %>' />
                                                        </itemtemplate>
                                                    </asp:TemplateField>
                                                </columns>
                                                <footerstyle backcolor="White" forecolor="#333333" />
                                                <pagerstyle backcolor="#b34788" forecolor="White" horizontalalign="Center" />
                                                <rowstyle backcolor="White" forecolor="#333333" height="50px" horizontalalign="Left" verticalalign="Middle" />
                                                <selectedrowstyle backcolor="#339966" forecolor="White" />
                                                <sortedascendingcellstyle backcolor="#F7F7F7" />
                                                <sortedascendingheaderstyle backcolor="#487575" />
                                                <sorteddescendingcellstyle backcolor="#E5E5E5" />
                                                <sorteddescendingheaderstyle backcolor="#275353" />
                                            </asp:GridView>
                                            <div id="dialog" style="display: none">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                </fieldset>
            </contenttemplate>
        </asp:UpdatePanel>
        <style type="text/css">
            .button {
                width: 20px;
                height: 40px;
                border-color: #99ccff;
            }
        </style>
</asp:Content>

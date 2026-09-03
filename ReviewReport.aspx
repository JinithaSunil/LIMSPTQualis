<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ReviewReport.aspx.cs" Inherits="LIMS.ReviewReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <fieldset>
                <div class="row clearfix">
                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <div class="card"><hr class="violet"><div class="header">
                            
                                <h3>Test Report List</h3>
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
                                            DataKeyNames="TestResultID,sampleID,CloneCRFSampleID,ID,EnquiryID" 
                                            class="table table-bordered table-striped table-hover js-basic-example dataTable" 
                                            OnRowCommand="gvSampleList_RowCommand" OnRowDataBound="gvSampleList_RowDataBound">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="10%">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="TestReportNo" HeaderText="TestReportNo" ItemStyle-Width="10%" />
                                                <asp:BoundField DataField="SampleIdSeries" HeaderText="SampleId" ItemStyle-Width="10%" />
                                                <asp:BoundField DataField="ReceivedDate" HeaderText="Received Date" DataFormatString="{0:dd, MMM yy}"  ItemStyle-Width="12%"/>
                                                <asp:TemplateField HeaderText="Status">
                                                    <ItemTemplate>   
                                                        <asp:Label ID="lblStatusName" Text='<%# Eval("StatusName") %>' runat="server"></asp:Label> 
                                                    </ItemTemplate>
                                                </asp:TemplateField>     
                                                <asp:TemplateField HeaderText="Report">
                                                    <ItemTemplate>   
                                                        <asp:HyperLink ID="hypReport" runat="server" Target="_blank" NavigateUrl='<%# "~/TestReport/TestReport-" + Eval("TestResultID") + Eval("sampleID") + ".htm" %>' Visible="false">- Test Report</asp:HyperLink><br />
                                                       <asp:HyperLink ID="hypTestResultsNN" runat="server" Text="- Test Report(Non NABL)" NavigateUrl='<%# "~/TestReport/TestReport-" + Eval("TestResultID") + Eval("sampleID") + "_NN.htm" %>' Target="_blank" Visible="false"></asp:HyperLink>
                                                    </ItemTemplate>
                                                </asp:TemplateField>     
                                                <asp:TemplateField HeaderText="Review">
                                                    <ItemTemplate> 
                                                        <asp:Button id="btnAccept"  Visible='<%# Eval("StatusName").ToString() == "Test Report Submitted for Review" ? true : false %>' CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' Text="Review" CommandName="Accept" runat="server" class="btn btn-success btn-xs" Height="30px" />                                                        
                                                    </ItemTemplate>
                                                </asp:TemplateField>  
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Button ID="btnEditCRF" runat="server" Text="Edit TAF" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' CommandName="EditCRF"  class="btn btn-info btn-xs" Height="30px"/>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <FooterStyle BackColor="White" ForeColor="#333333" />
                                            <PagerStyle BackColor="#b34788" ForeColor="White" HorizontalAlign="Center" />
                                            <RowStyle BackColor="White" ForeColor="#333333" Height="50px" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <SelectedRowStyle BackColor="#339966" ForeColor="White" />
                                            <SortedAscendingCellStyle BackColor="#F7F7F7" />
                                            <SortedAscendingHeaderStyle BackColor="#487575" />
                                            <SortedDescendingCellStyle BackColor="#E5E5E5" />
                                            <SortedDescendingHeaderStyle BackColor="#275353" />
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
    <style type="text/css">
        .button {
            width: 20px;
            height: 40px;
            border-color:#99ccff;
        }
    </style>
</asp:Content>

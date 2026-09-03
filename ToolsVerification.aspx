<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ToolsVerification.aspx.cs" Inherits="LIMS.ToolsVerification" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <fieldset>
                <div class="row clearfix">
                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <div class="card">
                            <hr class="violet">
                            <div class="header">
                                <h3>TOOL LIST FOR VERIFICATION </h3>
                            </div>
                            <div class="body">
                                <div class="row">
                                    <asp:GridView ID="gvInstruments" runat="server" DataKeyNames="RequestID"
                                        AutoGenerateColumns="false" class="table table-bordered table-striped table-hover js-basic-example dataTable"
                                        OnRowCommand="gvInstruments_RowCommand" OnRowDataBound="gvInstruments_RowDataBound">
                                        <Columns>
                                            <asp:TemplateField HeaderText="NO." ItemStyle-Width="100">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                    <asp:HiddenField ID="hdRequestID" runat="server" Value='<%#Eval("RequestID") %>' />
                                                    <asp:HiddenField ID="hdToolStatus" runat="server" Value='<%#Eval("CalibrationStatusID") %>' />
                                                    <asp:HiddenField ID="hdInstrumentID" runat="server" Value='<%#Eval("InstrumentID") %>' />
                                                </ItemTemplate>
                                                <ItemStyle Width="50px"></ItemStyle>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="InstrumentName" runat="server" ReadOnly="true" HeaderText="Nama Alat" />
                                            <asp:BoundField DataField="Brand" runat="server" ReadOnly="true" HeaderText="Merek" />
                                            <asp:BoundField DataField="InstrumentType" runat="server" ReadOnly="true" HeaderText="Tipe" />
                                            <asp:BoundField DataField="SerialNumber" runat="server" ReadOnly="true" HeaderText="No. Seri" />
                                            <asp:BoundField DataField="CalibrationRange" runat="server" ReadOnly="true" HeaderText="Range Kalibrasi" />
                                            <asp:BoundField DataField="Quantity" runat="server" ReadOnly="true" HeaderText="Jumlah" />
                                            <asp:BoundField DataField="ComplianceCheck" runat="server" ReadOnly="true" HeaderText="Cek Kesesuaian*(P/S/M/C)" />
                                            <asp:BoundField DataField="CalibrationStatus" runat="server" ReadOnly="true" HeaderText="Status" />                                            
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblStatus" runat="server"></asp:Label>
                                                    <asp:Button ID="btnVerify" runat="server" Text="Verify" CssClass="btn btn-primary" CommandName="Verify" CommandArgument='<%#Eval("RequestID") %>' Visible="false" />
                                                    <asp:Button ID="btnSPK" runat="server" Text="SPK" CssClass="btn btn-info" CommandName="SPK" CommandArgument='<%#Eval("RequestID") %>' Visible="false" />
                                                    <asp:Button ID="btnAllocate" runat="server" Text="Allocate" CssClass="btn btn-primary waves-effect waves-light" CommandName="Allocate" CommandArgument='<%# Eval("RequestID") %>' Visible="false"></asp:Button>                                                    
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="TestRecords">
                                                <ItemTemplate>
                                                    <asp:Panel ID="pnlReport" runat="server" Visible="false">
                                                        <asp:Repeater ID="rptReports" runat="server" OnItemCommand="rptReports_ItemCommand">
                                                            <ItemTemplate>
                                                                <div>
                                                                    <%# Container.ItemIndex + 1 %> . <asp:LinkButton ID="hypReport" runat="server" Text='<%#Eval("ReportName") %>' CommandArgument='<%#Eval("ReportID") %>' CommandName="CalibResult"></asp:LinkButton>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </asp:Panel>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <asp:HiddenField ID="hdQuotationID" runat="server" />
                                </div>

                            </div>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="SampleRevert.aspx.cs" Inherits="LIMS.SampleRevert" %>

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
                            
                                <h3>Revert Sample Status</h3>
                            </div>
                            <div class="body">
                                <div class="row clearfix">
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <asp:DropDownList ID="ddlSampleID" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlSampleID_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <asp:Label ID="lblSampleStatus" runat="server" CssClass="form-control" Enabled="false"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-2">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <asp:DropDownList ID="ddlNewStatus" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlNewStatus_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <asp:Button ID="btnUpdateStatus" runat="server" Text="Revert Status" CssClass="btn btn-primary" OnClick="btnUpdateStatus_Click" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:HiddenField ID="hdSampleStatusID" runat="server" />
</asp:Content>

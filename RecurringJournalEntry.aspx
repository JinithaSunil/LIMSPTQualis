<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="RecurringJournalEntry.aspx.cs" Inherits="LIMS.RecurringJournalEntry" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <!-- Basic Examples -->
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-6">
                            <h2>ADD RECURRING JOURNAL ENTRY</h2>
                        </div>
                    </div>
                </div>
                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <div class="row clearfix">
                                <div class="col-sm-4">
                                    Profile Name:
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:TextBox ID="txtProfile" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row clearfix">
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        Repeat Every
                                        <div class="form-line">
                                            <asp:DropDownList ID="ddlFrequency" runat="server" Width="90%" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlFrequency_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row clearfix">
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        Starts On
                                         <div class="form-line">
                                             <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                         </div>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        End On
                                      <div class="form-line">
                                          <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                      </div>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <asp:CheckBox ID="chkNeverExpires" runat="server" Text="Never Expires" OnCheckedChanged="chkNeverExpires_CheckedChanged" AutoPostBack="true" />
                                </div>
                            </div>
                            </div>
                              <div class="row clearfix">
                                  <div class="col-sm-4">
                                      <div class="form-group">
                                          Reference
                                           <div class="form-line">
                                               <asp:TextBox ID="txtReference" runat="server" CssClass="form-control"></asp:TextBox>
                                           </div>
                                      </div>
                                  </div>
                                  <div class="row clearfix">
                                      <div class="col-sm-4">
                                          <div class="form-group">
                                              Notes
                                             <div class="form-line">
                                                 <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control"></asp:TextBox>
                                             </div>
                                          </div>
                                      </div>
                                  </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="SampleReturn_TSQ-CAL-FM-028.aspx.cs" Inherits="LIMS.SampleReturn_TSQ_CAL_FM_028" %>

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
                                <h3>FORM Pengembalian Sampel</h3>
                            </div>
                            <div class="body">
                                <table style="width: 100%; overflow: scroll">
                                    <tr>
                                        <td><b>Lab. Kalibrasi</b>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtLab" class="form-control" placeholder="Lab" runat="server"></asp:TextBox>
                                                    <asp:Label ID="lblCustomerMasterID" runat="server" Visible="false"></asp:Label>
                                                </div>
                                            </div>
                                        </td>

                                        <td style="padding-left: 20px"><b>Tanggal Diterima:</b>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtDateReceived" class="form-control" runat="server" TextMode="Date"></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>

                                        <td style="padding-left: 20px"><b>Prioritas:</b>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:RadioButtonList ID="radPriority" runat="server">
                                                        <asp:ListItem>Urgent</asp:ListItem>
                                                        <asp:ListItem>Nirmal</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </div>
                                            </div>
                                        </td>
                                        <td style="padding-left: 20px"><b>Tanggal Dikembalikan:</b>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtReturnDate" class="form-control" TextMode="Date" AutoComplete="off" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>No. WO</b>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtWONumber" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                        <td style="padding-left: 20px"><b>Kode Sampel</b>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtSampleID" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                        <td style="padding-left: 20px"><b>Pelanggan</b>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtCustomerName" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                                </td>
                                    </tr>
                                </table>
                                <br />
                                <asp:GridView ID="gvInstruments" runat="server" DataKeyNames="RequestID" AutoGenerateColumns="false" class="table table-bordered table-striped table-hover js-basic-example dataTable"
                                    OnRowDataBound="gvInstruments_RowDataBound" OnRowEditing="gvInstruments_RowEditing"
                                    OnRowUpdating="gvInstruments_RowUpdating" OnRowCancelingEdit="gvInstruments_RowCancelingEdit">
                                    <Columns>
                                        <asp:TemplateField HeaderText="NO." ItemStyle-Width="100">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle Width="50px"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="InstrumentName" runat="server" ReadOnly="true" HeaderText="Nama Alat" />
                                        <asp:BoundField DataField="Brand" runat="server" ReadOnly="true" HeaderText="Merek" />
                                        <asp:BoundField DataField="SerialNumber" runat="server" ReadOnly="true" HeaderText="No. Seri" />
                                        <asp:BoundField DataField="CalibrationRange" runat="server" ReadOnly="true" HeaderText="Kapasitas" />
                                        <asp:BoundField DataField="Quantity" runat="server" ReadOnly="true" HeaderText="Jumlah" />
                                        <asp:TemplateField HeaderText="Kondisi Alat(Baik/Rusak)" ItemStyle-Width="7%">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("VerificationStatus")%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                            <div>
                                <table style="width: 90%;">
                                    <tr>
                                        <td style="padding-left: 20px">Dibuat oleh 
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtPreparedBy" runat="server" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                        <td style="padding-left: 20px">Dibuat Tanggal
                                             <div class="form-group">
                                                 <div class="form-line">
                                                     <asp:TextBox ID="txtPreparedDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                                 </div>
                                             </div>
                                        </td>
                                        <td style="padding-left: 20px">Diperiksa oleh           
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtCheckedBy" runat="server" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                        <td style="padding-left: 20px">Diperiksa Tanggal
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtCheckedDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding-left: 20px">Disetujui oleh
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtApprovedBy" runat="server" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                        <td style="padding-left: 20px">Disetujui Tanggal
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtApprovedDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                        <td style="padding-left: 20px">Diberikan oleh
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtSentBy" runat="server" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                        <td style="padding-left: 20px">Diberikan Tanggal
                                             <div class="form-group">
                                                 <div class="form-line">
                                                     <asp:TextBox ID="txtSentDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                                 </div>
                                             </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding-left: 20px">Diterima oleh
                                         <div class="form-group">
                                             <div class="form-line">
                                                 <asp:TextBox ID="txtAcceptedBy" runat="server" CssClass="form-control"></asp:TextBox>
                                             </div>
                                         </div>
                                        </td>
                                        <td style="padding-left: 20px">Diterima Tanggal
                                          <div class="form-group">
                                              <div class="form-line">
                                                  <asp:TextBox ID="txtAcceptedDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                              </div>
                                          </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div class="col-12" style="text-align: center">
                                <asp:Button ID="btnSave" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSave_Click" Visible="false" />
                                <asp:Button ID="btnChecked" runat="server" Text="Checked" OnClick="btnChecked_Click" Visible="false" />
                                <asp:Button ID="btnApproved" runat="server" Text="Approved" OnClick="btnApproved_Click" Visible="false" />
                                <asp:Button ID="btnAccepted" runat="server" Text="Accepted" OnClick="btnAccepted_Click" Visible="false" />
                                <asp:Button ID="btnSent" runat="server" Text="Sent" OnClick="btnSent_Click" Visible="false" />                                

                                <br />
                                <br />
                            </div>

                        </div>
                    </div>
                </div>
                <asp:HiddenField ID="hdQuotationID" runat="server" />
                <asp:HiddenField ID="hdRequestID" runat="server" />
                <asp:HiddenField ID="hdRoleID" runat="server" />
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

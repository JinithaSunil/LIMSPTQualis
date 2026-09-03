<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ToolReceiptRecord-TSQ-CAL-FM-003.aspx.cs" Inherits="LIMS.ToolReceiptRecord_TSQ_CAL_FM_003" %>

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
                                <h3>FORM REKAMAN PENERIMAAN ALAT </h3>
                            </div>
                            <div class="body">
                                <table style="width: 100%; overflow: scroll">
                                    <tr>
                                        <td><b>Pelanggan:</b>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtCustomerName" class="form-control" placeholder="Customer Name" runat="server" disabled></asp:TextBox>
                                                    <asp:Label ID="lblCustomerMasterID" runat="server" Visible="false"></asp:Label>
                                                </div>
                                            </div>
                                        </td>

                                        <td style="padding-left: 20px"><b>No.PO:</b>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtPONumber" class="form-control" runat="server" disabled></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>

                                        <td style="padding-left: 20px"><b>Tanggal penerimaan:</b>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtReceiptDate" class="form-control" TextMode="Date" AutoComplete="off" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                        <td style="padding-left: 20px"><b>Tempat:</b>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtPlace" class="form-control" AutoComplete="off" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            <table style="width: 100%">
                                                <tr>
                                                    <td>
                                                        <table style="width: 100%">
                                                            <tr>
                                                                <td>
                                                                    <h5><u>Konfirmasi penerima</u></h5>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Nama 
                                                                  <div class="form-group">
                                                                      <div class="form-line">
                                                                          <asp:TextBox ID="txtCheckedBy" runat="server" CssClass="form-control"></asp:TextBox>
                                                                      </div>
                                                                  </div>
                                                                    Paraf           
                                                                 <div class="form-group">
                                                                     <div class="form-line">
                                                                         <asp:TextBox ID="txtCheckedByDesignation" runat="server" CssClass="form-control"></asp:TextBox>
                                                                     </div>
                                                                 </div>

                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td style="padding-left: 20px">
                                                        <table style="width: 100%">
                                                            <tr>
                                                                <td>
                                                                    <h5><u>Persetujuan Customer</u></h5>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Nama
                                                                <div class="form-group">
                                                                    <div class="form-line">
                                                                        <asp:TextBox ID="txtCustomerContact" runat="server" CssClass="form-control"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                                    Paraf
                                                                 <div class="form-group">
                                                                     <div class="form-line">
                                                                         <asp:TextBox ID="txtCustomerContactDesignation" runat="server" CssClass="form-control"></asp:TextBox>
                                                                     </div>
                                                                 </div>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <h5><u>Scope Kalibrasi</u></h5>
                                            <asp:CheckBoxList ID="chkScope" runat="server"></asp:CheckBoxList>
                                        </td>
                                    </tr>

                                </table>
                                </td>
                                    </tr>
                                </table>
                                <br />
                                <h5><u>Detail</u></h5>
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
                                        <asp:BoundField DataField="InstrumentType" runat="server" ReadOnly="true" HeaderText="Tipe" />
                                        <asp:BoundField DataField="SerialNumber" runat="server" ReadOnly="true" HeaderText="No. Seri" />
                                        <asp:BoundField DataField="CalibrationRange" runat="server" ReadOnly="true" HeaderText="Range Kalibrasi" />
                                        <asp:BoundField DataField="Quantity" runat="server" ReadOnly="true" HeaderText="Jumlah" />
                                        <asp:BoundField DataField="ComplianceCheck" runat="server" ReadOnly="true" HeaderText="Cek Kesesuaian*(P/S/M/C)" />
                                        <asp:TemplateField HeaderText="Kondisi" ItemStyle-Width="7%">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("VerificationStatus")%>'></asp:Label>
                                            </ItemTemplate>
                                            <ControlStyle Width="100%" />
                                            <EditItemTemplate>
                                                <asp:RadioButtonList ID="radStatus" runat="server">
                                                    <asp:ListItem Value="1" Selected="True">Baik</asp:ListItem>
                                                    <asp:ListItem Value="0">Tidak Baik</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:CommandField HeaderText="Action" ShowEditButton="true" ButtonType="Image" EditImageUrl="~/assets/img/edit.png" ControlStyle-Width="30px" UpdateImageUrl="~/assets/img/accept1.png" CancelImageUrl="~/assets/img/cross.png" ItemStyle-Width="7%" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                            <div>
                                <asp:TextBox id="txtNotes" runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="col-12" style="text-align: center">
                                <asp:Button ID="btnSave" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSave_Click" Visible="false" /><br />
                                <br />
                            </div>

                        </div>
                    </div>
                </div>
                <asp:HiddenField ID="hdQuotationID" runat="server" />
                <asp:HiddenField ID="hdRequestID" runat="server" />
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

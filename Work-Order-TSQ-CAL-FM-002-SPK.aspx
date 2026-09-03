<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="Work-Order-TSQ-CAL-FM-002-SPK.aspx.cs" Inherits="LIMS.Work_Order_TSQ_CAL_FM_002_SPK" %>

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
                                <h3>FORM SPK</h3>
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

                                        <td style="padding-left: 20px"><b>Tanggal kalibrasi:</b>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtServiceDueDate" class="form-control" TextMode="Date" AutoComplete="off" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                        <td style="padding-left: 20px"><b>Tanggal Terbit Sertifikat:</b>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtCertificateIssueDate" class="form-control" TextMode="Date" AutoComplete="off" runat="server"></asp:TextBox>
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
                                        <td style="padding-left: 20px"><b>No. Permohonan</b>
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtApplicationNo" class="form-control" runat="server"></asp:TextBox>
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
                                <asp:GridView ID="gvInstruments" runat="server" DataKeyNames="RequestID" AutoGenerateColumns="false" class="table table-bordered table-striped table-hover js-basic-example dataTable">
                                    <Columns>
                                        <asp:TemplateField HeaderText="NO." ItemStyle-Width="100">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle Width="50px"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="InstrumentName" runat="server" ReadOnly="true" HeaderText="Nama Alat" />
                                        <asp:BoundField DataField="Brand" runat="server" ReadOnly="true" HeaderText="Merek" />
                                        <asp:TemplateField HeaderText="Status" ItemStyle-Width="7%">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="SerialNumber" runat="server" ReadOnly="true" HeaderText="No. Seri" />
                                        <asp:BoundField DataField="CalibrationRange" runat="server" ReadOnly="true" HeaderText="Kapasitas / Spesifikasi" />
                                        <asp:BoundField DataField="Quantity" runat="server" ReadOnly="true" HeaderText="Jumlah" />
                                        <asp:TemplateField HeaderText="Keterangan" ItemStyle-Width="7%">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRemarks" runat="server" Text=""></asp:Label>
                                            </ItemTemplate>
                                            <ControlStyle Width="100%" />
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control"></asp:TextBox>
                                            </EditItemTemplate>
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
                                        <td style="padding-left: 20px">Diterima oleh
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtReceivedBy" runat="server" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                        <td style="padding-left: 20px">Diterima Tanggal 
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtReceivedDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                        <td style="padding-left: 20px">Dikembalikan oleh
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtReturnedBy" runat="server" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                        </td>
                                        <td style="padding-left: 20px">Dikembalikan Tanggal
                                             <div class="form-group">
                                                 <div class="form-line">
                                                     <asp:TextBox ID="txtReturnedDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                                 </div>
                                             </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div class="col-12" style="text-align: center">
                                <asp:Button ID="btnSave" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSave_Click" /><br />
                                <br />
                            </div>

                        </div>
                    </div>
                </div>
                <asp:HiddenField ID="hdQuotationID" runat="server" />
                <asp:HiddenField ID="hdRequestID" runat="server" />
                <asp:HiddenField ID="hdRoleID" runat="server" />
                <asp:HiddenField ID="hdSampleID" runat="server" />
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

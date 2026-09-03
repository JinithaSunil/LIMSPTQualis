<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="SampleImages.aspx.cs" Inherits="LIMS.SampleImages" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .cb label {
            height: 43px;
        }

        .rbWidth label {
            height: 40px;
        }

        [type="radio"]:not(:checked) + label {
            padding-left: 26px;
            height: 43px;
            line-height: 25px;
            font-size: 13px;
            font-weight: normal;
        }

        [type="radio"]:checked + label {
            padding-left: 26px;
            height: 43px;
            line-height: 25px;
            font-size: 13px;
            font-weight: normal;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div id="divtest" runat="server"></div>
<%--    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <fieldset>--%>
                <div class="row clearfix">
                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <div class="card">
                            <hr class="violet">
                            <div class="header">

                                <h3>Sample Images</h3>
                            </div>
                            <div class="body">
                                <div class="row clearfix">
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <strong>Enquiry ID: </strong>
                                                <asp:Label ID="lblInquiryID" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <strong>Sub Contractor: </strong>
                                                <asp:Label ID="lblSubContractor" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <asp:GridView ID="gvInstruments" runat="server" DataKeyNames="RequestID,EnquiryID,QuotationID,SubContractorID" AutoGenerateColumns="false" class="table table-bordered table-striped table-hover js-basic-example dataTable"
                                    OnRowEditing="gvInstruments_RowEditing" OnRowCancelingEdit="gvInstruments_RowCancelingEdit" OnRowUpdating="gvInstruments_RowUpdating"
                                    OnRowCommand="gvInstruments_RowCommand" OnRowDataBound="gvInstruments_RowDataBound">
                                    <Columns>
                                        <asp:TemplateField HeaderText="NO." ItemStyle-Width="100">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle Width="50px"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Nama Alat">
                                            <ItemTemplate>
                                                <asp:Label ID="InstrumentName" runat="server" Text='<%#Eval("InstrumentName") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblInstrumentName" runat="server" Text='<%#Eval("InstrumentName") %>'></asp:Label>
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Brand" runat="server" ReadOnly="true" HeaderText="Merek" />
                                        <asp:BoundField DataField="SerialNumber" runat="server" ReadOnly="true" HeaderText="No. Seri" />
                                        <asp:BoundField DataField="CalibrationRange" runat="server" ReadOnly="true" HeaderText="Kapasitas" />
                                        <asp:BoundField DataField="Quantity" runat="server" ReadOnly="true" HeaderText="Jumlah" />
                                        <asp:TemplateField HeaderText="Image">
                                            <ItemTemplate>
                                                <asp:Image ID="imgPhoto" runat="server" ImageUrl='<%# "SampleImages/"+Eval("SampleImage") %>' Width="80px" />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:FileUpload ID="fpSampleImage" runat="server" />
                                                <br />
                                                <asp:Image ID="imgEditPhoto" runat="server" ImageUrl='<%# Eval("SampleImage") %>' Width="80px" />
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:CommandField ShowEditButton="True" />
                                    </Columns>
                                </asp:GridView>
                                <asp:Label ID="lblEnquiryID" runat="server" Visible="false"></asp:Label>
                                <asp:Label ID="lblQuotationID" runat="server" Visible="false"></asp:Label>
                                <asp:Label ID="lblSubContractorID" runat="server" Visible="false"></asp:Label>
                                <table style="width: 100%">
                                    <tr>
                                        <td style="text-align: center">
                                            <asp:Button ID="btnSubmit" runat="server" class="btn bg-teal m-t-15 waves-effect" Text="Save" OnClick="btnSubmit_Click" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <asp:HiddenField ID="HiddenField1" Value="0" runat="server" />
                        </div>
                    </div>
                </div>
                </div>
            </fieldset>
            </div>
            </div>
       <%-- </ContentTemplate>
         <Triggers>
                <asp:PostBackTrigger ControlID="fpSampleImage" />
            </Triggers>
    </asp:UpdatePanel>--%>
    <asp:HiddenField ID="hdSampleHistory" runat="server" />
    <asp:HiddenField ID="hdSampleCategory" runat="server" />
    <asp:HiddenField ID="hdSampleID" runat="server" />
    <asp:HiddenField ID="hdDepartmentID" runat="server" />
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="CalibrationSubContractorEval-TSQ-CAL-FM-006.aspx.cs" Inherits="LIMS.CalibrationSubContractorEval_TSQ_CAL_FM_006" %>

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
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <fieldset>
                <div class="row clearfix">
                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <div class="card">
                            <hr class="violet">
                            <div class="header">

                                <h3>FORM EVALUASI SUBKONTRAKTOR / JASA KALIBRASI </h3>
                            </div>
                            <div class="body">
                                <div class="row clearfix">
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <strong>Category Type: </strong>
                                                <asp:Label ID="lblCategoryType" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <strong>Category Name: </strong>
                                                <asp:Label ID="lblCategoryName" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <strong>Enquiry ID: </strong>
                                                <asp:Label ID="lblInquiryID" runat="server"></asp:Label>
                                                <asp:Label ID="lblEnquiryId" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row clearfix">
                                    <div class="col-sm-12">
                                        <div class="form-group">
                                            <div class="form-line">
                                               <b> Nama Subkontraktor / Penyedia Jasa Kalibrasi: </b>
                                                <asp:TextBox ID="txtSubContractorName" class="form-control" AutoComplete="off" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row clearfix">
                                    <div class="col-sm-12">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <b>Alamat: </b>
                                                <asp:TextBox ID="txtAddress" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row clearfix">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div class="form-line"><b>Telepon:</b>
                                                <asp:TextBox ID="txtPhone" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div class="form-line"><b>Fax:</b>
                                                <asp:TextBox ID="txtFax" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row clearfix">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div class="form-line"><b>Kontak Pribadi:</b>
                                                <asp:TextBox ID="txtContactPerson" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div class="form-line"><b>Email:</b>
                                                <asp:TextBox ID="txtEmail" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row clearfix">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div class="form-line"><b>Dievaluasi oleh:</b>
                                                <asp:TextBox ID="txtEvaluatedBy" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div class="form-line"><b>Tanggal:</b>
                                                <asp:TextBox ID="txtDate" class="form-control" AutoComplete="off" runat="server"></asp:TextBox>
                                                <ajaxToolkit:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtDate" Format="dd/MMM/yyyy"></ajaxToolkit:CalendarExtender>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row clearfix">
                                    <asp:GridView ID="gvSubContractorEvaluation" runat="server" class="table table-bordered table-striped table-hover js-basic-example dataTable" AutoGenerateColumns="False"
                                        DataKeyNames="ChecklistID" OnRowEditing="gvSubContractorEvaluation_RowEditing"
                                        OnRowCancelingEdit="gvSubContractorEvaluation_RowCancelingEdit"
                                        OnRowUpdating="gvSubContractorEvaluation_RowUpdating">
                                        <Columns>
                                            <asp:BoundField DataField="Item" HeaderText="Persyaratan" ReadOnly="true" />
                                            <asp:TemplateField HeaderText="Nilai">
                                                <ItemTemplate>                                                    
                                                    <asp:Label ID="lblValue" runat="server" Text='<%#Eval("Value") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:HiddenField ID="hdChecklistID" runat="server" />
                                                    <asp:TextBox ID="txtValue" runat="server"></asp:TextBox>
                                                </EditItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Keterangan">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRemarks" runat="server" Text='<%#Eval("Remarks") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="txtRemarks" runat="server"></asp:TextBox>
                                                </EditItemTemplate>
                                            </asp:TemplateField>
                                            <asp:CommandField ShowEditButton="True" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
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
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:HiddenField ID="hdSampleHistory" runat="server" />
    <asp:HiddenField ID="hdSampleCategory" runat="server" />
    <asp:HiddenField ID="hdSampleID" runat="server" />
    <asp:HiddenField ID="hdDepartmentID" runat="server" />
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="TestReportReview.aspx.cs" Inherits="LIMS.TestReportReview" %>

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
                        <div class="card"><hr class="violet"><div class="header">
                            
                                <h3>Test Report Review</h3>
                            </div>
                            <div class="body">
                                <div class="row clearfix">
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <strong>Category Type: </strong>
                                                <asp:Label ID="lblCategoryType" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <strong>Category Name: </strong>
                                                <asp:Label ID="lblCategoryName" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <strong>Enquiry ID: </strong>
                                                <asp:Label ID="lblInquiryID" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <strong>Sample ID: </strong>
                                                <asp:Label ID="lblSampleID" runat="server" Visible="false"></asp:Label>
                                                <asp:Label ID="lblSampleSeries" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="row clearfix">
                                    <div id="divCheckBox" runat="server">
                                        <div id="checklist1" runat="server">
                                            <div class="col-sm-4">
                                                <div class="form-group">
                                                    <asp:RadioButtonList ID="rbCheckList" CssClass="rbWidth" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" OnSelectedIndexChanged="rbCheckList_SelectedIndexChanged" RepeatColumns="3"></asp:RadioButtonList>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-sm-4">
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:CheckBoxList ID="chkSampleCheck" CssClass="cb" runat="server" Height="45px" AutoPostBack="true"></asp:CheckBoxList>
                                                </div>
                                            </div>
                                        </div>
                                        <div id="divDynamicTextBox" runat="server" class="col-sm-4">
                                            <div class="form-group">
                                                <asp:TextBox ID="txt1" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="txt2" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="txt3" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="txt4" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="txt5" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="txt6" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="txt7" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="txt8" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="txt9" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="txt10" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="txt11" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="txt12" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="txt13" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="txt14" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="txt15" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="txt16" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="txt17" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="txt18" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="txt19" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                                <asp:TextBox ID="txt20" runat="server"></asp:TextBox>
                                            </div>

                                        </div>
                                    </div>
                                    <table style="width: 100%">
                                        <tr>
                                            <td style="text-align: center">
                                                <asp:Button ID="btnAccept" runat="server" class="btn bg-green m-t-15 waves-effect" Text="Approve" OnClick="btnApprove_Click" />&nbsp;&nbsp;
                                                <asp:Button ID="btnReject" runat="server" class="btn bg-red m-t-15 waves-effect" Text="Reject" OnClick="btnReject_Click" />

                                            </td>
                                            <td>
                                                <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                            <asp:HiddenField ID="HiddenField1" Value="0" runat="server" />
                            <asp:HiddenField ID="hdrbCheckList" Value="" runat="server" />
                        </div>
                    </div>
                </div>
                </div>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:HiddenField ID="hdSampleHistory" runat="server" />
    <asp:HiddenField ID="hdSampleCategory" runat="server" />
    <asp:HiddenField ID="hdSampleID" runat="server" />
    <asp:HiddenField ID="hdDepartmentID" runat="server" />
</asp:Content>

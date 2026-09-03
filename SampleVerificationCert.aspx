<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="SampleVerificationCert.aspx.cs" Inherits="LIMS.SampleVerificationCert" %>

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
                            
                                <h3>Sample Management</h3>
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
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row clearfix">
                                    <div class="col-sm-6">
                                        <h5>Sampling & Sample Preparation</h5>
                                    </div>
                                </div>
                                <div class="row clearfix">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <asp:DropDownList ID="ddlSamplingMethod" runat="server" CssClass="form-control"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row clearfix">
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <asp:TextBox ID="txtSampleReceivedDate" class="form-control" AutoComplete="off" placeholder="Sample Received Date" runat="server" OnTextChanged="txtSampleReceivedDate_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtSampleReceivedDate" Format="dd/MMM/yyyy"></ajaxToolkit:CalendarExtender>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <asp:TextBox ID="txtSamplingLocation" class="form-control" placeholder="Place of Loading & Sampling" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <asp:TextBox ID="txtSampleConditionRemarks" class="form-control" placeholder="Sample Condition Remarks" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row clearfix">
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <asp:TextBox ID="txtCargoDescription" class="form-control" placeholder="Cargo Description" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>                                   
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <asp:TextBox ID="txtDateOfSampling" class="form-control" AutoComplete="off" placeholder="Date of Sampling" runat="server"></asp:TextBox>
                                                <ajaxToolkit:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtDateOfSampling" Format="dd/MMM/yyyy"></ajaxToolkit:CalendarExtender>
                                            </div>
                                        </div>
                                    </div>
                                     <div class="col-sm-4" id="div4" runat="server">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <asp:TextBox ID="txtReceiver" class="form-control" placeholder="Receiver" runat="server"></asp:TextBox>
                                                <asp:TextBox ID="txtStackQty" class="form-control" placeholder="Stack Qty" runat="server"></asp:TextBox>                                                
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row clearfix" id="div1" runat="server">
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <asp:TextBox ID="txtTransportMode" class="form-control" placeholder="Transport Mode" runat="server"></asp:TextBox>
                                                 <asp:TextBox ID="txtVesselName" class="form-control" placeholder="Vessel Name" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <asp:TextBox ID="txtLoadingDate" class="form-control"  AutoComplete="off" placeholder="Loading Date" runat="server"></asp:TextBox>
                                                <ajaxToolkit:CalendarExtender ID="CalendarExtender4" runat="server" TargetControlID="txtLoadingDate" Format="dd/MMM/yyyy"></ajaxToolkit:CalendarExtender>
                                            </div>
                                        </div>
                                    </div>
                                     <div class="col-sm-4">
                                        <div class="form-group">
                                            <div class="form-line">
                                                 <asp:TextBox ID="txtLoadedQty" class="form-control" placeholder="Loaded Qty" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row clearfix" id="div2" runat="server">
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <asp:TextBox ID="txtRRNo" class="form-control" placeholder="RR No." runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <asp:TextBox ID="txtRRDate" class="form-control" placeholder="RR Date" runat="server"></asp:TextBox>
                                                <ajaxToolkit:CalendarExtender ID="CalendarExtender3" runat="server" TargetControlID="txtRRDate" Format="dd/MMM/yyyy"></ajaxToolkit:CalendarExtender>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <asp:TextBox ID="txtRRQty" class="form-control" AutoComplete="off" placeholder="RR Qty" runat="server"></asp:TextBox>                                                
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row clearfix" id="div3" runat="server">
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <asp:TextBox ID="txtNoOfTrucks" class="form-control" placeholder="No. Of Trucks" runat="server"></asp:TextBox>
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


                                        </div>
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
                                </div>
                            <asp:HiddenField ID="HiddenField1" Value="0" runat="server" />
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

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="TestRequestReviewChecklist.aspx.cs" Inherits="LIMS.TestRequestReviewChecklist" %>

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

                                <h3>Test Request Review</h3>
                            </div>
                            <div class="body">
                                <div class="row clearfix">
                                    <div class="col-sm-12">
                                        <table style="width: 100%">
                                            <tr>
                                                <td style="width: 25%"><strong>Enquiry ID:<asp:Label ID="lblInquiryID" runat="server"></asp:Label>
                                                </strong></td>
                                                <td style="width: 25%"><strong>Sample Name: </strong>
                                                    <asp:Label ID="lblSampleName" runat="server"></asp:Label></td>
                                                <td style="width: 25%"><strong>Sample Qty: </strong>
                                                    <asp:Label ID="lblSampleQty" runat="server"></asp:Label><br />
                                                </td>                                            
                                                <td style="width: 25%"><strong>Review Date: </strong>
                                                    <asp:TextBox ID="txtReviewDate" class="form-control" Width="100px" AutoComplete="off" runat="server"></asp:TextBox>
                                                    <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtReviewDate" Format="dd/MM/yyyy"></ajaxToolkit:CalendarExtender>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>

                                <div class="row clearfix">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <strong>Customer Name: </strong>
                                            <asp:Label ID="lblCustomerName" runat="server"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <strong>Address</strong><br />
                                            <asp:Label ID="lblAddressLine1" runat="server"></asp:Label><br />
                                            <asp:Label ID="lblAddressLine2" runat="server"></asp:Label><br />
                                            <asp:Label ID="lblDistrict" runat="server"></asp:Label>
                                            -
                                            <asp:Label ID="lblPinCode" runat="server"></asp:Label>,<asp:Label ID="lblState" runat="server"></asp:Label>
                                            -
                                            <asp:Label ID="lblCountry" runat="server"></asp:Label>
                                        </div>
                                    </div>
                                </div>

                                <div class="row clearfix">
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <strong>Delivery of Test report (approx.)</strong>
                                               <asp:TextBox ID="txtDeliveryDate" class="form-control" Width="100px" AutoComplete="off" runat="server"></asp:TextBox>
                                                    <ajaxToolkit:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtDeliveryDate" Format="dd/MM/yyyy"></ajaxToolkit:CalendarExtender>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <strong>Remarks</strong>
                                                <asp:TextBox ID="txtRemarks" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row clearfix">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <strong>Equipment Condition (OK or not)</strong>
                                                <asp:RadioButtonList ID="radEquip" runat="server" RepeatDirection="Horizontal">
                                                    <asp:ListItem>Ok</asp:ListItem>
                                                    <asp:ListItem>Not Ok</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <strong>Manpower Availability(OK or not)</strong>
                                                <asp:RadioButtonList ID="radManpower" runat="server" RepeatDirection="Horizontal">
                                                    <asp:ListItem>Ok</asp:ListItem>
                                                    <asp:ListItem>Not Ok</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row clearfix">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <strong>Reagent/Std Availability(OK or not)</strong>
                                                <asp:RadioButtonList ID="radReagent" runat="server" RepeatDirection="Horizontal">
                                                    <asp:ListItem>Ok</asp:ListItem>
                                                    <asp:ListItem>Not Ok</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <strong>Test Method/Specification (OK or not)</strong>
                                                <asp:RadioButtonList ID="radTestMethod" runat="server" RepeatDirection="Horizontal">
                                                    <asp:ListItem>Ok</asp:ListItem>
                                                    <asp:ListItem>Not Ok</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div style="text-align: center">
                                    <asp:Button ID="btnAccept" runat="server" Text="Accept" OnClick="btnAccept_Click" CssClass="btn btn-success" />&nbsp;
                                    <asp:Button ID="btnReject" runat="server" Text="Reject" OnClick="btnReject_Click" CssClass="btn btn-danger" />
                                </div>

                            </div>

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

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="SampleVerification.aspx.cs" Inherits="LIMS.SampleManagement" %>

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

    <style>
        .gridview {
            width: 100%;
            border-collapse: collapse;
        }

            .gridview th, .gridview td {
                border: 1px solid #ddd;
                padding: 8px;
                text-align: center;
            }

            .gridview th {
                background-color: #b34788;
                color: white;
            }

            .gridview tr:nth-child(even) {
                background-color: #f2f2f2;
            }

            .gridview tr:hover {
                background-color: #ddd;
            }

        .img-thumbnail {
            max-width: 100px;
            max-height: 100px;
            border: 1px solid #ddd;
            border-radius: 4px;
            padding: 5px;
        }

        .delete-button {
            background-color: #b34788;
            color: white;
            border: none;
            padding: 5px 10px;
            border-radius: 3px;
            cursor: pointer;
        }

            .delete-button:hover {
                background-color: #b34788;
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
                                                <asp:Label ID="lblEnquiryId" runat="server"></asp:Label>
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
                                                <asp:TextBox ID="txtSamplingLocation" class="form-control" placeholder="Sampling Location" runat="server"></asp:TextBox>
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
                                                <asp:TextBox ID="txtTransportReceiptTemp" class="form-control" placeholder="Transport & Receipt Temp." runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <asp:TextBox ID="txtHumidity" class="form-control" placeholder="Humidity" runat="server"></asp:TextBox>
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
                                                <strong>Sampling Method</strong>
                                                <asp:DropDownList ID="ddlSamplingMethod" runat="server" CssClass="form-control"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div class="form-line">
                                                <strong>Delivery of Test report (approx.)</strong>
                                                <asp:TextBox ID="txtDeliveryDate" class="form-control" Width="100px" AutoComplete="off" runat="server"></asp:TextBox>
                                                <asp:CalendarExtender ID="CalendarExtender3" runat="server" TargetControlID="txtDeliveryDate" Format="dd/MM/yyyy"></asp:CalendarExtender>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row clearfix">
                                    <div id="divCheckBox" runat="server">
                                        <div id="checklist1" runat="server">
                                            <div class="col-sm-2">
                                                <div class="form-group">
                                                    <asp:RadioButtonList ID="rbCheckList" CssClass="rbWidth" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" OnSelectedIndexChanged="rbCheckList_SelectedIndexChanged" RepeatColumns="2" Visible="false"></asp:RadioButtonList>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-sm-5">
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
                                            </div><br />
                                            <div class="form-group">
                                                <asp:TextBox ID="txt7" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="form-group">
                                           
                                        </div>
                                    </div>                                    
                                </div>
                                <div class="row clearfix">
                                        <div class="col-sm-2">
                                            <div class="form-group" style="margin-left: 20px;">
                                                <label>Image Name: </label>
                                                <asp:TextBox ID="txtSampleName" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-sm-2">
                                            <asp:FileUpload ID="fpSampleImage" runat="server" Width="100%" Style="margin-top: 23px;" EnableViewState="true" />
                                        </div>
                                        <div class="col-sm-2">
                                            <asp:Button ID="btnUpload" runat="server" Text="Upload Image" OnClick="btnUpload_Click" CssClass="btn btn-info" Style="margin-top: 20px;" />
                                        </div>
                                    </div>

                                 <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>

                                    <%--<asp:GridView ID="GridView1" runat="server"
                                        AutoGenerateColumns="false"
                                        CssClass="gridview"
                                        OnRowCommand="GridView1_RowCommand"
                                        OnRowDataBound="GridView1_RowDataBound">
                                        <Columns>
                                            <asp:TemplateField HeaderText="#">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRowNumber" runat="server"
                                                        Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Image">
                                            <ItemTemplate>
                                                <asp:Image ID="imgSample" runat="server"
                                                    CssClass="img-thumbnail"
                                                    AlternateText='<%# Eval("SampleText") %>' />
                                                <br />
                                                <asp:Label ID="lblFileName" runat="server"
                                                    Text='<%# Eval("filename") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="SampleText" HeaderText="Sample Name" />

                                        <asp:TemplateField HeaderText="Action">
                                            <ItemTemplate>
                                                <asp:Button ID="btnDelete" runat="server"
                                                    Text="Delete"
                                                    CssClass="delete-button"
                                                    CommandName="DeleteRow"
                                                    CommandArgument='<%# Eval("ID") %>'
                                                    OnClientClick="return confirm('Are you sure you want to delete this image?');" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    </asp:GridView>--%>

                                    <asp:GridView ID="GridView1" runat="server"
                                        AutoGenerateColumns="false"
                                        CssClass="gridview"
                                        DataKeyNames="ID, filename"
                                        OnRowCommand="GridView1_RowCommand"
                                        OnRowDataBound="GridView1_RowDataBound">
                                        <Columns>
                                            <asp:TemplateField HeaderText="#">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRowNumber" runat="server"
                                                        Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Image">
                                                <ItemTemplate>
                                                    <asp:Image ID="imgSample" runat="server"
                                                        CssClass="img-thumbnail"
                                                        AlternateText='<%# Eval("SampleText") %>' />
                                                    <br />
                                                    <asp:Label ID="lblFileName" runat="server"
                                                        Text='<%# Eval("filename") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="SampleText" HeaderText="Sample Name" />

                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <asp:Button ID="btnDelete" runat="server"
                                                        Text="Delete"
                                                        CssClass="delete-button"
                                                        CommandName="DeleteRow"
                                                        CommandArgument='<%# Container.DisplayIndex %>'
                                                        OnClientClick="return confirm('Are you sure you want to delete this image?');" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                </asp:GridView>
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
        <Triggers>
            <asp:PostBackTrigger ControlID="btnUpload" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:HiddenField ID="hdSampleHistory" runat="server" />
    <asp:HiddenField ID="hdSampleCategory" runat="server" />
    <asp:HiddenField ID="hdSampleID" runat="server" />
    <asp:HiddenField ID="hdDepartmentID" runat="server" />
</asp:Content>

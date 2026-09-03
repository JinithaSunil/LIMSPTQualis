<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" ValidateRequest="false" AutoEventWireup="true" CodeBehind="CustomerInquiry.aspx.cs" Inherits="LIMS.CustomerInquiry" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .table th {
            text-align: center !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <!-- CKEditor -->

    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">

                    <h2>NEW ENQUIRY</h2>
                </div>
                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <div class="row clearfix">
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:DropDownList ID="ddlDivision" runat="server" Width="90%" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:DropDownList ID="ddlDepartment" runat="server" Width="90%" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlDepartment_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-1">
                                    <div class="form-group">
                                        <asp:CheckBox ID="chkAll" runat="server" Text="All" OnCheckedChanged="chkAll_CheckedChanged" AutoPostBack="true" />
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:DropDownList ID="ddlCustomerName" runat="server" Width="90%" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomerName_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row clearfix" id="divGroup" runat="server">
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:DropDownList ID="ddlProductGroup" runat="server" Width="90%" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlProductGroup_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:DropDownList ID="ddlProductSubGroup" runat="server" Width="90%" class="form-control" OnSelectedIndexChanged="ddlProductSubGroup_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row clearfix">
                                <div class="col-sm-8" id="divFormat" runat="server">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:HyperLink ID="hypReport" runat="server"></asp:HyperLink>
                                            <asp:DropDownList ID="ddlReportFormat" runat="server" Width="90%" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlReportFormat_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row clearfix" id="divCategory" runat="server" visible="false">
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:DropDownList ID="ddlTestCategory" runat="server" Width="90%" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlTestCategory_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-4" id="divCableType" runat="server" visible="false">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:DropDownList ID="ddlCableType" runat="server" Width="90%" class="form-control"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:DropDownList ID="ddlStandard" runat="server" Width="90%" CssClass="form-control" OnSelectedIndexChanged="ddlStandard_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row clearfix" id="divStandard" runat="server" visible="false">
                                <div class="col-12">
                                    <div class="form-group">
                                        <div class="form-line">

                                            <asp:GridView ID="gvTests" runat="server" DataKeyNames="ReportID" OnRowCommand="gvTests_RowCommand" OnRowDataBound="gvTests_RowDataBound" AutoGenerateColumns="false" class="table table-bordered table-striped table-hover js-basic-example dataTable">
                                                <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Test Name" ItemStyle-Width="29%" ItemStyle-HorizontalAlign="left">
                                                        <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblReportID" runat="server" Text='<%#Eval("ReportID") %>' Visible="false"></asp:Label>
                                                            <asp:CheckBox ID="chkTest" runat="server" Text='<%#Eval("ReportName") %>' Checked="true" OnCheckedChanged="chkTest_CheckedChanged" AutoPostBack="true" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Test Method" ItemStyle-Width="29%" ItemStyle-HorizontalAlign="Center">
                                                        <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtMethod" runat="server" Width="250" Style="text-align: center;"></asp:TextBox>
                                                            <asp:Label ID="lblMethod" runat="server" Text='<%#Eval("TestMethodID") %>' Visible="false"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Part" ItemStyle-Width="14%" ItemStyle-HorizontalAlign="Center">
                                                        <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtPart" runat="server" Text="-" Style="text-align: center;"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Requirement" ItemStyle-Width="14%" ItemStyle-HorizontalAlign="Center">
                                                        <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtRequirement" runat="server" Text="-" Style="text-align: center;"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Remarks" ItemStyle-Width="14%" ItemStyle-HorizontalAlign="Center">
                                                        <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtRemarks" runat="server" Style="text-align: center;"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>

                                            <!-- Selected Button -->
                                            <div class="row mt-3">
                                                <div class="col-12 text-center">
                                                    <asp:Button ID="btnAddSelected" runat="server" Text="Add Selected Tests"
                                                        CssClass="btn btn-primary" OnClick="btnAddSelected_Click" Visible="false" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="row" id="divCalibInstruments" runat="server" visible="false">
                                <asp:CheckBoxList ID="chkInstrument" runat="server"></asp:CheckBoxList>
                                <div class="row mt-3">
                                    <div class="col-12 text-center">
                                        <asp:Button ID="btnAdd" runat="server" Text="Add Selected Instruments"
                                            CssClass="btn btn-primary" OnClick="btnAdd_Click" Visible="true" />
                                        <asp:HiddenField ID="hdSelectedInstruments" runat="server" />
                                        <div class="row clearfix mt-4" id="divInstruments" runat="server" visible="false" style="margin-left:10px">
                                            <div class="col-sm-9">
                                                <div class="form-group">
                                                    <div class="form-line">
                                                        <h4>Selected Instruments</h4>
                                                        <asp:GridView ID="gvPreview" runat="server"
                                                            AutoGenerateColumns="false" class="table table-bordered table-striped table-hover"
                                                            OnRowCommand="gvPreview_RowCommand">

                                                            <Columns>
                                                                <asp:BoundField DataField="GroupName" HeaderText="Group" />
                                                                <asp:BoundField DataField="SubGroupName" HeaderText="Sub Group" />
                                                                <asp:BoundField DataField="InstrumentNames" HeaderText="Instruments" />

                                                                <asp:TemplateField HeaderText="Action">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="btnRemove"
                                                                            runat="server"
                                                                            Text="Remove"
                                                                            CommandName="RemoveRow"
                                                                            CommandArgument="<%# Container.DataItemIndex %>"
                                                                            OnClientClick="return confirm('Remove this entry?');" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>

                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- Selected Tests Grid -->
                            <div class="row clearfix mt-4" id="selectedtests" runat="server" visible="false">
                                <div class="col-12">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <h4>Selected Tests</h4>
                                            <asp:GridView ID="gvSelectedTests" runat="server" DataKeyNames="ReportID"
                                                AutoGenerateColumns="false" class="table table-bordered table-striped table-hover">
                                                <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Test Name" ItemStyle-Width="25%" ItemStyle-HorizontalAlign="left">
                                                        <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSelectedReportID" runat="server" Text='<%#Eval("ReportID") %>' Visible="false"></asp:Label>
                                                            <asp:Label ID="lblSelectedTestName" runat="server" Text='<%#Eval("ReportName") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Standard" ItemStyle-Width="12%" ItemStyle-HorizontalAlign="Center">
                                                        <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSelectedStandardID" runat="server" Text='<%#Eval("StandardID") %>' Visible="false"></asp:Label>
                                                            <asp:Label ID="lblSelectedStandard" runat="server" Text='<%#Eval("Standard") %>' Style="text-align: center;"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Test Method" ItemStyle-Width="25%" ItemStyle-HorizontalAlign="Center">
                                                        <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSelectedMethod" runat="server" Text='<%#Eval("TestMethod") %>' Style="text-align: center;"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Part" ItemStyle-Width="12%" ItemStyle-HorizontalAlign="Center">
                                                        <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSelectedPart" runat="server" Text='<%#Eval("Part") %>' Style="text-align: center;"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Requirement" ItemStyle-Width="12%" ItemStyle-HorizontalAlign="Center">
                                                        <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSelectedRequirement" runat="server" Text='<%#Eval("Requirement") %>' Style="text-align: center;"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Remarks" ItemStyle-Width="12%" ItemStyle-HorizontalAlign="Center">
                                                        <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSelectedRemarks" runat="server" Text='<%#Eval("Remarks") %>' Style="text-align: center;"></asp:Label>
                                                            <asp:Label ID="lblCategoryTypeStd" runat="server" Text='<%#Eval("CategoryTypeStd") %>' Style="text-align: center;" Visible="false"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Action" ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Center">
                                                        <HeaderStyle HorizontalAlign="Center" CssClass="text-center" />
                                                        <ItemTemplate>
                                                            <asp:Button ID="btnRemove" runat="server" Text="Remove" CssClass="btn btn-sm btn-danger"
                                                                CommandName="RemoveTest" CommandArgument='<%#Eval("ReportID") %>'
                                                                OnCommand="btnRemove_Command" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row clearfix">
                                <div class="col-sm-12">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <asp:TextBox ID="txtInquiryDetail" AutoPostBack="true" runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control" placeholder="Inquiry Details" MaxLength="1000"></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="rvInquiry" runat="server" ControlToValidate="txtInquiryDetail" ValidationExpression="^[\s\S]{0,5000}$" ErrorMessage="Please enter a maximum of 1000 characters" Display="Dynamic"></asp:RegularExpressionValidator>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div class="row clearfix">
                        <div class="form-group form-float" style="padding-left: 30px;">
                            <div class="center-block">
                                <asp:Button ID="btnSave" runat="server" class="btn bg-violet m-t-15 waves-effect" Text="Save" OnClick="btnSave_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                            </div>
                        </div>
                    </div>
                </div>
                <div id="divError" runat="server" visible="false" class="alert alert-danger">
                    <strong>Data Not Saved!</strong>
                    <asp:Label ID="lblMsg" runat="server"></asp:Label>
                </div>
            </div>
        </div>
        <asp:HiddenField ID="hdcustomerMasterID" runat="server" />
        <asp:HiddenField ID="hdcustomerID" runat="server" />
        <asp:HiddenField ID="hdTests" runat="server" />
        <asp:HiddenField ID="hdMethods" runat="server" />
        <asp:HiddenField ID="hdParts" runat="server" />
        <asp:HiddenField ID="hdReqmts" runat="server" />
        <asp:HiddenField ID="hdRemarks" runat="server" />
    </div>
    <!-- #END# CKEditor -->

    <%--<script src="plugins/ckeditor/ckeditor.js"></script>    
    <script src="js/editors.js"></script>--%>
    <script src="tinymce/jscripts/tiny_mce/tiny_mce.js"></script>
    <script type="text/javascript">

        tinyMCE.init({

            mode: "textareas",
            elements: "txtInquiryDetail",
            encoding: 'xml',
            width: "650"

        });
    </script>

</asp:Content>

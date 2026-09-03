<%@ Page Title="" Language="C#" MasterPageFile="~/CRF.Master" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeBehind="CloneCustomerFM_Chemical.aspx.cs" Inherits="LIMS.CloneCustomerFM_Chemical" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .GridViewEditRow input[type=text] {
            width: 100px;
        }
        /* size textboxes */
        .GridViewEditRow select {
            width: 100px;
        }
        /* size drop down lists */
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="bg-container-contact100" style="background-image: url(assets/img/bg-01.jpg);">
        <div class="contact100-header flex-sb-m"></div>
    </div>
    <div>
        <div>
            <div class="container-contact100">
                <div style="background-color: white !important">
                    <div>
                        <span style="float: left; padding-left: 20px">
                            <br />
                            <br />
                            <br />
                            <img src="assets/img/qualis-2.png" height="70" /></span>
                        <h2 style="text-align: center; padding-top: 40px; clear: both; margin-bottom: 30px;">TESTING APPLICATION FORM</h2>
                    </div>

                    <form runat="server" class="contact100-form validate-form" data-dpmaxz-eid="1">
                        <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <fieldset>
                                    <div id="divCRF" runat="server" class="row clearfix">
                                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">

                                            <!-- Customer Information Section -->
                                            <div class="card">
                                                <hr class="violet">
                                                <div class="header">
                                                    <h3>Customer Information</h3>
                                                </div>
                                                <div class="body">
                                                    <div class="row clearfix">
                                                        <div class="col-sm-6">
                                                            <div class="form-group">
                                                                <div class="form-line disabled">
                                                                    <b>Date of Request:</b>
                                                                    <asp:TextBox ID="txtDateOfRequest" class="form-control" AutoComplete="off" placeholder="Date Of Request" runat="server" Enabled="false"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-sm-6">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Enquiry ID:</b>
                                                                    <asp:Label ID="lblInquiry" runat="server" CssClass="form-control" Style="border: 1px solid #ddd; padding: 8px; background: #f9f9f9;"></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row clearfix">
                                                        <div class="col-sm-6">
                                                            <div class="form-group">
                                                                <div class="form-line disabled">
                                                                    <b>Customer:</b>
                                                                    <asp:Label ID="txtCustomer" runat="server" CssClass="form-control" Style="border: 1px solid #ddd; padding: 8px; background: #f9f9f9;"></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-sm-6">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Address:</b>
                                                                    <asp:Label ID="txtAddress" runat="server" CssClass="form-control" Style="border: 1px solid #ddd; padding: 8px; background: #f9f9f9; min-height: 60px;"></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row clearfix">
                                                        <div class="col-sm-6">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Contact Person:</b>
                                                                    <asp:Label ID="lblPerson" runat="server" CssClass="form-control" Style="border: 1px solid #ddd; padding: 8px; background: #f9f9f9;"></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-sm-6">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Contact Number:</b>
                                                                    <asp:Label ID="txtPhone" runat="server" CssClass="form-control" Style="border: 1px solid #ddd; padding: 8px; background: #f9f9f9;"></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row clearfix">
                                                        <div class="col-sm-6">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Mobile:</b>
                                                                    <asp:Label ID="txtMobile" runat="server" CssClass="form-control" Style="border: 1px solid #ddd; padding: 8px; background: #f9f9f9;"></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-sm-6">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Email:</b>
                                                                    <asp:Label ID="txtEmail" runat="server" CssClass="form-control" Style="border: 1px solid #ddd; padding: 8px; background: #f9f9f9;"></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row clearfix">
                                                        <div class="col-sm-6">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Designation:</b>
                                                                    <asp:Label ID="txtDesignation" runat="server" CssClass="form-control" Style="border: 1px solid #ddd; padding: 8px; background: #f9f9f9;"></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-sm-6">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Department:</b>
                                                                    <asp:Label ID="txtDepartment" runat="server" CssClass="form-control" Style="border: 1px solid #ddd; padding: 8px; background: #f9f9f9;"></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row clearfix">
                                                        <div class="col-sm-6">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Shipping Agency:</b>
                                                                    <asp:Label ID="txtShippingAgency" runat="server" CssClass="form-control" Style="border: 1px solid #ddd; padding: 8px; background: #f9f9f9;"></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-sm-6">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Airway Bill#:</b>
                                                                    <asp:Label ID="txtAirwayBillNo" runat="server" CssClass="form-control" Style="border: 1px solid #ddd; padding: 8px; background: #f9f9f9;"></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <br />

                                            <!-- Customer Request Form Section -->
                                            <div class="card">
                                                <hr class="violet">
                                                <div class="header">
                                                    <h3>Customer Request Form</h3>
                                                </div>
                                                <div class="body">
                                                    <div class="row clearfix">
                                                        <div class="col-sm-6">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Sample Name:</b>
                                                                    <asp:Label ID="lblSampleName" runat="server" CssClass="form-control" Style="border: 1px solid #ddd; padding: 8px; background: #f9f9f9;"></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-sm-6">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Batch#:</b>
                                                                    <asp:Label ID="lblBatchNo" runat="server" CssClass="form-control" Style="border: 1px solid #ddd; padding: 8px; background: #f9f9f9;"></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row clearfix">
                                                        <div class="col-sm-6">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Sample Qty#:</b>
                                                                    <asp:Label ID="lblSampleQty" runat="server" CssClass="form-control" Style="border: 1px solid #ddd; padding: 8px; background: #f9f9f9;"></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-sm-6">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Requirement:</b>
                                                                    <asp:Label ID="lblRequirement" runat="server" CssClass="form-control" Style="border: 1px solid #ddd; padding: 8px; background: #f9f9f9;"></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row clearfix">
                                                        <div class="col-sm-12">
                                                            <div style="overflow-x: auto;">
                                                                <asp:GridView ID="gvCustomerCRF" runat="server" AutoGenerateColumns="false"
                                                                    CssClass="table table-bordered table-striped table-hover"
                                                                    BorderWidth="1px" CellPadding="8" Width="100%"
                                                                    OnRowCancelingEdit="gvCustomerCRF_RowCancelingEdit"
                                                                    OnRowDeleting="gvCustomerCRF_RowDeleting"
                                                                    OnRowEditing="gvCustomerCRF_RowEditing"
                                                                    OnRowUpdating="gvCustomerCRF_RowUpdating"
                                                                    OnRowCommand="gvCustomerCRF_RowCommand">
                                                                    <HeaderStyle Font-Bold="True" BackColor="#B34788" ForeColor="White" Height="40px" />
                                                                    <RowStyle Height="35px" />
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Test Parameter" ItemStyle-Width="150px">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblTestParameter" runat="server" Text='<%#Eval("TestParameter") %>'></asp:Label>
                                                                                <asp:Label ID="lblTestParameterID" runat="server" Visible="false" Text='<%#Eval("TestParameterID") %>'></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Sub Parameter" ItemStyle-Width="150px">
                                                                            <ItemTemplate>
                                                                                <%# Eval("TestSubParameter") + (String.IsNullOrEmpty(Eval("TestSubSubParameter").ToString()) ? "" : " -> " + Eval("TestSubSubParameter")) %>
                                                                                <asp:Label ID="lblTestSubParameter" runat="server" Text='<%#Eval("TestSubParameter") %>' Visible="false"></asp:Label>
                                                                                <asp:Label ID="lblTestSubParameterID" runat="server" Text='<%#Eval("TestSubParameterID") %>' Visible="false"></asp:Label>
                                                                                <asp:Label ID="lblTestSubSubParameter" runat="server" Text='<%#Eval("TestSubSubParameter") %>' Visible="false"></asp:Label>
                                                                                <asp:Label ID="lblTestSubSubParameterID" runat="server" Text='<%#Eval("TestSubSubParameterID") %>' Visible="false"></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:BoundField DataField="TestMethod" HeaderText="Test Method" ItemStyle-Width="180px" />
                                                                        <asp:BoundField DataField="UnitOfMeasurement" HeaderText="UOM" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center" />
                                                                        <asp:BoundField DataField="CASNo" HeaderText="CASNo" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center" />
                                                                        <asp:BoundField DataField="MDL" HeaderText="M.D.L" ItemStyle-Width="80px" ItemStyle-HorizontalAlign="Center" />
                                                                        <asp:BoundField DataField="CodeCombination" HeaderText="Code Combination" ItemStyle-Width="120px" />
                                                                        <asp:BoundField DataField="Specification" HeaderText="Specification" ItemStyle-Width="120px" />
                                                                        <asp:BoundField DataField="Remarks" HeaderText="Remarks" ItemStyle-Width="120px" />
                                                                        <asp:CommandField ShowEditButton="true" ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Center" />
                                                                        <asp:CommandField ShowDeleteButton="true" ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Center" />
                                                                    </Columns>
                                                                    <FooterStyle BackColor="White" ForeColor="#333333" />
                                                                    <PagerStyle BackColor="#b34788" ForeColor="White" HorizontalAlign="Center" />
                                                                    <RowStyle BackColor="White" ForeColor="#333333" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    <SelectedRowStyle BackColor="#339966" Font-Bold="True" ForeColor="White" />
                                                                    <SortedAscendingCellStyle BackColor="#F7F7F7" />
                                                                    <SortedAscendingHeaderStyle BackColor="#487575" />
                                                                    <SortedDescendingCellStyle BackColor="#E5E5E5" />
                                                                    <SortedDescendingHeaderStyle BackColor="#275353" />
                                                                    <EditRowStyle CssClass="GridViewEditRow" />
                                                                </asp:GridView>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <br />

                                            <!-- Scope of Contract Section -->
                                            <div class="card">
                                                <hr class="violet">
                                                <div class="header">
                                                    <h3>Scope of Contract</h3>
                                                </div>
                                                <div class="body">
                                                    <div class="row clearfix">
                                                        <div class="col-sm-4">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Sample Name:</b>
                                                                    <asp:TextBox ID="txtSampleName" class="form-control" AutoComplete="off" placeholder="Sample Name" runat="server"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-sm-4">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Batch No/ID:</b>
                                                                    <asp:TextBox ID="txtBatchNo" class="form-control" AutoComplete="off" placeholder="Batch No/ID" runat="server"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-sm-4">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Size/Nos of Sample:</b>
                                                                    <asp:TextBox ID="txtSizeofSample" runat="server" class="form-control" AutoComplete="off" placeholder="Size / Nos of sample"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row clearfix">
                                                        <div class="col-sm-4">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Requirement:</b>
                                                                    <asp:DropDownList ID="ddlRequirement" runat="server" CssClass="form-control"></asp:DropDownList>
                                                                    <asp:HiddenField ID="hdRequirement" runat="server" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-sm-4">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Test Parameter:</b>
                                                                    <asp:TextBox ID="txtTestParameter" class="form-control" placeholder="Test Parameter" runat="server" OnTextChanged="txtTestParameter_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                    <asp:Label ID="lblTestParameterID" runat="server" Visible="false"></asp:Label>
                                                                    <asp:AutoCompleteExtender ServiceMethod="GetTestParameters" MinimumPrefixLength="1"
                                                                        CompletionInterval="10" EnableCaching="false" CompletionSetCount="1" TargetControlID="txtTestParameter"
                                                                        ID="AutoCompleteExtender2" runat="server" FirstRowSelected="false">
                                                                    </asp:AutoCompleteExtender>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-sm-4">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Sub Parameter:</b>
                                                                    <asp:DropDownList ID="ddlTestSubParameter" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlTestSubParameter_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                    <asp:Label ID="lblTestSubParameterID" runat="server" Visible="false"></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row clearfix">
                                                        <div class="col-sm-4">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Sub Sub Parameter:</b>
                                                                    <asp:DropDownList ID="ddlTestSubSubParameter" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlTestSubSubParameter_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                    <asp:Label ID="lblTestSubSubParameterID" runat="server" Visible="false"></asp:Label>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-sm-4">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Test Method:</b>
                                                                    <asp:DropDownList ID="ddlTestMethod" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlTestMethod_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-sm-4">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Unit:</b>
                                                                    <asp:DropDownList ID="ddlUOM" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlUOM_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row clearfix">
                                                        <div class="col-sm-4">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>CASNo:</b>
                                                                    <asp:TextBox ID="txtCASNo" runat="server" CssClass="form-control" disabled></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-sm-4">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>M.D.L:</b>
                                                                    <asp:TextBox ID="txtMDL" runat="server" CssClass="form-control" disabled></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-sm-4">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Code Combination:</b>
                                                                    <asp:TextBox ID="txtCodeCombination" runat="server" CssClass="form-control" disabled></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row clearfix">
                                                        <div class="col-sm-5">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Specification If Any:</b>
                                                                    <asp:TextBox ID="txtSpecification" class="form-control" AutoComplete="off" placeholder="Specification If Any" runat="server"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-sm-5">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    <b>Remarks:</b>
                                                                    <asp:TextBox ID="txtRemarks" class="form-control" AutoComplete="off" placeholder="Remarks" runat="server"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div class="col-sm-2">
                                                            <div class="form-group">
                                                                <div class="form-line" style="text-align: right;" >
                                                                    <asp:Button ID="btnAdd" runat="server" class="btn bg-teal m-t-15 waves-effect" Style="background: #B34788; color: #fff; padding: 10px 20px; border: none; border-radius: 3px;" Text=" + " OnClick="btnAdd_Click" />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                                    <div class="row clearfix" style="margin-top: 30px;">
                                                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                            <ContentTemplate>
                                                                <fieldset>
                                                                    <div style="text-align: center; padding-left: 15px; display: flex; justify-content: center; gap: 10px;">
                                                                        <asp:Button ID="btnSubmit" runat="server" class="contact100-form-btn" Text="Save" OnClick="btnSubmit_Click"
                                                                            Style="background: #B34788; color: white; padding: 12px 30px; border: none; border-radius: 5px; font-size: 16px; font-weight: bold;" />

                                                                        <asp:Button ID="btnCancel" runat="server" class="contact100-form-btn" Text="Cancel" OnClick="btnCancel_Click"
                                                                            Style="background: #808080; color: white; padding: 12px 30px; border: none; border-radius: 5px; font-size: 16px; font-weight: bold;" />
                                                                    </div>
                                                                </fieldset>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row clearfix">
                                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                            <div id="divFail" runat="server" visible="false" class="card" style="text-align: center; background: #d4edda; border: 1px solid #c3e6cb; color: #155724;">
                                                <div class="body">
                                                    <h4 style="margin: 40px 0;">REQUEST FORM RECEIVED.<br />
                                                        <br />
                                                        For any changes contact Admin.
                                                    </h4>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </fieldset>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:HiddenField ID="hdInquiryId" runat="server" />
                        <asp:HiddenField ID="hdPersonID" runat="server" />
                    </form>
                </div>
            </div>
        </div>
    </div>
    <script src="js/jquery-3.2.1.min.js"></script>
    <script src="js/animsition.js"></script>
    <script src="js/popper.js"></script>
    <script src="js/bootstrap.min.js"></script>
    <script src="js/select2.min.js"></script>
    <script src="js/moment.min.js"></script>
    <script src="js/daterangepicker.js"></script>
    <script src="js/countdowntime.js"></script>
    <script src="js/main.js"></script>
    <script async="" src="LIMS/js/js"></script>
</asp:Content>

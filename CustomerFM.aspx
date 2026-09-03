<%@ Page Language="C#" MasterPageFile="~/CRF.Master" AutoEventWireup="true" CodeBehind="CustomerFM.aspx.cs" Inherits="LIMS.CustomerFM" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
                        <h2 style="text-align: center; padding-top: 40px">TESTING APPLICATION FORM</h2>
                    </div>
                    <form runat="server" class="contact100-form validate-form" data-dpmaxz-eid="1">
                        <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <fieldset>
                                    <div id="divCRF" runat="server" class="row clearfix">
                                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                            <div class="card">
                                                <hr class="violet">
                                                <div class="header">
                                                    <div class="card">
                                                        <hr class="violet">
                                                        <div class="header">

                                                            <div class="body">
                                                                <div class="row clearfix">
                                                                    <div class="col-sm-6">
                                                                        <div class="form-group">
                                                                            <div class="form-line disabled">
                                                                                <asp:TextBox ID="txtDateOfRequest" class="form-control" AutoComplete="off" placeholder="Date Of Request" runat="server" Enabled="false"></asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-sm-6">
                                                                        <div class="form-group">
                                                                            <div class="form-line">
                                                                                <%--<asp:DropDownList ID="ddlInquiry" runat="server" AutoPostBack="true" Width="90%" class="form-control" OnSelectedIndexChanged="ddlInquiry_SelectedIndexChanged"></asp:DropDownList>--%>
                                                                                <asp:Label ID="lblInquiry" runat="server" class="form-control"></asp:Label>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <div class="row clearfix">
                                                                    <div class="col-sm-6">
                                                                        <div class="form-group">
                                                                            <div class="form-line disabled">
                                                                                <asp:TextBox ID="txtCustomer" class="form-control disabled" AutoComplete="off" placeholder="CustomerName" runat="server" disabled></asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-sm-6">
                                                                        <div class="form-group">
                                                                            <div class="form-line">
                                                                                <asp:TextBox ID="txtAddress" class="form-control" AutoComplete="off" placeholder="Address" runat="server"></asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                </div>

                                                                <div class="row clearfix">
                                                                    <div class="col-sm-3">
                                                                        <div class="form-group">
                                                                            <div class="form-line">
                                                                                <asp:DropDownList ID="ddlContactPerson" runat="server" AutoPostBack="true" Width="90%" class="form-control" OnSelectedIndexChanged="ddlContactPerson_SelectedIndexChanged"></asp:DropDownList>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-sm-3">
                                                                        <div class="form-group">
                                                                            <div class="form-line">
                                                                                <asp:TextBox ID="txtPhone" class="form-control" AutoComplete="off" AutoPostBack="true" placeholder="Phone" runat="server"></asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-sm-3">
                                                                        <div class="form-group">
                                                                            <div class="form-line">
                                                                                <asp:TextBox ID="txtMobile" class="form-control" AutoComplete="off" placeholder="Fax" runat="server"></asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-sm-3">
                                                                        <div class="form-group">
                                                                            <div class="form-line">
                                                                                <asp:TextBox ID="txtEmail" class="form-control" AutoComplete="off" placeholder="Email" runat="server"></asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                </div>
                                                                <div class="row clearfix">
                                                                    <div class="col-sm-6">
                                                                        <div class="form-group">
                                                                            <div class="form-line">
                                                                                <asp:TextBox ID="txtDesignation" class="form-control" AutoComplete="off" placeholder="Designation" runat="server"></asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-sm-6">
                                                                        <div class="form-group">
                                                                            <div class="form-line">
                                                                                <asp:TextBox ID="txtDepartment" class="form-control" AutoComplete="off" placeholder="Department" runat="server"></asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <div class="row clearfix">
                                                                    <div class="col-sm-6">
                                                                        <div class="form-group">
                                                                            <div class="form-line">
                                                                                <asp:TextBox ID="txtShippingAgency" class="form-control" AutoComplete="off" placeholder="Shipping Agency" runat="server"></asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-sm-6">
                                                                        <div class="form-group">
                                                                            <div class="form-line">
                                                                                <asp:TextBox ID="txtAirwayBillNo" class="form-control" AutoComplete="off" placeholder="AirWay Bill Number" runat="server"></asp:TextBox>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <br />
                                                        <div class="card">
                                                            <hr class="violet">
                                                            <div class="header">

                                                                <h3>Scope of Contract
                                
                                                                </h3>

                                                                <ul class="header-dropdown m-r--5">
                                                                    <li class="dropdown">
                                                                        <a href="javascript:void(0);" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">
                                                                            <i class="material-icons"></i>
                                                                        </a>
                                                                        <ul class="dropdown-menu pull-right">
                                                                            <li><a href="javascript:void(0);">Action</a></li>
                                                                            <li><a href="javascript:void(0);">Another action</a></li>
                                                                            <li><a href="javascript:void(0);">Something else here</a></li>
                                                                        </ul>
                                                                    </li>
                                                                </ul>
                                                            </div>
                                                            <div class="body">
                                                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                                    <ContentTemplate>
                                                                        <fieldset>
                                                                            <br />
                                                                            <table style="width: 100%">
                                                                                <tr>
                                                                                    <td>
                                                                                        <asp:Label ID="lblSampleName" runat="server" Font-Size="Medium" Font-Bold="True"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:Label ID="lblBatchNo" runat="server" Font-Size="Medium" Font-Bold="True"></asp:Label>
                                                                                    </td>
                                                                                    <td>
                                                                                        <asp:Label ID="lblSize" runat="server" Font-Size="Medium" Font-Bold="True"></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            <br />
                                                                            <asp:GridView ID="gvCRF" runat="server" AutoGenerateColumns="False" class="table table-bordered table-striped table-hover js-basic-example dataTable" BorderWidth="1px" CellPadding="4" Width="100%" OnRowCommand="gvCRF_RowCommand">
                                                                                <Columns>
                                                                                    <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="100">
                                                                                        <ItemTemplate>
                                                                                            <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                                                        </ItemTemplate>
                                                                                        <ItemStyle Width="100px"></ItemStyle>
                                                                                    </asp:TemplateField>
                                                                                    <asp:BoundField DataField="TestParameter" HeaderText="Test Parameter" />
                                                                                    <asp:BoundField DataField="TestSubParameter" HeaderText="Test Sub Parameter" />
                                                                                    <asp:BoundField DataField="TestMethod" HeaderText="Test Method" />
                                                                                    <asp:BoundField DataField="Specification" HeaderText="Specification If Any" />
                                                                                    <asp:BoundField DataField="Remarks" HeaderText="Remarks" />
                                                                                    <asp:ButtonField ButtonType="Image" ImageUrl="~/assets/img/del.ico" ControlStyle-Width="15px" CommandName="del" />
                                                                                </Columns>
                                                                                <FooterStyle BackColor="White" ForeColor="#333333" />
                                                                                <PagerStyle BackColor="#b34788" ForeColor="White" HorizontalAlign="Center" />
                                                                                <RowStyle BackColor="White" ForeColor="#333333" Height="50px" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                <SelectedRowStyle BackColor="#339966" Font-Bold="True" ForeColor="White" />
                                                                                <SortedAscendingCellStyle BackColor="#F7F7F7" />
                                                                                <SortedAscendingHeaderStyle BackColor="#487575" />
                                                                                <SortedDescendingCellStyle BackColor="#E5E5E5" />
                                                                                <SortedDescendingHeaderStyle BackColor="#275353" />
                                                                            </asp:GridView>
                                                                        </fieldset>
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                                <br />
                                                                <table style="width: 100%">
                                                                    <tr>
                                                                        <td style="width: 30%; text-align: left">
                                                                            <div class="col-sm-12">
                                                                                <div class="form-group">
                                                                                    <div class="form-line">
                                                                                        <asp:TextBox ID="txtSampleName" class="form-control" AutoComplete="off" placeholder="Sample Name" runat="server"></asp:TextBox>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </td>

                                                                        <td style="width: 30%; text-align: left">
                                                                            <div class="col-sm-12">
                                                                                <div class="form-group">
                                                                                    <div class="form-line">
                                                                                        <asp:TextBox ID="txtBatchNo" class="form-control" AutoComplete="off" placeholder="Batch No/ID" runat="server"></asp:TextBox>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </td>
                                                                        <td style="width: 30%; text-align: left">
                                                                            <div class="col-sm-12">
                                                                                <div class="form-group">
                                                                                    <div class="form-line">
                                                                                        <asp:TextBox ID="txtSizeofSample" runat="server" class="form-control" AutoComplete="off" placeholder="Size / Nos of sample"></asp:TextBox>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </td>
                                                                        <td style="width: 30%; text-align: left">
                                                                            <asp:Button ID="btnAddSample" runat="server" class="btn bg-teal m-t-15 waves-effect" Style="background: #846add; color: #fff; width: 40px;" Text=" + " OnClick="btnAddSample_Click" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <table id="tblTestParam" runat="server">
                                                                    <tr>
                                                                        <td style="width: 30%; text-align: left">
                                                                            <div class="col-sm-12">
                                                                                <div class="form-group">
                                                                                    <div class="form-line">
                                                                                        <asp:TextBox ID="txtTestParameter" class="form-control" placeholder="Test Parameter" runat="server" AutoPostBack="true" OnTextChanged="txtTestParameter_TextChanged"></asp:TextBox>
                                                                                        <asp:AutoCompleteExtender ServiceMethod="GetTestParameters" MinimumPrefixLength="1"
                                                                                            CompletionInterval="10" EnableCaching="false" CompletionSetCount="1" TargetControlID="txtTestParameter"
                                                                                            ID="AutoCompleteExtender2" runat="server" FirstRowSelected="false">
                                                                                        </asp:AutoCompleteExtender>
                                                                                        <%--<asp:DropDownList ID="ddlTestParameter" runat="server" CssClass="form-control"></asp:DropDownList>--%>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </td>
                                                                        <td style="width: 30%; text-align: left">
                                                                            <div class="col-sm-12">
                                                                                <div class="form-group">
                                                                                    <div class="form-line">
                                                                                        <asp:DropDownList ID="ddlTestSubParameter" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlTestSubParameter_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </td>
                                                                        <td style="width: 30%; text-align: left">
                                                                            <div class="col-sm-12">
                                                                                <div class="form-group">
                                                                                    <div class="form-line">
                                                                                        <asp:TextBox ID="txtTestMethod" class="form-control" placeholder="Test Method" runat="server"></asp:TextBox>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </td>


                                                                    </tr>

                                                                    <tr>
                                                                        <td style="width: 30%; text-align: left">
                                                                            <div class="col-sm-12">
                                                                                <div class="form-group">
                                                                                    <div class="form-line">
                                                                                        <asp:TextBox ID="txtSpecification" class="form-control" AutoComplete="off" placeholder="Specification If Any" runat="server"></asp:TextBox>

                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </td>
                                                                        <td style="width: 30%; text-align: left">
                                                                            <div class="col-sm-12">
                                                                                <div class="form-group">
                                                                                    <div class="form-line">
                                                                                        <asp:TextBox ID="txtRemarks" class="form-control" AutoComplete="off" placeholder="Remarks" runat="server"></asp:TextBox>
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                        </td>
                                                                        <td style="width: 30%; text-align: center">
                                                                            <div class="col-sm-12">
                                                                                <div class="form-group">
                                                                                    <asp:Button ID="btnAdd" runat="server" class="btn bg-teal m-t-15 waves-effect" Style="background: #846add; color: #fff; width: 40px;" Text=" + " OnClick="btnAdd_Click" />
                                                                                </div>
                                                                            </div>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                                    <ContentTemplate>
                                                                        <fieldset>
                                                                            <table style="width: 90%">
                                                                                <tr>
                                                                                    <td colspan="2" style="text-align: center; display: block; margin: auto; padding-left: 45%">
                                                                                        <asp:Button ID="btnSubmit" runat="server" class="contact100-form-btn" Text="Submit" OnClick="btnSubmit_Click" />
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </fieldset>
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row clearfix">
                                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                                    <div id="divFail" runat="server" visible="false" class="card" style="text-align: center">
                                                        <br />
                                                        <br />
                                                        <br />
                                                        <h4>REQUEST FORM RECEIVED.<br />
                                                            <br />
                                                            For any changes contact Admin.</h4>
                                                        <div style="margin-bottom: 20%"></div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </fieldset>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:HiddenField ID="hdInquiryId" runat="server" />
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

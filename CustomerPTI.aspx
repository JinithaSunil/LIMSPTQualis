<%@ Page Language="C#" MasterPageFile="~/CRF.Master" AutoEventWireup="true" CodeBehind="CustomerPTI.aspx.cs" Inherits="LIMS.CustomerPTI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="bg-container-contact100" style="background-image: url(assets/img/bg-01.jpg);">
        <div class="contact100-header flex-sb-m">
        </div>
    </div>

    <div>
        <div>
            <div class="container-contact100">
                <div style="background-color: white">
                    <div class="contact100-form-title" style="background-image: url(assets/img/bg-02.jpg);">
                        <span>
                            <img src="images/hemshell-logo.png" /></span>
                    </div>
                    <form runat="server" class="contact100-form validate-form" data-dpmaxz-eid="1">
                        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <fieldset>
                                    <div id="divCRF" runat="server" class="row clearfix">
                                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                            <div class="card"><hr class="violet"><div class="header">
                                                
                                                    <center><h3>CUSTOMER REQUEST FORM</h3></center>
                                                    <br />
                                                </div>
                                                <div class="body">
                                                    <div class="row clearfix">
                                                        <div class="col-sm-6">
                                                            <div class="form-group">
                                                                <div class="form-line">
                                                                    Date of Request:
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
                                        </div>
                                        <br />
                                        <div class="card"><hr class="violet"><div class="header">
                                            
                                                <center><h4>Scope of Contract</h4></center>
                                                <br />
                                            </div>
                                            <div class="body">
                                                <asp:GridView ID="gvSample" runat="server" AutoGenerateColumns="False" 
                                                    CssClass="table table-striped table-bordered table-hover table-checkable order-column" 
                                                    BorderWidth="3px" CellPadding="4" Width="100%" OnRowCommand="gvSample_RowCommand" OnRowDeleting="gvSample_RowDeleting">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="100">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                            </ItemTemplate>
                                                            <ItemStyle Width="100px"></ItemStyle>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="SampleName" HeaderText="Sample Name" />
                                                        <asp:BoundField DataField="SampleType" HeaderText="Sample Type" />
                                                        <asp:BoundField DataField="TestMethod" HeaderText="Test Method" />
                                                        <asp:BoundField DataField="TestParameter" HeaderText="Test Parameter" />
                                                        <asp:BoundField DataField="SizeofSample" HeaderText="Size/ Nos of Sample" />
                                                        <asp:BoundField DataField="Remarks" HeaderText="Remarks" />
                                                        <asp:ButtonField ButtonType="Image" ImageUrl="~/assets/img/del.ico" ControlStyle-Width="35px" CommandName="del" />
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


                                                <div class="row clearfix">
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtSampleName" runat="server" AutoComplete="off" class="form-control" placeholder="SampleName"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <%--<asp:DropDownList ID="ddlSampleType" runat="server" Width="100%" CssClass="form-control">
                                                                    <asp:ListItem>Select Sample Type</asp:ListItem>
                                                                    <asp:ListItem>PET Resin</asp:ListItem>
                                                                    <asp:ListItem>PET Preform</asp:ListItem>
                                                                    <asp:ListItem>Bottles</asp:ListItem>
                                                                </asp:DropDownList>--%>
                                                                <asp:TextBox ID="txtSampleType" runat="server" Width="100%" class="form-control" placeholder="Sample Type"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                     <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtSizeofSample" runat="server" AutoComplete="off" class="form-control" placeholder="Sample Qty"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>                                                                                                       
                                                </div>
                                                <div class="row clearfix">
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtTestParameter" class="form-control" placeholder="Test Parameter" runat="server" AutoPostBack="true"></asp:TextBox>
                                                                <asp:AutoCompleteExtender ServiceMethod="GetTestParameters" MinimumPrefixLength="1"
                                                                    CompletionInterval="10" EnableCaching="false" CompletionSetCount="1" TargetControlID="txtTestParameter"
                                                                    ID="AutoCompleteExtender1" runat="server" FirstRowSelected="false">
                                                                </asp:AutoCompleteExtender>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtTestMethod" class="form-control" placeholder="Test Method" runat="server" AutoPostBack="true"></asp:TextBox>
                                                                <asp:AutoCompleteExtender ServiceMethod="GetTestMethods" MinimumPrefixLength="1"
                                                                    CompletionInterval="10" EnableCaching="false" CompletionSetCount="1" TargetControlID="txtTestMethod"
                                                                    ID="AutoCompleteExtender2" runat="server" FirstRowSelected="false">
                                                                </asp:AutoCompleteExtender>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <%-- <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtPreforms" runat="server" Width="100%" class="form-control" placeholder="For PET Preform"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtBottles" runat="server" Width="100%" class="form-control" placeholder="For Bottles"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>--%>
                                                    <%--  <div class="col-sm-2">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtSizeofSample" runat="server" class="form-control" AutoComplete="off" placeholder="Size / Nos of sample"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>--%>
                                                    <div class="col-sm-4">
                                                        <div class="form-group">
                                                            <div class="form-line">
                                                                <asp:TextBox ID="txtRemarks" runat="server" AutoComplete="off" class="form-control" placeholder="Remarks"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="row clearfix">
                                                    <div class="col-sm-6">
                                                        <div class="form-group">
                                                            <asp:Button ID="btnAdd" runat="server" Style="background: #846add; color: #fff; width: 40px;" class="btn bg-teal m-t-15 waves-effect" OnClick="btnAdd_Click" Text=" + " />
                                                        </div>
                                                    </div>
                                                </div>
                                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                    <ContentTemplate>
                                                        <fieldset>

                                                            <table style="width: 90%">

                                                                <tr>
                                                                    <td>&nbsp;</td>
                                                                    <td align="center">
                                                                        <asp:Button ID="btnSubmit" runat="server" class="contact100-form-btn" Text="Submit" OnClick="btnSubmit_Click" />
                                                                        &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp                                           
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
                                </fieldset>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:HiddenField ID="hdInquiryId" runat="server" />
                    </form>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

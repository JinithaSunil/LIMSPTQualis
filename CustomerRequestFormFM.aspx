<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="CustomerRequestFormFM.aspx.cs" Inherits="LIMS.CustomerRequestFormFM" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="updPanel" runat="server">
        <ContentTemplate>
            <fieldset>
                <div class="row clearfix">
                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <div class="card"><hr class="violet"><div class="header">
                            <div class="card"><hr class="violet"><div class="header">
                                
                                    <h2>Customer Request Form
                                
                                    </h2>
                                    <ul class="header-dropdown m-r--5">
                                        <li class="dropdown">
                                            <a href="javascript:void(0);" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">
                                                <i class="material-icons">more_vert</i>
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
                                    <div class="row clearfix">
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtDateOfRequest" class="form-control" AutoComplete="off" placeholder="Date Of Request" runat="server"></asp:TextBox>
                                                    <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtDateOfRequest" Format="dd/MMM/yyyy"></ajaxToolkit:CalendarExtender>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:DropDownList ID="ddlInquiry" runat="server" AutoPostBack="true" Width="90%" class="form-control" OnSelectedIndexChanged="ddlInquiry_SelectedIndexChanged"></asp:DropDownList>
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
                                                    <asp:DropDownList ID="ddlContactPerson" runat="server" AutoPostBack="true" Width="90%" class="form-control" OnSelectedIndexChanged="ddlContactPerson_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row clearfix">
                                        <div class="col-sm-3">
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtMobile" class="form-control" AutoComplete="off" AutoPostBack="true" placeholder="Mobile" runat="server"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-sm-3">
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtFax" class="form-control" AutoComplete="off" placeholder="Fax" runat="server"></asp:TextBox>
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
                                        <div class="col-sm-3">
                                            <div class="form-group">
                                                <div class="form-line">
                                                    <asp:TextBox ID="txtAddress" class="form-control" AutoComplete="off" placeholder="Address" runat="server"></asp:TextBox>
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
                            <div class="card"><hr class="violet"><div class="header">
                                
                                    <h2>Scope of Contract
                                
                                    </h2>
                                    <ul class="header-dropdown m-r--5">
                                        <li class="dropdown">
                                            <a href="javascript:void(0);" class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">
                                                <i class="material-icons">more_vert</i>
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
                                                <asp:GridView ID="gvCRF" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered table-hover table-checkable order-column" BorderWidth="3px" CellPadding="4" Width="100%" OnRowCommand="gvCRF_RowCommand">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="100">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                            </ItemTemplate>
                                                            <ItemStyle Width="100px"></ItemStyle>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="SampleName" HeaderText="Sample Name" />
                                                        <asp:BoundField DataField="BatchNo" HeaderText="Batch No/ID" />
                                                        <asp:BoundField DataField="TestMethod" HeaderText="Test Method" />
                                                        <asp:BoundField DataField="TestParameter" HeaderText="Test Parameter" />
                                                        <asp:BoundField DataField="Specification" HeaderText="Specification If Any" />
                                                        <asp:BoundField DataField="SizeofSample" HeaderText="Size/ Nos of Sample" />
                                                        <asp:BoundField DataField="Remarks" HeaderText="Remarks" />
                                                        <asp:ButtonField ButtonType="Image" ImageUrl="~/assets/img/del.ico" ControlStyle-Width="35px" CommandName="del" />
                                                    </Columns>
                                                    <FooterStyle BackColor="White" ForeColor="#333333" />
                                                    <HeaderStyle BackColor="#333333" Font-Bold="True" ForeColor="White" />
                                                    <PagerStyle BackColor="#b34788" ForeColor="White" HorizontalAlign="Center" />
                                                    <RowStyle BackColor="White" ForeColor="#333333" />
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
                                            <td style="text-align: left; width: 30%">
                                                <div class="col-sm-12">
                                                    <div class="form-group">
                                                        <div class="form-line">
                                                            <asp:TextBox ID="txtSampleName" class="form-control" AutoComplete="off" placeholder="Sample Name" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </td>

                                            <td style="text-align: left; width: 30%">
                                                <div class="col-sm-12">
                                                    <div class="form-group">
                                                        <div class="form-line">
                                                            <asp:TextBox ID="txtBatchNo" class="form-control" AutoComplete="off" placeholder="Batch No/ID" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="left" width="30%">
                                                <div class="col-sm-12">
                                                    <div class="form-group">
                                                        <div class="form-line">
                                                            <asp:TextBox ID="txtTestMethod" class="form-control" AutoComplete="off" placeholder="Test Method" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </td>

                                            <td align="left" width="30%">
                                                <div class="col-sm-12">
                                                    <div class="form-group">
                                                        <div class="form-line">
                                                            <asp:TextBox ID="txtTestParameter" class="form-control" AutoComplete="off" placeholder="Test Parameter" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="left" width="30%">
                                                <div class="col-sm-12">
                                                    <div class="form-group">
                                                        <div class="form-line">
                                                            <asp:TextBox ID="txtSpecification" class="form-control" AutoComplete="off" placeholder="Specification If Any" runat="server"></asp:TextBox>


                                                        </div>
                                                    </div>
                                                </div>
                                            </td>

                                            <td align="left" width="30%">
                                                <div class="col-sm-12">
                                                    <div class="form-group">
                                                        <div class="form-line">
                                                            <asp:DropDownList ID="ddlSizeofSample" runat="server" Width="90%" class="form-control">
                                                                <asp:ListItem Value="" style="color: darkgray">Select Size of Sample</asp:ListItem>
                                                                <asp:ListItem Value="CFU/gm">CFU/gm</asp:ListItem>
                                                                <asp:ListItem Value="CFU/ml">CFU/ml</asp:ListItem>
                                                                <asp:ListItem Value="CFU/swab">CFU/swab</asp:ListItem>
                                                                <asp:ListItem Value="DegreeBix">Degree Bix</asp:ListItem>
                                                                <asp:ListItem Value="dl/gm">dl/gm</asp:ListItem>
                                                                <asp:ListItem Value="g">g</asp:ListItem>
                                                                <asp:ListItem Value="g/100g">g/100g</asp:ListItem>
                                                                <asp:ListItem Value="g/ml">g/ml</asp:ListItem>
                                                                <asp:ListItem Value="kcal/100g">kcal/100g</asp:ListItem>
                                                                <asp:ListItem Value="lacticAcid/100ml">lacticAcid/100ml</asp:ListItem>
                                                                <asp:ListItem Value="meq/kg">meq/kg</asp:ListItem>
                                                                <asp:ListItem Value="mg/100g">mg/100g</asp:ListItem>
                                                                <asp:ListItem Value="mg/dm2">mg/dm2</asp:ListItem>
                                                                <asp:ListItem Value="mg/kg">mg/kg</asp:ListItem>
                                                                <asp:ListItem Value="mg/l"></asp:ListItem>
                                                                <asp:ListItem Value="min">min</asp:ListItem>
                                                                <asp:ListItem Value="ml/100g">ml/100g</asp:ListItem>

                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="left" width="30%">
                                                <div class="col-sm-12">
                                                    <div class="form-group">
                                                        <div class="form-line">
                                                            <asp:TextBox ID="txtRemarks" class="form-control" AutoComplete="off" placeholder="Remarks" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </td>
                                            <td align="center" width="30%">
                                                <div class="col-sm-12">
                                                    <div class="form-group">
                                                        <asp:Button ID="btnAdd" runat="server" class="btn bg-teal m-t-15 waves-effect" OnClick="btnAdd_Click" Text=" + " />
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div class="col-sm-12">
                                                    <div class="form-group">
                                                        <div class="form-line">
                                                            <asp:TextBox ID="txtCustomerName" class="form-control" AutoComplete="off" placeholder="Customer Name" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </td>
                                            <td>
                                                <div class="col-sm-12">
                                                    <div class="form-group">
                                                        <div class="form-line">
                                                            <asp:TextBox ID="txtCustomerSignature" class="form-control" AutoComplete="off" placeholder="Customer Signature" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                        <ContentTemplate>
                                            <fieldset>
                                                <table width="90%">
                                                    <tr>
                                                        <td>&nbsp;</td>
                                                        <td align="center">
                                                            <asp:Button ID="btnSubmit" runat="server" class="btn bg-teal m-t-15 waves-effect" Text="Save" OnClick="btnSubmit_Click" />
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
                </div>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

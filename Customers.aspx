<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="Customers.aspx.cs" Inherits="LIMS.Customers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Favicon-->
    <link rel="icon" href="../../favicon.ico" type="image/x-icon">

    <!-- Google Fonts -->
    <link href="https://fonts.googleapis.com/css?family=Roboto:400,700&subset=latin,cyrillic-ext" rel="stylesheet" type="text/css">
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet" type="text/css">

    <!-- Bootstrap Core Css -->
    <link href="../../plugins/bootstrap/css/bootstrap.css" rel="stylesheet">

    <!-- Waves Effect Css -->
    <link href="../../plugins/node-waves/waves.css" rel="stylesheet" />

    <!-- Animation Css -->
    <link href="../../plugins/animate-css/animate.css" rel="stylesheet" />

    <!-- JQuery DataTable Css -->
    <link href="../../plugins/jquery-datatable/skin/bootstrap/css/dataTables.bootstrap.css" rel="stylesheet">
	
	

    <!-- Custom Css -->
    <link href="../../css/style.css" rel="stylesheet">

    <!-- AdminBSB Themes. You can choose a theme from css/themes instead of get all themes -->
    <link href="../../css/themes/all-themes.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    
            <!-- Basic Examples -->
            <div class="row clearfix">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="card"><hr class="violet"><div class="header">
                        
                           
                            <div class="row">
                                 <div class="col-sm-6">
                                        <h2>NEW CUSTOMERS</h2>
                                </div>
                                <div class="col-sm-4">
                                  <asp:TextBox ID="txtSearch" runat="server"  class="form-control" placeholder="Search Customer" OnTextChanged="txtSearch_TextChanged" AutoPostBack="true"></asp:TextBox>                                       
                                </div>
                                <div class="col-sm-1">
                                    <asp:ImageButton ID="imgSearch" runat="server" ImageUrl="images/search.png" OnClick="imgSearch_Click" />
                                </div>
                            </div>
                            
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <asp:Panel ID="pnlView" runat="server">
            <asp:GridView ID="gvCustomers" runat="server" AutoGenerateColumns="False" Width="90%" Font-Bold="True" 
                DataKeyNames="CustomerMasterID" class="table table-bordered table-striped table-hover js-basic-example dataTable" 
                OnPageIndexChanging="gvCustomers_PageIndexChanging" AllowPaging="true" PageSize="5" 
                OnRowDataBound="gvCustomers_RowDataBound" AllowSorting="true" OnSorting="gvCustomers_Sorting" >
            <Columns>
                <asp:TemplateField HeaderText = "Sl#">
                    <ItemTemplate>
                        <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                    </ItemTemplate>
                    <ItemStyle Width="5%"></ItemStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Name" SortExpression="FirstName">
                <ItemTemplate>
                <asp:HyperLink ID="hypCustomer" runat="server" Text='<%# Eval("CustomerName").ToString().ToUpper() %>' Target="_blank"></asp:HyperLink> 
                </ItemTemplate>
                </asp:TemplateField>                 
                <asp:BoundField DataField="Address" HeaderText="Address" />  
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
        <asp:Label ID="lblNoRecords" runat="server" Text="No Data!" Font-Bold="True" Font-Size="X-Large" ForeColor="Red" Visible="False"></asp:Label>
            <br />
                                 </asp:Panel>
                             </div>
                        </div>
                    </div>
                </div>
            </div>
            <!-- #END# Basic Examples -->
          

     <!-- Jquery Core Js -->
    <script src="../../plugins/jquery/jquery.min.js"></script>

    <!-- Bootstrap Core Js -->
    <script src="../../plugins/bootstrap/js/bootstrap.js"></script>

    <!-- Select Plugin Js -->
    <script src="../../plugins/bootstrap-select/js/bootstrap-select.js"></script>

    <!-- Slimscroll Plugin Js -->
    <script src="../../plugins/jquery-slimscroll/jquery.slimscroll.js"></script>

    <!-- Waves Effect Plugin Js -->
    <script src="../../plugins/node-waves/waves.js"></script>

    <!-- Jquery DataTable Plugin Js -->
    <script src="../../plugins/jquery-datatable/jquery.dataTables.js"></script>
    <script src="../../plugins/jquery-datatable/skin/bootstrap/js/dataTables.bootstrap.js"></script>
    <script src="../../plugins/jquery-datatable/extensions/export/dataTables.buttons.min.js"></script>
    <script src="../../plugins/jquery-datatable/extensions/export/buttons.flash.min.js"></script>
    <script src="../../plugins/jquery-datatable/extensions/export/jszip.min.js"></script>
    <script src="../../plugins/jquery-datatable/extensions/export/pdfmake.min.js"></script>
    <script src="../../plugins/jquery-datatable/extensions/export/vfs_fonts.js"></script>
    <script src="../../plugins/jquery-datatable/extensions/export/buttons.html5.min.js"></script>
    <script src="../../plugins/jquery-datatable/extensions/export/buttons.print.min.js"></script>

    <!-- Custom Js -->
    <script src="../../js/admin.js"></script>
    <script src="../../js/pages/tables/jquery-datatable.js"></script>

    <!-- Demo Js -->
    <script src="../../js/demo.js"></script>
</asp:Content>
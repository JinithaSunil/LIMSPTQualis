<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="VendorList.aspx.cs" Inherits="LIMS.VendorList" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .gridPager td
{
    text-align: center !important;
}

.gridPager table
{
    margin: 0 auto;
}

        .form-control {
            width: 100%; /* Ensures it takes full width of its parent */
        }

        .auto-style1 {
            display: block;
            font-size: 14px;
            line-height: 1.42857143;
            color: #555;
            border-radius: 4px;
            -webkit-box-shadow: inset 0 1px 1px rgba(0, 0, 0, .075);
            box-shadow: inset 0 1px 1px rgba(0, 0, 0, .075);
            -webkit-transition: border-color ease-in-out .15s, -webkit-box-shadow ease-in-out .15s;
            -o-transition: border-color ease-in-out .15s, box-shadow ease-in-out .15s;
            transition: border-color ease-in-out .15s, box-shadow ease-in-out .15s;
            border: 1px solid #ccc;
            padding: 6px 12px;
            background-color: #fff;
            background-image: none;
        }

        .card .header {
            padding: 20px;
            border-bottom: 1px solid rgba(204, 204, 204, 0.35);
        }

        .card .body {
            padding: 20px;
        }

        hr.violet {
            border-top: 3px solid #7b1fa2;
            margin-top: 0;
        }

        .btn-secondary {
            color: #fff;
            background-color: #6c757d;
            border-color: #6c757d;
            margin-right: 5px;
        }

        .btn-primary {
            color: #fff;
            background-color: #007bff;
            border-color: #007bff;
        }

        .table {
            width: 100%;
            margin-bottom: 1rem;
            background-color: transparent;
        }

        .table-striped tbody tr:nth-of-type(odd) {
            background-color: rgba(0, 0, 0, 0.05);
        }

        .table th, .table td {
            padding: 8px;
            text-align: left;
            border: 1px solid #ddd;
        }

        .table-hover tbody tr:hover {
            background-color: rgba(0, 0, 0, 0.075);
        }

        .padding-0 {
            padding: 0;
        }

        .form-group {
            margin-bottom: 15px;
        }

        .form-line {
            position: relative;
        }

        /* Additional styles from first code */
        .table-striped dt-responsive {
            width: 100% !important;
        }

        .button-group {
            margin: 10px 0;
        }

        .icon-md {
            font-size: 18px;
            margin: 0 10px;
        }

        .btn-group {
            margin-bottom: 20px;
            margin-right: 10px;
        }

        .btn-secondary {
            color: #fff;
            background-color: #6c757d;
            border-color: #6c757d;
            margin-right: 5px;
        }

            .btn-secondary:hover {
                background-color: #5a6268;
                border-color: #545b62;
            }

        .btn-primary.btn-xs {
            padding: 1px 5px;
            font-size: 12px;
            line-height: 1.5;
            border-radius: 3px;
            background-color: #2196F3;
            border-color: #2196F3;
        }

        .btn-danger.btn-xs {
            padding: 1px 5px;
            font-size: 12px;
            line-height: 1.5;
            border-radius: 3px;
        }

        .table {
            margin-bottom: 0;
        }

        .table-hover tbody tr:hover {
            background-color: rgba(0,0,0,.075);
        }

        .table-responsive {
            min-height: .01%;
            overflow-x: auto;
        }

        .hyperlink-column a {
            color: #2196F3;
            text-decoration: none;
        }

            .hyperlink-column a:hover {
                text-decoration: underline;
            }

    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-3">
                            <h2>VENDOR LIST</h2>
                        </div>
                        <div id="divSearch" class="col-sm-7" runat="server">
                            <asp:TextBox ID="txtSearch" runat="server" placeholder="Enter Vendor Name / Code" CssClass="form-control" Width="50%" OnTextChanged="txtSearch_TextChanged" AutoPostBack="true"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <br />

                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <div class="row table-responsive">

                                <div>
                                    Total records: <span id="spanCount" runat="server"></span>
                                    <span style="float: right">
                                        <div class="d-flex align-items-center gap-2">
                                            <span>Show</span>

                                            <asp:DropDownList ID="ddlPageSize" runat="server"
                                                CssClass="form-select"
                                                Style="width: 80px;"
                                                AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged">
                                                <asp:ListItem Text="10" Value="10" />
                                                <asp:ListItem Text="25" Value="25" />
                                                <asp:ListItem Text="50" Value="50" />
                                                <asp:ListItem Text="100" Value="100" />
                                            </asp:DropDownList>

                                            <span>records</span>
                                        </div>
                                    </span>
                                </div>



                                <br />

                                <div class="col-lg-12 col-md-12 col-12 padding-0">
                                    <asp:GridView ID="gvVendor" runat="server" class="table table-bordered table-striped table-hover js-basic-example dataTable" Font-Bold="true"
                                        AutoGenerateColumns="false" OnRowDataBound="gvVendor_RowDataBound"
                                        ShowHeaderWhenEmpty="true" OnRowEditing="gvVendor_RowEditing" 
                                        OnRowUpdating="gvVendor_RowUpdating"
                                        OnRowCancelingEdit="gvVendor_RowCancelingEdit"
                                        PageSize="10" 
                                        OnPageIndexChanging="gvVendor_PageIndexChanging"
                                        AllowPaging="true">

                                        <Columns>
                                            <asp:TemplateField HeaderText="Sl#">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                    <asp:Label ID="lblVendorID" Text='<%#Eval("VendorID") %>' runat="server" Visible="false" />
                                                    <asp:Label ID="lblVendorStatus1" runat="server" Text='<%#Eval("VendorStatus") %>' Visible="false"></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="5%"></ItemStyle>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Vendor Name" SortExpression="VendorName"  ItemStyle-Width="40%">
                                            <ItemTemplate>
                                                <asp:HyperLink ID="hypVendorName" runat="server" Text='<%# Eval("VendorName").ToString().ToUpper() %>' Target="_blank" NavigateUrl='<%# "GenerateVendorDetailsReport.aspx?VendorID="+Eval("VendorID").ToString()+"&s=1" %>'></asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Code" SortExpression="FirstName" ItemStyle-Width="30%">
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="hypVendor" runat="server" Text='<%# Eval("VendorCode").ToString().ToUpper() %>' NavigateUrl='<%# "~/VendorProfile.aspx?ID=" + Eval("VendorCode") %>' Target="_blank"></asp:HyperLink>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <%--<asp:BoundField DataField="State" HeaderText="State" ItemStyle-Width="20%" ReadOnly="true" />
                                                <asp:BoundField DataField="Country" HeaderText="Country" ItemStyle-Width="20%" ReadOnly="true" />--%>
                                            <asp:TemplateField HeaderText="Black Listed">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblVendorStatus" runat="server" Text='<%#Eval("VendorStatus") %>'></asp:Label>
                                                </ItemTemplate>
                                                <EditItemTemplate>
                                                    <asp:RadioButtonList ID="radVendorStatus" runat="server" RepeatDirection="Horizontal">
                                                        <asp:ListItem Value="0">Y</asp:ListItem>
                                                        <asp:ListItem Value="1">N</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </EditItemTemplate>
                                                <ItemStyle Width="20%" />
                                            </asp:TemplateField>
                                            <asp:CommandField ShowEditButton="true" HeaderText="Action" ButtonType="Image" EditImageUrl="~/assets/img/edit.png" ControlStyle-Width="20px" ItemStyle-Width="5%" UpdateImageUrl="~/assets/img/accept1.png" CancelImageUrl="~/assets/img/cross.png" />
                                        </Columns>
                                          <FooterStyle BackColor="White" ForeColor="#333333" HorizontalAlign="Center" />
                                    <PagerStyle BackColor="#b34788" ForeColor="White" HorizontalAlign="Center" CssClass="gridPager" />
                                    <RowStyle BackColor="White" ForeColor="#333333" Height="50px" HorizontalAlign="center" VerticalAlign="Middle" />
                                    <SelectedRowStyle BackColor="#339966" Font-Bold="True" ForeColor="White" />
                                    <SortedAscendingCellStyle BackColor="#F7F7F7" />
                                    <SortedAscendingHeaderStyle BackColor="#487575" />
                                    <SortedDescendingCellStyle BackColor="#E5E5E5" />
                                    <SortedDescendingHeaderStyle BackColor="#275353" />
                                        <EmptyDataTemplate>
                                            <div>No records found.</div>
                                        </EmptyDataTemplate>

                                    </asp:GridView>

                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

            </div>

        </div>


    </div>
</asp:Content>


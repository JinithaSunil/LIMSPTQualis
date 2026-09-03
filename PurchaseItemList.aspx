<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="PurchaseItemList.aspx.cs" Inherits="LIMS.PurchaseItemList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .form-control {
            width: 100%;
        }

        #ddlVAT {
            width: 70%;
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

        /* .table-striped tbody tr:nth-of-type(odd) {
            background-color: rgba(0, 0, 0, 0.05);
        }

        .table th, .table td {
            padding: 8px;
            text-align: left;
            border: 1px solid #ddd;
        }

        .table-hover tbody tr:hover {
            background-color: rgba(0, 0, 0, 0.075);
        }*/

        /* .padding-0 {
            padding: 0;
        }

        .table-striped dt-responsive {
            width: 100% !important;
        }*/

        .header {
            display: flex;
            align-items: center;
            gap: 15px;
            padding: 15px 20px;
            margin-bottom: 0;
            background-color: #fff;
        }

            .header h2 {
                margin: 0;
                font-size: 18px;
                white-space: nowrap;
                min-width: 150px;
            }
        /*
.col-sm-3, .col-sm-4, .col-sm-2, .col-sm-1 {
    padding: 0 10px;
}

.form-control {
    height: 34px;
    border: 1px solid #ddd;
    border-radius: 4px;
    padding: 6px 12px;
}
*/
        /*.header .col-sm-1 {
    display: flex;
    align-items: center;
    padding-top: 5px;
}*/

        /*.violet {
    border-top: 2px solid #e91e63;
    margin: 0;
}*/
    </style>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>


    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="col-sm-2">
                        <h2>PURCHASE ITEMS</h2>
                    </div>
                    <div class="col-sm-3">
                        <asp:DropDownList ID="ddlPurchaseGroup" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="ddlPurchaseGroup_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtSearch" runat="server" class="form-control" placeholder="Search Item Code" OnTextChanged="txtSearch_TextChanged" AutoPostBack="true"></asp:TextBox>
                    </div>
                    <div class="col-sm-3">
                        <asp:TextBox ID="txtSearchText" runat="server" CssClass="form-control" placeholder="Search Description" OnTextChanged="txtSearchText_TextChanged" AutoPostBack="true"></asp:TextBox>
                    </div>
                    <div class="col-sm-1">
                        <asp:ImageButton ID="imgSearch" runat="server" ImageUrl="images/search.png" OnClick="imgSearch_Click" />
                    </div>
                </div>

                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <div class=" row">
                                <br />
                                <br />
                                <div class="col-lg-12 col-md-12 col-12 padding-0" style="width: 100%; ">
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
                                    <asp:GridView ID="gvPurchaseItem" runat="server" AutoGenerateColumns="false" class="table table-bordered table-striped table-hover dt-responsive display" Width="100%"
                                        OnRowCommand="gvPurchaseItem_RowCommand" DataKeyNames="ItemID" OnRowUpdating="gvPurchaseItem_RowUpdating"
                                        OnRowDataBound="gvPurchaseItem_RowDataBound" OnPageIndexChanging="gvPurchaseItem_PageIndexChanging"
                                        AllowPaging="true" AllowCustomPaging="true">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Sl No">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                </ItemTemplate>
                                                <ItemStyle Width="3%"/>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="ItemCode">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblItemCode" runat="server" Text='<%# Eval("ItemCode") %>' Width="100%" Visible="false"></asp:Label>
                                                    <asp:HyperLink ID="hypItemCode" runat="server" Text='<%# Eval("ItemCode") %>' NavigateUrl='<%# Eval("ItemID","PurchaseItemAdd.aspx?ID={0}") %>'></asp:HyperLink>
                                                </ItemTemplate>
                                                <ItemStyle Width="5%" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Description">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("ItemDescription") %>' ToolTip='<%# Eval("ItemDescription") %>' Width="100%"></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="10%" HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="UOM">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblUOM" runat="server" Text='<%# Eval("UOMName") %>' Width="100%"></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="5%" />
                                            </asp:TemplateField>
                                            <%--<asp:TemplateField HeaderText="Conversion UOM">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblConversionUOM" runat="server" Text='<%# Eval("ConversionUOMID") %>' Width="100%"></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="5%" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="HSNCode">
                                                <ItemTemplate>
                                                    <asp:Label ID="txtHSNCode" runat="server" Text='<%# Eval("HSNCode") %>' Width="100%"></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="5%" />
                                            </asp:TemplateField>
                                            <%-- <asp:TemplateField HeaderText="Make">
                                                <ItemTemplate>
                                                    <asp:Label ID="txtMake" runat="server" Text='<%# Eval("Make") %>' Width="100%"></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="10%" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="LeadTime">
                                                <ItemTemplate>
                                                    <asp:Label ID="txtLeadTime" runat="server" Text='<%# Eval("LeadTime") %>' Width="100%"></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="5%" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Model">
                                                <ItemTemplate>
                                                    <asp:Label ID="txtModel" runat="server" Text='<%# Eval("ModelNo") %>' Width="100%"></asp:Label>
                                                    
                                                </ItemTemplate>
                                                <ItemStyle Width="10%" />
                                            </asp:TemplateField>--%>
                                            <asp:TemplateField HeaderText="Action">
                                                <ItemTemplate>
                                                    <div class="btn-group">
                                                        <asp:Label ID="lblDisabled" runat="server" Text='<%# Eval("DisabledItem") %>' Visible="false"></asp:Label>
                                                        <asp:Button ID="btnUpdate" runat="server" Text="Update" CommandName="update" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' CssClass="btn btn-primary" Visible="false" />
                                                        <asp:Button ID="btnDisable" runat="server" Text="Disable" CommandName="disable" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' CssClass="btn btn-danger" />
                                                        <asp:Button ID="btnEnable" runat="server" Text="Enable" CommandName="enable" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' CssClass="btn" BackColor="#3fa08d" />
                                                    </div>
                                                </ItemTemplate>
                                                <ItemStyle Width="10%" />
                                            </asp:TemplateField>
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
                            <!--submit & clear-->
                            <%--   <div class="row clearfix">
                            <div class="col-sm-2">
                                <div class="form-group">
                                    <label>&nbsp;</label>
                                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                                </div>
                            </div>

                            <div class="col-sm-2">
                                <label>&nbsp;</label>
                                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-warning" OnClick="btnClear_Click" />
                            </div>

                        </div>--%>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>


</asp:Content>

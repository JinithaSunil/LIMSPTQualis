<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="CodeList.aspx.cs" Inherits="LIMS.CodeList" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
         .rbl1 [type="radio"]:not(:checked) + label {
     padding-left: 26px;
     height: 25px;
     line-height: 25px;
     font-size: 11px;
     font-weight: normal;
     vertical-align: top;
 }

 .rbl1 [type="radio"]:checked + label {
     padding-left: 26px;
     height: 25px;
     line-height: 25px;
     font-size: 11px;
     font-weight: bold;
     vertical-align: top;
 }
        .form-control {
            width: 100%;
            font-size: 14px;
            line-height: 1.42857143;
            color: #555;
            border-radius: 4px;
            -webkit-box-shadow: inset 0 1px 1px rgba(0, 0, 0, .075);
            box-shadow: inset 0 1px 1px rgba(0, 0, 0, .075);
            -webkit-transition: border-color ease-in-out .15s, -webkit-box-shadow ease-in-out .15s;
            transition: border-color ease-in-out .15s, box-shadow ease-in-out .15s;
            border: 1px solid #ccc;
            padding: 6px 12px;
            background-color: #fff;
        }

        body {
            font-size: 14px;
        }

        .card {
            background: #fff;
            min-height: 50px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
            position: relative;
            margin-bottom: 30px;
            border-radius: 2px;
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
        /*#txtSearch {
    width: 150px;*/ /* Reduce the width of the ItemCode field */
        /*height: 30px;
    font-size: 13px;
    padding: 5px 10px;
    display: inline-block;
}*/
    </style>

    <script type="text/javascript">
        function copyToClipboard(element) {
            var text = element.innerText || element.textContent; // Get hyperlink text
            var tempInput = document.createElement("input");
            tempInput.style.position = "absolute";
            tempInput.style.left = "-9999px";
            tempInput.value = text;
            document.body.appendChild(tempInput);
            tempInput.select();
            document.execCommand("copy");
            document.body.removeChild(tempInput);
            alert("Copied: " + text);
        }
</script>
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
                            <h2>CODE LIST</h2>
                        </div>
                        <div class="col-sm-4">
                            <asp:TextBox ID="txtSearch" runat="server" class="form-control" placeholder="Search Item Code" OnTextChanged="txtSearch_TextChanged" AutoPostBack="true"></asp:TextBox>
                        </div>
                        <div class="col-sm-4">
                            <asp:TextBox ID="txtSearchText" runat="server" CssClass="form-control" placeholder="Search Description" OnTextChanged="txtSearchText_TextChanged" AutoPostBack="true"></asp:TextBox>
                        </div>
                        <div class="col-sm-1">
                            <asp:ImageButton ID="imgSearch" runat="server" ImageUrl="images/search.png" OnClick="imgSearch_Click" />
                        </div>
                    </div>
                </div>

                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <!-- Store Type Selection -->
                            <div class="row clearfix">
                                <div class="col-sm-12" style="vertical-align:top;">
                                    <div class="form-group">
                                            <label for="lblStoreType">Item Type</label>
                                            <asp:RadioButtonList ID="rblStoreType" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="rblStoreType_SelectedIndexChanged" AutoPostBack="true" CssClass="form-control rbl1">
                                            </asp:RadioButtonList>
                                    </div>
                                </div>
                            </div>
                            <br /><br />
                            <!-- Purchase Group and Sub Group Selection -->
                            <div class="row clearfix">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Purchase Group:</label>
                                            <%--        <asp:DropDownList ID="ddlPurchaseGroup" runat="server" CssClass="form-control"  OnSelectedIndexChanged="ddlPurchaseGroup_SelectedIndexChanged"AutoPostBack="true">
                                                </asp:DropDownList>--%>
                                            <asp:DropDownList ID="ddlPurchaseGroup" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlPurchaseGroup_SelectedIndexChanged" AutoPostBack="true">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Purchase Sub Group:</label>
                                            <asp:DropDownList ID="ddlPurchaseSubGroup" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlPurchaseSubGroup_SelectedIndexChanged" AutoPostBack="true">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- GridView Section -->
                            <%--     <div class="row" style="width: auto;">
                                <div class="col-lg-12 col-md-12 col-12 padding-0">
                                    <div style="width: 100%; height: 500px; overflow: auto">
                                        <asp:GridView ID="gvStockList" runat="server" AutoGenerateColumns="false"
                                            class="table table-striped table-hover dt-responsive display"
                                            DataKeyNames="ItemID" Width="100%">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl#">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="3%" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Item Code">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblItemID" runat="server" Text='<%# Eval("ItemID") %>' Visible="false"></asp:Label>
                                                        <asp:LinkButton ID="lnkItemCode" runat="server" Text='<%# Eval("ItemCode") %>'
                                                            CommandName="Select" CommandArgument='<%# Container.DataItemIndex %>' class="btn btn-link"></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="8%" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Description">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDescription" runat="server"
                                                            Text='<%# Eval("ItemDescription") + " " + Eval("Specification") %>'
                                                            ToolTip='<%# Eval("ItemDescription") + " " + Eval("Specification") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="20%" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Stock Qty">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStockQty" runat="server" Text='<%# Eval("StockQuantity") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="7%" HorizontalAlign="Right" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="UOM">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblUOM" runat="server" Text='<%# Eval("UOMID") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="5%" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="HSN Code">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblHSNCode" runat="server" Text='<%# Eval("HSNCode") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="8%" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Model No">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblModelNo" runat="server" Text='<%# Eval("ModelNo") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="8%" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Rate">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRate" runat="server" Text='<%# Eval("Rate", "{0:N2}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="8%" HorizontalAlign="Right" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Min Stock">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMinStock" runat="server" Text='<%# Eval("MinStock") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="7%" HorizontalAlign="Right" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Max Stock">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblMaxStock" runat="server" Text='<%# Eval("MaxStock") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="7%" HorizontalAlign="Right" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Created Date">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCreatedDate" runat="server" Text='<%# Eval("CreatedDate", "{0:dd-MMM-yyyy}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="8%" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>--%>




                            <!-- GridView Section -->
                            <div class="row table-responsive" style="width: auto;">
                                <div class="col-lg-12 col-md-12 col-12 padding-0">
                                    <div  class="table-responsive" style="width: 100%; height: 500px; overflow: auto">
                                        <asp:GridView ID="gvItemList" runat="server" AutoGenerateColumns="false"
                                       CssClass="table table-bordered table-striped table-hover"
                                            OnRowCommand="gvItemList_RowCommand" DataKeyNames="ItemID" Width="100%">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl#">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="5%"></ItemStyle>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Material Code">
                                                    <ItemTemplate>
                                                        <asp:Label ID="txtItemID" runat="server" Text='<%# Eval("ItemID") %>' Visible="false"></asp:Label>
                                                        <asp:LinkButton ID="lnkItemCode" runat="server"
                                                            Text='<%# Eval("ItemCode").ToString() %>'
                                                            CommandName="Select"
                                                            CommandArgument='<%# DataBinder.Eval(Container,"RowIndex") %>'
                                                            OnClientClick="copyToClipboard(this); return false;">
        </asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="5%"></ItemStyle>
                                                </asp:TemplateField>
                                   <%--             <asp:TemplateField HeaderText="Available Stock">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAvailableStock" runat="server" Text='<%# Eval("AvailableStock") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="5%"></ItemStyle>
                                                </asp:TemplateField>--%>
                                                <%--<asp:BoundField DataField="ItemCode" HeaderText="ItemCode" />
                                                    <asp:TemplateField HeaderText="ItemDescription">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblItemDescription" runat="server" Text='<%# Eval("ItemDescription")  %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="5%"></ItemStyle>
                                                    </asp:TemplateField>--%>
                                                <%--     <asp:BoundField DataField="Make" HeaderText="Make" />--%>
                                                <asp:BoundField DataField="ItemDescription" HeaderText="Item" />
                                                <asp:BoundField DataField="UOMName" HeaderText="UOM" />
                                                <asp:BoundField DataField="PurchaseGroupName" HeaderText="Group" />
                                                <asp:BoundField DataField="PurchaseSubGroupName" HeaderText="Sub-Group" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeBehind="FloatEnquiry.aspx.cs" Inherits="LIMS.FloatEnquiry" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        /*  .chk1 input[type="checkbox"] {
    width: 16px;
    height: 16px;
    accent-color: #0d6efd;
}
    input[type="checkbox"] {
    opacity: 0;
    position: absolute;
    width: 0;
    height: 0;
}
*/
        /* hide default checkbox */
        input[type="checkbox"] {
            appearance: none;
            -webkit-appearance: none;
            -moz-appearance: none;
            width: 0;
            height: 0;
            margin: 0;
            position: absolute;
            opacity: 0;
        }
            /* checkbox box */
            input[type="checkbox"] + span {
                position: relative;
                padding-left: 26px;
                line-height: 20px;
                cursor: pointer;
                font-size: 13px;
            }

                /* unchecked box */
                input[type="checkbox"] + span::before {
                    content: "";
                    position: absolute;
                    left: 0;
                    top: 2px;
                    width: 16px;
                    height: 16px;
                    border: 2px solid #666;
                    background: #fff;
                }

            /* checked state */
            input[type="checkbox"]:checked + span::after {
                content: "";
                position: absolute;
                left: 4px;
                top: 6px;
                width: 6px;
                height: 10px;
                border: solid #c2185b;
                border-width: 0 2px 2px 0;
                transform: rotate(45deg);
            }

        .form-control {
            width: 100%;
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

        body {
            font-size: 12px;
        }

        table tbody tr th {
            text-align: center !important;
        }

        .violet {
            border-top: 3px solid #673AB7;
            margin: 0;
        }

        .selected-row {
            background-color: #e0e0e0 !important;
        }

        .table-custom th, .table-custom td {
            text-align: center;
            padding: 8px;
            vertical-align: middle;
        }

        .table-custom th {
            background-color: #0b4b95;
            color: white;
        }

        .table-custom td {
            background-color: #f8f9fa;
        }

        .table-custom tr:hover {
            background-color: #f1f1f1;
        }
    </style>

    <script type="text/javascript">
        function validateFileUpload() {
            var fileUpload = document.getElementById('<%= fpAttach.ClientID %>');
            var hdnFileUpload = document.getElementById('<%= hdnFileUpload.ClientID %>');

            if (fileUpload.value != '') {
                hdnFileUpload.value = fileUpload.value;
                return true;
            }
            else {
                alert('Please select a file to upload.');
                return false;
            }
        }
        function hideMessage() {
            document.getElementById('<%= lblMessage.ClientID %>').innerText = "";
        }
        function updateSelectAllCheckbox() {
            var allChecked = true;
            var gridView = document.getElementById('<%= gvSuppliers.ClientID %>');
            var checkboxes = gridView.getElementsByTagName('input');

            for (var i = 0; i < checkboxes.length; i++) {
                if (checkboxes[i].type == 'checkbox' && checkboxes[i].id.indexOf('chkSelectVendor') > -1) {
                    if (!checkboxes[i].checked) {
                        allChecked = false;
                        break;
                    }
                }
            }

            var selectAllCheckbox = document.getElementById('<%= chkSelectAll.ClientID %>');
            if (selectAllCheckbox) {
                selectAllCheckbox.checked = allChecked;
            }
        }

        function attachCheckboxEvents() {
            var gridView = document.getElementById('<%= gvSuppliers.ClientID %>');
            var checkboxes = gridView.getElementsByTagName('input');

            for (var i = 0; i < checkboxes.length; i++) {
                if (checkboxes[i].type == 'checkbox' && checkboxes[i].id.indexOf('chkSelectVendor') > -1) {
                    checkboxes[i].onclick = updateSelectAllCheckbox;
                }
            }
        }

        // Attach events on page load
        if (window.addEventListener) {
            window.addEventListener('load', attachCheckboxEvents, false);
        } else if (window.attachEvent) {
            window.attachEvent('onload', attachCheckboxEvents);
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
                    <h2>FLOAT ENQUIRY</h2>
                </div>
                <div class="body">
                    <div class="row clearfix">
                        <div class="col-md-3">Division</div>
                        <div class="col-md-3">From Date</div>
                        <div class="col-md-3">To Date</div>
                        <div class="col-md-3">PP#</div>
                    </div>
                    <div class="row clearfix">
                        <div class="col-md-3">
                            <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                        </div>
                        <div class="col-md-3">
                            <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" OnTextChanged="txtDate_TextChanged" AutoPostBack="true" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" OnTextChanged="txtToDate_TextChanged" AutoPostBack="true" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <asp:TextBox ID="txtPPNo" runat="server" CssClass="form-control" OnTextChanged="txtPPNo_TextChanged" AutoPostBack="true" placeholder="PP#"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row clearfix">
                        <div class="col-sm-6">
                            <div class="form-group">
                                <div class="form-line">
                                    <label>Purchase Group:</label>
                                    <asp:DropDownList ID="ddlPurchaseGroup" runat="server" CssClass="form-control"
                                        OnSelectedIndexChanged="ddlPurchaseGroup_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-6">
                            <div class="form-group">
                                <div class="form-line">
                                    <label>Purchase Sub Group:</label>
                                    <asp:DropDownList ID="ddlPurchaseSubGroup" runat="server" CssClass="form-control"
                                        OnSelectedIndexChanged="ddlPurchaseSubGroup_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row clearfix" id="divGrid" runat="server">
                        <div>
                            <h5>Enquiry List</h5>
                        </div>
                    </div>
                    <div class="row">
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
                    </div>
                    <div class="row">
                        <asp:CheckBox ID="CheckBox1" runat="server" Text="Select All"
                            AutoPostBack="true" OnCheckedChanged="chkSelectAllEnquiries_CheckedChanged" />
                        <div class="table-responsive">
                            <asp:GridView ID="gvEnquiry" runat="server" Width="95%"
                                CssClass="table table-bordered table-striped table-hover"
                                AutoGenerateColumns="false"
                                AllowPaging="true"
                                AllowCustomPaging="true" 
                                PageSize="10" OnPageIndexChanging="gvEnquiry_PageIndexChanging">
                                <Columns>
                                    <asp:TemplateField HeaderText="Sl#" HeaderStyle-Width="1%">
                                        <ItemTemplate>
                                            <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                        </ItemTemplate>
                                        <ItemStyle Width="1%" HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Select to Float" HeaderStyle-Width="3%">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkFloat" runat="server" AutoPostBack="false"
                                                Text='<%# "PR-ID:" + Eval("PurchaseRequisitionID").ToString() %>' />
                                            <asp:HiddenField ID="hdnItemCode" runat="server" Value='<%# Eval("ItemCode") %>' />
                                            <asp:HiddenField ID="hdnItemID" runat="server" Value='<%# Eval("ItemID") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="3%" HorizontalAlign="Center"></ItemStyle>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="PR#">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="hypPRFinal" runat="server"
                                                Text='<%# Eval("PR") %>'
                                                NavigateUrl='<%# Eval("PRID", "GeneratePurchaseRequisitionReport.aspx?ID={0}") %>' Target="_blank" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="PRDate" HeaderText="PR Dt."
                                        ItemStyle-HorizontalAlign="Center" DataFormatString="{0:dd-MMM-yyyy}" ItemStyle-Width="10%" />
                                    <asp:BoundField DataField="PurchaseGroupName" HeaderText="Group"
                                        ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="PurchaseSubGroupName" HeaderText="Category"
                                        ItemStyle-HorizontalAlign="left" ItemStyle-Width="15%" />
                                    <asp:TemplateField HeaderText="Item" HeaderStyle-Width="20%">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="hypItem" runat="server" Text='<%# Eval("ItemDescription") %>' NavigateUrl='<%# String.Format("~/ItemRateHistory.aspx?ID={0}", Eval("ItemID")) %>' Target="_blank"></asp:HyperLink>
                                        </ItemTemplate>
                                        <ItemStyle Width="12%" HorizontalAlign="left"></ItemStyle>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="stock" HeaderText="Stock"
                                        ItemStyle-HorizontalAlign="Center" />
                                    <asp:TemplateField HeaderText="Enquiry Qty" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtEnquiryQty" runat="server" Width="50px" MaxLength="4"
                                                Text='<%# Eval("PRQty") %>' />
                                            <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender27" runat="server"
                                                FilterType="Numbers, Custom" ValidChars="." TargetControlID="txtEnquiryQty" />
                                        </ItemTemplate>
                                        <ItemStyle Width="5%" HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Remarks" HeaderText="Remarks"
                                        ItemStyle-HorizontalAlign="Center" />
                                </Columns>
                                <FooterStyle BackColor="White" ForeColor="#333333" />
                                <PagerStyle BackColor="#b34788" ForeColor="White" HorizontalAlign="Center" />
                                <PagerSettings Mode="NumericFirstLast"
                                    FirstPageText="First"
                                    LastPageText="Last" />
                                <RowStyle BackColor="White" ForeColor="#333333" Height="50px" HorizontalAlign="Left" VerticalAlign="Middle" />
                                <SelectedRowStyle BackColor="#339966" Font-Bold="True" ForeColor="White" />
                                <SortedAscendingCellStyle BackColor="#F7F7F7" />
                                <SortedAscendingHeaderStyle BackColor="#487575" />
                                <SortedDescendingCellStyle BackColor="#E5E5E5" />
                                <SortedDescendingHeaderStyle BackColor="#275353" />
                            </asp:GridView>
                        </div>
                    </div>
                    <div class="row clearfix">
                        <div class="col-sm-6">
                            <div class="form-group">
                                <div class="form-line">
                                    <label>Remarks:</label>
                                    <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-5">
                            <div class="form-group">
                                <div class="form-line">
                                    <label>Attach Document:</label>
                                    <br />
                                    <br />
                                    <div style="display: flex; gap: 10px; align-items: center;">
                                        <asp:FileUpload ID="fpAttach" runat="server" CssClass="btn btn-primary" Width="320px" />

                                    </div>
                                    <asp:HiddenField ID="hdnFileUpload" runat="server" Value="" />
                                    <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-1">
                            <div class="form-group">
                                <label>&nbsp;</label><br />
                                <br />
                                <asp:Button ID="btnAdd" runat="server" Text="+" CssClass="btn btn-primary" OnClick="btnUpload_Click" />
                            </div>
                        </div>

                    </div>

                    <div class="row clearfix">
                        <div class="col-lg-12">
                            <asp:GridView ID="gvAttach" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-striped table-hover"
                                OnRowCommand="gvAttach_RowCommand">
                                <Columns>
                                    <asp:TemplateField HeaderText="Sl#">
                                        <ItemTemplate>
                                            <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="FileName" HeaderText="File Name">
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Actions">
                                        <ItemTemplate>
                                            <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="del"
                                                CommandArgument='<%# Container.DataItemIndex %>' CssClass="btn btn-danger btn-sm" />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>

                        </div>
                    </div>


                    <div class="row clearfix">
                        <div class="col-lg-12">
                            <h5>Supplier List</h5>

                            <asp:CheckBox ID="chkSelectAll" runat="server" Text="Select All"
                                AutoPostBack="true" OnCheckedChanged="chkSelectAll_CheckedChanged" />

                            <div class="table-responsive">
                                <asp:GridView ID="gvSuppliers" runat="server" AutoGenerateColumns="False"
                                    CssClass="table table-bordered table-striped table-hover">
                                    <Columns>

                                        <asp:TemplateField HeaderText="Sl#" HeaderStyle-Width="5%">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Select" HeaderStyle-Width="60px" ItemStyle-Width="60px">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkSelectVendor" runat="server" Text=" " />
                                                <asp:HiddenField ID="hdnVendorID" runat="server" Value='<%# Eval("VendorID") %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Vendor ID" HeaderStyle-Width="10%">
                                            <ItemTemplate>
                                                <asp:HyperLink ID="hypVendorID" runat="server"
                                                    Text='<%# "ID:" + Eval("VendorID").ToString() %>'
                                                    NavigateUrl='<%# GetVendorProfileUrl(Eval("VendorID")) %>'
                                                    Style="text-decoration: underline; color: blue; cursor: pointer;"
                                                    Target="_blank">
                                                </asp:HyperLink>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="VendorName" HeaderText="Vendor Name" HeaderStyle-Width="20%">
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="ContactEmail" HeaderText="Email" HeaderStyle-Width="25%">
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="contactmobile" HeaderText="Contact" HeaderStyle-Width="15%">
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="TypeOfBusiness" HeaderText="Business Type" HeaderStyle-Width="20%">
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>

                    <div class="row clearfix">
                        <div class="col-sm-12">
                            <div class="form-group">
                                <asp:Button ID="btnSave" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
</asp:Content>






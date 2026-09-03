<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="GenrateCode.aspx.cs" Inherits="LIMS.GenrateCode" %>



<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
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
    </style>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <h2>GENERATE CODE</h2>
                </div>
                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>


                            <div class="row">
                                <div class="col-md-3 mb-3" id="div1" runat="server">
                                    <label for="lblType">Type</label>
                                    <span id="Span2" style="color: Red" runat="server">*</span>
                                    <asp:DropDownList ID="ddlType" runat="server" Height="34px" CssClass="form-control" OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                                        <asp:ListItem>Select Type</asp:ListItem>
                                        <asp:ListItem>Vendor</asp:ListItem>
                                        <asp:ListItem>Customer</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-3 mb-3">
                                    <label for="lblCode">Code</label>
                                    <span id="spUn" style="color: Red" runat="server">*</span>
                                    <asp:TextBox ID="txtCode" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="col-md-3 mb-3">
                                    <label for="lblReference">Reference</label>
                                    <span id="Span1" style="color: Red" runat="server">*</span>
                                    <asp:TextBox ID="txtReference" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                </div>
                                 <div class="col-md-3 mb-3" id="div2" runat="server">
                                    <label for="lblEmail">Email</label>
                                    <span id="Span19" style="color: Red" runat="server">*</span>
                                    <asp:TextBox ID="txtEmail" class="form-control" runat="server"></asp:TextBox>
                                    <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" FilterType="LowercaseLetters, UppercaseLetters, Custom" TargetControlID="txtEmail" ValidChars="@1234567890.-_" />
                                </div>
                            </div>


                            <div class="form-row">
                                <asp:Button ID="btnGenerateReference" runat="server" Text="Generate" CssClass="btn btn-primary" OnClick="btnGenerateReference_Click" />
                                <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" Enabled="false" />
                            </div>
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Verification Records</h4>
                                    <asp:GridView ID="gvVerification" runat="server" AutoGenerateColumns="false"
                                        CssClass="table table-bordered table-striped table-hover dataTable js-exportable"
                                        AllowPaging="true" PageSize="5" OnPageIndexChanging="gvVerification_PageIndexChanging">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Sr. No.">
                                                <ItemTemplate>
                                                    <%# Container.DataItemIndex + 1 %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Type" HeaderText="Type" />
                                            <asp:BoundField DataField="created" HeaderText="Created Date" DataFormatString="{0:dd/MM/yyyy}" />
                                            <asp:BoundField DataField="Code" HeaderText="Code" />
                                            <asp:TemplateField HeaderText="Reference">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkReference" runat="server"
                                                        Text='<%# Eval("Reference").ToString() %>'
                                                        CommandName="Select"
                                                        CommandArgument='<%# DataBinder.Eval(Container,"RowIndex") %>'
                                                        OnClientClick='<%# "copyToClipboard(\"" + Eval("Reference").ToString() + "\"); return false;" %>'>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                        <EmptyDataTemplate>
                                            <div class="text-center">No records found</div>
                                        </EmptyDataTemplate>
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


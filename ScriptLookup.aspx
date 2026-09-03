<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ScriptLookup.aspx.cs" Inherits="LIMS.ScriptLookup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <fieldset>

                <div class="col-xs-12 ol-sm-12 col-md-12 col-lg-12">
                    <div class="panel-group" id="accordion_2" role="tablist" aria-multiselectable="true">
                        <div class="panel panel-default">
                            <div class="panel-heading" role="tab" id="headingOne_2">
                                <h4 class="panel-title">
                                    <a role="button" data-toggle="collapse" data-parent="#accordion_2" href="#collapseOne_2" aria-expanded="true" aria-controls="collapseOne_2">Symbols Lookup
                                                    </a>
                                </h4>
                            </div>
                            <div id="collapseOne_2" class="panel-collapse collapse in" role="tabpanel" aria-labelledby="headingOne_2">
                                <div class="panel-body">
                                    <div class="row clearfix">
                                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                            <div class="card"><hr class="violet"><div class="header">

                                                <div class="body">
                                                    <div class="row clearfix">
                                                        <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                                                            <asp:GridView ID="gvScriptLookUp" runat="server" AutoGenerateColumns="false" DataKeyNames="scriptID,scriptHex"
                                                                class="table table-bordered table-striped table-hover js-basic-example dataTable"
                                                                OnRowCommand="gvScriptLookUp_RowCommand" OnRowDataBound="gvScriptLookUp_RowDataBound">
                                                                <Columns>
                                                                    <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="70">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                                        </ItemTemplate>
                                                                        <ItemStyle Width="30px"></ItemStyle>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Symbol">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblSymbol" runat="server" CssClass="form-control" Text='<%# Eval("scriptHex") %>' Enabled="false"></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:BoundField DataField="scriptName" HeaderText="Description" />
                                                                    <asp:BoundField DataField="scriptHex" HeaderText="HexCode" Visible="false" />
                                                                    <asp:TemplateField HeaderText="Code">
                                                                        <ItemTemplate>
                                                                            <span>
                                                                                <asp:Button ID="btnScript" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' CommandName="getScript" Text='<%# Eval("scriptCode") %>' runat="server" class="btn btn-success waves-effect" Height="30px" />
                                                                            </span>
                                                                        </ItemTemplate>
                                                                        <ItemStyle Width="15%"></ItemStyle>
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                            </asp:GridView>
                                                        </div>
                                                        <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">
                                                            <asp:TextBox ID="txtInput" runat="server" Width="50%" CssClass="form-control" placeholder="Type Text, Click Code button for superscript"></asp:TextBox>
                                                            <button type="button" class="btn btn-default waves-effect m-r-20" data-toggle="modal" data-target="#defaultModal">?</button>
                                                            <asp:TextBox ID="txtType" runat="server" Width="50%" CssClass="form-control" OnTextChanged="txtType_TextChanged" AutoPostBack="true" ></asp:TextBox>
                                                        </div>

                                                        <div class="col-lg-2 col-md-2 col-sm-2 col-xs-2">
                                                            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save" class="btn btn-success waves-effect" Height="30px" />
                                                        </div>
                                                        <div class="col-lg-4 col-md-4 col-sm-6 col-xs-6">
                                                            <asp:GridView ID="gvScriptSample" runat="server" class="table table-bordered table-striped table-hover js-basic-example dataTable"
                                                                DataKeyNames="ID" OnRowDeleting="gvScriptSample_RowDeleting" AutoGenerateColumns="false" OnRowCommand="gvScriptSample_RowCommand">
                                                                <Columns>
                                                                    <asp:BoundField DataField="ScriptSample" HeaderText="ScriptSample" />
                                                                    <asp:TemplateField HeaderText="Action">
                                                                        <ItemTemplate>
                                                                            <asp:LinkButton ID="lnkdelete" runat="server" CommandName="Delete" CommandArgument='<%#Eval("ID")%>'>Delete</asp:LinkButton>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                            </asp:GridView>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

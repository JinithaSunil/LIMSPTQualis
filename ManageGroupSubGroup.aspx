<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ManageGroupSubGroup.aspx.cs" Inherits="LIMS.ManageGroupSubGroup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        $(function () {
            var hashId = 0,
                $accordion = $('#accordion');
            if (window.location.hash) {
                $accordion.children('h3').each(function (i) {
                    var txt = $(this).text().toLowerCase().replace(/\s+/g, '_');
                    this.id = txt;
                    if (txt === window.location.hash.slice(1)) {
                        hashId = i;
                    }
                });
            }

            $accordion.accordion({
                active: hashId,
                animate: false,
                heightStyle: 'content',
                collapsible: true,
                create: function (event, ui) {
                    $accordion.children('h3').each(function (i) {
                        $(this).before('<a class="accordion-link link" data-index="' + i + '" href="#' + this.id + '"></a>');
                    });
                    $accordion.find('.accordion-link').click(function () {
                        $accordion.accordion("option", "active", $(this).data('index'));
                    });
                }
            });
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="body">
        <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel ID="updPanel" runat="server">
            <ContentTemplate>
                <fieldset>
                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <div class="card">
                            <hr class="violet">
                            <div class="header">
                                <div class="row clearfix">
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <h2><b>Departments / Product Groups / Product Sub Groups</b></h2>
                                        </div>
                                    </div>

                                </div>
                                <div class="body">
                                    <div class="panel-group" id="accordion" role="tablist" aria-multiselectable="true">
                                        <div class="panel panel-col-violet">
                                            <div class="panel-heading" role="tab" id="headingOne_1">
                                                <h4 class="panel-title">
                                                    <a role="button" data-toggle="collapse" data-parent="#accordion_1" href="#collapseOne_1" aria-expanded="true" aria-controls="collapseOne_1">Departments #1
                                                    </a>
                                                </h4>
                                            </div>
                                            <div id="collapseOne_1" class="panel-collapse collapse in" role="tabpanel" aria-labelledby="headingOne_1">
                                                <div class="panel-body">
                                                    <div class="body table-responsive">
                                                        <asp:GridView ID="gvDepartments" runat="server" class="table table-striped" AutoGenerateColumns="false">
                                                            <Columns>
                                                                <asp:BoundField DataField="DepartmentID" HeaderText="ID" ItemStyle-Width="10%" />
                                                                <asp:BoundField DataField="DepartmentName" HeaderText="Department" ItemStyle-Width="90%" />
                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="panel panel-primary">
                                                <div class="panel-heading" role="tab" id="headingTwo_1">
                                                    <h4 class="panel-title">
                                                        <a class="collapsed" role="button" data-toggle="collapse" data-parent="#accordion_1" href="#collapseTwo_1" aria-expanded="false"
                                                            aria-controls="collapseTwo_1">Product Groups #2
                                                        </a>
                                                    </h4>
                                                </div>
                                                <div id="collapseTwo_1" class="panel-collapse collapse in" role="tabpanel" aria-labelledby="headingTwo_1">
                                                    <div class="panel-body">
                                                        <div class="row clearfix">
                                                            <div class="col-sm-4" style="padding-left: 35px;">
                                                                <div class="form-group form-float">
                                                                    <div class="form-line">
                                                                        <asp:DropDownList ID="ddlDepartments" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="ddlDepartments_SelectedIndexChanged"></asp:DropDownList>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="col-sm-2">
                                                                <asp:Button ID="btnShowProductGroup" runat="server" class="btn bg-violet m-t-15 waves-effect" Text=" + " OnClick="btnShowProductGroup_Click" />
                                                            </div>
                                                            <div class="col-sm-4" id="divProductGroup" runat="server" visible="false">
                                                                <asp:TextBox ID="txtProductGroup" runat="server" CssClass="form-control" placeholder="Product Group Name"></asp:TextBox>
                                                            </div>
                                                            <div class="col-sm-2">
                                                                <asp:Button ID="btnAddProductGroup" runat="server" Text="ADD" OnClick="btnAddProductGroup_Click" class="btn bg-violet m-t-15 waves-effect" />
                                                            </div>
                                                        </div>
                                                        <div class="row clearfix">
                                                            <div class="body table-responsive">
                                                                <asp:GridView ID="gvProductGroup" runat="server" class="table table-striped" AutoGenerateColumns="false">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="ProductGroupName" HeaderText="Product Group Name" ItemStyle-Width="100%" />
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="panel panel-primary">
                                                    <div class="panel-heading" role="tab" id="headingThree_1">
                                                        <h4 class="panel-title">
                                                            <a role="button" data-toggle="collapse" data-parent="#accordion_1" href="#collapseThree_1" aria-expanded="false"
                                                                aria-controls="collapseThree_1">Product Sub Groups #3
                                                            </a>
                                                        </h4>
                                                    </div>
                                                    <div id="collapseThree_1" class="panel-collapse collapse in" role="tabpanel" aria-labelledby="headingThree_1">
                                                        <div class="panel-body">
                                                            <div class="row clearfix">
                                                                <div class="col-sm-4" style="padding-left: 35px;">
                                                                    <div class="form-group form-float">
                                                                        <div class="form-line">
                                                                            <asp:DropDownList ID="ddlDepartments2" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="ddlDepartments2_SelectedIndexChanged"></asp:DropDownList>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div class="col-sm-2">
                                                                    <asp:DropDownList ID="ddlProductGroup" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="ddlProductGroup_SelectedIndexChanged"></asp:DropDownList>
                                                                </div>
                                                                <div class="col-sm-2">
                                                                    <asp:Button ID="btnShowProductSubGroup" runat="server" class="btn bg-violet m-t-15 waves-effect" Text=" + " OnClick="btnShowProductSubGroup_Click" />
                                                                </div>
                                                            </div>
                                                            <div class="row clearfix">
                                                                <div class="col-sm-4" id="divProductSubGroup" runat="server" visible="false">
                                                                    <asp:TextBox ID="txtProductSubGroup" runat="server" CssClass="form-control" placeholder="Product Group Name"></asp:TextBox>
                                                                </div>
                                                                <div class="col-sm-2">
                                                                    <asp:Button ID="btnAddProductSubGroup" runat="server" Text="ADD" OnClick="btnAddProductSubGroup_Click" class="btn bg-violet m-t-15 waves-effect" />
                                                                </div>
                                                            </div>
                                                            <div class="row clearfix">
                                                                <div class="body table-responsive">
                                                                    <asp:GridView ID="gvProductSubGroup" runat="server" class="table table-striped" AutoGenerateColumns="false">
                                                                        <Columns>
                                                                            <asp:BoundField DataField="SubGroupID" HeaderText="ID" ItemStyle-Width="10%" />
                                                                            <asp:BoundField DataField="SubGroupName" HeaderText="Product Group Name" ItemStyle-Width="90%" />
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
                </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>

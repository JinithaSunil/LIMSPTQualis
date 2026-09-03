<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ManageTestParamMethod.aspx.cs" Inherits="LIMS.ManageTestParamMethod" %>

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
                    <div class="row clearfix">
                        <div class="col-xs-12 ol-sm-12 col-md-12 col-lg-12">
                            <b>Test Methods / Parameters</b>
                            <div class="panel-group" id="accordion" role="tablist" aria-multiselectable="true">
                                <div class="panel panel-col-teal">                                  
                                    <div class="panel panel-primary">
                                        <div class="panel-heading" role="tab" id="headingTwo_1">
                                            <h4 class="panel-title">
                                                <a class="collapsed" role="button" data-toggle="collapse" data-parent="#accordion_1" href="#collapseTwo_1" aria-expanded="false"
                                                    aria-controls="collapseTwo_1">Test Methods #1
                                                    </a>
                                            </h4>
                                        </div>
                                        <div id="collapseTwo_1" class="panel-collapse collapse in" role="tabpanel" aria-labelledby="headingTwo_1">
                                            <div class="panel-body">
                                                <div class="row clearfix">
                                                    <div class="col-sm-2">
                                                        <asp:Button ID="btnShowTestMethod" runat="server" class="btn bg-teal m-t-15 waves-effect" Text=" + " OnClick="btnShowTestMethod_Click" />
                                                    </div>
                                                    <div class="col-sm-8" id="divTestMethod" runat="server" visible="false">
                                                        <asp:TextBox ID="txtTestMethod" runat="server" CssClass="form-control" placeholder="Test Method Name"></asp:TextBox>
                                                        <asp:Button ID="btnAddTestMethod" runat="server" Text="ADD" OnClick="btnAddTestMethod_Click"  class="btn bg-teal m-t-15 waves-effect"/>
                                                    </div>
                                                </div>
                                                <div class="row clearfix">
                                                    <div class="body table-responsive">
                                                        <asp:GridView ID="gvTestMethod" runat="server" class="table table-striped" AutoGenerateColumns="false">
                                                            <Columns>
                                                                <asp:BoundField DataField="TestMethod" HeaderText="Test Method Name" ItemStyle-Width="100%" />
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
                                                        aria-controls="collapseThree_1">Test Parameters #2
                                                    </a>
                                                </h4>
                                            </div>
                                            <div id="collapseThree_1" class="panel-collapse collapse in" role="tabpanel" aria-labelledby="headingThree_1">
                                                <div class="panel-body">
                                                    <div class="row clearfix">
                                                        <div class="col-sm-2">
                                                            <asp:Button ID="btnShowTestParameter" runat="server" class="btn bg-teal m-t-15 waves-effect" Text=" + " OnClick="btnShowTestParameter_Click" />
                                                        </div>
                                                    </div>
                                                    <div class="row clearfix">
                                                    <div class="col-sm-6" id="divTestParameter" runat="server" visible="false">
                                                        <asp:TextBox ID="txtTestParameter" runat="server" CssClass="form-control" placeholder="Test Parameter Name"></asp:TextBox>
                                                        <asp:Button ID="btnAddTestParameter" runat="server" Text="ADD"  OnClick="btnAddTestParameter_Click"  class="btn bg-teal m-t-15 waves-effect"/>
                                                    </div>
                                                    </div>
                                                    <div class="row clearfix">
                                                        <div class="body table-responsive">
                                                            <asp:GridView ID="gvTestParameter" runat="server" class="table table-striped" AutoGenerateColumns="false">
                                                                <Columns>
                                                                    <asp:BoundField DataField="TestParameter" HeaderText="Test Parameter Name" ItemStyle-Width="90%" />
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

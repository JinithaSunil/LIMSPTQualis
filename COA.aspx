<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="COA.aspx.cs" Inherits="LIMS.COA" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        function toggleNode(row) {

            let level = parseInt(row.dataset.level);
            let icon = row.querySelector(".node-icon");
            let expand = icon.innerHTML === "▶";

            icon.innerHTML = expand ? "▼" : "▶";

            let next = row.nextElementSibling;

            while (next) {
                let nextLevel = parseInt(next.dataset.level);

                if (nextLevel <= level) break;

                if (expand) {
                    // expand: show only direct children
                    if (nextLevel === level + 1)
                        next.style.display = "";
                } else {
                    // collapse all children
                    next.style.display = "none";

                    // reset their icons to collapsed
                    let childIcon = next.querySelector(".node-icon");
                    if (childIcon) childIcon.innerHTML = "▶";
                }

                next = next.nextElementSibling;
            }
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <!-- Basic Examples -->
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-12">
                            <h2>CHART OF ACCOUNTS</h2>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                     <table>
                         <tr>
                             <td>
                                 <asp:LinkButton ID="lnkAdd" runat="server"
                                     CssClass="btn btn-primary" OnClick="btnAdd_Click">
                                     Manage COA
                                 </asp:LinkButton>
                             </td>
                         </tr>
                     </table>
 </div>


                </div>
                <div class="body">
                    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <fieldset>

                                <asp:GridView ID="gvCOA" runat="server"
                                    AutoGenerateColumns="False"
                                    CssClass="treegrid"
                                    GridLines="None"
                                    OnRowDataBound="gvCOA_RowDataBound"
                                    Width="100%">

                                    <Columns>
                                        <asp:TemplateField HeaderText="Account">
                                            <ItemTemplate>
                                                <span id="spTree" runat="server"></span>
                                                <%# Eval("AccountCode") %> — <%# Eval("AccountName") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="AccountNameEng" HeaderText="Type" />
                                        <asp:BoundField DataField="Tax" HeaderText="Tax" />
                                        <asp:BoundField DataField="Balance" HeaderText="Balance" DataFormatString="{0:N2}" />

                                    </Columns>

                                </asp:GridView>

                            </fieldset>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

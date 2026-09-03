<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="WorkList_Transport.aspx.cs" Inherits="LIMS.WorkList_Transport" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function showModal() {
            $("#myModal").modal('show');
        }

        function ShowPopup() {
            $("#btnShowPopup").click();
        }
    </script>
    <style>
        @media screen and (min-width: 768px) {
            .modal-dialog {
                width: 300px; /* New width for default modal */
            }

            .modal-sm {
                width: 150px; /* New width for small modal */
            }
        }

        @media screen and (min-width: 992px) {
            .modal-lg {
                width: 40%; /* New width for large modal */
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">

     <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

 <div class="container-fluid">
     <button type="button" style="display: none;" id="btnShowPopup" class="btn btn-primary btn-lg"
         data-toggle="modal" data-target="#basic">
         Launch demo modal
     </button>
     <div class="row clearfix">
         <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
             <div class="card">
                 <hr class="violet">
                 <div class="header">

                     <h3>Work List</h3>
                 </div>
                 <div class="body">
                     <div class="row clearfix">
                         <table>
                             <tr>
                                 <td>
                                     <div class="col-4" style="margin-right: 10px;">
                                         <asp:DropDownList ID="ddlYear" runat="server" class="form-control" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                     </div>
                                 </td>
                                 
                                 <td>
                                     <div class="col-4" style="margin-right: 10px;">
                                         <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlMonth_SelectedIndexChanged" AutoPostBack="true">
                                             <asp:ListItem Value="0">Select Month</asp:ListItem>
                                             <asp:ListItem Value="1">January</asp:ListItem>
                                             <asp:ListItem Value="2">February</asp:ListItem>
                                             <asp:ListItem Value="3">March</asp:ListItem>
                                             <asp:ListItem Value="4">April</asp:ListItem>
                                             <asp:ListItem Value="5">May</asp:ListItem>
                                             <asp:ListItem Value="6">June</asp:ListItem>
                                             <asp:ListItem Value="7">July</asp:ListItem>
                                             <asp:ListItem Value="8">August</asp:ListItem>
                                             <asp:ListItem Value="9">September</asp:ListItem>
                                             <asp:ListItem Value="10">October</asp:ListItem>
                                             <asp:ListItem Value="11">November</asp:ListItem>
                                             <asp:ListItem Value="12">December</asp:ListItem>
                                         </asp:DropDownList>
                                     </div>
                                 </td>
                                
                                 <td>
                                     <div class="col-4" style="margin-right: 10px;">
                                         <asp:DropDownList ID="ddlsampleseries" runat="server" class="form-control" OnSelectedIndexChanged="ddlsampleseries_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                     </div>
                                 </td>
                             </tr>

                         </table>
                     </div>
                     <br /><br />
                     <div class="row clearfix">
                         <div class="form-group" style="overflow-x: auto;">
                             <asp:GridView ID="gvWorkList" runat="server" AutoGenerateColumns="false" DataKeyNames="ID,SampleId,CloneCRFSampleID,InquiryID,ReportID" CssClass="table table-striped table-bordered table-hover table-checkable order-column" OnRowDataBound="gvWorkList_RowDataBound" OnRowCommand="gvWorkList_RowCommand">
                                 <Columns>
                                     <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="70">
                                         <ItemTemplate>
                                             <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                         </ItemTemplate>
                                         <ItemStyle Width="70px"></ItemStyle>
                                     </asp:TemplateField>
                                     <asp:BoundField DataField="SampleIdSeries" HeaderText="SampleId Series" />
                                       <asp:BoundField DataField="ID" HeaderText="SampleId" Visible="false" />
                                     <asp:TemplateField HeaderText="Test Name">
                                         <ItemTemplate>
                                             <asp:Label ID="lblReportName" runat="server" Visible="True"></asp:Label>
                                              <asp:Label ID="lblCodefile" runat="server" Text='<%#Eval("CodeFileName") %>' Visible="false"></asp:Label>
                                         </ItemTemplate>
                                     </asp:TemplateField>
                                     <asp:TemplateField HeaderText="Action">
                                         <ItemTemplate>
                                             <asp:HyperLink ID="hlAllocation" runat="server" Target="_blank">View Allocation</asp:HyperLink><br />
                                         </ItemTemplate>
                                     </asp:TemplateField>
                                     <asp:ButtonField ButtonType="Button" ControlStyle-CssClass="btn btn-primary waves-effect" Text="Chat" CommandName="Chat" Visible="false" />
                                     <asp:TemplateField HeaderText="Status">
                                         <ItemTemplate>
                                             <asp:Label ID="lblStatusName" Text='<%# Eval("StatusName") %>' runat="server"></asp:Label>
                                         </ItemTemplate>
                                     </asp:TemplateField>
                                     <asp:TemplateField HeaderText="Action">
                                         <ItemTemplate>
                                             <asp:Button ID="btnTestResult" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' runat="server" class="btn btn-primary waves-effect" Text="Test Results" CommandName="AddTestResults" Visible="true" />
                                         </ItemTemplate>
                                     </asp:TemplateField>
                                 </Columns>
                                 <FooterStyle BackColor="White" ForeColor="#333333" />
                                 <HeaderStyle BackColor="White" Font-Bold="True" ForeColor="Black" />
                                 <PagerStyle BackColor="#b34788" ForeColor="White" HorizontalAlign="Center" />
                                 <RowStyle BackColor="White" ForeColor="#333333" />
                                 <SelectedRowStyle BackColor="#ffe6e6" Font-Bold="True" ForeColor="Black" />
                                 <SortedAscendingCellStyle BackColor="#F7F7F7" />
                                 <SortedAscendingHeaderStyle BackColor="#487575" />
                                 <SortedDescendingCellStyle BackColor="#E5E5E5" />
                                 <SortedDescendingHeaderStyle BackColor="#275353" />
                             </asp:GridView>
                         </div>
                     </div>
                     <div class="modal fade" id="myModal" role="dialog" data-backdrop="static" data-keyboard="false">
                         <div class="modal-dialog modal-lg">
                             <div class="modal-content">
                                 <div class="modal-header">
                                     <button type="button" class="close" data-dismiss="modal">&times;</button>
                                     <h4 class="modal-title">
                                         <center>Chat</center>
                                     </h4>
                                 </div>
                                 <div class="modal-body" style="margin: 20px">

                                     <div class="row">
                                         <div class="col-4">
                                             <asp:TextBox ID="txtMsg" class="form-control" AutoComplete="off" placeholder="Message" AutoPostBack="true" ReadOnly="true" Height="290px" Width="410px" TextMode="MultiLine" runat="server"></asp:TextBox>
                                             <asp:Timer ID="tmMsg" runat="server" OnTick="tmMsg_Tick"></asp:Timer>
                                         </div>
                                     </div>
                                     <br />
                                     <div class="row">
                                         <div style="align-items: center">
                                             <asp:TextBox ID="txtSend" class="form-control" AutoComplete="off" placeholder="Type a Message" runat="server" TextMode="MultiLine" Width="100%"></asp:TextBox>
                                         </div>
                                     </div>
                                     <div class="row">
                                         <div style="text-align: center">
                                             <asp:Button ID="Button1" runat="server" class="btn bg-teal m-t-15 waves-effect" Text="Send " OnClick="btnSend_Click" />
                                         </div>
                                     </div>
                                     <br />
                                 </div>

                             </div>
                         </div>
                     </div>
                 </div>
             </div>
         </div>
     </div>
 </div>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="TaskCompletedList.aspx.cs" EnableEventValidation="true" Inherits="LIMS.TaskCompletedList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        function printDiv() {
            var printContents = document.getElementById("printableArea").innerHTML;
            var originalContents = document.body.innerHTML;
            document.body.innerHTML = printContents;
            window.print();
            document.body.innerHTML = originalContents;           
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <fieldset>
                <div class="row clearfix">
                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <div class="card"><hr class="violet"><div class="header">
                            
                                <h3>Test Completed List</h3>
                            </div>
                            <div class="body">
                                <div class="row clearfix">
                                    <div class="form-group">
                                        <asp:GridView ID="gvCompletedList" runat="server" AutoGenerateColumns="false" DataKeyNames="ID,InquiryID,CloneCRFSampleID" class="table table-bordered table-striped table-hover js-basic-example dataTable" OnRowCommand="gvCompletedList_RowCommand" OnRowDataBound="gvCompletedList_RowDataBound">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="5%">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>    
                                                <asp:BoundField DataField="SampleIdSeries" HeaderText="SampleId" ItemStyle-Width="10%" />                                           
                                                <asp:TemplateField HeaderText="Action" ItemStyle-Width="10%">
                                                    <ItemTemplate>
                                                        <asp:HyperLink ID="hypCustomerFeedback" runat="server" Target="_blank" Visible="false">- CustomerFeedback</asp:HyperLink>
                                                        <asp:Button ID="btnSendFeedbackForm" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' CommandName="SendFeedbackForm" Text="Send Customer Feedback Form" runat="server" class="btn btn-primary btn-xs" Height="30px" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>                                               
                                                <asp:TemplateField HeaderText="Status">
                                                    <ItemTemplate>   
                                                        <asp:Label ID="lblStatusName" Text='<%# Eval("StatusName") %>' runat="server"></asp:Label> 
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <FooterStyle BackColor="White" ForeColor="#333333" />
                                            <PagerStyle BackColor="#b34788" ForeColor="White" HorizontalAlign="Center" />
                                            <RowStyle BackColor="White" ForeColor="#333333" Height="50px" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <SelectedRowStyle BackColor="#339966" ForeColor="White" />
                                            <SortedAscendingCellStyle BackColor="#F7F7F7" />
                                            <SortedAscendingHeaderStyle BackColor="#487575" />
                                            <SortedDescendingCellStyle BackColor="#E5E5E5" />
                                            <SortedDescendingHeaderStyle BackColor="#275353" />
                                        </asp:GridView>
                                    </div>
                                </div>                              
                            </div>
                        </div>
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
    <style type="text/css">
        .button {
            width: 20px;
            height: 40px;
            border-color:#99ccff;
        }
    </style>


</asp:Content>

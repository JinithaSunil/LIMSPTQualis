<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="SampleList.aspx.cs" EnableEventValidation="true" Inherits="LIMS.SampleList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script>
        function printDiv() {
            var printContents = document.getElementById("printableArea").innerHTML;
            var originalContents = document.body.innerHTML;
            document.body.innerHTML = printContents;
            window.print();
            document.body.innerHTML = originalContents;
        }
        function showMyImage() {
            window.open("http://hemetekLIMS.rnisolutions.in/images/MyQRImage.jpg", "Window Title", "width=500, height=450");
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
                        <div class="card">
                            <hr class="violet">
                            <div class="header">

                                <h3>Sample List</h3>
                            </div>
                            <div class="body">
                                <div class="row clearfix">
                                    <table>
                                        <tr>
                                            <td>
                                                <div class="col-4">
                                                    <asp:DropDownList ID="ddlYear" runat="server" class="form-control" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                </div>
                                            </td>
                                            <td>
                                                <div class="col-4">
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
                                        </tr>
                                        <tr>
                                            <td colspan="2">&nbsp;</td>
                                        </tr>
                                    </table>

                                </div>
                                <div class="row clearfix">
                                    <div class="form-group">
                                        <asp:GridView ID="gvSampleList" runat="server" AutoGenerateColumns="false" DataKeyNames="ID,InquiryID,SampleID,CloneCRFSampleID" class="table table-bordered table-striped table-hover js-basic-example dataTable" OnRowDataBound="gvSampleList_RowDataBound" OnRowCommand="gvSampleList_RowCommand">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="70">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="30px"></ItemStyle>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Request No.">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEnquiry" Text='<%# "R/" + Eval("RequestNo").ToString().ToUpper() %>' runat="server" />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="5%"></ItemStyle>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="SampleIdSeries" HeaderText="SampleId" />
                                                <asp:BoundField DataField="ReceivedDate" HeaderText="Received Date" DataFormatString="{0:dd, MMM yy}" />
                                                <asp:BoundField DataField="DestroyDate" HeaderText="Destroy Date" DataFormatString="{0:dd, MMM yy}" Visible="false" />
                                                <asp:TemplateField HeaderText="Allocation" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAllocatedTo" runat="server"></asp:Label>
                                                        <asp:DropDownList ID="ddlAllocatedTo" runat="server" class="form-control"></asp:DropDownList>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="150px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="View">
                                                    <ItemTemplate>
                                                        <asp:HyperLink ID="hypCustCRF" runat="server" Text="- Customer TAF" Target="_blank" Visible="false"></asp:HyperLink>
                                                        <br />
                                                        <asp:HyperLink ID="hypCRF" runat="server" Target="_blank">- Final TAF</asp:HyperLink><br />
                                                        <asp:HyperLink ID="hypAllocation" runat="server" Target="_blank" Visible="false">- Allocation</asp:HyperLink><br />
                                                        <asp:HyperLink ID="hypResult" runat="server" Target="_blank">- Test Report</asp:HyperLink>
                                                        <asp:HyperLink ID="hypCustomerFeedback" runat="server" Target="_blank">- Feedback</asp:HyperLink>
                                                        <asp:HyperLink ID="hypWO" runat="server" Target="_blank" NavigateUrl='<%# "~/generateWorkOrder.aspx?i=" + Eval("InquiryID") + "&s=" + Eval("ID") %>' Visible="false">- WO</asp:HyperLink>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="120px" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Status">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblStatusName" Text='<%# Eval("StatusName") %>' runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="120px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action">
                                                    <ItemTemplate>
                                                        <asp:Button ID="btnAccept" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' Text="Accept" CommandName="Accept" runat="server" class="btn btn-success btn-xs" Height="30px" Visible="false" />
                                                        <asp:Button ID="btnReject" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' Text="Reject" CommandName="Reject" runat="server" class="btn btn-danger btn-xs" Height="30px" Visible="false" />
                                                        <asp:Button ID="btnSampleManagement" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' CommandName="SampleManagement" Text="Sample Mangmt" runat="server" class="btn btn-primary btn-xs" Height="30px" Visible="false" />
                                                        <asp:Button ID="btnAllocate" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' Visible="false" CommandName="Save" Text="Allocate" runat="server" class="btn btn-primary btn-xs" Height="30px" />
                                                        <asp:Button ID="btnMultiple" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' CommandName="Multiple" Text="Allocate" runat="server" class="btn btn-primary btn-xs" Height="30px" />
                                                        <asp:HyperLink ID="hypReport" runat="server" Target="_blank">Generate Report</asp:HyperLink>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:ButtonField ButtonType="Image" ImageUrl="~/assets/img/QR.png" ControlStyle-Width="35px" CommandName="Select" />
                                                <asp:TemplateField HeaderText="Action">
                                                    <ItemTemplate>
                                                        <div class="form-group">
                                                            <asp:Image ID="imgageQRCode" Width="100px" Height="100px" runat="server" Visible="false" />
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <%--<asp:ButtonField ButtonType="Image" HeaderText="QR Code" ControlStyle-Width="35px" />--%>
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
                                        <div id="dialog" style="display: none">
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
            border-color: #99ccff;
        }
    </style>


</asp:Content>

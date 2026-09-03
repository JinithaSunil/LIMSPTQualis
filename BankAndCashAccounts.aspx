<%@ Page Title="" Language="C#" MasterPageFile="~/HemetekLIMS.Master" AutoEventWireup="true" CodeBehind="BankAndCashAccounts.aspx.cs" Inherits="LIMS.BankAndCashAccounts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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

                                <h3>BANK & CASH Accounts</h3>
                            </div>
                            <div class="body">
                                <div class="row clearfix">
                                    <div class="form-group">
                                        <asp:GridView ID="gvAccounts" runat="server" AutoGenerateColumns="false" DataKeyNames="ID,InquiryID,SampleID,CloneCRFSampleID" class="table table-bordered table-striped table-hover js-basic-example dataTable">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="70">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="30px"></ItemStyle>
                                                </asp:TemplateField>
                                            </Columns>
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
</asp:Content>

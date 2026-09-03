<%@ Control Language="C#" AutoEventWireup="true"
    CodeBehind="JournalEntry.ascx.cs"
    Inherits="LIMS.UserControls.JournalEntry" %>
    <div class="card shadow-sm">

    <div class="card-header bg-primary text-white">

        <h5 class="mb-0">
            Accounting Distribution
        </h5>

    </div>

    <div class="card-body">

        <asp:GridView ID="gvJournal"
            runat="server"
            AutoGenerateColumns="False"
            CssClass="table table-bordered table-hover">

            <Columns>

                <asp:TemplateField HeaderText="#">

                    <ItemTemplate>

                        <%# Container.DataItemIndex + 1 %>

                    </ItemTemplate>

                </asp:TemplateField>

                <asp:TemplateField HeaderText="Dr / Cr">

                    <ItemTemplate>

                        <asp:DropDownList
                            ID="ddlType"
                            runat="server"
                            CssClass="form-select">

                            <asp:ListItem>Debit</asp:ListItem>
                            <asp:ListItem>Credit</asp:ListItem>

                        </asp:DropDownList>

                    </ItemTemplate>

                </asp:TemplateField>

                <asp:TemplateField HeaderText="Account">

                    <ItemTemplate>

                        <asp:DropDownList
                            ID="ddlAccount"
                            runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>

                    </ItemTemplate>

                </asp:TemplateField>

                <asp:TemplateField HeaderText="Cost Centre">

                    <ItemTemplate>

                        <asp:DropDownList
                            ID="ddlCostCentre"
                            runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>

                    </ItemTemplate>

                </asp:TemplateField>

                <asp:TemplateField HeaderText="Project">

                    <ItemTemplate>

                        <asp:DropDownList
                            ID="ddlProject"
                            runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>

                    </ItemTemplate>

                </asp:TemplateField>

                <asp:TemplateField HeaderText="Debit">

                    <ItemTemplate>

                        <asp:TextBox
                            ID="txtDebit"
                            runat="server"
                            CssClass="form-control text-end">
                        </asp:TextBox>

                    </ItemTemplate>

                </asp:TemplateField>

                <asp:TemplateField HeaderText="Credit">

                    <ItemTemplate>

                        <asp:TextBox
                            ID="txtCredit"
                            runat="server"
                            CssClass="form-control text-end">
                        </asp:TextBox>

                    </ItemTemplate>

                </asp:TemplateField>

                <asp:TemplateField HeaderText="Remarks">

                    <ItemTemplate>

                        <asp:TextBox
                            ID="txtRemarks"
                            runat="server"
                            CssClass="form-control">
                        </asp:TextBox>

                    </ItemTemplate>

                </asp:TemplateField>

                <asp:TemplateField>

                    <ItemTemplate>

                        <asp:LinkButton
                            ID="btnDelete"
                            runat="server"
                            CssClass="btn btn-danger btn-sm">

                            <i class="ti ti-trash"></i>

                        </asp:LinkButton>

                    </ItemTemplate>

                </asp:TemplateField>

            </Columns>

        </asp:GridView>

    </div>

    <div class="card-footer">

        <asp:Button
            ID="btnAddLine"
            runat="server"
            CssClass="btn btn-success"
            Text="+ Add Line" />

        <div class="float-end">

            Debit :
            <asp:Label
                ID="lblDebit"
                runat="server" />

            &nbsp;&nbsp;

            Credit :
            <asp:Label
                ID="lblCredit"
                runat="server" />

            &nbsp;&nbsp;

            Difference :

            <asp:Label
                ID="lblDifference"
                runat="server"
                CssClass="text-danger fw-bold" />

        </div>

    </div>

</div>

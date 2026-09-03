<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="SampleManagement.aspx.cs" Inherits="LIMS.SampleManagement1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .kanban-board {
    display: flex;
    gap: 20px;
    overflow-x: auto;
}

.kanban-column {
    background: #f9fbfd;
    border-radius: 15px;
    width: 280px;
    padding: 15px;
}

.column-header {
    font-weight: bold;
    margin-bottom: 10px;
}

.badge {
    background: #ffd6d6;
    padding: 3px 8px;
    border-radius: 12px;
    float: right;
}

.kanban-card {
    background: white;
    padding: 12px;
    margin-bottom: 10px;
    border-radius: 12px;
    box-shadow: 0 2px 6px rgba(0,0,0,0.05);
    cursor: pointer;
}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="kanban-board">

    <asp:Repeater ID="rptColumns" runat="server">
        <ItemTemplate>
            <div class="kanban-column">
                <div class="column-header">
                    <%# Eval("StatusName") %>
                    <span class="badge">
                        <%# Eval("Count") %>
                    </span>
                </div>

                <asp:Repeater ID="rptCards" runat="server" 
                    DataSource='<%# Eval("Cards") %>'>
                    <ItemTemplate>
                        <div class="kanban-card">
                            <b><%# Eval("ApplicationNo") %></b><br />
                            <%# Eval("SampleName") %><br />
                            <%# Eval("CustomerName") %><br />
                            <small>
                                <%# Eval("DaysPending") %> Days Pending
                            </small>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

            </div>
        </ItemTemplate>
    </asp:Repeater>

</div>
</asp:Content>

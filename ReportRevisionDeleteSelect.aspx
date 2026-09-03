<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ReportRevisionDeleteSelect.aspx.cs" Inherits="LIMS.ReportRevisionDeleteSelect" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .header.bg-violet {
            background-color: #6f42c1;
            padding: 15px 20px;
            border-radius: 4px 4px 0 0;
        }

            .header.bg-violet h2 {
                margin: 0;
                color: white !important;
            }

        .card {
            border: 1px solid #ddd;
            border-radius: 4px;
            margin: 20px;
        }

        .body {
            padding: 20px;
        }

        .sample-info {
            font-size: 16px;
            font-weight: bold;
            margin-bottom: 10px;
        }

        .no-files {
            font-size: 12px;
            color: #999;
            font-style: italic;
        }

        .protected-note {
            color: #dc3545;
            font-weight: bold;
            background: #f8d7da;
            padding: 10px 15px;
            border-radius: 4px;
            margin-bottom: 15px;
            border-left: 4px solid #dc3545;
        }

        .message-success {
            color: #28a745;
            font-weight: bold;
            display: block;
            margin-bottom: 10px;
        }

        .message-warning {
            color: #ffc107;
            font-weight: bold;
            display: block;
            margin-bottom: 10px;
        }

        .message-error {
            color: #dc3545;
            font-weight: bold;
            display: block;
            margin-bottom: 10px;
        }

        /* GridView styling */
        .grid-view {
            width: 100%;
            border-collapse: collapse;
            margin-top: 15px;
            
        }

            .grid-view th {
                background-color: #b34788 !important;
                padding: 10px 12px;
                text-align: left;
                font-size: 12px;
                text-transform: uppercase;
                color: white;
                border-bottom: 2px solid #ddd;
            }

            .grid-view td {
                padding: 10px 12px;
                border-bottom: 1px solid #eee;
                vertical-align: middle;
            }

            .grid-view tr:hover td {
                background-color: #f9f9f9;
            }

            .grid-view .checkbox-col {
                width: 50px;
                text-align: center !important;
                vertical-align: middle !important;
            }

        /* CUSTOM CHECKBOX - bypasses master page's checkbox JS entirely */
        .custom-checkbox {
            position: relative;
            width: 22px;
            height: 22px;
            display: inline-block;
            cursor: pointer;
            vertical-align: middle;
        }

            .custom-checkbox::before {
                content: "";
                position: absolute;
                top: 0;
                left: 0;
                width: 20px;
                height: 20px;
                border: 2px solid #666;
                border-radius: 3px;
                background: #fff;
                box-sizing: border-box;
            }

            .custom-checkbox.checked::before {
                background: #dc3545;
                border-color: #dc3545;
            }

            .custom-checkbox.checked::after {
                content: "\2713";
                position: absolute;
                top: -2px;
                left: 4px;
                color: #fff;
                font-weight: bold;
                font-size: 15px;
                line-height: 20px;
            }

            .custom-checkbox.disabled {
                cursor: not-allowed;
                opacity: 0.4;
            }

            /* Real checkbox stays in the DOM for postback, but is invisible
                   and cannot receive clicks itself. This is what keeps the
                   master page's checkbox script from ever intercepting it. */
            .custom-checkbox .real-checkbox {
                position: absolute;
                top: 0;
                left: 0;
                width: 20px;
                height: 20px;
                margin: 0;
                opacity: 0;
                pointer-events: none;
            }

        .grid-view tr.row-locked td {
            color: #aaa;
        }

        .grid-view tr.row-locked .file-link {
            color: #99c2ff;
        }

        .grid-view .final-badge {
            background-color: #dc3545;
            color: white;
            padding: 3px 10px;
            border-radius: 3px;
            font-size: 11px;
            display: inline-block;
        }

        .grid-view .old-badge {
            background-color: #ffc107;
            color: #333;
            padding: 3px 10px;
            border-radius: 3px;
            font-size: 11px;
            display: inline-block;
        }

        .grid-view .file-link {
            color: #007bff;
            text-decoration: underline;
        }

            .grid-view .file-link:hover {
                color: #0056b3;
            }

        .btn {
            padding: 8px 20px;
            border-radius: 4px;
            cursor: pointer;
            text-decoration: none;
            display: inline-block;
            font-weight: bold;
        }

        .btn-danger {
            background-color: #dc3545;
            color: white;
            border: 1px solid #dc3545;
        }

            .btn-danger:hover {
                background-color: #c82333;
            }

        .btn-default {
            background-color: #6c757d;
            color: white;
            border: 1px solid #6c757d;
        }

            .btn-default:hover {
                background-color: #5a6268;
            }

        .btn-group {
            margin-top: 20px;
            padding: 10px 0;
        }

        .footer-text {
            margin-top: 30px;
            text-align: center;
            font-size: 12px;
            color: #999;
            border-top: 1px solid #eee;
            padding-top: 15px;
        }

        .table-responsive {
            overflow-x: auto;
            margin-top: 15px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="card">
                <div class="header bg-violet">
                    <h2>Select Files to Delete</h2>
                </div>
                <div class="body">
                    <asp:Label ID="lblSampleInfo" runat="server" CssClass="sample-info" />
                    <p class="no-files">Each PDF checkbox also removes its matching DOCX and PNG (if present) when deleted.</p>

                    <div class="protected-note">
                        ⚠️ The final revision (Current) is protected and cannot be deleted.
                    </div>

                    <hr />

                    <asp:Label ID="lblMessage" runat="server" />
                    <asp:LinkButton ID="btnSelectAll" runat="server"
                        CssClass="btn btn-primary"
                        Text="Select All"
                        OnClientClick="SelectAllCheckboxes(); return false;" />
                    &nbsp;
                    <asp:LinkButton ID="btnDeselectAll" runat="server"
                        CssClass="btn btn-secondary"
                        Text="Deselect All"
                        OnClientClick="DeselectAllCheckboxes(); return false;" />

                    <div class="table-responsive">
                        <asp:GridView ID="gvFiles" runat="server"
                            AutoGenerateColumns="False"
                            CssClass="grid-view"
                            EmptyDataText="No report files found for this sample."
                            OnRowDataBound="gvFiles_RowDataBound"
                            GridLines="None"
                            Width="100%">
                            <Columns>
                                <asp:TemplateField HeaderText="Select"
                                    ItemStyle-CssClass="checkbox-col"
                                    HeaderStyle-CssClass="checkbox-col"
                                    ItemStyle-HorizontalAlign="Center"
                                    HeaderStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <div class='custom-checkbox <%# (bool)Eval("IsFinal") ? "disabled" : "" %>'
                                            onclick="ToggleCustomCheckbox(this);">
                                            <asp:CheckBox ID="chkSelect" runat="server" CssClass="real-checkbox"
                                                Checked='<%# false %>'
                                                Enabled='<%# !(bool)Eval("IsFinal") %>' />
                                        </div>
                                    </ItemTemplate>
                                    <HeaderStyle Width="50px" />
                                    <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Revision" ItemStyle-CssClass="revision-col" HeaderStyle-CssClass="revision-col">
                                    <ItemTemplate>
                                        <%# Eval("RevisionLabel") %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Status" ItemStyle-CssClass="status-col" HeaderStyle-CssClass="status-col">
                                    <ItemTemplate>
                                        <span class='<%# (bool)Eval("IsFinal") ? "final-badge" : "old-badge" %>'>
                                            <%# (bool)Eval("IsFinal") ? "Current (Protected)" : "Previous" %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Folder" ItemStyle-CssClass="folder-col" HeaderStyle-CssClass="folder-col">
                                    <ItemTemplate>
                                        <%# Eval("Folder") %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="File Name" ItemStyle-CssClass="file-col" HeaderStyle-CssClass="file-col">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="lnkView" runat="server"
                                            NavigateUrl='<%# Eval("RelativeUrl") %>'
                                            Text='<%# Eval("PdfFileName") %>'
                                            Target="_blank"
                                            CssClass="file-link" />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Related Files" ItemStyle-CssClass="file-col" HeaderStyle-CssClass="file-col">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRelatedFiles" runat="server"
                                            Text='<%# GetRelatedFilesDisplay(Eval("RelatedFileNames")) %>'
                                            Font-Size="11px"
                                            ForeColor="#888" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>

                    <div style="margin-top:25px;">
                        <asp:LinkButton ID="btnDeleteSelected" runat="server"
                            CssClass="btn btn-danger"
                            Text="Delete Selected Files"
                            OnClick="btnDeleteSelected_Click"
                            OnClientClick="return confirm('Delete the checked PDFs and their matching DOCX/PNG files? This cannot be undone.');" />
                        &nbsp;
                        <asp:LinkButton ID="btnClose" runat="server"
                            CssClass="btn btn-secondary"
                            Text="Close"
                            OnClientClick="window.close(); return false;" />
                        &nbsp;

                        
                    </div>


                </div>
            </div>

            <script type="text/javascript">
                // Toggles the visible custom box AND the real (invisible) checkbox
                // behind it, which is what actually posts back to the server.
                function ToggleCustomCheckbox(wrapper) {
                    if (wrapper.classList.contains('disabled')) return;

                    var input = wrapper.querySelector('input[type=checkbox]');
                    if (!input || input.disabled) return;

                    input.checked = !input.checked;
                    wrapper.classList.toggle('checked', input.checked);
                }

                function SelectAllCheckboxes() {
                    var wrappers = document.querySelectorAll('.custom-checkbox:not(.disabled)');
                    for (var i = 0; i < wrappers.length; i++) {
                        var input = wrappers[i].querySelector('input[type=checkbox]');
                        if (input && !input.disabled) {
                            input.checked = true;
                            wrappers[i].classList.add('checked');
                        }
                    }
                    return false;
                }

                function DeselectAllCheckboxes() {
                    var wrappers = document.querySelectorAll('.custom-checkbox');
                    for (var i = 0; i < wrappers.length; i++) {
                        var input = wrappers[i].querySelector('input[type=checkbox]');
                        if (input) {
                            input.checked = false;
                        }
                        wrappers[i].classList.remove('checked');
                    }
                    return false;
                }
            </script>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

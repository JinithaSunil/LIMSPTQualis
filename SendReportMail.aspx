<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" ValidateRequest="false" AutoEventWireup="true" CodeBehind="SendReportMail.aspx.cs" Inherits="LIMS.SendReportMail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <script type="text/javascript">
         function changeInnerHtml() {
             var div = document.getElementById('<%=  divMessage.ClientID %>');
             document.getElementById('<%=  hdMessage.ClientID %>').value = div.innerHTML;
         }

        function removeAttachment(el, idx) {
            // remove the row visually
            el.parentElement.remove();

            // remove this index from the hidden "remaining indexes" list
            var hd = document.getElementById('<%= hdRemainingIndexes.ClientID %>');
            var indexes = hd.value.split(',').filter(function (i) {
                return i !== '' && i !== String(idx);
            });
            hd.value = indexes.join(',');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card"><hr class="violet"><div class="header">
                
                    <h3>Send Report - Mail</h3>
                </div>
                <div class="body">
                    <div class="row clearfix">
                        <div class="col-sm-6">
                            <div class="form-group">
                                <div class="form-line">
                                    <label class="form-label">To:</label>
                                    <asp:TextBox ID="txtTo" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row clearfix">
                        <div class="col-sm-6">
                            <div class="form-group">
                                <div class="form-line">
                                    <label class="form-label">CC:</label>
                                    <asp:TextBox ID="txtCC" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row clearfix">
                        <div class="col-sm-6">
                            <div class="form-group">
                                <div class="form-line">
                                    <label class="form-label">BCC:</label>
                                    <asp:TextBox ID="txtBCC" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row clearfix">
                        <div class="col-sm-6">
                            <div class="form-group">
                                <div class="form-line">
                                    <label class="form-label">Subject:</label>
                                    <asp:TextBox ID="txtSubject" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row clearfix">
                        <div class="col-sm-6">
                            <div class="form-group">
                                <div class="form-line">
                            <label class="form-label">Attacment:</label><br />
                            <br />
                            <asp:Literal ID="litAttachments" runat="server"></asp:Literal>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row clearfix">
                        <div class="col-sm-6">
                            <div class="form-group">
                                <div class="form-line">
                                    <asp:Literal ID="ltMessage" runat="server" Visible="false"></asp:Literal>
                                    <div id="divMessage" contenteditable="true" runat="server" visible="false" style="font-size: 14px;line-height: 1.42857143;color: #555;"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <asp:HiddenField ID="hdMessage" runat="server" />
            <asp:HiddenField ID="hdRemainingIndexes" runat="server" />
                    <div class="col-12">

                        <div class='float-left'>
                            <asp:Button ID="btnSend" runat="server" CssClass="btn btn-primary" Text="SEND" OnClick="btnSend_Click" OnClientClick="changeInnerHtml();" />
                            <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-primary" Text="CANCEL" OnClick="btnCancel_Click"/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

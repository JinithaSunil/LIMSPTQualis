<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/PTQualisMaster.Master" CodeBehind="ViewReport.aspx.cs" Inherits="LIMS.ViewReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Expires" content="0" />
    <style>
        @media print {
            #noprint {
                visibility: hidden;
            }
        }

        #pageFooter {
            display: table-footer-group;
            float: right;
            margin-right: 5%;
        }

            #pageFooter:after {
                counter-increment: page;
                content: counter(page) " of " counter(page);
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="row clearfix">

        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <h3>Test Reports</h3>
                </div>
                <div class="body">
                    <div id="divTestResult" runat="server">
                    </div>
                    <div id="pageFooter">Page </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

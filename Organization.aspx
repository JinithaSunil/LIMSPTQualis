<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="Organization.aspx.cs" Inherits="LIMS.Organization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .vertical {
            position: relative;
            width: 2px;
            height: 25px;
            left: 50%;
            background-color: #000000;
        }


        table {
            vertical-align: top;
            text-align: center;
            border-collapse: collapse;
        }

            table td {
                border: 0;
                vertical-align: top;
                text-align: center;
                word-wrap: break-word;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card"><hr class="violet"><div class="header">
                
                    <h3>ORGANIZATION CHART</h3>
                </div>
                <div class="body">
                    <div class="row clearfix">
                        <div class="form-group">
                            <div id="abc" runat="server"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

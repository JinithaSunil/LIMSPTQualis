<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="CustomerFeedback.aspx.cs" Inherits="LIMS.CustomerFeedback" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Customer Feedback Form</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link rel="icon" href="favicon.ico" type="image/x-icon" />
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <link href="css/font-awesome.min.css" rel="stylesheet" />
    <link href="css/material-design-iconic-font.min.css" rel="stylesheet" />
    <link href="css/icon-font.min.css" rel="stylesheet" />
    <link href="css/animate.css" rel="stylesheet" />
    <link href="css/hamburgers.min.css" rel="stylesheet" />
    <link href="plugins/animate-css/animate.min.css" rel="stylesheet" />
    <link href="css/select2.min.css" rel="stylesheet" />
    <link href="css/daterangepicker.css" rel="stylesheet" />
    <link href="css/util.css" rel="stylesheet" />
    <link href="css/main.css" rel="stylesheet" />
    <script type="text/javascript">
        function openModal() {
            $('#myModal').modal('show');
        }
    </script>
    <style type="text/css">
        .rad label {
            display: inline;
            padding-left: 10%;
            padding-top: 10%;
            width: auto;
        }

        #dpmaxz_fbm_gui *, #dpmaxz_fbm_gui *:after, #dpmaxz_fbm_gui *:before, #dpmaxz_fbi_root *, #dpmaxz_fbi_root *:after, #dpmaxz_fbi_root *:before {
            margin: 0;
            padding: 0;
            box-sizing: content-box;
            width: 100%;
        }

        .dpmaxz_hidden {
            visibility: hidden;
        }

        #dpmaxz_fbm_gui {
            position: static;
            -moz-user-select: none;
            -webkit-user-select: none;
            margin: 0;
            padding: 0;
            cursor: default;
        }

        #dpmaxz_fbi_root {
            -webkit-user-select: none;
            position: relative;
            display: inline-block;
            z-index: 2147483646;
            margin: 0;
            padding: 0;
        }

            #dpmaxz_fbi_root .dpmaxz_fbi_mainbkg {
                display: block;
                position: absolute;
                cursor: default;
                width: 100px;
                height: 24px;
                background: transparent url(chrome-extension://pkdnjfgdoolnmiacpdamadcneoblphbj/images/mainbkg.png) no-repeat;
            }

                #dpmaxz_fbi_root .dpmaxz_fbi_mainbkg .dpmaxz_fbi_fbishield {
                    float: left;
                    margin: -16px 0 0 3px;
                    padding: 0;
                    background: transparent url(chrome-extension://pkdnjfgdoolnmiacpdamadcneoblphbj/images/mainimg.png) no-repeat;
                }

                #dpmaxz_fbi_root .dpmaxz_fbi_mainbkg .dpmaxz_fbi_authok {
                    background: transparent url(chrome-extension://pkdnjfgdoolnmiacpdamadcneoblphbj/images/mainimg_ok.png) no-repeat;
                }

                #dpmaxz_fbi_root .dpmaxz_fbi_mainbkg .dpmaxz_fbi_authno {
                    background: transparent url(chrome-extension://pkdnjfgdoolnmiacpdamadcneoblphbj/images/mainimg_no.png) no-repeat;
                }

                #dpmaxz_fbi_root .dpmaxz_fbi_mainbkg .dpmaxz_fbi_xchg {
                    background: transparent url(chrome-extension://pkdnjfgdoolnmiacpdamadcneoblphbj/images/mainxchg.png) no-repeat;
                }

                #dpmaxz_fbi_root .dpmaxz_fbi_mainbkg .dpmaxz_fbi_menuact {
                    background: transparent url(chrome-extension://pkdnjfgdoolnmiacpdamadcneoblphbj/images/mainmact.png) no-repeat;
                }

                #dpmaxz_fbi_root .dpmaxz_fbi_mainbkg .dpmaxz_fbi_add {
                    height: 16px;
                    width: 15px;
                    float: right;
                    background: transparent url(chrome-extension://pkdnjfgdoolnmiacpdamadcneoblphbj/images/mainadd.png) no-repeat;
                    margin: 0 -16px 0 0;
                }

        #dpmaxz_fbm_gui .dpmaxz_fbm_menus {
            position: absolute;
            z-index: 2147483647;
        }

        .dpmaxz_fbm_menusrel {
            position: relative;
        }

        #dpmaxz_fbm_gui .dpmaxz_fbm_menus ul {
            position: absolute;
            left: 0;
            top: 22px;
            list-style: none;
            white-space: nowrap;
            border: 1px solid #a0a0a0;
            background-image: -moz-linear-gradient(top, rgb(240,245,255) 73%, white 100%);
            background-image: -webkit-linear-gradient(top, rgb(240,245,255) 73%, white 100%);
            box-shadow: 4px 4px 4px -3px #605F70;
            font-family: 'Segoe UI',Verdana,Arial;
            font-style: normal;
            font-size: 1em;
            padding: 3px 3px;
            margin: 2px 4px 2px 4px;
        }

            #dpmaxz_fbm_gui .dpmaxz_fbm_menus ul li {
                text-decoration: none;
                text-align: left;
                margin: 0;
                padding: 0;
                list-style: none;
            }

                #dpmaxz_fbm_gui .dpmaxz_fbm_menus ul li:hover {
                    border: 1px solid #aecff7;
                    -webkit-border-radius: 4px;
                    background-color: #edf1f6;
                    margin: -1px;
                }

        #dpmaxz_fbm_gui .dpmaxz_fbm_menus .dpmaxz_fbm_row {
            position: relative;
            color: black;
            height: 22px;
            line-height: 22px;
            vertical-align: middle;
            float: none;
        }

            #dpmaxz_fbm_gui .dpmaxz_fbm_menus .dpmaxz_fbm_row span {
                display: inline-block !important;
            }

        #dpmaxz_fbm_gui .dpmaxz_fbm_menus .dpmaxz_fbm_rowicon {
            display: inline-block;
            width: 26px;
            height: 22px;
            position: relative;
            vertical-align: top;
            border-right: 1px solid #e2e3e3;
            font-size: 12px;
            color: black;
        }

        #dpmaxz_fbm_gui .dpmaxz_fbm_menus .dpmaxz_fbm_rowtext {
            display: inline-block;
            border-left: 1px solid #ffffff;
            padding: 0 4px;
            font: normal 1em 'Segoe UI',Verdana,Arial;
            font-size: 12px;
        }

        .dpmaxz_highlight {
            border-image: url(chrome-extension://pkdnjfgdoolnmiacpdamadcneoblphbj/images/highlighter.png) 5 9 8 6 repeat;
            border-style: solid;
        }

        #dpmaxz_fbi_root .dpmaxz_fbi_mainbkg .dpmaxz_fbi_fbishield {
            width: 32px;
            height: 37px;
        }

        .dpmaxz_cf:after {
            clear: both;
            content: '.';
            display: block;
            font-size: 0;
            height: 0;
            line-height: 0;
            visibility: hidden;
        }

        .dpmaxz_hide {
            display: none;
        }

        #dpmaxz_all div {
            margin: 0;
            padding: 0;
            display: block;
            white-space: normal;
            text-align: left;
        }

        #dpmaxz_all *, #dpmaxz_all *:after, #dpmaxz_all *:before {
            margin: 0;
            padding: 0;
            box-sizing: content-box;
            width: auto;
        }

        #dpmaxz_all {
            position: fixed;
            z-index: 2147483646;
            -webkit-user-select: none;
            cursor: default;
            transition: visibility .2s cubic-bezier(0, 0, 0, 1.21),opacity .2s cubic-bezier(0, 0, 0, 1.21);
        }

        .dpmaxz_all_hidden {
            visibility: hidden;
            opacity: 0;
        }

        .dpmaxz_fbb_shadowframe {
            position: relative;
        }

        .dpmaxz_fbb_caption {
            position: relative;
        }

        .dpmaxz_fbb_mainimg {
            float: left;
        }

        .dpmaxz_float_right {
            float: right;
        }

        #dpmaxz_all * {
            box-sizing: content-box;
        }

        #dpmaxz_all input[type='button'] {
            width: auto;
            height: auto;
            min-width: auto;
        }

        #dpmaxz_all input[type='checkbox'] {
            margin: 0;
            width: auto;
            height: auto;
            opacity: 1;
            float: none;
            position: static;
        }

        #dpmaxz_all {
            right: 380px;
            top: 10px;
        }

            #dpmaxz_all .dpmaxz_fbb_shadowframe {
                width: 370px;
                background: url(chrome-extension://pkdnjfgdoolnmiacpdamadcneoblphbj/images/bar-bkg.png) repeat scroll 0 0;
                background-size: 100% 100%;
                background-color: #f9f9f9;
                border: 1px solid #bebebe;
                border-radius: 3px;
                box-shadow: 2px 2px 2px rgba(0, 0, 0, 0.1);
            }

            #dpmaxz_all #dpmaxz_cap {
                margin-left: 10px;
                vertical-align: top;
            }

            #dpmaxz_all .dpmaxz_fbb_caption {
                height: 34px;
                padding: 12px 0 0 18px;
                font: normal normal bold 15px/26px 'Helvetica Neue',Helvetica,Arial,sans-serif;
                background-color: rgba(184, 184, 184, 0.3);
                color: #5f5f5f;
            }

            #dpmaxz_all .dpmaxz_fbb_mainbody {
                padding: 16px 12px 14px;
            }

            #dpmaxz_all .dpmaxz_fbb_maintext {
                margin-left: 60px;
                font: normal normal normal 12px/18px 'Helvetica Neue',Helvetica,Arial,sans-serif;
                color: #414141;
            }

            #dpmaxz_all .dpmaxz_btn_wrap {
                margin-top: 20px;
            }

            #dpmaxz_all input[type='button'].dpmaxz_fbb_btn {
                line-height: normal;
                overflow: visible;
                cursor: pointer;
                text-align: center;
                vertical-align: middle;
                text-decoration: none;
                transition: all .2s ease 0;
                font: normal 12px/18px 'Helvetica Neue',Helvetica,Arial,sans-serif;
                letter-spacing: 0.03em;
                border: 1px solid #aaa;
                border-radius: 2px;
            }

                #dpmaxz_all input[type='button'].dpmaxz_fbb_btn:hover, #dpmaxz_all input[type='button'].dpmaxz_fbb_btn:focus {
                    color: #212121;
                    border: 1px solid #999;
                    background-color: #fff;
                    border-color: #8b8b8b;
                    box-shadow: 1px 1px 2px 2px rgba(119, 119, 119, 0.23);
                }

                    #dpmaxz_all input[type='button'].dpmaxz_fbb_btn:hover:disabled {
                        box-shadow: none;
                    }

            #dpmaxz_all input[type='button'].dpmaxz_fbb_mbtn {
                position: relative;
                display: inline-block;
                padding: 8px 15px;
                color: #000;
                background-color: #f8f8f8;
            }

                #dpmaxz_all input[type='button'].dpmaxz_fbb_mbtn:hover, #dpmaxz_all input[type='button'].dpmaxz_fbb_mbtn:focus {
                }

                #dpmaxz_all input[type='button'].dpmaxz_fbb_mbtn:active {
                    box-shadow: none;
                }

                #dpmaxz_all input[type='button'].dpmaxz_fbb_mbtn:disabled {
                    color: #bbb;
                    cursor: default;
                    border: 1px solid #e3e3e3;
                }

            #dpmaxz_all input[type='button']#dpmaxs_c.dpmaxz_fbb_close {
                position: absolute;
                right: 0;
                width: 42px;
                margin: 0 12px;
                padding: 2px 10px;
                color: #555;
                background-color: rgba(68,202,255,0);
            }

            #dpmaxz_all .dpmaxz_fbb_chk * {
                font: normal normal normal 12px/18px 'Helvetica Neue',Helvetica,Arial,sans-serif;
                color: #414141;
            }

            #dpmaxz_all .dpmaxz_fbb_chk label {
                display: block;
                margin-top: 10px;
                margin-left: 60px;
            }

            #dpmaxz_all .dpmaxz_fbb_chk span {
                margin-left: 6px;
                line-height: 20px;
                vertical-align: middle;
                color: #414141;
            }

            #dpmaxz_all input[type='checkbox'] {
                -ms-transform: scale(1.2);
                -moz-transform: scale(1.2);
                -webkit-transform: scale(1.2);
                -webkit-appearance: checkbox;
                margin: 0;
            }

            #dpmaxz_all .dpmaxz_fbb_shadow {
                background: url(chrome-extension://pkdnjfgdoolnmiacpdamadcneoblphbj/images/bar-shadow.png) no-repeat scroll 0 0;
                background-color: transparent;
                background-size: 100% 100%;
                height: 33px;
            }
    </style>
</head>
<body>
    <div class="bg-container-contact100" style="background-image: url(assets/img/bg-01.jpg);">
        <div class="contact100-header flex-sb-m">
            <%-- <a href="http://www.rniinternational.com" class="contact100-header-logo">
                <img src="RequestDemo/images/logo.png" alt="LOGO" />
            </a>--%>
        </div>
    </div>
    <div>

        <div>
            <div class="container-contact100">
                <div style="background-color: white; width: 85%">
                    <div class="contact100-form-title" style="background-image: url(assets/img/bg-02.jpg);">
                        <span>Customer Feedback Form</span>
                    </div>
                    <form runat="server" style="width: 80%; margin-left: 10%">
                        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <fieldset>
                                    <div class="row clearfix">
                                        <div id="divCustomer" runat="server" class="body">
                                            <table style="width: 100%;">
                                                <tr>
                                                    <td style="padding-left: 10px; text-align: left; width: 10%; padding-top: 15px;"><b>ENQUIRYID:</b></td>
                                                    <td style="padding-left: 10px; padding-top: 25px; text-align: left; width: 30%">
                                                        <div class="form-group">
                                                            <b>
                                                                <asp:Label ID="lblEnquiryID" class="form-control" AutoComplete="off" placeholder="Customer" runat="server" disabled></asp:Label></b>
                                                        </div>
                                                    </td>
                                                    <td style="text-align: right; width: 30%; padding-top: 15px;"></td>
                                                    <td style="padding-left: 10px; padding-top: 25px; text-align: left; width: 30%">
                                                        <div class="form-group">
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 10px; text-align: right; width: 10%; padding-top: 15px;"><b>CUSTOMER:</b></td>
                                                    <td style="padding-left: 10px; padding-top: 25px; text-align: left; width: 30%">
                                                        <div class="form-group">
                                                            <b>
                                                                <asp:TextBox ID="txtCustomer" class="form-control" AutoComplete="off" placeholder="Customer" runat="server" disabled></asp:TextBox></b>
                                                        </div>
                                                    </td>
                                                    <td style="text-align: right; width: 30%; padding-top: 15px;"><b>DATE:</b></td>
                                                    <td style="padding-left: 10px; padding-top: 25px; text-align: left; width: 30%">
                                                        <div class="form-group">
                                                            <asp:TextBox ID="txtDate" class="form-control" AutoComplete="off" placeholder="Date" runat="server" disabled></asp:TextBox>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                            <br />
                                        </div>
                                        <div id="divFeedback" runat="server">
                                            <table style="width: 100%" border="1">
                                                <tr>
                                                    <td style="width: 10%; text-align: center; padding: 5px;"><b>SR NO.</b></td>
                                                    <td style="width: 30%; text-align: center"><b>CUSTOMER’S OBSERVATION</b></td>
                                                    <td style="width: 30%; text-align: center"><b></b></td>
                                                    <td style="width: 30%; text-align: center"><b>CUSTOMER REMARK</b></td>
                                                </tr>

                                                <tr>
                                                    <td style="padding-left: 15px; text-align: center;">1</td>
                                                    <td style="padding-left: 15px;">AVAILABILITY OF INFORMATION<br />
                                                        REGARDING LABORATORY’S SERVICE
                                                    </td>
                                                    <td style="padding-left: 5%">
                                                        <asp:RadioButtonList CssClass="rad" ID="rdl1" runat="server" Width="100%">
                                                            <asp:ListItem Value="Excellent">EXCELLENT</asp:ListItem>
                                                            <asp:ListItem Value="VeryGood">VERY GOOD</asp:ListItem>
                                                            <asp:ListItem Value="Good">GOOD</asp:ListItem>
                                                            <asp:ListItem Value="Fair">FAIR</asp:ListItem>
                                                            <asp:ListItem Value="NeedsImprovement">NEEDS IMPROVEMENT</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                    <td style="padding: 20px;" align="center">
                                                        <div class="form-group">
                                                            <asp:TextBox ID="txtCustomer1" AutoComplete="off" placeholder="Remark" runat="server" Width="200px" BorderColor="#99ccff"></asp:TextBox>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 15px; text-align: center;">2</td>
                                                    <td style="padding-left: 15px;">RESPONSE TO QUERY OVER
                                            <br />
                                                        TELEPHONE, E-MAIL, FAX.
                                                    </td>
                                                    <td style="padding-left: 5%">
                                                        <asp:RadioButtonList CssClass="rad" ID="rdl2" runat="server" Width="100%">
                                                            <asp:ListItem Value="Excellent">EXCELLENT</asp:ListItem>
                                                            <asp:ListItem Value="VeryGood">VERY GOOD</asp:ListItem>
                                                            <asp:ListItem Value="Good">GOOD</asp:ListItem>
                                                            <asp:ListItem Value="Fair">FAIR</asp:ListItem>
                                                            <asp:ListItem Value="NeedsImprovement">NEEDS IMPROVEMENT</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                    <td style="padding: 20px;" align="center">
                                                        <div class="form-group">
                                                            <asp:TextBox ID="txtCustomer2" AutoComplete="off" placeholder="Remark" runat="server" Width="200px" BorderColor="#99ccff"></asp:TextBox>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 15px; text-align: center;">3</td>
                                                    <td style="padding-left: 15px;">ATTENTION AND SERVICE DURING VISIT 
                                                        TO LAB.
                                                    </td>
                                                    <td style="padding-left: 5%">
                                                        <asp:RadioButtonList CssClass="rad" ID="rdl3" runat="server" Width="100%">
                                                            <asp:ListItem Value="Excellent">EXCELLENT</asp:ListItem>
                                                            <asp:ListItem Value="VeryGood">VERY GOOD</asp:ListItem>
                                                            <asp:ListItem Value="Good">GOOD</asp:ListItem>
                                                            <asp:ListItem Value="Fair">FAIR</asp:ListItem>
                                                            <asp:ListItem Value="NeedsImprovement">NEEDS IMPROVEMENT</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                    <td style="padding: 20px;" align="center">
                                                        <div class="form-group">
                                                            <asp:TextBox ID="txtCustomer3" AutoComplete="off" placeholder="Remark" runat="server" Width="200px" BorderColor="#99ccff"></asp:TextBox>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 15px; text-align: center;">4</td>
                                                    <td style="padding-left: 15px;">ADEQUACY OF TEST EQIPMENTS AND TOOLS
                                                    </td>
                                                    <td style="padding-left: 5%">
                                                        <asp:RadioButtonList CssClass="rad" ID="rdl4" runat="server" Width="100%">
                                                            <asp:ListItem Value="Excellent">EXCELLENT</asp:ListItem>
                                                            <asp:ListItem Value="VeryGood">VERY GOOD</asp:ListItem>
                                                            <asp:ListItem Value="Good">GOOD</asp:ListItem>
                                                            <asp:ListItem Value="Fair">FAIR</asp:ListItem>
                                                            <asp:ListItem Value="NeedsImprovement">NEEDS IMPROVEMENT</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                    <td style="padding: 20px;" align="center">
                                                        <div class="form-group">
                                                            <asp:TextBox ID="txtCustomer4" AutoComplete="off" placeholder="Remark" runat="server" Width="200px" BorderColor="#99ccff"></asp:TextBox>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 15px; text-align: center;">5</td>
                                                    <td style="padding-left: 15px;">ON-TIME COMPLETION OF WORK<br />
                                                        INCLUDING DELIVERY OF TEST<br />
                                                        REPORTS.
                                                    </td>
                                                    <td style="padding-left: 5%">
                                                        <asp:RadioButtonList CssClass="rad" ID="rdl5" runat="server" Width="100%">
                                                            <asp:ListItem Value="Excellent">EXCELLENT</asp:ListItem>
                                                            <asp:ListItem Value="VeryGood">VERY GOOD</asp:ListItem>
                                                            <asp:ListItem Value="Good">GOOD</asp:ListItem>
                                                            <asp:ListItem Value="Fair">FAIR</asp:ListItem>
                                                            <asp:ListItem Value="NeedsImprovement">NEEDS IMPROVEMENT</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                    <td style="padding: 20px;" align="center">
                                                        <div class="form-group">
                                                            <asp:TextBox ID="txtCustomer5" AutoComplete="off" placeholder="Remark" runat="server" Width="200px" BorderColor="#99ccff"></asp:TextBox>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 15px; text-align: center;">6</td>
                                                    <td style="padding-left: 15px;">BRIEFING QUALITY FOR TEST<br />
                                                        PROCEDURES AND DOCUMENTATION 
                                                    </td>
                                                    <td style="padding-left: 5%">
                                                        <asp:RadioButtonList CssClass="rad" ID="rdl6" runat="server" Width="100%">
                                                            <asp:ListItem Value="Excellent">EXCELLENT</asp:ListItem>
                                                            <asp:ListItem Value="VeryGood">VERY GOOD</asp:ListItem>
                                                            <asp:ListItem Value="Good">GOOD</asp:ListItem>
                                                            <asp:ListItem Value="Fair">FAIR</asp:ListItem>
                                                            <asp:ListItem Value="NeedsImprovement">NEEDS IMPROVEMENT</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                    <td style="padding: 20px;" align="center">
                                                        <div class="form-group">
                                                            <asp:TextBox ID="txtCustomer6" AutoComplete="off" placeholder="Remark" runat="server" Width="200px" BorderColor="#99ccff"></asp:TextBox>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 15px; text-align: center;">7</td>
                                                    <td style="padding-left: 15px;">RELIABILITY OF TEST RESULTS
                                                    </td>
                                                    <td style="padding-left: 5%">
                                                        <asp:RadioButtonList CssClass="rad" ID="rdl7" runat="server" Width="100%">
                                                            <asp:ListItem Value="Excellent">EXCELLENT</asp:ListItem>
                                                            <asp:ListItem Value="VeryGood">VERY GOOD</asp:ListItem>
                                                            <asp:ListItem Value="Good">GOOD</asp:ListItem>
                                                            <asp:ListItem Value="Fair">FAIR</asp:ListItem>
                                                            <asp:ListItem Value="NeedsImprovement">NEEDS IMPROVEMENT</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                    <td style="padding: 20px;" align="center">
                                                        <div class="form-group">
                                                            <asp:TextBox ID="txtCustomer7" AutoComplete="off" placeholder="Remark" runat="server" Width="200px" BorderColor="#99ccff"></asp:TextBox>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 15px; text-align: center;">8</td>
                                                    <td style="padding-left: 15px;">VALUE FOR MONEY
                                                    </td>
                                                    <td style="padding-left: 5%">
                                                        <asp:RadioButtonList CssClass="rad" ID="rdl8" runat="server" Width="100%">
                                                            <asp:ListItem Value="Excellent">EXCELLENT</asp:ListItem>
                                                            <asp:ListItem Value="VeryGood">VERY GOOD</asp:ListItem>
                                                            <asp:ListItem Value="Good">GOOD</asp:ListItem>
                                                            <asp:ListItem Value="Fair">FAIR</asp:ListItem>
                                                            <asp:ListItem Value="NeedsImprovement">NEEDS IMPROVEMENT</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                    <td style="padding: 20px;" align="center">
                                                        <div class="form-group">
                                                            <asp:TextBox ID="txtCustomer8" AutoComplete="off" placeholder="Remark" runat="server" Width="200px" BorderColor="#99ccff"></asp:TextBox>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 15px; text-align: center;">9</td>
                                                    <td style="padding-left: 15px;">RECEPTIVENESS TO COMPLAINTS<br />
                                                        & SUGGESTIONS
                                                    </td>
                                                    <td style="padding-left: 5%">
                                                        <asp:RadioButtonList CssClass="rad" ID="rdl9" runat="server" Width="100%">
                                                            <asp:ListItem Value="Excellent">EXCELLENT</asp:ListItem>
                                                            <asp:ListItem Value="VeryGood">VERY GOOD</asp:ListItem>
                                                            <asp:ListItem Value="Good">GOOD</asp:ListItem>
                                                            <asp:ListItem Value="Fair">FAIR</asp:ListItem>
                                                            <asp:ListItem Value="NeedsImprovement">NEEDS IMPROVEMENT</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                    <td style="padding: 20px;" align="center">
                                                        <div class="form-group">
                                                            <asp:TextBox ID="txtCustomer9" AutoComplete="off" placeholder="Remark" runat="server" Width="200px" BorderColor="#99ccff"></asp:TextBox>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="padding-left: 15px; text-align: center;">10</td>
                                                    <td style="padding-left: 15px;">OVER ALL SATISFACTION
                                                    </td>
                                                    <td style="padding-left: 5%">
                                                        <asp:RadioButtonList CssClass="rad" ID="rdl10" runat="server" Width="100%">
                                                            <asp:ListItem Value="Excellent">EXCELLENT</asp:ListItem>
                                                            <asp:ListItem Value="VeryGood">VERY GOOD</asp:ListItem>
                                                            <asp:ListItem Value="Good">GOOD</asp:ListItem>
                                                            <asp:ListItem Value="Fair">FAIR</asp:ListItem>
                                                            <asp:ListItem Value="NeedsImprovement">NEEDS IMPROVEMENT</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                    <td style="padding: 20px;" align="center">
                                                        <div class="form-group">
                                                            <asp:TextBox ID="txtCustomer10" AutoComplete="off" placeholder="Remark" runat="server" Width="200px" BorderColor="#99ccff"></asp:TextBox>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="4" style="padding-top: 5%; padding-bottom: 5%; text-align: center; padding-left: 40%;">
                                                        <asp:Button ID="btnSubmit" runat="server" class="contact100-form-btn" Text="Save" OnClick="btnSubmit_Click" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                                    <div class="row clearfix">
                            <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12">
                                <div id="divSuccess" runat="server" visible="false" class="card">
                                    <br />
                                    <br />
                                    <br />
                                    <h2>FEEDBACK SENT!!</h2>
                                    <div style="margin-bottom: 20%">
                                        Thank you for your Feedback. We appreciate it and shall work towards providing better service in future!
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12">
                                <div id="divFail" runat="server" visible="false" class="card">
                                    <br />
                                    <br />
                                    <br />
                                    <h2>ACCESS DENIED!!</h2>
                                    <div style="margin-bottom: 20%"></div>
                                </div>
                            </div>
                        </div>
                                </fieldset>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                    </form>
                </div>
            </div>
        </div>
    </div>
    <script src="js/jquery-3.2.1.min.js"></script>
    <script src="js/animsition.js"></script>
    <script src="js/popper.js"></script>
    <script src="js/bootstrap.min.js"></script>
    <script src="js/select2.min.js"></script>
    <script src="js/moment.min.js"></script>
    <script src="js/daterangepicker.js"></script>
    <script src="js/countdowntime.js"></script>
    <script src="js/main.js"></script>
    <script src="js/pages/ui/modals.js"></script>
</body>
</html>

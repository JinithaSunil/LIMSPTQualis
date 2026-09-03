<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="LIMS.Logout" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>RNI - LIMS</title>
    <link rel="icon" href="favicon.ico" type="image/x-icon"/>
       <!-- Google Fonts -->
    <link href="https://fonts.googleapis.com/css?family=Roboto:400,700&subset=latin,cyrillic-ext" rel="stylesheet" type="text/css"/>
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet" type="text/css"/>

    <!-- Bootstrap Core Css -->
    <link href="plugins/bootstrap/css/bootstrap.css" rel="stylesheet"/>

    <!-- Waves Effect Css -->
    <link href="plugins/node-waves/waves.css" rel="stylesheet" />

    <!-- Animation Css -->
    <link href="plugins/animate-css/animate.css" rel="stylesheet" />

    <!-- Custom Css -->
    <link href="css/style.css" rel="stylesheet"/>
</head>

<body class="login-page">
    <div class="login-box">
        <div class="logo">
            <a href="javascript:void(0);"><img src="images/Logo.png" alt="RNI"/></a>
        </div>
        <div class="card"><hr class="violet"><div class="header">
            <div class="body">
                    
                 <form id="Form1" runat="server">
                     <div class="msg">You are now Logged Out Click Button below to Return to RNI LIMS</div>
                   
                    <div class="row">
                        <div class="col-xs-8 p-t-5">
                           
                        </div>
                        <div class="col-xs-12">
                            <asp:Button ID="btnSubmit" runat="server" Text="RETURN TO LOGIN" OnClick="btnSubmit_Click" class="btn btn-block bg-pink waves-effect"/>
                        </div>
                    </div>
                    <div class="row m-t-15 m-b--20">
                        <div class="col-xs-6">
                            
                        </div>
                        <div class="col-xs-6 align-right">
                           
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>
</body>

</html>
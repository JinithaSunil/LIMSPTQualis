<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="LIMS.Login" %>

<!DOCTYPE html>

<html xmlns="https://www.w3.org/1999/xhtml" lang="en">
<head>
    <title>PT Qualis ERP</title>
   <meta charset="utf-8" />
<meta
  name="viewport"
  content="width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0" />

<!-- Favicon -->
<link rel="icon" type="image/x-icon" href="favicon.png" />

<%--<!-- Fonts -->
    Commented to optimize
<link rel="preconnect" href="https://fonts.googleapis.com" />
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
<link
  href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&ampdisplay=swap"
  rel="stylesheet" />--%>


<!-- Core CSS -->
<link rel="stylesheet" href="assets/vendor/css/core.css" class="template-customizer-core-css" />
<link rel="stylesheet" href="assets/vendor/css/theme-default.css" class="template-customizer-theme-css" />


<!-- Page CSS -->
<!-- Page -->
<link rel="stylesheet" href="assets/vendor/css/pages/page-auth.css" />
    <style>
        @font-face {
    font-family: 'Inter';
    src: url('/assets/fonts/inter.woff2') format('woff2');
    font-display: swap;
}

body {
    font-family: 'Inter', sans-serif;
}
    </style>
</head>

<body class="login-page">
      <!-- Content -->
   <form id="form" runat="server">
       <main>
 <div class="position-relative">
   <div class="authentication-wrapper authentication-basic container-p-y" style="padding-top:2px !important;">
     <div class="authentication-inner py-6 mx-4">
       <!-- Login -->
       <div class="card p-7">
         <!-- Logo -->
         <div class="app-brand justify-content-center mt-5">
           <a href="default.aspx" class="app-brand-link gap-3">
             <span class="app-brand-logo demo">
               <img src="assets/img/logo.png" alt="RNI ERP" height="80" />
                 <%--<img src="assets/img/logo91.png" alt="RNI ERP" height="80" />--%>
             </span>
             <span class="app-brand-text demo text-heading fw-semibold">RNI ERP</span>
           </a>
         </div>
         <!-- /Logo -->

         <div class="card-body mt-1">
           <p class="mb-5" style="text-align:center; font-size:large">Sign In</p>

           
             <div class="form-floating form-floating-outline mb-5">
                 <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control" placeholder="Username" required autofocus></asp:TextBox>                 
               <label for="email">Username</label>
             </div>
             <div class="mb-5">
               <div class="form-password-toggle">
                 <div class="input-group input-group-merge">
                   <div class="form-floating form-floating-outline">
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Password" TextMode="Password" required autofocus></asp:TextBox>
                     <label for="password">Password</label>
                   </div>
                   <span class="input-group-text cursor-pointer"><i class="ri-eye-off-line ri-20px"></i></span>
                 </div>
               </div>
             </div>
             <div class="mb-5 pb-2 d-flex justify-content-between pt-2 align-items-center">
               <div class="form-check mb-0">
                 <input class="form-check-input" type="checkbox" id="remember-me" />
                 <label class="form-check-label" for="remember-me"> Remember Me </label>
               </div>
               
             </div>
             <div class="mb-5" style="height:50px !important">
                 <asp:Button ID="btnSubmit" runat="server" Text="SIGN IN" OnClick="btnSubmit_Click" class="btn btn-primary d-grid w-100" Height="40px" BackColor="#b34788"/>                
             </div>

         </div>
       </div>
       <!-- /Login -->
       <img
         src="assets/img/illustrations/tree-3.png"
         alt="auth-tree"
         class="authentication-image-object-left d-none d-lg-block" />
       <img
         src="assets/img/illustrations/auth-basic-mask-light.png"
         class="authentication-image d-none d-lg-block"
         height="172"
         alt="triangle-bg"
         data-app-light-img="illustrations/auth-basic-mask-light.png"
         data-app-dark-img="illustrations/auth-basic-mask-dark.png" />
       <img
         src="assets/img/illustrations/tree.png"
         alt="auth-tree"
         class="authentication-image-object-right d-none d-lg-block" />
     </div>
   </div>
 </div>
           </main>
       </form>
 <!-- / Content -->


 <!-- Core JS -->
 <!-- build:js assets/vendor/js/core.js -->
 <script src="assets/vendor/libs/jquery/jquery.js"></script>
 <script src="assets/vendor/libs/popper/popper.js"></script>
 <script src="assets/vendor/js/bootstrap.bundle.min.js" defer></script>

 <!-- endbuild -->

 <!-- Vendors JS -->

 <!-- Page JS -->

 <!-- Place this tag before closing body tag for github widget button. -->
 <%--<script async defer src="https://buttons.github.io/buttons.js"></script>--%>
</body>

</html>
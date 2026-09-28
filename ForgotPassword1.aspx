<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgotPassword1.aspx.cs" Inherits="LIMS.ForgotPassword1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Forgot Password</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            /*background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);*/
            min-height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .container {
            background: white;
            padding: 40px;
            border-radius: 12px;
            box-shadow: 0 20px 60px rgba(0,0,0,0.3);
            width: 420px;
            max-width: 90%;
        }

        .header {
            text-align: center;
            margin-bottom: 30px;
        }

            .header h2 {
                color: #333;
                font-size: 24px;
                margin-bottom: 5px;
            }

            .header p {
                color: #777;
                font-size: 14px;
            }

        .form-group {
            margin-bottom: 20px;
        }

        label {
            display: block;
            margin-bottom: 8px;
            font-weight: 600;
            color: #555;
            font-size: 14px;
        }

        .input-group {
            position: relative;
        }

            .input-group .icon {
                position: absolute;
                left: 12px;
                top: 50%;
                transform: translateY(-50%);
                color: #999;
            }

        input[type="text"], select {
            width: 100%;
            padding: 12px 12px 12px 40px;
            border: 2px solid #e0e0e0;
            border-radius: 8px;
            font-size: 14px;
            transition: border-color 0.3s;
            background: #f8f9fa;
        }

            input[type="text"]:focus, select:focus {
                outline: none;
                border-color: #667eea;
                background: white;
                box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
            }

        select {
            padding: 12px 12px 12px 40px;
            appearance: none;
            cursor: pointer;
        }

        .select-wrapper {
            position: relative;
        }

            .select-wrapper::after {
                content: '▼';
                position: absolute;
                right: 12px;
                top: 50%;
                transform: translateY(-50%);
                color: #999;
                pointer-events: none;
            }

        .btn {
            width: 100%;
            padding: 14px;
            background: linear-gradient(135deg, #b34788 0%, #764ba2 100%);
            color: white;
            border: none;
            border-radius: 8px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: transform 0.2s, box-shadow 0.2s;
        }

            .btn:hover {
                transform: translateY(-2px);
                box-shadow: 0 10px 20px rgba(102, 126, 234, 0.3);
            }

            .btn:active {
                transform: translateY(0);
            }

            .btn:disabled {
                opacity: 0.6;
                cursor: not-allowed;
                transform: none;
            }

        .message {
            margin-top: 20px;
            padding: 12px 16px;
            border-radius: 8px;
            text-align: center;
            font-size: 14px;
            display: none;
        }

            .message.show {
                display: block;
            }

            .message.success {
                background: #d4edda;
                color: #155724;
                border: 1px solid #c3e6cb;
            }

            .message.error {
                background: #f8d7da;
                color: #721c24;
                border: 1px solid #f5c6cb;
            }

            .message.info {
                background: #d1ecf1;
                color: #0c5460;
                border: 1px solid #bee5eb;
            }

        .loader {
            display: none;
            text-align: center;
            margin: 10px 0;
        }

            .loader.show {
                display: block;
            }

            .loader .spinner {
                width: 30px;
                height: 30px;
                border: 3px solid #f3f3f3;
                border-top: 3px solid #667eea;
                border-radius: 50%;
                animation: spin 1s linear infinite;
                margin: 0 auto;
            }

        @keyframes spin {
            0% {
                transform: rotate(0deg);
            }

            100% {
                transform: rotate(360deg);
            }
        }

        .footer-links {
            margin-top: 20px;
            text-align: center;
        }

            .footer-links a {
                color: #667eea;
                text-decoration: none;
                font-size: 14px;
                font-weight: 500;
            }

                .footer-links a:hover {
                    text-decoration: underline;
                }

        .password-strength {
            margin-top: 5px;
            font-size: 12px;
        }

        .required-star {
            color: red;
        }

        /* Success animation */
        .success-icon {
            text-align: center;
            font-size: 48px;
            margin-bottom: 10px;
        }

            .success-icon .checkmark {
                color: #28a745;
                animation: scaleIn 0.5s ease;
            }

        @keyframes scaleIn {
            0% {
                transform: scale(0);
            }

            50% {
                transform: scale(1.2);
            }

            100% {
                transform: scale(1);
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <span class="app-brand-logo demo">
                    <img src="assets/img/logo.png" alt="RNI ERP" height="80" />
                </span>
                <span class="app-brand-text demo text-heading fw-semibold mb-4">RNI ERP</span>
                
                <h2>🔑 Forgot Password?</h2>
                <p>Enter your credentials to receive your password</p>
            </div>

            <div id="mainForm">
                <div class="form-group">
                    <label for="txtUserName">
                        Username <span class="required-star">*</span>
                    </label>
                    <div class="input-group">
                        <span class="icon">👤</span>
                        <asp:TextBox ID="txtUserName" runat="server"
                            placeholder="Enter your username" />
                    </div>
                    <asp:RequiredFieldValidator ID="rfvUsername" runat="server"
                        ControlToValidate="txtUserName"
                        ErrorMessage="Username is required"
                        ForeColor="Red" Display="Dynamic"
                        Font-Size="12px" />
                </div>

                <div class="form-group">
                    <label for="ddlRole">
                        Select Role <span class="required-star">*</span>
                    </label>
                    <div class="select-wrapper">
                        <div class="input-group">
                            <span class="icon">👔</span>
                            <asp:DropDownList ID="ddlRole" runat="server">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <asp:RequiredFieldValidator ID="rfvRole" runat="server"
                        ControlToValidate="ddlRole"
                        ErrorMessage="Please select your role"
                        ForeColor="Red" Display="Dynamic"
                        Font-Size="12px" InitialValue="" />
                </div>

                <asp:Button ID="btnSendPassword" runat="server"
                    Text="Send Password to Email"
                    CssClass="btn"
                    OnClick="btnSendPassword_Click" />

                <div class="loader" id="loaderDiv">
                    <div class="spinner"></div>
                    <p style="margin-top: 10px; color: #666; font-size: 14px;">Sending password...</p>
                </div>
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="message" />

            <div class="footer-links">
                <a href="Login.aspx">← Back to Login</a>
                <%-- &nbsp;|&nbsp;--%>
                <%--<a href="Register.aspx">Create Account</a>--%>
            </div>
        </div>
    </form>

    <script type="text/javascript">
        // Show loader on button click
        function showLoader() {
            document.getElementById('loaderDiv').classList.add('show');
            document.getElementById('btnSendPassword').disabled = true;
        }

        // Client-side validation
        function validateForm() {
            var username = document.getElementById('<%= txtUserName.ClientID %>').value.trim();
            var role = document.getElementById('<%= ddlRole.ClientID %>').value;

            if (username === '') {
                alert('Please enter your username.');
                return false;
            }
            if (role === '') {
                alert('Please select your role.');
                return false;
            }
            return true;
        }
    </script>
</body>
</html>


<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccessDenied.aspx.cs" Inherits="LIMS.AccessDenied" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Access Denied</title>

    <style>
        body {
            font-family: Arial;
            background: #f5f6f8;
        }

        .box {
            width: 500px;
            margin: 120px auto;
            padding: 35px;
            text-align: center;
            background: white;
            border: 1px solid #ddd;
            border-radius: 6px;
        }

        .title {
            font-size: 24px;
            font-weight: bold;
            margin-bottom: 15px;
        }

        .message {
            color: #666;
            margin-bottom: 25px;
        }

        .btn {
            padding: 10px 25px;
        }
    </style>
</head>

<body>

<form id="form1" runat="server">

    <div class="box">

        <div class="title">
            Access Denied
        </div>

        <div class="message">
            You have no access to the documents you are trying to open.
        </div>
    </div>

</form>

</body>
</html>

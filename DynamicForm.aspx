<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DynamicForm.aspx.cs" Inherits="LIMS.DynamicForm" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Dynamic Bootstrap Form</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .form-container {
            padding: 20px;
            background-color: #f8f9fa;
            border-radius: 5px;
            margin-top: 20px;
        }

        .terms-container {
            margin-bottom: 15px;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 4px;
        }

        .terms-main-label {
            display: block;
            font-weight: bold;
            margin-bottom: 10px;
        }

        .form-check {
            margin-left: 20px;
            margin-bottom: 5px;
        }

        .form-check-input {
            margin-right: 8px;
        }

        .form-check-label {
            font-weight: normal;
        }


        /* Radio Button Fixes */
        .radio-list {
            display: flex;
            gap: 15px;
            margin: 5px 0;
        }

        .radio-item {
            margin: 0 !important;
            padding: 0 !important;
            box-shadow: none !important;
        }

            .radio-item input[type="radio"] {
                margin-right: 5px;
                box-shadow: none;
                -webkit-appearance: none;
                -moz-appearance: none;
                appearance: none;
                border: 1px solid #ccc;
                width: 16px;
                height: 16px;
                border-radius: 50%;
            }

                .radio-item input[type="radio"]:checked {
                    background: radial-gradient(circle, #007bff 40%, transparent 50%);
                    border-color: #007bff;
                }

        /* Checkbox Fixes */
        .clean-checkbox {
            box-shadow: none !important;
            margin-right: 5px;
        }

            .clean-checkbox input[type="checkbox"] {
                -webkit-appearance: none;
                -moz-appearance: none;
                appearance: none;
                width: 16px;
                height: 16px;
                border: 1px solid #ccc;
                border-radius: 3px;
                vertical-align: middle;
                position: relative;
                top: -1px;
            }

                .clean-checkbox input[type="checkbox"]:checked {
                    background-color: #007bff;
                    border-color: #007bff;
                    background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 12 12'%3E%3Cpath fill='white' d='M10.28 2.28L4 8.56 1.72 6.28 0 8l4 4 8-8z'/%3E%3C/svg%3E");
                    background-repeat: no-repeat;
                    background-position: center;
                }

        /* Terms and Conditions styling */
        .terms-container {
            margin: 10px 0;
            padding: 10px;
            background: #f8f9fa;
            border-radius: 4px;
        }

        .terms-label {
            font-weight: bold;
            margin-bottom: 8px;
            display: block;
        }

        .checkbox-option {
            margin: 5px 0;
            padding-left: 5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <%--<asp:TextBox ID="TextBox1" runat="server" TextMode="Email"></asp:TextBox>--%>

        <div class="container">
            <div class="row justify-content-center">
                <div class="col-md-10">
                    <div class="form-container">
                        <h2 class="text-center mb-4">Dynamic Form</h2>
                        <asp:PlaceHolder ID="phFormContainer" runat="server"></asp:PlaceHolder>
                        <div class="row mt-4">
                            <div class="col-md-12 text-center">
                                <asp:Button ID="btnSubmit" runat="server" Text="Submit"
                                    CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>

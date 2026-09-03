<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="GRNGenerate.aspx.cs" Inherits="LIMS.GRNGenerate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .preview-container {
            display: inline-block;
            position: relative;
            margin: 5px;
        }

        .preview-image {
            max-width: 100px;
            max-height: 100px;
        }

        .remove-btn {
            position: absolute;
            top: 1px;
            right: 1px;
            width: 20px; /* Set the desired width */
            height: 20px; /* Set the desired height */
            display: flex; /* Use flexbox for content alignment */
            justify-content: center; /* Center content horizontally */
            align-items: center; /* Center content vertically */
        }
    </style>
    <style>
        .gridview-header th {
            background-color: #e7f2ff;
            color: #333;
            font-weight: bold;
            border-color: #ccc;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="page-wrapper">
        <div class="page-content">
            <!--end breadcrumb-->
            <div class="row">
                <div class="col-xl-12 mx-auto">

                    <div class="card">
                        <div class="header bg-violet">
                            <h2 style="color: white !important">GRN GENERATE</h2>
                        </div>
                        <div class="body">

                            <div class="row mt-4 mb-4">

                                <div class="col-md-3">
                                    <label for="input1" class="form-label">PO Number </label>
                                    <asp:TextBox ID="txtPoNumber" runat="server" class="form-control"></asp:TextBox>

                                    <ajaxToolkit:AutoCompleteExtender ID="autoCompleteExtender1" runat="server"
                                        TargetControlID="txtPoNumber"
                                        MinimumPrefixLength="1"
                                        EnableCaching="true"
                                        CompletionInterval="100"
                                        CompletionSetCount="10"
                                        ServiceMethod="GetSuggestions">
                                    </ajaxToolkit:AutoCompleteExtender>

                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtPoNumber"
                                        InitialValue="" ErrorMessage="PO Required" Display="Dynamic" CssClass="text-danger">
                                    </asp:RequiredFieldValidator>
                                </div>

                                <div class="col-md-1">
                                    <button type="button" onserverclick="btnSearch_Click" runat="server" class="btn btn-primary" style="margin-top: 22px;">Search</button>
                                </div>

                                <div class="col-md-6">
                                    <label for="input1" class="form-label">Document Upload</label>
                                    <asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="True" CssClass="form-control" />
                                    <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                    ControlToValidate="FileUpload1"
                                    Display="Dynamic"
                                    ErrorMessage="Please select a file to upload."
                                    ForeColor="Red"
                                    SetFocusOnError="True">
                                </asp:RequiredFieldValidator>--%>
                                </div>

                                <div class="col-md-3">
                                    <asp:Button ID="btnUploadFile" runat="server" Text="Upload" class="btn btn-primary px-5" />
                                </div>



                            </div>

                            <div class="row mt-4 mb-4">
                                <div class="table-responsive">
                                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
                                        HeaderStyle-CssClass="gridview-header" DataKeyNames="IT_ID">
                                        <Columns>
                                            <asp:BoundField DataField="ItemCode" HeaderText="Item Code" SortExpression="ItemCode" />
                                            <%--<asp:BoundField DataField="ItemName" HeaderText="Item Name" SortExpression="ItemName" />--%>
                                            <asp:TemplateField HeaderText="Item Name">
                                                <ItemTemplate>
                                                    <div style="word-wrap: break-word; white-space: normal; width: 200px;">
                                                        <%# Eval("ItemName") %>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="IT_OrderedQty" HeaderText="Order Qty." SortExpression="OrderQty" />
                                            <asp:BoundField DataField="IT_PreviousReceivedQty" HeaderText="Pre.Rec.Qty." />
                                            <asp:BoundField DataField="IT_PreviousShortFall" HeaderText="Pre.Shortfall" />
                                            <asp:BoundField DataField="IT_ReceivedQty" HeaderText="Received Qty." SortExpression="PoReceivedQty" />
                                            <asp:BoundField DataField="IT_AcceptedQty" HeaderText="Accepted Qty." SortExpression="PoReceivedQty" />
                                            <asp:BoundField DataField="IT_RejectedQty" HeaderText="Rejected Qty." SortExpression="PoReceivedQty" />
                                            <asp:BoundField DataField="IT_ShortFall" HeaderText="Shortfall" />
                                            <asp:BoundField DataField="IT_BalanceQty" HeaderText="Balance Qty." SortExpression="PoBalanceQty" />
                                            <asp:BoundField DataField="IT_BasicPrice" HeaderText="Basic Price(₹)" SortExpression="PoBasicPrice" />
                                            <asp:BoundField DataField="Percentage" HeaderText="VAT(%)" SortExpression="Percentage" />
                                            <asp:TemplateField HeaderText="Total Price(₹)">
                                                <ItemTemplate>
                                                    <%# String.Format("{0:0.00}", Convert.ToDecimal(Eval("IT_BasicPrice")) * Convert.ToDecimal(Eval("IT_ReceivedQty")) * (1 + Convert.ToDecimal(Eval("Percentage")) / 100)) %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="UOMName" HeaderText="UOM" />
                                        </Columns>
                                    </asp:GridView>


                                </div>
                            </div>

                            <div class="row mt-4 mb-4">
                                <div id="file-preview"></div>
                            </div>

                            <div class="row g-3 mt-3 mb-3">
                                <div class="col-md-4">
                                    <asp:Button ID="btnAddGRN" runat="server" Text="GRN Generate" CssClass="btn btn-primary" OnClick="btnAddGRN_Click" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!--end row-->
    </div>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.12.313/pdf.min.js"></script>

    <script>
        $(document).ready(function () {
            $('#<%= FileUpload1.ClientID %>').change(function () {
                $('#file-preview').empty();
                if (this.files && this.files.length > 0) {
                    for (let i = 0; i < this.files.length; i++) {
                        const file = this.files[i];
                        const reader = new FileReader();

                        if (file.type === 'application/pdf') {
                            reader.onload = function (e) {
                                const pdf = $('<div class="preview-container">' +
                                    '<p>' + file.name + '</p>' +
                                    '<iframe src="' + e.target.result + '#view=FitH" class="preview-pdf" style="width:100px; height:100px;"></iframe>' +
                                    '<button class="btn btn-sm btn-outline-danger remove-btn">X</button>' +
                                    '</div>');
                                $('#file-preview').append(pdf);
                            };
                            reader.readAsDataURL(file);
                        } else if (file.type.startsWith('image/')) {
                            reader.onload = function (e) {
                                const img = $('<div class="preview-container">' +
                                    '<p>' + file.name + '</p>' +
                                    '<img src="' + e.target.result + '" class="preview-image" />' +
                                    '<button class="btn btn-sm btn-outline-danger remove-btn">X</button>' +
                                    '</div>');
                                $('#file-preview').append(img);
                            };
                            reader.readAsDataURL(file);
                        } else {
                            const otherFile = $('<div class="preview-container">' +
                                '<p>' + file.name + '</p>' +
                                '<img src="UploadImages/DefaultFileImages.png" class="preview-image" />' +
                                '<button class="btn btn-sm btn-outline-danger remove-btn">X</button>' +
                                '</div>');
                            $('#file-preview').append(otherFile);
                        }
                    }
                }
            });

            //$(document).on('click', '.remove-btn', function () {
            //    $(this).parent('.preview-container').remove();
            //   container.remove();
            //});

            $(document).on('click', '.remove-btn', function () {
                var previewContainer = $(this).parent('.preview-container');
                var indexToRemove = previewContainer.data('file-index');
                // Remove the corresponding file from the FileUpload1 control
                var filesInput = $('#<%= FileUpload1.ClientID %>')[0];
            filesInput.files = Array.from(filesInput.files).filter((file, index) => index !== indexToRemove);
            // Remove the preview container from the UI
            previewContainer.remove();
        });


            $('#<%= btnUploadFile.ClientID %>').click(function () {
                var formData = new FormData();
                var files = $('#<%= FileUpload1.ClientID %>')[0].files;
            for (var i = 0; i < files.length; i++) {
                formData.append('files', files[i]);
            }
            $.ajax({
                type: 'POST',
                url: 'GRNGenerate.aspx/UploadFiles',
                data: formData,
                contentType: false,
                processData: false,
                success: function (response) {
                    alert('Files uploaded successfully!');
                },
                error: function () {
                    alert('Error uploading files.');
                }
            });
        });
        });
    </script>
</asp:Content>

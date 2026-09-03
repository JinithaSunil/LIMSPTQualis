<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" ValidateRequest="false" AutoEventWireup="true" CodeBehind="EditCRFDetails.aspx.cs" Inherits="LIMS.EditCRFDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <!-- CKEditor -->

    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card"><hr class="violet"><div class="header">
                
                    <h2>EDIT CRF DETAILS</h2>
                </div>
                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                           <asp:GridView ID="gvcrfsampledetails" runat="server" AutoGenerateColumns="false" 
                               OnRowEditing="gvcrfsampledetails_RowEditing" OnRowUpdating="gvcrfsampledetails_RowUpdating" 
                               OnRowCancelingEdit="gvcrfsampledetails_RowCancelingEdit" 
                               DataKeyNames="CRFSampleID,InquiryID,CustomerRequestID,CustomerID" class="table table-bordered table-striped table-hover js-basic-example dataTable">
                               <Columns>
                                   <asp:TemplateField HeaderText="Sample Name">
                                       <ItemTemplate>
                                           <asp:Label ID="lblSampleName" runat="server" Text='<%#Eval("SampleName") %>'></asp:Label>
                                       </ItemTemplate>
                                       <EditItemTemplate>
                                           <asp:Label ID="lblSampleName" runat="server" Text='<%#Eval("SampleName") %>' Visible="false"></asp:Label>
                                           <asp:TextBox ID="txtSampleName" runat="server" Text='<%#Eval("SampleName") %>'></asp:TextBox>
                                       </EditItemTemplate>
                                   </asp:TemplateField>
                                   <asp:TemplateField HeaderText="Batch#">
                                       <ItemTemplate>
                                           <asp:Label ID="lblBatchNo" runat="server" Text='<%#Eval("BatchNo") %>'></asp:Label>
                                       </ItemTemplate>
                                       <EditItemTemplate>
                                           <asp:Label ID="lblBatchNo" runat="server" Text='<%#Eval("BatchNo") %>' Visible="false"></asp:Label>
                                           <asp:TextBox ID="txtBatchNo" runat="server" Text='<%#Eval("BatchNo") %>'></asp:TextBox>
                                       </EditItemTemplate>
                                   </asp:TemplateField>
                                   <asp:TemplateField HeaderText="Test Name">
                                       <ItemTemplate>
                                           <asp:Label ID="lblTestMethod" runat="server" Text='<%#Eval("TestMethod") %>'></asp:Label>
                                       </ItemTemplate>
                                       <EditItemTemplate>
                                           <asp:Label ID="lblTestMethod" runat="server" Text='<%#Eval("TestMethod") %>' Visible="false"></asp:Label>
                                           <asp:TextBox ID="txtTestMethod" runat="server" Text='<%#Eval("TestMethod") %>'></asp:TextBox>
                                       </EditItemTemplate>
                                   </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Test Parameter">
                                       <ItemTemplate>
                                           <asp:Label ID="lblTestParameter" runat="server" Text='<%#Eval("TestParameter") %>'></asp:Label>
                                       </ItemTemplate>
                                       <EditItemTemplate>
                                           <asp:Label ID="lblTestParameter" runat="server" Text='<%#Eval("TestParameter") %>' Visible="false"></asp:Label>
                                           <asp:TextBox ID="txtTestParameter" runat="server" Text='<%#Eval("TestParameter") %>'></asp:TextBox>
                                       </EditItemTemplate>
                                   </asp:TemplateField>
                                   <asp:CommandField ShowEditButton="true" ButtonType="Image" EditImageUrl="~/assets/img/edit.png" ControlStyle-Width="20px" ItemStyle-Width="5%" UpdateImageUrl="~/assets/img/accept1.png" CancelImageUrl="~/assets/img/cross.png" />
                               </Columns>
                           </asp:GridView>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                   
                    <div class="row clearfix">
                        <div class="form-group form-float" style="padding-left: 30px;">
                            <div class="center-block">
                                
                            </div>
                        </div>
                    </div>
                </div>
                 <div id="divError" runat="server" visible="false" class="alert alert-danger">
                                <strong>Data Not Saved!</strong> <asp:Label ID="lblMsg" runat="server"></asp:Label>
                        </div>
            </div>
        </div>
        <asp:HiddenField ID="hdEnquiryID" runat="server" />
        <asp:HiddenField ID="hdcustomerRequestID" runat="server" />
        <asp:HiddenField ID="hdcustomerID" runat="server" />
    </div>
    <!-- #END# CKEditor -->

    <%--<script src="plugins/ckeditor/ckeditor.js"></script>    
    <script src="js/editors.js"></script>--%>
    <script src="tinymce/jscripts/tiny_mce/tiny_mce.js"></script>
    <script type="text/javascript">

        tinyMCE.init({

            mode: "textareas",
            elements: "txtInquiryDetail",
            encoding: 'xml',
            width: "650"

        });
    </script>

</asp:Content>

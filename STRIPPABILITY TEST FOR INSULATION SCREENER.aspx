<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="STRIPPABILITY TEST FOR INSULATION SCREENER.aspx.cs" Inherits="LIMSStaging.STRIPPABILITY_TEST_FOR_INSULATION_SCREENER" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 20px;
            background-color: #f5f5f5;
        }

        .card {
            background: white;
            border-radius: 3px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            margin-bottom: 30px;
        }

        .violet {
            border: 0;
            height: 4px;
            background-color: #b34788;
            margin: 0;
        }

        .header {
            padding: 15px 20px;
            border-bottom: 1px solid #e0e0e0;
        }

            .header h2 {
                margin: 0;
                color: #333;
                font-size: 18px;
                font-weight: bold;
            }

        .body {
            padding: 20px;
        }

        .table-responsive {
            overflow-x: auto;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            background-color: white;
        }

        table, th, td {
            border: 1px solid #ddd;
        }

        th {
            background-color: #f2f2f2;
            color: #333;
            padding: 10px;
            text-align: center;
            font-weight: bold;
        }

        td {
            padding: 8px;
            text-align: center;
        }

        .info-table td:first-child {
            background-color: #f2f2f2;
            width: 25%;
            font-weight: bold;
        }

        .test-table th {
            background-color: #e0e0e0;
            vertical-align: middle;
        }

        input[type="text"], input[type="number"], input[type="date"] {
            width: 90%;
            padding: 6px;
            border: 1px solid #ddd;
            border-radius: 3px;
        }

        .btn-primary {
            background-color: #b34788;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 3px;
            cursor: pointer;
            font-weight: bold;
        }

            .btn-primary:hover {
                background-color: #933375;
            }

        .form-actions {
            margin-top: 20px;
            text-align: center;
        }

        /* Make header cells and input aligned */
        .tests-section th, .test-row td {
            vertical-align: middle;
        }
    </style>
   <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
  <script>
      function initializeStrippabilityCalculations() {
          function calculateMinMax(color) {
              let values = [];

              // Debug line to check what we're selecting
              console.log(`Calculating for ${color}`);

              // Let's log all the textboxes we find
              $('input[type="text"]').each(function () {
                  console.log('Found input with ID:', $(this).attr('id'));
              });

              // Modified selector to match your exact ID pattern
              $(`[id*='txtMeasured${color}']`).each(function () {
                  let val = parseFloat($(this).val());
                  console.log(`Found value: ${val} for element ${$(this).attr('id')}`);
                  if (!isNaN(val)) {
                      values.push(val);
                  }
              });

              console.log(`Values found for ${color}:`, values);

              if (values.length > 0) {
                  let minValue = Math.min(...values).toFixed(2);
                  let maxValue = Math.max(...values).toFixed(2);

                  console.log(`Min: ${minValue}, Max: ${maxValue}`);

                  // Update labels
                  $(`[id*='lblMin${color}']`).text(minValue);
                  $(`[id*='lblMax${color}']`).text(maxValue);
              }
          }

          // Add change and input event listeners to all measured fields
          $('[id*="txtMeasured"]').on('change input', function () {
              let id = $(this).attr('id');
              console.log('Input changed:', id);

              if (id.includes('Red')) {
                  calculateMinMax('Red');
              } else if (id.includes('Blue')) {
                  calculateMinMax('Blue');
              } else if (id.includes('Yellow')) {
                  calculateMinMax('Yellow');
              }
          });

          // Initial calculation
          calculateMinMax('Red');
          calculateMinMax('Blue');
          calculateMinMax('Yellow');
      }

      // Initialize on document ready
      $(document).ready(function () {
          console.log('Document ready');
          initializeStrippabilityCalculations();
      });

      // Re-initialize after each partial postback
      if (typeof Sys !== 'undefined' && Sys.WebForms) {
          Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
              console.log('Partial postback completed');
              initializeStrippabilityCalculations();
          });
      }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="updt" runat="server">
        <ContentTemplate>
            <div class="row clearfix">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="card">
                        <hr class="violet">
                        <div class="header">
                            <h2>STRIPPABILITY TEST FOR INSULATION SCREEN</h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td><b>Type</b></td>
                                        <td>
                                            <asp:TextBox ID="cableType" runat="server" CssClass="form-control" /></td>
                                        <td><b>Ref.Spec.</b></td>
                                        <td>
                                            <asp:TextBox ID="refSpec" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Size</b></td>
                                        <td>
                                            <asp:TextBox ID="cableSize" runat="server" CssClass="form-control" /></td>
                                        <td><b>Test Method No.</b></td>
                                        <td>
                                            <asp:TextBox ID="testMethodNo" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Voltage</b></td>
                                        <td>
                                            <asp:TextBox ID="voltage" runat="server" CssClass="form-control" /></td>
                                        <td><b>Job Order No.</b></td>
                                        <td>
                                            <asp:TextBox ID="jobOrderNo" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                </table>
                            </div>

                            <div class="table-responsive tests-section" style="margin-top: 20px;">
                                <table class="table table-bordered">
                                    <thead>
                                        <tr>
                                            <th rowspan="2">Date</th>
                                            <th rowspan="2">Item Test</th>
                                            <th rowspan="2">Core Phase</th>
                                            <th rowspan="2">Specimen No.</th>
                                            <th>Width strip insulation screen</th>
                                            <th>Pulling Speed</th>
                                            <th>Measured</th>
                                            <th colspan="2">Strippability</th>
                                        </tr>

                                        <tr>
                                            <th>(mm)</th>
                                            <th>(mm/min)</th>
                                            <th>(N)</th>
                                            <th>Min.</th>
                                            <th>Max.</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <!-- Before Ageing - Red -->
                                        <tr>
                                            <td rowspan="9">26/06/2024</td>
                                            <td rowspan="9">Before Ageing</td>
                                            <td rowspan="3">Red</td>
                                            <td>1</td>
                                            <td>
                                                <asp:TextBox ID="txtWidthRed1" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtPullingRed1" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredRed1" runat="server" CssClass="form-control" /></td>
                                            <td rowspan="3">
                                                <asp:Label ID="lblMinRedBA" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td rowspan="3">
                                                <asp:Label ID="lblMaxRedBA" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>2</td>
                                            <td>
                                                <asp:TextBox ID="txtWidthRed2" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtPullingRed2" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredRed2" runat="server" CssClass="form-control" /></td>

                                        </tr>
                                        <tr>
                                            <td>3</td>
                                            <td>
                                                <asp:TextBox ID="txtWidthRed3" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtPullingRed3" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredRed3" runat="server" CssClass="form-control" /></td>
                                        </tr>

                                        <!-- Before Ageing - Blue -->
                                        <tr>
                                            <td rowspan="3">Blue</td>
                                            <td>1</td>
                                            <td>
                                                <asp:TextBox ID="txtWidthBlue1" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtPullingBlue1" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredBlue1" runat="server" CssClass="form-control" /></td>
                                            <td rowspan="3">
                                                <asp:Label ID="lblMinBlueBA" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td rowspan="3">
                                                <asp:Label ID="lblMaxBlueBA" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>2</td>
                                            <td>
                                                <asp:TextBox ID="txtWidthBlue2" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtPullingBlue2" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredBlue2" runat="server" CssClass="form-control" /></td>
                                        </tr>
                                        <tr>
                                            <td>3</td>
                                            <td>
                                                <asp:TextBox ID="txtWidthBlue3" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtPullingBlue3" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredBlue3" runat="server" CssClass="form-control" /></td>
                                        </tr>

                                        <!-- Before Ageing - Yellow -->
                                        <tr>
                                            <td rowspan="3">Yellow</td>
                                            <td>1</td>
                                            <td>
                                                <asp:TextBox ID="txtWidthYellow1" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtPullingYellow1" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredYellow1" runat="server" CssClass="form-control" /></td>
                                            <td rowspan="3">
                                                <asp:Label ID="lblMinYellowBA" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td rowspan="3">
                                                <asp:Label ID="lblMaxYellowBA" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>2</td>
                                            <td>
                                                <asp:TextBox ID="txtWidthYellow2" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtPullingYellow2" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredYellow2" runat="server" CssClass="form-control" /></td>
                                        </tr>
                                        <tr>
                                            <td>3</td>
                                            <td>
                                                <asp:TextBox ID="txtWidthYellow3" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtPullingYellow3" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredYellow3" runat="server" CssClass="form-control" /></td>
                                        </tr>

                                        <!-- After Ageing - Red -->
                                        <tr>
                                            <td rowspan="9">26/06/2024</td>
                                            <td rowspan="9">After Ageing Complete Cable 100°C/168h</td>
                                            <td rowspan="3">Red</td>
                                            <td>1</td>
                                            <td>
                                                <asp:TextBox ID="txtWidthRedA1" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtPullingRedA1" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredRedA1" runat="server" CssClass="form-control" /></td>
                                            <td rowspan="3">
                                                <asp:Label ID="lblMinRedAA" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td rowspan="3">
                                                <asp:Label ID="lblMaxRedAA" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>2</td>
                                            <td>
                                                <asp:TextBox ID="txtWidthRedA2" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtPullingRedA2" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredRedA2" runat="server" CssClass="form-control" /></td>
                                        </tr>
                                        <tr>
                                            <td>3</td>
                                            <td>
                                                <asp:TextBox ID="txtWidthRedA3" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtPullingRedA3" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredRedA3" runat="server" CssClass="form-control" /></td>
                                        </tr>

                                        <!-- AFter Ageing - Blue -->
                                        <tr>
                                            <td rowspan="3">Blue</td>
                                            <td>1</td>
                                            <td>
                                                <asp:TextBox ID="txtWidthBlueA1" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtPullingBlueA1" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredBlueA1" runat="server" CssClass="form-control" /></td>
                                            <td rowspan="3">
                                                <asp:Label ID="lblMinBlueAA" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td rowspan="3">
                                                <asp:Label ID="lblMaxBlueAA" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>2</td>
                                            <td>
                                                <asp:TextBox ID="txtWidthBlueA2" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtPullingBlueA2" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredBlueA2" runat="server" CssClass="form-control" /></td>
                                        </tr>
                                        <tr>
                                            <td>3</td>
                                            <td>
                                                <asp:TextBox ID="txtWidthBlueA3" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtPullingBlueA3" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredBlueA3" runat="server" CssClass="form-control" /></td>
                                        </tr>

                                        <!-- AFter Ageing - Yellow -->
                                        <tr>
                                            <td rowspan="3">Yellow</td>
                                            <td>1</td>
                                            <td>
                                                <asp:TextBox ID="txtWidthYellowA1" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtPullingYellowA1" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredYellowA1" runat="server" CssClass="form-control" /></td>
                                            <td rowspan="3">
                                                <asp:Label ID="lblMinYellowAA" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td rowspan="3">
                                                <asp:Label ID="lblMaxYellowAA" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>2</td>
                                            <td>
                                                <asp:TextBox ID="txtWidthYellowA2" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtPullingYellowA2" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredYellowA2" runat="server" CssClass="form-control" /></td>
                                        </tr>
                                        <tr>
                                            <td>3</td>
                                            <td>
                                                <asp:TextBox ID="txtWidthYellowA3" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtPullingYellowA3" runat="server" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMeasuredYellowA3" runat="server" CssClass="form-control" /></td>

                                        </tr>
                                    </tbody>
                                </table>

                            </div>

                            <div class="form-actions">
                                <asp:Button ID="btnSaveData" runat="server" CssClass="btn-primary" Text="Save Data" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>
</asp:Content>

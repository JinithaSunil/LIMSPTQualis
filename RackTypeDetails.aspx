<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="RackTypeDetails.aspx.cs" Inherits="LIMS.RackTypeDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        /* Compact Table Styles */
        .compact-table {
            margin-top: 15px;
        }

            .compact-table table {
                margin-bottom: 5px;
                width: 100%;
            }

            .compact-table th,
            .compact-table td {
                padding: 8px 10px;
                white-space: nowrap;
            }

        /* Search Header Styles */
        .header-search {
            float: right;
            width: 300px;
            margin-top: -50px;
        }

            .header-search .input-group {
                margin-bottom: 0;
            }

        /* Compact Pagination Styles */
        .compact-pagination {
            margin-top: 10px;
        }

        .pagination-info {
            line-height: 34px;
            margin-right: 15px;
            color: #555;
            font-size: 13px;
        }

        #pageNumbers .btn {
            min-width: 34px;
            padding: 5px 10px;
            margin: 0 2px;
        }

        /* Form Styles */
        .form-group {
            margin-bottom: 15px;
        }

        /* Button Styles */
        .btn-sm {
            padding: 5px 10px;
            min-width: 80px;
        }

        /* Action column */
        .action-column {
            width: 70px;
            text-align: center;
        }

        /* Responsive adjustments */
        @media (max-width: 768px) {
            .header-search {
                float: none;
                width: 100%;
                margin-top: 10px;
            }

            .form-group {
                margin-bottom: 10px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <div class="header bg-violet">
                    <h2 style="color: white !important">RACK TYPE MANAGEMENT</h2>
                </div>
                <div class="header" style="margin-top:50px;">
                    <%--<h2>RACK TYPE MANAGEMENT</h2>--%>
                    <div class="header-search">
                        <div class="input-group" style="margin-left: 0px; margin-top: 36px;">
                            <span class="input-group-addon">
                                <i class="material-icons">search</i>
                            </span>
                            <div class="form-line">
                                <input type="text" id="searchInput" class="form-control" placeholder="Search by Rack Type...">
                            </div>
                        </div>
                    </div>
                </div>
                <div class="body">
                    <div class="row">
                        <asp:Label ID="lblPageID" runat="server"></asp:Label>
                        <%--<div class="col-md-4">
                            <div class="form-group form-float">
                                <div class="form-line">
                                    <p><b>Company Unit</b></p>
                                    <asp:DropDownList ID="ddlBranchUnit" CssClass="form-control show-tick" runat="server">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>--%>

                        <div class="col-md-4">
                            <div class="form-group form-float">
                                <div class="form-line">
                                    <p><b>Rack Name</b></p>
                                    <asp:TextBox ID="txtRackType" runat="server" class="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtRackType"
                                        ErrorMessage="Rack Name Required." Display="Dynamic" CssClass="text-danger">
                                    </asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>

                        <div class="col-md-4">
                            <asp:Button ID="btnAdd" runat="server" Text="ADD" class="btn btn-primary waves-effect btn-sm" OnClick="btnAdd_Click" />
                            <asp:Button ID="btnUpdate" runat="server" Text="UPDATE" class="btn btn-success waves-effect btn-sm" OnClick="btnUpdate_Click" />
                            <asp:Button ID="btnCancel" runat="server" Text="CANCEL" class="btn btn-default waves-effect btn-sm" OnClick="btnCancel_Click" />
                        </div>
                    </div>

                    <div class="table-responsive compact-table">
                        <table id="rackTable" class="table table-bordered table-striped table-hover table-condensed">
                            <thead>
                                <tr>
                                    <th style="width: 50px;">#</th>
                                    <%--<th>Unit</th>--%>
                                    <th>Rack Type</th>
                                    <th class="action-column">Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater runat="server" ID="RepShowData" OnItemCommand="RepShowData_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Container.ItemIndex + 1 %></td>
                                            <%--<td class="unit-name"><%# Eval("UnitName") %></td>--%>
                                            <td class="rack-type"><%# Eval("RackType") %></td>
                                            <td class="text-center">
                                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="ItemEdit"
                                                    CommandArgument='<%# Eval("RackTypeId") %>'
                                                    ToolTip="Edit" CssClass="btn btn-sm btn-default waves-effect">
                                                    <i class="material-icons">edit</i>
                                                </asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>

                    <div class="pagination-container compact-pagination">
                        <div class="pull-left">
                            <span class="pagination-info">Showing page <span id="currentPage">1</span> of <span id="totalPages">1</span>
                                (<span id="totalRecords">0</span> records)
                            </span>
                        </div>
                        <div class="pull-right">
                            <div class="btn-group">
                                <button id="firstPage" class="btn btn-default btn-sm waves-effect" title="First">
                                    <i class="material-icons">first_page</i>
                                </button>
                                <%--<button id="prevPage" class="btn btn-default btn-sm waves-effect" title="Previous">
                                    <i class="material-icons">chevron_left</i>
                                </button>--%>
                                <div id="pageNumbers" class="btn-group" style="margin-top: 4px;"></div>
                                <%--<button id="nextPage" class="btn btn-default btn-sm waves-effect" title="Next">
                                    <i class="material-icons">chevron_right</i>
                                </button>--%>
                                <button id="lastPage" class="btn btn-default btn-sm waves-effect" title="Last">
                                    <i class="material-icons">last_page</i>
                                </button>
                            </div>
                        </div>
                        <div class="clearfix"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script>
        $(document).ready(function () {
            // Initialize variables
            var currentPage = 1;
            var recordsPerPage = 5; // Show 5 records per page
            var $table = $('#rackTable');
            var $allRows = $table.find('tbody tr'); // Store ALL rows
            var $filteredRows = $allRows; // Store filtered rows
            var totalRecords = $allRows.length;
            var totalPages = Math.ceil(totalRecords / recordsPerPage);

            // Initialize display
            updatePagination();
            showPage(currentPage);
            updateRecordCount();

            // Search functionality
            $('#searchInput').on('keyup', function () {
                var searchText = $(this).val().toLowerCase();

                if (searchText === '') {
                    // When search is cleared, reset to show all rows
                    $filteredRows = $allRows;
                    $allRows.show(); // Show all rows again
                } else {
                    // Filter rows based on search text
                    $filteredRows = $allRows.filter(function () {
                        /*var unitName = $(this).find('.unit-name').text().toLowerCase();*/
                        var rackType = $(this).find('.rack-type').text().toLowerCase();
                        return rackType.indexOf(searchText) > -1;
                    });

                    // Hide all rows first
                    $allRows.hide();
                    // Then show only filtered rows
                    $filteredRows.show();
                }

                // Update pagination with filtered rows
                totalRecords = $filteredRows.length;
                totalPages = Math.ceil(totalRecords / recordsPerPage);
                currentPage = 1; // Reset to first page when searching
                updatePagination();
                showPage(currentPage);
                updateRecordCount();
            });

            // Pagination controls
            $('#firstPage').click(function () {
                if (currentPage > 1) {
                    currentPage = 1;
                    showPage(currentPage);
                    updatePagination();
                }
            });

            $('#prevPage').click(function () {
                if (currentPage > 1) {
                    currentPage--;
                    showPage(currentPage);
                    updatePagination();
                }
            });

            $('#nextPage').click(function () {
                if (currentPage < totalPages) {
                    currentPage++;
                    showPage(currentPage);
                    updatePagination();
                }
            });

            $('#lastPage').click(function () {
                if (currentPage < totalPages) {
                    currentPage = totalPages;
                    showPage(currentPage);
                    updatePagination();
                }
            });

            // Function to show a specific page
            function showPage(page) {
                // Hide all filtered rows first
                $filteredRows.hide();

                var start = (page - 1) * recordsPerPage;
                var end = start + recordsPerPage;

                // Show only the rows for current page
                $filteredRows.slice(start, end).show();

                // Update current page display
                $('#currentPage').text(page);
            }

            // Function to update pagination controls
            function updatePagination() {
                $('#totalPages').text(totalPages);

                // Enable/disable navigation buttons
                $('#firstPage, #prevPage').prop('disabled', currentPage === 1);
                $('#nextPage, #lastPage').prop('disabled', currentPage === totalPages || totalPages === 0);

                // Generate page numbers
                var $pageNumbers = $('#pageNumbers');
                $pageNumbers.empty();

                var maxVisiblePages = 5; // Show up to 5 page numbers
                var startPage, endPage;

                if (totalPages <= maxVisiblePages) {
                    // Show all pages
                    startPage = 1;
                    endPage = totalPages;
                } else {
                    // Calculate start and end pages
                    var maxPagesBeforeCurrent = Math.floor(maxVisiblePages / 2);
                    var maxPagesAfterCurrent = Math.ceil(maxVisiblePages / 2) - 1;

                    if (currentPage <= maxPagesBeforeCurrent) {
                        // Current page near the start
                        startPage = 1;
                        endPage = maxVisiblePages;
                    } else if (currentPage + maxPagesAfterCurrent >= totalPages) {
                        // Current page near the end
                        startPage = totalPages - maxVisiblePages + 1;
                        endPage = totalPages;
                    } else {
                        // Current page somewhere in the middle
                        startPage = currentPage - maxPagesBeforeCurrent;
                        endPage = currentPage + maxPagesAfterCurrent;
                    }
                }

                // Add "First" if needed
                if (startPage > 1) {
                    $pageNumbers.append('<button class="btn btn-default page-number" data-page="1">1</button>');
                    if (startPage > 2) {
                        $pageNumbers.append('<span class="page-ellipsis">...</span>');
                    }
                }

                // Add page numbers
                for (var i = startPage; i <= endPage; i++) {
                    var $btn = $('<button class="btn ' + (i === currentPage ? 'btn-primary' : 'btn-default') +
                        ' page-number" data-page="' + i + '">' + i + '</button>');
                    $pageNumbers.append($btn);
                }

                // Add "Last" if needed
                if (endPage < totalPages) {
                    if (endPage < totalPages - 1) {
                        $pageNumbers.append('<span class="page-ellipsis">...</span>');
                    }
                    $pageNumbers.append('<button class="btn btn-default page-number" data-page="' + totalPages + '">' + totalPages + '</button>');
                }

                // Add click handler for page numbers
                $('.page-number').click(function () {
                    currentPage = parseInt($(this).data('page'));
                    showPage(currentPage);
                    updatePagination();
                });
            }

            // Function to update record count display
            function updateRecordCount() {
                $('#totalRecords').text($filteredRows.length);
            }
        });
    </script>
</asp:Content>

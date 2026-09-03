<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TemporaryPageforTesting.aspx.cs" Inherits="LIMS.TemporaryPageforTesting" %>


<!DOCTYPE html>
<html>
<head runat="server">
    <title>Advanced Form Builder</title>
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.13.0/themes/base/jquery-ui.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />

    <!-- Flatpickr CSS -->
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css">
    <!-- Flatpickr JS -->
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
    <style>
        /* Control Actions (Edit/Delete buttons) */
        .control-actions {
            position: absolute;
            top: 5px;
            right: 5px;
            display: flex;
            gap: 5px;
            opacity: 0;
            transition: opacity 0.3s ease;
        }

        .form-control {
            position: relative;
            border: 1px solid #ddd;
            padding: 15px;
            margin-bottom: 10px;
            border-radius: 4px;
            transition: all 0.3s ease;
        }

            .form-control:hover .control-actions {
                opacity: 1;
            }

            .form-control.edit-mode {
                border: 2px solid #007bff !important;
                background-color: #f8f9fa;
            }

            .form-control.selected {
                border-color: #007bff;
                box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
            }

        .edit-btn, .delete-btn {
            width: 30px;
            height: 30px;
            border: none;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            cursor: pointer;
            font-size: 12px;
            transition: all 0.3s ease;
        }

        .edit-btn {
            background-color: #28a745;
            color: white;
        }

            .edit-btn:hover {
                background-color: #218838;
            }

        .delete-btn {
            background-color: #dc3545;
            color: white;
        }

            .delete-btn:hover {
                background-color: #c82333;
            }

        /* Disabled state styling */
        .form-control input[disabled],
        .form-control textarea[disabled],
        .form-control select[disabled],
        .form-control button[disabled] {
            background-color: #e9ecef;
            opacity: 0.6;
            cursor: not-allowed;
        }

        /* Edit mode styling */
        .form-control.edit-mode input:not([disabled]),
        .form-control.edit-mode textarea:not([disabled]),
        .form-control.edit-mode select:not([disabled]) {
            background-color: white;
            opacity: 1;
            cursor: text;
        }

        /* Property editor styling */
        #propertyEditor {
            border: 1px solid #ddd;
            border-radius: 4px;
            padding: 15px;
            background-color: white;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }

        /* Options list styling */
        #optionsList .option-item {
            display: flex;
            gap: 10px;
            align-items: center;
            margin-bottom: 10px;
            padding: 10px;
            border: 1px solid #e0e0e0;
            border-radius: 4px;
            background-color: #f9f9f9;
        }

            #optionsList .option-item input {
                flex: 1;
                padding: 8px;
                border: 1px solid #ddd;
                border-radius: 4px;
            }

            #optionsList .option-item .btn {
                padding: 8px 12px;
                min-width: auto;
            }

        /* Add option button */
        #btnAddOption {
            background-color: #28a745;
            color: white;
            border: none;
            padding: 10px 15px;
            border-radius: 4px;
            cursor: pointer;
            margin-top: 10px;
        }

            #btnAddOption:hover {
                background-color: #218838;
            }

        /* Form area styling */
        #formArea {
            min-height: 400px;
            border: 2px dashed #ddd;
            border-radius: 4px;
            padding: 20px;
            background-color: #fafafa;
        }

            #formArea:empty::before {
                content: "Drag form elements here";
                color: #999;
                font-style: italic;
                display: block;
                text-align: center;
                margin-top: 100px;
            }

        /* Responsive design */
        @media (max-width: 768px) {
            .control-actions {
                position: static;
                opacity: 1;
                justify-content: flex-end;
                margin-bottom: 10px;
            }

            .form-control {
                padding: 10px;
            }
        }

        .builder-preview input {
            pointer-events: none;
        }

        .date-picker-wrapper {
            display: flex;
            align-items: center;
            gap: 5px;
        }

        .date-input {
            flex: 1;
            background-color: #fff;
            padding: 6px 8px;
            border: 1px solid #ccc;
            border-radius: 4px;
        }

        .calendar-btn {
            background: transparent;
            border: none;
            font-size: 16px;
            cursor: pointer;
            padding: 6px;
            color: #555;
        }

        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 20px;
            background-color: #f5f5f5;
        }

        .container {
            display: flex;
            gap: 20px;
            max-width: 1400px;
            margin: 0 auto;
        }

        #toolbox {
            width: 250px;
            border: 1px solid #ddd;
            border-radius: 5px;
            padding: 10px;
            background: #f9f9f9;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }

        .toolbox-header {
            font-size: 16px;
            font-weight: bold;
            margin-bottom: 15px;
            padding-bottom: 10px;
            border-bottom: 1px solid #eee;
        }

        .tool {
            padding: 10px;
            margin: 5px 0;
            background: #fff;
            border: 1px solid #ddd;
            border-radius: 4px;
            cursor: move;
            display: flex;
            align-items: center;
            gap: 8px;
            font-size: 14px;
        }

            .tool i {
                color: #555;
                width: 20px;
                text-align: center;
            }

        #formArea {
            flex: 1;
            min-height: 600px;
            border: 2px dashed #ccc;
            border-radius: 5px;
            padding: 20px;
            background: #fefefe;
            box-shadow: 0 2px 5px rgba(0,0,0,0.05);
        }

        #propertyEditor {
            width: 350px;
            border: 1px solid #ddd;
            border-radius: 5px;
            padding: 15px;
            background: #f9f9f9;
            display: none;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }

        .property-header {
            font-size: 16px;
            font-weight: bold;
            margin-bottom: 15px;
            padding-bottom: 10px;
            border-bottom: 1px solid #eee;
        }

        .form-control {
            margin-bottom: 15px;
            padding: 15px;
            border: 1px solid #eee;
            border-radius: 4px;
            background: #fff;
            position: relative;
            box-shadow: 0 1px 3px rgba(0,0,0,0.05);
        }

            .form-control.selected {
                border: 2px solid #4CAF50;
                background-color: #f8fff8;
            }

            .form-control label {
                display: block;
                margin-bottom: 8px;
                font-weight: bold;
                color: #333;
            }

            .form-control input[type="text"],
            .form-control input[type="password"],
            .form-control input[type="number"],
            .form-control input[type="date"],
            .form-control input[type="file"],
            .form-control select,
            .form-control textarea {
                width: 100%;
                padding: 8px 12px;
                border: 1px solid #ddd;
                border-radius: 4px;
                box-sizing: border-box;
            }

            .form-control textarea {
                min-height: 80px;
            }

        .required-asterisk {
            color: red;
            margin-left: 3px;
        }

        .option-item {
            display: flex;
            align-items: center;
            margin-bottom: 8px;
            gap: 8px;
        }

            .option-item input[type="text"] {
                flex: 1;
                padding: 6px 10px;
                border: 1px solid #ddd;
                border-radius: 4px;
            }

        .option-actions {
            display: flex;
            gap: 5px;
        }

        .btn {
            padding: 8px 12px;
            background: #4CAF50;
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
            display: inline-flex;
            align-items: center;
            gap: 5px;
        }

            .btn:hover {
                background: #45a049;
            }

        .btn-danger {
            background: #f44336;
        }

            .btn-danger:hover {
                background: #d32f2f;
            }

        .btn-sm {
            padding: 5px 8px;
            font-size: 12px;
        }

        .form-actions {
            margin-top: 20px;
            display: flex;
            gap: 10px;
        }

        .tab-content {
            display: none;
        }

            .tab-content.active {
                display: block;
            }

        .tabs {
            display: flex;
            margin-bottom: 15px;
            border-bottom: 1px solid #ddd;
        }

        .tab {
            padding: 8px 15px;
            cursor: pointer;
            border-bottom: 2px solid transparent;
            font-size: 14px;
        }

            .tab.active {
                border-bottom: 2px solid #4CAF50;
                font-weight: bold;
            }

        .help-text {
            font-size: 12px;
            color: #666;
            margin-top: 5px;
            font-style: italic;
        }

        .form-group {
            margin-bottom: 15px;
        }

            .form-group label {
                display: block;
                margin-bottom: 5px;
                font-weight: bold;
                font-size: 14px;
            }

            .form-group input[type="text"],
            .form-group input[type="number"],
            .form-group select {
                width: 100%;
                padding: 8px 12px;
                border: 1px solid #ddd;
                border-radius: 4px;
                box-sizing: border-box;
            }

            .form-group.checkbox {
                display: flex;
                align-items: center;
                gap: 8px;
            }

                .form-group.checkbox input {
                    margin: 0;
                }

        .checkbox-options {
            display: flex;
            flex-direction: column;
            gap: 8px;
        }

            .checkbox-options.inline {
                flex-direction: row;
                flex-wrap: wrap;
            }

                .checkbox-options.inline .option-item {
                    margin-right: 15px;
                }

        .access-roles {
            margin-top: 10px;
        }

            .access-roles select {
                width: 100%;
                height: 100px;
                padding: 5px;
                border: 1px solid #ddd;
                border-radius: 4px;
            }

        .other-option {
            margin-top: 10px;
            display: none;
        }

            .other-option input {
                width: 100%;
                padding: 8px 12px;
                border: 1px solid #ddd;
                border-radius: 4px;
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div id="toolbox">
                <div class="toolbox-header">Form Elements</div>
                <div class="tool" data-type="Text"><i class="fas fa-font"></i>Text Field</div>
                <div class="tool" data-type="TextArea"><i class="fas fa-align-left"></i>Text Area</div>
                <div class="tool" data-type="Number"><i class="fas fa-hashtag"></i>Number</div>
                <div class="tool" data-type="Password"><i class="fas fa-lock"></i>Password</div>
                <div class="tool" data-type="Date"><i class="far fa-calendar-alt"></i>Date Field</div>
                <div class="tool" data-type="File"><i class="far fa-file"></i>File Upload</div>
                <div class="tool" data-type="Radio"><i class="far fa-dot-circle"></i>Radio Group</div>
                <div class="tool" data-type="Checkbox"><i class="far fa-check-square"></i>Checkbox Group</div>
                <div class="tool" data-type="Select"><i class="fas fa-caret-down"></i>Select</div>
                <div class="tool" data-type="Button"><i class="fas fa-hand-pointer"></i>Button</div>
                <div class="tool" data-type="Header"><i class="fas fa-heading"></i>Header</div>
                <div class="tool" data-type="Paragraph"><i class="fas fa-paragraph"></i>Paragraph</div>
            </div>

            <div id="formArea">
                <!-- Form elements will be added here -->
            </div>

            <div id="propertyEditor">
                <div class="property-header">Properties</div>
                <div class="tabs">
                    <div class="tab active" data-tab="basic">Basic</div>
                    <div class="tab" data-tab="advanced">Advanced</div>
                    <div class="tab" data-tab="validation">Validation</div>
                </div>

                <div id="basicTab" class="tab-content active">
                    <div class="form-group">
                        <label for="propLabel">Label</label>
                        <input type="text" id="propLabel" class="form-control" />
                    </div>
                    <div class="form-group">
                        <label for="propName">Name</label>
                        <input type="text" id="propName" class="form-control" />
                    </div>
                    <div id="placeholderContainer" class="form-group">
                        <label for="propPlaceholder">Placeholder</label>
                        <input type="text" id="propPlaceholder" class="form-control" />
                    </div>
                    <div class="form-group">
                        <label for="propHelpText">Help Text</label>
                        <input type="text" id="propHelpText" class="form-control" />
                    </div>
                    <div id="requiredContainer" class="form-group checkbox">
                        <input type="checkbox" id="propRequired" />
                        <label for="propRequired">Required</label>
                    </div>
                    <div id="inlineDisplay" class="form-group checkbox" style="display: none;">
                        <input type="checkbox" id="propInline" />
                        <label for="propInline">Display options inline</label>
                    </div>
                    <div id="enableOther" class="form-group checkbox" style="display: none;">
                        <input type="checkbox" id="propEnableOther" />
                        <label for="propEnableOther">Enable "Other" option</label>
                    </div>
                    <div id="optionsContainer" style="display: none; margin-top: 15px;">
                        <label>Options</label>
                        <div id="optionsList" class="checkbox-options"></div>
                        <button type="button" class="btn btn-sm" id="btnAddOption"><i class="fas fa-plus"></i>Add Option</button>

                    </div>
                </div>

                <div id="advancedTab" class="tab-content">
                    <div class="form-group">
                        <label for="propClass">Class</label>
                        <input type="text" id="propClass" class="form-control" placeholder="space separated classes" />
                    </div>
                    <div class="form-group">
                        <label for="propDefaultValue">Default Value</label>
                        <input type="text" id="propDefaultValue" class="form-control" />
                    </div>
                    <div id="accessContainer" class="form-group" style="display: none;">
                        <label>Access</label>
                        <div class="access-roles">
                            <label>Limit access to one or more of the following roles:</label>
                            <select multiple>
                                <option>Admin</option>
                                <option>Editor</option>
                                <option>Viewer</option>
                                <option>Guest</option>
                            </select>
                        </div>
                    </div>
                </div>

                <div id="validationTab" class="tab-content">
                    <div class="form-group">
                        <label for="propMinLength">Min Length</label>
                        <input type="number" id="propMinLength" class="form-control" />
                    </div>
                    <div class="form-group">
                        <label for="propMaxLength">Max Length</label>
                        <input type="number" id="propMaxLength" class="form-control" />
                    </div>
                    <div class="form-group">
                        <label for="propPattern">Pattern (RegEx)</label>
                        <input type="text" id="propPattern" class="form-control" />
                    </div>
                </div>

                <div class="form-actions">
                    <button type="button" class="btn" onclick="applyProperties()"><i class="fas fa-check"></i>Apply</button>
                    <button type="button" class="btn btn-danger" onclick="cancelProperties()"><i class="fas fa-times"></i>Cancel</button>
                </div>
            </div>
        </div>

        <div class="form-actions" style="margin-top: 20px; justify-content: center;">
            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-danger" OnClientClick="clearForm(); return false;" />
            <asp:Button ID="btnSave" runat="server" Text="Save Form" CssClass="btn" OnClick="btnSubmit_Click" />
        </div>

        <asp:HiddenField ID="hfFormData" runat="server" />
    </form>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://code.jquery.com/ui/1.13.0/jquery-ui.min.js"></script>
    <script>
        $(function () {
            // Make tools draggable
            $(".tool").draggable({
                helper: "clone",
                revert: "invalid"
            });

            // Make form area droppable
            $("#formArea").droppable({
                accept: ".tool",
                drop: function (event, ui) {
                    var type = ui.draggable.data("type");
                    var id = "control_" + Date.now();
                    var html = createControlHtml(type, id);
                    $(this).append(html);

                    // Select the newly added control
                    $("#" + id).click();
                }
            });

            // FIXED: Changed event delegation to use document instead of #optionsContainer
            $(document).on("click", "#btnAddOption", function (e) {
                e.preventDefault();
                e.stopPropagation();
                addOption();
            });

            // Alternative: If the button has a specific class, use that instead
            $(document).on("click", ".add-option-btn", function (e) {
                e.preventDefault();
                e.stopPropagation();
                addOption();
            });
            // Tab switching
            $(".tab").click(function () {
                $(".tab").removeClass("active");
                $(this).addClass("active");
                var tabId = $(this).data("tab") + "Tab";
                $(".tab-content").removeClass("active");
                $("#" + tabId).addClass("active");
            });
            $("#formArea").on("click", ".form-control.edit-mode", function (e) {
                e.stopPropagation();
                $(".form-control").removeClass("selected");
                $(this).addClass("selected");
                showProperties($(this));
            });
            //$("#formArea").on("click", ".form-control", function (e) {
            //    e.stopPropagation(); // optional, prevents triggering outer clicks

            //    $(".form-control").removeClass("selected");
            //    $(this).addClass("selected");
            //    showProperties($(this));
            //});

            // Close property editor when clicking outside (but not inside property editor or form controls)
            $(document).click(function (e) {
                // Check if clicked outside both editor and any form control
                if (!$(e.target).closest("#propertyEditor").length &&
                    !$(e.target).closest(".form-control").length) {

                    // Don't close if any form control is in edit mode
                    if ($(".form-control.edit-mode").length === 0) {
                        $(".form-control").removeClass("selected");
                        $("#propertyEditor").hide();
                    }
                }
            });


            $(document).on("click", ".delete-btn", function (e) {
                e.preventDefault();
                e.stopPropagation();
                var control = $(this).closest(".form-control");
                //deleteControl(control);//
            });
            // Prevent property editor from closing when clicking inside it
            $("#propertyEditor").click(function (e) {
                e.stopPropagation();
            });
        });

        function createControlHtml(type, id) {
            var html = '';
            var name = type.toLowerCase() + "_" + Date.now();
            var controlActions = `
        <div class="control-actions">
            <button type="button" class="edit-btn" onclick="enterEditMode($('#${id}'))">
                <i class="fas fa-edit"></i>
            </button>
            <button type="button" class="delete-btn" onclick="deleteControl($('#${id}'))">
                <i class="fas fa-trash"></i>
            </button>
        </div>`;

            switch (type) {
                case "Text":
                    html = `<div class="form-control" id="${id}" data-type="${type}">
                        <label>Text Field</label>
                        <input type="text" name="${name}" placeholder="Enter text" />
                        ${controlActions}
                    </div>`;
                    break;
                case "TextArea":
                    html = `<div class="form-control" id="${id}" data-type="${type}">
                        <label>Text Area</label>
                        <textarea name="${name}" placeholder="Enter text"></textarea>
                        ${controlActions}
                    </div>`;
                    break;
                case "Number":
                    html = `<div class="form-control" id="${id}" data-type="${type}">
                        <label>Number</label>
                        <input type="number" name="${name}" placeholder="Enter number" />
                        ${controlActions}
                    </div>`;
                    break;
                case "Password":
                    html = `<div class="form-control" id="${id}" data-type="${type}">
                        <label>Password</label>
                        <input type="password" name="${name}" placeholder="Enter password" />
                        ${controlActions}
                    </div>`;
                    break;
                case "Date":
                    html = `<div class="form-control" id="${id}" data-type="${type}">
                        <label>Date Field</label>
                        <div class="date-picker-wrapper">
                            <input type="text" name="${name}" class="date-input" placeholder="Select date" />
                            <button type="button" class="calendar-btn" onclick="showCalendar(this)">
                                <i class="fas fa-calendar-alt"></i>
                            </button>
                        </div>
                        ${controlActions}
                    </div>`;
                    break;
                case "File":
                    html = `<div class="form-control" id="${id}" data-type="${type}">
                        <label>File Upload</label>
                        <input type="file" name="${name}" />
                        ${controlActions}
                    </div>`;
                    break;
                case "Radio":
                    html = `<div class="form-control" id="${id}" data-type="${type}">
                        <label>Radio Group</label>
                        <div class="radio-options">
                            <div class="option-item">
                                <input type="radio" name="${name}" id="${name}_1" value="Option 1" />
                                <label for="${name}_1">Option 1</label>
                            </div>
                            <div class="option-item">
                                <input type="radio" name="${name}" id="${name}_2" value="Option 2" />
                                <label for="${name}_2">Option 2</label>
                            </div>
                        </div>
                        ${controlActions}
                    </div>`;
                    break;
                case "Checkbox":
                    html = `<div class="form-control" id="${id}" data-type="${type}">
                        <label>Checkbox Group</label>
                        <div class="checkbox-options">
                            <div class="option-item">
                                <input type="checkbox" name="${name}[]" id="${name}_1" value="Option 1" />
                                <label for="${name}_1">Option 1</label>
                            </div>
                            <div class="option-item">
                                <input type="checkbox" name="${name}[]" id="${name}_2" value="Option 2" />
                                <label for="${name}_2">Option 2</label>
                            </div>
                        </div>
                        ${controlActions}
                    </div>`;
                    break;
                case "Select":
                    html = `<div class="form-control" id="${id}" data-type="${type}">
                        <label>Select</label>
                        <select name="${name}">
                            <option value="">Select an option</option>
                            <option value="Option 1">Option 1</option>
                            <option value="Option 2">Option 2</option>
                        </select>
                        ${controlActions}
                    </div>`;
                    break;
                case "Button":
                    html = `<div class="form-control" id="${id}" data-type="${type}">
                        <button type="button" name="${name}" class="btn">Button</button>
                        ${controlActions}
                    </div>`;
                    break;
                case "Header":
                    html = `<div class="form-control" id="${id}" data-type="${type}">
                        <h3>Header</h3>
                        ${controlActions}
                    </div>`;
                    break;
                case "Paragraph":
                    html = `<div class="form-control" id="${id}" data-type="${type}">
                        <p>Paragraph text goes here.</p>
                        ${controlActions}
                    </div>`;
                    break;
            }

            return html;
        }

        function enterEditMode(control) {
            // Remove edit mode from all other controls
            $(".form-control").removeClass("edit-mode selected");
            $("#propertyEditor").hide();

            // Enable edit mode for this control
            control.addClass("edit-mode selected");

            // Enable form inputs within this control
            control.find("input, textarea, select, button").not(".edit-btn, .delete-btn").prop("disabled", false);

            // Show properties panel
            showProperties(control);

            // Show visual indication that control is in edit mode
            control.css("border", "2px solid #007bff");
        }

        function exitEditMode(control) {
            control.removeClass("edit-mode selected");
            control.find("input, textarea, select, button").not(".edit-btn, .delete-btn").prop("disabled", true);
            control.css("border", "");
            $("#propertyEditor").hide();
        }

        function deleteControl(control) {
            if (confirm("Are you sure you want to delete this field?")) {
                control.remove();
                $("#propertyEditor").hide();
            }
        }

        function showProperties(control) {
            var type = control.data("type");
            $("#propertyEditor").show();

            // Reset all optional fields visibility
            $("#placeholderContainer, #requiredContainer, #inlineDisplay, #enableOther, #optionsContainer, #accessContainer").hide();

            // Show fields based on control type
            switch (type) {
                case "Text":
                case "TextArea":
                case "Number":
                case "Password":
                case "Date":
                case "File":
                    $("#placeholderContainer, #requiredContainer").show();
                    $("#propLabel").val(control.find("label:first").text().replace('*', '').trim());
                    $("#propName").val(control.find("input, textarea").first().attr("name") || '');
                    $("#propPlaceholder").val(control.find("input, textarea").first().attr("placeholder") || '');
                    $("#propHelpText").val(control.find(".help-text").text() || '');
                    $("#propRequired").prop("checked", control.find("input, textarea").first().prop("required"));
                    break;

                case "Radio":
                case "Checkbox":
                    $("#requiredContainer, #inlineDisplay, #enableOther, #optionsContainer").show();
                    $("#propLabel").val(control.find("label:first").text().replace('*', '').trim());
                    $("#propName").val(control.find("input[type='" + type.toLowerCase() + "']").first().attr("name").replace('[]', ''));
                    $("#propHelpText").val(control.find(".help-text").text() || '');
                    $("#propRequired").prop("checked", control.data("required") || false);
                    $("#propInline").prop("checked", control.find(".checkbox-options").hasClass("inline"));
                    $("#propEnableOther").prop("checked", control.find('input[value="other"]').length > 0);
                    loadOptions(control);
                    break;

                case "Select":
                    $("#requiredContainer, #optionsContainer").show();
                    $("#propLabel").val(control.find("label:first").text().replace('*', '').trim());
                    $("#propName").val(control.find("select").attr("name"));
                    $("#propHelpText").val(control.find(".help-text").text() || '');
                    $("#propRequired").prop("checked", control.find("select option:first").val() === '');
                    loadOptions(control);
                    break;

                case "Header":
                    $("#propLabel").val(control.find("h1, h2, h3, h4, h5, h6").first().text());
                    $("#propName").val('');
                    break;

                case "Paragraph":
                    $("#propLabel").val(control.find("p").first().text());
                    $("#propName").val('');
                    break;

                case "Button":
                    $("#propLabel").val(control.find("button").first().text());
                    $("#propName").val(control.find("button").attr("name") || '');
                    break;
            }
            $("#btnAddOption").off("click").on("click", function (e) {
                e.preventDefault();
                e.stopPropagation();
                addOption();
            });
            // Show advanced properties for all controls
            $("#accessContainer").show();

            // Store reference to current control
            $("#propertyEditor").data("currentControl", control);
        }

        function loadOptions(control) {
            $("#optionsList").empty();
            var type = control.data("type");

            if (type === "Select") {
                control.find("select option").each(function () {
                    if ($(this).val() !== '') {
                        addOptionToEditor($(this).val(), $(this).text());
                    }
                });
            } else if (type === "Radio" || type === "Checkbox") {
                control.find(".option-item").each(function () {
                    var input = $(this).find("input");
                    if (input.val() !== "other") { // Skip "other" option
                        var value = input.val();
                        var label = $(this).find("label").text();
                        addOptionToEditor(value, label);
                    }
                });
            }

            // Add one empty option if none exist
            if ($("#optionsList").children().length === 0) {
                addOption();
            }
        }

        function addOption() {
            console.log("addOption function called");
            addOptionToEditor('', '');

            // Force refresh of the property panel
            var control = $("#propertyEditor").data("currentControl");
            if (control) {
                // Don't call showProperties again as it would reset everything
                // Just ensure the control stays selected
                control.addClass("selected");
            }
        }

        function addOptionToEditor(value, text) {
            var optionId = Date.now();
            var html = `
<div class="option-item" data-id="${optionId}">
    <input type="text" class="option-value" value="${value}" placeholder="Value" />
    <input type="text" class="option-text" value="${text}" placeholder="Display Text" />
    <button class="btn btn-danger btn-sm" onclick="removeOption(${optionId})">
        <i class="fas fa-trash"></i>
    </button>
</div>`;
            $("#optionsList").append(html);
        }

        function removeOption(id) {
            $(`.option-item[data-id="${id}"]`).remove();
        }

        function applyProperties() {
            var control = $("#propertyEditor").data("currentControl");
            if (!control) return;

            var type = control.data("type");
            var label = $("#propLabel").val();
            var name = $("#propName").val();
            var placeholder = $("#propPlaceholder").val();
            var helpText = $("#propHelpText").val();
            var required = $("#propRequired").is(":checked");
            var inline = $("#propInline").is(":checked");
            var enableOther = $("#propEnableOther").is(":checked");
            var defaultValue = $("#propDefaultValue").val();
            var cssClass = $("#propClass").val();

            // Update control based on type
            switch (type) {
                case "Text":
                case "TextArea":
                case "Number":
                case "Password":
                case "Date":
                case "File":
                    if (label) {
                        control.find("label:first").html(label + (required ? '<span class="required-asterisk">*</span>' : ''));

                    }
                    if (name) {
                        control.find("input, textarea").first().attr("name", name);
                    }
                    if (placeholder) {
                        control.find("input, textarea").first().attr("placeholder", placeholder);
                    }
                    control.find("input, textarea").first().prop("required", required);
                    break;

                case "Radio":
                case "Checkbox":
                    if (label) {
                        control.find("label:first").html(label + (required ? '<span class="required-asterisk">*</span>' : ''));

                    }
                    if (name) {
                        control.find("input[type='" + type.toLowerCase() + "']").attr("name", type === "Checkbox" ? name + "[]" : name);
                    }
                    control.data("required", required);
                    updateControlOptions(control, required, inline, enableOther, defaultValue);
                    break;

                case "Select":
                    if (label) {
                        control.find("label:first").html(label + (required ? '<span class="required-asterisk">*</span>' : ''));

                    }
                    if (name) {
                        control.find("select").attr("name", name);
                    }
                    updateControlOptions(control, required, inline, enableOther, defaultValue);
                    break;

                case "Header":
                    if (label) {
                        control.find("h1, h2, h3, h4, h5, h6").first().text(label);
                    }
                    break;

                case "Paragraph":
                    if (label) {
                        control.find("p").first().text(label);
                    }
                    break;

                case "Button":
                    if (label) {
                        control.find("button").first().text(label);
                    }
                    if (name) {
                        control.find("button").attr("name", name);
                    }
                    break;
            }

            // Update help text if it exists
            if (helpText) {
                var helpTextElement = control.find(".help-text");
                if (helpTextElement.length === 0) {
                    control.append('<div class="help-text">' + helpText + '</div>');
                } else {
                    helpTextElement.text(helpText);
                }
            } else {
                control.find(".help-text").remove();
            }

            // Update CSS class
            if (cssClass) {
                control.attr("class", "form-control " + cssClass);
            } else {
                control.attr("class", "form-control");
            }

            // Hide property editor and deselect control
            $("#propertyEditor").hide();
            control.removeClass("selected");
        }

        function updateControlOptions(control, required, inline, enableOther, defaultValue) {
            var type = control.data("type");
            var name = control.find("input, select").first().attr("name").replace('[]', '');
            var optionsContainer = type === "Select" ? control.find("select") : control.find(".radio-options, .checkbox-options");

            // Clear existing options
            if (type === "Select") {
                optionsContainer.empty();
                if (required) {
                    optionsContainer.append('<option value="">Select an option</option>');
                }
            } else {
                optionsContainer.empty();
                if (inline) {
                    optionsContainer.addClass("inline");
                } else {
                    optionsContainer.removeClass("inline");
                }
            }

            // Add new options
            $("#optionsList .option-item").each(function () {
                var value = $(this).find(".option-value").val();
                var text = $(this).find(".option-text").val();

                if (value && text) {
                    if (type === "Select") {
                        optionsContainer.append(`<option value="${value}" ${defaultValue === value ? 'selected' : ''}>${text}</option>`);
                    } else {
                        var optionId = name + "_" + value.replace(/\s+/g, '-').toLowerCase();
                        var html = `
                    <div class="option-item">
                        <input type="${type.toLowerCase()}" name="${type === 'Checkbox' ? name + '[]' : name}" 
                            id="${optionId}" value="${value}" ${defaultValue === value ? 'checked' : ''} />
                        <label for="${optionId}">${text}</label>
                    </div>`;
                        optionsContainer.append(html);
                    }
                }
            });

            // Add "Other" option if enabled
            if (enableOther && (type === "Radio" || type === "Checkbox")) {
                var otherId = name + "_other";
                var html = `
            <div class="option-item">
                <input type="${type.toLowerCase()}" name="${type === 'Checkbox' ? name + '[]' : name}" 
                    id="${otherId}" value="other" />
                <label for="${otherId}">Other</label>
            </div>
            <div class="other-option">
                <input type="text" name="${name}_other_text" placeholder="Please specify" />
            </div>`;
                optionsContainer.append(html);

                // Show/hide other text field based on selection
                control.find(`#${otherId}`).change(function () {
                    if ($(this).is(":checked")) {
                        $(this).closest(".option-item").next(".other-option").show();
                    } else {
                        $(this).closest(".option-item").next(".other-option").hide().find("input").val('');
                    }
                });
            }
        }

        function cancelProperties() {
            $("#propertyEditor").hide();
            $(".form-control").removeClass("selected");
        }

        function clearForm() {
            if (confirm("Are you sure you want to clear the form?")) {
                $("#formArea").empty();
            }
        }

        function showCalendar(btn) {
            var input = $(btn).siblings(".date-input");

            // Initialize Flatpickr only once
            if (!input.hasClass("flatpickr-attached")) {
                flatpickr(input[0], {
                    dateFormat: "Y-m-d",
                    allowInput: true,
                    onClose: function () {
                        input.removeClass("flatpickr-attached");
                    }
                });
                input.addClass("flatpickr-attached");
            }

            // Programmatically open Flatpickr
            input[0]._flatpickr.open();
        }

        // Before form submission, serialize the form configuration
        $("form").submit(function () {
            var formData = [];
            $("#formArea .form-control").each(function () {
                var control = $(this);
                var controlData = {
                    type: control.data("type"),
                    label: control.find("label:first").text().replace('*', '').trim(),
                    name: control.find("input, select, textarea, button").first().attr("name"),
                    placeholder: control.find("input, textarea").first().attr("placeholder") || '',
                    helpText: control.find(".help-text").text() || '',
                    required: control.data("required") || control.find("input, select, textarea").first().prop("required"),
                    options: getControlOptions(control),
                    cssClass: control.attr("class").replace("form-control", "").trim()
                };

                formData.push(controlData);
            });
            $("#hfFormData").val(JSON.stringify(formData));
        });

        function getControlOptions(control) {
            var type = control.data("type");
            var options = [];

            if (type === "Select") {
                control.find("option").each(function () {
                    if ($(this).val() !== '') {
                        options.push({
                            value: $(this).val(),
                            text: $(this).text()
                        });
                    }
                });
            } else if (type === "Radio" || type === "Checkbox") {
                control.find(".option-item input").each(function () {
                    if ($(this).val() !== "other") { // Skip "other" option
                        options.push({
                            value: $(this).val(),
                            text: $(this).next("label").text()
                        });
                    }
                });
            }

            return options;
        }
    </script>
</body>
</html>


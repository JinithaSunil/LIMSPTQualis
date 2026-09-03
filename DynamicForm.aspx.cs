using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LIMS
{
    public partial class DynamicForm : Page
    {
        DAL.CM _cm = new DAL.CM();
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadDynamicControls();
        }

        private void LoadDynamicControls()
        {
            string query = @"SELECT * FROM DynamicFormControlTb ORDER BY CreatedDateTime";

            using (SqlConnection conn = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                Panel formRow = new Panel { CssClass = "row" };
                phFormContainer.Controls.Add(formRow);

                while (reader.Read())
                {
                    string controlType = reader["ControlType"].ToString().ToLower();
                    string controlId = reader["ControlTypeId"].ToString();
                    string labelText = reader["ControlLabelName"].ToString();
                    string cssClass = reader["ControlTypeclass"].ToString();
                    string columnSize = reader["ColumnSize"].ToString() ?? "col-md-6";
                    string controlSubType = reader["ControlSubType"]?.ToString().ToLower();
                    string dataSource = reader["DataSourceName"]?.ToString();
                    string fileAcceptTypes = reader["AcceptFileTypes"]?.ToString();
                    string controlValues = reader["ControlValues"]?.ToString();

                    Panel formGroup = new Panel { CssClass = $"{columnSize} form-group mb-3" };

                    Label lbl = new Label
                    {
                        ID = "lbl" + controlId,
                        Text = labelText,
                        CssClass = "form-label d-block"
                    };
                    formGroup.Controls.Add(lbl);

                    WebControl control = null;

                    switch (controlType)
                    {
                        case "textbox":
                            var txt = new TextBox
                            {
                                ID = controlId,
                                CssClass = $"form-control {cssClass}"
                            };

                            if (controlSubType == "password")
                                txt.TextMode = TextBoxMode.Password;
                            else if (controlSubType == "multiline")
                            {
                                txt.TextMode = TextBoxMode.MultiLine;
                                txt.Rows = 4;
                            }
                            else if (controlSubType == "date")
                                txt.TextMode = TextBoxMode.Date;
                            else if (controlSubType == "time")
                                txt.TextMode = TextBoxMode.Time;
                            else if (controlSubType == "datetime")
                                txt.TextMode = TextBoxMode.DateTimeLocal;
                            else if (controlSubType == "number")
                                txt.TextMode = TextBoxMode.Number;

                            control = txt;
                            break;

                        case "dropdownlist":
                            var ddl = new DropDownList
                            {
                                ID = controlId,
                                CssClass = $"form-select {cssClass}"
                            };
                            BindListControlData(ddl, dataSource, controlValues);
                            control = ddl;
                            break;


                        case "checkbox":
                            var cbl = new CheckBoxList
                            {
                                ID = controlId,
                                CssClass = cssClass,
                                RepeatDirection = RepeatDirection.Horizontal
                            };
                            BindListControlData(cbl, dataSource, controlValues);
                            control = cbl;
                            break;

                        case "radiobuttonlist":
                            var rbl = new RadioButtonList
                            {
                                ID = controlId,
                                CssClass = "radio-list",
                                RepeatDirection = RepeatDirection.Horizontal,
                                RepeatLayout = RepeatLayout.Flow
                            };
                            BindListControlData(rbl, dataSource, controlValues);

                            // Apply styles to remove shadows and fix spacing
                            rbl.Attributes.Add("style", "list-style-type:none; padding:0; margin:0;");

                            // Apply to individual radio buttons
                            foreach (ListItem item in rbl.Items)
                            {
                                item.Attributes["class"] = "radio-item";
                            }

                            control = rbl;
                            break;

                        case "checkboxlist":
                            if (controlSubType == "checkboxlist")
                            {
                                Panel termsContainer = new Panel { CssClass = "terms-container" };

                                // Main label
                                termsContainer.Controls.Add(new LiteralControl(
                                    $"<div class='terms-label'>{labelText}</div>"));
                                lbl.Visible = false;

                                string[] options = !string.IsNullOrEmpty(controlValues) ?
                                    controlValues.Split(',') : new[] { "Yes", "No" };

                                foreach (string option in options)
                                {
                                    string trimmedOption = option.Trim();

                                    Panel optionPanel = new Panel { CssClass = "checkbox-option" };

                                    CheckBox chk = new CheckBox
                                    {
                                        ID = $"{controlId}_{trimmedOption}",
                                        CssClass = "clean-checkbox",
                                        Text = trimmedOption
                                    };

                                    optionPanel.Controls.Add(chk);
                                    termsContainer.Controls.Add(optionPanel);
                                }

                                control = termsContainer;
                            }
                            else
                            {
                                CheckBox chk = new CheckBox
                                {
                                    ID = controlId,
                                    CssClass = "clean-checkbox",
                                    Text = labelText
                                };
                                lbl.Visible = false;
                                control = chk;
                            }
                            break;

                        case "fileupload":
                            var fu = new FileUpload
                            {
                                ID = controlId,
                                CssClass = $"form-control {cssClass}"
                            };
                            if (!string.IsNullOrEmpty(fileAcceptTypes))
                                fu.Attributes["accept"] = fileAcceptTypes;
                            control = fu;
                            break;
                    }

                    if (control != null)
                    {
                        formGroup.Controls.Add(control);
                        formRow.Controls.Add(formGroup);
                    }
                }
            }
        }

        private void BindListControlData(ListControl listControl, string dataSource, string fallbackControlValues)
        {
            // Existing implementation remains the same
            if (!string.IsNullOrWhiteSpace(dataSource))
            {
                if (dataSource.ToLower() == "uom")
                {
                    DataTable dt = _cm.getUOM();
                    if (dt.Rows.Count > 0)
                    {
                        listControl.DataSource = dt;
                        listControl.DataTextField = "UOMName";
                        listControl.DataValueField = "UOMId";
                        listControl.DataBind();

                        if (listControl is DropDownList)
                            listControl.Items.Insert(0, new ListItem("--Select UOM--", ""));
                        return;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(fallbackControlValues))
            {
                var values = fallbackControlValues.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string val in values)
                {
                    string trimmed = val.Trim();
                    listControl.Items.Add(new ListItem(trimmed, trimmed));
                }

                if (listControl is DropDownList)
                    listControl.Items.Insert(0, new ListItem("--Select--", ""));
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("<div class='alert alert-info mt-3'><h4>Form Submitted</h4><ul>");

            foreach (Control outer in phFormContainer.Controls)
            {
                if (outer is Panel rowPanel)
                {
                    foreach (Control inner in rowPanel.Controls)
                    {
                        if (inner is Panel group)
                        {
                            string label = GetLabelText(group);

                            foreach (Control ctrl in group.Controls)
                            {
                                if (ctrl is TextBox txt)
                                    sb.Append($"<li>{label}: {txt.Text}</li>");

                                else if (ctrl is DropDownList ddl)
                                    sb.Append($"<li>{label}: {ddl.SelectedItem.Text}</li>");

                                else if (ctrl is RadioButtonList rbl)
                                    sb.Append($"<li>{label}: {rbl.SelectedItem?.Text}</li>");

                                else if (ctrl is CheckBoxList cbl)
                                {
                                    var selected = string.Join(", ", cbl.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Text));
                                    sb.Append($"<li>{label}: {selected}</li>");
                                }

                                else if (ctrl is CheckBox chk)
                                {
                                    // Handle both regular checkboxes and terms checkboxes
                                    if (chk.ID.EndsWith("_Yes") || chk.ID.EndsWith("_No"))
                                    {
                                        sb.Append($"<li>{chk.Text}: {(chk.Checked ? "Checked" : "Unchecked")}</li>");
                                    }
                                    else
                                    {
                                        sb.Append($"<li>{label}: {(chk.Checked ? "Yes" : "No")}</li>");
                                    }
                                }

                                else if (ctrl is FileUpload fu && fu.HasFile)
                                {
                                    string fileName = System.IO.Path.GetFileName(fu.FileName);
                                    fu.SaveAs(Server.MapPath("~/SampleImages/" + fileName));
                                    sb.Append($"<li>{label}: Uploaded {fileName}</li>");
                                }
                            }
                        }
                    }
                }
            }

            sb.Append("</ul></div>");
            phFormContainer.Controls.Add(new LiteralControl(sb.ToString()));
        }

        private string GetLabelText(Control group)
        {
            foreach (Control c in group.Controls)
                if (c is Label lbl && lbl.Visible) return lbl.Text;
            return "";
        }


    }
}
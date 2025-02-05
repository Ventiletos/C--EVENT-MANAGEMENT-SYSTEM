using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DarkDemo
{
    public partial class student : Form, IThemeable
    {
        MySqlConnection conn;
        MySqlCommand cmd;
        MySqlDataReader dr;
        private string id, sid, sname, contactno, address, department, section;
        private Image selectedImage; // Add this at the class level


        string conns = "server=localhost;port=3307;username=root;password=;database=events_management;";

        public student()
        {
            InitializeComponent();
            conn = new MySqlConnection(conns);
            LoadData();
            SearchData("");
        }

        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        //Add
        private void btnnew_Click(object sender, EventArgs e)
        {
            studentForm sf = new studentForm(this);
            sf.btnUpdate.BackColor = Color.DarkGray;
            sf.btnUpdate.Enabled = false;
            sf.ShowDialog();
        }

        //Load
        public void LoadData(string department = "All", string section = "All")
        {
            guna2DataGridView1.Rows.Clear();

            string query = "SELECT `id`, `studentid`, `dp`, `studentname`, `contactno`, `address`, `department`, `section` FROM `student_table` WHERE 1=1";

            // Add department and section filters if not "All"
            if (department != "All")
            {
                query += " AND department = @department";
            }
            if (section != "All")
            {
                query += " AND section = @section";
            }

            cmd = new MySqlCommand(query, conn);

            // Add parameters if filters are applied
            if (department != "All")
            {
                cmd.Parameters.AddWithValue("@department", department);
            }
            if (section != "All")
            {
                cmd.Parameters.AddWithValue("@section", section);
            }

            try
            {
                conn.Open();
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    byte[] imagedata = dr["dp"] as byte[];
                    Image image = null;

                    if (imagedata != null && imagedata.Length > 0)
                    {
                        using (MemoryStream ms = new MemoryStream(imagedata))
                        {
                            image = Image.FromStream(ms, true, true);
                        }
                    }

                    guna2DataGridView1.Rows.Add(
                        guna2DataGridView1.RowCount + 1,
                        dr["id"].ToString(),
                        dr["studentid"].ToString(),
                        dr["studentname"].ToString(),
                        dr["contactno"].ToString(),
                        dr["address"].ToString(),
                        dr["department"].ToString(),
                        dr["section"].ToString(),
                        image
                    );
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void student_Load(object sender, EventArgs e)
        {
            LoadComboBoxes();
            LoadData();
        }

        //DataGrid
        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = guna2DataGridView1.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                studentForm sf = new studentForm(this);
                sf.btnSave.Enabled = false;
                sf.btnUpdate.Enabled = true;

                // Populate text fields as well
                sf.txtsid.Text = sid;
                sf.txtname.Text = sname;
                sf.txtno.Text = contactno;
                sf.txtaddress.Text = address;
                sf.txtdepartment.Text = department;
                sf.txtsection.Text = section;
                if (selectedImage != null)
                {
                    sf.pictureBoxDP.Image = selectedImage;
                }
                sf.ShowDialog();

                
                //if()

            }
            else if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure you want to delete this Data?", "Delete Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MySqlTransaction transaction = null;
                    try
                    {
                        // Open the connection only if it is not already open
                        if (conn.State != System.Data.ConnectionState.Open)
                        {
                            conn.Open();
                        }

                        // Start a transaction
                        transaction = conn.BeginTransaction();

                        // Log the Event ID for debugging
                        Console.WriteLine($"Event ID being deleted: {id}");

                        // Delete related records from report_table
                        string deleteReportQuery = "DELETE FROM report_table WHERE studentid = @studentid";
                        cmd = new MySqlCommand(deleteReportQuery, conn, transaction);
                        cmd.Parameters.AddWithValue("@studentid", sid);
                        int reportRowsAffected = cmd.ExecuteNonQuery();

                        if (reportRowsAffected > 0)
                        {
                            // If rows are affected in report_table, proceed with deleting from event_table
                            string deleteEventQuery = "DELETE FROM student_table WHERE id = @studentid";
                            cmd = new MySqlCommand(deleteEventQuery, conn, transaction);
                            cmd.Parameters.AddWithValue("@studentid", id);
                            int eventRowsAffected = cmd.ExecuteNonQuery();

                            if (eventRowsAffected > 0)
                            {
                                // Commit transaction if both deletes are successful
                                transaction.Commit();
                                MessageBox.Show("Data deleted successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                // Event deletion failed
                                MessageBox.Show("Event data deletion failed. No matching event found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                transaction.Rollback();
                            }
                        }
                        else
                        {
                            // No rows were deleted from report_table
                            MessageBox.Show("No related report data found to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            transaction.Rollback();
                        }

                        // Close the connection after the transaction and reload data
                        conn.Close();
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        // Print error details
                        Console.WriteLine($"Error: {ex.Message}");

                        // Handle errors and rollback transaction
                        transaction?.Rollback();
                        MessageBox.Show($"Something went wrong! {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        // Ensure the connection is closed after the operation if it's still open
                        if (conn.State == System.Data.ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }
        }
        private void guna2DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            int i = guna2DataGridView1.CurrentRow.Index;

            // Accessing regular text columns
            id = guna2DataGridView1[1, i].Value.ToString();
            sid = guna2DataGridView1[2, i].Value.ToString();
            sname = guna2DataGridView1[3, i].Value.ToString();
            contactno = guna2DataGridView1[4, i].Value.ToString();
            address = guna2DataGridView1[5, i].Value.ToString();
            department = guna2DataGridView1[6, i].Value.ToString();
            section = guna2DataGridView1[7, i].Value.ToString(); // Assuming section is in column 7
            var cellValue = guna2DataGridView1[8, i].Value;
            if (cellValue is Image image)
            {
                selectedImage = image; // Set the selected image if it's already an Image type
            }
            else
            {
                selectedImage = null;
            }
        }
        public void SearchData(string data)
        {
            int i = 0;
            guna2DataGridView1.Rows.Clear();

            string query = "SELECT * FROM student_table WHERE CONCAT(studentid, studentname, contactno, address) LIKE @search";

            bool hasDepartmentFilter = cmbdepartment.SelectedItem != null && cmbdepartment.SelectedItem.ToString() != "All";
            bool hasSectionFilter = cmbsection.SelectedItem != null && cmbsection.SelectedItem.ToString() != "All";

            if (hasDepartmentFilter)
            {
                query += " AND department = @department";
            }
            if (hasSectionFilter)
            {
                query += " AND section = @section";
            }

            cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@search", "%" + data + "%");

            if (hasDepartmentFilter)
            {
                cmd.Parameters.AddWithValue("@department", cmbdepartment.SelectedItem.ToString());
            }
            if (hasSectionFilter)
            {
                cmd.Parameters.AddWithValue("@section", cmbsection.SelectedItem.ToString());
            }

            try
            {
                conn.Open();
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    i += 1;
                    guna2DataGridView1.Rows.Add(
                        i,
                        dr["id"].ToString(),
                        dr["studentid"].ToString(),
                        dr["studentname"].ToString(),
                        dr["contactno"].ToString(),
                        dr["address"].ToString(),
                        dr["department"].ToString(),
                        dr["section"].ToString()
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching data: " + ex.Message, "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dr.Close();
                conn.Close();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchData(txtSearch.Text);
        }

        //for the themes
        public void ChangeTheme(bool IsDarkMode)
        {
            mainPanel.BackColor = IsDarkMode ? Color.FromArgb(255, 255, 255) : Color.FromArgb(41, 44, 51);
            bpanel.BackgroundColor = IsDarkMode ? Color.Gainsboro : Color.FromArgb(36, 39, 45);
            bpanel1.BackgroundColor = IsDarkMode ? Color.Gainsboro : Color.FromArgb(36, 39, 45);
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = IsDarkMode ? Color.Gainsboro : Color.FromArgb(36, 39, 45);
            guna2DataGridView1.BackgroundColor = IsDarkMode ? Color.Gainsboro : Color.FromArgb(36, 39, 45);
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = IsDarkMode ? Color.Gainsboro : Color.FromArgb(36, 39, 45);
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = IsDarkMode ? Color.FromArgb(64, 64, 64) : Color.White;
            guna2DataGridView1.Refresh();

            cmbdepartment.ForeColor = IsDarkMode ? Color.FromArgb(41, 44, 51) : Color.Gainsboro;
            cmbsection.ForeColor = IsDarkMode ? Color.FromArgb(41, 44, 51) : Color.Gainsboro;
            btnnew.ForeColor = IsDarkMode ? Color.FromArgb(41, 44, 51) : Color.Gainsboro;
            lbldepartment.ForeColor = IsDarkMode ? Color.FromArgb(41, 44, 51) : Color.Gainsboro;
            lblsection.ForeColor = IsDarkMode ? Color.FromArgb(41, 44, 51) : Color.Gainsboro;

            cmbdepartment.FillColor = IsDarkMode ? Color.WhiteSmoke : Color.FromArgb(41, 44, 51);
            cmbsection.FillColor = IsDarkMode ? Color.WhiteSmoke : Color.FromArgb(41, 44, 51);
            btnnew.FillColor = IsDarkMode ? Color.WhiteSmoke : Color.FromArgb(41, 44, 51);
            lbldepartment.BackColor = IsDarkMode ? Color.WhiteSmoke : Color.FromArgb(41, 44, 51);
            lblsection.BackColor = IsDarkMode ? Color.WhiteSmoke : Color.FromArgb(41, 44, 51);

            cmbdepartment.BackColor = IsDarkMode ? Color.Gainsboro : Color.FromArgb(36, 39, 45);
            cmbsection.BackColor = IsDarkMode ? Color.Gainsboro : Color.FromArgb(36, 39, 45);
        }

        //some random shits
        private void cmb_Click(object sender, EventArgs e)
        {
            if (sender == cmbdepartment)
                lbldepartment.Visible = false;
            else if (sender == cmbsection)
                lblsection.Visible = false;
        }
        private void cmb_Leave(object sender, EventArgs e)
        {
            if (sender == cmbdepartment && cmbdepartment.SelectedIndex == -1)
                lbldepartment.Visible = true;
            else if (sender == cmbsection && cmbsection.SelectedIndex == -1)
                lblsection.Visible = true;
        }
        private void cmbdepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if SelectedItem is not null
            string selectedDepartment = cmbdepartment.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedDepartment))
            {
                // Reload sections based on selected department
                LoadSectionsByDepartment(selectedDepartment);

                // Reload data based on selected department and section
                string selectedSection = cmbsection.SelectedItem?.ToString();
                LoadData(selectedDepartment, selectedSection);
            }
            else
            {
                // Handle case where no valid department is selected, e.g., reload all data
                LoadData("All", "All");
            }
        }
        private void LoadSectionsByDepartment(string department)
        {
            try
            {
                conn.Open();

                // Load Sections based on selected department
                string sectionQuery = "SELECT DISTINCT section FROM student_table WHERE department = @department";
                cmd = new MySqlCommand(sectionQuery, conn);
                cmd.Parameters.AddWithValue("@department", department);
                MySqlDataReader reader = cmd.ExecuteReader();

                cmbsection.Items.Clear();
                cmbsection.Items.Add("All"); // Add default value "All"

                while (reader.Read())
                {
                    cmbsection.Items.Add(reader["section"].ToString());
                }
                reader.Close();

                // Set default selection to "All"
                cmbsection.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sections: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void cmbsection_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if SelectedItem is not null
            string selectedDepartment = cmbdepartment.SelectedItem?.ToString();
            string selectedSection = cmbsection.SelectedItem?.ToString();

            // Reload data based on selected department and section
            if (!string.IsNullOrEmpty(selectedDepartment) && !string.IsNullOrEmpty(selectedSection))
            {
                LoadData(selectedDepartment, selectedSection);
            }
            else
            {
                // Handle case where no valid selection is made, e.g., reload all data
                LoadData("All", "All");
            }
        }
        private void LoadComboBoxes()
        {
            try
            {
                conn.Open();

                // Load Departments into cmbdepartment
                string deptQuery = "SELECT DISTINCT department FROM student_table";
                cmd = new MySqlCommand(deptQuery, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                cmbdepartment.Items.Clear();
                cmbdepartment.Items.Add("All"); // Add default value "All"

                while (reader.Read())
                {
                    cmbdepartment.Items.Add(reader["department"].ToString());
                }
                reader.Close();

                // Load Sections into cmbsection
                string sectionQuery = "SELECT DISTINCT section FROM student_table";
                cmd = new MySqlCommand(sectionQuery, conn);
                reader = cmd.ExecuteReader();
                cmbsection.Items.Clear();
                cmbsection.Items.Add("All"); // Add default value "All"

                while (reader.Read())
                {
                    cmbsection.Items.Add(reader["section"].ToString());
                }
                reader.Close();

                // Set default selections
                cmbdepartment.SelectedIndex = 0;  // Default to "All"
                cmbsection.SelectedIndex = 0;     // Default to "All"
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading combo boxes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}



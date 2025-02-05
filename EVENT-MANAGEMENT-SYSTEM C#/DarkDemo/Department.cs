using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DarkDemo
{
    public partial class Department : Form, IThemeable
    {
        MySqlConnection conn;
        MySqlCommand cmd;
        MySqlDataReader dr;
        string conns = "server=localhost;port=3307;username=root;password=;database=events_management;";
        private string id, department, section;

        public Department()
        {
            InitializeComponent();
            conn = new MySqlConnection(conns);
        }
        public string ID
        {
            get { return id; }
            set { id = value; }
        }
        private void Department_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadDepartment();
            LoadSection();
        }
        public void LoadData()
        {
            int i = 0;
            guna2DataGridView1.Rows.Clear();
            cmd = new MySqlCommand("Select * from department_table order by department,section", conn);
            conn.Open();
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                i += 1;
                guna2DataGridView1.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString());
            }
            dr.Close();
            conn.Close();
        }
        private void btnadd_Click(object sender, EventArgs e)
        {
            DepartmentForm form = new DepartmentForm(this);
            form.btnUpdate.BackColor = Color.DarkGray;
            form.btnUpdate.Enabled = false;
            form.ShowDialog();
        }
        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = guna2DataGridView1.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {

                DepartmentForm d2 = new DepartmentForm(this);
                d2.btnSave.Enabled = false;
                d2.btnUpdate.Enabled = true;

                d2.txtdepartment.Text = department;
                d2.txtsection.Text = section;
                d2.ShowDialog();
            }
            else if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure you want to delete this Data?", "Delete Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    conn.Open();
                    cmd = new MySqlCommand($"delete from department_table where id = '{id}'", conn);
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        conn.Close();
                        MessageBox.Show("Data deleted successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong! Try again later!", "Form", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
        private void guna2DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            int i = guna2DataGridView1.CurrentRow.Index;
            id = guna2DataGridView1[1, i].Value.ToString();
            department = guna2DataGridView1[2, i].Value.ToString();
            section = guna2DataGridView1[3, i].Value.ToString();
        }

        //Seach feauture and filtering
        public void SearchData(string data)
        {
            int i = 0;
            guna2DataGridView1.Rows.Clear();

            string query = "SELECT * FROM department_table WHERE CONCAT(department, section) LIKE @search";
            bool hasDepartmentFilter = cmbdepartment.SelectedIndex != -1 && cmbdepartment.Text != "All";
            bool hasSectionFilter = cmbsection.SelectedIndex != -1 && cmbsection.Text != "All";

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
                cmd.Parameters.AddWithValue("@department", cmbdepartment.Text);
            }
            if (hasSectionFilter)
            {
                cmd.Parameters.AddWithValue("@section", cmbsection.Text);
            }

            conn.Open();
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                i += 1;
                guna2DataGridView1.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString());
            }
            dr.Close();
            conn.Close();
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchData(txtSearch.Text);
        }
        //****//
        private void cmb_Click(object sender, EventArgs e)
        {
            if (sender == cmbdepartment)
                lbldepartment.Visible = false;
            else if (sender == cmbsection)
                lblsection.Visible = false;
        }

        private void cmb_SelectionChangeCommitted(object sender, EventArgs e)
        {

        }

        private void cmb_Leave(object sender, EventArgs e)
        {
            if (sender == cmbdepartment && cmbdepartment.SelectedIndex == -1)
                lbldepartment.Visible = true;
            else if (sender == cmbsection && cmbsection.SelectedIndex == -1)
                lblsection.Visible = true;
        }
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
            btnadd.ForeColor = IsDarkMode ? Color.FromArgb(41, 44, 51) : Color.Gainsboro;
            lbldepartment.ForeColor = IsDarkMode ? Color.FromArgb(41, 44, 51) : Color.Gainsboro;
            lblsection.ForeColor = IsDarkMode ? Color.FromArgb(41, 44, 51) : Color.Gainsboro;

            cmbdepartment.FillColor = IsDarkMode ? Color.WhiteSmoke : Color.FromArgb(41, 44, 51);
            cmbsection.FillColor = IsDarkMode ? Color.WhiteSmoke : Color.FromArgb(41, 44, 51);
            btnadd.FillColor = IsDarkMode ? Color.WhiteSmoke : Color.FromArgb(41, 44, 51);
            lbldepartment.BackColor = IsDarkMode ? Color.WhiteSmoke : Color.FromArgb(41, 44, 51);
            lblsection.BackColor = IsDarkMode ? Color.WhiteSmoke : Color.FromArgb(41, 44, 51);

            cmbdepartment.BackColor = IsDarkMode ? Color.Gainsboro : Color.FromArgb(36, 39, 45);
            cmbsection.BackColor = IsDarkMode ? Color.Gainsboro : Color.FromArgb(36, 39, 45);
        }

        private void LoadDepartment()
        {
            using (MySqlConnection userConn = new MySqlConnection(conns))
            {
                try
                {
                    userConn.Open();
                    string query = "SELECT DISTINCT department FROM department_table";
                    MySqlCommand cmd = new MySqlCommand(query, userConn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    cmbdepartment.Items.Clear();
                    cmbdepartment.Items.Add("All"); // Add the "All" option

                    while (reader.Read())
                    {
                        cmbdepartment.Items.Add(reader["department"].ToString());
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading Department: " + ex.Message);
                }
            }
        }
        private void LoadSection()
        {
            using (MySqlConnection userConn = new MySqlConnection(conns))
            {
                try
                {
                    userConn.Open();
                    string query = "SELECT DISTINCT section FROM department_table";
                    MySqlCommand cmd = new MySqlCommand(query, userConn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    cmbsection.Items.Clear();
                    cmbsection.Items.Add("All"); // Add the "All" option

                    while (reader.Read())
                    {
                        cmbsection.Items.Add(reader["section"].ToString());
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading Section: " + ex.Message);
                }
            }
        }
        private void cmbdepartment_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmbdepartment.Text == "All")
            {
                // Load all sections when "All" is selected
                LoadSection();
            }
            else
            {
                // Load sections specific to the selected department
                LoadSectionForDepartment(cmbdepartment.Text);
            }

            // Apply the filter immediately
            SearchData(txtSearch.Text);
        }

        private void cmbsection_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SearchData(txtSearch.Text); // Apply the filter immediately
        }
        private void LoadSectionForDepartment(string department)
        {
            cmbsection.Items.Clear();
            cmbsection.Items.Add("All"); // Add the "All" option for sections

            using (MySqlConnection userConn = new MySqlConnection(conns))
            {
                try
                {
                    userConn.Open();
                    string query = "SELECT DISTINCT section FROM department_table WHERE department = @department";
                    MySqlCommand cmd = new MySqlCommand(query, userConn);
                    cmd.Parameters.AddWithValue("@department", department);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        cmbsection.Items.Add(reader["section"].ToString());
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading sections for department: " + ex.Message);
                }
            }
        }
    }
}

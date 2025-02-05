
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace DarkDemo
{
    public partial class DepartmentForm : Form
    {
        MySqlConnection conn;
        MySqlCommand cmd;
        Department d1;

        string conns = "server=localhost;port=3307;username=root;password=;database=events_management;";
        public DepartmentForm(Department D1)
        {
            InitializeComponent();
            d1= D1;
            conn = new MySqlConnection(conns);
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if ((txtdepartment.Text == string.Empty) || (txtsection.Text == string.Empty))
                {
                    MessageBox.Show("Warning: Missing field required!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                conn.Open();
                cmd = new MySqlCommand("INSERT INTO department_table(department,section) VALUES('" + txtdepartment.Text + "','" + txtsection.Text + "')", conn);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    conn.Close();
                    MessageBox.Show("Data successfully saved!", "Form", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    d1.LoadData();
                    ClearForm();
                    this.Dispose();
                }
                else
                {
                    MessageBox.Show("Checked the data you enter!", "Form", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show("Warning: " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if ((txtdepartment.Text == string.Empty) || (txtsection.Text == string.Empty))
                {
                    MessageBox.Show("Warning: Missing field required!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                conn.Open();
                cmd = new MySqlCommand("update department_table set department='" + txtdepartment.Text + "',section='" + txtsection.Text + "' where id = '" + d1.ID + "'", conn);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    conn.Close();
                    MessageBox.Show("Data updated successfully!", "Form", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    d1.LoadData();
                    this.Dispose();
                }
                else
                {
                    MessageBox.Show("Something went wrong! Try again later!", "Form", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show("Warning: " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void ClearForm()
        {
            txtdepartment.Clear();
            txtsection.Clear();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
    }
}

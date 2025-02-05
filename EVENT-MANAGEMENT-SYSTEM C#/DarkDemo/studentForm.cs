
using DarkDemo.DashboardCon;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DarkDemo
{
    public partial class studentForm : Form
    {
        dashBoardConnections d = new dashBoardConnections();
        MySqlConnection conn;
        MySqlCommand cmd;
        student s1;
        private string imagePath;

        string conns = "server=localhost;port=3307;username=root;password=;database=events_management;";
        public studentForm(student S1)
        {
            InitializeComponent();
            s1 = S1;
            conn = new MySqlConnection(conns);
        }
        private void studentForm_Load(object sender, EventArgs e)
        {
            LoadDepartment();
            txtdepartment.SelectedIndexChanged += txtdepartment_SelectedIndexChanged; 
        }
        private byte[] ImageToByteArray(Image image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            using (MemoryStream ms = new MemoryStream())
            {
                // Save the image as a PNG format for compatibility
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }
        //private bool IsValidImageFormat(Image img)
        //{
        //    try
        //    {
        //        // Test if the image can be saved in a common format (e.g., PNG)
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        //        }
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (pictureBoxDP.Image == null)
            {
                MessageBox.Show("Please select a profile image.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //if (!IsValidImageFormat(pictureBoxDP.Image))
            //{
            //    MessageBox.Show("Invalid image format. Please select a valid image.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            byte[] imageBytes;
            try
            {
                imageBytes = ImageToByteArray(pictureBoxDP.Image); // Convert image to byte array
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing the image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(txtsid.Text) || string.IsNullOrWhiteSpace(txtname.Text) ||
                    string.IsNullOrWhiteSpace(txtno.Text) || string.IsNullOrWhiteSpace(txtaddress.Text) ||
                    string.IsNullOrWhiteSpace(txtdepartment.Text) || string.IsNullOrWhiteSpace(txtsection.Text))
                {
                    MessageBox.Show("Please fill out all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                conn.Open();
                cmd = new MySqlCommand("INSERT INTO student_table(studentid, studentname, contactno, address, department, section, dp) VALUES(@sid, @name, @no, @address, @department, @section, @dp)", conn);

                cmd.Parameters.AddWithValue("@sid", txtsid.Text);
                cmd.Parameters.AddWithValue("@name", txtname.Text);
                cmd.Parameters.AddWithValue("@no", txtno.Text);
                cmd.Parameters.AddWithValue("@address", txtaddress.Text);
                cmd.Parameters.AddWithValue("@department", txtdepartment.Text);
                cmd.Parameters.AddWithValue("@section", txtsection.Text);
                cmd.Parameters.AddWithValue("@dp", imageBytes);

                if (cmd.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("Data successfully saved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    d.PopulateReportTable();
                    s1.LoadData();
                    ClearForm();
                    this.Dispose();
                }
                else
                {
                    MessageBox.Show("Failed to save data. Please check the entered information.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (pictureBoxDP.Image == null)
            {
                MessageBox.Show("Please select a profile image.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //if (!IsValidImageFormat(pictureBoxDP.Image))
            //{
            //    MessageBox.Show("Invalid image format. Please select a valid image.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            byte[] imageBytes;
            try
            {
                imageBytes = ImageToByteArray(pictureBoxDP.Image); // Convert image to byte array
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing the image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(txtsid.Text) || string.IsNullOrWhiteSpace(txtname.Text) ||
                    string.IsNullOrWhiteSpace(txtno.Text) || string.IsNullOrWhiteSpace(txtaddress.Text) ||
                    string.IsNullOrWhiteSpace(txtdepartment.Text) || string.IsNullOrWhiteSpace(txtsection.Text))
                {
                    MessageBox.Show("Please fill out all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                conn.Open();
                MySqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    cmd = new MySqlCommand("UPDATE student_table SET studentid=@sid, studentname=@name, contactno=@no, address=@address, department=@department, section=@section, dp=@dp WHERE id=@id", conn, transaction);
                    cmd.Parameters.AddWithValue("@sid", txtsid.Text);
                    cmd.Parameters.AddWithValue("@name", txtname.Text);
                    cmd.Parameters.AddWithValue("@no", txtno.Text);
                    cmd.Parameters.AddWithValue("@address", txtaddress.Text);
                    cmd.Parameters.AddWithValue("@department", txtdepartment.Text);
                    cmd.Parameters.AddWithValue("@section", txtsection.Text);
                    cmd.Parameters.AddWithValue("@dp", imageBytes);
                    cmd.Parameters.AddWithValue("@id", s1.ID);

                    if (cmd.ExecuteNonQuery() <= 0)
                        throw new Exception("Failed to update student data.");

                    cmd = new MySqlCommand("UPDATE report_table SET studentname=@name, section=@section WHERE studentid=@sid", conn, transaction);
                    cmd.Parameters.AddWithValue("@sid", txtsid.Text);
                    cmd.Parameters.AddWithValue("@name", txtname.Text);
                    cmd.Parameters.AddWithValue("@section", txtsection.Text);

                    if (cmd.ExecuteNonQuery() <= 0)
                        throw new Exception("Failed to update report data.");

                    transaction.Commit();
                    MessageBox.Show("Data updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    d.PopulateReportTable();
                    s1.LoadData();
                    ClearForm();
                    this.Dispose();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtsid.Clear();
            txtname.Clear();
            txtno.Clear();
            txtaddress.Clear();
            txtdepartment.Text =string.Empty;
            txtsection.Text = string.Empty;
            pictureBoxDP.Image = null;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void LoadDepartment()
        {
            using (MySqlConnection userConn = new MySqlConnection(conns))
            {
                try
                {
                    userConn.Open();
                    string query = "SELECT DISTINCT department FROM department_table"; // Fetch all unique departments

                    MySqlCommand cmd = new MySqlCommand(query, userConn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string department = reader["department"].ToString();

                        if (!txtdepartment.Items.Contains(department))
                        {
                            txtdepartment.Items.Add(department); // Add each unique department
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading Departments: " + ex.Message);
                }
            }
        }
        private void LoadSection(string department)
        {
            txtsection.Items.Clear(); // Clear previous sections

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
                        string section = reader["section"].ToString();
                        if (!txtsection.Items.Contains(section))
                        {
                            txtsection.Items.Add(section); // Add each unique section for the selected department
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading Sections: " + ex.Message);
                }
            }
        }
        private void txtdepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (txtdepartment.SelectedItem != null)
            {
                string selectedDepartment = txtdepartment.SelectedItem.ToString();
                LoadSection(selectedDepartment); // Load sections for the selected department
            }
        }

        private void btnbrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            openFileDialog.Title = "Select Profile Picture";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Display the image in the PictureBox
                pictureBoxDP.Image = new Bitmap(openFileDialog.FileName);
                imagePath = openFileDialog.FileName;

            }
        }

        private void txtsection_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lbl_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void txtaddress_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtno_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtname_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtsid_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBoxDP_Click(object sender, EventArgs e)
        {

        }
    }
}

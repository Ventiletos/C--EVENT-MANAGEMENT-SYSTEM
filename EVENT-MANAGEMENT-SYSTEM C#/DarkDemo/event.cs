using Bunifu.UI.WinForms;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MySql.Data.MySqlClient;
using System.Data.Common;
using Mysqlx.Crud;
using System.Transactions;

namespace DarkDemo
{
    public partial class @event : Form, IThemeable
    {
        MySqlConnection conn;
        MySqlCommand cmd;
        MySqlDataReader dr;
        int i = 0;
        string conns = "server=localhost;port=3307;username=root;password=;database=events_management;";
        private string id,eid,ename,eveneu,ecategory,start,end,fines;

        public @event()
        {
            InitializeComponent();
            conn = new MySqlConnection(conns);
            //LoadData();
            SearchData("");
            form.Hide();
        }
        public string ID
        {
            get { return id; }
            set { id = value; }
        }
       
        private void event_Load(object sender, EventArgs e)
        {
            dateformat();
            LoadData();
            eventitemfilter();
        }
        private void btnadd_Click(object sender, EventArgs e)
        {
            form.Visible=true;
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnUpdate.BackColor = Color.Gray;
            btnadd2.Visible = true;
            btnadd.Visible = false;
            txteid.Text=GenerateRandomEventID();
            
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string startDateTime = date.Value.ToString("yyyy-MM-dd HH:mm:ss");
                string endDateTime = date1.Value.ToString("yyyy-MM-dd HH:mm:ss");

                if (string.IsNullOrEmpty(txteid.Text) || string.IsNullOrEmpty(txtename.Text) ||
                    string.IsNullOrEmpty(txtveneu.Text) || string.IsNullOrEmpty(txtcategory.Text) ||
                    string.IsNullOrEmpty(txtfines.Text))
                {
                    MessageBox.Show("Warning: Missing field required!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (conn.State != ConnectionState.Open) conn.Open(); // Check connection state

                cmd = new MySqlCommand(
                    "INSERT INTO event_table(eventid, eventname, eventvenue, eventcategory, eventstart, eventend, fines) " +
                    "VALUES(@EventId, @EventName, @EventVenue, @EventCategory, @EventStart, @EventEnd, @Fines)", conn);

                cmd.Parameters.AddWithValue("@EventId", txteid.Text);
                cmd.Parameters.AddWithValue("@EventName", txtename.Text);
                cmd.Parameters.AddWithValue("@EventVenue", txtveneu.Text);
                cmd.Parameters.AddWithValue("@EventCategory", txtcategory.Text);
                cmd.Parameters.AddWithValue("@EventStart", startDateTime);
                cmd.Parameters.AddWithValue("@EventEnd", endDateTime);
                cmd.Parameters.AddWithValue("@Fines", txtfines.Text);

                if (cmd.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("Data successfully saved!", "Form", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ClearForm();
                    form.Hide();
                }
                else
                {
                    MessageBox.Show("Please check the data you entered!", "Form", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Warning: " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close(); // Ensure connection is closed
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                string startDateTime = date.Value.ToString("yyyy-MM-dd HH:mm:ss");
                string endDateTime = date1.Value.ToString("yyyy-MM-dd HH:mm:ss");

                if (string.IsNullOrEmpty(txteid.Text) || string.IsNullOrEmpty(txtename.Text) ||
                    string.IsNullOrEmpty(txtveneu.Text) || string.IsNullOrEmpty(txtcategory.Text) ||
                    string.IsNullOrEmpty(txtfines.Text))
                {
                    MessageBox.Show("Warning: Missing field required!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (conn.State != ConnectionState.Open) conn.Open(); // Check connection state

                cmd = new MySqlCommand(
                    "UPDATE event_table " +
                    "SET eventname = @EventName, eventvenue = @EventVenue, eventcategory = @EventCategory, " +
                    "eventstart = @EventStart, eventend = @EventEnd, fines = @Fines " +
                    "WHERE eventid = @EventId", conn);

                cmd.Parameters.AddWithValue("@EventId", txteid.Text);
                cmd.Parameters.AddWithValue("@EventName", txtename.Text);
                cmd.Parameters.AddWithValue("@EventVenue", txtveneu.Text);
                cmd.Parameters.AddWithValue("@EventCategory", txtcategory.Text);
                cmd.Parameters.AddWithValue("@EventStart", startDateTime);
                cmd.Parameters.AddWithValue("@EventEnd", endDateTime);
                cmd.Parameters.AddWithValue("@Fines", txtfines.Text);

                if (cmd.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("Data successfully updated!", "Form", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ClearForm();
                    form.Hide();
                }
                else
                {
                    MessageBox.Show("No changes were made. Please check the data you entered.", "Form", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Warning: " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close(); // Ensure connection is closed
            }
        }
        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = guna2DataGridView1.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {

                btnSave.Enabled = false;
                btnUpdate.Enabled = true;

                txteid.Text = eid;
                txtename.Text = ename;
                txtveneu.Text = eveneu;
                txtcategory.Text = ecategory;
                date.Value = DateTime.Parse(start);
                date1.Value = DateTime.Parse(end);
                txtfines.Text = fines;
                form.Show();
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

                        // Delete related records from report_table based on eventid
                        string deleteReportQuery = "DELETE FROM report_table WHERE eventid = @EventId";
                        cmd = new MySqlCommand(deleteReportQuery, conn, transaction);
                        cmd.Parameters.AddWithValue("@EventId", eid);
                        cmd.ExecuteNonQuery();

                        // Delete record from event_table
                        string deleteEventQuery = "DELETE FROM event_table WHERE id = @EventId";
                        cmd = new MySqlCommand(deleteEventQuery, conn, transaction);
                        cmd.Parameters.AddWithValue("@EventId", id);
                        cmd.ExecuteNonQuery();

                        // Commit transaction
                        transaction.Commit();
                        MessageBox.Show("Data deleted successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Refresh the DataGridView after closing the current connection
                        conn.Close(); // Close the connection to avoid conflicts while loading data

                        // Reload data in a new connection
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction in case of an error
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
            id = guna2DataGridView1[1, i].Value.ToString();
            eid = guna2DataGridView1[2, i].Value.ToString();
            ename = guna2DataGridView1[3, i].Value.ToString();
            eveneu = guna2DataGridView1[4, i].Value.ToString();
            ecategory = guna2DataGridView1[5, i].Value.ToString();
            start = guna2DataGridView1[6, i].Value.ToString();
            end = guna2DataGridView1[7, i].Value.ToString();
            fines = guna2DataGridView1[8, i].Value.ToString();
        }
        public void LoadData()
        {
            try
            {
                int i = 0;
                string query = "SELECT * FROM event_table ORDER BY eventstart, eventend"; // Order by start date for better clarity
                guna2DataGridView1.Rows.Clear();

                if (conn.State != ConnectionState.Open) // Ensure the connection is only opened if not already open
                {
                    conn.Open();
                }

                cmd = new MySqlCommand(query, conn);
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    i += 1;
                    guna2DataGridView1.Rows.Add(
                        i,
                        dr[0].ToString(),
                        dr[1].ToString(),
                        dr[2].ToString(),
                        dr[3].ToString(),
                        dr[4].ToString(),
                        dr[5].ToString(),
                        dr[6].ToString(),
                        dr[7].ToString()
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dr?.Close(); // Ensure the DataReader is closed
                if (conn.State == ConnectionState.Open) conn.Close(); // Close connection if open
            }
        }
        private void btnadd2_Click(object sender, EventArgs e)
        {
            form.Visible = true;
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            btnUpdate.BackColor = Color.Gray;
            btnadd2.Visible = true;
            btnadd.Visible = false;
        }

        public void SearchData(string data)
        {
            int i = 0;
            guna2DataGridView1.Rows.Clear();
            cmd = new MySqlCommand("Select * from event_table where Concat(eventid,eventname,eventcategory) like '%" + data + "%'", conn);
            conn.Open();
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                i += 1;
                guna2DataGridView1.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), dr[7].ToString());
            }
            dr.Close();
            conn.Close();
        }
        private void ClearForm()
        {
            txtename.Clear();
            txtveneu.Clear();
            txtcategory.Text = string.Empty;
            date.Value = DateTime.Now;
            date1.Value = DateTime.Now;
            txtfines.Clear();

            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchData(txtSearch.Text);
        }
        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            form.Visible=false;
           
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
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

            cmbeventfilter.ForeColor = IsDarkMode ? Color.FromArgb(41, 44, 51) : Color.Gainsboro;
           
            btnadd.ForeColor = IsDarkMode ? Color.FromArgb(41, 44, 51) : Color.Gainsboro;

            cmbeventfilter.FillColor = IsDarkMode ? Color.WhiteSmoke : Color.FromArgb(41, 44, 51);
          
            btnadd.FillColor = IsDarkMode ? Color.WhiteSmoke : Color.FromArgb(41, 44, 51);
        }
        private string GenerateRandomEventID()
        {

            Random random = new Random();
            return random.Next(10000, 100000).ToString(); // Generates a 5-digit number between 10000 and 99999
        }
        private void cmbeventfilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterEvents();
        }
        public void FilterEvents()
        {
            int i = 0;
            string filter = txtSearch.Text;  // Default filter text from search box
            string filterQuery = "Select * from event_table where Concat(eventid,eventname,eventcategory) like '%" + filter + "%'";

            DateTime currentDate = DateTime.Now;

            // Filter events based on selected option in cmbeventfilter
            if (cmbeventfilter.SelectedIndex == 0) // All
            {
                // No additional conditions needed
            }
            else if (cmbeventfilter.SelectedIndex == 1) // Ongoing
            {
                // Ongoing events are between the start and end date
                filterQuery += " AND eventstart <= '" + currentDate.ToString("yyyy-MM-dd HH:mm:ss") + "' AND eventend >= '" + currentDate.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }
            else if (cmbeventfilter.SelectedIndex == 2) // Upcoming
            {
                // Upcoming events are after the current date
                filterQuery += " AND eventstart > '" + currentDate.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }
            else if (cmbeventfilter.SelectedIndex == 3) // Ended
            {
                // Ended events are those that have finished (start time is before current date and end time is also before current date)
                filterQuery += " AND eventend < '" + currentDate.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }

            // Apply the filter query to the database and load the data
            guna2DataGridView1.Rows.Clear();
            cmd = new MySqlCommand(filterQuery, conn);
            conn.Open();
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                i += 1;
                guna2DataGridView1.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), dr[7].ToString());
            }
            dr.Close();
            conn.Close();
        }

        public void eventitemfilter()
        {
            cmbeventfilter.Items.Add("All");
            cmbeventfilter.Items.Add("Ongoing");
            cmbeventfilter.Items.Add("Upcoming");
            cmbeventfilter.Items.Add("Ended");
            cmbeventfilter.SelectedIndex = 0; // Select the first item by default

            cmbeventfilter.SelectedIndexChanged += cmbeventfilter_SelectedIndexChanged;
        }
        public void dateformat()
        {
            date.Format = DateTimePickerFormat.Custom;
            date.CustomFormat = "MM/dd/yyyy hh:mm tt"; // Includes time in 12-hour format with AM/PM
            date1.Format = DateTimePickerFormat.Custom;
            date1.CustomFormat = "MM/dd/yyyy hh:mm tt";
        }
    }

}



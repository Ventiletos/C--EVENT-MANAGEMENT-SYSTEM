using DarkDemo.DashboardCon;
using DarkDemo.SqlCon;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DarkDemo
{
    public partial class Reports : Form, IThemeable
    {
        private MySqlConnection conn;
        private MySqlCommand cmd;
        private MySqlDataReader dr;
        private string connectionString = "server=localhost;port=3307;username=root;password=;database=events_management;";

        public Reports()
        {
            InitializeComponent();
            LoadReportTable();
            ResetAutoIncrement();
        }
        private void Reports_Load(object sender, EventArgs e)
        {
            dashBoardConnections d = new dashBoardConnections();
            d.PopulateReportTable();
            LoadReportTable();
            PopulateSectionComboBox();

            // Populate event filter combo box with options
            cmbeventfilter.Items.Add("All");
            cmbeventfilter.Items.Add("Ongoing");
            cmbeventfilter.Items.Add("Ended");
            cmbeventfilter.Items.Add("Upcoming");
            datetimenow.Value = DateTime.Now;
            // Set the default selection
            cmbeventfilter.SelectedIndex = 0; // Select "All" by default
        }
        public void LoadReportTable()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Prepare the SQL query with filters
                string query = @"
            SELECT r.id, e.eventstart, e.eventend, r.event, r.studentid, r.studentname, r.section, r.date, 
                   r.loginam, r.logoutam, r.loginpm, r.logoutpm, r.fines
            FROM report_table r
            JOIN event_table e ON r.event = e.eventname
            WHERE 1 = 1";

                // Filter by section if selected
                if (!string.IsNullOrEmpty(cmbsection.SelectedItem?.ToString()) && cmbsection.SelectedItem.ToString() != "All")
                {
                    query += " AND r.section = @Section";
                }

                // Filter by event status
                string eventStatusFilter = cmbeventfilter.SelectedItem?.ToString();
                if (eventStatusFilter == "Ongoing")
                {
                    query += " AND e.eventstart <= @CurrentDate AND e.eventend >= @CurrentDate";
                }
                else if (eventStatusFilter == "Ended")
                {
                    query += " AND e.eventend < @CurrentDate";
                }
                else if (eventStatusFilter == "Upcoming")
                {
                    query += " AND e.eventstart > @CurrentDate";
                }

                // Filter by date (if needed)
                if (datetimenow.Value.Date != DateTime.Now.Date)
                {
                    query += " AND r.date = @Date";
                }

                // Adding search filter for student name/ID if any text is entered
                string searchText = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(searchText))
                {
                    query += " AND (r.studentname LIKE @SearchText OR r.studentid LIKE @SearchText)";
                }

                using (var command = new MySqlCommand(query, connection))
                {
                    // Add parameters
                    command.Parameters.AddWithValue("@CurrentDate", DateTime.Now);
                    command.Parameters.AddWithValue("@Date", datetimenow.Value.Date);

                    if (!string.IsNullOrEmpty(cmbsection.SelectedItem?.ToString()) && cmbsection.SelectedItem.ToString() != "All")
                    {
                        command.Parameters.AddWithValue("@Section", cmbsection.SelectedItem.ToString());
                    }

                    if (!string.IsNullOrEmpty(searchText))
                    {
                        command.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        guna2DataGridView1.Rows.Clear();

                        while (reader.Read())
                        {
                            string eventStart = reader["eventstart"] != DBNull.Value ? reader.GetDateTime("eventstart").ToString("yyyy-MM-dd") : string.Empty;
                            string eventEnd = reader["eventend"] != DBNull.Value ? reader.GetDateTime("eventend").ToString("yyyy-MM-dd") : string.Empty;

                            guna2DataGridView1.Rows.Add(
                                reader.GetInt32("id"),
                                eventStart,
                                eventEnd,
                                reader.GetString("event"),
                                reader.GetString("studentid"),
                                reader.GetString("studentname"),
                                reader.GetString("section"),
                                reader.GetDateTime("date").ToString("yyyy-MM-dd"),
                                reader.GetString("loginam"),
                                reader.GetString("logoutam"),
                                reader.GetString("loginpm"),
                                reader.GetString("logoutpm"),
                                reader.GetDecimal("fines")
                            );
                        }
                    }
                }

                // Calculate the total fines based on the same filters
                UpdateTotalFines(connection);
            }
        }
        private void UpdateTotalFines(MySqlConnection connection)
        {
            string query = @"
        SELECT SUM(r.fines) AS TotalFines
        FROM report_table r
        JOIN event_table e ON r.event = e.eventname
        WHERE 1 = 1";

            // Add the same filters as in LoadReportTable
            if (!string.IsNullOrEmpty(cmbsection.SelectedItem?.ToString()) && cmbsection.SelectedItem.ToString() != "All")
            {
                query += " AND r.section = @Section";
            }

            string eventStatusFilter = cmbeventfilter.SelectedItem?.ToString();
            if (eventStatusFilter == "Ongoing")
            {
                query += " AND e.eventstart <= @CurrentDate AND e.eventend >= @CurrentDate";
            }
            else if (eventStatusFilter == "Ended")
            {
                query += " AND e.eventend < @CurrentDate";
            }
            else if (eventStatusFilter == "Upcoming")
            {
                query += " AND e.eventstart > @CurrentDate";
            }

            if (datetimenow.Value.Date != DateTime.Now.Date)
            {
                query += " AND r.date = @Date";
            }

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CurrentDate", DateTime.Now);
                command.Parameters.AddWithValue("@Date", datetimenow.Value.Date);

                if (!string.IsNullOrEmpty(cmbsection.SelectedItem?.ToString()) && cmbsection.SelectedItem.ToString() != "All")
                {
                    command.Parameters.AddWithValue("@Section", cmbsection.SelectedItem.ToString());
                }

                object result = command.ExecuteScalar();
                txtTotalFines.Text = result != DBNull.Value ? Convert.ToDecimal(result).ToString("C") : "₱0.00";
            }
        }

        private void cmbeventfilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadReportTable(); // Reload the table when the event filter changes
        }

        // Event handler for cmbsection selection change
        private void cmbsection_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadReportTable(); // Reload the table when the section filter changes
        }

        private void datetimenow_ValueChanged(object sender, EventArgs e)
        {
            LoadReportTable(); // Reload the table when the date is changed
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadReportTable();
        }
        private void btnRview_Click(object sender, EventArgs e)
        {
            reportsprint r = new reportsprint();
            r.Show();
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

            datetimenow.BackColor = IsDarkMode ? Color.FromArgb(41, 44, 51) : Color.Gainsboro;


            datetimenow.BorderColor = IsDarkMode ? Color.Gainsboro : Color.FromArgb(41, 44, 51);
            cmbeventfilter.ForeColor = IsDarkMode ? Color.FromArgb(41, 44, 51) : Color.Gainsboro;
            cmbsection.ForeColor = IsDarkMode ? Color.FromArgb(41, 44, 51) : Color.Gainsboro;
            btnRview.ForeColor = IsDarkMode ? Color.FromArgb(41, 44, 51) : Color.Gainsboro;
            btnrefresh.ForeColor = IsDarkMode ? Color.FromArgb(41, 44, 51) : Color.Gainsboro;

            datetimenow.BackColor = IsDarkMode ? Color.Gainsboro : Color.FromArgb(41, 44, 51);

            cmbsection.FillColor = IsDarkMode ? Color.WhiteSmoke : Color.FromArgb(41, 44, 51);
            cmbeventfilter.FillColor = IsDarkMode ? Color.WhiteSmoke : Color.FromArgb(41, 44, 51);
            btnRview.FillColor = IsDarkMode ? Color.WhiteSmoke : Color.FromArgb(41, 44, 51);
            btnrefresh.FillColor = IsDarkMode ? Color.WhiteSmoke : Color.FromArgb(41, 44, 51);


            cmbsection.BackColor = IsDarkMode ? Color.Gainsboro : Color.FromArgb(36, 39, 45);
            cmbeventfilter.BackColor = IsDarkMode ? Color.Gainsboro : Color.FromArgb(36, 39, 45);
        }
        public void ResetAutoIncrement()
        {
            string connectionString = "server=localhost;port=3307;username=root;password=;database=events_management;";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string resetAutoIncrementQuery = "ALTER TABLE report_table AUTO_INCREMENT = 1;";

                using (var command = new MySqlCommand(resetAutoIncrementQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        private void PopulateSectionComboBox()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT DISTINCT section FROM student_table"; // Retrieve distinct sections
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    cmbsection.Items.Clear();
                    cmbsection.Items.Add("All"); // Add "All" option to show all sections
                    while (reader.Read())
                    {
                        cmbsection.Items.Add(reader.GetString("section"));
                    }
                }
            }
        }

        private void btnrefresh_Click(object sender, EventArgs e)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string deleteAllQuery = "DELETE FROM report_table";
                    using (var command = new MySqlCommand(deleteAllQuery, connection))
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        //MessageBox.Show($"Deleted {rowsAffected} rows from report_table.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    ResetAutoIncrement();
                    LoadReportTable(); // Reload the table to reflect changes in the UI
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while deleting data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private void InsertOrUpdateIntoReportTable(MySqlConnection connection, int eventid, string eventname, string studentid, string studentname, string section, DateTime eventstart, DateTime eventend, decimal eventfine, byte[] dp, DateTime date)
        //{
        //    // Query to check if the record already exists based on eventid and studentid
        //    string checkExistenceQuery = @"
        //SELECT COUNT(*) FROM report_table
        //WHERE eventid = @EventID AND studentid = @StudentID";

        //    using (var checkCommand = new MySqlCommand(checkExistenceQuery, connection))
        //    {
        //        checkCommand.Parameters.AddWithValue("@EventID", eventid);
        //        checkCommand.Parameters.AddWithValue("@StudentID", studentid);

        //        // Check if the record exists
        //        int count = Convert.ToInt32(checkCommand.ExecuteScalar());

        //        if (count == 0)
        //        {
        //            // If record doesn't exist, insert it with eventid and fines
        //            string insertQuery = @"
        //        INSERT INTO report_table (eventid, event, studentid, studentname, section, eventstart, eventend, date, dp, eventfine)
        //        VALUES (@EventID, @Event, @StudentID, @StudentName, @Section, @EventStart, @EventEnd, @Date, @DP, @EventFine);";

        //            try
        //            {
        //                using (var insertCommand = new MySqlCommand(insertQuery, connection))
        //                {
        //                    insertCommand.Parameters.AddWithValue("@EventID", eventid);
        //                    insertCommand.Parameters.AddWithValue("@Event", eventname);
        //                    insertCommand.Parameters.AddWithValue("@StudentID", studentid);
        //                    insertCommand.Parameters.AddWithValue("@StudentName", studentname);
        //                    insertCommand.Parameters.AddWithValue("@Section", section);
        //                    insertCommand.Parameters.AddWithValue("@EventStart", eventstart);
        //                    insertCommand.Parameters.AddWithValue("@EventEnd", eventend);
        //                    insertCommand.Parameters.AddWithValue("@Date", date);
        //                    insertCommand.Parameters.AddWithValue("@DP", dp ?? (object)DBNull.Value);
        //                    insertCommand.Parameters.AddWithValue("@EventFine", eventfine);

        //                    insertCommand.ExecuteNonQuery();
        //                    Console.WriteLine($"Inserted record: {eventname}, {studentname}, Fine: {eventfine}, DP Exists: {dp != null}");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show($"Error inserting data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            }
        //        }
        //        else
        //        {
        //            // If record exists, update it with the latest data, keeping the eventid unique
        //            string updateQuery = @"
        //        UPDATE report_table
        //        SET eventid = @EventID,
        //            event = @Event,
        //            studentid = @StudentID,
        //            studentname = @StudentName,
        //            section = @Section,
        //            eventstart = @EventStart,
        //            eventend = @EventEnd,
        //            date = @Date,
        //            dp = @DP,
        //            eventfine = @EventFine
        //        WHERE eventid = @EventID AND studentid = @StudentID";

        //            try
        //            {
        //                using (var updateCommand = new MySqlCommand(updateQuery, connection))
        //                {
        //                    updateCommand.Parameters.AddWithValue("@EventID", eventid);
        //                    updateCommand.Parameters.AddWithValue("@Event", eventname);
        //                    updateCommand.Parameters.AddWithValue("@StudentID", studentid);
        //                    updateCommand.Parameters.AddWithValue("@StudentName", studentname);
        //                    updateCommand.Parameters.AddWithValue("@Section", section);
        //                    updateCommand.Parameters.AddWithValue("@EventStart", eventstart);
        //                    updateCommand.Parameters.AddWithValue("@EventEnd", eventend);
        //                    updateCommand.Parameters.AddWithValue("@Date", date);
        //                    updateCommand.Parameters.AddWithValue("@DP", dp ?? (object)DBNull.Value);
        //                    updateCommand.Parameters.AddWithValue("@EventFine", eventfine);

        //                    updateCommand.ExecuteNonQuery();
        //                    Console.WriteLine($"Updated record: {eventname}, {studentname}, Fine: {eventfine}, DP Exists: {dp != null}");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show($"Error updating data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            }
        //        }
        //    }
        //}

    }
}






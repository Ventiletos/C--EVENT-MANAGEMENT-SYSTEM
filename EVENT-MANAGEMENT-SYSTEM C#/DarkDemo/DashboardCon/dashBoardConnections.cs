using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DarkDemo.DashboardCon
{
    internal class dashBoardConnections
    {
        string connectionString = "server=localhost;port=3307;username=root;password=;database=events_management;";
        public int GetEventCount()
        {
            int eventCount = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM event_table";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    eventCount = Convert.ToInt32(command.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            return eventCount;
        }

        public int GetTotalDepartments()
        {
            int departmentCount = 0;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(DISTINCT department) FROM department_table";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    departmentCount = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            return departmentCount;
        }
        public int GetTotalId()
        {
            int studentidCount = 0;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(DISTINCT studentid) FROM student_table";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    studentidCount = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            return studentidCount;
        }
        public decimal GetTotalFines()
        {
            decimal totalFines = 0; // Use decimal to handle precise calculations
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT SUM(fines) FROM report_table";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    // Execute the query and handle null values
                    var result = cmd.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        totalFines = Convert.ToDecimal(result);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            return totalFines;
        }
        public void PopulateReportTable()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                var dataToInsert = new List<(int eventid, string eventname, string studentid, string studentname, string section, DateTime eventstart, DateTime eventend, decimal eventfine, byte[] dp)>(); // Modify the tuple to include eventid

                var selectQuery = @"
        SELECT e.eventid, e.eventname, e.fines AS eventfine, s.studentid, s.studentname, s.section, e.eventstart, e.eventend, s.dp
        FROM event_table e
        JOIN student_table s ON 1=1;"; // Assuming a join with no specific condition, replace with proper condition if needed

                using (var selectCommand = new MySqlCommand(selectQuery, connection))
                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int eventid = reader.GetInt32("eventid");  // Get the eventid
                        string eventname = reader.GetString("eventname");
                        decimal eventfine = reader.GetDecimal("eventfine");
                        string studentid = reader.GetString("studentid");
                        string studentname = reader.GetString("studentname");
                        string section = reader.GetString("section");
                        DateTime eventstart = reader.GetDateTime("eventstart");
                        DateTime eventend = reader.GetDateTime("eventend");
                        byte[] dp = reader["dp"] as byte[];  // Retrieve the image in byte array form

                        // Debugging log to check values before inserting
                        Console.WriteLine($"EventID: {eventid}, Event: {eventname}, Fine: {eventfine}, Student: {studentname}, DP exists: {dp != null}");

                        dataToInsert.Add((eventid, eventname, studentid, studentname, section, eventstart, eventend, eventfine, dp));
                    }
                }

                DateTime currentDate = DateTime.Now;
                foreach (var data in dataToInsert)
                {
                    // Insert or update with fines value and eventid
                    InsertOrUpdateIntoReportTable(connection, data.eventid, data.eventname, data.studentid, data.studentname, data.section, data.eventstart, data.eventend, data.eventfine, data.dp, currentDate);
                }
            }
        }
        private void InsertOrUpdateIntoReportTable(MySqlConnection connection, int eventid, string eventname, string studentid, string studentname, string section, DateTime eventstart, DateTime eventend, decimal eventfine, byte[] dp, DateTime date)
        {
            // Query to check if the record already exists
            string checkExistenceQuery = @"
    SELECT COUNT(*) FROM report_table
    WHERE eventid = @EventID AND studentid = @StudentID AND studentname = @StudentName";

            using (var checkCommand = new MySqlCommand(checkExistenceQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@EventID", eventid);
                checkCommand.Parameters.AddWithValue("@StudentID", studentid);
                checkCommand.Parameters.AddWithValue("@StudentName", studentname);

                // Check if record exists
                int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (count == 0)
                {
                    // If record doesn't exist, insert it with eventid and fines
                    string insertQuery = @"
            INSERT INTO report_table (eventid, event, studentid, studentname, section, eventstart, eventend, date, dp, eventfine)
            VALUES (@EventID, @Event, @StudentID, @StudentName, @Section, @EventStart, @EventEnd, @Date, @DP, @EventFine);";

                    try
                    {
                        using (var insertCommand = new MySqlCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@EventID", eventid);
                            insertCommand.Parameters.AddWithValue("@Event", eventname);
                            insertCommand.Parameters.AddWithValue("@StudentID", studentid);
                            insertCommand.Parameters.AddWithValue("@StudentName", studentname);
                            insertCommand.Parameters.AddWithValue("@Section", section);
                            insertCommand.Parameters.AddWithValue("@EventStart", eventstart);
                            insertCommand.Parameters.AddWithValue("@EventEnd", eventend);
                            insertCommand.Parameters.AddWithValue("@Date", date);
                            insertCommand.Parameters.AddWithValue("@DP", dp ?? (object)DBNull.Value);
                            insertCommand.Parameters.AddWithValue("@EventFine", eventfine);

                            insertCommand.ExecuteNonQuery();
                            Console.WriteLine($"Inserted record: {eventname}, {studentname}, Fine: {eventfine}, DP Exists: {dp != null}");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error inserting data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // If record exists, update it with eventid and fines
                    string updateQuery = @"
            UPDATE report_table
            SET eventid = @EventID,
                event = @Event,
                studentid = @StudentID,
                studentname = @StudentName,
                section = @Section,
                eventstart = @EventStart,
                eventend = @EventEnd,
                date = @Date,
                dp = @DP,
                eventfine = @EventFine
            WHERE eventid = @EventID AND studentid = @StudentID AND studentname = @StudentName;";

                    try
                    {
                        using (var updateCommand = new MySqlCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@EventID", eventid);
                            updateCommand.Parameters.AddWithValue("@Event", eventname);
                            updateCommand.Parameters.AddWithValue("@StudentID", studentid);
                            updateCommand.Parameters.AddWithValue("@StudentName", studentname);
                            updateCommand.Parameters.AddWithValue("@Section", section);
                            updateCommand.Parameters.AddWithValue("@EventStart", eventstart);
                            updateCommand.Parameters.AddWithValue("@EventEnd", eventend);
                            updateCommand.Parameters.AddWithValue("@Date", date);
                            updateCommand.Parameters.AddWithValue("@DP", dp ?? (object)DBNull.Value);
                            updateCommand.Parameters.AddWithValue("@EventFine", eventfine);

                            updateCommand.ExecuteNonQuery();
                            Console.WriteLine($"Updated record: {eventname}, {studentname}, Fine: {eventfine}, DP Exists: {dp != null}");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

    }
}

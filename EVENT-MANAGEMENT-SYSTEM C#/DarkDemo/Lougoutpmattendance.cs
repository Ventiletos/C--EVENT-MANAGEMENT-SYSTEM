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
    public partial class Lougoutpmattendance : Form
    {
        MySqlConnection conn;
        MySqlCommand cmd;
        string connectionString = "server=localhost;port=3307;username=root;password=;database=events_management;";
        private Timer timerTimeUpdate;

        public Lougoutpmattendance()
        {
            InitializeComponent();
            conn = new MySqlConnection(connectionString);

            timerTimeUpdate = new Timer();
            timerTimeUpdate.Interval = 1000; // 1 second
            timerTimeUpdate.Tick += TimerTimeUpdate_Tick;
            timerTimeUpdate.Start();
            LoadClosestEvent();

        }
        private void TimerTimeUpdate_Tick(object sender, EventArgs e)
        {
            try
            {
                // Get the current UTC time
                DateTime utcNow = DateTime.UtcNow;

                // Get the TimeZoneInfo object for the Philippine Standard Time (PST)
                TimeZoneInfo philippinesTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");

                // Convert UTC time to Philippine time
                DateTime philippinesTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, philippinesTimeZone);

                // Display the Philippine Time in 12-hour AM/PM format
                txttime.Text = philippinesTime.ToString("hh:mm:ss tt"); // 12-hour format with AM/PM

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnlogoutpmttracker_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtname.Text) ||
      string.IsNullOrWhiteSpace(txtsid.Text) ||
      string.IsNullOrWhiteSpace(txtevent.Text))
            {
                MessageBox.Show("Student ID required. Please fill out the valid details.",
                                "Validation Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return; // Exit the method if any field is empty
            }

            // Check if the student has already logged in
            string savedLoginTime = GetLogoutnpmTime();
            if (savedLoginTime != null)
            {
                lblmessage.Text = $"{txtname.Text} has already logged in at {savedLoginTime}.";
                lblmessage.ForeColor = Color.Orange;
                return; // Exit if already logged in
            }

            DateTime currentTime = DateTime.Now;
            TimeSpan startTime = new TimeSpan(18, 30, 0); // 6:30 PM start time
            TimeSpan endTime = new TimeSpan(19, 30, 0);   // 7:30 PM end time

            // Check if current time is within the allowed range (6:00 AM to 7:30 AM)
            if (currentTime.TimeOfDay >= startTime && currentTime.TimeOfDay <= endTime)
            {
                lblmessage.Text = $"{txtname.Text} successfully logged in at {currentTime.ToString("hh:mm:ss tt")}.";
                lblmessage.ForeColor = Color.Green;

                // Update loginam with the current time (mark as on time)
                UpdateLogoutpmTime(currentTime);
            }
            else
            {
                // Check if the time is before the allowed range (early) or after (late)
                if (currentTime.TimeOfDay < startTime)
                {
                    lblmessage.Text = $"{txtname.Text} attempted to log in too early. Login not accepted.";
                    lblmessage.ForeColor = Color.Blue;
                    return;
                }
                else if (currentTime.TimeOfDay > endTime)
                {
                    // Apply fine for late login
                    ApplyFine();

                    lblmessage.Text = $"{txtname.Text} failed to login at the correct time. Marked as absent and fine applied.";
                    lblmessage.ForeColor = Color.Red;
                }

                // Update loginam with the actual login time (even if late)
                UpdateLogoutpmTime(currentTime);
            }
        }
        private void UpdateLogoutpmTime(DateTime loginTime)
        {
            try
            {
                conn.Open();
                string query = "UPDATE report_table SET logoutpm = @loginTime WHERE studentid = @studentid AND event = @eventName AND (logoutpm = '00:00:00' OR logoutpm IS NULL)";
                cmd = new MySqlCommand(query, conn);
                // Save the time in 12-hour format (AM/PM)
                cmd.Parameters.AddWithValue("@loginTime", loginTime.ToString("hh:mm:ss tt")); // 12-hour format with AM/PM
                cmd.Parameters.AddWithValue("@studentid", txtsid.Text);
                cmd.Parameters.AddWithValue("@eventName", txtevent.Text);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    MessageBox.Show("No rows were updated. Check if the query matched any records.", "Update Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Login time updated successfully.", "Update Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating login time: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }
        private string GetLogoutnpmTime()
        {
            try
            {
                conn.Open();
                string query = "SELECT logoutpm FROM report_table WHERE studentid = @studentid AND event = @eventName";
                cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@studentid", txtsid.Text);
                cmd.Parameters.AddWithValue("@eventName", txtevent.Text);

                var result = cmd.ExecuteScalar();
                if (result != DBNull.Value && !string.IsNullOrEmpty(result.ToString()) && result.ToString() != "00:00:00")
                {
                    // Return login time in 12-hour format
                    DateTime loginDateTime = DateTime.Parse(result.ToString());
                    return loginDateTime.ToString("hh:mm:ss tt"); // Format as 12-hour AM/PM
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking login status: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                conn.Close();
            }
        }

        private bool IsTimeInRange(TimeSpan currentTime, TimeSpan startTime, TimeSpan endTime)
        {
            return currentTime >= startTime && currentTime <= endTime;
        }

        // Method to apply the fine dynamically based on eventfine in the database
        private void ApplyFine()
        {
            try
            {
                conn.Open();

                // Fetch the eventfine from the report_table for the specific student and event
                string query = "SELECT eventfine FROM report_table WHERE studentid = @studentid AND event = @eventName";
                cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@studentid", txtsid.Text);
                cmd.Parameters.AddWithValue("@eventName", txtevent.Text);

                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    int fineAmount = Convert.ToInt32(result);  // Assuming eventfine is an integer in the database

                    // Update the fines directly in the report_table for the student
                    string updateQuery = "UPDATE report_table SET fines = fines + @fine WHERE studentid = @studentid AND event = @eventName";
                    cmd = new MySqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@fine", fineAmount);
                    cmd.Parameters.AddWithValue("@studentid", txtsid.Text);
                    cmd.Parameters.AddWithValue("@eventName", txtevent.Text);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show($"Fine of {fineAmount} applied to student.");
                }
                else
                {
                    MessageBox.Show("Event fine is not set for this event.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error applying fine: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }
        private void txtsid_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtsid.Text))
            {
                FetchStudentData();
            }
            else
            {
                ClearFields(); // Clear the fields if Student ID is empty
            }
        }
        private void FetchStudentData()
        {
            try
            {
                conn.Open();
                // Query to retrieve student information from report_table for ongoing event
                string query = "SELECT studentname, section, loginam, dp, event " +
                               "FROM report_table " +
                               "WHERE studentid = @studentid AND eventstart <= NOW() AND eventend >= NOW() " +
                               "LIMIT 1";
                cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@studentid", txtsid.Text);

                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtname.Text = reader["studentname"].ToString();
                    txtsection.Text = reader["section"].ToString();
                    txtdate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                    // Retrieve the event name
                    txtevent.Text = reader["event"].ToString();

                    // Load the profile picture
                    byte[] dpBytes = reader["dp"] as byte[];
                    if (dpBytes != null && dpBytes.Length > 0)
                    {
                        using (var ms = new MemoryStream(dpBytes))
                        {
                            pictureBoxDP.Image = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        pictureBoxDP.Image = null;
                    }
                }
                else
                {
                    //MessageBox.Show("No ongoing event found for this student.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                }
                reader.Close();
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
        private void LoadClosestEvent()
        {
            try
            {
                conn.Open();
                string query = @"
            SELECT event, eventstart, eventend 
            FROM report_table 
            WHERE eventstart <= NOW() AND eventend >= NOW() -- Fetch ongoing events
            ORDER BY eventstart ASC 
            LIMIT 1"; // Prioritize the closest ongoing event

                cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string eventName = reader["event"].ToString();
                    string eventStart = reader["eventstart"].ToString();
                    string eventEnd = reader["eventend"].ToString();

                    DateTime eventStartTime = DateTime.Parse(eventStart);
                    DateTime eventEndTime = DateTime.Parse(eventEnd);

                    lblmessage.Text = $"Ongoing Event: {eventName} - Date: {eventStartTime:yyyy-MM-dd}";
                    lblmessage.ForeColor = Color.Green;

                    btnlogoutpmttracker.Enabled = true; // Enable the login tracker for ongoing events

                    MessageBox.Show($"Ongoing Event: {eventName}\nStart: {eventStart}\nEnd: {eventEnd}");

                    // Track the current event's end time
                    TrackEvent(eventEndTime);
                }
                else
                {
                    lblmessage.Text = "No ongoing events at the moment.";
                    lblmessage.ForeColor = Color.Gray;
                    btnlogoutpmttracker.Enabled = false; // Disable interaction if no ongoing events are found
                }

                reader.Close();
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
        private void TrackEvent(DateTime currentEventEndTime)
        {
            // Timer to keep checking the event end time every second
            Timer eventTrackingTimer = new Timer();
            eventTrackingTimer.Interval = 1000; // 1 second interval
            eventTrackingTimer.Tick += (sender, e) =>
            {
                if (DateTime.Now >= currentEventEndTime)
                {
                    // Event has ended, load the next closest event
                    eventTrackingTimer.Stop(); // Stop the current timer
                    LoadNextEvent(); // Load the next event
                }
            };
        }

        private void LoadNextEvent()
        {
            try
            {
                conn.Open();
                // Query to find the next closest upcoming event
                string query = "SELECT eventname, eventstart, eventend FROM event_table " +
                               "WHERE eventstart >= NOW() ORDER BY eventstart LIMIT 1";
                cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@currentTime", DateTime.Now);

                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // Display the next closest event details
                    string eventName = reader["eventname"].ToString();
                    string eventStart = reader["eventstart"].ToString();
                    string eventEnd = reader["eventend"].ToString();

                    DateTime eventStartTime = DateTime.Parse(eventStart);
                    DateTime eventEndTime = DateTime.Parse(eventEnd);

                    // Show the next upcoming event
                    MessageBox.Show($"Next Event: {eventName}\nStart: {eventStart}\nEnd: {eventEnd}");

                    // Start a loop to track the event and transition when it ends
                    TrackEvent(eventEndTime);
                }
                else
                {
                    MessageBox.Show("No more upcoming events.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                reader.Close();
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

        private void ClearFields()
        {
            txtname.Clear();
            txtsection.Clear();
            txtevent.Clear();
            txttime.Clear();
            txtdate.Clear();
            pictureBoxDP.Image = null;
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}


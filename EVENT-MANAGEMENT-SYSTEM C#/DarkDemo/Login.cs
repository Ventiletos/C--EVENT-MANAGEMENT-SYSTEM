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
using Utilities.BunifuGradientPanel.Transitions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DarkDemo
{
    public partial class Login : Form
    {
        MySqlConnection conn;
        MySqlCommand cmd;
        string connectionString = "server=localhost;port=3307;username=root;password=;database=events_management;";
        public Login()
        {
            InitializeComponent();
            cmbaattend.Hide();
            conn = new MySqlConnection(connectionString);
           //CheckForEventAndReset();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Username.Text = "";
            Password.Text = "";
        }
   
        private void guna2ToggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2ToggleSwitch1.Checked)
            {
                this.BackColor = Color.FromArgb(255, 255, 255);
                btnlogin.FillColor = Color.ForestGreen;
               
            }     
            else
            {
                this.BackColor = Color.FromArgb(41, 44, 51);
                btnlogin.FillColor = Color.FromArgb(41, 44, 51);
            }
        }
        private void Username_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Password.Focus(); // Move focus to password field
            }
        }

        // When "Enter" is pressed in the password field, attempt login
        private void Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnadd_Click(sender, e); // Call the login button click event to perform login
            }
        }
        private void show_Click(object sender, EventArgs e)
        {
                Hide.BringToFront();
                Password.PasswordChar = '\0';
        }

        private void Hide_Click(object sender, EventArgs e)
        {
                show.BringToFront();
                Password.PasswordChar = '●';
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnadd_Click(object sender, EventArgs e)
        {
            string username = "123";
            string password = "123";

            if (Username.Text == username && Password.Text == password)
            {

                Dashboard dashboard = new Dashboard();
                dashboard.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("The username or password is  incorrect.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnloginattend_Click(object sender, EventArgs e)
        {
            cmbaattend.Show();
            cmbaattend.DroppedDown = true;
        }
        private void cmbLoginType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = cmbaattend.SelectedItem.ToString();

            // Check which item was selected and open the corresponding form
            if (selectedItem == "Login_am")
            {
               Loginattendance l = new Loginattendance();
                l.Show();
                cmbaattend.Hide();
            }
            else if (selectedItem == "Logout_am")
            {
                Logoutattendance l = new Logoutattendance();
                l.Show();
                cmbaattend.Hide();

            }
            else if (selectedItem == "Login_pm")
            {
               Loginpmattendance l = new Loginpmattendance();
                l.Show();
                cmbaattend.Hide();
            }
            else if (selectedItem == "Logout_pm")
            {
                Lougoutpmattendance l = new Lougoutpmattendance(); 
                l.Show();
                cmbaattend.Hide();
            }
        }
        private void CheckForEventAndReset()
        {
            try
            {
                conn.Open();

                // Get the current date and time
                DateTime currentTime = DateTime.Now;

                // Query to get the closest event starting after the current time
                string query = "SELECT eventname, eventstart, eventend FROM event_table WHERE eventstart >= @currentTime ORDER BY eventstart LIMIT 1";
                cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@currentTime", currentTime);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    DateTime eventStart = DateTime.Parse(reader["eventstart"].ToString());
                    DateTime eventEnd = DateTime.Parse(reader["eventend"].ToString());

                    // Check if the current time is within the event period
                    if (currentTime >= eventStart && currentTime <= eventEnd)
                    {
                        // Reset daily attendance data for the current event period
                        ResetDailyAttendance();
                        MessageBox.Show("Attendance data reset for the current event.");
                    }
                    else if (currentTime > eventEnd)
                    {
                        // If the event has ended, load the next event and reset for it
                        LoadNextEvent();
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while checking events: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void ResetDailyAttendance()
        {
            try
            {
                conn.Open();
                // Only reset login/logout fields in report_table, not affecting other tables
                string resetQuery = "UPDATE report_table SET loginam = NULL, logoutam = NULL, loginpm = NULL, logoutpm = NULL WHERE DATE(date) < CURDATE()";
                cmd = new MySqlCommand(resetQuery, conn);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Attendance data reset successfully for previous days in the report_table.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error resetting attendance data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }
        private void LoadNextEvent()
        {
            try
            {
                conn.Open();
                string query = "SELECT eventname, eventstart, eventend FROM event_table WHERE eventstart >= CURDATE() ORDER BY eventstart LIMIT 1";
                cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string eventName = reader["eventname"].ToString();
                    string eventStart = reader["eventstart"].ToString();
                    string eventEnd = reader["eventend"].ToString();

                    MessageBox.Show($"Next Event: {eventName}\nStart: {eventStart}\nEnd: {eventEnd}");
                }
                else
                {
                    MessageBox.Show("No upcoming events found.");
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while loading next event: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}

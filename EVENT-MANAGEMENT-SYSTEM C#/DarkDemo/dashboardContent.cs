using DarkDemo.DashboardCon;
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
using System.Windows.Forms.DataVisualization.Charting;

namespace DarkDemo
{
    public partial class dashboardContent : Form, IThemeable
    {
        MySqlConnection conn;
        MySqlCommand cmd;
        string connectionString = "server=localhost;port=3307;username=root;password=;database=events_management;";
        public dashboardContent()
        {
            InitializeComponent();
            showchart();
            LoadDashboardConn();
        }

        private void showchart()
        {
            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();

                // Clear previous data from the chart
                chart1.Series[0].Points.Clear();
                chart1.Series[1].Points.Clear();

                // Query to fetch event count by category
                string eventCategoryQuery = "SELECT eventcategory, COUNT(*) AS eventCount FROM event_table GROUP BY eventcategory";
                cmd = new MySqlCommand(eventCategoryQuery, conn);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string eventCategory = reader["eventcategory"].ToString();
                    int eventCount = Convert.ToInt32(reader["eventCount"]);
                    // Add data to chart for event count by category
                    chart1.Series[0].Points.AddXY(eventCategory, eventCount);
                }
                reader.Close();

                // Query to fetch total fines by event category
                string finesQuery = "SELECT eventcategory, SUM(fines) AS totalFines FROM event_table GROUP BY eventcategory";
                cmd = new MySqlCommand(finesQuery, conn);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string eventCategory = reader["eventcategory"].ToString();
                    decimal totalFines = Convert.ToDecimal(reader["totalFines"]);
                    // Add data to chart for total fines by category
                    chart1.Series[1].Points.AddXY(eventCategory, totalFines);
                }
                reader.Close();

                // Customize chart design (color, style, etc.)
                //chart1.Series[0].Color = Color.FromArgb(0, 191, 255);  // Bright cyan for event count
                chart1.Series[1].Color = Color.ForestGreen;  // Hot pink for fines
                chart1.Series[0].BorderWidth = 3;
                chart1.Series[1].BorderWidth = 3;

                // Apply gradient styles to chart areas
                //chart1.Series[0].BackGradientStyle = GradientStyle.TopBottom;
                //chart1.Series[0].BackSecondaryColor = Color.FromArgb(0, 255, 255);
                chart1.Series[1].BackGradientStyle = GradientStyle.TopBottom;
                chart1.Series[1].BackSecondaryColor = Color.FromArgb(255, 182, 193);

                // Set chart area background and gradient style
                chart1.ChartAreas[0].BackColor = Color.FromArgb(41, 44, 51);
                //chart1.ChartAreas[0].BackGradientStyle = GradientStyle.TopBottom;
                chart1.ChartAreas[0].BackSecondaryColor = Color.FromArgb(30, 30, 30);

                // Customize axis labels with a modern, readable style
                chart1.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;  // X-axis labels color
                chart1.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;  // Y-axis labels color
                chart1.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Arial", 12, FontStyle.Bold);
                chart1.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Arial", 12, FontStyle.Bold);

                // Show value labels on top of bars
                chart1.Series[0].IsValueShownAsLabel = true;
                chart1.Series[1].IsValueShownAsLabel = true;
                chart1.Series[0].Font = new Font("Arial", 10, FontStyle.Bold);
                chart1.Series[1].Font = new Font("Arial", 10, FontStyle.Bold);

                // Refresh chart
                chart1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading chart data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        public void ChangeTheme(bool isDarkMode)
        {
            mainPanel.BackColor = isDarkMode ? Color.FromArgb(255, 255, 255) : Color.FromArgb(41, 44, 51);
            chart1.ChartAreas[0].AxisY.LabelStyle.ForeColor = isDarkMode ? Color.FromArgb(64, 64, 64) : Color.White;
            chart1.ChartAreas[0].AxisX.LabelStyle.ForeColor = isDarkMode ? Color.FromArgb(64, 64, 64) : Color.White;

            label.ForeColor = isDarkMode ? Color.FromArgb(64, 64, 64) : Color.White;
            chart1.ChartAreas[0].BackColor = isDarkMode ? Color.White : Color.FromArgb(41, 44, 51);
            label1.ForeColor = isDarkMode ? Color.FromArgb(64, 64, 64) : Color.White;
            label2.ForeColor = isDarkMode ? Color.FromArgb(64, 64, 64) : Color.White;
            label3.ForeColor = isDarkMode ? Color.FromArgb(64, 64, 64) : Color.White;
            label4.ForeColor = isDarkMode ? Color.FromArgb(64, 64, 64) : Color. White;
            label5.ForeColor = isDarkMode ? Color.FromArgb(64, 64, 64) : Color.White;
                  
            pb.BackColor = isDarkMode ? Color.FromArgb(255, 255, 255) : Color.FromArgb(41, 44, 51);
            chart1.BackColor = isDarkMode ? Color.FromArgb(255, 255, 255) : Color.FromArgb(41, 44, 51);
        }
        public void LoadDashboardConn()
        {
            dashBoardConnections d = new dashBoardConnections();
            int eventCount = d.GetEventCount();
            lblevents.Text = $" {eventCount}";
            lbltotalevent.Text = $"Total events: {eventCount}/{eventCount}";

            int departmentCount = d.GetTotalDepartments();
            lbldepartment.Text = $" {departmentCount}";
            lbltotaldepartment.Text = $"Total Departments: {departmentCount}/{departmentCount}";

            int studentidCount = d.GetTotalId();
            lblstudentcount.Text = $" {studentidCount}";
            lbltotalstudent.Text = $"Total Students: {studentidCount}/{studentidCount}";

            decimal totalfines = d.GetTotalFines();
            lbltotalfines.Text = $" {totalfines}";

        }

        private void mainPanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

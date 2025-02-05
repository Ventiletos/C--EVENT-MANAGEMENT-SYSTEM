using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Microsoft.Reporting.WinForms;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DarkDemo
{
    public partial class reportsprint : Form
    {
        public reportsprint()
        {
            InitializeComponent();
            LoadReport();
        }
        string connectionString = "server=localhost;port=3307;username=root;password=;database=events_management;";

        private void InitializeComboBoxesAndDatePicker()
        {
            cmbeventfilter.Items.Clear();
            cmbeventfilter.Items.Add("All");
            cmbeventfilter.Items.Add("Ongoing");
            cmbeventfilter.Items.Add("Ended");
            cmbeventfilter.Items.Add("Upcoming");
            cmbeventfilter.SelectedIndex = 0; 

            cmbsection.Items.Clear();
            cmbsection.Items.Add("All");
            cmbsection.SelectedIndex = 0;

            datetimenow.Value = DateTime.Now;
        }
        public DataTable GetFilteredData(string eventStatus, string section, DateTime selectedDate, out decimal totalFines)
        {
            DataTable filteredData = new DataTable();
            totalFines = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT r.*, e.*, r.fines " +
                               "FROM report_table r " +
                               "JOIN event_table e ON r.event = e.eventname " +
                               "WHERE 1 = 1";

                if (!string.IsNullOrEmpty(eventStatus) && eventStatus != "All")
                {
                    if (eventStatus == "Ongoing")
                    {
                        query += " AND e.eventstart <= @CurrentDate AND e.eventend >= @CurrentDate";
                    }
                    else if (eventStatus == "Ended")
                    {
                        query += " AND e.eventend < @CurrentDate";
                    }
                    else if (eventStatus == "Upcoming")
                    {
                        query += " AND e.eventstart > @CurrentDate";
                    }
                }

                if (!string.IsNullOrEmpty(section) && section != "All")
                {
                    query += " AND r.section = @Section";
                }

                query += " AND r.date = @Date";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CurrentDate", DateTime.Now);
                    command.Parameters.AddWithValue("@Section", section ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Date", selectedDate.Date);

                    connection.Open();
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(filteredData);
                    }

                    totalFines = filteredData.AsEnumerable()
                        .Sum(row =>
                        {
                            var finesValue = row["fines"];

                            if (finesValue == DBNull.Value)
                            {
                                return 0m; 
                            }

                            if (finesValue is int intFines)
                            {
                                return Convert.ToDecimal(intFines);
                            }

                            return 0m;
                        });
                }
            }

            return filteredData;
        }
        private void LoadReport()
        {
            // Get filter values
            string eventStatus = cmbeventfilter.SelectedItem?.ToString();
            string section = cmbsection.SelectedItem?.ToString();
            DateTime selectedDate = datetimenow.Value.Date;

            // Get filtered data and calculate total fines
            decimal totalFines;
            DataTable filteredData = GetFilteredData(eventStatus, section, selectedDate, out totalFines);

            // Bind the filtered data to the report
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", filteredData));

            // Pass the TotalFines parameter to the report
            var totalFinesParameter = new ReportParameter("TotalFines", totalFines.ToString("F2"));

            // Pass the DatePrint parameter to the report (current date)
            var datePrintParameter = new ReportParameter("DatePrint", DateTime.Now.ToString("MM/dd/yyyy"));

            // Set the parameters to the report
            reportViewer1.LocalReport.SetParameters(new ReportParameter[] { totalFinesParameter, datePrintParameter });

            reportViewer1.RefreshReport();
        }

        private void cmbeventfilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadReport(); // Reload the report when the filter changes
        }

        // Event handler for the section filter combo box
        private void cmbsection_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadReport(); // Reload the report when the filter changes
        }

        // Event handler for the date picker value change
        private void datetimenow_ValueChanged(object sender, EventArgs e)
        {
            LoadReport(); // Reload the report when the date changes
        }
        private void reportsprint_Load(object sender, EventArgs e)
        {
            InitializeComboBoxesAndDatePicker();

            // Populate the section combo box with distinct sections from the database
            LoadSections();

            // Load the report immediately after the form loads
            LoadReport();

            // Refresh the report viewer
            this.reportViewer1.RefreshReport();
        }
        private void LoadSections()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT DISTINCT section FROM report_table";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        cmbsection.Items.Clear();
                        cmbsection.Items.Add("All");  // Add "All" as an option
                        while (reader.Read())
                        {
                            cmbsection.Items.Add(reader.GetString("section"));
                        }
                    }
                }
            }
        }

        private void reportViewer1_Load(object sender, EventArgs e)
        {

        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

        }
    }
}

using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkDemo.SqlCon
{
    internal class SqlConnections
    {
        
        public static DataTable GetReportData()
        {
            DataTable dataTable = new DataTable();
            string connectionString = "server=localhost;port=3307;username=root;password=;database=events_management;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT event, studentid, studentname, section, date, loginam, logoutam, loginpm, logoutpm, fines FROM report_table;";

                using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }
        public static void LoadDataIntoDataGridView()
        {
            Reports reports = new Reports();
            DataTable reportData = GetReportData();
            reports.guna2DataGridView1.DataSource = reportData;
        }
    }
}

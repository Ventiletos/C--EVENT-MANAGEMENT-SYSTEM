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

namespace DarkDemo
{
    public partial class ReportsView : Form
    {
        private string connectionString = "server=localhost;port=3307;username=root;password=;database=event_management;";
        public ReportsView()
        {
            InitializeComponent();
        }

        private void ReportsView_Load(object sender, EventArgs e)
        {
           
        }
       
    }
}

using DarkDemo.DashboardCon;
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
namespace DarkDemo
{
    public partial class Dashboard : Form
    {
        public static bool IsDarkMode { get; set; } = false;
        public Dashboard()
        {
            this.modes = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            InitializeComponent();
            
            LoadDash();
        }
        public void LoadForm(Form form)
        {

            if (this.mainPanel.Controls.Count > 0)
                this.mainPanel.Controls.Clear();


            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            this.mainPanel.Controls.Add(form);
            form.Show();
        }

        //functions for the light mode and dark mode
        private void modes_CheckedChanged(object sender, EventArgs e)
        {
            if (modes.Checked)
            {
                panel1.BackColor = Color.FromArgb(255, 255, 255);
                panel2.BackColor = Color.FromArgb(255, 255, 255);
                panel4.BackColor = Color.FromArgb(255, 255, 255);
                pb.BackColor = Color.FromArgb(255, 255, 255);
                mainPanel.BackColor = Color.FromArgb(255, 255, 255);
                label1.ForeColor = Color.FromArgb(64, 64, 64);
                label2.ForeColor = Color.FromArgb(64, 64, 64);
                label3.ForeColor = Color.FromArgb(64, 64, 64);
                label18.ForeColor = Color.FromArgb(64, 64, 64);
            }
            else
            {
                panel1.BackColor = Color.FromArgb(41, 44, 51);
                panel2.BackColor = Color.FromArgb(41, 44, 51);
                panel4.BackColor = Color.FromArgb(41, 44, 51);
                pb.BackColor = Color.FromArgb(41, 44, 51);
                mainPanel.BackColor = Color.FromArgb(41, 44, 51);
                label1.ForeColor = Color.White;
                label2.ForeColor = Color.White;
                label3.ForeColor = Color.White;
                label18.ForeColor = Color.White;
                label3.ForeColor = Color.White;
            }
            Dashboard.IsDarkMode = modes.Checked;
            if (mainPanel.Controls.Count > 0)
            {
                var currentForm = mainPanel.Controls[0] as IThemeable;

                currentForm?.ChangeTheme(Dashboard.IsDarkMode);
            }
        }

        //btn for event management form
        private void eventmanagement_Click_1(object sender, EventArgs e)
        {
            @event events = new @event();
            LoadForm(events);

            var eventManagement = new @event();
            eventManagement.ChangeTheme(IsDarkMode); 
            LoadForm(eventManagement);
        }
        public void LoadDash()
        {
            dashboardContent d = new dashboardContent();
            LoadForm(d);
        }
        private void dashbord_Click(object sender, EventArgs e)
        {
            LoadDash();
            var dashboard = new dashboardContent();
            dashboard.ChangeTheme(Dashboard.IsDarkMode); 
            LoadForm(dashboard);
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void department_Click(object sender, EventArgs e)
        {
            Department d = new Department();
            LoadForm(d);

            var d1 = new Department();
            d1.ChangeTheme(IsDarkMode);
            LoadForm(d1);
        }
        private void student_Click(object sender, EventArgs e)
        {
            student s = new student();
            LoadForm(s);

            var s2 = new student();
            s2.ChangeTheme(IsDarkMode);
            LoadForm(s2);
        }
        private void btnlogout_Click(object sender, EventArgs e)
        {
            dashBoardConnections d = new dashBoardConnections();
            d.PopulateReportTable();
            Login login = new Login();
            login.Show();
            modes.Checked = false;
            modes_CheckedChanged(sender, e);
            this.Close();
        }

        private void btnreports_Click(object sender, EventArgs e)
        {
           Reports reports = new Reports();
            LoadForm(reports);

            var r1 = new Reports();
            r1.ChangeTheme(IsDarkMode);
            LoadForm(r1);
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2CirclePictureBox1_Click(object sender, EventArgs e)
        {
            LoadForm(new dashboardContent());
        }
    }
}

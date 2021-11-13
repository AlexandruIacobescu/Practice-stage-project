using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Practice_stage_project
{
    public partial class adminForm : Form
    {
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MARKET;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public adminForm()
        {
            InitializeComponent();
        }

        int RND()
        {
            Random rd = new Random();
            int rand_num = rd.Next(1000000, 9999999);
            return rand_num;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 newf = new Form1();
            newf.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" && comboBox1.SelectedItem != null)
                {
                    SqlConnection con = new SqlConnection(connectionString);
                    con.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Employees(eID, FirstName, LastName, SocialSecurityNumber, EmailAddress, Department) VALUES(@eid, @fnm, @lnm, @ssn, @email, @dpd)", con);
                    cmd.Parameters.AddWithValue("eid", textBox1.Text);
                    cmd.Parameters.AddWithValue("fnm", textBox2.Text);
                    cmd.Parameters.AddWithValue("lnm", textBox3.Text);
                    cmd.Parameters.AddWithValue("ssn", textBox4.Text);
                    cmd.Parameters.AddWithValue("email", textBox5.Text);
                    cmd.Parameters.AddWithValue("dpd", comboBox1.SelectedItem.ToString());
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    con.Close();                   
                    MessageBox.Show("Employee created succesfully.", "Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = string.Empty;
                    comboBox1.SelectedItem = null;
                }
                else
                {
                    MessageBox.Show("There are empty mandatory fields left.", "Admin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                displayEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void displayEmployees()
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM Employees", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dataGridView1.DataSource = dt;
            dataGridView1.AutoGenerateColumns = false;
            con.Close();
        }

        private void adminForm_Load(object sender, EventArgs e)
        {
            displayEmployees();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            string fn = "";
            fn = fn + textBox2.Text[0] + textBox2.Text[1];
            string ln = "";
            ln = ln + textBox3.Text[0] + textBox3.Text[1];
            string dpd = "";
            dpd = dpd + comboBox1.SelectedItem.ToString()[0] + comboBox1.SelectedItem.ToString()[1];

            textBox1.Text = fn + ln + dpd;

            textBox1.Text = textBox1.Text + RND().ToString();

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "" && textBox3.Text != "" && comboBox1.SelectedItem != null && textBox2.Text.Length >= 2 && textBox3.Text.Length >= 2)
                button3.Visible = true;
            else 
                button3.Visible = false;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "" && textBox3.Text != "" && comboBox1.SelectedItem != null && textBox2.Text.Length >= 2 && textBox3.Text.Length >= 2)
                button3.Visible = true;
            else
                button3.Visible = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "" && textBox3.Text != "" && comboBox1.SelectedItem != null && textBox2.Text.Length >= 2 && textBox3.Text.Length >= 2)
                button3.Visible = true;
            else
                button3.Visible = false;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}

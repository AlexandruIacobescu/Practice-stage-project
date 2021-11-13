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
    public partial class Form3 : Form
    {
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MARKET;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public Form3()
        {
            InitializeComponent();
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
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM Accounts WHERE Username = @usr", con);
                sda.SelectCommand.Parameters.AddWithValue("usr", textBox1.Text);
                DataTable dtb = new DataTable();
                sda.Fill(dtb);

                if (dtb.Rows.Count == 1)
                {
                    MessageBox.Show("This username is already taken", "Register", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Text = textBox2.Text = textBox3.Text = "";
                }
                else if ((textBox2.Text).Length < 8)
                {
                    MessageBox.Show("The password must be at least 8 characters long!", "Register", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox2.Text = textBox3.Text = "";
                }
                else if (textBox2.Text != textBox3.Text)
                {
                    MessageBox.Show("The passwords do not match!", "Register", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox2.Text = textBox3.Text = "";
                }
                else
                {
                    sda = new SqlDataAdapter("SELECT * FROM Employees WHERE eID = @eid", con);
                    sda.SelectCommand.Parameters.AddWithValue("eid", textBox4.Text);
                    dtb = new DataTable();
                    sda.Fill(dtb);
                    if (dtb.Rows.Count != 1)
                    {
                        MessageBox.Show("You are not authorized to register!\nEmployee security ID not found.", "Register", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = "";
                    }
                    else
                    {
                        SqlDataAdapter sda2 = new SqlDataAdapter("SELECT eID FROM Employees WHERE eID = @eid", con);
                        sda2.SelectCommand.Parameters.AddWithValue("eid", textBox4.Text);
                        DataTable dtbl = new DataTable();
                        sda2.Fill(dtbl);
                        SqlDataAdapter sda3 = new SqlDataAdapter("SELECT sID FROM Accounts WHERE sID = @sid", con);
                        sda3.SelectCommand.Parameters.AddWithValue("sid", textBox4.Text);
                        DataTable dtbl2 = new DataTable();
                        sda3.Fill(dtbl2);
                        if (dtbl2.Rows.Count == 1)
                        {
                            MessageBox.Show("This employee already has an account!", "Register", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            string qry = "INSERT INTO Accounts (sID, Username, Password) VALUES(@sid, @usr, @psw)";
                            SqlCommand cmd = new SqlCommand(@qry, con);
                            cmd.Parameters.AddWithValue("sid", dtbl.Rows[0]["eID"]);
                            cmd.Parameters.AddWithValue("usr", textBox1.Text);
                            cmd.Parameters.AddWithValue("psw", textBox2.Text);
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            MessageBox.Show("Registration successfull!", "Register", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Hide();
                            Form1 newf = new Form1();
                            newf.Show();
                        }
                    }
                }
                con.Close();                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace Practice_stage_project
{
    public partial class Form1 : Form
    {

        const string sqlConnection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MARKET;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection sqlcon = new SqlConnection(sqlConnection);
            sqlcon.Open();
            string query = "SELECT * FROM Accounts WHERE Username COLLATE Latin1_General_CS_AS= @usr AND Password COLLATE Latin1_General_CS_AS = @psw ";
            SqlDataAdapter sda = new SqlDataAdapter(query, sqlcon);
            sda.SelectCommand.Parameters.AddWithValue("usr", textBox1.Text);
            sda.SelectCommand.Parameters.AddWithValue("psw", textBox2.Text);
            sda.SelectCommand.ExecuteNonQuery();
            DataTable dtbl = new DataTable();
            sda.Fill(dtbl);
            if(dtbl.Rows.Count == 1)
            {
                Form2 newf = new Form2();
                this.Hide();
                newf.Show();
            }
            else
            {
                MessageBox.Show("Username and/or password incorrect.", "Products Manager Toolkit", MessageBoxButtons.OK, MessageBoxIcon.Warning );
                textBox1.Text = textBox2.Text = "";
            }
            sqlcon.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*this.Hide();
            messageForm newf = new messageForm();
            newf.Show();        
            */
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form3 newf = new Form3();
            newf.Show();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
                textBox2.PasswordChar = '\0';
            else
                textBox2.PasswordChar = '•';
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            adminLoginForm newf = new adminLoginForm();
            newf.Show();
        }
    }
}

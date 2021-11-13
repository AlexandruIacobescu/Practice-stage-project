using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Practice_stage_project
{
    public partial class adminLoginForm : Form
    {
        public adminLoginForm()
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
            if (textBox1.Text == "admin" && textBox2.Text == "admin")
            {
                this.Hide();
                adminForm newf = new adminForm();
                newf.Show();
            }
            else
            {
                MessageBox.Show("Username or Password incorrect.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Text = textBox2.Text = string.Empty;
            }
        }
    }
}

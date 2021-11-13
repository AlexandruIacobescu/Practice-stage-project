using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Practice_stage_project
{
    public partial class Form2 : Form
    {
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MARKET;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";



        public Form2()
        {
            InitializeComponent();
        }

        void deleteDB()
        {
            string reseed = "DBCC CHECKIDENT('[Products]', RESEED, 0)";
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM Products", conn);
            cmd.ExecuteNonQuery();
            cmd = new SqlCommand(reseed, conn);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            conn.Close();
            displayDb();
        }

        void displayDb()
        {
            SqlConnection sqlcon = new SqlConnection(connectionString);
            sqlcon.Open();
            SqlDataAdapter sqlDa = new SqlDataAdapter("select * from dbo.Products", sqlcon);
            DataTable dtbl = new DataTable();
            sqlDa.Fill(dtbl);
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = dtbl;
        }

        string upper(string text)
        {
            string aux = text;
            string[] auxa = aux.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            aux = string.Empty;
            for (int i = 0; i < auxa.Length; i++)
            {
                auxa[i].Trim();
                if (i == auxa.Length - 1)
                {
                    aux = aux + char.ToUpper(auxa[i][0]) + auxa[i].Substring(1);
                }
                else
                {
                    aux = aux + char.ToUpper(auxa[i][0]) + auxa[i].Substring(1) + " ";
                }
            }
            return aux;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button10.Visible = false;
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            try
            {
                if (catBox.Text == "" || catBox.Text == "" || qtyBox.Text == "" || titleBox.Text == "")
                {
                    MessageBox.Show("There is at least one empty text field, which is not permitted!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (catBox.Text != "" && catBox.Text != "" && qtyBox.Text != "" && titleBox.Text != "")
                {                                                            
                    SqlCommand cmd = new SqlCommand("INSERT INTO Products(Title, Category, Quantity, ExpirationDate) VALUES(@tit, @cat, @qty, @date)", conn);
                    cmd.Parameters.AddWithValue("tit", upper(titleBox.Text));
                    cmd.Parameters.AddWithValue("cat", upper(catBox.Text));
                    cmd.Parameters.AddWithValue("qty", qtyBox.Text);
                    DateTime DT;
                    if (comboBox1.SelectedIndex == 1)
                    {
                        DT = Convert.ToDateTime(dateBox.Text, System.Globalization.CultureInfo.GetCultureInfo("fr-FR"));
                    }
                    else
                    {
                        DT = Convert.ToDateTime(dateBox.Text);
                    }                    
                    cmd.Parameters.AddWithValue("date", DT);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    displayDb();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 newf = new Form1();
            newf.Show();
        }


        void check_today()
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string date = DateTime.UtcNow.ToString("MM-dd-yyyy");
            SqlDataAdapter sda = new SqlDataAdapter("select * from Products where ExpirationDate = '" + date + "'", con);
            DataTable dtbl = new DataTable();
            sda.Fill(dtbl);
            if (dtbl.Rows.Count >= 1)
            {
                MessageBox.Show("There are products in the Database that expire today!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }            
            con.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            button10.Visible = false;
            comboBox1.SelectedIndex = 0;
            button11.Visible = false;
            check_today();
            textBox5.Visible = false;
            button6.Visible = false;
            displayDb();
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            string query = "select ID from Products";
            SqlDataAdapter sda = new SqlDataAdapter(query, conn);
            DataTable dtbl = new DataTable();
            sda.Fill(dtbl);
            int max = 0;
            for(int i=0;i<dtbl.Rows.Count;i++)
            {
                int d = Int32.Parse(dtbl.Rows[i]["ID"].ToString());
                if (max < d)
                    max = d;
            }
            SqlCommand cmd = new SqlCommand("DBCC CHECKIDENT('[Products]', RESEED, " + max.ToString() + ")", conn);
            cmd.ExecuteNonQuery();
            conn.Close();            
        }


        void loadFrom_csv(string path)
        {
            if (path != "")
            {
                try
                {
                    SqlConnection conn = new SqlConnection(connectionString);
                    conn.Open();
                    StreamReader sr = new StreamReader(@path);
                    string str;
                    char[] split = { ',' };
                    DateTime dt;
                    SqlCommand cmd;
                    sr.ReadLine();
                    while ((str = sr.ReadLine()) != null)
                    {
                        string[] strings = str.Split(split);
                        cmd = new SqlCommand("INSERT INTO Products(Title, Category, Quantity, ExpirationDate) VALUES (@tit, @cat, @qty, @bb)", conn);
                        cmd.Parameters.AddWithValue("tit", strings[0].Trim());
                        cmd.Parameters.AddWithValue("cat", strings[1].Trim());
                        cmd.Parameters.AddWithValue("qty", strings[2].Trim());
                        //dt = Convert.ToDateTime(strings[3].Trim(), System.Globalization.CultureInfo.GetCultureInfo("fr-FR"));
                        dt = Convert.ToDateTime(strings[3].Trim());
                        cmd.Parameters.AddWithValue("bb", dt);
                        cmd.ExecuteNonQuery();
                    }

                    displayDb();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    MessageBox.Show("Invalid csv formatting.\nThe csv rows should have the following data types:\n[string, string, integer, date (MM/DD/YYYY)]","Database Management",MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button10.Visible = false;
            if (checkBox1.Checked == false)
            {
                string csv_default_path = Application.StartupPath + "database.csv";
                loadFrom_csv(csv_default_path);
            }            
            else
            {                
                string csv_path = textBox5.Text;
                try
                {
                    loadFrom_csv(csv_path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button10.Visible = false;
            DialogResult dr = MessageBox.Show("Are you sure you want to delete all data in the [Products] table?", "Delete Table Data", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.OK)
            {
                deleteDB();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                button11.Visible = true;
                textBox5.Visible = true;
                label6.Visible = true;
                checkBox5.Visible = true;
                if(checkBox5.Checked == false)
                    button13.Visible = true;
            }
            else
            {
                button11.Visible = false;
                textBox5.Visible = false ;
                label6.Visible = false ;
                checkBox5.Visible = false;
                button13.Visible = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox2.Checked == true)
            {
                button6.Visible = true;
            }
            else
            {
                button6.Visible = false;
            }
        }

        private async void del_msg(int m, int no)
        {
            if (m == 1)
            {
                label7.Text = "Delete operation successful.\n["+no.ToString()+"] items Deleted"; 
                label7.ForeColor = System.Drawing.Color.Green;
                label7.Visible = true;
                await Task.Delay(3500);
                label7.Visible = false;
            }
            else if(m == 2)
            {
                label7.Text = "No item found to be deleted.";
                label7.ForeColor = System.Drawing.Color.Red;
                label7.Visible = true;
                await Task.Delay(2500);
                label7.Visible = false;
            }
            else if(m == 3)
            {
                label7.Text = "Empty search result.";
                label7.ForeColor = System.Drawing.Color.Red;
                label7.Visible = true;
                await Task.Delay(2500);
                label7.Visible = false;
            }
        }


        private  void button6_Click_1(object sender, EventArgs e)
        {   
            DateTime DT = Convert.ToDateTime("1/1/2001");
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            string msg = "SELECT * FROM Products WHERE ";
            try
            {
                string query = "DELETE FROM Products WHERE ";                

                msg = "SELECT * FROM Products WHERE ";

                bool ok = false;
                if(titleBox.Text!="")
                {
                    ok = true;
                    msg = msg + "Title = @tit";
                    query = query + "Title = @tit";
                }
                if(catBox.Text != "")
                {
                    if (ok)
                    {
                        msg = msg + " AND Category = @cat";
                        query = query + " AND Category = @cat";
                    }

                    else
                    {
                        msg = msg + "Category = @cat";
                        query = query + "Category = @cat";
                    }

                    ok = true;
                }
                if(qtyBox.Text !="")
                {
                    if (ok)
                    {
                        msg = msg + " AND Quantity = @qty";
                        query = query + " AND Quantity = @qty";
                    }
                    else
                    {
                        msg = msg + "Quantity = @qty";
                        query = query + "Quantity = @qty";
                    }
                    ok = true;
                }
                if (dateBox.Text != "")
                {
                    try
                    {
                        if (comboBox1.SelectedIndex == 1)
                        {
                            DT = Convert.ToDateTime(dateBox.Text, System.Globalization.CultureInfo.GetCultureInfo("fr-FR"));
                        }
                        else
                        {
                            DT = Convert.ToDateTime(dateBox.Text);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Date format is wrong.\nPlease change the date formatting.", "Database Management", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (ok)
                    {
                        msg = msg + " AND ExpirationDate = @date";
                        query = query + " AND ExpirationDate = @date";
                    }
                    else
                    {
                        msg = msg + "ExpirationDate = @date";
                        query = query + "ExpirationDate = @date"; ;
                    }
                }
                SqlDataAdapter sda = new SqlDataAdapter(msg, conn);
                sda.SelectCommand.Parameters.AddWithValue("tit", titleBox.Text);
                sda.SelectCommand.Parameters.AddWithValue("cat", catBox.Text);
                sda.SelectCommand.Parameters.AddWithValue("qty", qtyBox.Text);
                if(dateBox.Text != "")
                    sda.SelectCommand.Parameters.AddWithValue("date", DT);
                else
                    sda.SelectCommand.Parameters.AddWithValue("date", dateBox.Text);
                DataTable dtbl = new DataTable();
                sda.Fill(dtbl);
                if(dtbl.Rows.Count != 0)
                {
                    del_msg(1, dtbl.Rows.Count);
                }
                else
                {
                    del_msg(2, 0);
                }
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("tit", titleBox.Text);
                cmd.Parameters.AddWithValue("cat", catBox.Text);
                cmd.Parameters.AddWithValue("qty", qtyBox.Text);
                cmd.Parameters.AddWithValue("date", DT);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Close();
                displayDb();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            titleBox.Text = catBox.Text = qtyBox.Text = dateBox.Text = "";
        }

        DataTable search_dtbl;

        private void button8_Click(object sender, EventArgs e)
        {
            DateTime DT = Convert.ToDateTime("1/1/2001");
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string msg = "SELECT * FROM Products WHERE ";
            bool ok = false;
            if (titleBox.Text != "")
            {
                ok = true;
                msg = msg + "Title = @tit";
            }
            if (catBox.Text != "")
            {
                if (ok)
                {
                    msg = msg + " AND Category = @cat";
                }

                else
                {
                    msg = msg + "Category = @cat";
                }

                ok = true;
            }
            if (qtyBox.Text != "")
            {
                if (ok)
                {
                    msg = msg + " AND Quantity = @qty";
                }
                else
                {
                    msg = msg + "Quantity = @qty";
                }
                ok = true;
            }
            if (dateBox.Text != "")
            {
                try
                {
                    if (comboBox1.SelectedIndex == 1)
                    {
                        DT = Convert.ToDateTime(dateBox.Text, System.Globalization.CultureInfo.GetCultureInfo("fr-FR"));
                    }
                    else
                    {
                        DT = Convert.ToDateTime(dateBox.Text);
                    }
                }
                catch
                {
                    MessageBox.Show("Date format is wrong.\nPlease change the date formatting.", "Database Management", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (ok)
                {
                    msg = msg + " AND ExpirationDate = @date";
                }
                else
                {
                    msg = msg + "ExpirationDate = @date";
                }
            }
            try
            {
                if (titleBox.Text != "" || catBox.Text != "" || qtyBox.Text != "" || dateBox.Text != "")
                {
                    SqlDataAdapter sqlDa = new SqlDataAdapter(msg, con);
                    sqlDa.SelectCommand.Parameters.AddWithValue("tit", titleBox.Text);
                    sqlDa.SelectCommand.Parameters.AddWithValue("cat", catBox.Text);
                    sqlDa.SelectCommand.Parameters.AddWithValue("qty", qtyBox.Text);
                    if (dateBox.Text != "")
                        sqlDa.SelectCommand.Parameters.AddWithValue("date", DT);
                    else
                        sqlDa.SelectCommand.Parameters.AddWithValue("date", dateBox.Text);
                    search_dtbl = new DataTable();
                    sqlDa.Fill(search_dtbl);
                    if (search_dtbl.Rows.Count >= 1)
                    {
                        dataGridView1.AutoGenerateColumns = false;
                        dataGridView1.DataSource = search_dtbl;
                        button10.Visible = true;
                        label8.Text = "Search Results :";
                    }
                    else
                    {
                        del_msg(3, 0);
                        button10.Visible = false;
                        displayDb();
                    }
                }
                else if(titleBox.Text == "" && catBox.Text == "" && qtyBox.Text == "" && dateBox.Text == "")
                {
                    MessageBox.Show("All fields are empty, which is not permitted!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            con.Close();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            button10.Visible = false;
            displayDb();
            label8.Text = "Products :";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string del_comm = "DELETE FROM PRODUCTS WHERE Title = @tit AND Category = @cat AND Quantity = @qty AND ExpirationDate = @date";
            SqlCommand comm;
            try
            {
                for (int i = 0; i < search_dtbl.Rows.Count; i++)
                {
                    comm = new SqlCommand(del_comm, con);
                    comm.Parameters.AddWithValue("tit", search_dtbl.Rows[i]["Title"]);
                    comm.Parameters.AddWithValue("cat", search_dtbl.Rows[i]["Category"]);
                    comm.Parameters.AddWithValue("qty", search_dtbl.Rows[i]["Quantity"]);
                    comm.Parameters.AddWithValue("date", search_dtbl.Rows[i]["ExpirationDate"]);
                    comm.ExecuteNonQuery();
                    comm.Dispose();                    
                }
                button10.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            displayDb();
            del_msg(1, search_dtbl.Rows.Count);
            con.Close();
            label8.Text = "Products :";
        }

        private void button11_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "csv files (*.csv)|*.csv";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    var fileStream = openFileDialog.OpenFile();
                }
            }

            textBox5.Text = filePath;
            loadFrom_csv(filePath);
        }

        private void menuStrip1_ItemClicked_3(object sender, ToolStripItemClickedEventArgs e)
        {
            Form4 newf = new Form4();
            newf.Show();
        }
        private string nume_fisier = "";
        private void button12_Click(object sender, EventArgs e)
        {
            
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            string command;
            if(checkBox3.Checked == true)
            {
                command = "SELECT * FROM Products";
            }
            else
            {
                command = "SELECT Title, Category, Quantity, ExpirationDate FROM Products";
            }
            SqlDataAdapter sda = new SqlDataAdapter(command, con);
            DataTable dt = new DataTable();
            sda.Fill(dt);

            bool ok;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Save file";
            saveFileDialog1.Filter = "csv files (*.csv)|*.csv";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.OverwritePrompt = false;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                nume_fisier = saveFileDialog1.FileName;
                ok = true;
            }
            else
            {
                ok = false;
            }
            if (ok && File.Exists(nume_fisier))
            {
                DialogResult dr = MessageBox.Show("A file with this title already exists in this directory. Do you want to overwrite it?", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (dr == DialogResult.Cancel)
                {
                    ok = false;
                }
            }
            if (ok)
            {
                StreamWriter flux = File.CreateText(nume_fisier);
                if (checkBox3.Checked == true)
                {
                    flux.WriteLine("ID,Title,Category,Quantity,Expiration Date (mm/dd/yyyy)");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        flux.Write(dt.Rows[i][0]);
                        flux.Write(",");
                        flux.Write(dt.Rows[i][1]);
                        flux.Write(",");
                        flux.Write(dt.Rows[i][2]);
                        flux.Write(",");
                        flux.Write(dt.Rows[i][3]);
                        flux.Write(",");
                        DateTime DT = Convert.ToDateTime(dt.Rows[i][4]);
                        string date = DT.ToString("MM-dd-yyyy");
                        flux.Write(date);
                        flux.Write("\n");
                    }
                }
                else
                {
                    flux.WriteLine("Title,Category,Quantity,Expiration Date (mm/dd/yyyy)");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        flux.Write(dt.Rows[i][0]);
                        flux.Write(",");
                        flux.Write(dt.Rows[i][1]);
                        flux.Write(",");
                        flux.Write(dt.Rows[i][2]);
                        flux.Write(",");
                        DateTime DT = Convert.ToDateTime(dt.Rows[i][3]);
                        string date = DT.ToString("MM-dd-yyyy");
                        flux.Write(date);
                        flux.Write("\n");
                    }
                }
                flux.Close();
                //this.richTextBox1.Modified = false;
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime dt = Convert.ToDateTime(dateTimePicker1.Text);
            if(comboBox1.SelectedIndex == 0)
                dateBox.Text = dt.ToString("MM/dd/yyyy"); 
            else if(comboBox1.SelectedIndex == 1)
                dateBox.Text = dt.ToString("dd/MM/yyyy");
            /*else
            {
                dateBox.Text = dt.ToString("MM/dd/yyyy");
                comboBox1.SelectedIndex = 0;
            }*/
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked == true)
            {
                try
                {
                    if (dateBox.Text != string.Empty)
                    {
                        DateTime DT;
                        if (comboBox1.SelectedIndex == 0)
                        {
                            DT = Convert.ToDateTime(dateBox.Text, System.Globalization.CultureInfo.GetCultureInfo("fr-FR"));
                            dateBox.Text = DT.ToString("MM/dd/yyyy");
                        }
                        else
                        {
                            DT = Convert.ToDateTime(dateBox.Text);
                            dateBox.Text = DT.ToString("dd/MM/yyyy");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        int autoDateFormatCounter = 0;

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox4.Checked == true && autoDateFormatCounter == 0)
            {
                MessageBox.Show("Be sure that the correct date format is entered before changing the date format. Otherwise, a conversion error will be displayed, or an incorrect date will be inserted into the table.", "Database Management", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            autoDateFormatCounter++;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox5.Checked)
            {
                textBox5.ReadOnly = true;
                button13.Visible = false;
            }
            else
            {
                textBox5.ReadOnly = false;
                button13.Visible = true;
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            string csv_path = textBox5.Text;
            try
            {
                loadFrom_csv(csv_path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            

        }
    }
}

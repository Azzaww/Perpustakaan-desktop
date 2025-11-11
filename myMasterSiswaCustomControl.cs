using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace desainperpus_fatimah
{
    public partial class myMasterSiswaCustomControl : UserControl
    {

        public SqlConnection connection = new SqlConnection(Koneksi.conn);
        public SqlCommand command, command1;
        public SqlDataAdapter adapter;
        public DataTable tabel;
        public SqlDataReader reader;
        public int id_siswa;
        public int id_user;

        public myMasterSiswaCustomControl()
        {
            InitializeComponent();

            kelas.Text = "Pilih--";
            showData();
        }

        public void showData()
        {
            try
            {
                connection.Close();
                connection.Open();

                string sql = "SELECT siswa.id_siswa, siswa.id_user, siswa.nis, [user].nama, siswa.kelas, " +
                             "siswa.alamat, [user].no_telp, [user].email, [user].username, [user].password FROM siswa " +
                             "INNER JOIN [user] ON siswa.id_user = [user].id_user";

                command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);
                tabel = new DataTable();
                adapter.Fill(tabel);

                dataGridView1.DataSource = tabel;
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[1].Visible = false;
                dataGridView1.Columns[2].HeaderText = "NIS";
                dataGridView1.Columns[3].HeaderText = "Nama";
                dataGridView1.Columns[4].HeaderText = "Kelas";
                dataGridView1.Columns[5].HeaderText = "Alamat";
                dataGridView1.Columns[6].HeaderText = "No Telp";
                dataGridView1.Columns[7].HeaderText = "Email";
                dataGridView1.Columns[8].HeaderText = "Username";
                dataGridView1.Columns[9].HeaderText = "Password";

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error at: " + ex);
            }
        }


        public void clear()
        {
            nis.Text = "";
            nama.Text = "";
            kelas.Text = "Pilih--";
            alamat.Text = "";
            notelp.Text = "";
            email.Text = "";
            username.Text = "";
            password.Text = "";
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // gaa
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.CurrentRow.Selected = true;

            id_siswa = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
            id_user = Convert.ToInt32(dataGridView1.CurrentRow.Cells[1].Value);

            nis.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            nama.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            kelas.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            alamat.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            notelp.Text = dataGridView1.CurrentRow.Cells[6].Value.ToString();
            email.Text = dataGridView1.CurrentRow.Cells[7].Value.ToString();
            username.Text = dataGridView1.CurrentRow.Cells[8].Value.ToString();
            password.Text = dataGridView1.CurrentRow.Cells[9].Value.ToString();
        }

        public void insertData()
        {
            if (!string.IsNullOrEmpty(nis.Text) &&
                !string.IsNullOrEmpty(nama.Text) &&
                !string.IsNullOrEmpty(kelas.Text) && kelas.Text != "Pilih--" &&
                !string.IsNullOrEmpty(alamat.Text) &&
                !string.IsNullOrEmpty(notelp.Text) &&
                !string.IsNullOrEmpty(username.Text))
            {
                try
                {
                    connection.Close();
                    connection.Open();

                    string sql = "INSERT INTO [user] (nama, role, email, no_telp, username, password) " +
                                 "VALUES (@nama, @role, @email, @no_telp, @username, @password)";

                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@nama", nama.Text);
                    command.Parameters.AddWithValue("@role", "siswa");
                    command.Parameters.AddWithValue("@email", email.Text);
                    command.Parameters.AddWithValue("@no_telp", notelp.Text);
                    command.Parameters.AddWithValue("@username", username.Text);
                    command.Parameters.AddWithValue("@password", password.Text);

                    command.ExecuteNonQuery();

                    // Ambil id_user yang baru saja dimasukkan
                    sql = "SELECT id_user FROM [user] WHERE username = '" + username.Text + "' " +
                          "AND password = '" + password.Text + "'";
                    command = new SqlCommand(sql, connection);
                    reader = command.ExecuteReader();
                    reader.Read();
                    int a = reader.GetInt32(0);
                    reader.Close();

                    connection.Close();
                    connection.Open();

                    sql = "INSERT INTO siswa (id_user, nis, kelas, alamat) " +
       "VALUES (@a, @nis, @kelas, @alamat)";

                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@a", a);
                    command.Parameters.AddWithValue("@nis", nis.Text);
                    command.Parameters.AddWithValue("@kelas", kelas.SelectedItem);
                    command.Parameters.AddWithValue("@alamat", alamat.Text);

                    command.ExecuteNonQuery();

                    MessageBox.Show("Successfully inserted!", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    clear();
                    showData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error at: " + ex);
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("Please fill all data before insert!");
            }
        }

        public void editData()
        {
            if (dataGridView1.CurrentRow.Selected)
            {
                if (!string.IsNullOrEmpty(nis.Text) && !string.IsNullOrEmpty(nama.Text)
                    && !string.IsNullOrEmpty(kelas.Text) && kelas.Text != "Pilih--"
                    && !string.IsNullOrEmpty(alamat.Text) && !string.IsNullOrEmpty(notelp.Text)
                    && !string.IsNullOrEmpty(username.Text) && !string.IsNullOrEmpty(password.Text))
                {
                    DialogResult result = MessageBox.Show("Are you sure to update",
                        "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        string sql1 = "UPDATE [user] SET nama = @nama, email = @email, no_telp = @no_telp, " +
                                      "username = @username, password = @password WHERE id_user = " + id_user;

                        string sql2 = "UPDATE siswa SET nis = @nis, kelas = @kelas, alamat = @alamat " +
                                      "WHERE id_siswa = " + id_siswa;

                        command = new SqlCommand(sql1, connection);
                        command.Parameters.AddWithValue("@nama", nama.Text);
                        command.Parameters.AddWithValue("@email", email.Text);
                        command.Parameters.AddWithValue("@no_telp", notelp.Text);
                        command.Parameters.AddWithValue("@username", username.Text);
                        command.Parameters.AddWithValue("@password", password.Text);

                        command1 = new SqlCommand(sql2, connection);
                        command1.Parameters.AddWithValue("@nis", nis.Text);
                        command1.Parameters.AddWithValue("@kelas", kelas.SelectedItem);
                        command1.Parameters.AddWithValue("@alamat", alamat.Text);

                        try
                        {
                            connection.Open();
                            command.ExecuteNonQuery();
                            command1.ExecuteNonQuery();
                            MessageBox.Show("Successfully updated!", "Information", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            clear();
                            showData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please fill all data!");
                }
            }
            else
            {
                MessageBox.Show("Please select the data!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            insertData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            editData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            deleteData();
        }

        public void deleteData()
        {
            if (dataGridView1.CurrentRow.Selected)
            {
                if (!string.IsNullOrEmpty(nis.Text) && !string.IsNullOrEmpty(nama.Text)
                    && !string.IsNullOrEmpty(kelas.Text) && kelas.Text != "Pilih--"
                    && !string.IsNullOrEmpty(alamat.Text) && !string.IsNullOrEmpty(notelp.Text)
                    && !string.IsNullOrEmpty(username.Text) && !string.IsNullOrEmpty(password.Text))
                {
                    DialogResult result = MessageBox.Show("Are you sure to delete?", "Confirmation",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string sql1 = "DELETE FROM siswa WHERE id_siswa = " + id_siswa;
                        string sql2 = "DELETE FROM [user] WHERE id_user = " + id_user;

                        command = new SqlCommand(sql1, connection);
                        command1 = new SqlCommand(sql2, connection);

                        try
                        {
                            connection.Open();
                            command.ExecuteNonQuery();
                            command1.ExecuteNonQuery();
                            MessageBox.Show("Successfully deleted!", "Information", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                            clear();
                            showData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please fill all data!");
                    }
                }
                else
                {
                    MessageBox.Show("Please select the data!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void search_TextChanged(object sender, EventArgs e)
        {
            searchData();
        }

        public void searchData()
        {
            if (!string.IsNullOrEmpty(search.Text))
            {
                try
                {
                    string sql = "SELECT * FROM siswa INNER JOIN [user] ON siswa.id_user = [user].id_user " +
                                 "WHERE nis LIKE '%' + @search + '%' OR nama LIKE '%' + @search + '%'" +
                                 " OR kelas LIKE '%' + @search + '%' OR alamat LIKE '%' + @search + '%'" +
                                 " OR no_telp LIKE '%' + @search + '%' OR email LIKE '%' + @search + '%'" +
                                 " OR username LIKE '%' + @search + '%'" +
                                 " OR password LIKE '%' + @search + '%'";

                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@search", search.Text);
                    adapter = new SqlDataAdapter(command);
                    tabel = new DataTable();
                    adapter.Fill(tabel);

                    dataGridView1.DataSource = tabel;
                    dataGridView1.Columns[0].Visible = false;
                    dataGridView1.Columns[1].Visible = false;
                    dataGridView1.Columns[2].HeaderText = "NIS";
                    dataGridView1.Columns[3].HeaderText = "Nama";
                    dataGridView1.Columns[4].HeaderText = "Kelas";
                    dataGridView1.Columns[5].HeaderText = "Alamat";
                    dataGridView1.Columns[6].HeaderText = "No Telp";
                    dataGridView1.Columns[7].HeaderText = "Email";
                    dataGridView1.Columns[8].HeaderText = "Username";
                    dataGridView1.Columns[9].HeaderText = "Password";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error at: " + ex);
                }
            }
            else
            {
                showData();
            }

        }
    }
}
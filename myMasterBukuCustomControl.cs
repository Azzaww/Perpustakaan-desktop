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

namespace desainperpus_fatimah
{
    public partial class myMasterBukuCustomControl : UserControl

    {
        public SqlConnection connection = new SqlConnection(Koneksi.conn);
        public SqlCommand command;
        public SqlDataAdapter adapter;
        public DataTable tabel;
        public SqlDataReader reader;
        public int id_buku;

        public myMasterBukuCustomControl()
        {
            InitializeComponent();
            showData();
            
        }

        public void showData()
        {
            try
            {
                connection.Close();
                connection.Open();

                string sql = "SELECT * FROM buku";

                command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);
                tabel = new DataTable();
                adapter.Fill(tabel);

                dataGridView1.DataSource = tabel;
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[1].HeaderText = "Judul Buku";
                dataGridView1.Columns[2].HeaderText = "Pengarang";
                dataGridView1.Columns[3].HeaderText = "Penerbit";
                dataGridView1.Columns[4].HeaderText = "Tahun Terbit";
                dataGridView1.Columns[5].HeaderText = "Stok";

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error at: " + ex.Message);
            }
        }

        public void clear()
        {
            judul_buku.Text = "";
            pengarang.Text = "";
            penerbit.Text = "";
            tahun_terbit.Text = "";
            stok.Text = "";
        }

        public void insertData()
        {
            if (!string.IsNullOrEmpty(judul_buku.Text) &&
                !string.IsNullOrEmpty(pengarang.Text) &&
                !string.IsNullOrEmpty(penerbit.Text) &&
                !string.IsNullOrEmpty(tahun_terbit.Text) &&
                !string.IsNullOrEmpty(stok.Text))
            {
                try
                {
                    connection.Close();
                    connection.Open();

                    string sql = "INSERT INTO buku (judul_buku, pengarang, penerbit, tahun_terbit, stok) " +
                                 "VALUES (@judul_buku, @pengarang, @penerbit, @tahun_terbit, @stok)";
                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@judul_buku", judul_buku.Text);
                    command.Parameters.AddWithValue("@pengarang", pengarang.Text);
                    command.Parameters.AddWithValue("@penerbit", penerbit.Text);
                    command.Parameters.AddWithValue("@tahun_terbit", tahun_terbit.Text);
                    command.Parameters.AddWithValue("@stok", stok.Text);

                    command.ExecuteNonQuery();

                    MessageBox.Show("Successfully inserted!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    clear();
                    showData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error at: " + ex.Message);
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

        private void label6_Click(object sender, EventArgs e)
        {
            //kepencet
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.CurrentRow.Selected = true;

            id_buku = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);

            judul_buku.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            pengarang.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            penerbit.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            tahun_terbit.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            stok.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
        }


        public void editData()
        {
            if (dataGridView1.CurrentRow.Selected)
            {
                if (!string.IsNullOrEmpty(judul_buku.Text) &&
                    !string.IsNullOrEmpty(pengarang.Text) &&
                    !string.IsNullOrEmpty(penerbit.Text) &&
                    !string.IsNullOrEmpty(tahun_terbit.Text) &&
                    !string.IsNullOrEmpty(stok.Text))
                {
                    DialogResult result = MessageBox.Show("Are you sure to update?", "Confirmation",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            connection.Open();
                            string sql = "UPDATE buku SET judul_buku = @judul_buku, pengarang = @pengarang, " +
                                         "penerbit = @penerbit, tahun_terbit = @tahun_terbit, stok = @stok " +
                                         "WHERE id_buku = @id_buku";

                            command = new SqlCommand(sql, connection);
                            command.Parameters.AddWithValue("@judul_buku", judul_buku.Text);
                            command.Parameters.AddWithValue("@pengarang", pengarang.Text);
                            command.Parameters.AddWithValue("@penerbit", penerbit.Text);
                            command.Parameters.AddWithValue("@tahun_terbit", tahun_terbit.Text);
                            command.Parameters.AddWithValue("@stok", stok.Text);
                            command.Parameters.AddWithValue("@id_buku", id_buku);

                            command.ExecuteNonQuery();

                            MessageBox.Show("Successfully updated!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            clear();
                            showData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error at: " + ex.Message);
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

        public void deleteData()
        {
            if (dataGridView1.CurrentRow.Selected)
            {
                DialogResult result = MessageBox.Show("Are you sure to delete?", "Confirmation",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        connection.Open();
                        string sql = "DELETE FROM buku WHERE id_buku = @id_buku";
                        command = new SqlCommand(sql, connection);
                        command.Parameters.AddWithValue("@id_buku", id_buku);

                        command.ExecuteNonQuery();

                        MessageBox.Show("Successfully deleted!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        clear();
                        showData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error at: " + ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select data to delete!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        public void searchData()
        {
            if (!string.IsNullOrEmpty(search.Text))
            {
                try
                {
                    string sql = "SELECT * FROM buku WHERE " +
                                 "judul_buku LIKE '%' + @search + '%' OR " +
                                 "pengarang LIKE '%' + @search + '%' OR " +
                                 "penerbit LIKE '%' + @search + '%' OR " +
                                 "CAST(tahun_terbit AS NVARCHAR) LIKE '%' + @search + '%' OR " +
                                 "CAST(stok AS NVARCHAR) LIKE '%' + @search + '%'";

                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@search", search.Text);
                    adapter = new SqlDataAdapter(command);
                    tabel = new DataTable();
                    adapter.Fill(tabel);

                    dataGridView1.DataSource = tabel;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error at: " + ex.Message);
                }
            }
            else
            {
                showData();
            }
        }

        private void search_TextChanged(object sender, EventArgs e)
        {
            searchData();
        }
    }
}

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
    public partial class myPengembalianCustomControl : UserControl
    {
        public SqlConnection connection = new SqlConnection(Koneksi.conn);
        public SqlCommand command;
        public SqlDataAdapter adapter;
        public DataTable tabel;
        public SqlDataReader reader;

        int id_peminjaman = 0;

        public myPengembalianCustomControl()
        {
            InitializeComponent();

            idbuku.Text = "Pilih--";
            // idbuku.DropDownStyle = ComboBoxStyle.DropDownList;


            judulbuku.Enabled = false;
            denda.Enabled = false;

            showPengembalian();
            showPeminjaman();
        }

        public void showPengembalian()
        {
            try
            {
                connection.Close();
                connection.Open();

                string sql = "SELECT * FROM pengembalian";
                command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);
                tabel = new DataTable();
                adapter.Fill(tabel);

                dataGridView2.DataSource = tabel;
                dataGridView2.Columns[0].HeaderText = "ID Pengembalian";
                dataGridView2.Columns[1].HeaderText = "ID Peminjaman";
                dataGridView2.Columns[2].HeaderText = "ID Buku";
                dataGridView2.Columns[3].HeaderText = "Tanggal Kembali";
                dataGridView2.Columns[4].HeaderText = "Denda";

                connection.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error at:" + ex);
            }
        }

        public void showPeminjaman()
        {
            try
            {
                connection.Close();
                connection.Open();

                string sql = "SELECT * FROM peminjaman";
                command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);
                tabel = new DataTable();
                adapter.Fill(tabel);

                dataGridView1.DataSource = tabel;
                dataGridView1.Columns[0].HeaderText = "ID Peminjaman";
                dataGridView1.Columns[1].HeaderText = "ID User";
                dataGridView1.Columns[2].HeaderText = "Tanggal Pinjam";
                dataGridView1.Columns[3].HeaderText = "Tanggal Kembali";
                dataGridView1.Columns[4].HeaderText = "Durasi Pinjam";
                dataGridView1.Columns[5].HeaderText = "Denda";

                connection.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error at:" + ex);
            }
        }

        public void insertPengembalian()
        {
            try
            {
                // Ensure the row is selected
                if (dataGridView1.CurrentRow != null)
                {
                    dataGridView1.CurrentRow.Selected = true; // Select the current row
                    id_peminjaman = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value); // Get id_peminjaman_buku

                    if (!string.IsNullOrEmpty(jumlahkembali.Text) && !string.IsNullOrEmpty(denda.Text))
                    {
                        try
                        {
                            connection.Close();
                            connection.Open();

                            string sql5 = "INSERT INTO pengembalian (id_peminjaman, id_buku, tgl_kembali, denda) " +
                            "VALUES (@idpeminjaman, @idbuku, @tglkembali, @denda)";

                            command = new SqlCommand(sql5, connection);
                            command.Parameters.AddWithValue("@idpeminjaman", id_peminjaman);
                            command.Parameters.AddWithValue("@idbuku", idbuku.SelectedItem?.ToString());
                            command.Parameters.AddWithValue("@tglkembali", dateTimePicker1.Value);
                            command.Parameters.AddWithValue("@denda", denda.Text);

                            command.ExecuteNonQuery();

                            // Now, update the book's stock in the buku table
                            // Increment the stock by the returned amount
                            string sqlUpdateStock = "UPDATE buku SET stok = stok + @jumlahkembali WHERE id_buku = @idbuku";

                            command = new SqlCommand(sqlUpdateStock, connection);
                            command.Parameters.AddWithValue("@jumlahkembali", jumlahkembali.Text); // Increment stock by the returned quantity
                            command.Parameters.AddWithValue("@idbuku", idbuku.SelectedItem);

                            command.ExecuteNonQuery();

                            MessageBox.Show("Successfully inserted!", "Information", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            showPengembalian();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error at:" + ex);
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
                else
                {
                    MessageBox.Show("Please select a valid row from the table.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void showIDBukuDipinjam()
        {
            try
            {
                dataGridView1.CurrentRow.Selected = true;

                id_peminjaman = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);

                connection.Open();  // Buka koneksi

                // Query SQL untuk mengambil data
                string sql = "SELECT id_buku FROM peminjaman_buku WHERE id_peminjaman = @idpeminjaman";
                command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@idpeminjaman", id_peminjaman);

                // Eksekusi query dan ambil data
                reader = command.ExecuteReader();

                // Bersihkan ComboBox sebelum mengisi data
                idbuku.Items.Clear();

                // Mengisi ComboBox dengan data dari database
                while (reader.Read())
                {
                    // Mengambil data dari kolom "nis"
                    idbuku.Items.Add(reader["id_buku"].ToString());
                }

                reader.Close();  // Tutup reader
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error at:" + ex);
            }
            finally
            {
                // Menutup koneksi database setelah selesai
                connection.Close();
            }
        }

        public void showJudulBuku()
        {
            if (idbuku.SelectedItem != null)
            {
                try
                {
        
                        connection.Open(); // Buka koneksi

                    // Query untuk mengambil data berdasarkan id buku
                    string sql = "SELECT judul_buku FROM buku WHERE id_buku = @idbuku";
                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@idbuku", idbuku.SelectedItem);

                    reader = command.ExecuteReader(); // Eksekusi query dan ambil data

                    // Jika data ditemukan, tampilkan di TextBox
                    if (reader.Read())
                    {
                        judulbuku.Text = reader["judul_buku"].ToString();  // Ganti dengan kolom yang sesuai
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    // Menampilkan pesan jika terjadi error
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            insertPengembalian();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            showIDBukuDipinjam();
        }

        private void idbuku_SelectedIndexChanged(object sender, EventArgs e)
        {
           showJudulBuku();
        }

        private void jumlahkembali_SelectedItemChanged(object sender, EventArgs e)
        {
            if (idbuku.Text != "Pilih--" && !string.IsNullOrEmpty(judulbuku.Text))
            {
                if (!string.IsNullOrEmpty(jumlahkembali.Text) && jumlahkembali.Text != "0")
                {
                    dataGridView1.CurrentRow.Selected = true;

                    id_peminjaman = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);

                    string sql = "SELECT jml_pinjam FROM peminjaman_buku WHERE id_peminjaman = @idpeminjaman AND id_buku = @idbuku";
                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@idpeminjaman", id_peminjaman);
                    command.Parameters.AddWithValue("@idbuku", idbuku.SelectedItem);

                    // Execute the query and retrieve the borrowed quantity (jml_pinjam)
                    connection.Open();
                    object result = command.ExecuteScalar();
                    connection.Close();

                    if (result != null)
                    {
                        int jumlahPinjam = Convert.ToInt32(result);
                        int jumlahKembali = Convert.ToInt32(jumlahkembali.Text);

                        // Check if the returned quantity is greater than the borrowed quantity
                        if (jumlahKembali > jumlahPinjam)
                        {
                            MessageBox.Show("Returned quantity cannot be greater than the borrowed quantity.");
                            return; // Stop further processing
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to retrieve borrowed quantity.");
                    }
                }
                else
                {
                    MessageBox.Show("Please fill in the correct data!");
                }
            }
            else
            {
                MessageBox.Show("Please select book borrowing data first!");
            }
        
    }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.CurrentRow.Selected = true;

            id_peminjaman = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);

            string sql = "SELECT tgl_pinjam FROM peminjaman WHERE id_peminjaman = @idpeminjaman";
            command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@idpeminjaman", id_peminjaman);

            // Execute the query and retrieve the borrowed quantity (jml_pinjam)
            connection.Open();
            object result = command.ExecuteScalar();
            connection.Close();

            if (result != null)
            {
                // Convert the result to a DateTime (the borrowed date)
                DateTime tglPinjam = Convert.ToDateTime(result);

                // Compare the return date with the borrowed date
                DateTime tglKembali = dateTimePicker1.Value;

                // Calculate the difference in days between the return date and the borrowed date
                TimeSpan difference = tglKembali - tglPinjam;
                int daysLate = difference.Days;

                // If the return date is more than 7 days after the borrowed date, calculate the fine
                if (daysLate > 7)
                {
                    int fine = (daysLate - 7) * 1000;
                    denda.Text = fine.ToString(); // simpan angka murni
                }
                else
                {
                    denda.Text = "0";
                }

                if (tglKembali < tglPinjam)
                {
                    // Show an error message if the return date is earlier than the borrowed date
                    MessageBox.Show("Tanggal Kembali tidak boleh lebih awal dari Tanggal Pinjam!", "Invalid Return Date", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Optionally, reset the return date to the borrowed date
                    dateTimePicker1.Value = tglPinjam;
                }
            }
            else
            {
                MessageBox.Show("Failed to retrieve the borrowed date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        
    }
        public void searchPeminjaman()
        {
            try
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();

                connection.Open();

                string sql = @"SELECT * FROM peminjaman WHERE 
                       id_peminjaman LIKE '%' + @keyword + '%' OR
                       id_user LIKE '%' + @keyword + '%' OR
                       tgl_pinjam LIKE '%' + @keyword + '%' OR
                       tgl_kembali LIKE '%' + @keyword + '%' OR
                       durasi_pinjam LIKE '%' + @keyword + '%' OR
                       denda LIKE '%' + @keyword + '%'";

                command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@keyword", search.Text);

                adapter = new SqlDataAdapter(command);
                tabel = new DataTable();
                adapter.Fill(tabel);
                dataGridView1.DataSource = tabel;

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan saat pencarian: " + ex.Message);
            }
        }

        private void search_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(search.Text))
                    showPeminjaman();   // tampilkan semua data kalau textbox kosong
                else
                    searchPeminjaman(); // panggil fungsi pencarian
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public void searchPengembalian()
        {
            try
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();

                connection.Open();

                string sql = @"SELECT * FROM pengembalian WHERE 
                       id_pengembalian LIKE '%' + @keyword + '%' OR
                       id_peminjaman LIKE '%' + @keyword + '%' OR
                       id_buku LIKE '%' + @keyword + '%' OR
                       tgl_kembali LIKE '%' + @keyword + '%' OR
                       denda LIKE '%' + @keyword + '%'";

                command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@keyword", cariPengembalian.Text);

                adapter = new SqlDataAdapter(command);
                tabel = new DataTable();
                adapter.Fill(tabel);
                dataGridView2.DataSource = tabel;

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan saat pencarian: " + ex.Message);
            }
        }

        private void cariPengembalian_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cariPengembalian.Text))
                    showPengembalian();   // tampilkan semua data kalau textbox kosong
                else
                    searchPengembalian(); // panggil fungsi pencarian
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}

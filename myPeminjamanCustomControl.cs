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
    public partial class myPeminjamanCustomControl : UserControl
    {
        public SqlConnection connection = new SqlConnection(Koneksi.conn);
        public SqlCommand command;
        public SqlDataAdapter adapter;
        public DataTable tabel;
        public SqlDataReader reader;

        bool isEditing = false;
        int id_peminjaman = 0;

        int id_peminjaman_buku = 0;

        public myPeminjamanCustomControl()
        {
            InitializeComponent();
            showNIS();
            showIDBuku();

            nama.Enabled = false;
            judulbuku.Enabled = false;
            jumlahpinjam.Enabled = false;

            nis.Text = "Pilih--";
            idbuku.Text = "Pilih--";

            showPeminjaman();
        }


        // ==== LOAD NIS KE COMBOBOX ====
        public void showNIS()
        {
            try
            {

                connection.Open(); // buka koneksi

                // Query SQL untuk mengambil data
                string sql = "SELECT nis FROM siswa";
                command = new SqlCommand(sql, connection);

                // Eksekusi query dan ambil data
                reader = command.ExecuteReader();

                // Bersihkan ComboBox sebelum mengisi data
                nis.Items.Clear();

                // Mengisi ComboBox dengan data dari database
                while (reader.Read())
                {
                    // Mengambil data dari kolom "nis"
                    nis.Items.Add(reader["nis"].ToString());
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
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public void showIDBuku()
        {
            try
            {
                connection.Open(); // Buka koneksi

                // Query SQL untuk mengambil data
                string sql = "SELECT id_buku FROM buku";
                command = new SqlCommand(sql, connection);

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

                reader.Close(); // Tutup reader
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
                    command.Parameters.AddWithValue("@idbuku", idbuku.Text);

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

        public void showPeminjaman()
        {
            try
            {
                // Ensure the connection is closed before opening
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
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

        public void showPinjamBuku()
        {
            try
            {
                // Ensure the connection is closed before opening
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                connection.Open();

                string sql1 = "SELECT * FROM peminjaman_buku WHERE id_peminjaman = @idpeminjaman";
                command = new SqlCommand(sql1, connection);
                command.Parameters.AddWithValue("@idpeminjaman", id_peminjaman);
                adapter = new SqlDataAdapter(command);
                tabel = new DataTable();
                adapter.Fill(tabel);

                dataGridView2.DataSource = tabel;
                dataGridView2.Columns[0].HeaderText = "ID Peminjaman Buku";
                dataGridView2.Columns[1].HeaderText = "ID Peminjaman";
                dataGridView2.Columns[2].HeaderText = "ID Buku";
                dataGridView2.Columns[3].HeaderText = "Jumlah Pinjam";

                connection.Close(); // Close after operation

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error at:" + ex);
            }
        }

        public void insertPinjamBuku()
        {
            if (nis.Text != "Pilih--" && !string.IsNullOrEmpty(nama.Text)
                && idbuku.Text != "Pilih--" && !string.IsNullOrEmpty(judulbuku.Text)
                && !string.IsNullOrEmpty(jumlahpinjam.Text) && jumlahpinjam.Text != "0")
            {
                try
                {

                    if (!isEditing)
                    {
                        connection.Open();

                        string sql = "SELECT [user].id_user FROM [user] INNER JOIN siswa ON [user].id_user = siswa.id_user WHERE nis = '" + nis.SelectedItem + "'" +
                            "AND nama = '" + nama.Text + "'";
                        command = new SqlCommand(sql, connection);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            int id_user = reader.GetInt32(0);
                            reader.Close();

                            string tglPinjam = "GETDATE()";
                            string tglKembali = "DATEADD(DAY, 7, GETDATE())";

                            sql = "INSERT INTO peminjaman (id_user, tgl_pinjam, tgl_kembali, durasi_pinjam, denda) " +
                            "VALUES (@iduser, " + tglPinjam + ", " + tglKembali + ", @durasipinjam, @denda) ;" +
                             "SELECT SCOPE_IDENTITY();"; // Ambil id_peminjaman yang baru dimasukkan

                            command = new SqlCommand(sql, connection);
                            command.Parameters.AddWithValue("@iduser", id_user);
                            command.Parameters.AddWithValue("@durasipinjam", 7);
                            command.Parameters.AddWithValue("@denda", 0);

                            id_peminjaman = Convert.ToInt32(command.ExecuteScalar());
                        }
                        else
                        {
                            MessageBox.Show("User not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    // Menambahkan buku yang dipinjam
                    string[] bukuIds = idbuku.Text.Split(','); // Misalnya buku dipilih lebih dari 1, dan dipisah dengan koma
                    foreach (string bukuId in bukuIds)
                    {
                        if (connection.State == ConnectionState.Closed) { connection.Open(); }

                        string sql = "INSERT INTO peminjaman_buku (id_peminjaman, id_buku, jml_pinjam) " +
                        "VALUES (@idpeminjaman, @idbuku, @jumlahpinjam)";

                        command = new SqlCommand(sql, connection);
                        command.Parameters.AddWithValue("@idpeminjaman", id_peminjaman);
                        command.Parameters.AddWithValue("@idbuku", bukuId);
                        command.Parameters.AddWithValue("@jumlahpinjam", jumlahpinjam.Text);

                        command.ExecuteNonQuery();

                        // Reduce the stock in the buku table
                        sql = "UPDATE buku SET stok = stok - @jumlahpinjam WHERE id_buku = @idbuku";
                        command = new SqlCommand(sql, connection);
                        command.Parameters.AddWithValue("@jumlahpinjam", jumlahpinjam.Text);
                        command.Parameters.AddWithValue("@idbuku", bukuId.Trim()); // Ensure no extra spaces

                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Successfully inserted!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    nis.Enabled = false;
                    isEditing = true;
                    idbuku.Text = "Pilih--";
                    judulbuku.Text = "";
                    jumlahpinjam.Text = "";

                    showPinjamBuku();
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

        public void editPinjamBuku()
        {
            if (dataGridView2.CurrentRow.Selected)
            {
                if (idbuku.Text != "Pilih--" && !string.IsNullOrEmpty(judulbuku.Text)
                    && !string.IsNullOrEmpty(jumlahpinjam.Text) && jumlahpinjam.Text != "0")
                {
                    DialogResult result = MessageBox.Show("Are you sure to update", "Confirmation",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string sql = "SELECT id_buku, jml_pinjam FROM peminjaman_buku WHERE id_peminjaman_buku = @idpeminjaman_buku";
                        command = new SqlCommand(sql, connection);
                        command.Parameters.AddWithValue("@idpeminjaman_buku", id_peminjaman_buku);

                        connection.Open();
                        reader = command.ExecuteReader();
                        int oldBookId = 0;
                        int oldBorrowedQuantity = 0;
                        if (reader.Read())
                        {
                            oldBookId = reader.GetInt32(0);
                            oldBorrowedQuantity = reader.GetInt32(1);
                        }
                        reader.Close();

                        // Step 2: Calculate the difference in borrowed quantity
                        int newBorrowedQuantity = Convert.ToInt32(jumlahpinjam.Text);
                        int quantityDifference = newBorrowedQuantity - oldBorrowedQuantity;

                        // Step 3: Update the stok in the buku table
                        if (quantityDifference != 0)
                        {
                            // First, update the stock of the old book
                            string updateOldBookStockSql = "UPDATE buku SET stok = stok + @quantityDifference WHERE id_buku = @oldBookId";
                            command = new SqlCommand(updateOldBookStockSql, connection);
                            command.Parameters.AddWithValue("@quantityDifference", oldBorrowedQuantity);
                            command.Parameters.AddWithValue("@oldBookId", oldBookId);
                            command.ExecuteNonQuery();

                            // Then, update the stock of the new book (if changed)
                            if (idbuku.SelectedItem.ToString() != oldBookId.ToString())
                            {
                                string updateNewBookStockSql = "UPDATE buku SET stok = stok - @newBorrowedQuantity WHERE id_buku = @newBookId";
                                command = new SqlCommand(updateNewBookStockSql, connection);
                                command.Parameters.AddWithValue("@newBorrowedQuantity", newBorrowedQuantity);
                                command.Parameters.AddWithValue("@newBookId", idbuku.SelectedItem);
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                // Update the same book stock if not changed
                                string updateSameBookStockSql = "UPDATE buku SET stok = stok - @newBorrowedQuantity WHERE id_buku = @newBookId";
                                command = new SqlCommand(updateSameBookStockSql, connection);
                                command.Parameters.AddWithValue("@newBorrowedQuantity", newBorrowedQuantity);
                                command.Parameters.AddWithValue("@newBookId", idbuku.SelectedItem);
                                command.ExecuteNonQuery();
                            }
                        }

                        sql = "UPDATE peminjaman_buku SET id_buku = @idbuku, jml_pinjam = @jumlahpinjam WHERE id_peminjaman_buku = " + id_peminjaman_buku;

                        command = new SqlCommand(sql, connection);
                        command.Parameters.AddWithValue("@idbuku", idbuku.SelectedItem);
                        command.Parameters.AddWithValue("@jumlahpinjam", jumlahpinjam.Text);

                        try
                        {
                            connection.Open();
                            command.ExecuteNonQuery();
                            MessageBox.Show("Successfully updated!", "Information", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            nis.Enabled = false;
                            isEditing = true;
                            idbuku.Text = "Pilih--";
                            judulbuku.Text = "";
                            jumlahpinjam.Text = "";

                            //showPinjamBuku();

                            sql = "SELECT * FROM peminjaman_buku WHERE id_peminjaman = @idpeminjaman";

                            command = new SqlCommand(sql, connection);
                            command.Parameters.AddWithValue("@idpeminjaman", id_peminjaman);
                            adapter = new SqlDataAdapter(command);
                            tabel = new DataTable();
                            adapter.Fill(tabel);

                            dataGridView2.DataSource = tabel;
                            dataGridView2.Columns[0].HeaderText = "ID Peminjaman Buku";
                            dataGridView2.Columns[1].HeaderText = "ID Peminjaman";
                            dataGridView2.Columns[2].HeaderText = "ID Buku";
                            dataGridView2.Columns[3].HeaderText = "Jumlah Pinjam";
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

        public void deletePinjamBuku()
        {
            if (dataGridView2.CurrentRow.Selected)
            {
                if (idbuku.Text != "Pilih--" && !string.IsNullOrEmpty(judulbuku.Text)
                    && !string.IsNullOrEmpty(jumlahpinjam.Text) && jumlahpinjam.Text != "0")
                {
                    DialogResult result = MessageBox.Show("Are you sure to delete?", "Confirmation",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            // Step 1: Retrieve the id_buku and jml_pinjam for the selected record before deletion
                            string sql = "SELECT id_buku, jml_pinjam FROM peminjaman_buku WHERE id_peminjaman_buku = @id_peminjaman_buku";
                            command = new SqlCommand(sql, connection);
                            command.Parameters.AddWithValue("@id_peminjaman_buku", id_peminjaman_buku);
                            connection.Open();
                            reader = command.ExecuteReader();

                            int idBukuToDelete = 0;
                            int jumlahPinjamToDelete = 0;

                            if (reader.Read())
                            {
                                idBukuToDelete = reader.GetInt32(0);
                                jumlahPinjamToDelete = reader.GetInt32(1);
                            }
                            reader.Close();

                            // Step 2: Update the stock in the buku table by adding back the borrowed quantity
                            if (idBukuToDelete != 0)
                            {
                                string updateStockSql = "UPDATE buku SET stok = stok + @jumlahPinjamToDelete WHERE id_buku = @idBukuToDelete";
                                command = new SqlCommand(updateStockSql, connection);
                                command.Parameters.AddWithValue("@jumlahPinjamToDelete", jumlahPinjamToDelete);
                                command.Parameters.AddWithValue("@idBukuToDelete", idBukuToDelete);
                                command.ExecuteNonQuery();
                            }

                            // Step 3: Delete the record from peminjaman_buku
                            sql = "DELETE FROM peminjaman_buku WHERE id_peminjaman_buku = @id_peminjaman_buku";
                            command = new SqlCommand(sql, connection);
                            command.Parameters.AddWithValue("@id_peminjaman_buku", id_peminjaman_buku);
                            command.ExecuteNonQuery();

                            // Step 4: Check if there are any remaining books for this peminjaman record
                            sql = "SELECT COUNT(*) FROM peminjaman_buku WHERE id_peminjaman = @idpeminjaman";
                            command = new SqlCommand(sql, connection);
                            command.Parameters.AddWithValue("@idpeminjaman", id_peminjaman);
                            int remainingBooks = (int)command.ExecuteScalar();

                            // Step 5: If no books remain for the peminjaman, delete the peminjaman record
                            if (remainingBooks == 0)
                            {
                                sql = "DELETE FROM peminjaman WHERE id_peminjaman = @idpeminjaman";
                                command = new SqlCommand(sql, connection);
                                command.Parameters.AddWithValue("@idpeminjaman", id_peminjaman);
                                command.ExecuteNonQuery();
                            }

                            MessageBox.Show("Successfully deleted!", "Information", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            // Step 6: Clear the form
                            nis.Enabled = false;
                            isEditing = true;
                            idbuku.Text = "Pilih--";
                            judulbuku.Text = "";
                            jumlahpinjam.Text = "";

                            // Step 7: Refresh the data grid to show the updated list of borrowed books
                            sql = "SELECT * FROM peminjaman_buku WHERE id_peminjaman = @idpeminjaman";
                            command = new SqlCommand(sql, connection);
                            command.Parameters.AddWithValue("@idpeminjaman", id_peminjaman);
                            adapter = new SqlDataAdapter(command);
                            tabel = new DataTable();
                            adapter.Fill(tabel);

                            // Display the updated data in dataGridView2
                            dataGridView2.DataSource = tabel;
                            dataGridView2.Columns[0].HeaderText = "ID Peminjaman Buku";
                            dataGridView2.Columns[1].HeaderText = "ID Peminjaman";
                            dataGridView2.Columns[2].HeaderText = "ID Buku";
                            dataGridView2.Columns[3].HeaderText = "Jumlah Pinjam";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please fill all data!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select the data!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void insertPeminjaman()
        {
            if (nis.Text != "Pilih--" && !string.IsNullOrEmpty(nama.Text)
                && idbuku.Text != "Pilih--" && !string.IsNullOrEmpty(judulbuku.Text)
                && !string.IsNullOrEmpty(jumlahpinjam.Text) && jumlahpinjam.Text != "0")
            {
                try
                {
                    connection.Close();
                    connection.Open();

                    string sql = "SELECT [user].id_user FROM [user] INNER JOIN siswa ON [user].id_user = siswa.id_user WHERE nis = '" + nis.SelectedItem + "'" +
                        "AND nama = '" + nama.Text + "'";
                    command = new SqlCommand(sql, connection);

                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        int a = reader.GetInt32(0);
                        reader.Close();

                        string tglPinjam = "GETDATE()";
                        string tglKembali = "DATEADD(DAY, 7, GETDATE())";

                        sql = "INSERT INTO peminjaman (id_user, tgl_pinjam, tgl_kembali, durasi_pinjam, denda) " +
                        "VALUES (@a, " + tglPinjam + ", " + tglKembali + ", @durasipinjam, @denda) ;" +
                         "SELECT SCOPE_IDENTITY();"; // Ambil id_peminjaman yang baru dimasukkan

                        command = new SqlCommand(sql, connection);
                        command.Parameters.AddWithValue("@a", a);
                        command.Parameters.AddWithValue("@durasipinjam", 7);
                        command.Parameters.AddWithValue("@denda", 0);

                        int b = Convert.ToInt32(command.ExecuteScalar());

                        // Menambahkan buku yang dipinjam
                        string[] bukuIds = idbuku.Text.Split(','); // Misalnya buku dipilih lebih dari 1, dan dipisah dengan koma
                        foreach (string bukuId in bukuIds)
                        {

                            sql = "INSERT INTO peminjaman_buku (id_peminjaman, id_buku, jml_pinjam) " +
                            "VALUES (@b, @idbuku, @jumlahpinjam)";

                            command = new SqlCommand(sql, connection);
                            command.Parameters.AddWithValue("@b", b);
                            command.Parameters.AddWithValue("@idbuku", bukuId);
                            command.Parameters.AddWithValue("@jumlahpinjam", jumlahpinjam.Text);

                            command.ExecuteNonQuery();
                        }
                        MessageBox.Show("Successfully inserted!", "Information", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        nis.Enabled = false;
                        idbuku.Text = "Pilih--";
                        judulbuku.Text = "";
                        jumlahpinjam.Text = "";


                    }
                    else
                    {
                        MessageBox.Show("User not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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


        // ==== INSERT DATA KE PEMINJAMAN + PEMINJAMAN_BUKU ====
        private void btInsert_Click(object sender, EventArgs e)
        {
            insertPinjamBuku();
            save.Enabled = true;
        }

        // ==== EDIT DATA ====
        private void btEdit_Click(object sender, EventArgs e)
        {
            editPinjamBuku();
            btInsert.Enabled = true;
        }

        // ==== DELETE DATA ====
        private void btDelete_Click(object sender, EventArgs e)
        {
            deletePinjamBuku();
            save.Enabled = true;
        }

        // ==== SEARCH DATA ====
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

        private void label3_Click(object sender, EventArgs e)
        { 
            // Tidak digunakan 
        }

        private void nis_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (nis.SelectedItem != null)
            {
                try
                {
                    connection.Open(); // Buka koneksi

                    // Query untuk mengambil data berdasarkan nis
                    string sql = "SELECT nama FROM [user] INNER JOIN siswa ON [user].id_user = siswa.id_user " +
                        "WHERE nis = @nis";  // Ganti dengan kolom yang sesuai
                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@nis", nis.Text);

                    reader = command.ExecuteReader(); // Eksekusi query dan ambil data

                    // Jika data ditemukan, tampilkan di TextBox
                    if (reader.Read())
                    {
                        nama.Text = reader["nama"].ToString();  // Ganti dengan kolom yang sesuai
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

        private void idbuku_SelectedIndexChanged(object sender, EventArgs e)
        {
            showJudulBuku();
            jumlahpinjam.Enabled = true;
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Ensure the row is selected
                if (dataGridView2.CurrentRow != null)
                {
                    btInsert.Enabled = false;

                    dataGridView2.CurrentRow.Selected = true; // Select the current row
                    id_peminjaman_buku = Convert.ToInt32(dataGridView2.CurrentRow.Cells[0].Value); // Get id_peminjaman_buku
                    idbuku.Text = dataGridView2.CurrentRow.Cells[2].Value.ToString(); // Get id_buku
                    showJudulBuku(); // Show the title of the selected book
                    jumlahpinjam.Text = dataGridView2.CurrentRow.Cells[3].Value.ToString(); // Get jumlah pinjam

                    // Get the id_peminjaman from the current row
                    int idPeminjaman = Convert.ToInt32(dataGridView2.CurrentRow.Cells[1].Value); // Assuming this column holds id_peminjaman

                    // Now, query the peminjaman table to get the id_user associated with this id_peminjaman
                    string sql = "SELECT id_user FROM peminjaman WHERE id_peminjaman = @idPeminjaman";
                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@idPeminjaman", idPeminjaman);

                    // Open connection and retrieve id_user
                    connection.Open();
                    object result = command.ExecuteScalar();
                    connection.Close();

                    if (result != null)
                    {
                        int idUser = Convert.ToInt32(result); // Retrieve the id_user

                        // Query the siswa table to get the nis based on id_user
                        sql = "SELECT nis FROM siswa WHERE id_user = @idUser";
                        command = new SqlCommand(sql, connection);
                        command.Parameters.AddWithValue("@idUser", idUser);

                        connection.Open();
                        object nisResult = command.ExecuteScalar();
                        connection.Close();

                        if (nisResult != null)
                        {
                            string NIS = nisResult.ToString(); // Get the nis value
                                                               // Now, populate the nis into the ComboBox
                            nis.SelectedItem = NIS; // Assuming your ComboBox is named nisComboBox
                        }
                        else
                        {
                            MessageBox.Show("NIS not found for the selected user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("User not found for the selected peminjaman.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void save_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure to save?", "Confirmation",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {


                try
                {
                    // If dataGridView2 is bound to a DataTable
                    if (dataGridView2.DataSource is DataTable dt)
                    {
                        dt.Clear();  // Clears all rows from the DataTable
                    }
                    else
                    {
                        // Alternatively, clear rows if not using data binding
                        dataGridView2.Rows.Clear();
                    }

                    // Reset other form controls
                    nis.Enabled = true;
                    nis.Text = "Pilih--";
                    nama.Text = "";
                    idbuku.Text = "Pilih--";
                    judulbuku.Text = "";
                    jumlahpinjam.Text = "";

                    // Call your other reset methods (e.g., showPeminjaman())
                    showPeminjaman();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.CurrentRow.Selected = true;

            id_peminjaman = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);

            string sql = "SELECT * FROM peminjaman_buku WHERE id_peminjaman = @idpeminjaman";

            command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@idpeminjaman", id_peminjaman);
            adapter = new SqlDataAdapter(command);
            tabel = new DataTable();
            adapter.Fill(tabel);

            dataGridView2.DataSource = tabel;
            dataGridView2.Columns[0].HeaderText = "ID Peminjaman Buku";
            dataGridView2.Columns[1].HeaderText = "ID Peminjaman";
            dataGridView2.Columns[2].HeaderText = "ID Buku";
            dataGridView2.Columns[3].HeaderText = "Jumlah Pinjam";
        }

        private void jumlahpinjam_SelectedItemChanged(object sender, EventArgs e)
        {
            try
            {
                // Fetch stock data from the buku table
                string sql = "SELECT stok FROM buku WHERE id_buku = @idbuku";
                command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@idbuku", idbuku.SelectedItem);

                connection.Open();
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    int stok = Convert.ToInt32(result);  // Get the available stock
                    int jumlahPinjam = Convert.ToInt32(jumlahpinjam.Text);  // Get the requested quantity

                    // Check if the requested quantity exceeds the available stock
                    if (jumlahPinjam > stok)
                    {
                        MessageBox.Show("Jumlah pinjam tidak boleh melebihi stok.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // Stop further processing
                    }
                }
                else
                {
                    MessageBox.Show("Failed to retrieve stock data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Ensure that the connection is always closed
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        }
    }


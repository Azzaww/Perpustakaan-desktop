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
    public partial class HistoryPeminjaman : UserControl
    {
        public SqlConnection connection = new SqlConnection(Koneksi.conn);
        public SqlCommand command;
        public SqlDataAdapter adapter;
        public DataTable tabel;
        public SqlDataReader reader;

        public HistoryPeminjaman()
        {
            InitializeComponent();
            showPeminjaman();
        }


        public void showPeminjaman()
        {
            try
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();

                connection.Open();

                string sql;
                if (sessions.Role == "siswa")
                {
                    sql = "SELECT * FROM peminjaman WHERE id_user = @idUser";
                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@idUser", sessions.UserID);
                }

                adapter = new SqlDataAdapter(command);
                tabel = new DataTable();
                adapter.Fill(tabel);

                dataGridView1.DataSource = tabel;

                // Pastikan jumlah kolom sesuai; hindari exception jika struktur berbeda
                if (dataGridView1.Columns.Count >= 6)
                {
                    dataGridView1.Columns[0].HeaderText = "ID Peminjaman";
                    dataGridView1.Columns[1].HeaderText = "ID User";
                    dataGridView1.Columns[2].HeaderText = "Tanggal Pinjam";
                    dataGridView1.Columns[3].HeaderText = "Tanggal Kembali";
                    dataGridView1.Columns[4].HeaderText = "Durasi Pinjam";
                    dataGridView1.Columns[5].HeaderText = "Denda";
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi error: " + ex.Message);
            }
        }

        public void searchPeminjaman()
        {
            try
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();

                connection.Open();

                string sql;

                if (sessions.Role == "siswa")
                {
                    sql = "SELECT * FROM peminjaman WHERE id_user = @idUser AND " +
                          "(CAST(id_peminjaman AS NVARCHAR) LIKE '%' + @search + '%' OR " +
                          "CAST(tgl_pinjam AS NVARCHAR) LIKE '%' + @search + '%' OR " +
                          "CAST(tgl_kembali AS NVARCHAR) LIKE '%' + @search + '%' OR " +
                          "CAST(durasi_pinjam AS NVARCHAR) LIKE '%' + @search + '%' OR " +
                          "CAST(denda AS NVARCHAR) LIKE '%' + @search + '%')";
                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@idUser", sessions.UserID);
                }
                else
                {
                    sql = "SELECT * FROM peminjaman WHERE " +
                          "CAST(id_peminjaman AS NVARCHAR) LIKE '%' + @search + '%' OR " +
                          "CAST(id_user AS NVARCHAR) LIKE '%' + @search + '%' OR " +
                          "CAST(tgl_pinjam AS NVARCHAR) LIKE '%' + @search + '%' OR " +
                          "CAST(tgl_kembali AS NVARCHAR) LIKE '%' + @search + '%' OR " +
                          "CAST(durasi_pinjam AS NVARCHAR) LIKE '%' + @search + '%' OR " +
                          "CAST(denda AS NVARCHAR) LIKE '%' + @search + '%'";
                    command = new SqlCommand(sql, connection);
                }

                command.Parameters.AddWithValue("@search", search.Text);

                adapter = new SqlDataAdapter(command);
                tabel = new DataTable();
                adapter.Fill(tabel);
                dataGridView1.DataSource = tabel;

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat mencari data: " + ex.Message);
            }
        }

        private void search_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(search.Text))
                showPeminjaman();
            else
                searchPeminjaman();
        }
    }
}

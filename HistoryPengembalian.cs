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
    public partial class HistoryPengembalian : UserControl
    {
        public SqlConnection connection = new SqlConnection(Koneksi.conn);
        public SqlCommand command;
        public SqlDataAdapter adapter;
        public DataTable tabel;
        public SqlDataReader reader;
        public HistoryPengembalian()
        {
            InitializeComponent();
            showPengembalian();
        }

        public void showPengembalian()
        {
            try
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();

                connection.Open();

                string sql;

                // Jika role adalah siswa, tampilkan hanya data pengembalian miliknya
                if (sessions.Role == "siswa")
                {
                    sql = @"SELECT p.id_pengembalian, p.id_peminjaman, p.id_buku, p.tgl_kembali, p.denda 
                            FROM pengembalian p
                            JOIN peminjaman pm ON p.id_peminjaman = pm.id_peminjaman
                            WHERE pm.id_user = @idUser";
                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@idUser", sessions.UserID);
                }

                adapter = new SqlDataAdapter(command);
                tabel = new DataTable();
                adapter.Fill(tabel);

                dataGridView1.DataSource = tabel;
                dataGridView1.Columns[0].HeaderText = "ID Pengembalian";
                dataGridView1.Columns[1].HeaderText = "ID Peminjaman";
                dataGridView1.Columns[2].HeaderText = "ID Buku";
                dataGridView1.Columns[3].HeaderText = "Tanggal Kembali";
                dataGridView1.Columns[4].HeaderText = "Denda";

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error at: " + ex.Message);
            }
        }

        public void searchPengembalian()
        {
            try
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();

                connection.Open();

                string sql;

                if (sessions.Role == "siswa")
                {
                    sql = @"SELECT p.id_pengembalian, p.id_peminjaman, p.id_buku, p.tgl_kembali, p.denda 
                            FROM pengembalian p
                            JOIN peminjaman pm ON p.id_peminjaman = pm.id_peminjaman
                            WHERE pm.id_user = @idUser AND
                            (CAST(p.id_pengembalian AS NVARCHAR) LIKE '%' + @search + '%' OR
                             CAST(p.id_peminjaman AS NVARCHAR) LIKE '%' + @search + '%' OR
                             CAST(p.id_buku AS NVARCHAR) LIKE '%' + @search + '%' OR
                             CAST(p.tgl_kembali AS NVARCHAR) LIKE '%' + @search + '%' OR
                             CAST(p.denda AS NVARCHAR) LIKE '%' + @search + '%')";
                    command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@idUser", sessions.UserID);
                }
                else
                {
                    sql = @"SELECT p.id_pengembalian, p.id_peminjaman, p.id_buku, p.tgl_kembali, p.denda 
                            FROM pengembalian p
                            JOIN peminjaman pm ON p.id_peminjaman = pm.id_peminjaman
                            WHERE (CAST(p.id_pengembalian AS NVARCHAR) LIKE '%' + @search + '%' OR
                                   CAST(p.id_peminjaman AS NVARCHAR) LIKE '%' + @search + '%' OR
                                   CAST(p.id_buku AS NVARCHAR) LIKE '%' + @search + '%' OR
                                   CAST(p.tgl_kembali AS NVARCHAR) LIKE '%' + @search + '%' OR
                                   CAST(p.denda AS NVARCHAR) LIKE '%' + @search + '%')";
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
                showPengembalian();
            else
                searchPengembalian();
        }
    }
}

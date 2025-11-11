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
using System.Windows.Forms.DataVisualization.Charting;

namespace desainperpus_fatimah
{
    public partial class myDashboardCustomControl : UserControl
    {
        public SqlConnection connection = new SqlConnection(Koneksi.conn);
        public SqlCommand command;
        public SqlDataAdapter adapter;
        public DataTable tabel;
        public SqlDataReader reader;
        private Timer refreshTimer; // Tambahkan timer
        public myDashboardCustomControl()
        {
            InitializeComponent();
        }

        private void TampilJumlahSiswa()
        {
            try
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM siswa"; // ubah sesuai nama tabelmu
                command = new SqlCommand(query, connection);
                int jumlahSiswa = (int)command.ExecuteScalar();
                label1.Text = jumlahSiswa.ToString();
            }
            catch
            {
                // Kosongkan biar tidak ganggu tampilan saat koneksi belum siap
            }
            finally
            {
                connection.Close();
            }
        }


        private void TampilJumlahBuku()
        {
            try
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM buku"; // ubah sesuai nama tabelmu
                command = new SqlCommand(query, connection);
                int jumlahBuku = (int)command.ExecuteScalar();
                label4.Text = jumlahBuku.ToString();
            }
            catch
            {
                // Kosongkan biar tidak ganggu tampilan saat koneksi belum siap
            }
            finally
            {
                connection.Close();
            }
        }

        private void TampilGrafikPeminjaman()
        {
            try
            {
                connection.Open();

                string query = @"
                    SELECT 
                        DATENAME(MONTH, p.tgl_pinjam) AS Bulan,
                        COUNT(pb.id_buku) AS JumlahPeminjaman
                    FROM peminjaman p
                    JOIN peminjaman_buku pb ON p.id_peminjaman = pb.id_peminjaman
                    WHERE YEAR(p.tgl_pinjam) = YEAR(GETDATE())
                    GROUP BY DATENAME(MONTH, p.tgl_pinjam), MONTH(p.tgl_pinjam)
                    ORDER BY MONTH(p.tgl_pinjam)
                ";

                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();

                chartPeminjaman.Series.Clear();
                chartPeminjaman.Titles.Clear();
                chartPeminjaman.Titles.Add("Grafik Peminjaman Buku per Bulan");

                Series series = new Series("Jumlah Peminjaman");
                series.ChartType = SeriesChartType.Column;
                series.IsValueShownAsLabel = true; // tampilkan nilai di atas batang

                while (reader.Read())
                {
                    series.Points.AddXY(reader["Bulan"].ToString(), Convert.ToInt32(reader["JumlahPeminjaman"]));
                }

                chartPeminjaman.Series.Add(series);
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void myDashboardCustomControl_Load(object sender, EventArgs e)
        {
            TampilJumlahSiswa();
            TampilJumlahBuku();
            TampilGrafikPeminjaman();

            // Inisialisasi timer auto-refresh
            refreshTimer = new Timer();
            refreshTimer.Interval = 1000; // 1000ms = 1 detik
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start(); // mulai timer
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            // Timer akan memanggil ulang fungsi untuk update label
            TampilJumlahSiswa();
            TampilJumlahBuku();
            TampilGrafikPeminjaman();
        }
    }
}

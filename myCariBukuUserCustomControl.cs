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
    public partial class myCariBukuUserCustomControl : UserControl
    {

        public SqlConnection connection = new SqlConnection(Koneksi.conn);
        public SqlCommand command;
        public SqlDataAdapter adapter;
        public DataTable tabel;
        public SqlDataReader reader;
        public int id_buku;

        public myCariBukuUserCustomControl()
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

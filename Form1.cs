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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace desainperpus_fatimah
{
    public partial class Form1 : Form
    {

        public SqlConnection connection = new SqlConnection(Koneksi.conn);
        public SqlCommand command;
        public SqlDataAdapter adapter;
        public DataTable tabel;
        public SqlDataReader reader;

        // Menyimpan teks captcha saat ini
        private string captchaText = "";

        public Form1()
        {
            InitializeComponent();
            // generate captcha saat form dibuka
            GenerateCaptcha();
        }

        private void bSignIn_Click(object sender, EventArgs e)
        {
            // CEK CAPTCHA DULU
            if (textBoxCaptcha.Text.Trim() != captchaText)
            {
                MessageBox.Show("Captcha salah! Silakan coba lagi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                GenerateCaptcha(); // buat captcha baru
                textBoxCaptcha.Clear(); // kosongkan input captcha
                return; // hentikan proses login
            }

            connection.Open();

            string sql = "SELECT * FROM [user] WHERE username = '" + username.Text + "' AND password = '" + password.Text + "'";
            command = new SqlCommand(sql, connection);
            adapter = new SqlDataAdapter(command);
            tabel = new DataTable();
            adapter.Fill(tabel);

            if (tabel.Rows.Count > 0)
            {
                foreach (DataRow dr in tabel.Rows)
                {
                    // Simpan data user yang login ke sessions
                    sessions.UserID = Convert.ToInt32(dr["id_user"]);
                    sessions.Username = dr["username"].ToString();
                    sessions.Role = dr["role"].ToString();
                    sessions.Name = dr["nama"].ToString();

                    if (dr["role"].ToString() == "admin")
                    {

                        reader = command.ExecuteReader();
                        reader.Read();
                        Model.name = reader.GetString(1);

                        this.Hide();
                        Form2 panggil = new Form2();
                        panggil.Show();
                    }
                    else if (dr["role"].ToString() == "siswa")
                    {

                        reader = command.ExecuteReader();
                        reader.Read();
                        Model.name = reader.GetString(1);

                        this.Hide();
                        Form3 panggil = new Form3();
                        panggil.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid Login please check username and password");
            }

            connection.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // 🧩 FUNGSI UNTUK MEMBUAT CAPTCHA DALAM BENTUK TEKS (Label)
        private void GenerateCaptcha()
        {
            // Buat objek Random untuk menghasilkan huruf acak
            Random rand = new Random();

            // Karakter yang mungkin digunakan (tanpa O/I dan 0/1)
            string chars = "ABCDEFGHJKMNPQRSTUVWXYZ23456789abcdefghjklmnpqrstuvwxyz";

            // Kosongkan dulu captcha lama
            captchaText = "";

            // Loop sebanyak 5 huruf untuk membentuk captcha
            for (int i = 0; i < 5; i++)
            {
                captchaText += chars[rand.Next(chars.Length)];
            }

            // Tampilkan captcha ke Label
            labelCaptcha.Text = captchaText;

            
        }


        private void btnRefreshCaptcha_Click(object sender, EventArgs e)
        {
            GenerateCaptcha();
            textBoxCaptcha.Clear();
        }
    }
}

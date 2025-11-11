using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace desainperpus_fatimah
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            label2.Text = Model.name;
            SidePanel.Height = button1.Height;
            SidePanel.Top = button1.Top;
            myCariBukuUserCustomControl1.BringToFront();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SidePanel.Height = button2.Height;
            SidePanel.Top = button2.Top;
            historyPeminjaman1.BringToFront();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SidePanel.Height = button1.Height;
            SidePanel.Top = button1.Top;
            myCariBukuUserCustomControl1.BringToFront();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SidePanel.Height = button3.Height;
            SidePanel.Top = button3.Top;
            historyPengembalian1.BringToFront();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Tampilkan form login
            Form1 login = new Form1();
            login.Show();

            // Tutup form utama (misal FormAdmin)
            this.Hide();
        }
    }
}

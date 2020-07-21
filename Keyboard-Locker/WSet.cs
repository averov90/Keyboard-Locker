using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace KeyboardLock
{
    public partial class WSet : Form
    {
        public WSet()
        {
            InitializeComponent();
        }
        public bool write;
        private void WSet_Load(object sender, EventArgs e)
        {
            label1.Text = (write ? "Запись: разрешена" : "Запись: запрещена");
            button1.Text = (write ? "Запретить" : "Разрешить");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            write = !write;
            Close();
        }
    }
}

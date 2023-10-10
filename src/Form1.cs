using System;
using System.Windows.Forms;

namespace Compressor
{
    public partial class Form1 : Form
    {
        private string[] _datasets;

        public Form1()
        {
            InitializeComponent();
            progressBar1.Value = 0;
            progressBar1.Maximum = 100;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;
                dialog.ShowDialog();
                _datasets = dialog.FileNames;
                textBox1_TextChanged(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show((ex.ToString()));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = "In Progress";
            progressBar1.Value = 0;

            switch (comboBox1.Text)
            {
                case "Decode":
                    Compressor.Decode(_datasets);
                    progressBar1.Value = 100;
                    break;
                case "Rename":
                    Compressor.Rename(_datasets);
                    progressBar1.Value = 100;
                    break;
                case "Compress":
                    Compressor.Compress(_datasets);
                    progressBar1.Value = 100;
                    break;
                case "Decompress":
                    Compressor.Decompress(_datasets);
                    progressBar1.Value = 100;
                    break;
                default:
                    MessageBox.Show("Operation not selected");
                    textBox2.Text = "Failed";
                    progressBar1.Value = 0;
                    break;
            }
            textBox2.Text = "Finished";

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.ScrollBars = ScrollBars.Both;
            textBox1.Text = string.Join(Environment.NewLine, _datasets);
        }

        private void Form1_Load(object sender, EventArgs e) {}

        private void label2_Click(object sender, EventArgs e) {}

        private void label1_Click(object sender, EventArgs e) {}

        private void progressBar1_Click(object sender, EventArgs e) {}

        private void textBox2_TextChanged_1(object sender, EventArgs e) {}

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {}
    }
}

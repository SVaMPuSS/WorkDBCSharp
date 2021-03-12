using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace AddCustomer
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }
        string specificFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AddCustomer");
        private void Settings_Load(object sender, EventArgs e)
        {
            try
            {
                using (StreamReader sw = new StreamReader(specificFolder+"\\Settings.txt", Encoding.GetEncoding(1251)))
                {
                    textBox1.Text = sw.ReadLine();
                    textBox2.Text = sw.ReadLine();
                }
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            textBox1.Text = folderBrowserDialog1.SelectedPath;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0 && textBox2.Text.Length > 0)
                button3.Enabled = true;
            else
                button3.Enabled = false;
            folderBrowserDialog1.SelectedPath = textBox1.Text;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0 && textBox2.Text.Length > 0)
                button3.Enabled = true;
            else
                button3.Enabled = false;
            folderBrowserDialog2.SelectedPath = textBox2.Text;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog2.ShowDialog();
            textBox2.Text = folderBrowserDialog2.SelectedPath;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(folderBrowserDialog1.SelectedPath))
            {
                try
                {
                    Directory.CreateDirectory(folderBrowserDialog1.SelectedPath);
                }catch (IOException ex)
                {
                    MessageBox.Show("В пути есть недопустимые символы!");
                    throw ex;
                }catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show("Недостаточно прав!");
                    throw ex;
                }
            }
            if (!Directory.Exists(folderBrowserDialog2.SelectedPath))
            {
                try
                {
                    Directory.CreateDirectory(folderBrowserDialog2.SelectedPath);
                }catch (IOException ex)
                {
                    MessageBox.Show("В пути есть недопустимые символы!");
                    throw ex;
                }catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show("Недостаточно прав!");
                    throw ex;
                }
            }
            try
            {
                using (StreamWriter sw = new StreamWriter(specificFolder + "\\Settings.txt", false, Encoding.GetEncoding(1251)))
                {
                    sw.WriteLine(folderBrowserDialog1.SelectedPath);
                    sw.WriteLine(folderBrowserDialog2.SelectedPath);
                }
            }catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Error"); 
            }
            button3.Enabled = false;
        }
    }
}
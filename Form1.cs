using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using QRCoder;
using System.IO;
using FirebirdSql.Data.FirebirdClient;
namespace AddCustomer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string specificFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AddCustomer");
        string waydatabase = "";
        string way = "";
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            if (!Directory.Exists(specificFolder))
            {
                Directory.CreateDirectory(specificFolder);
                using (FileStream fs = File.Create(specificFolder + "\\Settings.txt")){}
            }
        }
        // Проверка ввода
        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            char sumb = e.KeyChar;
            if (!Char.IsLetter(sumb) && sumb != 8)
                e.Handled = true;
        }
        // Проверка email
        public bool IsValidEmail(string email)
        {
            string pattern = "[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}";
            Regex check = new Regex(pattern, RegexOptions.IgnorePatternWhitespace);
            bool valid = false;
            if (string.IsNullOrEmpty(email))
                valid = false;
            else
                valid = check.IsMatch(email);
            return valid;
        }
        // Выгрузка данных
        private void button1_Click(object sender, EventArgs e)
        {
            bool fName = false;
            bool fPhoneNumber = false;
            bool fEmail = false;
            // Имя
            if(textBox1.TextLength > 0)
                fName = true;
            // Email
            try
            {
                fEmail = IsValidEmail(textBox5.Text);
                var addr = new System.Net.Mail.MailAddress(textBox5.Text);
            }catch{ }
            // Пол
            int cussex = 0;
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    cussex = 1;
                    break;
                case 1:
                    cussex = 2;
                    break;
            }
            string sity = "";
            if(textBox4.Text.Length < 1)
                sity = "NULL";
            else
                sity = "'"+textBox4.Text+"'";
            // Номер телефона
            string pattern = @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$";
            if (Regex.IsMatch(maskedTextBox2.Text, pattern, RegexOptions.IgnoreCase))
                fPhoneNumber = true;
            if (fName && fPhoneNumber && fEmail)
            {
                // Путь к бд и папке
                try
                {
                    using (StreamReader sw = new StreamReader(specificFolder + "\\Settings.txt", Encoding.GetEncoding(1251)))
                    {
                        way = sw.ReadLine();
                        waydatabase = sw.ReadLine();
                    }
                }
                catch { }
                try
                {
                    //работа с бд
                    FbConnection connecting = new FbConnection(@"Server=localhost; User=SYSDBA; Password=masterkey; Database=" + waydatabase + "MAIN.GDB; ");
                    connecting.Open();
                    string query = "SELECT coalesce(max(id),0),coalesce(max(code),0) from client";
                    FbCommand command = new FbCommand(query, connecting);
                    FbDataReader reader = command.ExecuteReader();
                    int id = 0;
                    int code = 0;
                    while (reader.Read())
                    {
                        id = int.Parse(reader[0].ToString()) + 1;
                        code = int.Parse(reader[1].ToString()) + 1;
                    }
                    // работа с qr
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode((2000000000 + id).ToString(), QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(2);
                    qrCodeImage.Save(way + "\\" + (2000000000 + id).ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    //загрузка данных в бд
                    query = "INSERT INTO CLIENT(id,code,parentid,name,city,regdate,sex,birthdate,email,telephone) VALUES(" + id.ToString() + "," + code.ToString() + ",0,'" + textBox1.Text + " " + textBox2.Text + " " + textBox3.Text + "'," + sity + ",'" + dateTimePicker2.Value.ToShortDateString() + "'," + cussex + ",'" + dateTimePicker1.Value.ToShortDateString() + "','" + textBox5.Text + "','" + maskedTextBox2.Text + "')";
                    command = new FbCommand(query, connecting);
                    reader = command.ExecuteReader();
                    reader.Close();
                    query = "INSERT INTO GRPCCARD(id, code, name, text)VALUES("+ id.ToString() + "," + code.ToString() +", 'card', 'card')";
                    command = new FbCommand(query, connecting);
                    reader = command.ExecuteReader();
                    reader.Close();
                    query = "INSERT INTO CCARD(id,code,GRPCCARDID,val) VALUES(" + id.ToString() + "," + code.ToString() + "," + id.ToString() + ","+id.ToString()+")";
                    command = new FbCommand(query, connecting);
                    reader = command.ExecuteReader();
                    reader.Close();
                    query = "INSERT INTO CLIENTCARD(id,CLIENTID,CCARDID) VALUES(" + id.ToString() + "," + id.ToString() + "," + id.ToString() + ")";
                    command = new FbCommand(query, connecting);
                    reader = command.ExecuteReader();
                    //закрытие подключения
                    reader.Close();
                    connecting.Close();
                }catch(FbException ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            } else
                MessageBox.Show("В данных ошибка!\n" + "Проверьте:\n" + "Имя\n" + "Телефон\n" + "Email\n","Ошибка");
        }
        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
        }
        private void просмотрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowData showData = new ShowData();
            showData.Show();
        }
    }
}
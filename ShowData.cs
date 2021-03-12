using System;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Data;
using FirebirdSql.Data.FirebirdClient;
using System.Text.RegularExpressions;
using CalendarDataGridView;
namespace AddCustomer
{
    public partial class ShowData : Form
    {
        public ShowData()
        {
            InitializeComponent();
        }
        string specificFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AddCustomer");
        string waysave = "";
        string waydatabase = "";
        private FbConnection Connection = null;
        private FbCommandBuilder CommandBuilder = null;
        private FbDataAdapter DataAdapter = null;
        private DataSet dataSet = null;
        private bool newRowAdding = false;
        private void LoadData()
        {
            try
            {
                DataAdapter = new FbDataAdapter("SELECT id,NAME,CITY,TELEPHONE,EMAIL,REGDATE,BIRTHDATE,'Удалить' FROM CLIENT", Connection);
                CommandBuilder = new FbCommandBuilder(DataAdapter);
                dataSet = new DataSet();
                DataAdapter.Fill(dataSet,"CLIENT");
                dataGridView1.DataSource = dataSet.Tables["CLIENT"];
                dataGridView1.Columns[0].HeaderText = "Номер покупателя";
                dataGridView1.Columns[1].HeaderText = "Имя";
                dataGridView1.Columns[2].HeaderText = "Город";
                dataGridView1.Columns[3].HeaderText = "Телефон";
                dataGridView1.Columns[4].HeaderText = "Почта";
                dataGridView1.Columns[5].HeaderText = "Дата регистрации";
                dataGridView1.Columns[6].HeaderText = "Дата рождения";
                dataGridView1.Columns[7].HeaderText = "Действие";
                dataGridView1.Columns[0].ReadOnly = true;
                for (int i = 0; i<dataGridView1.Rows.Count;i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[7,i] = linkCell; 
                    CalendarCell col = new CalendarCell();
                    dataGridView1[5, i] = col;
                    col = new CalendarCell();
                    dataGridView1[6, i] = col;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!");
            }
        }
        private void ReloadData()
        {
            try
            {
                dataSet.Tables["CLIENT"].Clear();
                DataAdapter.Fill(dataSet, "CLIENT");
                dataGridView1.DataSource = dataSet.Tables["CLIENT"];
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[7, i] = linkCell;
                    CalendarCell col = new CalendarCell();
                    dataGridView1[5, i] = col;
                    col = new CalendarCell();
                    dataGridView1[6, i] = col;
                }
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!");
            }
        }
        private void ShowData_Load(object sender, EventArgs e)
        {
            try
            {
                using (StreamReader sw = new StreamReader(specificFolder + "\\Settings.txt", Encoding.GetEncoding(1251)))
                {
                    waysave = sw.ReadLine();
                    waydatabase = sw.ReadLine();
                }
            }catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message, "Ошибка!"); 
            }
            Connection = new FbConnection(@"Server=localhost; User=SYSDBA; Password=masterkey; Database=" + waydatabase + "MAIN.GDB;");
            Connection.Open();
            LoadData();
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ReloadData();
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if(e.ColumnIndex == 7)
                {
                    string task = dataGridView1.Rows[e.RowIndex].Cells[7].Value.ToString();
                    if(task == "Удалить")
                    {
                        if(MessageBox.Show("Удалить этого пользователя?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            int rowIndex = e.RowIndex;
                            string index = dataGridView1.Rows[rowIndex].Cells[0].Value.ToString();
                            string query = "DELETE FROM CLIENTCARD WHERE id =" + index;
                            dataGridView1.Rows.RemoveAt(rowIndex);
                            dataSet.Tables["CLIENT"].Rows[rowIndex].Delete();
                            DataAdapter.Update(dataSet, "CLIENT");
                            FbCommand command = new FbCommand(query, Connection);
                            FbDataReader reader = command.ExecuteReader();
                            reader.Close();
                            query = "DELETE FROM CCARD WHERE id =" + index;
                            command = new FbCommand(query, Connection);
                            reader = command.ExecuteReader();
                            reader.Close();
                            query = "DELETE FROM GRPCCARD WHERE id =" + index;
                            command = new FbCommand(query, Connection);
                            reader = command.ExecuteReader();
                            reader.Close();
                        }
                    }else
                    if(task == "Обновить")
                    {
                        int r = e.RowIndex;
                        dataSet.Tables["CLIENT"].Rows[r]["NAME"] = dataGridView1.Rows[r].Cells["NAME"].Value;
                        dataSet.Tables["CLIENT"].Rows[r]["CITY"] = dataGridView1.Rows[r].Cells["CITY"].Value;
                        dataSet.Tables["CLIENT"].Rows[r]["TELEPHONE"] = dataGridView1.Rows[r].Cells["TELEPHONE"].Value;
                        dataSet.Tables["CLIENT"].Rows[r]["EMAIL"] = dataGridView1.Rows[r].Cells["EMAIL"].Value;
                        dataSet.Tables["CLIENT"].Rows[r]["REGDATE"] = dataGridView1.Rows[r].Cells["REGDATE"].Value;
                        dataSet.Tables["CLIENT"].Rows[r]["BIRTHDATE"] = dataGridView1.Rows[r].Cells["BIRTHDATE"].Value;
                        DataAdapter.Update(dataSet, "CLIENT");
                        dataGridView1.Rows[e.RowIndex].Cells[7].Value = "Удалить";
                    }
                    ReloadData();
                }
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!");
            }
        }
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if(newRowAdding == false)
                {
                    int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
                    DataGridViewRow editingRow = dataGridView1.Rows[rowIndex];
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[7, rowIndex] = linkCell;
                    editingRow.Cells[7].Value = "Обновить";
                }
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!");
            }
        }
        private void dataGridView1_Sorted(object sender, EventArgs e)
        {
            ReloadData();
        }
        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column_KeyPress);
            if(dataGridView1.CurrentCell.ColumnIndex == 2 || dataGridView1.CurrentCell.ColumnIndex == 1)
            {
                TextBox textBox = e.Control as TextBox;
                if (textBox != null)
                    textBox.KeyPress += new KeyPressEventHandler(Column_KeyPress);
            }
        }
        private void Column_KeyPress(object sender,KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 3)
            {
                bool fPhoneNumber = false;
                string pattern = @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$";
                if (Regex.IsMatch(dataGridView1[3, e.RowIndex].Value.ToString(), pattern, RegexOptions.IgnoreCase))
                    fPhoneNumber = true;
                if (!fPhoneNumber)
                {
                    MessageBox.Show("Неправильный номер", "Ошибка!");
                    dataGridView1.CancelEdit();
                }
            }else
            if(e.ColumnIndex == 4)
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(dataGridView1[4, e.RowIndex].Value.ToString());
                }catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка!");
                    dataGridView1.CancelEdit();
                }
            }
        }
    }
}
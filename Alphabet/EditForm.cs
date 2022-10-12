﻿using System;
using System.Drawing;
using System.Windows.Forms;

using Alphabet.View;
using Alphabet.View.PersonsOperations;
using Alphabet.Service;

namespace Alphabet
{
    public partial class EditForm : Form, IPersonsAddOrDeleteView, IProgressBarView
    {
        private DateTime _date;

        public event Action<string, DateTime, string> InsertTelegramEventHandler;

        public event Action<int, int, string> OperationsOnPersonsEventHandler;

        public event Action<long> DecisionBakingOnPersonEventHandler;

        public event Action<int> HandEditNoFindedPersonsEventHandler;

        public event Action<int> GetPersonDTEventHandler;

        public event Action ClosingFormEventHandler;

        public event Action<int, int> ChangeBorderRoutingEventHandler;

        public EditForm()
        {
            InitializeComponent();
        }

        public DialogResult ShowMessageBox(string message, string title, MessageBoxButtons button, MessageBoxIcon icon)
        {
            DialogResult result = DialogResult.No;
            Invoke((MethodInvoker)(() =>
            {
                result = MessageBox.Show(message, title, button, icon);
            }));

            return result;
        }

        public void CreateTableResult(object dataPersons)
        {
            Invoke((MethodInvoker)(() =>
            {
                foreach (DataGridViewRow item in dgvList.Rows)
                {
                    dgvList.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
                    dgvList.RowHeadersVisible = false;
                    dgvList.DataSource = dataPersons;
                    dgvList.Columns[1].HeaderText = "ФИО";
                    dgvList.Columns[2].HeaderText = "Дата рождения";
                    dgvList.Columns[3].HeaderText = "Отметка";
                    dgvList.Columns[4].HeaderText = "Срок контроля";
                    dgvList.Columns[5].HeaderText = "Пол";
                    dgvList.Columns[6].HeaderText = "Страна";
                    dgvList.Columns[7].HeaderText = "Место рождения";
                    dgvList.Columns[8].HeaderText = "Дополнительные сведения";
                    dgvList.Columns[9].HeaderText = "Задание";
                    dgvList.Columns[10].HeaderText = "Ключ";
                    dgvList.Columns[0].Visible = false;
                    dgvList.Columns[11].Visible = false;
                    dgvList.Columns[12].Visible = false;
                    dgvList.Columns[13].Visible = false;
                    dgvList.Columns[14].Visible = false;
                    dgvList.Columns[15].Visible = false;
                    lNumber.Text = string.Format("Количество - {0}", dgvList.RowCount);
                    UpdateTable();
                }
            }));
        }

        public void UpdateTable()
        {
            Invoke((MethodInvoker)(() =>
            {
                foreach (DataGridViewRow item in dgvList.Rows)
                {
                    //item.DefaultCellStyle.BackColor = (bool)item.Cells[10].Value ? Color.Red : (bool)item.Cells[9].Value ? Color.Orange : Color.White;
                    item.DefaultCellStyle.BackColor = (bool)item.Cells[13].Value ? Color.Red : (bool)item.Cells[12].Value ? Color.Orange : !(bool)item.Cells[12].Value ? Color.Orange : Color.White;
                }
            }));
        }

        public void SetEnableControls(bool state)
        {
            Invoke((MethodInvoker)(() =>
            {
                if (!state)
                    timer1.Start();
                else
                    timer1.Stop();
                menuStrip1.Enabled = state;
                bProcess.Enabled = state;
                groupBox1.Enabled = state;
                groupBox2.Enabled = state;
                groupBox3.Enabled = state;
                groupBox4.Enabled = state;
            }));
        }

        public void SetEnableControls()
        {
            Invoke((MethodInvoker)(() =>
            {
                int cdgvl = dgvList.Rows.Count;
                if (cdgvl > 0)
                {
                    menuStrip1.Enabled = false;
                    bProcess.Enabled = false;
                    groupBox1.Enabled = false;
                    groupBox2.Enabled = false;
                    groupBox3.Enabled = false;
                    groupBox4.Enabled = false;
                }
                else
                {
                    menuStrip1.Enabled = true;
                    bProcess.Enabled = true;
                    groupBox1.Enabled = true;
                    groupBox2.Enabled = true;
                    groupBox3.Enabled = true;
                    groupBox4.Enabled = true;
                }
            }));
        }

        public void UpdatePersonDT(object dataTable)
        {
            dgvAdd.DataSource = dataTable;
            if (dgvAdd.ColumnCount > 0)
                dgvAdd.Columns[0].Visible = false;
        }

        public void SetValue(int value)
        {
            Invoke((MethodInvoker)(() =>
            {
                progressBar1.Value = value;
            }));
        }

        public void SetMaximum(int max)
        {
            Invoke((MethodInvoker)(() =>
            {
                progressBar1.Maximum = max;
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //_date = DateTime.Now;

            //using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Архив | *.zip" })
            //{
            //    dgvList.DataSource = string.Empty;
            //    dgvAdd.DataSource = string.Empty;
            //    if (ofd.ShowDialog() == DialogResult.OK)
            //    {
            //        var folder = ofd.SafeFileName.Split('.')[0];
            //        Directory.CreateDirectory(folder);
            //        SetEnableControls(false);
            //        System.IO.Compression.ZipFile.ExtractToDirectory(ofd.FileName, folder, Encoding.GetEncoding(System.Globalization.CultureInfo.CurrentCulture.TextInfo.OEMCodePage));
            //        Person.List = new List<Person>();
            //        foreach (var file in Directory.GetFiles(folder, "*.txt"))
            //        {
            //            var t = file.Split(new char[] { '\\', '_' }, StringSplitOptions.RemoveEmptyEntries);
            //            dtDate.Value = DateTime.Parse(t.Last().Replace(".txt", "").Trim());
            //            await FileOperations.Read(file, t[5] == "1" ? 1 : 2, t[7].ToLower() == "въезд" ? 0 : 1);
            //        }
            //        Directory.Delete(folder, true);
            //        CreateTable();
            //        SetEnableControls(true);
            //        tbNumber.Text = folder.Remove(folder.Length -1);
            //        lNumber.Text = string.Format("Количество - {0}", dgvList.RowCount);
            //    }
            //}

        }

        private void button2_Click(object sender, EventArgs e)
        {
            progressBar1.Minimum = 0;
            progressBar1.Value = 0;
            _date = DateTime.Now;
            Invoke((MethodInvoker)(() =>
            {
                InsertTelegramEventHandler.Invoke(tbNumber.Text, dtDate.Value, "");
                OperationsOnPersonsEventHandler.Invoke(rbIn.Checked ? 0 : 1, rbAdd.Checked ? 2 : 1, dtDate.Value.ToShortDateString());
            }));
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dgvList.SelectedRows.Count != 0)
            {
                Invoke((MethodInvoker)(() =>
                {
                    GetPersonDTEventHandler.Invoke((int)dgvList.CurrentRow.Cells[0].Value);
                }));
            }
        }

        private void снятьСУчётаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvAdd.SelectedRows.Count != 0)
            {
                Invoke((MethodInvoker)(() =>
                {
                    DecisionBakingOnPersonEventHandler.Invoke(Convert.ToInt64(dgvAdd.CurrentRow.Cells[0].Value));
                    dgvAdd.DataSource = null;
                }));
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dgvList.SelectedRows.Count != 0)
            {
                Invoke((MethodInvoker)(() =>
                {
                    dgvAdd.DataSource = null;
                    HandEditNoFindedPersonsEventHandler.Invoke((int)dgvList.CurrentRow.Cells[0].Value);
                }));
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var date = DateTime.Now.Subtract(_date);
            lTime.Text = string.Format("Время выполнения: {0}:{1}", (date.Minutes > 9 ? "" + date.Minutes : "0" + date.Minutes), (date.Seconds > 9 ? "" + date.Seconds : "0" + date.Seconds));
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Logger.Writer(new SQLWriteSystemLogger(
                new AttributeSystemLog()
                {
                    DateTimeCreate = DateTime.Now,
                    LevelMessage = "Info",
                    Message = "Завершение работы в АРМе постановка/снятие."
                }));
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void опрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void справкаToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void rbIn_CheckedChanged(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)(() =>
            {
                bProcess.Text = rbAdd.Checked ? "Поставить на учёт" : "Снять с учёта";
                ChangeBorderRoutingEventHandler.Invoke(rbIn.Checked ? 0 : 1, rbAdd.Checked ? 2 : 1);
            }));
        }
    }
}

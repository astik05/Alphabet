﻿using System;
using System.Windows.Forms;

using Alphabet.View.PersonsOperations;
using Alphabet.Presenter;
using Alphabet.Model;

namespace Alphabet
{
    public partial class SearchForm : Form, IPersonsSelectView
    {
        public event Action<string> FindPersonsEventHandler;

        public event Action LoadDataOfFiltersSearchEventHandler;

        public object ViewUsers { get; set; }

        public object ViewMarks { get; set; }

        public object ViewCountries { get; set; }

        public SearchForm()
        {
            InitializeComponent();

            new PersonsOpearationsPresenter(new Person(), this);
        }

        public void UpdateFiltersControls()
        {
            cbUser.DataSource = ViewUsers;
            cbIndex.DataSource = ViewMarks;
            cbCountry.DataSource = ViewCountries;
        }

        public void UpdateFindedListPersons(object findedPersons)
        {
            dgvList.DataSource = findedPersons;
        }

        private void SelectFilter(object sender)
        {
            var cb = sender as CheckBox;
            var control = Controls.Find(cb.Tag.ToString(), true)[0];
            control.Enabled = cb.Checked;
        }

        private void ClearFilters()
        {
            foreach (Control item in Controls)
            {
                if (!(item is CheckBox) && !(item is Button))
                    item.ResetText();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearFilters();
        }

        private async void bProcess_Click(object sender, EventArgs e)
        {

            bProcess.Enabled = false;
            var filters = string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}",
                checkBox1.Checked ? "and p.FIO like '%" + tbName.Text + "%'" : "",
                  checkBox4.Checked ? "and p.IsDeleted = " + cbActive.SelectedIndex : "",
                   checkBox3.Checked ? "and p.Sex = " + cbSex.SelectedIndex : "",
                    checkBox5.Checked ? "and t.Number = '" + tbTelegram.Text + "'" : "",
                     checkBox6.Checked ? "and p.Route = " + cbRoute.SelectedIndex : "",
                      checkBox7.Checked ? "and p.IdCountry = " + cbCountry.SelectedValue : "",
                       checkBox8.Checked ? "and p.IdMark = " + cbIndex.SelectedValue : "",
                        checkBox9.Checked ? "and p.DateExpire = '" + dtDateExpire.Value.ToString("yyyyMMdd") + "'" : "",
                         checkBox10.Checked ? "and p.DateOfBirth = '" + dtDateOfBirth.Value.ToString("yyyyMMdd") + "'" : "",
                         checkBox2.Checked ? "and u.IdUser = " + cbUser.SelectedValue : ""
                );
            FindPersonsEventHandler.Invoke(filters);
            bProcess.Enabled = true;
            lNumber.Text = "Количество: " + dgvList.RowCount;
        }

        private void SearchForm_Load(object sender, EventArgs e)
        {
            LoadDataOfFiltersSearchEventHandler.Invoke();
        }

        private void checkBoxCheckedChanged(object sender, EventArgs e)
        {
            SelectFilter(sender);
        }
    }
}
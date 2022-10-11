using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Alphabet.View.PersonsOperations;
using Alphabet.Service;
using Alphabet.Model;

namespace Alphabet.Presenter
{
    internal class PersonsOpearationsPresenter
    {
        private Person _person;

        IPersonsAddOrDeleteView _personsOpearationsView;

        IPersonsSelectView _personsSelectView;

        private int _numberTelegram;

        public PersonsOpearationsPresenter(Person person, IPersonsOperationsView personsOpearationsView)
        {
            _person = person;
            SelectionView(personsOpearationsView);
        }

        private void SelectionView(IPersonsOperationsView personsOpearationsView)
        {
            if (personsOpearationsView is IPersonsAddOrDeleteView)
            {
                _personsOpearationsView = (IPersonsAddOrDeleteView)personsOpearationsView;
                SubscribeForPersonsAddOrDeleteView();
            }
            else if (personsOpearationsView is IPersonsSelectView)
            {
                _personsSelectView = (IPersonsSelectView)personsOpearationsView;
                SubscribeForPersonsSelectView();
            }
        }

        private void SubscribeForPersonsAddOrDeleteView()
        {
            _personsOpearationsView.InsertTelegramEventHandler += InsertingNumberTelegram;
            _personsOpearationsView.InsertPersonsEventHandler += InsertingPersons;
        }

        private void SubscribeForPersonsSelectView()
        {
            _personsSelectView.LoadDataOfFiltersSearchEventHandler += LoadingDataOfFiltersSearch;
            _personsSelectView.FindPersonsEventHandler += FindingPersons;
        }

        private void InsertingNumberTelegram(string numberTelegram, DateTime dateofSigning, string description)
        {
            var insertTelegram = new InsertTelegram(numberTelegram, dateofSigning, description);
            insertTelegram.Execute();

            _numberTelegram = int.Parse(insertTelegram.ParseTableResult(0, 0));
        }

        private void InsertingPersons(int borderRouting, int typeOperation)
        {
            var list = Person.List.Where(x => x.Route == borderRouting && x.Type == typeOperation).ToList();
            foreach (Person item in list)
            {
                var insertPerson = new InsertPerson(item, _numberTelegram);
                insertPerson.Execute();
                var flagInserted = insertPerson.ParseTableResult(0, 0);
                if (flagInserted != "")
                    Person.List.Remove(item);
            }
        }

        private void LoadingDataOfFiltersSearch()
        {
            var selectUsers = new SelectUsersLogin();
            selectUsers.Execute();
            _personsSelectView.ViewUsers = selectUsers.GetTableResult();

            var selectMarks = new SelectMarks();
            selectMarks.Execute();
            _personsSelectView.ViewMarks = selectMarks.GetTableResult();

            var selectCountries = new SelectCountries();
            selectCountries.Execute();
            _personsSelectView.ViewCountries = selectCountries.GetTableResult();

            _personsSelectView.UpdateFiltersControls();
        }

        private void FindingPersons(string filter)
        {
            var findPersons = new FindPersons(filter);
            findPersons.Execute();

            _personsSelectView.UpdateFindedListPersons(findPersons.GetTableResult());
        }
    }
}

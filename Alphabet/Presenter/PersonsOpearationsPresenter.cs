using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Alphabet.View;
using Alphabet.Service;

namespace Alphabet.Presenter
{
    internal class PersonsOpearationsPresenter
    {
        private Person _person;

        IPersonsOpearationsView _personsOpearationsView;

        private int _numberTelegram;

        public PersonsOpearationsPresenter(Person person, IPersonsOpearationsView personsOpearationsView)
        {
            _person = person;
            _personsOpearationsView = personsOpearationsView;

            Subscribe();
        }

        private void Subscribe()
        {
            _personsOpearationsView.InsertTelegramEventHandler += InsertingNumberTelegram;
            _personsOpearationsView.InsertPersonsEventHandler += InsertingPersons;
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
    }
}

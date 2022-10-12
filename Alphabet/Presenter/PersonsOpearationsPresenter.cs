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

        IPersonsAddOrDeleteView _personsAddOrDeleteView;

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
                _personsAddOrDeleteView = (IPersonsAddOrDeleteView)personsOpearationsView;
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
            _personsAddOrDeleteView.InsertTelegramEventHandler += InsertingNumberTelegram;
            _personsAddOrDeleteView.RegistrationPersonsEventHandler += InsertingNumberTelegram;
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

        private async void OperationsOnPersons(int borderRouting, int typeOperation)
        {
            string levelMessage = "Info";
            string message = string.Empty;
            await Task.Run(() =>
            {
                try
                {
                    var list = Person.List.Where(x => x.Route == borderRouting && x.Type == typeOperation).ToList();
                    if (typeOperation == 2)
                        RegistrationPersons(list, ref levelMessage, ref message);
                    else if (typeOperation == 1)
                        DeregistrationPersons(list, ref levelMessage, ref message);

                    _personsAddOrDeleteView.ViewResultOperation(list, message);
                }
                catch (Exception exception)
                {
                    levelMessage = "Error";
                    message = "Ошибка выполнения операции над персоной! " + exception.ToString();
                }
                finally
                {
                    Logger.Writer(new SQLWriteSystemLogger(
                        new AttributeSystemLog()
                        {
                            DateTimeCreate = DateTime.Now,
                            LevelMessage = levelMessage,
                            Message = message
                        }));
                }
            });
        }

        private void RegistrationPersons(List<Person> persons, ref string levelMessage, ref string message)
        {
            Logger.Writer(new SQLWriteSystemLogger(
                new AttributeSystemLog()
                {
                    DateTimeCreate = DateTime.Now,
                    LevelMessage = "Info",
                    Message = "Начало операции постановки на учёт."
                }));
            try
            {
                int count = 0;
                foreach (Person item in persons)
                {
                    var insertPerson = new InsertPerson(item, _numberTelegram);
                    insertPerson.Execute();
                    var flagInserted = insertPerson.ParseTableResult(0, 0);
                    if (flagInserted != "")
                    {
                        Person.List.Remove(item);
                        ++count;
                    }
                }
                levelMessage = "Info";
                message = "Завершение операции постановки на учёт. Количество обработанных записей - " + count.ToString();
            }
            catch (Exception exception)
            {
                levelMessage = "Error";
                message = "Ошибка операции постановки на учёт! " + exception.ToString();
            }
        }

        private void DeregistrationPersons(List<Person> persons, ref string levelMessage, ref string message)
        {
            Logger.Writer(new SQLWriteSystemLogger(
                new AttributeSystemLog()
                {
                    DateTimeCreate = DateTime.Now,
                    LevelMessage = "Info",
                    Message = "Начало операции снятия с учёта."
                }));
            try
            {
                foreach (Person item in persons)
                {
                    var comparePersons = new ComparePersons(item);
                    comparePersons.Execute();

                    item.DT = comparePersons.GetTableResult();
                    if (item.DT.Rows.Count == 0)
                        item.IsDeleted = false;
                    else if (item.DT.Rows.Count > 1)
                        item.IsMulty = true;
                    else
                    {
                        item.IsDeleted = true;
                        var changeStatePerson = new ChangeStatePerson(Convert.ToInt64(item.DT.Rows[0].ItemArray[0]), _numberTelegram);
                        changeStatePerson.Execute();
                    }
                }

                persons.RemoveAll(x => x.IsDeleted);
                levelMessage = "Info";
                var countPersonsNoDelete = persons.Where(x => !x.IsDeleted).Count();
                var countPersons = persons.Count();
                var res = string.Format("Количество снятых записей - {0}. Ожидают принятия решения - {1}.", countPersons - countPersonsNoDelete, countPersonsNoDelete));
                message = "Завершение операции снятия с учёта. " + res;
            }
            catch (Exception exception)
            {
                levelMessage = "Error";
                message = "Ошибка операции снятия с учёта! " + exception.ToString();
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphabet.View.PersonsOperations
{
    internal interface IPersonsAddOrDeleteView : IPersonsOperationsView
    {
        event Action<string, DateTime, string> InsertTelegramEventHandler;

        event Action<int, int> OperationsOnPersonsEventHandler;

        void ViewResultOperation(object dataPersons, string message);
    }
}

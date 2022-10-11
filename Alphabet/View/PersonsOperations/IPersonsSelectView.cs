using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Alphabet.View.PersonsOperations
{
    internal interface IPersonsSelectView : IPersonsOperationsView
    {
        event Action<string> FindPersonsEventHandler;

        event Action LoadDataOfFiltersSearchEventHandler;

        object ViewUsers { get; set; }

        object ViewMarks { get; set; }

        object ViewCountries { get; set; }

        void UpdateFiltersControls();

        void UpdateFindedListPersons(object findedPersons);
    }
}

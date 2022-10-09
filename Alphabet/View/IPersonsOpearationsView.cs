using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphabet.View
{
    internal interface IPersonsOpearationsView
    {
        event Action<string, DateTime, string> InsertTelegramEventHandler;

        event Action<int, int> InsertPersonsEventHandler;
    }
}

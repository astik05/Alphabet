using System.Windows.Forms;

namespace Alphabet.Service
{
    class ARMManager
    {
        public static ARM EnterARM(string nameARM)
        {
            switch (nameARM)
            {
                case ARMAdministrator.Name:
                    return new ARMAdministrator();
                default:
                    return new ARM();
            }
        }
    }

    internal class ARM
    {
        public Form ARMForm { get; set; }
    }

    class ARMAdministrator : ARM
    {
        public const string Name = "АРМ Администратора";

        public ARMAdministrator()
        {
            ARMForm = new FormAdministrator();
        }
    }
}

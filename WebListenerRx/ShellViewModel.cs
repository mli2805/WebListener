using System.Collections.Generic;

namespace WebListenerRx {
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase, IShell
    {
        public List<string> Rows { get; set; } = new List<string>();

        public ShellViewModel()
        {
            Rows.Add("Here the program started");
            Rows.Add("Second line");
        }
    }
}
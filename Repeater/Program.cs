using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Repeater
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Repeater r = new Repeater();
            Application.Run();
        }
    }
}

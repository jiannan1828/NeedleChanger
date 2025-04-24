using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InjectorInspector
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        /// 
        public static Form1 form1;
        [STAThread]
        static void Main()
        {
            Application.ApplicationExit += OnApplicationExit;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form1 = new Form1();
            Application.Run(form1);
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            XavierLogger.XavierLogger_Shutdown();
        }
    }
}

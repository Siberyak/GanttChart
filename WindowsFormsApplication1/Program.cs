using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.OData.Builder;
using System.Windows.Forms;

using KG.SE2.Modules.PlanGrafikModule.API.WebAPI.EDM;

using Microsoft.Data.OData;

using Simple.OData.Client;

namespace DAL
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.OData.Formatter;
using System.Web.OData.Formatter.Deserialization;
using System.Windows.Forms;

using KG.SE2.Modules.PlanGrafikModule.Model.API;
using KG.SE2.Modules.PlanGrafikModule.Model.Enums;

using Microsoft.OData.Edm;

using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

using Simple.OData.Client;

using Task = KG.SE2.Modules.PlanGrafikModule.Model.API.Task;

namespace DAL
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        



        private async void button1_Click(object sender, EventArgs e)
        {
            
            int projectId = 9001;

            ProjectInfo projectInfo = await ProjectInfo.Get(projectId);
            if (projectInfo.HeadProjectInfo != null)
                projectInfo = await ProjectInfo.Get(projectInfo.HeadProjectInfo.Id);

            ProjectInfo[] child = await projectInfo.Children();

            //Project project2 = await projectInfo.Entity();


            TaskInfo[] inf = await TaskInfo.ProjectTasks(projectId);

            var len = inf.Length;


            return;


        }


    }
}

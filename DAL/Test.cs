using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Braincase.GanttChart;
using KG.SE2.Modules.PlanGrafikModule.Model.API;
using KG.SE2.Modules.PlanGrafikModule.Model.Enums;
using Task = KG.SE2.Modules.PlanGrafikModule.Model.API.Task;

namespace DAL
{
    public class Test
    {
        public static async System.Threading.Tasks.Task Refresh(int projectId)
        {
            //http://localhost:8564/prediction/
            var baseAddress = EntityInfo.BaseUrl.Replace("api/PlanGrafik", "prediction/");
            await Check(baseAddress, "refresh", projectId);

            string state = null;
            var count = 0;
            while (state != "Planned")
            {
                state = await Check(baseAddress, "state", projectId);
                if (state == "Planned" || count++ > 100)
                    break;

                Thread.Sleep(100);
            }
        }

        static async Task<string> Check(string baseAddress, object method, object projectId)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);

            try
            {
                var response = await client.GetAsync($"{method}?projectId={projectId}");
                if (response.StatusCode != HttpStatusCode.OK)
                    return "error";

                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        public static async Task<ProjectManager> LoadData(int projectId)
        {
            var projectManager = new ProjectManager();


            //int projectId = 9005;//2003;

            ProjectInfo projectInfo = await ProjectInfo.Get(projectId);

            Project project1 = await projectInfo.Entity();


            if (projectInfo.HeadProjectInfo != null)
                projectInfo = await ProjectInfo.Get(projectInfo.HeadProjectInfo.Id);

            ProjectInfo[] child = (await projectInfo.Children()).OrderBy(x => x.Order).ToArray();

            Project project = await projectInfo.Entity();
            Func<DateTimeOffset?, int> getDate = (x) => (x - project.PredictionStartDate ?? TimeSpan.Zero).Days;

            if(child.Length == 0)
                child = new ProjectInfo[] { projectInfo };

            Dictionary<int, Braincase.GanttChart.Task> dictionary = new Dictionary<int, Braincase.GanttChart.Task>();

            Dictionary<int, OrgStrEl> resources = new Dictionary<int, OrgStrEl>();

            foreach (var info in child)
            {
                var entity = await info.Entity();
                
                var gantTask = new Braincase.GanttChart.Task() { Name = $"{entity.ShortName}{GetDatesInfo(entity)}" };
                
                dictionary.Add(info.Id, gantTask);
                projectManager.Add(gantTask);

                projectManager.SetStart(gantTask, getDate(entity.PredictionStartDate));
                projectManager.SetEnd(gantTask, getDate(entity.PredictionFinishDate)+1);

                if(entity is GroupProject)
                    continue;

                var projectTasks = await TaskInfo.ProjectTasks(info.Id);
                Dictionary<int, Braincase.GanttChart.Task> tasks = new Dictionary<int, Braincase.GanttChart.Task>();
                foreach (var projectTask in projectTasks.OrderBy(x => x.Order))
                {
                    try
                    {


                        Task taskEntity = await projectTask.Entity();

                        var taskGantTask = new Braincase.GanttChart.Task() { Name = taskEntity.Name };
                        projectManager.Add(taskGantTask);
                        
                        tasks.Add(projectTask.Id, taskGantTask);

                        projectManager.SetStart(taskGantTask, getDate(taskEntity.PredictionStartDate()));
                        projectManager.SetEnd(taskGantTask, getDate(taskEntity.PredictionFinishDate()) + 1);

                        var ass = await projectTask.Ass();
                        if(ass != null)
                        {
                            var en = await ass.Entity();
                            projectManager.Assign(taskGantTask, ass);
                            taskGantTask.Name = $"{taskGantTask.Name} [{ass}]";
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                foreach (var projectTask in projectTasks.OrderBy(x => x.Order))
                {
                    if(projectTask.ParentInfo != null)
                        projectManager.Group(tasks[projectTask.ParentInfo.Id], tasks[projectTask.Id]);
                    else
                        projectManager.Group(gantTask, tasks[projectTask.Id]);
                }

            }

            foreach (var info in child)
            {
                if(info.ParentInfo != null)
                    projectManager.Group(dictionary[info.ParentInfo.Id], dictionary[info.Id]);
            }

            TaskInfo[] inf = await TaskInfo.ProjectTasks(projectId);

            var len = inf.Length;


            return projectManager;
        }

        private static string GetDatesInfo(Project entity)
        {
            var simpleProject = entity as SimpleProject;

            if (simpleProject?.PlanningMode == PlanningMode.JIT)
                return $" <= {entity.PlannedFinishDate?.Date.ToShortDateString()}";
            if (simpleProject?.PlanningMode == PlanningMode.ASAP)
                return $" >= {entity.PlannedStartDate?.Date.ToShortDateString()}";

            return null;
        }
    }

    public static class Extender
    {
        public static DateTimeOffset? PredictionStartDate(this Task task)
        {
            return (task as GroupTask)?.PredictionStartDate
                ?? (task as InternalTask)?.PredictionStartDate
                ?? (task as ExternalTask)?.PredictionStartDate
                ;
        }
        public static DateTimeOffset? PredictionFinishDate(this Task task)
        {
            return (task as GroupTask)?.PredictionFinishDate
                ?? (task as InternalTask)?.PredictionFinishDate
                ?? (task as ExternalTask)?.PredictionFinishDate
                ;
        }
    }
}
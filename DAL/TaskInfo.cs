using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Task = KG.SE2.Modules.PlanGrafikModule.Model.API.Task;

namespace DAL
{
    public class TaskInfo : EntityInfo<Task>
    {
        private class TasksInfo
        {
            [JsonProperty("value")]
            public TaskInfo[] Infos { get; set; }
        }
        [JsonProperty("Order")]
        public int Order { get; set; }

        [JsonProperty("Parent")]
        public EntityInfo ParentInfo { get; set; }

        public static async Task<TaskInfo[]> ProjectTasks(int projectId)
        {
            TasksInfo inf = await Read<TasksInfo>($"Projects({projectId})/SE2.SimpleProject/Tasks?$expand=Parent($select=Id)&$select=Id,Parent,Order");
            return inf.Infos;
        }

        public async Task<OrgStrEl> Ass()
        {
            if (TypeName != "#SE2.InternalTask")
                return null;

            var content = await Content(GetEntityQuery(Id) + "?$expand=SE2.InternalTask/PredictionAssigned");
            var des = (JObject)JsonConvert.DeserializeObject(content);
            var id = des.Property("PredictionAssigned").Value.ToObject<OrgStrEl>();
//            var id = des.Property("PredictionAssigned").Value.SelectToken("Id").Value<int>();
            return id;
        }

        protected override string GetEntityQuery(int id)
        {
            var entityQuery = $"Tasks({id})";
            //if (TypeName == "#SE2.InternalTask")
            //{
            //    entityQuery += "?$expand=SE2.InternalTask/PredictionAssigned";
            //}
            return entityQuery;
        }
    }
}
using System;
using System.Net.Http;
using System.Threading.Tasks;
using KG.SE2.Modules.PlanGrafikModule.Model.API;
using Newtonsoft.Json;

namespace DAL
{
    public class ProjectInfo : EntityInfo<Project>
    {
        private EntityInfo _headProjectInfo;

        /*
<EntityType Name="Project" Abstract="true">

<NavigationProperty Name="ProjectTeams" Type="Collection(SE2.ProjectTeam)" ContainsTarget="true"/>
<NavigationProperty Name="Type" Type="SE2.ProjectType" ContainsTarget="true"/>
<NavigationProperty Name="Parent" Type="SE2.GroupProject" ContainsTarget="true"/>
<NavigationProperty Name="HeadProject" Type="SE2.GroupProject" ContainsTarget="true"/>
<NavigationProperty Name="Predecessors" Type="Collection(SE2.Project)" ContainsTarget="true"/>
<NavigationProperty Name="Successors" Type="Collection(SE2.Project)" ContainsTarget="true"/>
<NavigationProperty Name="JobTicket" Type="SE2.JobTicket" ContainsTarget="true"/>
<NavigationProperty Name="Product" Type="SE2.Product" ContainsTarget="true"/>
<NavigationProperty Name="Creator" Type="SE2.Employee" ContainsTarget="true"/>
<NavigationProperty Name="Modifier" Type="SE2.Employee" ContainsTarget="true"/>
<NavigationProperty Name="CalculationStatus" Type="SE2.CalculationStatus" ContainsTarget="true"/>


<EntityType Name="GroupProject" BaseType="SE2.Project">

<NavigationProperty Name="Assigned" Type="SE2.OrganizationalStructureElement" ContainsTarget="true"/>
<NavigationProperty Name="Customer" Type="SE2.OrganizationalStructureElement" ContainsTarget="true"/>
<NavigationProperty Name="Manager" Type="SE2.Employee" ContainsTarget="true"/>
<NavigationProperty Name="Children" Type="Collection(SE2.Project)" ContainsTarget="true"/>
<NavigationProperty Name="PlanGrafikTasks" Type="Collection(SE2.Task)" ContainsTarget="true"/>

<EntityType Name="SimpleProject" BaseType="SE2.Project">

<NavigationProperty Name="Tasks" Type="Collection(SE2.Task)" ContainsTarget="true"/>
<NavigationProperty Name="RootTasks" Type="Collection(SE2.Task)" ContainsTarget="true"/>
<NavigationProperty Name="Author" Type="SE2.OrganizationalStructureElement" ContainsTarget="true"/>
<NavigationProperty Name="ApproverLink" Type="SE2.ProjectApproverLink" ContainsTarget="true"/>
<NavigationProperty Name="ChiefDesigner" Type="SE2.OrganizationalStructureElement" ContainsTarget="true"/>
<NavigationProperty Name="ChiefDesignerAssistant" Type="SE2.OrganizationalStructureElement" ContainsTarget="true"/>
<NavigationProperty Name="GorManager" Type="SE2.OrganizationalStructureElement" ContainsTarget="true"/>
<NavigationProperty Name="InternalProblems" Type="Collection(SE2.Problem)" ContainsTarget="true"/>
 */
        class ProjectsInfo
        {
            [JsonProperty("value")]
            public ProjectInfo[] Infos { get; set; }
        }

        [JsonProperty("Order")]
        public int Order { get; set; }

        [JsonProperty("Parent")]
        public EntityInfo ParentInfo { get; set; }

        [JsonProperty("HeadProject")]
        public EntityInfo HeadProjectInfo
        {
            get
            {
                return _headProjectInfo;
            }
            set
            {
                if (value != null && value.TypeName == null)
                    value.TypeName = "#SE2.GroupProject";
                _headProjectInfo = value;
            }
        }

        public static async Task<ProjectInfo> Get(int projectId)
        {
            string url = $"Projects({projectId})?$expand=HeadProject($select=Id),Parent($select=Id)&$select=Id,Parent,HeadProject,Order";

            var response = await Read<ProjectInfo>(url);
            return response;
        }

        protected override Type ResolveType()
        {
            return typeof(GroupProject);
        }

        protected override string GetEntityQuery(int id)
        {
            return $"Projects({id})?$expand=HeadProject($select=Id),Parent($select=Id)";
            //return $"Projects({id})?$expand=HeadProject($select=Id),Parent($select=Id)&$select=Id,Parent,HeadProject,ShortName,PredictionStartDate,PredictionFinishDate";
        }

        public async Task<ProjectInfo[]> Children()
        {
            string url = $"Projects({Id})/SE2.GroupProject/Children?$expand=SE2.Project/Parent($select=Id)&$select=Id,Parent,Order";
            var response = await Read<ProjectsInfo>(url);
            return response.Infos;
        }
    }
}
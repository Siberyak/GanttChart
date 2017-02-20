using System.Collections.Generic;
using System.Drawing;
using Braincase.GanttChart;
using KG.SE2.Modules.PlanGrafikModule.Model.API;
using Newtonsoft.Json;

namespace DAL
{
    public class OrgStrEl : EntityInfo<OrganizationalStructureElement>, ITaskFormatProvider
    {
        static Queue<Color> _colors = new Queue<Color>
            (
            new [] {Color.Yellow, Color.Aqua, Color.Chartreuse, Color.DarkOrange, Color.BlueViolet, }
            );

        static Dictionary<int, TaskFormat> _formatsById = new Dictionary<int, TaskFormat>();


        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        protected override string GetEntityQuery(int id)
        {
            return $"OrganizationalStructureElements({id})";
        }

        public override string ToString()
        {
            return Name ?? LastName ?? base.ToString();
        }

        static TaskFormat Copy(Color? taskColor, TaskFormat original)
        {
            if (taskColor.HasValue)
                return new TaskFormat
                {
                    Color = original.Color,
                    Border = original.Border,
                    BackFill = new SolidBrush(taskColor.Value),
                    ForeFill = original.ForeFill,
                    SlackFill = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.LightDownwardDiagonal, taskColor.Value, Color.Transparent)
                };

            return original;
        }

        public TaskFormat TaskFormat => _formatsById.ContainsKey(Id) ? _formatsById[Id] : TryCreate();

        private TaskFormat TryCreate()
        {
            if (_colors.Count == 0)
                return Chart.DefaultTaskFormat;

            var format = Copy(_colors.Dequeue(), Chart.DefaultTaskFormat);
            _formatsById.Add(Id, format);
            return format;
        }

        public TaskFormat CriticalTaskFormat => Chart.DefaultCriticalTaskFormat;
    }
}
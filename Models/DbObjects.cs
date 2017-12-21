using System.Collections.Generic;

namespace Models
{
    public class DbObjects
    {
        public bool Checked { get; set; }
        public int? ExecOrder { get; set; }
        public string Name { get; set; }
        public string RelationName { get; set; }
        public string SQL { get; set; }
        public string RevertSQL { get; set; }

        public List<DbObjects> Dependencies { get; set; } = new List<DbObjects>();
    }
}

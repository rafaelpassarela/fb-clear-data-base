using System.Collections.Generic;

namespace Models
{
    public class DbObjects
    {
        public string Name { get; set; }
        public string RelationName { get; set; }
        public bool Checked { get; set; }
        public List<DbObjects> Dependencies { get; set; } = new List<DbObjects>();
    }
}

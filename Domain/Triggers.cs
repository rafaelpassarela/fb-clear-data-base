using Dapper;
using Models;
using System.Collections.Generic;
using System.Data;

namespace Domain
{
    public class Triggers : BaseDomain
    {
        public Triggers(IDbConnection connection, ILogWriter log) : base(connection, log)
        {
        }

        protected override string GetName()
        {
            return "Triggers";
        }

        public override string GetDeleteSQL(DbObjects item)
        {
            return $"DROP TRIGGER {item.Name};";
        }

        protected override IEnumerable<DbObjects> DoLoad()
        {
            return _connection.Query<DbObjects>(@"select t.rdb$trigger_name as name, 0 as checked
                                                 from rdb$triggers t
                                                 where t.rdb$trigger_name not like '%$%'
                                                   and t.rdb$trigger_name not like 'CHECK_%'
                                                 order by t.rdb$trigger_name");
        }
    }
}

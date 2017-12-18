using Dapper;
using Models;
using System.Collections.Generic;
using System.Data;

namespace Domain
{
    public class UniquesAndChecks : BaseDomain
    {
        public UniquesAndChecks(IDbConnection connection, ILogWriter log) : base(connection, log)
        {
        }

        protected override string GetName()
        {
            return "Uniques e Checks";
        }

        public override string GetDeleteSQL(DbObjects item)
        {
            return $"ALTER TABLE {item.RelationName} DROP CONSTRAINT {item.Name};";
        }

        protected override IEnumerable<DbObjects> DoLoad()
        {
            return _connection.Query<DbObjects>(@"select rc.rdb$constraint_name as name,
                                                        rc.rdb$relation_name as relationName,
                                                        0 as checked
                                                 from rdb$relation_constraints rc
                                                 where rc.rdb$constraint_type in ('UNIQUE', 'CHECK')
                                                 order by rc.rdb$relation_name, rc.rdb$constraint_name");
        }
    }
}

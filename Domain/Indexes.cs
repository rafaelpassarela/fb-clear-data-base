using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogUtils;
using Models;
using Dapper;
using Schemas;

namespace Domain
{
    public class Indexes : BaseDomain
    {
        public Indexes(IDbConnection connection, ILogWriter log, FbSchema schema) : base(connection, log, schema)
        {
        }

        public override string GetDeleteSQL(DbObjects item)
        {
            return $"drop index {item.Name.Trim()};";
        }

        public override string GetRollbackSQL(DbObjects item)
        {
            return _schema.Keys.GetCreateSQLForItem(item.Name.Trim());
        }

        protected override IEnumerable<DbObjects> DoLoad()
        {
            return _connection.Query<DbObjects>($@"select distinct i.rdb$index_name as name,
                                                          i.rdb$relation_name as relationname,
                                                          0 as checked
                                                   from rdb$indices i
                                                   left join rdb$index_segments isg on(isg.rdb$index_name = i.rdb$index_name)
                                                   left join rdb$relation_constraints c on(i.rdb$index_name = c.rdb$index_name)
                                                   where c.rdb$constraint_type is null
                                                     and i.rdb$index_name not like '%$%'
                                                   order by i.rdb$index_name, isg.rdb$field_position");
        }

        protected override string GetName()
        {
            return "Indexes";
        }
    }
}

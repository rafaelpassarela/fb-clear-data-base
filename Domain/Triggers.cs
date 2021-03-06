﻿using Dapper;
using LogUtils;
using Models;
using System.Collections.Generic;
using System.Data;
using Schemas;

namespace Domain
{
    public class Triggers : BaseDomain
    {
        public Triggers(IDbConnection connection, ILogWriter log, FbSchema schema) : base(connection, log, schema)
        {
        }

        protected override string GetName()
        {
            return "Triggers";
        }

        public override string GetDeleteSQL(DbObjects item)
        {
            return $"drop trigger {item.Name.Trim()};";
        }

        protected override IEnumerable<DbObjects> DoLoad()
        {
            return _connection.Query<DbObjects>(@"select t.rdb$trigger_name as name, 0 as checked
                                                 from rdb$triggers t
                                                 where t.rdb$trigger_name not like '%$%'
                                                   and t.rdb$trigger_name not like 'CHECK_%'
                                                 order by t.rdb$trigger_name");
        }

        public override string GetRollbackSQL(DbObjects item)
        {
            return _schema.Triggers.GetCreateSQLForItem(item.Name);
        }
    }
}

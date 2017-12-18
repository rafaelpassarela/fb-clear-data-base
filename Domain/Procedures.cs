using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using LocalizationHelper;

namespace Domain
{
    public class Procedures : BaseDomain
    {
        public Procedures(IDbConnection connection, ILogWriter log) : base(connection, log)
        {
        }

        public override void CheckDependencies()
        {
            Console.WriteLine($" - {strings.checkDependencies} {GetName()}");

            foreach (var item in Items)
            {
                foreach (var dep in GetDependents(item.Name))
                {
                    item.Dependencies.Add(Items.FirstOrDefault(x => x.Name == dep));
                }
            }
        }

        private IEnumerable<string> GetDependents(string masterName)
        {
            return _connection.Query<string>($@"select distinct trim(d.rdb$dependent_name) from rdb$dependencies d
                                                where d.rdb$dependent_type = 5
                                                  and d.rdb$depended_on_type = 5
                                                  and d.rdb$depended_on_name = '{masterName}'
                                                  and d.rdb$dependent_name <> '{masterName}'");
        }

        public override string GetDeleteSQL(DbObjects item)
        {
            return $"drop procedure {item.Name};";
        }

        protected override IEnumerable<DbObjects> DoLoad()
        {
            return _connection.Query<DbObjects>(@"select trim(p.rdb$procedure_name) as name, 0 as checked
                                                 from rdb$procedures p
                                                 where p.rdb$procedure_name not like '%$%'
                                                 order by p.rdb$procedure_name");
        }

        protected override string GetName()
        {
            return "Procedures";
        }
    }
}

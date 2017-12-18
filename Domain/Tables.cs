﻿using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Domain
{
    public class Tables : BaseDomain
    {
        public Tables(IDbConnection connection, ILogWriter log) : base(connection, log)
        {
        }

        public override void CheckDependencies()
        {
            Console.WriteLine($" - Verificando dependencias de {GetName()}");

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
            /* passa a tabela e retorna quem utiliza ela */
            return _connection.Query<string>($@"select distinct trim(rc.rdb$relation_name) as reference_table
                                                from rdb$relation_constraints rc
                                                join rdb$index_segments dis on (rc.rdb$index_name = dis.rdb$index_name)
                                                join rdb$ref_constraints ref on (rc.rdb$constraint_name = ref.rdb$constraint_name)
                                                join rdb$relation_constraints m on (ref.rdb$const_name_uq = m.rdb$constraint_name)
                                                join rdb$index_segments master_index_segments on (m.rdb$index_name = master_index_segments.rdb$index_name)
                                                where m.rdb$relation_name = '{masterName}'");
            //and rc.rdb$relation_name <> '{masterName}'");

            /* passa a tabela e retorna quem ela utiliza */
            //return _connection.Query<string>($@"select distinct trim(m.rdb$relation_name) as reference_table
            //                                    from rdb$relation_constraints rc
            //                                    join rdb$index_segments dis on (rc.rdb$index_name = dis.rdb$index_name)
            //                                    join rdb$ref_constraints ref on (rc.rdb$constraint_name = ref.rdb$constraint_name)
            //                                    join rdb$relation_constraints m on (ref.rdb$const_name_uq = m.rdb$constraint_name)
            //                                    join rdb$index_segments master_index_segments on (m.rdb$index_name = master_index_segments.rdb$index_name)
            //                                    where rc.rdb$relation_name = '{masterName}'
            //                                      and m.rdb$relation_name <> '{masterName}'");
        }

        public override string GetDeleteSQL(DbObjects item)
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<DbObjects> DoLoad()
        {
            return _connection.Query<DbObjects>(@"select trim(r.rdb$relation_name) as name, 0 as checked 
                                                 from rdb$relations r 
                                                 where r.rdb$relation_name not like '%$%'
                                                 order by r.rdb$relation_name");
        }

        protected override string GetName()
        {
            return "Tables";
        }

        public override void RemoveAll()
        {
            _log.MessageLn($"Removendo {GetName()}...");

            foreach (var item in Items)
            {
                // antes de remover as chaves do item, vai nas dependencias e remove as FKs com o item
                foreach (var dep in item.Dependencies)
                {
                    foreach (var fk in _connection.Query<string>(GetFKRelationSQL(item.Name, dep.Name)))
                    {
                        _log.Message($"-{item.Name}->{dep.Name} = {fk}");
                        try
                        {
                            _connection.Execute($"ALTER TABLE {dep.Name} DROP CONSTRAINT {fk};");
                            _log.MessageLn(" | OK");
                        }
                        catch (Exception e)
                        {
                            _log.ErrorLn($"FK ERROR {e.Message}");
                        }
                    }
                }

                // depois de remover as FK, remove a PK
                foreach (var pk in _connection.Query<string>(GetPKSQL(item.Name)))
                {
                    _log.Message($"-{item.Name} = {pk}");
                    try
                    {
                        _connection.Execute($"ALTER TABLE {item.Name} DROP CONSTRAINT {pk};");
                        _log.MessageLn(" | OK");
                    }
                    catch (Exception e)
                    {
                        _log.ErrorLn($"PK ERROR {e.Message}");
                    }
                }
            }

            _log.MessageLn("");
        }

        private string GetFKRelationSQL(string mainTable, string dependTable)
        {
            return $@"select distinct rc.rdb$constraint_name
                      from rdb$relation_constraints rc
                      join rdb$index_segments dis on (rc.rdb$index_name = dis.rdb$index_name)
                      join rdb$ref_constraints ref on (rc.rdb$constraint_name = ref.rdb$constraint_name)
                      join rdb$relation_constraints m on (ref.rdb$const_name_uq = m.rdb$constraint_name)
                      join rdb$index_segments master_index_segments on (m.rdb$index_name = master_index_segments.rdb$index_name)
                      where rc.rdb$relation_name = '{dependTable}'
                        and m.rdb$relation_name = '{mainTable}'
                        and rc.rdb$constraint_type = 'FOREIGN KEY'
                      order by 1";
        }

        private string GetPKSQL(string table)
        {
            return $@"select rc.rdb$constraint_name
                      from rdb$relation_constraints rc
                      where rc.rdb$relation_name = '{table}'
                        and rc.rdb$constraint_type = 'PRIMARY KEY'
                      order by 1";
        }
    }
}

using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Domain
{
    public abstract class BaseDomain
    {
        private readonly string C_EMPTY_TABLE_NAME = "                               ";

        public abstract string GetDeleteSQL(DbObjects item);
        protected abstract string GetName();
        protected abstract IEnumerable<DbObjects> DoLoad();

        public delegate void OnLog(string msg, bool isError);

        protected readonly IDbConnection _connection;
        protected readonly ILogWriter _log;

        public IEnumerable<DbObjects> Items { get; set; }

        public BaseDomain(IDbConnection connection, ILogWriter log)
        {
            _connection = connection;
            _log = log;
            Load();
        }

        public virtual void CheckDependencies() { }

        public virtual void RemoveAll()
        {
            _log.MessageLn($"Removendo {GetName()}...");

            foreach (var item in Items)
            {
                DoRemove(item);
            }

            _log.MessageLn("");
        }

        private void DoRemove(DbObjects item)
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;

            if (!item.Checked)
            {
                foreach (var depend in item.Dependencies)
                {
                    DoRemove(depend);
                }

                Console.SetCursorPosition(x, y);
                _log.Message($" - {item.Name.Insert(item.Name.Length, C_EMPTY_TABLE_NAME)}");

                try
                {
                    _connection.Execute(GetDeleteSQL(item));
                    item.Checked = true;
                    Console.SetCursorPosition(x, y);
                }
                catch (Exception e)
                {
                    _log.ErrorLn($"ERRO> {item.Name.Trim()} : {e.Message}");
                    x = Console.CursorLeft;
                    y = Console.CursorTop;
                }
            }
        }

        public void Load()
        {
            _log.Message($"Carregando Lista de {GetName()}... ");

            Items = DoLoad();
            _log.MessageLn(Items.Count().ToString());

            CheckDependencies();
            //WriteDependenciesToLog(Items, new LogFileWriter(false), 0);
        }

        //private void WriteDependenciesToLog(IEnumerable<DbObjects> list, ILogWriter file, int level)
        //{
        //    string dots = "".PadLeft(level, '-');
        //    foreach (var item in list)
        //    {
        //        file.MessageLn($"{dots}{item.Name}");
        //        foreach (var dep in item.Dependencies)
        //        {
        //            file.MessageLn($"-{dots}{dep.Name}");
        //            if (!item.Name.Equals(dep.Name))
        //            {
        //                WriteDependenciesToLog(dep.Dependencies, file, level + 2);
        //            }
        //        }
        //    }
        //}
    }
}

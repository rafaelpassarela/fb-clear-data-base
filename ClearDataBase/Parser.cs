using Domain;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Data;

namespace ClearDataBase
{
    class Parser
    {
        private readonly Options _options;
        private readonly ILogWriter _log;
        private IDbConnection dataBase;
        private Tables tables;
        private Procedures procedures;
        private Triggers triggers;
        private UniquesAndChecks uniqCheks;

        public Parser(Options options, ILogWriter log)
        {
            _options = options;
            _log = log;
        }

        private bool ConnectDataBase()
        {
            _log.MessageLn(strings.connectingDataBase);

            try
            {
                var con = new FbConnection($@"User={_options.UserName};Password={_options.Password};Database={_options.DataBase};DataSource={_options.ServerName};");
                con.Open();
                dataBase = con;
                return true;
            }
            catch (Exception e)
            {
                _log.ErrorLn(e.Message);
                return false;
            }
        }

        public void Execute()
        {
            _log.MessageLn("");
            if (ConnectDataBase())
            {
                tables = (Tables)GetObject<Tables>();
                triggers = (Triggers)GetObject<Triggers>();
                procedures = (Procedures)GetObject<Procedures>();
                uniqCheks = (UniquesAndChecks)GetObject<UniquesAndChecks>();

                _log.MessageLn("");

                triggers.RemoveAll();
                procedures.RemoveAll();
                tables.RemoveAll();
                uniqCheks.RemoveAll();
            }

        }

        private BaseDomain GetObject<T>() where T : BaseDomain
        {
            //var obj = Activator.CreateInstance<T>();
            var obj = (T) Activator.CreateInstance(typeof(T), dataBase, _log);
            //obj.SetConnection(dataBase);
            //obj.Load();
            return obj;
        }
    }
}

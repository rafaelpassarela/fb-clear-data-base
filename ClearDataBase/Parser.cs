using Domain;
using FirebirdSql.Data.FirebirdClient;
using LocalizationHelper;
using LogUtils;
using Schemas;
using System;
using System.Data;
using System.IO;
using System.Linq;

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
                InitSchemas();

                tables = (Tables)GetObject<Tables>();
                triggers = (Triggers)GetObject<Triggers>();
                procedures = (Procedures)GetObject<Procedures>();
                uniqCheks = (UniquesAndChecks)GetObject<UniquesAndChecks>();

                _log.MessageLn("");

                try
                {
                    triggers.RemoveAll();
                    procedures.RemoveAll();
                    tables.RemoveAll();
                    uniqCheks.RemoveAll();
                }
                finally
                {
                    SaveSQLLog();
                }
            }
        }

        private void InitSchemas()
        {
            var fb = new FbSchema((FbConnection)dataBase, _log);
            fb.Initialize();
        }

        private void SaveSQLLog()
        {
            string fileName = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\SQLs";
            Directory.CreateDirectory(fileName);

            fileName = fileName + $"\\SQL_{DateTime.Now.ToString("yyyy_MM_dd-HHmmss")}.sql";
            DoSaveSQLLog(fileName, triggers);
            DoSaveSQLLog(fileName, procedures);
            DoSaveSQLLog(fileName, tables);
            DoSaveSQLLog(fileName, uniqCheks);

            fileName = fileName.Replace(".sql", "_Rollback.sql");
            DoSaveRollbackSQLLog(fileName, triggers);
            DoSaveRollbackSQLLog(fileName, procedures);
            DoSaveRollbackSQLLog(fileName, tables);
            DoSaveRollbackSQLLog(fileName, uniqCheks);
        }

        private void DoSaveSQLLog(string fileName, BaseDomain obj)
        {
            using (StreamWriter sw = new StreamWriter(fileName, File.Exists(fileName)))
            {
                foreach (var item in obj.GetProcessedItems().OrderBy(x => x.ExecOrder))
                {
                    sw.WriteLine(item.SQL);
                }
                sw.Close();
            }
        }

        private void DoSaveRollbackSQLLog(string fileName, BaseDomain obj)
        {
            using (StreamWriter sw = new StreamWriter(fileName, File.Exists(fileName)))
            {
                foreach (var item in obj.GetProcessedItems().OrderByDescending(x => x.ExecOrder))
                {
                    sw.WriteLine(item.RevertSQL);
                }
                sw.Close();
            }
        }

        private BaseDomain GetObject<T>() where T : BaseDomain
        {
            //var obj = Activator.CreateInstance<T>();
            var obj = (T)Activator.CreateInstance(typeof(T), dataBase, _log);
            //obj.SetConnection(dataBase);
            //obj.Load();
            return obj;
        }
    }
}

using Domain;
using FirebirdSql.Data.FirebirdClient;
using LocalizationHelper;
using LogUtils;
using Schemas;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

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
        private Indexes indexes;
        private FbSchema fbSchema;

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

                _log.MessageLn("");

                tables = (Tables)GetObject<Tables>();
                indexes = (Indexes)GetObject<Indexes>();
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
                    indexes.RemoveAll();
                }
                finally
                {
                    SaveSQLLog();
                }
            }
        }

        private void InitSchemas()
        {
            fbSchema = new FbSchema((FbConnection)dataBase, _log);
            fbSchema.Initialize();
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
            DoSaveSQLLog(fileName, indexes);

            fileName = fileName.Replace(".sql", "_Rollback.sql");
            DoSaveRollbackSQLLog(fileName, procedures);
            DoSaveRollbackSQLLog(fileName, triggers);           
            DoSaveRollbackSQLLog(fileName, tables);
            DoSaveRollbackSQLLog(fileName, uniqCheks);
            DoSaveRollbackSQLLog(fileName, indexes);
        }

        private void DoSaveSQLLog(string fileName, BaseDomain obj)
        {
            using (StreamWriter sw = new StreamWriter(fileName, File.Exists(fileName), _options.GetEncoding()))
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
            using (StreamWriter sw = new StreamWriter(fileName, File.Exists(fileName), _options.GetEncoding()))
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
            var obj = (T)Activator.CreateInstance(typeof(T), dataBase, _log, fbSchema);
            return obj;
        }
    }
}

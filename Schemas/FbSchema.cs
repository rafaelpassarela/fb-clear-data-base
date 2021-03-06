﻿using FirebirdSql.Data.FirebirdClient;
using LocalizationHelper;
using LogUtils;
using System;
using System.Data;
using System.IO;

namespace Schemas
{
    public class FbSchema
    {

        /* 
         * Users - X
         * Databases - X
         * Tables - OK
         * Columns - OK
         * StructuredTypeMembers - X
         * Views - OK
         * ViewColumns - OK
         * ProcedureParameters - OK
         * Procedures - OK
         * IndexColumns - OK
         * Indexes - OK
         * UserDefinedTypes - X
         * ForeignKeys - OK
         * PrimaryKeys - OK
         * Triggers - Ok
         */

        private readonly FbConnection _con;
        private readonly ILogWriter _log;
        private string _workPath;

        public FbSchemaBaseCollection<FbSchemaTrigger> Triggers { get; private set; } = new FbSchemaBaseCollection<FbSchemaTrigger>();
        public FbSchemaProcedureCollection<FbSchemaProcedure> Procedures { get; private set; } = new FbSchemaProcedureCollection<FbSchemaProcedure>();
        public FbSchemaKeysCollection Keys { get; private set; } = new FbSchemaKeysCollection();

        private FbSchemaBaseCollection<FbSchemaProcedureParam> ProcParams { get; set; } = new FbSchemaBaseCollection<FbSchemaProcedureParam>();

        public FbSchema(FbConnection connection, ILogWriter log)
        {
            _con = connection;
            _log = log;
        }

        public void Initialize()
        {
            _workPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Schema";
            Directory.CreateDirectory(_workPath);

            foreach (var file in Directory.EnumerateFiles(_workPath, "*.log"))
            {
                File.Delete(file);
            }

            SaveSchema("Triggers", Triggers);
            SaveSchema("Procedures", Procedures);
            SaveSchema("ProcedureParameters", ProcParams);
            SaveSchema("PrimaryKeys", Keys.PrimaryKeys);
            SaveSchema("ForeignKeys", Keys.ForeignKeys);
            SaveSchema("ForeignKeyColumns", Keys.ForeignKeyColumns);
            SaveSchema("Indexes", Keys.Indexes);
            SaveSchema("IndexColumns", Keys.IndexesColumns);
            SaveSchema("CheckConstraintsByTable", Keys.CheckConstraints);
            //SaveSchema("CheckConstraints", Keys.CheckConstraints);

            //SaveSchema("Tables");
            //SaveSchema("Columns");
            //SaveSchema("Views");
            //SaveSchema("ViewColumns");

            Procedures.ProcessParameters(ProcParams);
        }

        private DataTable GetShema(string name)
        {
            return _con.GetSchema(name);
        }

        private void SaveSchema<T>(string schema, FbSchemaBaseCollection<T> collection) where T : IFbSchemaItem
        {
            IFbSchemaItem item;
            string fileName = $"{_workPath}\\{schema}.log";

            try
            {
                _log.MessageLn($"{strings.extractingSchema} {schema}...");
                var table = GetShema(schema);

                using (StreamWriter sw = new StreamWriter(fileName, File.Exists(fileName)))
                {
                    foreach (DataRow row in table.Rows)
                    {
                        item = collection.GetNewItem();
                        item.ProcessDataRow(row);

                        foreach (DataColumn col in table.Columns)
                        {
                            sw.WriteLine("{0} = {1}", col.ColumnName, row[col]);
                        }
                        sw.WriteLine("============================");
                    }
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                _log.ErrorLn($"{strings.error} {schema} - {e.Message}");
            }
        }
    }
}

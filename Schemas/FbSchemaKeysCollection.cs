using LocalizationHelper;
using System;
using System.Linq;

namespace Schemas
{
    public class FbSchemaKeysCollection
    {
        public FbSchemaBaseCollection<FbSchemaPrimaryKey> PrimaryKeys { get; private set; } = new FbSchemaBaseCollection<FbSchemaPrimaryKey>();
        public FbSchemaBaseCollection<FbSchemaForeignKey> ForeignKeys { get; private set; } = new FbSchemaBaseCollection<FbSchemaForeignKey>();
        public FbSchemaBaseCollection<FbSchemaForeignKeyColumns> ForeignKeyColumns { get; private set; } = new FbSchemaBaseCollection<FbSchemaForeignKeyColumns>();
        public FbSchemaBaseCollection<FbSchemaIndexes> Indexes { get; private set; } = new FbSchemaBaseCollection<FbSchemaIndexes>();
        public FbSchemaBaseCollection<FbSchemaIndexesColumns> IndexesColumns { get; private set; } = new FbSchemaBaseCollection<FbSchemaIndexesColumns>();

        public string GetCreateSQLForItem(string itemName)
        {
            // locate the item
            var idx = Indexes.Items.FirstOrDefault(x => x.GetName() == itemName);
            if (idx == null)
            {
                throw new Exception($"{strings.error}> [{itemName}] {strings.notFound}");
            }

            if ((idx as FbSchemaIndexes).Primary)
            {
                return GetPrimaryKeySQL(idx.GetName());
            }

            // locate as FK
            idx = ForeignKeys.Items.FirstOrDefault(x => x.GetName() == itemName);
            if (idx != null)
            {
                return GetForeignKeySQL((idx as FbSchemaForeignKey));
            }

            var sql = "";
            return sql;
        }

        private string GetForeignKeySQL(FbSchemaForeignKey fkObject)
        {
            var fields = "";
            var refFields = "";
            foreach (FbSchemaForeignKeyColumns item in ForeignKeyColumns.Items.Where(x => x.GetName() == fkObject.Name).OrderBy(y => IntfToFKColumn(y).Position))
            {
                fields += (!string.IsNullOrEmpty(fields) ? ", " : "") + item.ColumnName;
                refFields += (!string.IsNullOrEmpty(refFields) ? ", " : "") + item.ReferencedColumnName;
            }

            return $"ALTER TABLE {fkObject.TableName} ADD CONSTRAINT {fkObject.Name} FOREIGN KEY({fields}) REFERENCES {fkObject.ReferencedTable}({refFields}) {fkObject.GetDeleteRule()} {fkObject.GetUpdateRule()};";
        }

        private string GetPrimaryKeySQL(string name)
        {
            var fields = "";
            var table = "";
            foreach (FbSchemaPrimaryKey item in PrimaryKeys.Items.Where(x => x.GetName() == name).OrderBy(y => IntfToPrimaryKey(y).Postition))
            {
                table = item.GetMasterName();
                fields += (!string.IsNullOrEmpty(fields) ? ", " : "") + item.ColumnName;
            }
            return $"ALTER TABLE {table} ADD CONSTRAINT {name} PRIMARY KEY({fields});"; //USING INDEX PK_A02AABA
        }

        private FbSchemaPrimaryKey IntfToPrimaryKey(IFbSchemaItem interfacedObject)
        {
            return (FbSchemaPrimaryKey)interfacedObject;
        }

        private FbSchemaForeignKeyColumns IntfToFKColumn(IFbSchemaItem interfacedObject)
        {
            return (FbSchemaForeignKeyColumns)interfacedObject;
        }
    }
}

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
        public FbSchemaBaseCollection<FbSchemaCheckConstraint> CheckConstraints { get; private set; } = new FbSchemaBaseCollection<FbSchemaCheckConstraint>();

        public string GetCreateSQLForItem(string itemName)
        {
            // locate the item
            var idx = Indexes.Items.FirstOrDefault(x => x.GetName() == itemName);
            if (idx == null)
            {
                //if not, find any check
                idx = CheckConstraints.Items.FirstOrDefault(x => x.GetName() == itemName);
                if (idx == null)
                {
                    throw new Exception($"{strings.error} 01> - [{itemName}] {strings.notFound}");
                }

                return GetCheckConstraintSQL((idx as FbSchemaCheckConstraint));
            }

            if ((idx as FbSchemaIndexes).Primary)
            {
                return GetPrimaryKeySQL(idx.GetName());
            }

            if ((idx as FbSchemaIndexes).Unique)
            {
                return GetUniqueConstraint((idx as FbSchemaIndexes));
            }

            // locate as FK
            var fkIdx = ForeignKeys.Items.FirstOrDefault(x => x.GetName() == itemName);
            if (fkIdx != null)
            {
                return GetForeignKeySQL((fkIdx as FbSchemaForeignKey));
            }

            if ((idx as FbSchemaIndexes).IndexType == "0" || (idx as FbSchemaIndexes).IndexType == "1")
            {
                return GetIndexConstraint((idx as FbSchemaIndexes));
            }

            throw new Exception($"{strings.error} 02> - [{itemName}] {strings.notFound}");
        }

        private string GetIndexConstraint(FbSchemaIndexes idx)
        {
            var fields = "";
            foreach (FbSchemaIndexesColumns item in IndexesColumns.Items.Where(x => x.GetName() == idx.Name).OrderBy(y => IntfToIdxColumn(y).Position))
            {
                fields += (!string.IsNullOrEmpty(fields) ? ", " : "") + item.ColumnName;
            }
            return $"create {(idx.IndexType == "1" ? "descending " : "")}index {idx.Name} on {idx.TableName}({fields});";
        }

        private string GetUniqueConstraint(FbSchemaIndexes idx)
        {
            var fields = "";
            foreach (FbSchemaIndexesColumns item in IndexesColumns.Items.Where(x => x.GetName() == idx.Name).OrderBy(y => IntfToIdxColumn(y).Position))
            {
                fields += (!string.IsNullOrEmpty(fields) ? ", " : "") + item.ColumnName;
            }

            return $"alter table {idx.TableName} add constraint {idx.Name} unique ({fields});";
        }

        private string GetCheckConstraintSQL(FbSchemaCheckConstraint obj)
        {
            return $"alter table {obj.TableName} add constraint {obj.Name} {obj.Source};";
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

            return $"alter table {fkObject.TableName} add constraint {fkObject.Name} foreign key({fields}) references {fkObject.ReferencedTable}({refFields}) {fkObject.GetDeleteRule()} {fkObject.GetUpdateRule()};";
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
            return $"alter table {table} add constraint {name} primary key({fields});"; //USING INDEX PK_A02AABA
        }

        private FbSchemaIndexesColumns IntfToIdxColumn(IFbSchemaItem interfacedObject) => (FbSchemaIndexesColumns)interfacedObject;

        private FbSchemaPrimaryKey IntfToPrimaryKey(IFbSchemaItem interfacedObject) => (FbSchemaPrimaryKey)interfacedObject;

        private FbSchemaForeignKeyColumns IntfToFKColumn(IFbSchemaItem interfacedObject) => (FbSchemaForeignKeyColumns)interfacedObject;
    }
}

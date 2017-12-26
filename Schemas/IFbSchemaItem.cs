using System.Data;

namespace Schemas
{
    public interface IFbSchemaItem
    {
        void ProcessDataRow(DataRow row);
        string GetName();
        string GetMasterName();
        string GetCreateSQL();
    }
}
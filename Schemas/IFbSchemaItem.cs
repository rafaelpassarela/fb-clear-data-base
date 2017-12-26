using System.Data;

namespace Schemas
{
    public interface IFbSchemaItem
    {
        void ProcessDataRow(DataRow row);
        //string GetCreateSQL();
    }
}
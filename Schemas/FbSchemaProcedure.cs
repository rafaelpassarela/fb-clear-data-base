using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schemas
{
    public class FBSchemaProcedure : IFbSchemaItem
    {
        public string Name { get; set; }

        public string GetCreateSQL()
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            return Name;
        }

        public void ProcessDataRow(DataRow row)
        {
            throw new NotImplementedException();
        }
    }
}

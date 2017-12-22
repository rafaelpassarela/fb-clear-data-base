using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schemas
{
    public class FbSchemaTriggerCollection : FbSchemaBaseCollection
    {
        protected override FbSchemaBaseItem DoGetNewItem()
        {
            return new FbSchemaTrigger();
        }
    }
}

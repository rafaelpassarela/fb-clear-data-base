using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schemas
{
    public class FbSchemaProcedureCollection<T> : FbSchemaBaseCollection<T> where T : IFbSchemaItem
    {
        public void ProcessParameters(FbSchemaBaseCollection<FbSchemaProcedureParam> paramItems)
        {
            foreach (FbSchemaProcedure proc in Items)
            {
                foreach (var param in paramItems.Items.Where(x => x.GetMasterName() == proc.Name))
                {
                    proc.Params.Add( (FbSchemaProcedureParam) param );
                }
                //paramItems.Items.Where(x => x.GetMasterName().ToUpper() == proc.Name.ToUpper()).ToList().CopyTo(proc.Params.Items.AsEnumerable());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPP
{
    public class ServerType
    {

        private void storeProcedure(SAPbobsCOM.BoDataServerTypes serverType, string query, string param1, string param2, string param3)
        {
            if (serverType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
            {
                query = string.Format(" CALL {0} ('{1}', '{2}') ", query, param1, param2);
            }
            else
            {
                query = string.Format(" EXEC {0} '{1}', '{2}' ", query, param1, param2);
            }

        }

    }
}

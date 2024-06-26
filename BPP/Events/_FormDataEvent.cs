 
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPP
{
    public class _FormDataEvent
    {
        public void DoAction(ref BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                switch (BusinessObjectInfo.FormTypeEx)
                {

                    case "UDO_FT_BPP_CTADEST3":

                        CuentasDestinoFormDataEvent cuentasDestinoFormDataEvent = new CuentasDestinoFormDataEvent();
                        cuentasDestinoFormDataEvent.dataFormAction(ref BusinessObjectInfo, out BubbleEvent);
                        break;
                    case "UDO_FT_BPP_PAGM4":

                        PagosMasivosProveedoresGenerateDataEvent pagosMasivosProveedoresGenerateDataEvent = new PagosMasivosProveedoresGenerateDataEvent();
                        pagosMasivosProveedoresGenerateDataEvent.dataFormAction(ref BusinessObjectInfo, out BubbleEvent);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                // SAPMain.MensajeError(ex.Message);
            }
        }
    }
}

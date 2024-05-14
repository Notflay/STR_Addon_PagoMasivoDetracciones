 
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPP
{
    public class _ItemEvent
    {
        public void DoAction(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {


                switch (pVal.FormTypeEx)
                {
                    case "141":
                        PurchaseInvoiceItemEvent purchaseInvoiceItemEvent = new PurchaseInvoiceItemEvent();
                        purchaseInvoiceItemEvent.itemAction(FormUID, ref pVal, out BubbleEvent);
                        break;
                    case "181":
                        PurchaseCreditNoteItemEvent purchaseCreditNoteItemEvent = new PurchaseCreditNoteItemEvent();
                        purchaseCreditNoteItemEvent.itemAction(FormUID, ref pVal, out BubbleEvent);
                        break;
                    case "UDO_FT_BPP_CTADEST3":
                        CuentasDestinoItemEvent cuentasDestinoItemEvent = new CuentasDestinoItemEvent();
                        cuentasDestinoItemEvent.itemAction(FormUID, ref pVal, out BubbleEvent);
                        break;
                    case "UDO_FT_BPP_PAGM2":
                        PagosMasivosProveedoresItemEvent pagosMasivosProveedoresItemEvent = new PagosMasivosProveedoresItemEvent();
                        pagosMasivosProveedoresItemEvent.itemAction(FormUID, ref pVal, out BubbleEvent);

                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {

                //SAPMain.MensajeError(ex.Message);
            }

        }
    }
}

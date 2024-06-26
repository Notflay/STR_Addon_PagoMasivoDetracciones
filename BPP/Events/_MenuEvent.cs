 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPP
{
    public class _MenuEvent
    {

        public void DoAction(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {

            BubbleEvent = true;
            SAPbouiCOM.Form oForm = null;

            try
            {

                if (!pVal.BeforeAction)
                {
                    switch (pVal.MenuUID)
                    {
                        case "mnuCtaDest":
                            CuentasDestinoUI cuentasDestinoUI = new CuentasDestinoUI();
                            break;

                        case "mnuParam":
                            ParametrizacionesUI parametrizacionesUI = new ParametrizacionesUI();
                            break;
                        case "mnuPagos":
                            PagosMasivosProveedoresUI pagosMasivosProveedoresUI = new PagosMasivosProveedoresUI();
                            break;
                    }

                    try
                    {
                        oForm = SAPMain.SBO_Application.Forms.ActiveForm;
                    }
                    catch
                    {
                        return;
                    }
                    if (oForm.TypeEx == "UDO_FT_BPP_CTADEST3")
                    {
                        CuentasDestinoMenuEvent cuentasDestinoMenuEvent = new CuentasDestinoMenuEvent();
                        cuentasDestinoMenuEvent.menuAction(ref pVal, out BubbleEvent);
                    }

                    if (oForm.TypeEx == "UDO_FT_BPP_PAGM4")
                    {

                        PagosMasivosProveedoresMenuEvent pagosMasivosProveedoresMenuEvent = new PagosMasivosProveedoresMenuEvent();
                        pagosMasivosProveedoresMenuEvent.menuAction(ref pVal, out BubbleEvent);

                    }
                }

                else
                {

                    try
                    {
                        oForm = SAPMain.SBO_Application.Forms.ActiveForm;
                    }
                    catch
                    {
                        return;
                    }
                    if (oForm.TypeEx == "UDO_FT_BPP_CTADEST3")
                    {
                        CuentasDestinoMenuEvent cuentasDestinoMenuEvent = new CuentasDestinoMenuEvent();
                        cuentasDestinoMenuEvent.menuAction(ref pVal, out BubbleEvent);
                    }

                    if (oForm.TypeEx == "UDO_FT_BPP_PAGM4")
                    {

                        PagosMasivosProveedoresMenuEvent pagosMasivosProveedoresMenuEvent = new PagosMasivosProveedoresMenuEvent();
                        pagosMasivosProveedoresMenuEvent.menuAction(ref pVal, out BubbleEvent);

                    }
                }

            }
            catch (Exception ex)
            {
                //SAPMain.MensajeError(ex.Message);
            }
        }
    }
}

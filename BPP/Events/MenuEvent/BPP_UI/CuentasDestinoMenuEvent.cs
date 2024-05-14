 
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPP
{
    public class CuentasDestinoMenuEvent
    {
        private SAPbouiCOM.Form oForm;
        public void menuAction(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            this.oForm = SAPMain.SBO_Application.Forms.Item("UDO_FT_BPP_CTADEST3");
            oForm.Select();
            try
            {
                if (pVal.BeforeAction)
                {

                    switch (pVal.MenuUID)
                    {

                        case "1284":

                            break;


                    }

                }
                else
                {
                    switch (pVal.MenuUID)
                    {
                        case "1281":

                            oForm.Items.Item("0_U_E").Enabled = true;

                            break;

                        case "1282":

                            oForm.Items.Item("txtFeccre").Specific.Value = DateTime.Now.ToString("yyyyMMdd");
                            ((SAPbouiCOM.ComboBox)(oForm.Items.Item("txtEstado").Specific)).Select("Creado");

                            break;

                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}

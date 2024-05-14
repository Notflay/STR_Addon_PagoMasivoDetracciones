using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPP
{
    public class CuentasDestinoFormDataEvent
    {
        private SAPbouiCOM.Form oForm;
        public void dataFormAction(ref SAPbouiCOM.BusinessObjectInfo pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.FormTypeEx == "UDO_FT_BPP_CTADEST3")
            {
                this.oForm = SAPMain.SBO_Application.Forms.Item(pVal.FormUID);
                if (pVal.EventType == BoEventTypes.et_FORM_DATA_LOAD && !pVal.BeforeAction)
                {
                    setearCampos();
                }
            }
        }

        private void setearCampos()
        {
            try
            {

                Item oItem = oForm.Items.Item("txtEstado");
                ComboBox oCombo = (ComboBox)oItem.Specific;

                if (oCombo.Value == "Procesado")
                {
                    oItem = oForm.Items.Item("txtFeccre");
                    oItem.Enabled = false;
                    oItem = oForm.Items.Item("txtEstado");
                    oItem.Enabled = false;
                    oItem = oForm.Items.Item("21_U_E");
                    oItem.Enabled = false;
                    oItem = oForm.Items.Item("txtPeriodo");
                    oItem.Enabled = false;
                    oItem = oForm.Items.Item("btnConsult");
                    oItem.Enabled = false;

                    oItem = oForm.Items.Item("matDet1");
                    oItem.Enabled = false;


                }
                else
                {
                    oItem = oForm.Items.Item("btnConsult");
                    oItem.Enabled = true;

                    oItem = oForm.Items.Item("txtFeccre");
                    oItem.Enabled = true;
                    oItem = oForm.Items.Item("txtEstado");
                    oItem.Enabled = true;
                    oItem = oForm.Items.Item("21_U_E");
                    oItem.Enabled = true;
                    oItem = oForm.Items.Item("txtPeriodo");
                    oItem.Enabled = true;
                    oItem = oForm.Items.Item("matDet1");
                    oItem.Enabled = true;


                }



            }
            catch (Exception ex)
            {
                SAPMain.MensajeError(ex.Message);
            }
        }
    }
}

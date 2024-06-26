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
                Item item = oForm.Items.Item("txtEstado");
                ComboBox comboBox = (ComboBox)(dynamic)item.Specific;
                if (comboBox.Value == "Procesado")
                {
                    item = oForm.Items.Item("txtFeccre");
                    item.Enabled = false;
                    item = oForm.Items.Item("txtEstado");
                    item.Enabled = false;
                    item = oForm.Items.Item("21_U_E");
                    item.Enabled = false;
                    item = oForm.Items.Item("txtPeriodo");
                    item.Enabled = false;
                    item = oForm.Items.Item("btnConsult");
                    item.Enabled = false;
                    item = oForm.Items.Item("matDet1");
                    item.Enabled = false;
                }
                else
                {
                    item = oForm.Items.Item("btnConsult");
                    item.Enabled = true;
                    item = oForm.Items.Item("txtFeccre");
                    item.Enabled = true;
                    item = oForm.Items.Item("txtEstado");
                    item.Enabled = true;
                    item = oForm.Items.Item("21_U_E");
                    item.Enabled = true;
                    item = oForm.Items.Item("txtPeriodo");
                    item.Enabled = true;
                    item = oForm.Items.Item("matDet1");
                    item.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                SAPMain.MensajeError(ex.Message);
            }
        }
    }
}

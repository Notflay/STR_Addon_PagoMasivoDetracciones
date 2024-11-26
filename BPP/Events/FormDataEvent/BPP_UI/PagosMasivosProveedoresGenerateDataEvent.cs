using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPP
{
    public class PagosMasivosProveedoresGenerateDataEvent
    {
        private SAPbouiCOM.Form oForm;
        public void dataFormAction(ref SAPbouiCOM.BusinessObjectInfo pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.FormTypeEx == "UDO_FT_BPP_PAGM4")
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
                Item item = oForm.Items.Item("22_U_E");
                ComboBox comboBox = (ComboBox)(dynamic)item.Specific;
                if (comboBox.Value == "Creado" || comboBox.Value == "Procesado" || comboBox.Value == "Cancelado")
                {
                    item = oForm.Items.Item("txtFecini");
                    item.Enabled = false;
                    item = oForm.Items.Item("txtFecfin");
                    item.Enabled = false;
                    item = oForm.Items.Item("txtFecvini");
                    item.Enabled = false;
                    item = oForm.Items.Item("txtFecvfin");
                    item.Enabled = false;
                    item = oForm.Items.Item("txtCodprov");
                    item.Enabled = false;
                    item = oForm.Items.Item("cbxFiltro");
                    item.Enabled = false;
                    item = oForm.Items.Item("edtFiltval");
                    item.Enabled = false;
                    item = oForm.Items.Item("cbxFljCj");
                    item.Enabled = false;
                    item = oForm.Items.Item("22_U_E");
                    item.Enabled = false;
                    item = oForm.Items.Item("0_U_E");
                    item.Enabled = false;
                    item = oForm.Items.Item("txtSerie");
                    item.Enabled = false;
                    item = oForm.Items.Item("txtFeccrea");
                    item.Enabled = false;
                    item = oForm.Items.Item("btnConsult");
                    item.Enabled = false;
                    item = oForm.Items.Item("txtCuenban");
                    item.Enabled = false;
                    item = oForm.Items.Item("cmbTippago");
                    item.Enabled = false;
                    item = oForm.Items.Item("matDet1");
                    item.Enabled = false;
                    Matrix matrix = (Matrix)(dynamic)oForm.Items.Item("matDet1").Specific;
                    if (comboBox.Value == "Procesado" || comboBox.Value == "Cancelado")
                    {
                        item = oForm.Items.Item("txtFeceje");
                        item.Enabled = false;
                        item = oForm.Items.Item("btnProc");
                        item.Enabled = false;
                        item = oForm.Items.Item("cmbTippago");
                        item.Enabled = false;
                        item = oForm.Items.Item("txtPagodet");
                        item.Enabled = SAPMain.opcionPagoMasivo != 1;
                    }

                    if (comboBox.Value == "Creado" && ((dynamic)oForm.Items.Item("0_U_E").Specific).Value != "")
                    {
                        item = oForm.Items.Item("btnProc");
                        item.Enabled = true;
                        item = oForm.Items.Item("txtFeceje");
                        item.Enabled = true;
                        item = oForm.Items.Item("txtPagodet");
                        item.Enabled = true;
                        item = oForm.Items.Item("cmbTippago");
                        item.Enabled = true;
                    }
                }
                else
                {
                    Matrix matrix2 = (Matrix)(dynamic)oForm.Items.Item("matDet1").Specific;
                    item = oForm.Items.Item("txtFeceje");
                    item.Enabled = true;
                    item = oForm.Items.Item("txtFecvini");
                    item.Enabled = true;
                    item = oForm.Items.Item("txtFecvfin");
                    item.Enabled = true;
                    item = oForm.Items.Item("txtCodprov");
                    item.Enabled = true;
                    item = oForm.Items.Item("txtPagodet");
                    item.Enabled = true;
                    item = oForm.Items.Item("matDet1");
                    item.Enabled = true;
                    item = oForm.Items.Item("cmbTippago");
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

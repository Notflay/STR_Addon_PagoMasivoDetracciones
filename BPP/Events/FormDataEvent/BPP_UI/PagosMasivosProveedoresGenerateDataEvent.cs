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
            if (pVal.FormTypeEx == "UDO_FT_BPP_PAGM2")
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

                Item oItem = oForm.Items.Item("22_U_E");
                ComboBox oCombo = (ComboBox)oItem.Specific;

                if (oCombo.Value == "Creado" || oCombo.Value == "Procesado" || oCombo.Value == "Cancelado")
                {
                    oItem = oForm.Items.Item("txtFecini");
                    oItem.Enabled = false;
                    oItem = oForm.Items.Item("txtFecfin");
                    oItem.Enabled = false;
                    oItem = oForm.Items.Item("txtFecvini");
                    oItem.Enabled = false;
                    oItem = oForm.Items.Item("txtFecvfin");
                    oItem.Enabled = false;
                    oItem = oForm.Items.Item("txtCodprov");
                    oItem.Enabled = false;
                    oItem = oForm.Items.Item("22_U_E");
                    oItem.Enabled = false;
                    oItem = oForm.Items.Item("0_U_E");
                    oItem.Enabled = false;
                    oItem = oForm.Items.Item("txtSerie");
                    oItem.Enabled = false;
                    oItem = oForm.Items.Item("txtFeccrea");
                    oItem.Enabled = false;
                    oItem = oForm.Items.Item("btnConsult");
                    oItem.Enabled = false;
                    oItem = oForm.Items.Item("txtCuenban");
                    oItem.Enabled = false;
                    oItem = oForm.Items.Item("cmbTippago");
                    oItem.Enabled = false;
                    oItem = oForm.Items.Item("matDet1");
                    oItem.Enabled = false;

                    //oItem = oForm.Items.Item("chckGen");
                    //oItem.Enabled = false;

                    Matrix oMatrix = (Matrix)oForm.Items.Item("matDet1").Specific;
                    if (oCombo.Value == "Procesado" || oCombo.Value == "Cancelado")
                    {

                        oItem = oForm.Items.Item("txtFeceje");
                        oItem.Enabled = false;
                        oItem = oForm.Items.Item("btnProc");
                        oItem.Enabled = false;
                        oItem = oForm.Items.Item("cmbTippago");
                        oItem.Enabled = false;
                        //oMatrix.Columns.Item("colCheck").Editable = false;

                        oItem = oForm.Items.Item("txtPagodet");
                        oItem.Enabled = false;
                    }

                    if (oCombo.Value == "Creado" && oForm.Items.Item("0_U_E").Specific.Value != "")
                    {
                        oItem = oForm.Items.Item("btnProc");
                        oItem.Enabled = true;
                        //oMatrix.Columns.Item("colCheck").Editable = false;
                        oItem = oForm.Items.Item("txtFeceje");
                        oItem.Enabled = true;
                        oItem = oForm.Items.Item("txtPagodet");
                        oItem.Enabled = true;
                        oItem = oForm.Items.Item("cmbTippago");
                        oItem.Enabled = true;
                    }



                }
                else
                {
                    Matrix oMatrix = (Matrix)oForm.Items.Item("matDet1").Specific;

                    oItem = oForm.Items.Item("txtFeceje");
                    oItem.Enabled = true;
                    oItem = oForm.Items.Item("txtFecvini");
                    oItem.Enabled = true;
                    oItem = oForm.Items.Item("txtFecvfin");
                    oItem.Enabled = true;
                    oItem = oForm.Items.Item("txtCodprov");
                    oItem.Enabled = true;
                    oItem = oForm.Items.Item("txtPagodet");
                    oItem.Enabled = true;
                    oItem = oForm.Items.Item("matDet1");
                    oItem.Enabled = true;
                    oItem = oForm.Items.Item("cmbTippago");
                    oItem.Enabled = true;
                    //oMatrix.Columns.Item("colCheck").Editable = true;

                }



            }
            catch (Exception ex)
            {
                SAPMain.MensajeError(ex.Message);
            }
        }
    }
}

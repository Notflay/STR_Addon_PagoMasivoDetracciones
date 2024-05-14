 
using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPP
{
    public class CuentasDestinoUI : FormGUI
    {
        private SAPbouiCOM.Form oForm;
        Recordset oRecordSet = null;

        public CuentasDestinoUI()
        {
            try
            {
                string transTemp0 = "Resources/Localizacion/frmCtaDestino.srf";
                LoadFromXML(ref transTemp0);
                this.oForm = SAPMain.SBO_Application.Forms.Item("UDO_FT_BPP_CTADEST3");
                periodo();
                cargarTotales();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void cargarTotales()
        {
            try
            {
                Matrix oMatrix = (Matrix)oForm.Items.Item("matDet1").Specific;

                oMatrix.Columns.Item("C_0_9").ColumnSetting.SumType = BoColumnSumType.bst_Auto;
                oMatrix.Columns.Item("C_0_10").ColumnSetting.SumType = BoColumnSumType.bst_Auto;
                oMatrix.Columns.Item("C_0_11").ColumnSetting.SumType = BoColumnSumType.bst_Auto;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }


        private void periodo()
        {
            try
            {

                oForm.Items.Item("txtFeccre").Specific.Value = DateTime.Now.ToString("yyyyMMdd");
                ComboBox oComboEstado = oForm.Items.Item("txtEstado").Specific;
                oComboEstado.Select("Creado", BoSearchKey.psk_ByValue);

                oRecordSet = (Recordset)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                ComboBox oComboPeriodo = oForm.Items.Item("txtPeriodo").Specific;

                string query = "select \"Code\",\"AbsEntry\" from \"OFPR\" WHERE \"PeriodStat\" = 'N'";
                oRecordSet.DoQuery(query);

                for (int i = 0; i < oRecordSet.RecordCount; i++)
                {
                    string code = oRecordSet.Fields.Item("Code").Value.ToString();
                    string absEntry = oRecordSet.Fields.Item("AbsEntry").Value.ToString();
                    oComboPeriodo.ValidValues.Add(absEntry, code);
                    oRecordSet.MoveNext();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }
    }
}

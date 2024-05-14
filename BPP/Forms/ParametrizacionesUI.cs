 
using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPP
{
    public class ParametrizacionesUI : FormGUI
    {
        private SAPbouiCOM.Form oForm;
        Recordset oRecordSet = null;

        public ParametrizacionesUI()
        {
            try
            {
                string transTemp0 = "Resources/Localizacion/frmParam1.srf";
                LoadFromXML(ref transTemp0);
                this.oForm = SAPMain.SBO_Application.Forms.Item("frmParam");
                openForm();
            }
            catch (Exception ex)
            {
            }
        }

        private void openForm()
        {
            oForm.Mode = SAPbouiCOM.BoFormMode.fm_FIND_MODE;
            oForm.Items.Item("txtCode").Specific.Value = "BPP_CONFIG";
            oForm.Items.Item("1").Click();
            oForm.Items.Item("txtCode").Enabled = false;
            oForm.Items.Item("txtName").Enabled = false;

        }
        private void cargarLogo()
        {
            string sPath = System.Windows.Forms.Application.StartupPath.ToString();
            Button oButton = (Button)oForm.Items.Item("btnLogo").Specific;
            oButton.Type = BoButtonTypes.bt_Image;
            oButton.Image = sPath + "\\Resources\\Imgs\\logo_empresa_1.png";
            oButton = (Button)oForm.Items.Item("btnArch").Specific;
            oButton.Type = BoButtonTypes.bt_Image;
            oButton.Image = sPath + "\\Resources\\Imgs\\boton_archivo_1.png";
            oButton = (Button)oForm.Items.Item("btnProc").Specific;
            oButton.Type = BoButtonTypes.bt_Image;
            oButton.Image = sPath + "\\Resources\\Imgs\\boton_procesar_1.png";
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

                oForm.Items.Item("23_U_E").Specific.Value = DateTime.Now.ToString("yyyyMMdd");
                ComboBox oComboEstado = oForm.Items.Item("22_U_E").Specific;
                oComboEstado.Select("Procesado", BoSearchKey.psk_ByValue);

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

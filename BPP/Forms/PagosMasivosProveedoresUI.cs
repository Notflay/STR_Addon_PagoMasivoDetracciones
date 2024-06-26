
using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BPP
{
    public class PagosMasivosProveedoresUI : FormGUI
    {   
        private SAPbouiCOM.Form oForm;
        Recordset oRecordSet = null;

        public PagosMasivosProveedoresUI()
        {
            try
            {
                string FileName = "Resources/Localizacion/frmPagos.srf";
                LoadFromXML(ref FileName);
                oForm = SAPMain.SBO_Application.Forms.Item("UDO_FT_BPP_PAGM4");
                cargarLogo();
                cargarFormDefault();
                cflCodigoSN();
                cflCuentaBanco();
                cargarTipoPago();
                fn_ListaFlujodeCaja();
                fn_listarFiltros();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void cflCodigoSN()
        {
            try
            {
                ChooseFromListCollection chooseFromLists = oForm.ChooseFromLists;
                SAPbouiCOM.ChooseFromList chooseFromList = null;
                ChooseFromListCreationParams chooseFromListCreationParams = (ChooseFromListCreationParams)(dynamic)SAPMain.SBO_Application.CreateObject(BoCreatableObjectType.cot_ChooseFromListCreationParams);
                chooseFromListCreationParams.MultiSelection = false;
                chooseFromListCreationParams.ObjectType = "2";
                chooseFromListCreationParams.UniqueID = "cflCodigoSN";
                chooseFromList = chooseFromLists.Add(chooseFromListCreationParams);
                Conditions conditions = (Conditions)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("2BF953B9-745C-4CFE-8337-094653DE508B")));
                Condition condition = conditions.Add();
                conditions = (Conditions)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("2BF953B9-745C-4CFE-8337-094653DE508B")));
                condition = conditions.Add();
                condition.Alias = "CardType";
                condition.Operation = BoConditionOperation.co_EQUAL;
                condition.CondVal = "S";
                chooseFromList.SetConditions(conditions);
                EditText editText = (EditText)(dynamic)oForm.Items.Item("txtCodprov").Specific;
                editText.ChooseFromListUID = "cflCodigoSN";
                editText.ChooseFromListAlias = "CardCode";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void cflCuentaBanco()
        {
            try
            {
                ChooseFromListCollection chooseFromLists = oForm.ChooseFromLists;
                SAPbouiCOM.ChooseFromList chooseFromList = null;
                ChooseFromListCreationParams chooseFromListCreationParams = (ChooseFromListCreationParams)(dynamic)SAPMain.SBO_Application.CreateObject(BoCreatableObjectType.cot_ChooseFromListCreationParams);
                chooseFromListCreationParams.MultiSelection = false;
                chooseFromListCreationParams.ObjectType = "231";
                chooseFromListCreationParams.UniqueID = "cflCuentaBn";
                chooseFromList = chooseFromLists.Add(chooseFromListCreationParams);
                Conditions conditions = (Conditions)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("2BF953B9-745C-4CFE-8337-094653DE508B")));
                Condition condition = conditions.Add();
                conditions = (Conditions)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("2BF953B9-745C-4CFE-8337-094653DE508B")));
                condition = conditions.Add();
                condition.Alias = "U_BPP_GENTXT";
                condition.Operation = BoConditionOperation.co_EQUAL;
                condition.CondVal = "Y";
                chooseFromList.SetConditions(conditions);
                EditText editText = (EditText)(dynamic)oForm.Items.Item("txtCuenban").Specific;
                editText.ChooseFromListUID = "cflCuentaBn";
                editText.ChooseFromListAlias = "Account";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void cargarFormDefault()
        {
            SAPbouiCOM.ComboBox comboBox = (dynamic)oForm.Items.Item("txtSerie").Specific;
            comboBox.ValidValues.Add(DateTime.Now.Year.ToString(), DateTime.Now.Year.ToString());
            ((dynamic)oForm.Items.Item("txtFeccrea").Specific).Value = DateTime.Now.ToString("yyyyMMdd");
            ((SAPbouiCOM.ComboBox)(dynamic)oForm.Items.Item("22_U_E").Specific).Select("Creado");
            ((SAPbouiCOM.ComboBox)(dynamic)oForm.Items.Item("txtSerie").Specific).Select(DateTime.Now.Year.ToString());
        }

        private void fn_listarFiltros()
        {
            SAPbouiCOM.ComboBox comboBox = (SAPbouiCOM.ComboBox)(dynamic)oForm.Items.Item("cbxFiltro").Specific;
            Recordset recordset = (Recordset)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            string empty = string.Empty;
            try
            {
                empty = "SELECT \"Code\",\"Name\" FROM \"@BPP_PGM_PARAM\"";
                Global.WriteToFile(empty);
                recordset.DoQuery(empty);
                while (!recordset.EoF)
                {
                    comboBox.ValidValues.Add((dynamic)recordset.Fields.Item(0).Value, (dynamic)recordset.Fields.Item(1).Value);
                    recordset.MoveNext();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                recordset = null;
            }
        }

        private void fn_ListaFlujodeCaja()
        {
            Matrix matrix = (Matrix)(dynamic)oForm.Items.Item("matDet1").Specific;
            SAPbouiCOM.ComboBox comboBox = (SAPbouiCOM.ComboBox)(dynamic)oForm.Items.Item("cbxFljCj").Specific;
            Column column = matrix.Columns.Item("colCshFlw");
            Recordset recordset = (Recordset)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            string empty = string.Empty;
            try
            {
                empty = "SELECT \"CFWId\",\"CFWName\" FROM OCFW WHERE \"Postable\" = 'Y'";
                Global.WriteToFile(empty);
                recordset.DoQuery(empty);
                column.ValidValues.Add("---", "---");
                while (!recordset.EoF)
                {
                    column.ValidValues.Add((dynamic)recordset.Fields.Item(0).Value, (dynamic)recordset.Fields.Item(1).Value);
                    comboBox.ValidValues.Add((dynamic)recordset.Fields.Item(0).Value, (dynamic)recordset.Fields.Item(1).Value);
                    recordset.MoveNext();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                recordset = null;
            }
        }

        private void cargarCuentaContable()
        {
            try
            {
                SAPbouiCOM.ComboBox comboBox = (SAPbouiCOM.ComboBox)(dynamic)oForm.Items.Item("txtCuencon").Specific;
                Recordset recordset = (Recordset)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string empty = string.Empty;
                empty = ((SAPMain.oCompany.DbServerType != BoDataServerTypes.dst_HANADB) ? string.Format("EXEC SP_BPP_PARAMETROS_PGM '{0}' ,'','' ", "CUENTA_CONTABLE") : string.Format("CALL SP_BPP_PARAMETROS_PGM ('{0}','','')  ", "CUENTA_CONTABLE"));
                Global.WriteToFile(empty);
                recordset.DoQuery(empty);
                string empty2 = string.Empty;
                string empty3 = string.Empty;
                for (int i = 0; i < recordset.RecordCount; i++)
                {
                    empty3 = obtieneCuenta(((dynamic)recordset.Fields.Item("Code").Value).ToString(), false);
                    empty2 = ((dynamic)recordset.Fields.Item("Name").Value).ToString();
                    comboBox.ValidValues.Add(empty3, empty2);
                    recordset.MoveNext();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void cargarTipoPago()
        {
            try
            {
                SAPbouiCOM.ComboBox comboBox = (SAPbouiCOM.ComboBox)(dynamic)oForm.Items.Item("cmbTippago").Specific;
                Recordset recordset = (Recordset)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string empty = string.Empty;
                empty = ((SAPMain.oCompany.DbServerType != BoDataServerTypes.dst_HANADB) ? string.Format("EXEC SP_BPP_PARAMETROS_PGM '{0}' ,'','' ", "TIPO_PAGO") : string.Format("CALL SP_BPP_PARAMETROS_PGM ('{0}','','')  ", "TIPO_PAGO"));
                Global.WriteToFile(empty);
                recordset.DoQuery(empty);
                string empty2 = string.Empty;
                string empty3 = string.Empty;
                for (int i = 0; i < recordset.RecordCount; i++)
                {
                    empty2 = ((dynamic)recordset.Fields.Item("Code").Value).ToString();
                    empty3 = ((dynamic)recordset.Fields.Item("Name").Value).ToString();
                    comboBox.ValidValues.Add(empty2, empty3);
                    recordset.MoveNext();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string obtieneCuenta(string code, bool acctCod)
        {
            Recordset recordset = (Recordset)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            string empty = string.Empty;
            try
            {
                if (!SAPMain.segmentado)
                {
                    return code;
                }

                empty = (acctCod ? ("SELECT \"AcctCode\" FROM OACT WHERE \"Segment_0\" = '" + code + "'") : ("SELECT \"Segment_0\" FROM OACT WHERE \"AcctCode\" = '" + code + "' "));
                Global.WriteToFile(empty);
                recordset.DoQuery(empty);
                if (recordset.Fields.Count > 0)
                {
                    return ((dynamic)recordset.Fields.Item(0).Value).ToString();
                }

                SAPMain.MensajeAdvertencia("No se ha creado Segmento para este AcctCode");
                return "";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                recordset = null;
            }
        }

        private void cargarSeries()
        {
            try
            {
                SAPbouiCOM.ComboBox comboBox = (SAPbouiCOM.ComboBox)(dynamic)oForm.Items.Item("Item_4").Specific;
                Recordset recordset = (Recordset)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string text = "SELECT DISTINCT \"Indicator\" FROM OFPR ";
                Global.WriteToFile(text);
                recordset.DoQuery(text);
                string empty = string.Empty;
                for (int i = 0; i < recordset.RecordCount; i++)
                {
                    empty = ((dynamic)recordset.Fields.Item("Indicator").Value).ToString();
                    comboBox.ValidValues.Add(empty, empty);
                    recordset.MoveNext();
                }

                comboBox.Select(DateTime.Now.Year.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void cargarLogo()
        {
            string text = System.Windows.Forms.Application.StartupPath.ToString();
            SAPbouiCOM.Button button = (SAPbouiCOM.Button)(dynamic)oForm.Items.Item("btnLogo").Specific;
            button.Type = BoButtonTypes.bt_Image;
            button.Image = text + "\\Resources\\Imgs\\logo_empresa_1.png";
            button = (SAPbouiCOM.Button)(dynamic)oForm.Items.Item("btnArch").Specific;
            button.Type = BoButtonTypes.bt_Image;
            button.Image = text + "\\Resources\\Imgs\\boton_archivo_1.png";
            button = (SAPbouiCOM.Button)(dynamic)oForm.Items.Item("btnProc").Specific;
            button.Type = BoButtonTypes.bt_Image;
            button.Image = text + "\\Resources\\Imgs\\boton_procesar_1.png";
        }
    }
}

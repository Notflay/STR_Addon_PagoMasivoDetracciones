
using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
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
                string transTemp0 = "Resources/Localizacion/frmPagos.srf";
                LoadFromXML(ref transTemp0);

                this.oForm = SAPMain.SBO_Application.Forms.Item("UDO_FT_BPP_PAGM2");

                cargarLogo();
                cargarFormDefault();
                cflCodigoSN();
                cflCuentaBanco();
                cargarTipoPago();
                cargarCuentaContable();
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
                ChooseFromListCollection oCFLs = this.oForm.ChooseFromLists;
                SAPbouiCOM.ChooseFromList oCFL = null;
                ChooseFromListCreationParams oCFLCreationParams = ((SAPbouiCOM.ChooseFromListCreationParams)(SAPMain.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)));

                oCFLCreationParams.MultiSelection = false;
                oCFLCreationParams.ObjectType = "2";
                oCFLCreationParams.UniqueID = "cflCodigoSN";
                oCFL = oCFLs.Add(oCFLCreationParams);

                Conditions oCons = new Conditions();
                Condition oCon = oCons.Add();
                oCons = new Conditions();
                oCon = oCons.Add();
                oCon.Alias = "CardType";
                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                oCon.CondVal = "S";

                oCFL.SetConditions(oCons);

                EditText txtCodeSN = ((EditText)this.oForm.Items.Item("txtCodprov").Specific);
                txtCodeSN.ChooseFromListUID = "cflCodigoSN";
                txtCodeSN.ChooseFromListAlias = "CardCode";
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
                ChooseFromListCollection oCFLs = this.oForm.ChooseFromLists;
                SAPbouiCOM.ChooseFromList oCFL = null;
                ChooseFromListCreationParams oCFLCreationParams = ((SAPbouiCOM.ChooseFromListCreationParams)(SAPMain.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)));

                oCFLCreationParams.MultiSelection = false;
                oCFLCreationParams.ObjectType = "231";
                oCFLCreationParams.UniqueID = "cflCuentaBn";
                oCFL = oCFLs.Add(oCFLCreationParams);

                Conditions oCons = new Conditions();
                Condition oCon = oCons.Add();
                oCons = new Conditions();

                oCon = oCons.Add();
                oCon.Alias = "U_BPP_GENTXT";
                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                oCon.CondVal = "Y";

                oCFL.SetConditions(oCons);


                EditText txtCodeCB = ((EditText)this.oForm.Items.Item("txtCuenban").Specific);
                txtCodeCB.ChooseFromListUID = "cflCuentaBn";
                txtCodeCB.ChooseFromListAlias = "Account";
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }
        private void cargarFormDefault()
        {
            ComboBox oComboSerie = oForm.Items.Item("txtSerie").Specific;
            oComboSerie.ValidValues.Add(DateTime.Now.Year.ToString(), DateTime.Now.Year.ToString());

            oForm.Items.Item("txtFeccrea").Specific.Value = DateTime.Now.ToString("yyyyMMdd");
            ((SAPbouiCOM.ComboBox)(oForm.Items.Item("22_U_E").Specific)).Select("Creado");
            ((SAPbouiCOM.ComboBox)(oForm.Items.Item("txtSerie").Specific)).Select(DateTime.Now.Year.ToString());

        }

        private void cargarCuentaContable()
        {
            try
            {

                ComboBox oCombo = (ComboBox)oForm.Items.Item("txtCuencon").Specific;

                Recordset oRecordSet = (Recordset)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string query = string.Empty;

                if (SAPMain.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    query = string.Format("CALL SP_BPP_PARAMETROS_PGM ('{0}','','')  ", "CUENTA_CONTABLE");
                }
                else
                {
                    query = string.Format("EXEC SP_BPP_PARAMETROS_PGM '{0}' ,'','' ", "CUENTA_CONTABLE");
                }

                oRecordSet.DoQuery(query);
                string name = string.Empty;
                string code = string.Empty;

                for (int i = 0; i < oRecordSet.RecordCount; i++)
                {
                    code = obtieneCuenta(oRecordSet.Fields.Item("Code").Value.ToString(), false);
                    name = oRecordSet.Fields.Item("Name").Value.ToString();
                    oCombo.ValidValues.Add(code, name);

                    oRecordSet.MoveNext();
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

                ComboBox oCombo = (ComboBox)oForm.Items.Item("cmbTippago").Specific;

                Recordset oRecordSet = (Recordset)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string query = string.Empty;

                if (SAPMain.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    query = string.Format("CALL SP_BPP_PARAMETROS_PGM ('{0}','','')  ", "TIPO_PAGO");
                }
                else
                {
                    query = string.Format("EXEC SP_BPP_PARAMETROS_PGM '{0}' ,'','' ", "TIPO_PAGO");
                }

                oRecordSet.DoQuery(query);
                string code = string.Empty;
                string name = string.Empty;

                for (int i = 0; i < oRecordSet.RecordCount; i++)
                {
                    code = oRecordSet.Fields.Item("Code").Value.ToString();
                    name = oRecordSet.Fields.Item("Name").Value.ToString();
                    oCombo.ValidValues.Add(code, name);

                    oRecordSet.MoveNext();
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string obtieneCuenta(string code, bool acctCod)
        {
            Recordset oRecordSet = (Recordset)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            string qry = string.Empty;
            try
            {

                if (!SAPMain.segmentado)
                    return code;
                else
                {
                    if (!acctCod)
                        qry = $"SELECT \"Segment_0\" FROM OACT WHERE \"AcctCode\" = '{code}' ";
                    else
                        qry = $"SELECT \"AcctCode\" FROM OACT WHERE \"Segment_0\" = '{code}'";

                    oRecordSet.DoQuery(qry);

                    if (oRecordSet.Fields.Count > 0)
                    {
                        return oRecordSet.Fields.Item(0).Value.ToString();
                    }
                    else
                    {
                        SAPMain.MensajeAdvertencia("No se ha creado Segmento para este AcctCode");
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            finally
            {
                oRecordSet = null;
            }
        }
        private void cargarSeries()
        {
            try
            {

                ComboBox oCombo = (ComboBox)oForm.Items.Item("Item_4").Specific;

                Recordset oRecordSet = (Recordset)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string query = "SELECT DISTINCT \"Indicator\" FROM OFPR ";


                oRecordSet.DoQuery(query);
                string code = string.Empty;

                for (int i = 0; i < oRecordSet.RecordCount; i++)
                {
                    code = oRecordSet.Fields.Item("Indicator").Value.ToString();
                    oCombo.ValidValues.Add(code, code);

                    oRecordSet.MoveNext();
                }


                oCombo.Select(DateTime.Now.Year.ToString(), BoSearchKey.psk_ByValue);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
    }
}

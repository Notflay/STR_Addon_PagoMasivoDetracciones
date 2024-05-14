 
using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BPP
{
    public class PagosMasivosProveedoresItemEvent
    {
        private SAPbouiCOM.Form oForm;
        //private static readonly ILog logger = LogManager.GetLogger(typeof(PagosMasivosProveedoresItemEvent));

        public void itemAction(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            oForm = SAPMain.SBO_Application.Forms.Item(pVal.FormUID);
            try
            {
                if (!pVal.BeforeAction)
                {
                    string tipo = "";
                    //logger.Debug(pVal.EventType.ToString());
                    switch (pVal.EventType)
                    {
                        case BoEventTypes.et_CHOOSE_FROM_LIST:
                            if (pVal.ItemUID == "txtCodprov")
                            {

                                cflCodigoSN(ref pVal);
                            }
                            if (pVal.ItemUID == "txtCuenban")
                            {

                                cflCuentaBanco(ref pVal);
                            }
                            break;

                        case BoEventTypes.et_ITEM_PRESSED:


                            if (pVal.ItemUID == "btncom" && (pVal.FormMode == 1 || pVal.FormMode == 2 || pVal.FormMode == 3))
                            {


                                //string TablaHana = "EW_PERDATA";
                                //string matrixDetalle = "matDet1";

                                //if (validardatostablausuario(TablaHana))
                                //{
                                //    removecolumns(matrixDetalle);
                                //    createcolumns(matrixDetalle);

                                //    cargardatostablausuario(matrixDetalle, TablaHana);
                                //}
                                //else
                                //{
                                //    removecolumns(matrixDetalle);
                                //    createcolumns(matrixDetalle);
                                //}

                                //habilitarcamposanexos();
                            }





                            break;

                       


                        case BoEventTypes.et_COMBO_SELECT:

                            //if ((pVal.ItemUID == "matDet1" || pVal.ItemUID == "matDet2") && (pVal.ColUID == "colTipocom") && (pVal.FormMode == 3 || pVal.FormMode == 2))
                            //{
                            //    validarCodigoRecurso(ref pVal, pVal.ItemUID);
                            //}

                            break;


                        case BoEventTypes.et_VALIDATE:


                            if ((pVal.ItemUID == "matDet1" || pVal.ItemUID == "matDet2") && (pVal.ColUID == "colMonpag") && pVal.ItemChanged && (pVal.FormMode == 3 || pVal.FormMode == 2))
                            {
                                try
                                {
                                    calcularTotal(ref pVal);
                                }
                                catch (Exception ex)
                                {
                                    //logger.Error(ex.Message, ex);
                                }
                            }

                            //    if ((pVal.ItemUID == "matDet1"|| pVal.ItemUID == "matDet2") && (pVal.ColUID == "colMonto") && pVal.ItemChanged && (pVal.FormMode == 3 || pVal.FormMode == 2))
                            //    {
                            //        try
                            //        {                                
                            //            calculartotales(pVal.ItemUID);
                            //        }
                            //        catch (Exception ex)
                            //        {
                            //            logger.Error(ex.Message, ex);
                            //        }
                            //    }



                            break;

                    



                    }
                }

                else
                {
                    //logger.Debug(pVal.EventType.ToString());
                    switch (pVal.EventType)
                    {
                        case BoEventTypes.et_MATRIX_LINK_PRESSED:


                            if ((pVal.ItemUID == "matDet1" || pVal.ItemUID == "matDet2") && (pVal.ColUID == "colNumsap"))
                            {

                                setearLinkDocumentos(pVal.Row);
                            }

                            break;

                        case BoEventTypes.et_KEY_DOWN:
                            if ((pVal.ItemUID == "matDet1" || pVal.ItemUID == "matDet2") && (pVal.FormMode == 3 || pVal.FormMode == 1 || pVal.FormMode == 2))
                            {
                                //if (!validarbotonmatrix(pVal.ItemUID))
                                //{

                                //    //SAPMain.SBO_Application.MessageBox("Debe cargar los períodos.", 1, "Ok", "");

                                //    SAPMain.MensajeError("Debe cargar los períodos, de lo contrario no se guardaran sus datos.", true);
                                //    BubbleEvent = false;
                                //    return;

                                //}

                            }
                            break;

                        case BoEventTypes.et_ITEM_PRESSED:


                            if (pVal.ItemUID == "btnConsult" && pVal.FormMode == 3)
                            {
                                string oBanco = oForm.Items.Item("txtCuenban").Specific.Value;
                                if (oBanco.Equals(""))
                                {
                                    SAPMain.MensajeError("Debe seleccionar un Banco.", true);
                                }
                                else
                                {
                                    Matrix oMatrix = (Matrix)oForm.Items.Item("matDet1").Specific;
                                    oMatrix.Columns.Item("colCheck").Editable = true;
                                    cargarAsientos();
                                }
                            }
                            if (pVal.ItemUID == "btnArch" && pVal.FormMode == 1)
                            {
                                string oBanco = oForm.Items.Item("txtCodban").Specific.Value;
                                Recordset oRecordSet = (SAPbobsCOM.Recordset)SAPMain.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                                string query = string.Empty;
                                if (SAPMain.oCompany.DbServerType == BoDataServerTypes.dst_HANADB)
                                {
                                    // string.Format("SELECT \"BankCode\" \"BankCode\" FROM DSC1 WHERE \"BankCode\" = '{0}' AND  \"U_BPP_GENTXT\" = 'Y' ", oBanco);
                                    query = string.Format($"CALL SP_BPP_OBTENERBANKCODE('{oBanco}')");
                                }
                                else {
                                    query = string.Format($"EXEC SP_BPP_OBTENERBANKCODE '{oBanco}'");
                                }
                                oRecordSet.DoQuery(query);
                                string codBanco = oRecordSet.Fields.Item("BankCode").Value.ToString();
                                if (!codBanco.Equals(""))
                                {
                                    int rpta = SAPMain.SBO_Application.MessageBox("Se generará el archivo txt, la plantilla sera bloqueado. ¿Desea continuar?", 1, "Si", "No", "");
                                    if (rpta != 1) BubbleEvent = false; else generarArchivo(codBanco);
                                }
                                else
                                {
                                    SAPMain.MensajeError("No existe formato txt del banco seleccionado.", true);
                                }
                            }
                            if (pVal.ItemUID == "btnProc" && pVal.FormMode == 1)
                            {
                                int rpta = SAPMain.SBO_Application.MessageBox("Se generarán los pagos, ¿Desea continuar?", 1, "Si", "No", "");
                                if (rpta != 1)
                                {
                                    BubbleEvent = false;
                                }
                                else
                                {
                                    if (generarAsientos())
                                    {
                                        oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE;
                                    }
                                    else
                                    {
                                        BubbleEvent = false;
                                    }

                                }

                            }

                            if (pVal.ItemUID == "1" && pVal.FormMode == 2)
                            {
                                actualizarNumeroSunat();
                            }

                            if (pVal.ItemUID == "1" && pVal.FormMode == 3)
                            {
                                eliminarFilasNoSeleccionadas();
                                //BubbleEvent = generarAsientos();
                                //    return;

                                //if (validarcolocarcodigo())
                                //{

                                //    SAPMain.SBO_Application.MessageBox("Debe colocar el codigo del proyecto.", 1, "Ok", "");
                                //    BubbleEvent = false;
                                //    return;
                                //}
                                //else
                                //{
                                //    string valorMoneda = validarmoneda();

                                //    if (valorMoneda == "")
                                //    {
                                //        Forms.TCambioGUI tCambioGUI = new Forms.TCambioGUI();
                                //        //oFormCambio = SAPMain.SBO_Application.Forms.Item("frmMoneda");
                                //        //oFormCambio.EnableMenu("1292", true);
                                //        //oFormCambio.EnableMenu("1293", false);
                                //        cargarmoneda();
                                //    }
                                //    else
                                //    {
                                //        Forms.TCambioActualizarGUI tCambioActualizarGUI = new Forms.TCambioActualizarGUI();
                                //        //oFormCambio2 = SAPMain.SBO_Application.Forms.Item("frmMoneda2");
                                //        //oFormCambio2.EnableMenu("1292", true);
                                //        //oFormCambio2.EnableMenu("1293", false);
                                //        actualizarmoneda();
                                //    }

                                //}

                            }

                            break;
                    }


                }
            }
            catch (Exception ex)
            {
                SAPMain.MensajeError(ex.Message.ToString(), true);
                //logger.Error(ex.Message, ex);
            }

        }

        private void setearLinkDocumentos(int fila)
        {
            Matrix oMatrix = (Matrix)oForm.Items.Item("matDet1").Specific;

            EditText oEdit = ((EditText)(oMatrix.Columns.Item("colTipobj").Cells.Item(fila)).Specific);
            LinkedButton oLink = (LinkedButton)oMatrix.Columns.Item("colNumsap").ExtendedObject;

            switch (oEdit.String)
            {
                case "18":
                    oLink.LinkedObject = BoLinkedObject.lf_PurchaseInvoice;
                    break;
                case "204":
                    oLink.LinkedObject = (BoLinkedObject)204;
                    break;
                case "30":
                    oLink.LinkedObject = BoLinkedObject.lf_JournalPosting;
                    break;
            }
        }
        private void calcularTotal(ref SAPbouiCOM.ItemEvent pVal)
        {
            try
            {
                Matrix oMatrix = (Matrix)oForm.Items.Item("matDet1").Specific;
                double total = 0;

                for (int i = 0; i < oMatrix.RowCount; i++)
                {
                    EditText oColTotal = (EditText)oMatrix.Columns.Item(pVal.ColUID).Cells.Item(i + 1).Specific;

                    total += double.Parse(oColTotal.Value.ToString());

                }

                oForm.Items.Item("txtTotdet").Specific.Value = total.ToString();
            }
            catch (Exception ex)
            {

            }

           
        }
        private void eliminarFilasNoSeleccionadas()
        {
            try
            {
                oForm.Freeze(true);

                Item oItem = oForm.Items.Item("matDet1");
                Matrix oMatrix = (SAPbouiCOM.Matrix)oItem.Specific;
                CheckBox oCheckBox = null;

                int j = 0;
                double total = 0;
                for (int i = 1; i <= oMatrix.RowCount; i += j)
                {
                    oCheckBox = (SAPbouiCOM.CheckBox)oMatrix.Columns.Item("colCheck").Cells.Item(i).Specific;

                    if (!oCheckBox.Checked)
                    {
                        oMatrix.DeleteRow(i);

                        j = 0;
                    }
                    else
                    {
                        j = 1;
                        total = total + double.Parse(oMatrix.Columns.Item("colSaldo").Cells.Item(i).Specific.Value);
                    }
                }
                oForm.Freeze(false);

                oForm.Items.Item("txtTotdet").Specific.Value = total.ToString();

            }
            catch (Exception ex)
            {
                SAPMain.MensajeError(ex.Message.ToString(), true);
                //logger.Error("Error : " + ex.Message);
                oForm.Freeze(false);
            }

        }
        private void cflCodigoSN(ref SAPbouiCOM.ItemEvent pVal)
        {
            SAPbouiCOM.DataTable dtSelect = null;
            try
            {
                IChooseFromListEvent oCFLEvento = (IChooseFromListEvent)pVal;

                if (!oCFLEvento.Before_Action && oCFLEvento.ChooseFromListUID == "cflCodigoSN")
                {
                    dtSelect = oCFLEvento.SelectedObjects;

                    if (dtSelect != null)
                    {
                        EditText oEditCode = ((EditText)oForm.Items.Item("txtCodprov").Specific);
                        EditText oEditName = ((EditText)oForm.Items.Item("txtNomprov").Specific);
                        try { oEditCode.Value = dtSelect.GetValue("CardCode", 0).ToString(); } catch { }
                        try { oEditName.Value = dtSelect.GetValue("CardName", 0).ToString(); } catch { }
                        //try { oEditContacto.Value = dtSelect.GetValue("CntctPrsn", 0).ToString(); } catch { }
                        //try { oEditAddres.Value = dtSelect.GetValue("MailAddres", 0).ToString(); } catch { }

                    }
                }


            }
            catch (Exception ex)
            {
                SAPMain.MensajeError(ex.Message.ToString(), true);
                //logger.Error("Error : " + ex.Message);
            }



        }

        private void cflCuentaBanco(ref SAPbouiCOM.ItemEvent pVal)
        {
            SAPbouiCOM.DataTable dtSelect = null;
            try
            {
                IChooseFromListEvent oCFLEvento = (IChooseFromListEvent)pVal;

                if (!oCFLEvento.Before_Action && oCFLEvento.ChooseFromListUID == "cflCuentaBn")
                {
                    dtSelect = oCFLEvento.SelectedObjects;

                    if (dtSelect != null)
                    {
                        Recordset oRecordSet = (Recordset)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                        //string query = string.Empty;

                        //if (SAPMain.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                        //{
                        //    query = string.Format("CALL SP_BPP_PARAMETROS_PGM ('{0}','{1}','')  ", "CUENTA_BANCO", dtSelect.GetValue("GLAccount", 0).ToString());
                        //}
                        //else
                        //{
                        //    query = string.Format("EXEC SP_BPP_PARAMETROS_PGM '{0}','{1}','' ", "CUENTA_BANCO" ,);
                        //}

                        //oRecordSet.DoQuery(query);

                        EditText oEditCuenban = ((EditText)oForm.Items.Item("txtCuenban").Specific);
                        ComboBox oEditCuencon = ((ComboBox)oForm.Items.Item("txtCuencon").Specific);
                        EditText oEditMoneda = ((EditText)oForm.Items.Item("txtMoneda").Specific);
                        EditText oEditNomban = ((EditText)oForm.Items.Item("txtNomban").Specific);
                        EditText oEditCodban = ((EditText)oForm.Items.Item("txtCodban").Specific);
                        try { oEditCuenban.Value = dtSelect.GetValue("Account", 0).ToString(); } catch { }
                        try { oEditCuencon.Select(obtieneCuenta(dtSelect.GetValue("GLAccount", 0).ToString(), false), BoSearchKey.psk_ByValue); } catch { }
                        try { oEditMoneda.Value = dtSelect.GetValue("Branch", 0).ToString(); } catch { }
                        try { oEditNomban.Value = dtSelect.GetValue("AcctName", 0).ToString(); } catch { }
                        try { oEditCodban.Value = dtSelect.GetValue("BankCode", 0).ToString(); } catch { }


                    }
                }


            }
            catch (Exception ex)
            {
                SAPMain.MensajeError(ex.Message.ToString(), true);
                //logger.Error("Error : " + ex.Message);
            }



        }
        private void actualizarNumeroSunat()
        {
            try
            {
                Matrix oMatrix = (Matrix)oForm.Items.Item("matDet1").Specific;
                string qry = string.Empty;
                for (int i = 1; i <= oMatrix.RowCount; i++)
                {
                    EditText oEditTextpag = (EditText)oMatrix.Columns.Item("colPago").Cells.Item(i).Specific;
                    EditText oEditTextsap = (EditText)oMatrix.Columns.Item("colNumsap").Cells.Item(i).Specific;

                    string numeropago = oEditTextpag.Value;
                    string numerosap = oEditTextsap.Value;

                    // Ejecutar procedimiento "STR_BPP_ActualizarNumeroSUNAT"
                    Recordset oRecordSet = (Recordset)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                    if (SAPMain.oCompany.DbServerType == BoDataServerTypes.dst_HANADB)
                    {
                        qry = string.Format($"CALL STR_BPP_ActualizarNumeroSUNAT({numeropago},{numerosap},'','1')");
                    }
                    else
                    {
                        qry = string.Format($"EXEC STR_BPP_ActualizarNumeroSUNAT '{numeropago}', '{numerosap}', '' ,'1')");
                    }

                    oRecordSet.DoQuery(qry);


                    //Recordset oRecordSet = (Recordset)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                    //string query = string.Format("UPDATE OPCH SET \"U_BPP_DPNM\" =  '{0}' WHERE \"DocEntry\" = {1} ", numeropago, numerosap);
                    //logger.Debug("Query : " + query);
                    //oRecordSet.DoQuery(query);

                }

            }
            catch (Exception ex)
            {
                SAPMain.MensajeError(ex.Message.ToString(), true);
                //logger.Debug(ex.Message, ex);
            }


        }
        private void generarArchivo(string codigo)
        {
            int cantFilas = 0;
            string txt = string.Empty;
            string procCAB = string.Empty;
            string procDET = string.Empty;
            string oDocEntry = string.Empty;
            List<string> datas = new List<string>();
            string qry = string.Empty;
            string ofechaini = oForm.Items.Item("txtFecini").Specific.Value;
            string ofechafin = oForm.Items.Item("txtFecfin").Specific.Value;
            string oNumeroOper = oForm.Items.Item("txtPagodet").Specific.Value;
            bool hana = SAPMain.oCompany.DbServerType == BoDataServerTypes.dst_HANADB;
            Recordset recordCab = (Recordset)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Recordset recordDet = (Recordset)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

            string codBan = codigo.Replace("0", string.Empty);
            string banco = string.Empty;

            switch (codBan)
            {
                case "2":
                    procCAB = SAPMain.procBCPCab;
                    procDET = "SP_BPP_GNRTXT_BCP_DETV1";
                    banco = "BCP";
                    break;
                case "11":
                    procCAB = SAPMain.procBVACab;
                    procDET = SAPMain.procBVADet;
                    banco = "BBVA";
                    break;
                case "3":
                    procCAB = SAPMain.procINBKCab;
                    procDET = SAPMain.procINBKDet;
                    banco = "INTERBANK";
                    break;
                case "9":
                    procCAB = SAPMain.procSCBKCab;
                    procDET = SAPMain.procSCBKDet;
                    banco = "SCOTIABANK";
                    break;
                default:
                    throw new Exception($"No se encontró Código de Banco {codBan}");
            }

            try
            {
                oDocEntry = oForm.Items.Item("0_U_E").Specific.Value;
                string qryProc = hana ? "CALL" : "EXEC";
                string param = hana ? $"('{oDocEntry}')" : $"'{oDocEntry}'";
                qry = $"{qryProc} {procCAB} {param}";

                recordCab.DoQuery(qry);
                if (recordCab.Fields.Count > 0)
                {
                    string linea = string.Empty;
                    linea = recordCab.Fields.Item(0).Value;
                    linea = linea.Replace("@", string.Empty);
                    txt += linea;
                    txt += "\n";
                }
                /* else
                    throw new Exception("No hay elementos a generar en TXT");
                */
                recordCab = null;

                qry = $"{qryProc} {procDET} {param}";
                recordDet.DoQuery(qry);

                if (recordDet.Fields.Count > 0)
                {
                    while (!recordDet.EoF)
                    {
                        string linea = string.Empty;
                        linea = recordDet.Fields.Item(0).Value.ToString();
                        linea = linea.Replace("@", string.Empty);
                        txt += linea;
                        txt += "\n";

                        recordDet.MoveNext();
                    }
                }

                if (!SAPMain.rutaPagos.Equals(""))
                {
                    recordCab = (Recordset)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                    string fechaPago = DateTime.Now.ToString("yy-MM-dd-mm");
                    string namePagos = $"PAGOS_PROVEEDORES_{banco}_{fechaPago}";

                    string fileName = SAPMain.rutaPagos + ofechaini + namePagos + ".txt";       // Nombre del archivo

                    File.WriteAllText(fileName, txt);

                    qry = string.Format("UPDATE \"@BPP_PAGM_CAB\" SET  U_BPP_RUTATXT =  '{1}' WHERE \"DocEntry\"  = {0} ", oDocEntry, fileName);
                    recordCab.DoQuery(qry);

                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_ADD_MODE;
                    //oForm.Items.Item("txtRuta").Specific.Value = fileName;

                    SAPMain.MensajeExito("Se genero Satisfactoriamente el archivo txt.");
                }
                else
                {
                    SAPMain.MensajeError("Debe definir una ruta para generar el archivo txt.", false);
                }
            }
            catch (Exception e)
            {
                SAPMain.MensajeError(e.Message.ToString(), false);
            }
        }

        private void cargarAsientos()
        {
            Matrix oMatrix = (Matrix)oForm.Items.Item("matDet1").Specific;
            string oDocEntry = oForm.Items.Item("0_U_E").Specific.Value;
            string ofechaini = oForm.Items.Item("txtFecini").Specific.Value;
            string ofechafin = oForm.Items.Item("txtFecfin").Specific.Value;
            string ofechavenini = oForm.Items.Item("txtFecvini").Specific.Value;
            string ofechavenfin = oForm.Items.Item("txtFecvfin").Specific.Value;
            string oCodprov = oForm.Items.Item("txtCodprov").Specific.Value;
            string oMoneda = oForm.Items.Item("txtMoneda").Specific.Value;

            try
            {
                //oForm.Freeze(true);

                DBDataSource oDBDataSource = this.oForm.DataSources.DBDataSources.Item("@BPP_PAGM_DET1");
                oDBDataSource.Clear();

                Recordset oRecordSet = (Recordset)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string query = string.Empty;

                if (SAPMain.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    query = string.Format("CALL SP_BPP_CONSULTAR_PGM_PROVEEDORES ('{0}','{1}','{2}','{3}','{4}','{5}')  ", ofechaini, ofechafin, oCodprov, oMoneda, ofechavenini, ofechavenfin);
                }
                else
                {
                    query = string.Format("EXEC SP_BPP_CONSULTAR_PGM_PROVEEDORES '{0}','{1}','{2}','{3}','{4}','{5}'  ", ofechaini, ofechafin, oCodprov, oMoneda, ofechavenini, ofechavenfin);
                }

                //logger.Debug("Query : " + query);
                oRecordSet.DoQuery(query);


                double total = 0;
                for (int i = 0; i < oRecordSet.RecordCount; i++)
                {

                    oDBDataSource.InsertRecord(oDBDataSource.Size);
                    oDBDataSource.SetValue("U_BPP_CODPROV", oDBDataSource.Size - 1, oRecordSet.Fields.Item("CodigoProveedor").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_RUC", oDBDataSource.Size - 1, oRecordSet.Fields.Item("RUC").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_NOMPROV", oDBDataSource.Size - 1, oRecordSet.Fields.Item("NombreProveedor").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_TIPODOC", oDBDataSource.Size - 1, oRecordSet.Fields.Item("TipoDocumento").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_NUMDOC", oDBDataSource.Size - 1, oRecordSet.Fields.Item("NumeroDocumento").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_FECDOC", oDBDataSource.Size - 1, oRecordSet.Fields.Item("FechaDocumento").Value.ToString("yyyyMMdd"));
                    oDBDataSource.SetValue("U_BPP_FECCONT", oDBDataSource.Size - 1, oRecordSet.Fields.Item("FechaContabilizacion").Value.ToString("yyyyMMdd"));
                    oDBDataSource.SetValue("U_BPP_FECVEN", oDBDataSource.Size - 1, oRecordSet.Fields.Item("FechaVencimiento").Value.ToString("yyyyMMdd"));
                    oDBDataSource.SetValue("U_BPP_SALDO", oDBDataSource.Size - 1, oRecordSet.Fields.Item("Saldo").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_MONTOPAG", oDBDataSource.Size - 1, oRecordSet.Fields.Item("MontoPago").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_CUENBAN", oDBDataSource.Size - 1, oRecordSet.Fields.Item("CuentaBanco").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_NOMBAN", oDBDataSource.Size - 1, oRecordSet.Fields.Item("NombreBanco").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_MONBAN", oDBDataSource.Size - 1, oRecordSet.Fields.Item("MonedaBanco").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_MONEDA", oDBDataSource.Size - 1, oRecordSet.Fields.Item("Moneda").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_IMPORTE", oDBDataSource.Size - 1, oRecordSet.Fields.Item("ImporteDoc").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_OBJTYPE", oDBDataSource.Size - 1, oRecordSet.Fields.Item("ObjType").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_NUMSAP", oDBDataSource.Size - 1, oRecordSet.Fields.Item("NumeroSAP").Value.ToString());


                    oDBDataSource.SetValue("U_BPP_NUMCUOTA", oDBDataSource.Size - 1, oRecordSet.Fields.Item("NumeroCuota").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_NOMCUOTA", oDBDataSource.Size - 1, oRecordSet.Fields.Item("NombreCuota").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_IMPRET", oDBDataSource.Size - 1, oRecordSet.Fields.Item("ImporteRetencion").Value.ToString());

                    total = total + double.Parse(oRecordSet.Fields.Item("Saldo").Value.ToString());


                    oRecordSet.MoveNext();
                }
                oForm.Items.Item("txtTotdet").Specific.Value = total.ToString();
                oMatrix.Clear();
                oMatrix.LoadFromDataSource();
                oMatrix.AutoResizeColumns();

                oForm.Freeze(false);

                //logger.Debug("Se cargo satisfactoriamente los documentos.");

            }
            catch (Exception ex)
            {
                SAPMain.MensajeError(ex.Message.ToString(), true);
                //logger.Debug(ex.Message, ex);
                oForm.Freeze(false);
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
        private bool generarAsientos()
        {

            try
            {
                Recordset oRecordSet = (SAPbobsCOM.Recordset)SAPMain.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                JournalEntries oAsiento = null;
                Payments oPago = null;
                Matrix oMatrix = (Matrix)oForm.Items.Item("matDet1").Specific;

                string ofechacrea = oForm.Items.Item("txtFeccrea").Specific.Value;
                string oCuenta = obtieneCuenta(oForm.Items.Item("txtCuencon").Specific.Value, true);
                string oNumoper = oForm.Items.Item("txtPagodet").Specific.Value;
                string oDocEntry = oForm.Items.Item("0_U_E").Specific.Value;
                string ofechaeje = oForm.Items.Item("txtFeceje").Specific.Value;


                DBDataSource oDBDataSource = this.oForm.DataSources.DBDataSources.Item("@BPP_PAGM_DET1");
                string docEntryDoc;
                string cardCode;
                string cardName;
                double monto;
                string monedaDoc;
                string lineID;
                int numeroCuota;

                string objeto;
                List<string> listidPagos = new List<string>();

                if (oNumoper.Equals(""))
                {
                    SAPMain.MensajeError("Debe ingresar un numero de operación.", true);

                }
                else
                {
                    if (!SAPMain.oCompany.InTransaction)
                    {
                        SAPMain.oCompany.StartTransaction();
                    }

                    SAPMain.MensajeAdvertencia("Creando las Pagos. Espere por favor...");

                    for (int i = 0; i < oDBDataSource.Size; i++)
                    {

                        lineID = oDBDataSource.GetValue("LineId", i).ToString().Trim();
                        docEntryDoc = oDBDataSource.GetValue("U_BPP_NUMSAP", i).ToString().Trim();
                        cardCode = oDBDataSource.GetValue("U_BPP_CODPROV", i).ToString().Trim();
                        cardName = oDBDataSource.GetValue("U_BPP_NOMPROV", i).ToString().Trim();
                        monto = double.Parse(oDBDataSource.GetValue("U_BPP_MONTOPAG", i).ToString().Trim());
                        monedaDoc = oDBDataSource.GetValue("U_BPP_MONEDA", i).ToString().Trim();
                        numeroCuota = int.Parse( oDBDataSource.GetValue("U_BPP_NUMCUOTA", i).ToString().Trim());
                        objeto = oDBDataSource.GetValue("U_BPP_OBJTYPE", i).ToString().Trim();
                        oPago = (Payments)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.oVendorPayments);
                        oPago.DocType = BoRcptTypes.rSupplier;
                        oPago.DocDate = DateTime.ParseExact(ofechaeje, "yyyyMMdd", null);
                        oPago.TaxDate = DateTime.ParseExact(ofechaeje, "yyyyMMdd", null);
                        oPago.DueDate = DateTime.ParseExact(ofechaeje, "yyyyMMdd", null);

                        oPago.JournalRemarks = "Pago Masivos Nro. " + oDocEntry;
                        oPago.CardCode = cardCode;
                        oPago.CardName = cardName;
                        oPago.UserFields.Fields.Item("U_BPP_NUMPAGO").Value = oNumoper;
                        //oPago.UserFields.Fields.Item("U_BPP_TRAN").Value = "003";
                        //oPago.UserFields.Fields.Item("U_BPP_TARJ").Value = "000";
                        //oPago.UserFields.Fields.Item("U_BPP_LETR").Value = "000";
                        //oPago.UserFields.Fields.Item("U_BPP_EFEC").Value = "000";
                        //oPago.UserFields.Fields.Item("U_BPP_CHEQ").Value = "000";

                        oPago.TransferAccount = oCuenta;
                        oPago.TransferReference = oNumoper;
                        oPago.TransferDate = DateTime.ParseExact(DateTime.Now.ToString("yyyyMMdd"), "yyyyMMdd", null);

                        oPago.Invoices.DocEntry = int.Parse(docEntryDoc);
                        switch (objeto)
                        {
                            case "18":
                                oPago.Invoices.InvoiceType = BoRcptInvTypes.it_PurchaseInvoice;
                                break;
                            case "204":
                                oPago.Invoices.InvoiceType = BoRcptInvTypes.it_PurchaseDownPayment;
                                break;
                        }


                        if (monedaDoc == "SOL")
                        {
                            oPago.Invoices.SumApplied = monto;
                        }
                        else
                        {
                            oPago.Invoices.AppliedFC = monto;
                        }

                        oPago.DocCurrency = monedaDoc;
                        oPago.TransferSum = monto;


                        oPago.Invoices.InstallmentId = numeroCuota;
                        if (oPago.Add() != 0)
                        {
                            if (SAPMain.oCompany.InTransaction)
                            {
                                SAPMain.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                            }
                            //oForm.Freeze(false);

                            string error = string.Format("{0}-{1}", SAPMain.oCompany.GetLastErrorCode(), SAPMain.oCompany.GetLastErrorDescription());
                            SAPMain.MensajeError(error, true);
                            //logger.Error("Se mostro el siguiente error: " + error);
                            return false;
                        }
                        else
                        {
                            string idPago = SAPMain.oCompany.GetNewObjectKey();


                            listidPagos.Add(idPago + "|" + lineID);
                            //logger.Debug("Se creo el pago : " + idPago);
                        }

                    }
                    oMatrix.LoadFromDataSource();

                    if (SAPMain.oCompany.InTransaction)
                    {
                        SAPMain.oCompany.EndTransaction(BoWfTransOpt.wf_Commit);

                        oRecordSet = (Recordset)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                        string query = string.Format("UPDATE \"@BPP_PAGM_CAB\" SET  U_BPP_ESTADO = 'Procesado' WHERE \"DocEntry\"  = {0} ", oDocEntry);
                        //logger.Debug("Query : " + query);
                        oRecordSet.DoQuery(query);


                        for (int k = 0; k < listidPagos.Count; k++)
                        {
                            string[] datos = listidPagos[k].Split('|');
                            query = string.Format("UPDATE \"@BPP_PAGM_DET1\" SET \"U_BPP_PAGO\" = " + datos[0] + " WHERE \"DocEntry\" = " + oDocEntry + " AND \"LineId\" = " + datos[1]);
                            //logger.Debug("Query : " + query);
                            oRecordSet.DoQuery(query);


                        }

                        return true;
                    }
                }





                return false;

            }
            catch (Exception ex)
            {
                if (SAPMain.oCompany.InTransaction)
                {
                    SAPMain.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                    //logger.Error(ex.Message);
                    SAPMain.MensajeError(ex.Message, true);
                    //oForm.Freeze(false);
                }
                return false;
            }
        }
    }
}

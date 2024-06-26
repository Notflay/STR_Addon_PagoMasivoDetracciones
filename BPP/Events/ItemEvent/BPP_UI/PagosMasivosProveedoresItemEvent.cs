 
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
using BPP;

namespace BPP
{
    public class PagosMasivosProveedoresItemEvent
    {
        private SAPbouiCOM.Form oForm;
        //private static readonly ILog logger = LogManager.GetLogger(typeof(PagosMasivosProveedoresItemEvent));

        public void itemAction(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            oForm = SAPMain.SBO_Application.Forms.Item(pVal.FormUID);

            try
            {
                if (!pVal.BeforeAction)
                {
                    string text = "";
                    switch (pVal.EventType)
                    {
                        case BoEventTypes.et_COMBO_SELECT:
                            if (pVal.ItemUID == "cbxFljCj")
                            {
                                dynamic specific = oForm.Items.Item("cbxFljCj").Specific;
                                Matrix matrix = (Matrix)(dynamic)oForm.Items.Item("matDet1").Specific;
                                DBDataSource dBDataSource = oForm.DataSources.DBDataSources.Item("@BPP_PAGM_DET1");
                                for (int i = 0; i < matrix.RowCount; i++)
                                {
                                    dBDataSource.SetValue("U_BPP_FLJCAJ", i, specific.Value);
                                }

                                matrix.LoadFromDataSource();
                            }

                            break;
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
                            if (pVal.ItemUID == "btncom" && pVal.FormMode != 1 && pVal.FormMode != 2 && pVal.FormMode != 3)
                            {
                            }

                            break;
                        case BoEventTypes.et_VALIDATE:
                            if ((pVal.ItemUID == "matDet1" || pVal.ItemUID == "matDet2") && pVal.ColUID == "colMonpag" && pVal.ItemChanged && (pVal.FormMode == 3 || pVal.FormMode == 2))
                            {
                                try
                                {
                                    calcularTotal(ref pVal);
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    Global.WriteToFile(ex.Message.ToString());
                                    break;
                                }
                            }

                            break;
                    }

                    return;
                }

                switch (pVal.EventType)
                {
                    case BoEventTypes.et_MATRIX_LINK_PRESSED:
                        if ((pVal.ItemUID == "matDet1" || pVal.ItemUID == "matDet2") && pVal.ColUID == "colNumsap")
                        {
                            setearLinkDocumentos(pVal.Row);
                        }

                        break;
                    case BoEventTypes.et_KEY_DOWN:
                        if (pVal.ItemUID == "txtPagodet" && ((dynamic)oForm.Items.Item("22_U_E").Specific).Value == "Procesado")
                        {
                            Matrix matrix3 = (Matrix)(dynamic)oForm.Items.Item("matDet1").Specific;
                            matrix3.Columns.Item("colCheck").Editable = false;
                            matrix3.Columns.Item("colMonpag").Editable = false;
                            Item item = oForm.Items.Item("btnConsult");
                            item.Enabled = false;
                        }

                        break;
                    case BoEventTypes.et_ITEM_PRESSED:
                        if (pVal.ItemUID == "btnConsult" && pVal.FormMode == 3)
                        {
                            string text2 = ((dynamic)oForm.Items.Item("txtCuenban").Specific).Value;
                            if (text2.Equals(""))
                            {
                                SAPMain.MensajeError("Debe seleccionar un Banco.", estado: true);
                            }
                            else
                            {
                                Matrix matrix2 = (Matrix)(dynamic)oForm.Items.Item("matDet1").Specific;
                                matrix2.Columns.Item("colCheck").Editable = true;
                                cargarAsientos();
                            }
                        }

                        if (pVal.ItemUID == "btnArch" && pVal.FormMode == 1)
                        {
                            string text3 = ((dynamic)oForm.Items.Item("txtCodban").Specific).Value;
                            Recordset recordset = (Recordset)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                            string empty = string.Empty;
                            empty = ((SAPMain.oCompany.DbServerType != BoDataServerTypes.dst_HANADB) ? string.Format("EXEC SP_BPP_OBTENERBANKCODE '" + text3 + "'") : string.Format("CALL SP_BPP_OBTENERBANKCODE('" + text3 + "')"));
                            Global.WriteToFile(empty);
                            recordset.DoQuery(empty);
                            string text4 = ((dynamic)recordset.Fields.Item("BankCode").Value).ToString();
                            if (!text4.Equals(""))
                            {
                                int num = SAPMain.SBO_Application.MessageBox("Se generará el archivo txt, la plantilla sera bloqueado. ¿Desea continuar?", 1, "Si", "No");
                                if (num != 1)
                                {
                                    BubbleEvent = false;
                                }
                                else
                                {
                                    generarArchivo(text4);
                                }
                            }
                            else
                            {
                                SAPMain.MensajeError("No existe formato txt del banco seleccionado.", estado: true);
                            }
                        }

                        if (pVal.ItemUID == "btnProc" && pVal.FormMode == 1)
                        {
                            int num2 = SAPMain.SBO_Application.MessageBox("Se generarán los pagos, ¿Desea continuar?", 1, "Si", "No");
                            if (num2 != 1)
                            {
                                BubbleEvent = false;
                            }
                            else if (generarAsientos())
                            {
                                oForm.Mode = BoFormMode.fm_ADD_MODE;
                            }
                            else
                            {
                                BubbleEvent = false;
                            }
                        }

                        if (pVal.ItemUID == "1" && pVal.FormMode == 2)
                        {
                            actualizarNumeroSunat();
                        }

                        if (pVal.ItemUID == "1" && pVal.FormMode == 3)
                        {
                            eliminarFilasNoSeleccionadas();
                        }

                        break;
                }
            }
            catch (Exception ex2)
            {
                Global.WriteToFile(ex2.Message.ToString());
                SAPMain.MensajeError(ex2.Message.ToString(), estado: true);
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
                Global.WriteToFile(ex.Message.ToString());
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
                Global.WriteToFile(ex.Message.ToString());

                SAPMain.MensajeError(ex.Message.ToString(), true);
                //logger.Error("Error : " + ex.Message);
            }



        }
        private void actualizarNumeroSunat(string numeropago = "")
        {
            try
            {
                Matrix matrix = (Matrix)(dynamic)oForm.Items.Item("matDet1").Specific;
                string text = ((dynamic)oForm.Items.Item("txtPagodet").Specific).Value;
                string empty = string.Empty;
                for (int i = 1; i <= matrix.RowCount; i++)
                {
                    EditText editText = (EditText)(dynamic)matrix.Columns.Item("colPago").Cells.Item(i).Specific;
                    EditText editText2 = (EditText)(dynamic)matrix.Columns.Item("colNumsap").Cells.Item(i).Specific;
                    string value = editText.Value;
                    string value2 = editText2.Value;
                    if (SAPMain.opcionPagoMasivo == 1)
                    {
                        if (!string.IsNullOrEmpty(numeropago))
                        {
                            Recordset recordset = (Recordset)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                            empty = ((SAPMain.oCompany.DbServerType != BoDataServerTypes.dst_HANADB) ? string.Format("EXEC STR_BPP_ActualizarNumeroSUNAT '" + numeropago + "', '" + value2 + "', '' ,'1'") : string.Format("CALL STR_BPP_ActualizarNumeroSUNAT(" + numeropago + "," + value2 + ",'','1')"));
                            Global.WriteToFile(empty);
                            recordset.DoQuery(empty);
                        }
                    }
                    else if (SAPMain.opcionPagoMasivo == 2 && !string.IsNullOrEmpty(text))
                    {
                        Payments payments = (Payments)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.oVendorPayments);
                        if (payments.GetByKey(Convert.ToInt32(value)))
                        {
                            payments.JournalRemarks = text;
                            payments.Update();
                        }

                        payments = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Global.WriteToFile(ex.Message.ToString());
                SAPMain.MensajeError(ex.Message.ToString(), estado: true);
            }
        }
        private void generarArchivo(string codigo)
        {
            int num = 0;
            string text = string.Empty;
            string empty = string.Empty;
            string empty2 = string.Empty;
            string empty3 = string.Empty;
            List<string> list = new List<string>();
            string empty4 = string.Empty;
            string text2 = ((dynamic)oForm.Items.Item("txtFecini").Specific).Value;
            string text3 = ((dynamic)oForm.Items.Item("txtFecfin").Specific).Value;
            string text4 = ((dynamic)oForm.Items.Item("txtPagodet").Specific).Value;
            bool flag = SAPMain.oCompany.DbServerType == BoDataServerTypes.dst_HANADB;
            Recordset recordset = (Recordset)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Recordset recordset2 = (Recordset)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            string text5 = codigo.Replace("0", string.Empty);
            string empty5 = string.Empty;
            switch (text5)
            {
                case "2":
                    empty = SAPMain.procBCPCab;
                    empty2 = "SP_BPP_GNRTXT_BCP_DETV1";
                    empty5 = "BCP";
                    break;
                case "11":
                    empty = SAPMain.procBVACab;
                    empty2 = SAPMain.procBVADet;
                    empty5 = "BBVA";
                    break;
                case "3":
                    empty = SAPMain.procINBKCab;
                    empty2 = SAPMain.procINBKDet;
                    empty5 = "INTERBANK";
                    break;
                case "9":
                    empty = SAPMain.procSCBKCab;
                    empty2 = SAPMain.procSCBKDet;
                    empty5 = "SCOTIABANK";
                    break;
                default:
                    throw new Exception("No se encontró Código de Banco " + text5);
            }

            try
            {
                empty3 = ((dynamic)oForm.Items.Item("0_U_E").Specific).Value;
                string text6 = (flag ? "CALL" : "EXEC");
                string text7 = (flag ? ("('" + empty3 + "')") : ("'" + empty3 + "'"));
                empty4 = text6 + " " + empty + " " + text7;
                Global.WriteToFile(empty4);
                recordset.DoQuery(empty4);
                if (recordset.Fields.Count > 0)
                {
                    string empty6 = string.Empty;
                    empty6 = (dynamic)recordset.Fields.Item(0).Value;
                    empty6 = empty6.Replace("@", string.Empty);
                    text += empty6;
                    text += "\n";
                }

                recordset = null;
                empty4 = text6 + " " + empty2 + " " + text7;
                Global.WriteToFile(empty4);
                recordset2.DoQuery(empty4);
                if (recordset2.Fields.Count > 0)
                {
                    while (!recordset2.EoF)
                    {
                        string empty7 = string.Empty;
                        empty7 = ((dynamic)recordset2.Fields.Item(0).Value).ToString();
                        empty7 = empty7.Replace("@", string.Empty);
                        text += empty7;
                        text += "\n";
                        recordset2.MoveNext();
                    }
                }

                if (!SAPMain.rutaPagos.Equals(""))
                {
                    recordset = (Recordset)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                    string text8 = DateTime.Now.ToString("yy-MM-dd-mm");
                    string text9 = "PAGOS_PROVEEDORES_" + empty5 + "_" + text8;
                    string text10 = SAPMain.rutaPagos + text2 + text9 + ".txt";
                    File.WriteAllText(text10, text);
                    empty4 = string.Format("UPDATE \"@BPP_PAGM_CAB\" SET  U_BPP_RUTATXT =  '{1}' WHERE \"DocEntry\"  = {0} ", empty3, text10);
                    Global.WriteToFile(empty4);
                    recordset.DoQuery(empty4);
                    oForm.Mode = BoFormMode.fm_ADD_MODE;
                    SAPMain.MensajeExito("Se genero Satisfactoriamente el archivo txt.");
                }
                else
                {
                    SAPMain.MensajeError("Debe definir una ruta para generar el archivo txt.");
                }
            }
            catch (Exception ex)
            {
                Global.WriteToFile(ex.Message);
                SAPMain.MensajeError(ex.Message.ToString());
            }
        }

        private void cargarAsientos()
        {
            Matrix matrix = (Matrix)(dynamic)oForm.Items.Item("matDet1").Specific;
            string text = ((dynamic)oForm.Items.Item("0_U_E").Specific).Value;
            string text2 = ((dynamic)oForm.Items.Item("txtFecini").Specific).Value;
            string text3 = ((dynamic)oForm.Items.Item("txtFecfin").Specific).Value;
            string text4 = ((dynamic)oForm.Items.Item("txtFecvini").Specific).Value;
            string text5 = ((dynamic)oForm.Items.Item("txtFecvfin").Specific).Value;
            string text6 = ((dynamic)oForm.Items.Item("txtCodprov").Specific).Value;
            string text7 = ((dynamic)oForm.Items.Item("txtMoneda").Specific).Value;
            string text8 = ((dynamic)oForm.Items.Item("cbxFiltro").Specific).Value;
            string text9 = ((dynamic)oForm.Items.Item("edtFiltval").Specific).Value;
            try
            {
                DBDataSource dBDataSource = oForm.DataSources.DBDataSources.Item("@BPP_PAGM_DET1");
                dBDataSource.Clear();
                Recordset recordset = (Recordset)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string empty = string.Empty;
                empty = ((SAPMain.oCompany.DbServerType != BoDataServerTypes.dst_HANADB) ? $"EXEC SP_BPP_CONSULTAR_PGM_PROVEEDORES '{text2}','{text3}','{text6}','{text7}','{text4}','{text5}','{text8}','{text9}'" : $"CALL SP_BPP_CONSULTAR_PGM_PROVEEDORES ('{text2}','{text3}','{text6}','{text7}','{text4}','{text5}','{text8}','{text9}')");
                Global.WriteToFile(empty);
                recordset.DoQuery(empty);
                double num = 0.0;
                for (int i = 0; i < recordset.RecordCount; i++)
                {
                    dBDataSource.InsertRecord(dBDataSource.Size);
                    dBDataSource.SetValue("U_BPP_CODPROV", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("CodigoProveedor").Value).ToString());
                    dBDataSource.SetValue("U_BPP_RUC", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("RUC").Value).ToString());
                    dBDataSource.SetValue("U_BPP_NOMPROV", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("NombreProveedor").Value).ToString());
                    dBDataSource.SetValue("U_BPP_TIPODOC", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("TipoDocumento").Value).ToString());
                    dBDataSource.SetValue("U_BPP_NUMDOC", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("NumeroDocumento").Value).ToString());
                    dBDataSource.SetValue("U_BPP_FECDOC", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("FechaDocumento").Value).ToString("yyyyMMdd"));
                    dBDataSource.SetValue("U_BPP_FECCONT", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("FechaContabilizacion").Value).ToString("yyyyMMdd"));
                    dBDataSource.SetValue("U_BPP_FECVEN", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("FechaVencimiento").Value).ToString("yyyyMMdd"));
                    dBDataSource.SetValue("U_BPP_SALDO", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("Saldo").Value).ToString());
                    dBDataSource.SetValue("U_BPP_MONTOPAG", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("MontoPago").Value).ToString());
                    dBDataSource.SetValue("U_BPP_CUENBAN", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("CuentaBanco").Value).ToString());
                    dBDataSource.SetValue("U_BPP_NOMBAN", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("NombreBanco").Value).ToString());
                    dBDataSource.SetValue("U_BPP_MONBAN", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("MonedaBanco").Value).ToString());
                    dBDataSource.SetValue("U_BPP_MONEDA", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("Moneda").Value).ToString());
                    dBDataSource.SetValue("U_BPP_IMPORTE", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("ImporteDoc").Value).ToString());
                    dBDataSource.SetValue("U_BPP_OBJTYPE", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("ObjType").Value).ToString());
                    dBDataSource.SetValue("U_BPP_NUMSAP", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("NumeroSAP").Value).ToString());
                    dBDataSource.SetValue("U_BPP_NUMCUOTA", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("NumeroCuota").Value).ToString());
                    dBDataSource.SetValue("U_BPP_NOMCUOTA", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("NombreCuota").Value).ToString());
                    dBDataSource.SetValue("U_BPP_IMPRET", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("ImporteRetencion").Value).ToString());
                    dBDataSource.SetValue("U_BPP_DETPAGO", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("DetraccionPago").Value).ToString());
                    num += double.Parse(((dynamic)recordset.Fields.Item("Saldo").Value).ToString());
                    recordset.MoveNext();
                }

                ((dynamic)oForm.Items.Item("txtTotdet").Specific).Value = num.ToString();
                matrix.Clear();
                matrix.LoadFromDataSource();
                matrix.AutoResizeColumns();
                oForm.Freeze(newVal: false);
            }
            catch (Exception ex)
            {
                Global.WriteToFile(ex.Message.ToString());
                SAPMain.MensajeError(ex.Message.ToString(), estado: true);
                oForm.Freeze(newVal: false);
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
                    Global.WriteToFile(qry);
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
                Recordset recordset = (Recordset)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                JournalEntries journalEntries = null;
                Payments payments = null;
                Matrix matrix = (Matrix)(dynamic)oForm.Items.Item("matDet1").Specific;
                string text = ((dynamic)oForm.Items.Item("txtFeccrea").Specific).Value;
                string transferAccount = obtieneCuenta(((dynamic)oForm.Items.Item("txtCuencon").Specific).Value, true);
                string text2 = ((dynamic)oForm.Items.Item("txtPagodet").Specific).Value;
                string text3 = ((dynamic)oForm.Items.Item("0_U_E").Specific).Value;
                string s = ((dynamic)oForm.Items.Item("txtFeceje").Specific).Value;
                Global.WriteToFile(text + "\n" + transferAccount + "\n" + text2 + "\n" + text3 + "\n" + s);

                DBDataSource dBDataSource = oForm.DataSources.DBDataSources.Item("@BPP_PAGM_DET1");
                List<string> list = new List<string>();
                if (text2.Equals("") && SAPMain.opcionPagoMasivo == 1)
                {
                    SAPMain.MensajeError("Debe ingresar un numero de operación.", estado: true);
                }
                else
                {
                    if (!SAPMain.oCompany.InTransaction)
                    {
                        SAPMain.oCompany.StartTransaction();
                    }

                    SAPMain.MensajeAdvertencia("Creando las Pagos. Espere por favor...");
                    for (int i = 0; i < dBDataSource.Size; i++)
                    {
                        string text4 = dBDataSource.GetValue("LineId", i).ToString().Trim();
                        string s2 = dBDataSource.GetValue("U_BPP_NUMSAP", i).ToString().Trim();
                        string cardCode = dBDataSource.GetValue("U_BPP_CODPROV", i).ToString().Trim();
                        string cardName = dBDataSource.GetValue("U_BPP_NOMPROV", i).ToString().Trim();
                        double num = double.Parse(dBDataSource.GetValue("U_BPP_MONTOPAG", i).ToString().Trim());
                        string text5 = dBDataSource.GetValue("U_BPP_MONEDA", i).ToString().Trim();
                        int installmentId = int.Parse(dBDataSource.GetValue("U_BPP_NUMCUOTA", i).ToString().Trim());
                        string text6 = dBDataSource.GetValue("U_BPP_OBJTYPE", i).ToString().Trim();
                        payments = (Payments)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.oVendorPayments);
                        payments.DocType = BoRcptTypes.rSupplier;
                        payments.DocDate = DateTime.ParseExact(s, "yyyyMMdd", null);
                        payments.TaxDate = DateTime.ParseExact(s, "yyyyMMdd", null);
                        payments.DueDate = DateTime.ParseExact(s, "yyyyMMdd", null);
                        payments.JournalRemarks = "Pago Masivos Nro. " + text3;
                        payments.CardCode = cardCode;
                        payments.CardName = cardName;
                        payments.UserFields.Fields.Item("U_BPP_NUMPAGO").Value = text2;
                        Global.WriteToFile(transferAccount);
                        payments.TransferAccount = transferAccount;
                        payments.TransferReference = text2;
                        payments.TransferDate = DateTime.ParseExact(DateTime.Now.ToString("yyyyMMdd"), "yyyyMMdd", null);
                        payments.PrimaryFormItems.PaymentMeans = PaymentMeansTypeEnum.pmtBankTransfer;
                        string value = dBDataSource.GetValue("U_BPP_FLJCAJ", i).ToString();
                        payments.PrimaryFormItems.CashFlowLineItemID = ((!string.IsNullOrEmpty(value)) ? Convert.ToInt32(value) : 0);
                        payments.Invoices.DocEntry = int.Parse(s2);
                        string text7 = text6;
                        string text8 = text7;
                        if (!(text8 == "18"))
                        {
                            if (text8 == "204")
                            {
                                payments.Invoices.InvoiceType = BoRcptInvTypes.it_PurchaseDownPayment;
                            }
                        }
                        else
                        {
                            payments.Invoices.InvoiceType = BoRcptInvTypes.it_PurchaseInvoice;
                        }

                        if (text5 == "SOL")
                        {
                            payments.Invoices.SumApplied = num;
                        }
                        else
                        {
                            payments.Invoices.AppliedFC = num;
                        }

                        payments.DocCurrency = text5;
                        payments.TransferSum = num;
                        payments.Invoices.InstallmentId = installmentId;
                        if (payments.Add() != 0)
                        {
                            if (SAPMain.oCompany.InTransaction)
                            {
                                SAPMain.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                            }

                            string mensaje = $"{SAPMain.oCompany.GetLastErrorCode()}-{SAPMain.oCompany.GetLastErrorDescription()}";
                            SAPMain.MensajeError(mensaje, estado: true);
                            return false;
                        }

                        string newObjectKey = SAPMain.oCompany.GetNewObjectKey();
                        list.Add(newObjectKey + "|" + text4);
                    }

                    matrix.LoadFromDataSource();
                    if (SAPMain.oCompany.InTransaction)
                    {
                        SAPMain.oCompany.EndTransaction(BoWfTransOpt.wf_Commit);
                        recordset = (Recordset)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                        string text9 = $"UPDATE \"@BPP_PAGM_CAB\" SET  U_BPP_ESTADO = 'Procesado' WHERE \"DocEntry\"  = {text3} ";
                        Global.WriteToFile(text9);
                        recordset.DoQuery(text9);
                        for (int j = 0; j < list.Count; j++)
                        {
                            string[] array = list[j].Split('|');
                            text9 = string.Format("UPDATE \"@BPP_PAGM_DET1\" SET \"U_BPP_PAGO\" = " + array[0] + " WHERE \"DocEntry\" = " + text3 + " AND \"LineId\" = " + array[1]);
                            Global.WriteToFile(text9);
                            recordset.DoQuery(text9);
                        }

                        actualizarNumeroSunat(text2);
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Global.WriteToFile(ex.Message.ToString());
                if (SAPMain.oCompany.InTransaction)
                {
                    SAPMain.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                    SAPMain.MensajeError(ex.Message, estado: true);
                }

                return false;
            }
        }
    }
}

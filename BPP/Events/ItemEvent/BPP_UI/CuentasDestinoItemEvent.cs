using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPP
{
    public class CuentasDestinoItemEvent
    {
        private SAPbouiCOM.Form oForm;

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


                            //case BoEventTypes.et_VALIDATE:


                            //    if ((pVal.ItemUID == "matDet1" || pVal.ItemUID == "matDet2") && (pVal.ColUID == "colCodigo") && pVal.ItemChanged && (pVal.FormMode == 3 || pVal.FormMode == 2))
                            //    {
                            //        try
                            //        {                                 
                            //            buscardescripcion(ref pVal, pVal.ItemUID);
                            //        }
                            //        catch (Exception ex)
                            //        {
                            //            logger.Error(ex.Message, ex);
                            //        }
                            //    }

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



                            //    break;




                    }
                }

                else
                {
                    //logger.Debug(pVal.EventType.ToString());
                    switch (pVal.EventType)
                    {
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
                                cargarAsientos();
                            }


                            if (pVal.ItemUID == "1" && pVal.FormMode == 3)
                            {
                                BubbleEvent = generarAsientos();
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
                throw new Exception(ex.Message);
            }

        }

        private void cargarAsientos()
        {
            Matrix oMatrix = (Matrix)oForm.Items.Item("matDet1").Specific;
            string periodo = oForm.Items.Item("txtPeriodo").Specific.Value;

            try
            {
                oForm.Freeze(true);

                DBDataSource oDBDataSource = this.oForm.DataSources.DBDataSources.Item("@BPP_CTD_DET1");
                oDBDataSource.Clear();

                Recordset oRecordSet = (Recordset)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string query = string.Empty;
                if (SAPMain.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                {
                    query = string.Format(" CALL \"SP_BPP_CONSULTAR_DESTINOS\" ('{0}') ", periodo);
                }
                else
                {
                    query = string.Format("EXEC SP_BPP_CONSULTAR_DESTINOS '{0}'  ", periodo);
                }
                oRecordSet.DoQuery(query);
                for (int i = 0; i < oRecordSet.RecordCount; i++)
                {

                    oDBDataSource.InsertRecord(oDBDataSource.Size);

                    oDBDataSource.SetValue("U_BPP_ASIENTOORG", oDBDataSource.Size - 1, oRecordSet.Fields.Item("AsientoOrigen").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_CTANATU", oDBDataSource.Size - 1, oRecordSet.Fields.Item("CuentaNaturaleza").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_CTADEST", oDBDataSource.Size - 1, retornaSegmentAll(oRecordSet.Fields.Item("CuentaDestino").Value.ToString()));
                    oDBDataSource.SetValue("U_BPP_MONEDA", oDBDataSource.Size - 1, oRecordSet.Fields.Item("Moneda").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_MNTLOCAL", oDBDataSource.Size - 1, oRecordSet.Fields.Item("MontoLocal").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_MNTEXTR", oDBDataSource.Size - 1, oRecordSet.Fields.Item("MontoExtranjero").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_MNTSIST", oDBDataSource.Size - 1, oRecordSet.Fields.Item("MontoSistema").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_FECCONT", oDBDataSource.Size - 1, oRecordSet.Fields.Item("FechaContabilizacion").Value.ToString("yyyyMMdd"));
                    oDBDataSource.SetValue("U_BPP_FECDOC", oDBDataSource.Size - 1, oRecordSet.Fields.Item("FechaDocumento").Value.ToString("yyyyMMdd"));
                    oDBDataSource.SetValue("U_BPP_FECVENC", oDBDataSource.Size - 1, oRecordSet.Fields.Item("FechaVencimiento").Value.ToString("yyyyMMdd"));
                    oDBDataSource.SetValue("U_BPP_REFERENCIA", oDBDataSource.Size - 1, oRecordSet.Fields.Item("Referencia").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_REF2", oDBDataSource.Size - 1, oRecordSet.Fields.Item("Referencia2").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_COMENTARIO", oDBDataSource.Size - 1, oRecordSet.Fields.Item("Comentarios").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_CC1", oDBDataSource.Size - 1, oRecordSet.Fields.Item("CC1").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_CC2", oDBDataSource.Size - 1, oRecordSet.Fields.Item("CC2").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_CC3", oDBDataSource.Size - 1, oRecordSet.Fields.Item("CC3").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_CC4", oDBDataSource.Size - 1, oRecordSet.Fields.Item("CC4").Value.ToString());
                    oDBDataSource.SetValue("U_BPP_CC5", oDBDataSource.Size - 1, oRecordSet.Fields.Item("CC5").Value.ToString());


                    oRecordSet.MoveNext();
                }

                oMatrix.Clear();
                oMatrix.LoadFromDataSource();
                oMatrix.AutoResizeColumns();

                oForm.Freeze(false);

            }
            catch (Exception ex)
            {
                SAPMain.MensajeError(ex.Message.ToString(), true);
                oForm.Freeze(false);
            }
        }
        private bool generarAsientos()
        {
            int rpta = SAPMain.SBO_Application.MessageBox("Se generarán los asientos de destino, ¿Desea continuar?", 1, "Si", "No", "");
            if (rpta != 1) return false;
            try
            {
                Recordset oRecordSet = (SAPbobsCOM.Recordset)SAPMain.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                JournalEntries oAsiento = null;
                Matrix oMatrix = (Matrix)oForm.Items.Item("matDet1").Specific;
                int rows = oMatrix.RowCount;

                ComboBox oComboPeriodo = oForm.Items.Item("txtPeriodo").Specific;

                if (rows != 0 && !oComboPeriodo.Value.Equals(""))
                {
                    string periodo = oComboPeriodo.Selected.Description;
                    DBDataSource oDBDataSource = this.oForm.DataSources.DBDataSources.Item("@BPP_CTD_DET1");

                    if (!SAPMain.oCompany.InTransaction)
                    {
                        SAPMain.oCompany.StartTransaction();
                    }

                    SAPMain.MensajeAdvertencia("Creando los asientos destino. Espere por favor...");
                    oForm.Freeze(true);
                    for (int i = 0; i < oDBDataSource.Size; i++)
                    {

                        double montoLoc = double.Parse(oDBDataSource.GetValue("U_BPP_MNTLOCAL", i).ToString().Trim());
                        double montoExt = double.Parse(oDBDataSource.GetValue("U_BPP_MNTEXTR", i).ToString().Trim());
                        double montoSis = double.Parse(oDBDataSource.GetValue("U_BPP_MNTSIST", i).ToString().Trim());
                        oAsiento = (SAPbobsCOM.JournalEntries)SAPMain.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);

                        oAsiento.ReferenceDate = DateTime.ParseExact(oDBDataSource.GetValue("U_BPP_FECCONT", i).ToString().Trim(), "yyyyMMdd", null);
                        oAsiento.TaxDate = DateTime.ParseExact(oDBDataSource.GetValue("U_BPP_FECDOC", i).ToString().Trim(), "yyyyMMdd", null);
                        oAsiento.DueDate = DateTime.ParseExact(oDBDataSource.GetValue("U_BPP_FECVENC", i).ToString().Trim(), "yyyyMMdd", null);

                        oAsiento.TransactionCode = "DES";
                        oAsiento.Reference = oDBDataSource.GetValue("U_BPP_ASIENTOORG", i).ToString().Trim();
                        oAsiento.Reference2 = oDBDataSource.GetValue("U_BPP_REF2", i).ToString().Trim();
                        oAsiento.Memo = "ADD-ON BPP ASIENTO DE DESTINO " + periodo;

                        oAsiento.Lines.AccountCode = retornaAcctCode(oDBDataSource.GetValue("U_BPP_CTADEST", i).ToString().Trim());
                        oAsiento.Lines.ShortName = oAsiento.Lines.AccountCode;

                        oAsiento.Lines.Reference1 = oDBDataSource.GetValue("U_BPP_ASIENTOORG", i).ToString().Trim();
                        oAsiento.Lines.Reference2 = oDBDataSource.GetValue("U_BPP_REF2", i).ToString().Trim(); ;
                        oAsiento.Lines.LineMemo = oDBDataSource.GetValue("U_BPP_COMENTARIO", i).ToString().Trim();


                        if (montoLoc >= 0 && montoSis >= 0)
                        {
                            oAsiento.Lines.Debit = montoLoc;
                            oAsiento.Lines.DebitSys = montoSis;
                        }
                        else
                        {
                            oAsiento.Lines.Credit = montoLoc * -1;
                            oAsiento.Lines.CreditSys = montoSis * -1;
                        }

                        if (montoExt != 0)
                        {
                            oAsiento.Lines.FCCurrency = oDBDataSource.GetValue("U_BPP_MONEDA", i).ToString().Trim();
                            if (montoExt >= 0 && montoSis >= 0)
                                oAsiento.Lines.FCDebit = montoExt;
                            else
                                oAsiento.Lines.FCCredit = montoExt * -1;
                        }

                        oAsiento.Lines.Add();
                        oAsiento.Lines.SetCurrentLine(1);

                        oAsiento.Lines.AccountCode = retornaAcctCode(SAPMain.cuentaContraPartida);
                        oAsiento.Lines.ShortName = oAsiento.Lines.AccountCode;

                        oAsiento.Lines.Reference1 = oDBDataSource.GetValue("U_BPP_ASIENTOORG", i).ToString().Trim();
                        oAsiento.Lines.Reference2 = oDBDataSource.GetValue("U_BPP_REF2", i).ToString().Trim(); ;
                        oAsiento.Lines.LineMemo = oDBDataSource.GetValue("U_BPP_COMENTARIO", i).ToString().Trim();

                        oAsiento.Lines.CostingCode = oDBDataSource.GetValue("U_BPP_CC1", i).ToString().Trim();
                        oAsiento.Lines.CostingCode2 = oDBDataSource.GetValue("U_BPP_CC2", i).ToString().Trim();
                        oAsiento.Lines.CostingCode3 = oDBDataSource.GetValue("U_BPP_CC3", i).ToString().Trim();
                        oAsiento.Lines.CostingCode4 = oDBDataSource.GetValue("U_BPP_CC4", i).ToString().Trim();
                        oAsiento.Lines.CostingCode5 = oDBDataSource.GetValue("U_BPP_CC5", i).ToString().Trim();

                        if (montoLoc >= 0 && montoSis >= 0)
                        {
                            oAsiento.Lines.Credit = montoLoc;
                            oAsiento.Lines.CreditSys = montoSis;
                        }
                        else
                        {
                            oAsiento.Lines.Debit = montoLoc * -1;
                            oAsiento.Lines.DebitSys = montoSis * -1;
                        }

                        if (montoExt != 0)
                        {
                            oAsiento.Lines.FCCurrency = oDBDataSource.GetValue("U_BPP_MONEDA", i).ToString().Trim();
                            if (montoExt >= 0 && montoSis >= 0)
                                oAsiento.Lines.FCCredit = montoExt;
                            else
                                oAsiento.Lines.FCDebit = montoExt * -1;
                        }

                        if (oAsiento.Add() != 0)
                        {
                            if (SAPMain.oCompany.InTransaction)
                            {
                                SAPMain.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                            }
                            oForm.Freeze(false);

                            string error = string.Format("{0}-{1}", SAPMain.oCompany.GetLastErrorCode(), SAPMain.oCompany.GetLastErrorDescription());
                            SAPMain.MensajeError(error, true);

                            return false;

                        }
                        else
                        {
                            string transidDestino = SAPMain.oCompany.GetNewObjectKey();


                            oDBDataSource.SetValue("U_BPP_ASIENTODEST", i, transidDestino);


                            SAPMain.MensajeExito("Se creo el asiento : " + transidDestino);
                        }
                    }
                    oMatrix.LoadFromDataSource();

                    if (SAPMain.oCompany.InTransaction)
                    {
                        SAPMain.oCompany.EndTransaction(BoWfTransOpt.wf_Commit);
                        ComboBox oComboEstado = oForm.Items.Item("txtEstado").Specific;
                        oComboEstado.Select("Procesado", BoSearchKey.psk_ByValue);
                        oForm.Freeze(false);
                        return true;
                    }
                }
                else
                {
                    if (oComboPeriodo.Value.Equals(""))
                    {
                        SAPMain.MensajeError("Debe seleccionar un periodo.", true);
                    }
                    else if (rows == 0)
                    {
                        SAPMain.MensajeError("No existen filas por procesar.", true);
                    }
                }

                return false;

            }
            catch (Exception ex)
            {
                if (SAPMain.oCompany.InTransaction)
                {
                    SAPMain.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                    SAPMain.MensajeError(ex.ToString(), true);
                    oForm.Freeze(false);
                }
                SAPMain.MensajeError(ex.ToString(), true);
                return false;
            }
        }

        private string retornaSegmentAll(string code)
        {
            if (!SAPMain.segmentado)
                return code;
            else
            {
                Recordset oRecordSet = (Recordset)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                string query = string.Empty;

                if (SAPMain.oCompany.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                    query = $"SELECT \"Segment_0\"||'-'||\"Segment_1\"||'-'||\"Segment_2\" FROM OACT WHERE \"Segment_0\" = '{code}'";
                else
                    query = $"SELECT \"Segment_0\"+'-'+\"Segment_1\"+'-'+\"Segment_2\" FROM OACT WHERE \"Segment_0\" = '{code}'";

                oRecordSet.DoQuery(query);
                if (oRecordSet.Fields.Count > 0)
                {
                    return oRecordSet.Fields.Item(0).Value.ToString();
                }
                else
                { return code; }
            }

        }

        private string retornaAcctCode(string code)
        {
            string acctCode = string.Empty;
            Recordset oRecordSet = (Recordset)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

            try
            {
                if (!SAPMain.segmentado)
                    return code;
                else
                {
                    string[] codes = code.Split('-');

                    oRecordSet.DoQuery($"SELECT \"AcctCode\" FROM OACT WHERE \"Segment_0\" = '{codes[0]}'");
                    acctCode = oRecordSet.Fields.Item(0).Value.ToString();
                    if (string.IsNullOrEmpty(acctCode))
                        throw new Exception("No se encuentra el AcctCode con este Segmento");
                    else
                        return acctCode;
                }
            }
            finally
            {
                oRecordSet = null;
            }
        }
    }
}

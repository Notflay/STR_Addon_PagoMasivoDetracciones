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
                        case BoEventTypes.et_ITEM_PRESSED:
                            if (pVal.ItemUID == "btncom" && pVal.FormMode != 1 && pVal.FormMode != 2 && pVal.FormMode != 3)
                            {
                            }

                            break;
                    }

                    return;
                }

                switch (pVal.EventType)
                {
                    case BoEventTypes.et_KEY_DOWN:
                        if ((pVal.ItemUID == "matDet1" || pVal.ItemUID == "matDet2") && pVal.FormMode != 3 && pVal.FormMode != 1 && pVal.FormMode != 2)
                        {
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
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void cargarAsientos()
        {
            Matrix matrix = (Matrix)(dynamic)oForm.Items.Item("matDet1").Specific;
            string arg = ((dynamic)oForm.Items.Item("txtPeriodo").Specific).Value;
            try
            {
                oForm.Freeze(newVal: true);
                DBDataSource dBDataSource = oForm.DataSources.DBDataSources.Item("@BPP_CTD_DET1");
                dBDataSource.Clear();
                Recordset recordset = (Recordset)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string empty = string.Empty;
                empty = ((SAPMain.oCompany.DbServerType != BoDataServerTypes.dst_HANADB) ? $"EXEC SP_BPP_CONSULTAR_DESTINOS '{arg}'  " : $" CALL \"SP_BPP_CONSULTAR_DESTINOS\" ('{arg}') ");
                Global.WriteToFile(empty);
                recordset.DoQuery(empty);
                for (int i = 0; i < recordset.RecordCount; i++)
                {
                    dBDataSource.InsertRecord(dBDataSource.Size);
                    dBDataSource.SetValue("U_BPP_ASIENTOORG", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("AsientoOrigen").Value).ToString());
                    dBDataSource.SetValue("U_BPP_CTANATU", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("CuentaNaturaleza").Value).ToString());
                    dBDataSource.SetValue("U_BPP_CTADEST", dBDataSource.Size - 1, retornaSegmentAll(((dynamic)recordset.Fields.Item("CuentaDestino").Value).ToString()));
                    dBDataSource.SetValue("U_BPP_MONEDA", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("Moneda").Value).ToString());
                    dBDataSource.SetValue("U_BPP_MNTLOCAL", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("MontoLocal").Value).ToString());
                    dBDataSource.SetValue("U_BPP_MNTEXTR", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("MontoExtranjero").Value).ToString());
                    dBDataSource.SetValue("U_BPP_MNTSIST", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("MontoSistema").Value).ToString());
                    dBDataSource.SetValue("U_BPP_FECCONT", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("FechaContabilizacion").Value).ToString("yyyyMMdd"));
                    dBDataSource.SetValue("U_BPP_FECDOC", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("FechaDocumento").Value).ToString("yyyyMMdd"));
                    dBDataSource.SetValue("U_BPP_FECVENC", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("FechaVencimiento").Value).ToString("yyyyMMdd"));
                    dBDataSource.SetValue("U_BPP_REFERENCIA", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("Referencia").Value).ToString());
                    dBDataSource.SetValue("U_BPP_REF2", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("Referencia2").Value).ToString());
                    dBDataSource.SetValue("U_BPP_COMENTARIO", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("Comentarios").Value).ToString());
                    dBDataSource.SetValue("U_BPP_CC1", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("CC1").Value).ToString());
                    dBDataSource.SetValue("U_BPP_CC2", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("CC2").Value).ToString());
                    dBDataSource.SetValue("U_BPP_CC3", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("CC3").Value).ToString());
                    dBDataSource.SetValue("U_BPP_CC4", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("CC4").Value).ToString());
                    dBDataSource.SetValue("U_BPP_CC5", dBDataSource.Size - 1, ((dynamic)recordset.Fields.Item("CC5").Value).ToString());
                    recordset.MoveNext();
                }

                matrix.Clear();
                matrix.LoadFromDataSource();
                matrix.AutoResizeColumns();
                oForm.Freeze(newVal: false);
            }
            catch (Exception ex)
            {
                SAPMain.MensajeError(ex.Message.ToString(), estado: true);
                oForm.Freeze(newVal: false);
            }
        }
        private bool generarAsientos()
        {
            int num = SAPMain.SBO_Application.MessageBox("Se generarán los asientos de destino, ¿Desea continuar?", 1, "Si", "No");
            if (num != 1)
            {
                return false;
            }

            try
            {
                Recordset recordset = (Recordset)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                JournalEntries journalEntries = null;
                Matrix matrix = (Matrix)(dynamic)oForm.Items.Item("matDet1").Specific;
                int rowCount = matrix.RowCount;
                ComboBox comboBox = (dynamic)oForm.Items.Item("txtPeriodo").Specific;
                if (rowCount != 0 && !comboBox.Value.Equals(""))
                {
                    string description = comboBox.Selected.Description;
                    DBDataSource dBDataSource = oForm.DataSources.DBDataSources.Item("@BPP_CTD_DET1");
                    if (!SAPMain.oCompany.InTransaction)
                    {
                        SAPMain.oCompany.StartTransaction();
                    }

                    SAPMain.MensajeAdvertencia("Creando los asientos destino. Espere por favor...");
                    oForm.Freeze(newVal: true);
                    for (int i = 0; i < dBDataSource.Size; i++)
                    {
                        double num2 = double.Parse(dBDataSource.GetValue("U_BPP_MNTLOCAL", i).ToString().Trim());
                        double num3 = double.Parse(dBDataSource.GetValue("U_BPP_MNTEXTR", i).ToString().Trim());
                        double num4 = double.Parse(dBDataSource.GetValue("U_BPP_MNTSIST", i).ToString().Trim());
                        journalEntries = (JournalEntries)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.oJournalEntries);
                        journalEntries.ReferenceDate = DateTime.ParseExact(dBDataSource.GetValue("U_BPP_FECCONT", i).ToString().Trim(), "yyyyMMdd", null);
                        journalEntries.TaxDate = DateTime.ParseExact(dBDataSource.GetValue("U_BPP_FECDOC", i).ToString().Trim(), "yyyyMMdd", null);
                        journalEntries.DueDate = DateTime.ParseExact(dBDataSource.GetValue("U_BPP_FECVENC", i).ToString().Trim(), "yyyyMMdd", null);
                        journalEntries.TransactionCode = "DES";
                        journalEntries.Reference = dBDataSource.GetValue("U_BPP_ASIENTOORG", i).ToString().Trim();
                        journalEntries.Reference2 = dBDataSource.GetValue("U_BPP_REF2", i).ToString().Trim();
                        journalEntries.Memo = "ADD-ON BPP ASIENTO DE DESTINO " + description;
                        journalEntries.Lines.AccountCode = retornaAcctCode(dBDataSource.GetValue("U_BPP_CTADEST", i).ToString().Trim());
                        journalEntries.Lines.ShortName = journalEntries.Lines.AccountCode;
                        journalEntries.Lines.Reference1 = dBDataSource.GetValue("U_BPP_ASIENTOORG", i).ToString().Trim();
                        journalEntries.Lines.Reference2 = dBDataSource.GetValue("U_BPP_REF2", i).ToString().Trim();
                        journalEntries.Lines.LineMemo = dBDataSource.GetValue("U_BPP_COMENTARIO", i).ToString().Trim();
                        if (num2 >= 0.0 && num4 >= 0.0)
                        {
                            journalEntries.Lines.Debit = num2;
                            journalEntries.Lines.DebitSys = num4;
                        }
                        else
                        {
                            journalEntries.Lines.Credit = num2 * -1.0;
                            journalEntries.Lines.CreditSys = num4 * -1.0;
                        }

                        if (num3 != 0.0)
                        {
                            journalEntries.Lines.FCCurrency = dBDataSource.GetValue("U_BPP_MONEDA", i).ToString().Trim();
                            if (num3 >= 0.0 && num4 >= 0.0)
                            {
                                journalEntries.Lines.FCDebit = num3;
                            }
                            else
                            {
                                journalEntries.Lines.FCCredit = num3 * -1.0;
                            }
                        }

                        journalEntries.Lines.Add();
                        journalEntries.Lines.SetCurrentLine(1);
                        journalEntries.Lines.AccountCode = retornaAcctCode(SAPMain.cuentaContraPartida);
                        journalEntries.Lines.ShortName = journalEntries.Lines.AccountCode;
                        journalEntries.Lines.Reference1 = dBDataSource.GetValue("U_BPP_ASIENTOORG", i).ToString().Trim();
                        journalEntries.Lines.Reference2 = dBDataSource.GetValue("U_BPP_REF2", i).ToString().Trim();
                        journalEntries.Lines.LineMemo = dBDataSource.GetValue("U_BPP_COMENTARIO", i).ToString().Trim();
                        journalEntries.Lines.CostingCode = dBDataSource.GetValue("U_BPP_CC1", i).ToString().Trim();
                        journalEntries.Lines.CostingCode2 = dBDataSource.GetValue("U_BPP_CC2", i).ToString().Trim();
                        journalEntries.Lines.CostingCode3 = dBDataSource.GetValue("U_BPP_CC3", i).ToString().Trim();
                        journalEntries.Lines.CostingCode4 = dBDataSource.GetValue("U_BPP_CC4", i).ToString().Trim();
                        journalEntries.Lines.CostingCode5 = dBDataSource.GetValue("U_BPP_CC5", i).ToString().Trim();
                        if (num2 >= 0.0 && num4 >= 0.0)
                        {
                            journalEntries.Lines.Credit = num2;
                            journalEntries.Lines.CreditSys = num4;
                        }
                        else
                        {
                            journalEntries.Lines.Debit = num2 * -1.0;
                            journalEntries.Lines.DebitSys = num4 * -1.0;
                        }

                        if (num3 != 0.0)
                        {
                            journalEntries.Lines.FCCurrency = dBDataSource.GetValue("U_BPP_MONEDA", i).ToString().Trim();
                            if (num3 >= 0.0 && num4 >= 0.0)
                            {
                                journalEntries.Lines.FCCredit = num3;
                            }
                            else
                            {
                                journalEntries.Lines.FCDebit = num3 * -1.0;
                            }
                        }

                        if (journalEntries.Add() != 0)
                        {
                            if (SAPMain.oCompany.InTransaction)
                            {
                                SAPMain.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                            }

                            oForm.Freeze(newVal: false);
                            string mensaje = $"{SAPMain.oCompany.GetLastErrorCode()}-{SAPMain.oCompany.GetLastErrorDescription()}";
                            SAPMain.MensajeError(mensaje, estado: true);
                            return false;
                        }

                        string newObjectKey = SAPMain.oCompany.GetNewObjectKey();
                        dBDataSource.SetValue("U_BPP_ASIENTODEST", i, newObjectKey);
                        actlzAsientosExitoso(dBDataSource.GetValue("U_BPP_ASIENTOORG", i));
                        SAPMain.MensajeExito("Se creo el asiento : " + newObjectKey);
                    }

                    matrix.LoadFromDataSource();
                    if (SAPMain.oCompany.InTransaction)
                    {
                        SAPMain.oCompany.EndTransaction(BoWfTransOpt.wf_Commit);
                        ComboBox comboBox2 = (dynamic)oForm.Items.Item("txtEstado").Specific;
                        comboBox2.Select("Procesado");
                        oForm.Freeze(newVal: false);
                        return true;
                    }
                }
                else if (comboBox.Value.Equals(""))
                {
                    SAPMain.MensajeError("Debe seleccionar un periodo.", estado: true);
                }
                else if (rowCount == 0)
                {
                    SAPMain.MensajeError("No existen filas por procesar.", estado: true);
                }

                return false;
            }
            catch (Exception ex)
            {
                if (SAPMain.oCompany.InTransaction)
                {
                    SAPMain.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                    SAPMain.MensajeError(ex.ToString(), estado: true);
                    oForm.Freeze(newVal: false);
                }

                SAPMain.MensajeError(ex.ToString(), estado: true);
                return false;
            }
        }
        private void actlzAsientosExitoso(string transId)
        {
            try
            {
                Recordset recordset = (Recordset)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string text = "UPDATE OJDT SET \"U_STR_ADP\" = 'Y' WHERE \"TransId\" = '" + transId + "'";
                Global.WriteToFile(text);
                recordset.DoQuery(text);
                recordset = null;
            }
            catch (Exception ex)
            {
                Global.WriteToFile(ex.Message);
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

                Global.WriteToFile(query);
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
                    Global.WriteToFile($"SELECT \"AcctCode\" FROM OACT WHERE \"Segment_0\" = '{codes[0]}'");
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

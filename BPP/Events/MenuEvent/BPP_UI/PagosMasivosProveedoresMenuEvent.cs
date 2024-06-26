 
using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPP
{
    public class PagosMasivosProveedoresMenuEvent
    {
        private SAPbouiCOM.Form oForm;
        public void menuAction(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            Recordset oRecordSet = (SAPbobsCOM.Recordset)SAPMain.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            this.oForm = SAPMain.SBO_Application.Forms.Item("UDO_FT_BPP_PAGM4");
            oForm.Select();
            try
            {
                if (pVal.BeforeAction)
                {

                    switch (pVal.MenuUID)
                    {

                        case "1284":

                            string oDocEntry = oForm.Items.Item("0_U_E").Specific.Value;

                            try
                            {
                                Payments oPago = null;
                                oRecordSet = (Recordset)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                                string query = string.Format("SELECT T2.\"U_BPP_PAGO\" Pago FROM \"@BPP_PAGM_CAB\" T1 INNER JOIN \"@BPP_PAGM_DET1\" T2 ON T1.\"DocEntry\" = T2.\"DocEntry\" WHERE T1.\"DocEntry\"  = {0} ", oDocEntry);
                                oRecordSet.DoQuery(query);

                                if (!SAPMain.oCompany.InTransaction)
                                {
                                    SAPMain.oCompany.StartTransaction();
                                }

                                SAPMain.MensajeAdvertencia("Se estan cancelando los Pagos o la Planilla. Espere por favor...");
                                int cont = 0;

                                while (!oRecordSet.EoF)
                                {
                                    oPago = (Payments)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.oVendorPayments);

                                    string oDocentryPago = oRecordSet.Fields.Item("Pago").Value.ToString();

                                    if (oPago.GetByKey(int.Parse(oDocentryPago)))
                                    {

                                        if (oPago.Cancel() != 0)
                                        {
                                            if (SAPMain.oCompany.InTransaction)
                                            {
                                                SAPMain.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                                            }
                                            //oForm.Freeze(false);

                                            string error = string.Format("{0}-{1}", SAPMain.oCompany.GetLastErrorCode(), SAPMain.oCompany.GetLastErrorDescription());
                                            SAPMain.MensajeError(error, true);
                                            break;
                                        }
                                        else
                                        {
                                            SAPMain.MensajeAdvertencia("Se cancelo el pago : " + oDocentryPago);
                                        }

                                    }
                                    cont++;

                                    oRecordSet.MoveNext();
                                }

                                if (SAPMain.oCompany.InTransaction)
                                {
                                    SAPMain.oCompany.EndTransaction(BoWfTransOpt.wf_Commit);
                                    oRecordSet = (Recordset)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                                    query = string.Format("UPDATE \"@BPP_PAGM_CAB\" SET  U_BPP_ESTADO = 'Cancelado' WHERE \"DocEntry\"  = {0} ", oDocEntry);
                                    Global.WriteToFile(query);

                                    oRecordSet.DoQuery(query);

                                    SAPMain.MensajeAdvertencia("Se cancelaron un total de  : " + cont + "Pagos ");
                                    SAPMain.MensajeExito("Se cancelo la planilla satisfactoriamente.");
                                }

                            }


                            catch (Exception ex)
                            {
                                if (SAPMain.oCompany.InTransaction)
                                {
                                    SAPMain.oCompany.EndTransaction(BoWfTransOpt.wf_RollBack);
                                }
                                string error = ex.Message.ToString();
                                SAPMain.MensajeError(error, true);

                            }

                            break;

                    }

                }
                else
                {
                    switch (pVal.MenuUID)
                    {
                        case "1281":

                            oForm.Items.Item("0_U_E").Enabled = true;

                            break;

                        case "1282":
                            oForm.Items.Item("txtFeccrea").Specific.Value = DateTime.Now.ToString("yyyyMMdd");
                            ((SAPbouiCOM.ComboBox)(oForm.Items.Item("22_U_E").Specific)).Select("Creado");
                            ((SAPbouiCOM.ComboBox)(oForm.Items.Item("txtSerie").Specific)).Select(DateTime.Now.Year.ToString());
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

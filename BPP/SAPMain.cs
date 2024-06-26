using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPP
{
    public class SAPMain
    {
        public static SAPbouiCOM.Application SBO_Application = null;
        public static SAPbobsCOM.Company oCompany = null;

        public static string cuentaPuente = null;
        public static string numeroLote = null;
        public static string rutaDetraciones = null;
        public static string rutaPagos = null;
        public static string codTransaccionDestino = null;
        public static int opcionPagoMasivo = 1;

        public static string cuentaContraPartida = null;
        public static bool segmentado;

        public static string procBCPCab = "SP_BPP_GNRTXT_BCP_CAB";
        public static string procBCPDet = "SP_BPP_GNRTXT_BCP_DET";
        public static string procBVACab = "SP_BPP_GNRTXT_BBVA_CAB";
        public static string procBVADet = "SP_BPP_GNRTXT_BBVA_DET";
        public static string procINBKCab = "SP_BPP_GNRTXT_INTBK_CAB";
        public static string procINBKDet = "SP_BPP_GNRTXT_INTBK_DET";
        public static string procSCBKCab = "SP_BPP_GNRTXT_SCTBK_CAB";
        public static string procSCBKDet = "SP_BPP_GNRTXT_SCTBK_DET";

        public static void MensajeExito(string mensaje)
        {
            SBO_Application.StatusBar.SetText(mensaje, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
        }

        public static void MensajeAdvertencia(string mensaje)
        {
            SBO_Application.StatusBar.SetText(mensaje, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
        }

        public static void MensajeError(string mensaje, bool estado = false)
        {
            if (!estado)
                SBO_Application.MessageBox(mensaje);
            else
                SBO_Application.StatusBar.SetText(mensaje, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
        }

        public static void Mensaje(string mensaje)
        {
            SBO_Application.StatusBar.SetText(mensaje, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_None);
        }

        public enum FormularioUsuario
        {
            frmParam,
            UDO_FT_BPP_PAGM4,
            UDO_FT_BPP_DETR2,
            UDO_FT_BPP_CTADEST3
        }
    }
}

using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using static BPP.SAPMain;

namespace BPP
{
    public class SAPInit
    {

        public SAPInit()
        {
            SetApplication();
            if (ValidarRegion())
            {
                SetEvents();
                sb_Filters();
                AddnMenuItems();
                ConnectToCompany();
                ValidarRegion();
                SetearVariables();
            }

        }

        private static void SetApplication()
        {
            Global.WriteToFile("Se activo el ADDON DE PAGOS MASIVOS");
            SboGuiApi sboGuiApi = null;
            string fileStream = null;
            sboGuiApi = (SboGuiApi)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("6CF0D1E0-470B-4684-B9B5-70F9A5ACBB06")));
            sboGuiApi.Connect("0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056");
            SAPMain.SBO_Application = sboGuiApi.GetApplication();
            SAPMain.oCompany = (dynamic)SAPMain.SBO_Application.Company.GetDICompany();
        }

        private int ConnectToCompany()
        {
            int num = 0;
            return SAPMain.oCompany.Connect();
        }

        private bool ValidarRegion()
        {
            return true;
        }

        private void SetearVariables()
        {
            try
            {
                Recordset recordset = (Recordset)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                string fileStream = $"SELECT TOP 1 * FROM \"@BPP_PARAMS\"";
                Global.WriteToFile(fileStream);
                recordset.DoQuery(fileStream);
                SAPMain.cuentaPuente = ((dynamic)recordset.Fields.Item("U_BPP_CNTPUENTE").Value).ToString();
                SAPMain.numeroLote = ((dynamic)recordset.Fields.Item("U_BPP_NROLOTE").Value).ToString();
                SAPMain.rutaDetraciones = ((dynamic)recordset.Fields.Item("U_BPP_DETRUTA").Value).ToString();
                SAPMain.rutaPagos = ((dynamic)recordset.Fields.Item("U_BPP_PGMRUTA").Value).ToString();
                SAPMain.cuentaContraPartida = ((dynamic)recordset.Fields.Item("U_BPP_CNTCONTR").Value).ToString();
                SAPMain.opcionPagoMasivo = Convert.ToInt32((dynamic)recordset.Fields.Item("U_STR_NUMOPERTXT").Value);
                SAPMain.segmentado = GetSegmento();
            }
            catch (Exception)
            {
            }
        }

        private bool GetSegmento()
        {
            Recordset recordset = (Recordset)(dynamic)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            Global.WriteToFile("SELECT \"EnbSgmnAct\" FROM CINF");
            recordset.DoQuery("SELECT \"EnbSgmnAct\" FROM CINF");
            if (recordset.Fields.Count < 1)
            {
                Global.WriteToFile("No se cuenta con segmento activo");
                throw new Exception("No se cuenta con segmento activo");
            }
            return ((dynamic)recordset.Fields.Item(0).Value).Equals("Y");
        }

        public void SetEvents()
        {

            _AppEvent oAppEvent = new _AppEvent();
            _MenuEvent oMenuEvent = new _MenuEvent();
            _ItemEvent oItemEvent = new _ItemEvent();
            _FormDataEvent oFormDataEvent = new _FormDataEvent();
            ////_RightClickEvent oRightClickEvent = new _RightClickEvent();
            SAPMain.SBO_Application.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(oAppEvent.DoAction);
            SAPMain.SBO_Application.MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(oMenuEvent.DoAction);
            SAPMain.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(oItemEvent.DoAction);
            SAPMain.SBO_Application.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(oFormDataEvent.DoAction);
            //ClsMain.SBO_Application.RightClickEvent += new SAPbouiCOM._IApplicationEvents_RightClickEventEventHandler(oRightClickEvent.DoAction);

        }

        private void sb_Filters()
        {
            EventFilters fileStream = (EventFilters)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("400686C8-0705-4A36-BFA5-D49D1B361388")));
            EventFilter eventFilter = null;
            eventFilter = fileStream.Add(BoEventTypes.et_ALL_EVENTS);
            Enum.GetNames(typeof(SAPMain.FormularioUsuario)).Cast<string>().ToList()
                .ForEach(delegate (string s)
                {
                    eventFilter.AddEx(s);
                });
            SAPMain.SBO_Application.SetFilter(fileStream);
        }

        private void AddnMenuItems()
        {
            XmlDocument xmlDocument = new XmlDocument();
            SAPMain.SBO_Application.Forms.GetFormByTypeAndCount(169, 1).Freeze(newVal: true);
            try
            {
                Menus fileStream = null;
                SAPbouiCOM.MenuItem logDirInfo = null;
                MenuCreationParams logFileInfo = null;
                logFileInfo = (MenuCreationParams)(dynamic)SAPMain.SBO_Application.CreateObject(BoCreatableObjectType.cot_MenuCreationParams);
                logDirInfo = SAPMain.SBO_Application.Menus.Item("MNULOCALI");
                fileStream = logDirInfo.SubMenus;
                if (!fileStream.Exists("mnuParam"))
                {
                    logFileInfo.Type = BoMenuType.mt_STRING;
                    logFileInfo.UniqueID = "mnuParam";
                    logFileInfo.String = "Configuración Pagos Masivo";
                    logFileInfo.Position = 6;
                    logFileInfo.Image = "";
                    fileStream.AddEx(logFileInfo);
                }
                logDirInfo = SAPMain.SBO_Application.Menus.Item("MNULOCALI");
                fileStream = logDirInfo.SubMenus;
                if (!fileStream.Exists("mnuPagos"))
                {
                    logFileInfo.Type = BoMenuType.mt_STRING;
                    logFileInfo.UniqueID = "mnuPagos";
                    logFileInfo.String = "Pagos Masivos de Proveedores";
                    logFileInfo.Position = 8;
                    logFileInfo.Image = "";
                    fileStream.AddEx(logFileInfo);
                }
                logDirInfo = SAPMain.SBO_Application.Menus.Item("MNULOCALI");
                fileStream = logDirInfo.SubMenus;
                if (!fileStream.Exists("mnuCtaDest"))
                {
                    logFileInfo.Type = BoMenuType.mt_STRING;
                    logFileInfo.UniqueID = "mnuCtaDest";
                    logFileInfo.String = "Cuenta Destino - Detalle";
                    logFileInfo.Position = 9;
                    logFileInfo.Image = "";
                    fileStream.AddEx(logFileInfo);
                }
            }
            catch (FileNotFoundException)
            {
                Global.WriteToFile("El recurso: Menu.xml, no fue encontrado...");
                SAPMain.SBO_Application.StatusBar.SetText("El recurso: Menu.xml, no fue encontrado...", BoMessageTime.bmt_Short);
            }
            catch (Exception ex2)
            {
                Global.WriteToFile(ex2.Message.ToString());
                SAPMain.SBO_Application.StatusBar.SetText(ex2.Message, BoMessageTime.bmt_Short);
            }
            finally
            {
                SAPMain.SBO_Application.Forms.GetFormByTypeAndCount(169, 1).Freeze(newVal: false);
                SAPMain.SBO_Application.Forms.GetFormByTypeAndCount(169, 1).Update();
                xmlDocument = null;
            }
        }
    }
}

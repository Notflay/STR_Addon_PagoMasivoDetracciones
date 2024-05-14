using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

            SAPbouiCOM.SboGuiApi SboGuiApi = null;
            string sConnectionString = null;
            SboGuiApi = new SAPbouiCOM.SboGuiApi();
            // sConnectionString = System.Convert.ToString(Environment.GetCommandLineArgs().GetValue(1));
            SboGuiApi.Connect("0030002C0030002C00530041005000420044005F00440061007400650076002C0050004C006F006D0056004900490056");

            SAPMain.SBO_Application = SboGuiApi.GetApplication(-1);
            //SAPMain.SBO_Application.SetStatusBarMessage("error",SAPbouiCOM.BoMessageTime.bmt_Medium,true);
            SAPMain.oCompany = SAPMain.SBO_Application.Company.GetDICompany();
        }

        private int ConnectToCompany()
        {
            int connectToCompanyReturn = 0;
            connectToCompanyReturn = SAPMain.oCompany.Connect();
            return connectToCompanyReturn;

        }
        private bool ValidarRegion()
        {
            // Comentado Solo para DRESDEN 
            /*
            var regionInfo = RegionInfo.CurrentRegion;
            string name = regionInfo.Name;
            string englishName = regionInfo.EnglishName;
            string displayName = regionInfo.DisplayName;

            string s;

            s = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;

            if (!s.Equals("."))
            {
                SAPMain.MensajeError("El separador decimal es: '" + s + "'. Formato Incorrecto, actualizar la configuración de Windows a separador decimal '.' . ");

                Environment.Exit(0);
                return false;
            }
            //Console.WriteLine("El separador decimal es: '" + s + "'");
            s = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyGroupSeparator;

            if (!s.Equals(","))
            {
                SAPMain.MensajeError("El separador de miles es: '" + s + "'. Formato Incorrecto, actualizar la configuración de Windows a separador decimal ',' . ");
                Environment.Exit(0);
                return false;
            }

            //Console.WriteLine("El separador de miles es: '" + s + "'");
            */
            return true;

        }
        private void SetearVariables()
        {
            Recordset oRecordSet = (Recordset)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            string query = string.Format("SELECT TOP 1 * FROM \"@BPP_PARAMS\"");
            oRecordSet.DoQuery(query);

            SAPMain.cuentaPuente = oRecordSet.Fields.Item("U_BPP_CNTPUENTE").Value.ToString();
            SAPMain.numeroLote = oRecordSet.Fields.Item("U_BPP_NROLOTE").Value.ToString();
            SAPMain.rutaDetraciones = oRecordSet.Fields.Item("U_BPP_DETRUTA").Value.ToString();
            SAPMain.rutaPagos = oRecordSet.Fields.Item("U_BPP_PGMRUTA").Value.ToString();
            SAPMain.cuentaContraPartida = oRecordSet.Fields.Item("U_BPP_CNTCONTR").Value.ToString();
            SAPMain.segmentado = GetSegmento();
        }
        private bool GetSegmento()
        {
            Recordset oRecordSet = (Recordset)SAPMain.oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            oRecordSet.DoQuery("SELECT \"EnbSgmnAct\" FROM CINF");

            if (oRecordSet.Fields.Count < 1)
                throw new Exception("No se cuenta con segmento activo");

            return oRecordSet.Fields.Item(0).Value.Equals("Y");
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
            SAPbouiCOM.EventFilters eventFilters = new SAPbouiCOM.EventFilters();
            SAPbouiCOM.EventFilter eventFilter = null;

            eventFilter = eventFilters.Add(SAPbouiCOM.BoEventTypes.et_ALL_EVENTS);
            Enum.GetNames(typeof(FormularioUsuario)).Cast<string>().ToList().ForEach(s => eventFilter.AddEx(s));
            SBO_Application.SetFilter(eventFilters);
        }

        private void AddnMenuItems()
        {
            XmlDocument oMnuXML = new XmlDocument();
            SAPMain.SBO_Application.Forms.GetFormByTypeAndCount(169, 1).Freeze(true);
            try
            {
                //string rutaMenuXML = $"{Application.StartupPath}\\Menus\\Menu.xml";

                /*
                oMnuXML.LoadXml(Properties.Resources.Menu);
                SAPMain.SBO_Application.LoadBatchActions(oMnuXML.InnerXml);
                */

                SAPbouiCOM.Menus oMenus = null;
                SAPbouiCOM.MenuItem oMenuItem = null;
                SAPbouiCOM.MenuCreationParams oCreationPackage = null;

                oCreationPackage = ((SAPbouiCOM.MenuCreationParams)(SAPMain.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)));


                oMenuItem = SAPMain.SBO_Application.Menus.Item("MNULOCALI");
                oMenus = oMenuItem.SubMenus;

                if (!oMenus.Exists("mnuParam")) 
                {
                    oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                    oCreationPackage.UniqueID = "mnuParam";
                    oCreationPackage.String = "Configuración Pagos Masivo";
                    oCreationPackage.Position = 6;
                    oCreationPackage.Image = "";
                    oMenus.AddEx(oCreationPackage);
                }

                oMenuItem = SAPMain.SBO_Application.Menus.Item("MNULOCALI");
                oMenus = oMenuItem.SubMenus;

                if (!oMenus.Exists("mnuPagos"))
                {
                    oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                    oCreationPackage.UniqueID = "mnuPagos";
                    oCreationPackage.String = "Pagos Masivos de Proveedores";
                    oCreationPackage.Position = 8;
                    oCreationPackage.Image = "";
                    oMenus.AddEx(oCreationPackage);
                }

                    oMenuItem = SAPMain.SBO_Application.Menus.Item("MNULOCALI");
                oMenus = oMenuItem.SubMenus;

                if (!oMenus.Exists("mnuCtaDest"))
                {
                    oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                    oCreationPackage.UniqueID = "mnuCtaDest";
                    oCreationPackage.String = "Cuenta Destino - Detalle";
                    oCreationPackage.Position = 9;
                    oCreationPackage.Image = "";
                    oMenus.AddEx(oCreationPackage);
                }

            }
            catch (System.IO.FileNotFoundException)
            {
                SAPMain.SBO_Application.StatusBar.SetText("El recurso: Menu.xml, no fue encontrado...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            catch (Exception ex)
            {
                SAPMain.SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            finally
            {
                SAPMain.SBO_Application.Forms.GetFormByTypeAndCount(169, 1).Freeze(false);
                SAPMain.SBO_Application.Forms.GetFormByTypeAndCount(169, 1).Update();
                oMnuXML = null;
            }
        }
    }
}

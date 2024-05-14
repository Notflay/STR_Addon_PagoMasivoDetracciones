 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPP
{
    public class _AppEvent
    {
        public void DoAction(SAPbouiCOM.BoAppEventTypes EventType)
        {
            try
            {
                switch (EventType)
                {
                    case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                        System.Environment.Exit(0);
                        break;
                    case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                        System.Environment.Exit(0);
                        break;
                    case SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged:
                        break;
                }
            }
            catch (Exception ex)
            {
                //SAPMain.MensajeError(ex.Message);
            }
        }
    }
}

using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPP
{
    public class PurchaseInvoiceItemEvent
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


                    switch (pVal.EventType)
                    {

                        case BoEventTypes.et_FORM_LOAD:

                            FormUI(ref pVal);

                            break;
                    }
                }

            }
            catch (Exception ex)
            {
            }

        }
        private void FormUI(ref SAPbouiCOM.ItemEvent pVal)
        {
            SAPbouiCOM.Form sboForm;
            SAPbouiCOM.StaticText sboBtn;
            SAPbouiCOM.Item sboNewItem;
            SAPbouiCOM.Item sboItem;

            string colorcode = string.Empty;
            colorcode = "#66CC00";
            string colour = colorcode.TrimStart('#');
            string R = colour.Substring(0, 2);
            string G = colour.Substring(2, 2);
            string B = colour.Substring(4, 2);

            sboForm = SAPMain.SBO_Application.Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);

            sboNewItem = sboForm.Items.Add("lblEstado", SAPbouiCOM.BoFormItemTypes.it_STATIC);
            sboItem = sboForm.Items.Item("4");
            sboBtn = sboNewItem.Specific;
            sboBtn.Caption = "▼ Add-On Perú Encendido";
            sboNewItem.BackColor = Int32.Parse(B + G + R, NumberStyles.HexNumber);

            sboNewItem.Top = sboItem.Top;
            sboNewItem.Height = sboItem.Height;
            sboNewItem.Width = 130;
            sboNewItem.Left = sboItem.Left + sboItem.Width + 5;

        }
    }
}

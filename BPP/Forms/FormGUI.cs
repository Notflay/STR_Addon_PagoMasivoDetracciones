using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPP
{
    public class FormGUI
    {
        protected void LoadFromXML(ref string FileName)
        {
            string sPath = null;
            string sXML = null;

            System.Xml.XmlDocument oXmlDoc = null;
            oXmlDoc = new System.Xml.XmlDocument();
            sPath = System.Windows.Forms.Application.StartupPath.ToString();
            oXmlDoc.Load(sPath + "\\" + FileName);
            sXML = oXmlDoc.InnerXml.ToString();
            SAPMain.SBO_Application.LoadBatchActions(ref sXML);
        }
    }
}

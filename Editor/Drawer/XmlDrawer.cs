using System.Xml;
using Newtonsoft.Json;

namespace Inheo.UParser
{
    internal class XmlDrawer : BaseFileDrawer
    {
        private JsonTokenDrawer tokenDrawer;
        private XmlDocument xml;

        protected override bool IsNeedUpdateConditions => false;
        protected override string FileKey => "XmlFilePath";
        protected override string FileExtensions => ".xml";
        protected override string CurrentText
        {
            get
            {
                var node = JsonConvert.DeserializeXmlNode(tokenDrawer.Text);
                return node.OuterXml;
            }
        }

        public XmlDrawer()
        {
            xml = new XmlDocument();
            tokenDrawer = new JsonTokenDrawer();
        }

        protected override void DrawBody()
        {
            if (tokenDrawer.IsCurrentNull || tokenDrawer.Text == null || string.IsNullOrEmpty(tokenDrawer.Text))
            {
                UpdateCurrentData();
                return;
            }

            tokenDrawer.Draw();
        }

        protected override void UpdateCurrentData()
        {
            SaveFilePath();

            xml.LoadXml(FileText);
            string json = JsonConvert.SerializeXmlNode(xml);
            tokenDrawer.Load(json);
        }
    }
}
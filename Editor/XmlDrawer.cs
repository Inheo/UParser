using System;

namespace Inheo.UParser
{
    internal class XmlDrawer : BaseFileDrawer
    {
        protected override bool IsNeedUpdateConditions => false;
        protected override string FileKey => "XmlFilePath";
        protected override string FileExtensions => ".xml";

        protected override void DrawBody()
        {
        }

        protected override void UpdateCurrentData()
        {
        }
    }
}
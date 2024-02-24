namespace Inheo.UParser
{
    internal class JsonDrawer : BaseFileDrawer
    {
        private JsonTokenDrawer tokenDrawer;

        protected override bool IsNeedUpdateConditions => tokenDrawer.IsCurrentNull;
        protected override string FileKey => "JsonFilePath";
        protected override string FileExtensions => ".json";
        protected override string CurrentText => tokenDrawer.Text;

        public JsonDrawer()
        {
            tokenDrawer = new JsonTokenDrawer();
        }

        protected override void DrawBody()
        {
            tokenDrawer.Draw();
        }

        protected override void UpdateCurrentData()
        {
            SaveFilePath();
            tokenDrawer.Load(FileText);
        }
    }
}
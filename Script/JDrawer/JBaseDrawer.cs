using Newtonsoft.Json.Linq;

namespace Inheo.UParser.JDrawer
{
    internal abstract class JBaseDrawer
    {
        internal abstract void Draw(string label, JToken token);
        internal abstract void DrawBody(JToken token);
    }
}
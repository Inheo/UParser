using Newtonsoft.Json.Linq;

namespace Inheo.UParser.JDrawer
{
    internal abstract class JDrawer
    {
        internal abstract void Draw(string label, JToken token);
    }
}
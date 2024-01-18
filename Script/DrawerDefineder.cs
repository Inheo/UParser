using Inheo.UParser.JDrawer;
using Newtonsoft.Json.Linq;

namespace Inheo.UParser
{
    internal static class DrawerDefineder
    {
        private static JValueDrawer jValue;
        private static JObjectDrawer jObject;
        private static JArrayDrawer jArray;

        static DrawerDefineder()
        {
            jValue = new JValueDrawer();
            jObject = new JObjectDrawer();
            jArray = new JArrayDrawer();
        }

        internal static JBaseDrawer Find(JTokenType tokenType)
        {
            switch (tokenType)
            {
                case JTokenType.String:
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.Boolean:
                case JTokenType.Guid:
                    return jValue;
                case JTokenType.Object:
                    return jObject;
                case JTokenType.Array:
                    return jArray;
                default:
                    return null;
            }
        }
    }
}
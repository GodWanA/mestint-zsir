using System.Xml;

namespace WpfApp1.Classes
{
    internal interface IXMLSave
    {
        public void SaveToXML(ref XmlWriter xml);
        public void LoadFromXML(XmlNode xml);
    }
}

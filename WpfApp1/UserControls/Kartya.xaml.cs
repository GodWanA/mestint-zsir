using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml;
using WpfApp1.Classes;

namespace WpfApp1.UserControls
{
    /// <summary>
    /// Interaction logic for Kartya.xaml
    /// </summary>
    public partial class Kartya : UserControl, IXMLSave
    {
        public Szin Szine { get; private set; }
        public Ertek Erteke { get; private set; }
        public bool IsCardVisible { get; private set; }

        public Thickness OpenedMargin { get; set; } = new Thickness(0);
        public Thickness ClosedMargin { get; set; } = new Thickness(0);

        private const string sourcePath = "pack://application:,,,/Images/Kartya/";

        private bool _isInHand;

        public bool IsInHand
        {
            get { return _isInHand; }
            set
            {
                _isInHand = value;
                if (_isInHand) this.Margin = this.ClosedMargin;
                else this.Margin = this.OpenedMargin;
            }
        }

        public Kartya()
        {
            InitializeComponent();
            this.Szine = Szin.UNKNOWN;
            this.Erteke = Ertek.UNKNOWN;
            this.IsCardVisible = false;
        }

        public Kartya(Szin szin, Ertek ertek, bool visible)
        {
            InitializeComponent();
            this.Szine = szin;
            this.Erteke = ertek;
            this.IsCardVisible = visible;
        }

        public void SetCardVisible(bool visible)
        {
            this.IsCardVisible = visible;

            if (this.IsCardVisible && this.Szine != Szin.UNKNOWN && this.Erteke != Ertek.UNKNOWN)
            {
                var s = Kartya.sourcePath + this.Szine + "/" + this.Erteke + ".png";
                this.image_KartyaKep.Source = new BitmapImage(new Uri(s));
            }
            else
            {
                this.image_KartyaKep.Source = new BitmapImage(new Uri(Kartya.sourcePath + "Hatter.png"));
            }
        }

        public override string ToString()
        {
            // return base.ToString();
            var sb = new StringBuilder();
            sb.AppendLine("Neve: " + this.Name);
            sb.AppendLine("Színe: " + this.Szine + ";");
            sb.AppendLine("Értéke: " + this.Erteke + ";");
            sb.AppendLine("Látható-e: " + this.IsCardVisible + ";");

            return sb.ToString();
        }

        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.IsLoaded && this.IsInHand) this.Margin = this.OpenedMargin;
        }

        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.IsLoaded && this.IsInHand) this.Margin = this.ClosedMargin;
        }

        public void SetMargin(Thickness opened, Thickness closed)
        {
            this.OpenedMargin = opened;
            this.ClosedMargin = closed;

            if (this.IsInHand) this.Margin = this.ClosedMargin;
            else this.Margin = this.OpenedMargin;
        }

        public void SaveToXML(ref XmlWriter xml)
        {
            if (xml != null)
            {
                xml.WriteStartElement("Kartya");
                xml.WriteAttributeString("Szin", this.Szine.ToString());
                xml.WriteAttributeString("Erteke", this.Erteke.ToString());
                xml.WriteAttributeString("Lathato", this.IsCardVisible.ToString());
                xml.WriteAttributeString("Kezben", this.IsInHand.ToString());
                xml.WriteAttributeString("OpenedMargin", this.OpenedMargin.ToString());
                xml.WriteAttributeString("ClosedMargin", this.ClosedMargin.ToString());
                xml.WriteEndElement();
            }
        }

        public void LoadFromXML(XmlNode xml)
        {
            if (xml?.Attributes != null)
            {
                this.Szine = (Szin)Enum.Parse(Szin.Piros.GetType(), xml.Attributes["Szin"].Value);
                this.Erteke = (Ertek)Enum.Parse(Ertek.Also.GetType(), xml.Attributes["Erteke"].Value);
                this.IsCardVisible = bool.Parse(xml.Attributes["Lathato"].Value);
                this.IsInHand = bool.Parse(xml.Attributes["Kezben"].Value);
                this.OpenedMargin = this.ReadThicknes(xml.Attributes["OpenedMargin"].Value);
                this.ClosedMargin = this.ReadThicknes(xml.Attributes["ClosedMargin"].Value);
            }
        }

        private Thickness ReadThicknes(string value)
        {
            var tmp = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return new Thickness(
                double.Parse(tmp[0], System.Globalization.NumberStyles.Any),
                double.Parse(tmp[1], System.Globalization.NumberStyles.Any),
                double.Parse(tmp[2], System.Globalization.NumberStyles.Any),
                double.Parse(tmp[3], System.Globalization.NumberStyles.Any)
                );
        }
    }

    public enum Szin
    {
        UNKNOWN = -1,
        Piros,
        Tok,
        Zold,
        Makk,
    }

    public enum Ertek
    {
        UNKNOWN = -1,
        VII = 7,
        VIII = 8,
        IX = 9,
        X = 10,
        Also = 2,
        Felso = 3,
        Kiraly = 4,
        Asz = 11,
    }
}

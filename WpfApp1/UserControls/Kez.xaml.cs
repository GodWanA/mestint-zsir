using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using WpfApp1.Classes;

namespace WpfApp1.UserControls
{
    /// <summary>
    /// Interaction logic for Kez.xaml
    /// </summary>
    public partial class Kez : UserControl, IXMLSave
    {
        public event RoutedEventHandler LapotHuz;
        public event RoutedEventHandler TovabbAd;

        public delegate void Laplerakas(Kartya lap, object sender, EventArgs args);
        public event Laplerakas LapKijatszas;

        private Oldal _kepernyoOldal;
        public Oldal KepernyoOldal
        {
            get { return this._kepernyoOldal; }
            set
            {
                this._kepernyoOldal = value;
                if (value == Oldal.Fent || value == Oldal.Lent)
                {
                    this.stackpanel_controllok.Orientation = Orientation.Horizontal;
                    this.stackpanel_hand.Orientation = Orientation.Horizontal;

                    if (value == Oldal.Lent)
                    {
                        Grid.SetRow(this.stackpanel_controllok, 0);
                        Grid.SetRow(this.stackpanel_hand, 1);
                    }
                    else
                    {
                        Grid.SetRow(this.stackpanel_controllok, 1);
                        Grid.SetRow(this.stackpanel_hand, 0);
                    }

                    Grid.SetRowSpan(this.stackpanel_controllok, 1);
                    Grid.SetRowSpan(this.stackpanel_hand, 1);
                    Grid.SetColumnSpan(this.stackpanel_controllok, 2);
                    Grid.SetColumnSpan(this.stackpanel_hand, 2);
                }
                else
                {
                    this.stackpanel_hand.Orientation = Orientation.Vertical;
                    this.stackpanel_controllok.Orientation = Orientation.Vertical;

                    if (value == Oldal.Jobbra)
                    {
                        Grid.SetColumn(this.stackpanel_controllok, 0);
                        Grid.SetColumn(this.stackpanel_hand, 1);
                    }
                    else
                    {
                        Grid.SetColumn(this.stackpanel_controllok, 1);
                        Grid.SetColumn(this.stackpanel_hand, 0);
                    }

                    Grid.SetRowSpan(this.stackpanel_controllok, 2);
                    Grid.SetRowSpan(this.stackpanel_hand, 2);
                    Grid.SetColumnSpan(this.stackpanel_controllok, 1);
                    Grid.SetColumnSpan(this.stackpanel_hand, 1);
                }

                this.SetCardPos();
            }
        }

        internal void TisztaKez()
        {
            this.KartyaSzam = 0;
            this.stackpanel_hand.Children.Clear();
            this.PontSzam = 0;
            this.textBlock_pont.Text = "";
            this.IsEnabled = true;
            this.IsElengedEnabled = false;
        }

        public int KartyaSzam { get; internal set; } = 0;
        public int PontSzam { get; private set; } = 0;

        private bool _isElengedEnabled;
        public bool IsElengedEnabled
        {
            get { return this._isElengedEnabled; }
            internal set
            {
                //if (value != this._isElengedEnabled)
                //{
                this.button_tovabbAd.IsEnabled = value;
                this._isElengedEnabled = value;
                //}
            }
        }

        public Kez()
        {
            InitializeComponent();
            Pakli.ElfogyottPakli += Pakli_ElfogyottPakli;
            this.DataContext = this;
        }

        private void Pakli_ElfogyottPakli(object? sender, EventArgs e)
        {
            // this.button_huz.IsEnabled = false;
        }

        private void button_huz_Click(object sender, RoutedEventArgs e)
        {
            this.LapotHuz?.Invoke(sender, e);
        }

        private void button_tovabbAd_Click(object sender, RoutedEventArgs e)
        {
            this.TovabbAd?.Invoke(sender, e);
        }

        public void KezbeVesz(Kartya lap, bool lathato = false)
        {
            if (lap != null)
            {
                lap.HorizontalAlignment = HorizontalAlignment.Left;
                lap.VerticalAlignment = VerticalAlignment.Top;
                lap.IsInHand = true;
                lap.SetCardVisible(lathato);
                var t = this.CalculateMargin();
                lap.SetMargin(t["Opened"], t["Closed"]);
                lap.PreviewMouseDoubleClick += Lap_PreviewMouseDoubleClick;

                this.stackpanel_hand.Children.Add(lap);
                this.KartyaSzam++;
                this.UpdateLayout();
            }
        }

        internal void AddPont(int v)
        {
            this.PontSzam += v;
            this.textBlock_pont.Text = "Pontszám: " + this.PontSzam;
        }

        private void Lap_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var kartya = sender as Kartya;

            kartya.SetMargin(new Thickness(), new Thickness());
            kartya.IsInHand = false;

            this.KartyaSzam--;
            this.stackpanel_hand.Children.Remove(kartya);

            this.LapKijatszas?.Invoke(kartya, this, new EventArgs());

            kartya.PreviewMouseDoubleClick -= this.Lap_PreviewMouseDoubleClick;
        }

        private Dictionary<string, Thickness> CalculateMargin()
        {
            var ret = new Dictionary<string, Thickness>();
            var o = new Thickness(0);
            var c = new Thickness(0);

            switch (this.KepernyoOldal)
            {
                case Oldal.Lent:
                    c.Right = -70;
                    c.Top = 30;
                    c.Bottom = -50;
                    break;
                case Oldal.Fent:
                    c.Right = -70;
                    c.Top = -30;
                    c.Bottom = 50;
                    break;
                case Oldal.Balra:
                case Oldal.Jobbra:
                    o.Bottom = -30;
                    c.Bottom = -120;
                    break;
            }

            ret.Add("Opened", o);
            ret.Add("Closed", c);
            return ret;
        }

        private void SetCardPos()
        {
            var t = this.CalculateMargin();

            foreach (Kartya kartya in this.stackpanel_hand.Children)
            {
                kartya.SetMargin(t["Opened"], t["Closed"]);
            }
        }

        public void SaveToXML(ref XmlWriter xml)
        {
            if (xml != null)
            {
                xml.WriteStartElement("Kez");
                xml.WriteAttributeString("Neve", this.Name);
                xml.WriteAttributeString("Oldal", this.KepernyoOldal.ToString());
                xml.WriteAttributeString("KartyaSzam", this.KartyaSzam.ToString());
                xml.WriteAttributeString("IsEnabled", this.IsEnabled.ToString());
                xml.WriteAttributeString("PontSzam", this.PontSzam.ToString());

                xml.WriteStartElement("Pakli");
                foreach (var kartya in this.stackpanel_hand.Children.Cast<Kartya>())
                {
                    kartya.SaveToXML(ref xml);
                }
                xml.WriteEndElement();

                xml.WriteEndElement();
            }
        }

        public void LoadFromXML(XmlNode xml)
        {
            this.TisztaKez();

            if (xml != null)
            {
                this.KartyaSzam = int.Parse(xml.Attributes["KartyaSzam"].Value);
                this.IsEnabled = bool.Parse(xml.Attributes["IsEnabled"].Value);
                this.PontSzam = int.Parse(xml.Attributes["PontSzam"].Value);

                this.textBlock_pont.Text = "Pontszám: " + this.PontSzam;

                var pakliXML = xml.SelectSingleNode(".//Pakli");
                if (pakliXML != null)
                {
                    foreach (XmlNode kartya in pakliXML.SelectNodes(".//Kartya"))
                    {
                        var k = new Kartya();
                        k.LoadFromXML(kartya);
                        this.KezbeVesz(k, k.IsCardVisible);
                    }
                }
            }
        }

        internal void SetLathatosag(bool isVisible)
        {
            foreach (Kartya kartya in this.stackpanel_hand.Children)
            {
                kartya.SetCardVisible(isVisible);
            }
        }
    }

    public enum Oldal
    {
        Lent,
        Balra,
        Fent,
        Jobbra,
    }
}

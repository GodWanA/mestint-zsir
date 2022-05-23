using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using WpfApp1.Classes;

namespace WpfApp1.UserControls
{
    /// <summary>
    /// Interaction logic for JatszottLapok.xaml
    /// </summary>
    public partial class JatszottLapok : UserControl, IXMLSave
    {
        Dictionary<Oldal, List<Kartya>> Lapok = new Dictionary<Oldal, List<Kartya>>();

        public Oldal Hivo { get; private set; }

        public JatszottLapok()
        {
            InitializeComponent();
            this.LapTakaríto();
        }

        internal void AppendCard(Kartya lap, Oldal oldal)
        {
            lap.SetCardVisible(true);
            this.grid_content.Children.Add(lap);
            this.Lapok[oldal].Add(lap);

            var m = new Thickness();
            lap.SetMargin(m, m);

            switch (oldal)
            {
                case Oldal.Lent:
                    Grid.SetRow(lap, 1);
                    Grid.SetColumn(lap, 0);
                    break;
                case Oldal.Balra:
                    Grid.SetRow(lap, 0);
                    Grid.SetColumn(lap, 0);
                    break;
                case Oldal.Fent:
                    Grid.SetRow(lap, 0);
                    Grid.SetColumn(lap, 1);
                    break;
                case Oldal.Jobbra:
                    Grid.SetRow(lap, 1);
                    Grid.SetColumn(lap, 1);
                    break;
            }
        }

        internal void Takarit()
        {
            this.grid_content.Children.Clear();
            this.LapTakaríto();
        }

        private void LapTakaríto()
        {
            this.Lapok.Clear();
            this.Lapok.Add(Oldal.Lent, new List<Kartya>());
            this.Lapok.Add(Oldal.Balra, new List<Kartya>());
            this.Lapok.Add(Oldal.Fent, new List<Kartya>());
            this.Lapok.Add(Oldal.Jobbra, new List<Kartya>());
        }

        public KorEredmeny CalculateNyertes(Oldal hivo)
        {
            this.Hivo = hivo;

            var last = new Kartya[]{
                this.Lapok[Oldal.Lent].Last(),
                this.Lapok[Oldal.Balra].Last(),
                this.Lapok[Oldal.Fent].Last(),
                this.Lapok[Oldal.Jobbra].Last(),
            };

            var hivoLap = this.Lapok[hivo].Last();

            var vanHetes = last.Where(x => x.Erteke == Ertek.VII).FirstOrDefault() != null ? true : false;
            var c = this.Lapok.Where(x => x.Value.Last().Erteke == hivoLap.Erteke).Count();

            if (vanHetes)
            {
                var utok = this.Lapok.Where(x => x.Value.Last().Erteke == Ertek.VII).Select(x => x.Key);
                if (utok.Count() > 1) return KorEredmeny.Dontetlen;
                else
                {
                    switch (utok.FirstOrDefault())
                    {
                        case Oldal.Lent: return KorEredmeny.Jatekos;
                        case Oldal.Balra: return KorEredmeny.AI0;
                        case Oldal.Fent: return KorEredmeny.AI1;
                        case Oldal.Jobbra: return KorEredmeny.AI2;
                    }
                }
            }

            if (c > 1) return KorEredmeny.Dontetlen;
            else
            {
                switch (hivo)
                {
                    default: return KorEredmeny.Dontetlen;
                    case Oldal.Lent: return KorEredmeny.Jatekos;
                    case Oldal.Balra: return KorEredmeny.AI0;
                    case Oldal.Fent: return KorEredmeny.AI1;
                    case Oldal.Jobbra: return KorEredmeny.AI2;
                }
            }
        }

        public int CalculatePontszam()
        {
            var pont = this.grid_content.Children.Cast<Kartya>().Where(x => x.Erteke == Ertek.X || x.Erteke == Ertek.Asz).Count();
            this.Takarit();
            return pont;
        }

        public void SaveToXML(ref XmlWriter xml)
        {
            if (xml != null)
            {
                xml.WriteStartElement("JatszottLapok");
                xml.WriteAttributeString("Hivo", this.Hivo.ToString());

                xml.WriteStartElement("Pakli");
                foreach (var oldal in this.Lapok)
                {
                    xml.WriteStartElement("Kijatszott");
                    xml.WriteAttributeString("Oldal", oldal.Key.ToString());

                    foreach (var kartya in oldal.Value) kartya.SaveToXML(ref xml);

                    xml.WriteEndElement();
                }
                xml.WriteEndElement();

                xml.WriteEndElement();
            }
        }

        public void LoadFromXML(XmlNode xml)
        {
            this.Takarit();

            if (xml != null)
            {
                this.Hivo = (Oldal)Enum.Parse(Oldal.Fent.GetType(), xml.Attributes["Hivo"].Value);
                var pakliXML = xml.SelectSingleNode(".//Pakli");

                if (pakliXML != null)
                {
                    var kijatszottXML = pakliXML.SelectNodes(".//Kijatszott");
                    if (kijatszottXML != null)
                    {
                        foreach (XmlNode oldalXML in kijatszottXML)
                        {
                            var oldal = (Oldal)Enum.Parse(Oldal.Lent.GetType(), oldalXML.Attributes["Oldal"].Value);
                            foreach (XmlNode kartya in oldalXML.SelectNodes(".//Kartya"))
                            {
                                var k = new Kartya();
                                k.LoadFromXML(kartya);
                                this.AppendCard(k, oldal);
                            }
                        }
                    }
                }
            }
        }
    }

    public enum KorEredmeny
    {
        Dontetlen,
        Jatekos,
        AI0,
        AI1,
        AI2,
    }
}

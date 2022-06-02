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
    /// Interaction logic for Pakli.xaml
    /// </summary>
    public partial class Pakli : UserControl, IXMLSave
    {
        private List<Kartya> pakli = new List<Kartya>();

        public static int HetesLapokSzama { get; set; }
        public static int ZsirLapokSzama { get; set; }
        public static event EventHandler ElfogyottPakli;
        //public static event EventHandler UjPakli;

        public Pakli()
        {
            InitializeComponent();
        }

        private void Render()
        {
            this.grid_content.Children.Clear();
            foreach (var item in this.pakli) this.grid_content.Children.Add(item);
            this.textblock_lapszam.Text = this.pakli.Count + "db";
        }

        public void GeneratePakli()
        {
            this.pakli.Clear();

            for (Szin i = Szin.Piros; i <= Szin.Makk; i++)
            {
                for (Ertek j = Ertek.Also; j <= Ertek.Asz; j++)
                {
                    var s = Enum.GetName(Ertek.X.GetType(), j);
                    if (s != null)
                    {
                        var uj = new Kartya(i, j, false);
                        //uj.SetCardVisible(true);
                        this.pakli.Add(uj);
                        this.textblock_lapszam.Text = this.pakli.Count + " db";
                    }
                }
            }

            Pakli.HetesLapokSzama = 4;
            Pakli.ZsirLapokSzama = 8;

            this.Render();
        }

        public void PaklitKever()
        {
            var rng = new Random();
            var n = this.pakli.Count;

            while (n > 1)
            {
                var k = rng.Next(n--);
                var kartya = this.pakli[k];
                this.pakli[k] = this.pakli[n];
                this.pakli[n] = kartya;
            }

            this.Render();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.PaklitKever();
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            this.GeneratePakli();
            this.PaklitKever();
        }

        public Kartya LapotHuz()
        {
            if (this.pakli.Count > 0)
            {
                var lap = this.pakli.Last();
                this.pakli.Remove(lap);

                //if (lap.Erteke == Ertek.VII) --Pakli.HetesLapokSzama;
                //if (lap.Erteke == Ertek.X || lap.Erteke == Ertek.Asz) --Pakli.ZsirLapokSzama;
                if (this.pakli.Count == 0) Pakli.ElfogyottPakli?.Invoke(this, new EventArgs());

                this.Render();
                return lap;
            }
            else
            {
                return null;
            }
        }

        public bool CanHuzni()
        {
            return this.pakli.Count > 0;
        }

        public void SaveToXML(ref XmlWriter xml)
        {
            if (xml != null)
            {
                xml.WriteStartElement("Pakli");
                foreach (var kartya in this.pakli)
                {
                    kartya.SaveToXML(ref xml);
                }
                xml.WriteEndElement();
            }
        }

        public void LoadFromXML(XmlNode xml)
        {
            this.pakli.Clear();
            var kartyak = xml.SelectNodes(".//Kartya");

            if (kartyak != null)
            {
                foreach (XmlNode item in kartyak)
                {
                    var k = new Kartya();
                    k.LoadFromXML(item);
                    this.pakli.Add(k);
                }
            }

            this.Render();
        }
    }
}

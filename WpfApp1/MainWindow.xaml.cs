using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using WpfApp1.UserControls;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool Huz(Kez j, bool lathato)
        {
            var ret = this.pakli.CanHuzni();
            if (ret) j.KezbeVesz(this.pakli.LapotHuz(), lathato);
            return ret;
        }

        private void KezetPopulal(Kez j, bool lathato)
        {
            var ok = true;
            while (ok)
            {
                if (j.KartyaSzam < 4) ok = this.Huz(j, lathato);
                if (ok && j.KartyaSzam == 4) ok = false;
            }
        }

        private void jatekos_LapotHuz(object sender, RoutedEventArgs e)
        {
            //this.Huz(this.jatekos, true);
            this.KezetPopulal(this.jatekos, this.menu_megnez.IsChecked);
        }

        private void ai0_LapotHuz(object sender, RoutedEventArgs e)
        {
            //this.Huz(this.ai0, true);
            this.KezetPopulal(this.ai0, false);
        }

        private void ai1_LapotHuz(object sender, RoutedEventArgs e)
        {
            //this.Huz(this.ai1, true);
            this.KezetPopulal(this.ai1, false);
        }

        private void ai2_LapotHuz(object sender, RoutedEventArgs e)
        {
            //this.Huz(this.ai2, true);
            this.KezetPopulal(this.ai2, false);
        }

        private void jatekos_LapKijatszas(Kartya lap, object sender, EventArgs args)
        {
            this.kijatszott.AppendCard(lap, this.jatekos.KepernyoOldal);
            this.jatekos.IsEnabled = false;
            this.CanKorVege();
        }

        private void ai0_LapKijatszas(Kartya lap, object sender, EventArgs args)
        {
            this.kijatszott.AppendCard(lap, this.ai0.KepernyoOldal);
            this.ai0.IsEnabled = false;
            this.CanKorVege();
        }

        private void ai1_LapKijatszas(Kartya lap, object sender, EventArgs args)
        {
            this.kijatszott.AppendCard(lap, this.ai1.KepernyoOldal);
            this.ai1.IsEnabled = false;
            this.CanKorVege();
        }

        private void ai2_LapKijatszas(Kartya lap, object sender, EventArgs args)
        {
            this.kijatszott.AppendCard(lap, this.ai2.KepernyoOldal);
            this.ai2.IsEnabled = false;
            this.CanKorVege();
        }

        private void UjJatekotKezd()
        {
            this.menu_megnez.IsChecked = false;

            this.jatekos.TisztaKez();
            this.ai0.TisztaKez();
            this.ai1.TisztaKez();
            this.ai2.TisztaKez();

            this.kijatszott.Takarit();

            this.pakli.GeneratePakli();
            this.pakli.PaklitKever();

            for (int i = 0; i < 4; i++)
            {
                this.Huz(this.jatekos, true);
                this.Huz(this.ai0, false);
                this.Huz(this.ai1, false);
                this.Huz(this.ai2, false);
            }
        }

        private void menu_ujJatek_Click(object sender, RoutedEventArgs e)
        {
            this.UjJatekotKezd();
        }

        private void CanKorVege(bool isSokadikKor = false)
        {
            var korVege = this.jatekos.IsEnabled == false
                          && this.ai0.IsEnabled == false
                          && this.ai1.IsEnabled == false
                          && this.ai2.IsEnabled == false;

            if (!isSokadikKor)
            {
                if (korVege)
                {
                    var res = this.kijatszott.CalculateNyertes(Oldal.Lent);
                    switch (res)
                    {
                        case KorEredmeny.Jatekos:
                            this.jatekos.AddPont(this.kijatszott.CalculatePontszam());
                            break;
                        case KorEredmeny.AI0:
                            this.ai0.AddPont(this.kijatszott.CalculatePontszam());
                            break;
                        case KorEredmeny.AI1:
                            this.ai1.AddPont(this.kijatszott.CalculatePontszam());
                            break;
                        case KorEredmeny.AI2:
                            this.ai2.AddPont(this.kijatszott.CalculatePontszam());
                            break;
                    }

                    if (res != KorEredmeny.Dontetlen)
                    {
                        this.jatekos.IsElengedEnabled = false;
                        this.ai0.IsElengedEnabled = false;
                        this.ai1.IsElengedEnabled = false;
                        this.ai2.IsElengedEnabled = false;

                        this.KezetPopulal(this.jatekos, true);
                        this.KezetPopulal(this.ai0, this.menu_megnez.IsChecked);
                        this.KezetPopulal(this.ai1, this.menu_megnez.IsChecked);
                        this.KezetPopulal(this.ai2, this.menu_megnez.IsChecked);
                    }
                    else
                    {
                        this.jatekos.IsElengedEnabled = true;
                        this.ai0.IsElengedEnabled = true;
                        this.ai1.IsElengedEnabled = true;
                        this.ai2.IsElengedEnabled = true;
                    }

                    this.KorKezdes();
                }
            }
        }

        private void KorKezdes()
        {
            this.jatekos.IsEnabled = true;
            this.ai0.IsEnabled = true;
            this.ai1.IsEnabled = true;
            this.ai2.IsEnabled = true;
        }

        private void menu_mentes_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists("Save")) Directory.CreateDirectory("Save");

            var sfd = new SaveFileDialog
            {
                Filter = "XML|*.xml",
                Title = "Játékállás mentése",
                DefaultExt = ".xml",
                InitialDirectory = Environment.CurrentDirectory + "\\Save",
                FileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xml",
            };

            if (sfd.ShowDialog() == true)
            {
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                };

                var xml = XmlWriter.Create(sfd.FileName, settings);
                xml.WriteStartDocument();
                xml.WriteStartElement("Zsir");

                this.pakli.SaveToXML(ref xml);

                this.kijatszott.SaveToXML(ref xml);

                this.jatekos.SaveToXML(ref xml);
                this.ai0.SaveToXML(ref xml);
                this.ai1.SaveToXML(ref xml);
                this.ai2.SaveToXML(ref xml);

                xml.WriteEndElement();
                xml.WriteEndDocument();

                xml.Flush();
                xml.Close();
                xml.Dispose();
            }
        }

        private void menu_betoltes_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "XML|*.xml",
                Title = "Játékállás betöltése",
                DefaultExt = ".xml",
                InitialDirectory = Environment.CurrentDirectory + "\\Save",
            };

            if (ofd.ShowDialog() == true)
            {
                using (var stream = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
                {
                    var xml = new XmlDocument();
                    xml.Load(stream);

                    if (xml?.DocumentElement != null)
                    {
                        var pakliXML = xml.DocumentElement.SelectSingleNode(".//Pakli");
                        if (pakliXML != null) this.pakli.LoadFromXML(pakliXML);

                        var kijatszottXML = xml.DocumentElement.SelectSingleNode(".//JatszottLapok");
                        if (kijatszottXML != null) this.kijatszott.LoadFromXML(kijatszottXML);

                        var jatekosokXML = xml.DocumentElement.SelectNodes(".//Kez")?.Cast<XmlNode>();
                        if (jatekosokXML != null)
                        {
                            var jatekosXML = jatekosokXML.Where(x => x.Attributes["Neve"].Value == "jatekos").FirstOrDefault();
                            if (jatekosXML != null) this.jatekos.LoadFromXML(jatekosXML);

                            var ai0XML = jatekosokXML.Where(x => x.Attributes["Neve"].Value == "ai0").FirstOrDefault();
                            if (ai0XML != null) this.ai0.LoadFromXML(ai0XML);

                            var ai1XML = jatekosokXML.Where(x => x.Attributes["Neve"].Value == "ai1").FirstOrDefault();
                            if (ai1XML != null) this.ai1.LoadFromXML(ai1XML);

                            var ai2XML = jatekosokXML.Where(x => x.Attributes["Neve"].Value == "ai1").FirstOrDefault();
                            if (ai2XML != null) this.ai2.LoadFromXML(ai2XML);
                        }
                    }
                }
            }
        }

        private void menu_megnez_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                //var b = this.menu_megnez.IsChecked;
                //this.menu_megnez.IsChecked = !b;

                //this.jatekos.SetLathatosag(this.menu_megnez.IsChecked);
                this.ai0.SetLathatosag(this.menu_megnez.IsChecked);
                this.ai1.SetLathatosag(this.menu_megnez.IsChecked);
                this.ai2.SetLathatosag(this.menu_megnez.IsChecked);
            }
        }

        private void jatekos_TovabbAd(object sender, RoutedEventArgs e)
        {
            this.jatekos.IsEnabled = false;
            this.CanKorVege(true);
        }

        private void ai0_TovabbAd(object sender, RoutedEventArgs e)
        {
            this.jatekos.IsEnabled = false;
            this.CanKorVege(true);
        }



        private void ai1_TovabbAd(object sender, RoutedEventArgs e)
        {
            this.jatekos.IsEnabled = false;
            this.CanKorVege(true);
        }

        private void ai2_TovabbAd(object sender, RoutedEventArgs e)
        {
            this.jatekos.IsEnabled = false;
            this.CanKorVege(true);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using WpfApp1.UserControls;

namespace WpfApp1.Classes
{
    public interface IHeuriszticAIComponent
    {
        public static Kartya AICalculateCard(List<Kartya> kezben, Kartya utendo, List<Kartya> kijatszott)
        {
            Kartya ret = null;

            var hetesKezben = kezben.Where(x => x.Erteke == Ertek.VII);
            var tudUtni = kezben.Where(x => x.Erteke == utendo.Erteke);
            int zsirLapok = kezben.Where(x => x.Erteke == Ertek.X || x.Erteke == Ertek.Asz).Count();


            // Győzelmi faktorok:
            int megeriUtni = hetesKezben.Count() * 2;
            megeriUtni += tudUtni.Count() * 3;
            megeriUtni += kijatszott.Where(x => x.IsZsir).Count() * 10;

            // Rizikó faktorok:
            megeriUtni -= Pakli.ZsirLapokSzama;
            megeriUtni -= Pakli.HetesLapokSzama;

            // ha nagyobb, mint 0 megéri megverni ha nem, akkor beáldozza a "legértéktelenebb" lapját
            if (megeriUtni > 0)
            {
                if (tudUtni.Count() > 0) ret = tudUtni.FirstOrDefault();
                if (hetesKezben.Count() > 0) ret = hetesKezben.FirstOrDefault();
                else ret = kezben.Where(x => x.Erteke == kezben.Min(x => x.Erteke)).FirstOrDefault();
            }
            else
            {
                ret = kezben.Where(x => x.Erteke != Ertek.VII && !x.IsZsir && x.Erteke == kezben.Min(x => x.Erteke)).FirstOrDefault();
                if (ret == null) ret = kezben.Where(x => x.Erteke == kezben.Min(x => x.Erteke)).FirstOrDefault();
            }

            return ret;
        }
    }
}

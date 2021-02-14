using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RiproduttoreMusicale
{
    public class Brano
    {
        public string nome { get; set; }
        public string artista { get; set; }
        public string durata { get; set; }
        public int riproduzioni { get; set; }
        public string data_aggiunta { get; set; }
        public string path { get; set; }
        public byte[] img { get; set; }

        public Brano() { }

        public Brano(string n,string a,string d,int r,string data_add,string pth,byte[] ig)
        {
            nome = n;
            artista = a;
            durata = d;
            riproduzioni = r;
            data_aggiunta = data_add;
            path = pth;
            img = ig;
        }
        

        public override string ToString()
        {
            return nome + " " + artista + " " + durata + " " + riproduzioni+ " " + data_aggiunta + " " + path ;
        }
    }
}

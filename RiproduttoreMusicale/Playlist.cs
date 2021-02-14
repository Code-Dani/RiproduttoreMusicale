using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiproduttoreMusicale
{
    public class Playlist
    {
        public string nomePlaylist { get; set; }
        public List<Brano> braniPlaylist = new List<Brano>();
        public string pathImgAlbum { get; set; }
        public Playlist() { }


        public override string ToString()
        {
            return nomePlaylist + " " + pathImgAlbum;
        }

    }
}

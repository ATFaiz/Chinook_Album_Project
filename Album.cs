

using System;

namespace project{

    public class Album{
       
        public int AlbumId{get; set;}

        public string Title{get; set;}
        public int ArtistId {get; set;}
        public virtual Artist Artist{get; set;}

        public virtual Track Track{get; set;}

        public static implicit operator int(Album v)
        {
            throw new NotImplementedException();
        }
    }
}
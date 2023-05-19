
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using project;

namespace Project.Models{

        public class AlbumDetails{
        
         public int AlbumId{get; set;}
         [Required]
       public string Title{get; set;}
       public string ArtistName{get; set;}
       public string ArtistId{get; set;}
        public int TrackId{get; set;}
        public string TrackName {get; set;}
        public string MediaTypedName{get; set;}
        public int MTypeId{get; set;}
        public string GenreName{get; set;}
        public int GenreId{get; set;}
        public string Composer {get; set;}
        public int Milliseconds{get; set;}
        public int Bytes{get; set;}
        public decimal UnitPrice{get; set;}

        public List<AlbumDetailsViw> AlbumDetailsViwList {get; set;}

            }


}
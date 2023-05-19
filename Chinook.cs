using System;
using Microsoft.EntityFrameworkCore;


namespace project{

    public class Chinook : DbContext{


        public DbSet<Album> Albums {get; set;}
        public DbSet<Artist> Artists {get; set;}
        public DbSet<Track> Tracks {get; set;}
        public DbSet<Genre> Genres {get; set;}
        public DbSet<MediaType> Mediatypes {get; set;}



         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
          string path = System.IO.Path.Combine(System.Environment.CurrentDirectory, "chinook.db");
            optionsBuilder.UseSqlite($"Filename={path}; Foreign Keys=false");
            

        }

    

    }

}
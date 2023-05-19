using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Project.Models;
using project;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Below are the ActionResults that i used
        // ActionResult specifies how the server should respond to the request

        //ActionResult for Index
        [HttpGet]
        public IActionResult Index(string searchBy, string Search)
        {
            //Chinook Database
            Chinook db = new Chinook();

            //Search Album by Title
            if (searchBy == "Name")
            {
                List<HomeIndexViewModel> alb = db.Albums.Join(db.Artists, a => a.ArtistId,
                 art => art.ArtistId, (a, art) => new HomeIndexViewModel()
                 {
                     AlbumId = a.AlbumId,
                     Title = a.Title,
                     ArtistName = art.Name

                 }).Where(model => model.Title.StartsWith(Search) || Search == null).ToList();


                return View(alb);
            }

            // Search Album by Artist Name
            else if (searchBy == "Artist")
            {
                List<HomeIndexViewModel> alb = db.Albums.Join(db.Artists, a => a.ArtistId,
                 art => art.ArtistId, (a, art) => new HomeIndexViewModel()
                 {
                     AlbumId = a.AlbumId,
                     Title = a.Title,
                     ArtistName = art.Name

                 }).Where(model => model.ArtistName.StartsWith(Search) || Search == null).ToList();


                return View(alb);

            }
            // Display Albums
            else
            {
                List<HomeIndexViewModel> alb = db.Albums.Join(db.Artists, a => a.ArtistId,
                 art => art.ArtistId, (a, art) => new HomeIndexViewModel()
                 {
                     AlbumId = a.AlbumId,
                     Title = a.Title,
                     ArtistName = art.Name

                 }).ToList();

                return View(alb);

            }

        }

        //ActionResult for Album Details
        [HttpGet]
        public IActionResult Details(int id)
        {
            //Chinook Database
            Chinook db = new Chinook();

            //Join 3 tables and inherite an other 2 tabls
            List<AlbumDetails> AlbumInfo = (from alb in db.Albums
                                            join trc in db.Tracks
                                            on alb.AlbumId equals trc.AlbumId
                                            join mt in db.Mediatypes
                                            on trc.MTypeId equals mt.MediaTypeId
                                            select new AlbumDetails
                                            {
                                                AlbumId = alb.AlbumId,
                                                Title = alb.Title,
                                                ArtistName = alb.Artist.Name,
                                                TrackId = trc.TrackId,
                                                TrackName = trc.Name,
                                                MediaTypedName = mt.Name,
                                                GenreName = trc.Genre.Name,
                                                Composer = trc.Composer,
                                                Milliseconds = trc.Milliseconds,
                                                Bytes = trc.Bytes,
                                                UnitPrice = trc.UnitPrice

                                            }).Where(model => model.AlbumId == id).ToList();
            return View(AlbumInfo);


        }

        //ActionResult for Create 
        [HttpGet]
        public IActionResult Create()
        {
            //Chinook Database
            Chinook db = new Chinook();

            //MediaType dropdownList
            List<MediaType> MediaTypeList = db.Mediatypes.ToList();
            ViewBag.MediaTypeList = MediaTypeList;

            //Genre dropdownList
            List<Genre> GenreList = db.Genres.ToList();
            ViewBag.GenreList = GenreList;

            return View();
        }

        //ActionResult for Create - HttpPost
        [HttpPost]
        public IActionResult Create(AlbumDetails model)
        {

            //Chinook Database
            Chinook db = new Chinook();

            //Add data into artist table
            Artist art = new Artist();
            art.Name = model.ArtistName;
            db.Artists.Add(art);
            db.SaveChanges();

            int latestArtistId = art.ArtistId;

            //Add data into Album table
            var alb = new Album();
            alb.Title = model.Title;
            alb.ArtistId = latestArtistId;
            db.Albums.Add(alb);
            db.SaveChanges();

            int latestAlbumId = alb.AlbumId;

            // Add data into Track table
            for (int i = 0; i < model.AlbumDetailsViwList.Count; i++)
            {
                Chinook dbTrack = new Chinook();
                try
                {
                    Track trc = new Track();
                    trc.AlbumId = latestAlbumId;
                    trc.Name = model.AlbumDetailsViwList[i].TrackName;
                    trc.MTypeId = model.AlbumDetailsViwList[i].MTypeId;
                    trc.GenreId = model.AlbumDetailsViwList[i].GenreId;
                    trc.Composer = model.AlbumDetailsViwList[i].Composer;
                    trc.Milliseconds = model.AlbumDetailsViwList[i].Milliseconds;
                    trc.Bytes = model.AlbumDetailsViwList[i].Bytes;
                    trc.UnitPrice = model.AlbumDetailsViwList[i].UnitPrice;
                    trc.TrackId = model.AlbumDetailsViwList[i].TrackId;

                    dbTrack.Tracks.Add(trc);
                    dbTrack.SaveChanges();
                }
                finally
                {
                    dbTrack.Dispose();
                }
            }

            //Insert Message
            if (latestAlbumId > 0)
            {
                TempData["InsertMessage"] = "An album has been created";

                //redirect to Index page
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");

        }

        //ActionResult for Edit - HttpGet
        [HttpGet]
        public IActionResult Edit(int id)
        {
            //Chinook Database
            Chinook db = new Chinook();

            //Artist dropdown list
            List<Artist> ArtistList = db.Artists.ToList();
            ViewBag.ArtistList = ArtistList;

            //MediaType dropdown list
            List<MediaType> MediaTypeList = db.Mediatypes.ToList();
            ViewBag.MediaTypeList = MediaTypeList;

            //Genre dropdownList
            List<Genre> GenreList = db.Genres.ToList();
            ViewBag.GenreList = GenreList;

            //display Album Details 
            AlbumDetails model = new AlbumDetails();
            List<AlbumDetailsViw> AlbumInfo = (from alb in db.Albums
                                               join trc in db.Tracks
                                               on alb.AlbumId equals trc.AlbumId
                                               join mt in db.Mediatypes
                                               on trc.MTypeId equals mt.MediaTypeId
                                               select new AlbumDetailsViw
                                               {
                                                   AlbumId = alb.AlbumId,
                                                   Title = alb.Title,
                                                   ArtistName = alb.Artist.Name,
                                                   ArtistId = alb.ArtistId,
                                                   TrackId = trc.TrackId,
                                                   TrackName = trc.Name,
                                                   MediaTypedName = mt.Name,
                                                   MTypeId = mt.MediaTypeId,
                                                   GenreName = trc.Genre.Name,
                                                   GenreId = trc.GenreId,
                                                   Composer = trc.Composer,
                                                   Milliseconds = trc.Milliseconds,
                                                   Bytes = trc.Bytes,
                                                   UnitPrice = trc.UnitPrice


                                               }).Where(model => model.AlbumId == id).ToList();

            model.AlbumDetailsViwList = AlbumInfo;
            return View(model);
        }

        //ActionResult for Edit - HttpPost
        [HttpPost]
        public IActionResult Update(AlbumDetails model)

        {
            for (int i = 0; i < model.AlbumDetailsViwList.Count; i++)
            {
                //Chinook Database
                Chinook db = new Chinook();

                //Try and finally method has been used to handle the exception
                try
                {

                    //save changes in Album table
                    var alb = new Album();
                    alb.Title = model.AlbumDetailsViwList[0].Title;
                    alb.ArtistId = model.AlbumDetailsViwList[i].ArtistId;
                    alb.AlbumId = model.AlbumDetailsViwList[i].AlbumId;
                    db.Albums.Update(alb);
                    db.SaveChanges();


                    //save changes in Track table
                    Track trc = new Track();
                    trc.AlbumId = model.AlbumDetailsViwList[i].AlbumId;
                    trc.Name = model.AlbumDetailsViwList[i].TrackName;
                    trc.MTypeId = model.AlbumDetailsViwList[i].MTypeId;
                    trc.GenreId = model.AlbumDetailsViwList[i].GenreId;
                    trc.Composer = model.AlbumDetailsViwList[i].Composer;
                    trc.Milliseconds = model.AlbumDetailsViwList[i].Milliseconds;
                    trc.Bytes = model.AlbumDetailsViwList[i].Bytes;
                    trc.UnitPrice = model.AlbumDetailsViwList[i].UnitPrice;
                    trc.TrackId = model.AlbumDetailsViwList[i].TrackId;
                    db.Tracks.Update(trc);
                    db.SaveChanges();
                }
                finally
                {
                    db.Dispose();
                }


            }

            TempData["EditMessage"] = "The Album data has been edited";

            return RedirectToAction("Index");
        }

        //ActionResult for Delete - HttpGet
        [HttpGet]
        public IActionResult Delete(int id)
        {

            //Chinook Database
            Chinook db = new Chinook();

            AlbumDetails model = new AlbumDetails();

            //join tables and display album details
            List<AlbumDetailsViw> AlbumInfo = (from alb in db.Albums
                                               join trc in db.Tracks
                                               on alb.AlbumId equals trc.AlbumId
                                               join mt in db.Mediatypes
                                               on trc.MTypeId equals mt.MediaTypeId
                                               select new AlbumDetailsViw
                                               {
                                                   AlbumId = alb.AlbumId,
                                                   Title = alb.Title,
                                                   ArtistName = alb.Artist.Name,
                                                   ArtistId = alb.ArtistId,
                                                   TrackId = trc.TrackId,
                                                   TrackName = trc.Name,
                                                   MediaTypedName = mt.Name,
                                                   MTypeId = mt.MediaTypeId,
                                                   GenreName = trc.Genre.Name,
                                                   GenreId = trc.GenreId,
                                                   Composer = trc.Composer,
                                                   Milliseconds = trc.Milliseconds,
                                                   Bytes = trc.Bytes,
                                                   UnitPrice = trc.UnitPrice


                                               }).Where(model => model.AlbumId == id).ToList();

            model.AlbumDetailsViwList = AlbumInfo;
            return View(model);
        }

        //ActionResult for Delete - HttpPost
        [HttpPost]
        public IActionResult Delete(AlbumDetails model)
        {

            //used for loop to get all tracks
            for (int i = 0; i < model.AlbumDetailsViwList.Count; i++)
            {
                //Chinook Database
                Chinook db = new Chinook();
                try
                {
                    //first delete tracks
                    Track trc = new Track();
                    trc.TrackId = model.AlbumDetailsViwList[i].TrackId;
                    db.Tracks.Attach(trc);
                    db.Tracks.Remove(trc);
                    db.SaveChanges();

                }
                finally
                {
                    db.Dispose();
                }


            }

            //Chinook Database
            Chinook dbAlbum = new Chinook();
            var alb = new Album();

            //second delete Album
            alb.Title = model.AlbumDetailsViwList[0].Title;
            alb.ArtistId = model.AlbumDetailsViwList[0].ArtistId;
            alb.AlbumId = model.AlbumDetailsViwList[0].AlbumId;
            dbAlbum.Albums.Remove(alb);
            dbAlbum.SaveChanges();

            //Delete Message
            TempData["DeleteMessage"]="Tle album has been deleted";
            //redirect to Index page
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

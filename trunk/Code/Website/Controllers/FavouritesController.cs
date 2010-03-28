using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Security;
using Spoffice.Website.Models;
using Spoffice.Website.Models.Spotify.MetadataApi;

namespace Spoffice.Website.Controllers
{
    [Authorize]
    public class FavouritesController : AuthorizedController
    {
        public FavouritesController()
        {
        }
        public ActionResult Index()
        {
            return MultiformatView(typeof(FavouritesList), new FavouritesList { Favourites = DataContext.FavouriteRepository.GetUsersFavourites(UserGuid) });
        }
        public ActionResult Add(string id)
        {
            ModelState.Clear();
            Status status = DataContext.FavouriteRepository.AddToFavourites(id, UserGuid);
            return MultiformatView(typeof(Status), status, RedirectToAction("Search", "Music"));
        }
        public ActionResult Remove(string id)
        {
            ModelState.Clear();
            Status status = DataContext.FavouriteRepository.RemoveFromFavourites(id, UserGuid);
            string referrer = Request.UrlReferrer.AbsolutePath.ToString();
            return MultiformatView(typeof(Status), status, RedirectToAction("Search", "Music"));
        }
    }
 
}

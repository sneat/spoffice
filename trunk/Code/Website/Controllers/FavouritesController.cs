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
            ViewData["favourites"] = DataContext.FavouriteRepository.GetUsersFavourites(UserGuid);
        }
        public ActionResult Index()
        {
            return View(ViewData["favourites"] as List<Favourite>);
        }
        public ActionResult Add(string id)
        {
            ModelState.Clear();
            Status status = DataContext.FavouriteRepository.AddToFavourites(id, UserGuid);
            string referrer = Request.UrlReferrer.AbsolutePath.ToString();
            return MultiformatView(typeof(Status), status, referrer);
        }
        public ActionResult Remove(string id)
        {
            ModelState.Clear();
            Status status = DataContext.FavouriteRepository.RemoveFromFavourites(id, UserGuid);
            string referrer = Request.UrlReferrer.AbsolutePath.ToString();
            return MultiformatView(typeof(Status), status, referrer);
        } 
       
    }
 
}

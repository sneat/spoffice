using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Security;
using Spoffice.Website.Models;
using Spoffice.Website.Models.Output;

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
            return MultiformatView(typeof(FavouritesListOutput), new FavouritesListOutput { Favourites = DataContext.FavouriteRepository.GetUsersFavourites(UserGuid) });
        }
        public ActionResult Add(string id)
        {
            ModelState.Clear();
            StatusOutput status = DataContext.FavouriteRepository.AddToFavourites(id, UserGuid);
            return MultiformatView(typeof(StatusOutput), status, RedirectToAction("Search", "Music"));
        }
        public ActionResult Remove(string id)
        {
            ModelState.Clear();
            StatusOutput status = DataContext.FavouriteRepository.RemoveFromFavourites(id, UserGuid);
            string referrer = Request.UrlReferrer.AbsolutePath.ToString();
            return MultiformatView(typeof(StatusOutput), status, RedirectToAction("Search", "Music"));
        }
    }
 
}

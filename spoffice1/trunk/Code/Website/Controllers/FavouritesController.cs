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
            return MultiformatView(typeof(FavouritesListOutput), new FavouritesListOutput(DataContext.FavouriteRepository.GetUsersFavourites(UserGuid)));
        }
        public ActionResult Add(string id)
        {
            return MultiformatView(typeof(StatusOutput), DataContext.FavouriteRepository.AddToFavourites(new Guid(id), UserGuid));
        }
        public ActionResult Remove(string id)
        {
            return MultiformatView(typeof(StatusOutput), DataContext.FavouriteRepository.RemoveFromFavourites(new Guid(id), UserGuid));
        }
    }
 
}

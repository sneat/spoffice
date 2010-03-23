using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spoffice.Website.Models
{
    public static class DataContext
    {
        private static spofficeEntities _context;
        public static spofficeEntities Context
        {
            get
            {
                return _context ?? (_context = new spofficeEntities());
            }
        }
        private static IRatingRepository _ratingRepository;
        public static IRatingRepository RatingRepository
        {
            get
            {
                return _ratingRepository ?? (_ratingRepository = new RatingRepository());
            }
        }
        private static ITrackRepository _trackRepository;
        public static ITrackRepository TrackRepository
        {
            get
            {
                return _trackRepository ?? (_trackRepository = new TrackRepository());
            }
        }
        private static IFavouriteRepository _favouriteRepository;
        public static IFavouriteRepository FavouriteRepository
        {
            get
            {
                return _favouriteRepository ?? (_favouriteRepository = new FavouriteRepository());
            }
        }
    }
}

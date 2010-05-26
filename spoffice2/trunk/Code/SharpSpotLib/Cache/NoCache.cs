using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Cache
{
    public class NoCache : ICache
    {
        #region fields

        #endregion


        #region methods

        #endregion


        #region Cache members

        public void Clear()
        {
        }

        public void Clear(String category)
        {
        }

        public Boolean Contains(String category, String hash)
        {
            return false;
        }

        public Byte[] Load(String category, String hash)
        {
            return null;
        }

        public void Remove(String category, String hash)
        {
        }

        public void Store(String category, String hash, Byte[] data)
        {
        }

        public void Store(String category, String hash, Byte[] data, Int32 size)
        {
        }

        public String[] List(String category)
        {
            return new String[0];
        }

        #endregion


        #region construction

        public NoCache()
        {

        }

        #endregion
    }
}

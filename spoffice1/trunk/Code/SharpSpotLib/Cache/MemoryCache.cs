using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Cache
{
    public class MemoryCache : ICache
    {
        #region fields

        private Dictionary<String, Dictionary<String, Byte[]>> _data = new Dictionary<String, Dictionary<String, Byte[]>>();

        #endregion


        #region methods

        #endregion


        #region Cache members

        public void Clear()
        {
            _data.Clear();
        }

        public void Clear(String category)
        {
            _data[category].Clear();
        }

        public Boolean Contains(String category, String hash)
        {
            if (!_data.ContainsKey(category))
                return false;
            return _data[category].ContainsKey(hash);
        }

        public Byte[] Load(String category, String hash)
        {
            try
            {
                return _data[category][hash];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Remove(String category, String hash)
        {
            try
            {
                _data[category].Remove(hash);
            }
            catch (Exception)
            {
                throw; //FIXME
            }
        }

        public void Store(String category, String hash, Byte[] data)
        {
            Store(category, hash, data, data.Length);
        }

        public void Store(String category, String hash, Byte[] data, Int32 size)
        {
            try
            {
                Byte[] buffer;
                if (data.Length == size)
                    buffer = data;
                else
                {
                    buffer = new Byte[size];
                    Array.Copy(data, buffer, size);
                }

                if (!_data.ContainsKey(category))
                    _data.Add(category, new Dictionary<String, Byte[]>());

                _data[category].Add(hash, buffer);
            }
            catch (Exception)
            {
                //Ignore errors
            }
        }

        public String[] List(String category)
        {
            try
            {
                return _data[category].Keys.ToArray();
            }
            catch (Exception)
            {
                throw; //FIXME
            }
        }

        #endregion


        #region construction

        public MemoryCache()
        {

        }

        #endregion
    }
}

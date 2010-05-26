using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Cache
{
    //FIXME: Translate java comments to XML comments

    public interface ICache
    {
        /**
	     * Clear the entire cache.
	     */
        void Clear();

        /**
         * Clear the cache for the specified category.
         * 
         * @param category A cache category to clear.
         */
        void Clear(String category);

        /**
         * Check if the cache contains an item. 
         * 
         * @param category The cache category to check.
         * @param hash     The hash of the item to check.
         * 
         * @return {@code true} if it contains that item, {@code false} otherwise.
         */
        Boolean Contains(String category, String hash);

        /**
         * Load data from the cache.
         * 
         * @param category The cache category to load from.
         * @param hash     The hash of the item to load.
         * 
         * @return Cached data or {@code null}.
         */
        Byte[] Load(String category, String hash);

        /**
         * Remove a single item from the cache.
         * 
         * @param category The cache category to remove from.
         * @param hash     The hash of the item to remove.
         */
        void Remove(String category, String hash);

        /**
         * Store data in the cache.
         * 
         * @param category The cache category to store to.
         * @param hash     The hash of the item to store.
         * @param data     The data to store.
         */
        void Store(String category, String hash, Byte[] data);

        /**
         * Store data in the cache.
         * 
         * @param category The cache category to store to.
         * @param hash     The hash of the item to store.
         * @param data     The data to store.
         * @param size     The size of the data.
         */
        void Store(String category, String hash, Byte[] data, Int32 size);

        /**
         * List data in a cache category.
         * 
         * @param category The cache category to list.
         * 
         * @return A {@link List} of cache hashes.
         */
        String[] List(String category);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpSpotLib.Exceptions;
using SharpSpotLib.Protocol;
using SharpSpotLib.Cache;
using SharpSpotLib.Media;
using SharpSpotLib.Media.Parser;
using SharpSpotLib.Util;
using SharpSpotLib.Enums;
using SharpSpotLib.Crypto;
using SharpSpotLib.Protocol.Channel;
using System.IO;

namespace SharpSpotLib
{
    public class SharpSpotConnection : ICommandListener
    {
        #region fields


        private Session _session = new Session();
        private Protocol.Protocol _protocol = null;
        private User _user = null;
        private Semaphore _userSemaphore = new Semaphore(0, 2);
        private Cache.ICache _cache;
        private TimeSpan _timeout;
        private ChannelStreamer _streamer = null;

        #endregion


        #region properties

        public TimeSpan Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        #endregion


        #region methods


        /**
	     * Login to Spotify using the specified username and password.
	     * 
	     * @param username Username to use.
	     * @param password Corresponding password.
	     * 
	     * @throws ConnectionException
	     * @throws AuthenticationException
	     */
        public void Login(String username, String password)
        {
            if (this._protocol != null)
                throw new InvalidOperationException("Already logged in!");


            /* Authenticate session and get protocol. */
            this._protocol = this._session.Authenticate(username, password);

            /* Create user object. */
            this._user = new User(username);

            /* Add command handler. */
            this._protocol.AddListener(this); //FIXME
        }

        /**
         * Closes the connection to a Spotify server.
         * 
         * @throws ConnectionException
         */
        public void Close()
        {
		    /* This will make receivePacket return immediately. */
            if (this._protocol != null)
            {
                this._protocol.Disconnect();
                /* Reset protocol to 'null'. */
                this._protocol = null;
            }

            if (_userSemaphore is IDisposable)
            {
                (_userSemaphore as IDisposable).Dispose();
            }
	    }

        /**
	     * Continuously receives packets in order to handle them.
	     * Use a {@link Thread} to run this.
	     */
        public void Run()
        {
            /* Check if we're logged in. */
            if (this._protocol == null)
            {
                return;
                //throw new InvalidOperationException("You need to login first!");
            }

            /* Continuously receive packets until connection is closed. */
            try
            {
                while (this._protocol != null)
                {
                    this._protocol.ReceivePacket();
                }
            }
            catch (ProtocolException)
            {
                /* Connection was closed. */
            }
        }

        /**
	     * Get user info.
	     * 
	     * @return A {@link User} object.
	     * 
	     * @see User
	     */
        public User User()
        {
            /* Wait for data to become available (country, prodinfo). */
            try
            {
                if (!this._userSemaphore.TryAcquire(2, this.Timeout))
                    throw new TimeoutException("Timeout while waiting for user data.");

                /* Release so this can be called again. */
                this._userSemaphore.Release(2);
            }
            catch (TimeoutException e)
            {
                throw new Exception("Timeout", e);
            }
            catch (Exception)
            {
                throw;
            }

            return this._user;
        }


        /**
	     * Fetch a toplist.
	     * 
	     * @param type     A toplist type. e.g. "artist", "album" or "track".
	     * @param region   A region code or null. e.g. "SE" or "DE".
	     * @param username A username or null.
	     * 
	     * @return A {@link Result} object.
	     * 
	     * @see Result
	     */
        public Result Toplist(ToplistType type, String region, String username)
        {
            /* Create channel callback and parameter map. */
		    ChannelCallback callback   = new ChannelCallback();
		    Dictionary<String, String> parameters = new Dictionary<String, String>();

            //"artist", "album" or "track"
            String typeString = EnumUtil.GetName(typeof(ToplistType), type).ToLower(); 

		    /* Add parameters. */
            parameters.Add("type", typeString);
		    parameters.Add("region", region);
		    parameters.Add("username", username);
    		
		    /* Send toplist request. */
		    try
            {
                this._protocol.SendToplistRequest(callback, parameters);
		    }
		    catch(ProtocolException)
            {
			    return null;
		    }
    		
		    /* Get data. */
		    Byte[] data = callback.Get(this.Timeout);
    		
		    /* Create result from XML. */
		    return XMLMediaParser.ParseResult(data, Encoding.UTF8);
        }


        /**
	     * Search for an artist, album or track.
	     * 
	     * @param query Your search query.
	     * 
	     * @return A {@link Result} object.
	     * 
	     * @see Result
	     */
        public Result Search(String query)
        {
            /* Create channel callback. */
            ChannelCallback callback = new ChannelCallback();

            /* Send search query. */
            try
            {
                this._protocol.SendSearchQuery(callback, query);
            }
            catch (ProtocolException)
            {
                return null;
            }

            /* Get data. */
            byte[] data = callback.Get(this.Timeout);

            /* Create result from XML. */
            Result result = XMLMediaParser.ParseResult(data, Encoding.UTF8);

            result.Query = query;

            return result;
        }

        /**
	     * Get an image (e.g. artist portrait or cover) by requesting
	     * it from the server or loading it from the local cache, if
	     * available.
	     * 
	     * @param id Id of the image to get.
	     * 
	     * @return An {@link Image} or null if the request failed.
	     * 
	     * @see Image
	     */
        public System.Drawing.Image Image(String id)
        {
            
            /* Data buffer. */
            Byte[] data;

            /* Check cache. */
            if (this._cache != null && this._cache.Contains("image", id))
            {
                data = this._cache.Load("image", id);
            }
            else
            {
                /* Create channel callback. */
                ChannelCallback callback = new ChannelCallback();

                /* Send image request. */
                try
                {
                    this._protocol.SendImageRequest(callback, id);
                }
                catch (ProtocolException)
                {
                    return null;
                }

                /* Get data. */
                data = callback.Get(this.Timeout);

                /* Save to cache. */
                if (this._cache != null)
                {
                    this._cache.Store("image", id, data);
                }
            }

            /* Create Image. */
            try
            {
                //return System.Drawing.Image.FromStream(new MemoryStream(data));
                return new System.Drawing.Bitmap(new MemoryStream(data));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /**
	     * Browse artist, album or track info.
	     * 
	     * @param type Type of media to browse for.
	     * @param id   Id of media to browse.
	     * 
	     * @return An {@link XMLElement} object holding the data or null
	     *         on failure.
	     * 
	     * @see BrowseType
	     */
        private Object Browse(BrowseType type, String id)
        {
            /* Create channel callback. */
            ChannelCallback callback = new ChannelCallback();

            /* Send browse request. */
            try
            {
                this._protocol.SendBrowseRequest(callback, (Int32)type, id);
            }
            catch (ProtocolException)
            {
                return null;
            }

            /* Get data. */
            byte[] data = callback.Get(this.Timeout);

            /* Create object from XML. */
            return XMLMediaParser.Parse(data, Encoding.UTF8);
        }


        /**
	     * Browse artist info by id.
	     * 
	     * @param id An id identifying the artist to browse.
	     * 
	     * @retrun An {@link Artist} object holding more information about
	     *         the artist or null on failure.
	     * 
	     * @see Artist
	     */
        public Artist BrowseArtist(String id)
        {
            /* Browse. */
            return this.Browse(BrowseType.ARTIST, id) as Artist;
        }

        /**
	     * Browse artist info.
	     * 
	     * @param artist An {@link Artist} object identifying the artist to browse.
	     * 
	     * @retrun A new {@link Artist} object holding more information about
	     *         the artist or null on failure.
	     * 
	     * @see Artist
	     */
        public Artist Browse(Artist artist)
        {
            return this.BrowseArtist(artist.Id);
        }

        /**
	     * Browse album info by id.
	     * 
	     * @param id An id identifying the album to browse.
	     * 
	     * @retrun An {@link Album} object holding more information about
	     *         the album or null on failure.
	     * 
	     * @see Album
	     */
        public Album BrowseAlbum(String id)
        {
            return this.Browse(BrowseType.ALBUM, id) as Album;
        }

        /**
	     * Browse album info.
	     * 
	     * @param album An {@link Album} object identifying the album to browse.
	     * 
	     * @retrun A new {@link Album} object holding more information about
	     *         the album or null on failure.
	     * 
	     * @see Album
	     */
        public Album Browse(Album album)
        {
            return this.BrowseAlbum(album.Id);
        }

        /**
	     * Browse track info by id.
	     * 
	     * @param id An id identifying the track to browse.
	     * 
	     * @retrun A {@link Result} object holding more information about
	     *         the track or null on failure.
	     * 
	     * @see Track
	     * @see Result
	     */
        public Result BrowseTrack(String id)
        {
            return this.Browse(BrowseType.TRACK, id) as Result;
        }

        /**
	     * Browse track info.
	     * 
	     * @param album A {@link Track} object identifying the track to browse.
	     * 
	     * @retrun A {@link Result} object holding more information about
	     *         the track or null on failure.
	     * 
	     * @see Track
	     * @see Result
	     */
        public Result Browse(Track track)
        {
            return this.BrowseTrack(track.Id);
        }

        /**
	     * Browse information for multiple tracks by id.
	     * 
	     * @param tracks A {@link List} of ids identifying the tracks to browse.
	     * 
	     * @retrun A {@link Result} object holding more information about
	     *         the tracks or null on failure.
	     * 
	     * @see Track
	     * @see Result
	     */
        public Result BrowseTracks(List<String> ids)
        {
            /* Data buffer. */
		    byte[] data;
    		
		    /* Create cache hash. */
            String hash = String.Join("", ids.ToArray());
    		
		    hash = Hex.ToHex(Hash.Sha1(Hex.ToBytes(hash)));
    		
		    /* Check cache. */
		    if(this._cache != null && this._cache.Contains("browse", hash))
            {
			    data = this._cache.Load("browse", hash);
		    }
		    else{
			    /* Create channel callback */
			    ChannelCallback callback = new ChannelCallback();
    			
			    /* Send browse request. */
			    try
                {
				    this._protocol.SendBrowseRequest(callback, (Int32)BrowseType.TRACK, ids);
			    }
			    catch(ProtocolException)
                {
				    return null;
			    }
    			
			    /* Get data. */
			    data = callback.Get(this.Timeout);
    			
			    /* Save to cache. */
			    if(this._cache != null)
                {
				    this._cache.Store("browse", hash, data);
			    }
		    }
    		
		    /* Create result from XML. */
		    return XMLMediaParser.ParseResult(data, Encoding.UTF8);
        }

        /**
	     * Browse information for multiple tracks.
	     * 
	     * @param tracks A {@link List} of {@link Track} objects identifying
	     *               the tracks to browse.
	     * 
	     * @retrun A {@link Result} object holding more information about
	     *         the tracks or null on failure.
	     * 
	     * @see Track
	     * @see Result
	     */
        public Result Browse(List<Track> tracks)
        {
            /* Create id list. */
		    List<String> ids = new List<String>();
    		
		    foreach (Track track in tracks)
            {
			    ids.Add(track.Id);
		    }
    		
		    return this.BrowseTracks(ids);
        }

        /**
	     * Get a list of stored playlists.
	     * 
	     * @return A {@link List} of {@link Playlist} objects or null on failure.
	     *         (Note: {@link Playlist} objects only hold id and author)
	     * 
	     * @see Playlist
	     */
        public PlaylistContainer Playlists()
        {
            /* Create channel callback. */
            ChannelCallback callback = new ChannelCallback();

            /* Send stored playlists request. */
            try
            {
                this._protocol.SendUserPlaylistsRequest(callback);
            }
            catch (ProtocolException)
            {
                return PlaylistContainer.EMPTY;
            }

            /* Get data and inflate it. */
            try
            {
                byte[] data = callback.Get(this.Timeout);

                /* Load XML. */
                XMLElement playlistElement = XML.Load(
                    "<?xml version=\"1.0\" encoding=\"utf-8\" ?><playlist>" +
                    Encoding.UTF8.GetString(data, 0, data.Length) + "</playlist>");

                /* Create an return list. */
                return PlaylistContainer.FromXMLElement(playlistElement);
            }
            catch (Exception)
            {
                return PlaylistContainer.EMPTY;
            }
        }

        /**
	     * Add a playlist to the end of the list of stored playlists.
	     * 
	     * @param playlists A {@link PlaylistContainer} to add the playlist to.
	     * @param playlist  The {@link Playlist} to be added.
	     * 
	     * @return true on success and false on failure.
	     * 
	     * @see PlaylistContainer
	     */
        public Boolean PlaylistsAddPlaylist(PlaylistContainer playlists, Playlist playlist)
        {
            return this.PlaylistsAddPlaylist(playlists, playlist, playlists.Playlists.Count);
        }

        /**
         * Add a playlist to the list of stored playlists.
         * 
         * @param playlists A {@link PlaylistContainer} to add the playlist to.
         * @param playlist  The {@link Playlist} to be added.
         * @param position  The target position of the playlist.
         * 
         * @return true on success and false on failure.
         * 
         * @see PlaylistContainer
         */
        public Boolean PlaylistsAddPlaylist(PlaylistContainer playlists, Playlist playlist, int position)
        {
            String user = this._session.Username;
		
		    /* First add the playlist to calculate the new checksum. */
		    playlists.Playlists.Insert(position, playlist);
    		
            String xml = String.Format(
			    "<change><ops><add><i>{0}</i><items>{1}02</items></add></ops>" +
			    "<time>{2}</time><user>{3}</user></change>" +
			    "<version>{4},{5},{6},0</version>",
			    position, playlist.Id, Time.GetUnixTimestamp(), user,
			    PadLeft(playlists.Revision + 1, '0', 10), 
                PadLeft(playlists.Playlists.Count, '0', 10),
			    PadLeft(playlists.GetChecksum(), '0', 10)
		    );
    		
		    /* Remove the playlist again, because we need the old checksum for sending. */
		    playlists.Playlists.RemoveAt(position);
    		
		    /* Create channel callback */
		    ChannelCallback callback = new ChannelCallback();
    		
		    /* Send change playlists request. */
		    try
            {
			    this._protocol.SendChangeUserPlaylists(callback, playlists, xml);
		    }
		    catch (ProtocolException)
            {
			    return false;
		    }
    		
		    /* Get response. */
		    byte[] data = callback.Get(this.Timeout);
    		
		    XMLElement playlistElement = XML.Load(
			    "<?xml version=\"1.0\" encoding=\"utf-8\" ?><playlist>" +
                Encoding.UTF8.GetString(data, 0, data.Length) + "</playlist>");		
    		
		    /* Check for success. */
		    if (playlistElement.HasChild("confirm")){
			    /* Split version string into parts. */
			    String[] parts = playlistElement.GetChild("confirm").GetChildText("version").Split(new Char[] { ',' });
    			
			    /* Set values. */
			    playlists.Revision = Int64.Parse(parts[0]);
    			
			    /* Add the track, since operation was successful. */
			    playlists.Playlists.Insert(position, playlist);
    			
			    if (playlists.GetChecksum() != Int64.Parse(parts[2]))
                {
				    //System.out.println("Checksum error!");
                    
                    //FIXME: Maybe shouldn't halt here
                    throw new Exception("Checksum error!");
			    }
    			
			    return true;
		    }
    		
		    return false;
        }

        //FIXME
        // TODO: playlistsAddPlaylists, playlistsRemovePlaylist(s), playlistsMovePlaylist(s)
        // TODO: destroy playlist: http://despotify.pastebin.com/f7cb09c84


        /**
	     * Get a playlist.
	     * 
	     * @param id     Id of the playlist to load.
	     * @param cached Whether to use a cached version if available or not.
	     * 
	     * @return A {@link Playlist} object or null on failure.
	     * 
	     * @see Playlist
	     */
        public Playlist Playlist(String id, Boolean cached)
        {
            /* Data buffer. */
            byte[] data;

            if (cached && this._cache != null && this._cache.Contains("playlist", id))
            {
                data = this._cache.Load("playlist", id);
            }
            else
            {
                /* Create channel callback */
                ChannelCallback callback = new ChannelCallback();

                /* Send playlist request. */
                try
                {
                    this._protocol.SendPlaylistRequest(callback, id);
                }
                catch (ProtocolException)
                {
                    return null;
                }

                /* Get data and inflate it. */
                data = callback.Get(this.Timeout);

                /* Save data to cache. */
                if (this._cache != null)
                {
                    this._cache.Store("playlist", id, data);
                }
            }

            /* Load XML. */
            XMLElement playlistElement = XML.Load(
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?><playlist>" +
                Encoding.UTF8.GetString(data, 0, data.Length) + "</playlist>");

            /* Create and return playlist. */
            return Media.Playlist.FromXMLElement(playlistElement, id);
        }

        /**
	     * Get a playlist.
	     * 
	     * @param id Id of the playlist to load.
	     * 
	     * @return A {@link Playlist} object or null on failure.
	     * 
	     * @see Playlist
	     */
        public Playlist Playlist(String id)
        {
            return this.Playlist(id, false);
        }

        /**
         * Create a playlist.
         * 
         * @param name          The name of the playlist to create.
         * @param collaborative If the playlist shall be collaborative.
         * 
         * @return A {@link Playlist} object or null on failure.
         * 
         * @see Playlist
         */
        public Playlist PlaylistCreate(String name, Boolean collaborative)
        {
            String id = Hex.ToHex(RandomBytes.GetRandomBytes(16));
		    String user = this._session.Username;
		    Playlist playlist = new Playlist(id, name, user, collaborative);
    		
		    String xml = String.Format(
			    "<id-is-unique/><change><ops><create/><name>{0}</name></ops>" +
			    "<time>{1}</time><user>{2}</user></change>" +
			    "<version>0000000001,0000000000,0000000001,{3}</version>",
			    playlist.Name, Time.GetUnixTimestamp(),
			    playlist.Author, playlist.Collaborative ? 1 : 0);
    		
		    /* Create channel callback */
		    ChannelCallback callback = new ChannelCallback();
    		
		    /* Send change playlist request. */
		    try
            {
			    this._protocol.SendCreatePlaylist(callback, playlist, xml);
		    }
		    catch (ProtocolException)
            {
			    return null;
		    }
    		
		    /* Get response. */
		    byte[] data = callback.Get(this.Timeout);
    		
		    XMLElement playlistElement = XML.Load(
			    "<?xml version=\"1.0\" encoding=\"utf-8\" ?><playlist>" +
                Encoding.UTF8.GetString(data, 0, data.Length) + "</playlist>");		
    		
		    /* Check for success. */
		    if (playlistElement.HasChild("confirm")){
			    /* Split version string into parts. */
			    String[] parts = playlistElement.GetChild("confirm").GetChildText("version").Split(new Char[] { ',' });
    			
			    /* Set values. */
			    playlist.Revision = Int64.Parse(parts[0]);
			    playlist.Collaborative = (Int32.Parse(parts[3]) == 1);
    			
			    if (playlist.GetChecksum() != Int64.Parse(parts[2]))
                {
				    //System.out.println("Checksum error!");

                    //FIXME: Maybe shouldn't halt here
                    throw new Exception("Checksum error!");
			    }
    			
			    return playlist;
		    }
    		
		    return null;
        }

        /**
	     * Add a track to a playlist.
	     * 
	     * @param playlist The playlist.
	     * @param track    The track to be added.
	     * @param position The target position of the added track.
	     * 
	     * @return true on success and false on failure.
	     */
        public Boolean PlaylistAddTrack(Playlist playlist, Track track, Int32 position)
        {
            List<Track> tracks = new List<Track>();
            tracks.Add(track);
            return this.PlaylistAddTracks(playlist, tracks, position);
        }

        /**
	     * Add multiple tracks to a playlist.
	     * 
	     * @param playlist The playlist.
	     * @param tracks   A {@link List} of tracks to be added.
	     * @param position The target position of the added track.
	     * 
	     * @return true on success and false on failure.
	     */
        public Boolean PlaylistAddTracks(Playlist playlist, List<Track> tracks, Int32 position)
        {
            String user = this._session.Username;
		
		    /* Check if user is allowed to edit playlist. */
		    if (!playlist.Collaborative && playlist.Author != user)
            {
			    return false;
		    }
    		
		    /* First add the tracks to calculate the new checksum. */
		    playlist.Tracks.InsertRange(position, tracks);
    		
		    /* Build a comma separated list of tracks and append '01' to every id!. */
		    String trackList = "";
    		
		    for (int i = 0; i < tracks.Count; i++)
            {
			    trackList += ((i > 0)?",":"") + tracks[i].Id + "01";
		    }
    		
		    String xml = String.Format(
			    "<change><ops><add><i>{0}</i><items>{1}</items></add></ops>" +
			    "<time>{2}</time><user>{3}</user></change>" +
			    "<version>{4},{5},{6},{7}</version>",
			    position, trackList, Time.GetUnixTimestamp(), user,
			    PadLeft(playlist.Revision + 1, '0', 10),
                PadLeft(playlist.Tracks.Count, '0', 10),
			    PadLeft(playlist.GetChecksum(), '0', 10),
                playlist.Collaborative ? 1 : 0
		    );
    		
		    /* Remove the tracks again, because we need the old checksum for sending. */
		    for (int i = 0; i < tracks.Count; i++)
            {
			    playlist.Tracks.RemoveAt(position);
		    }
    		
		    /* Create channel callback */
		    ChannelCallback callback = new ChannelCallback();
    		
		    /* Send change playlist request. */
		    try
            {
			    this._protocol.SendChangePlaylist(callback, playlist, xml);
		    }
		    catch (ProtocolException)
            {
			    return false;
		    }
    		
		    /* Get response. */
		    byte[] data = callback.Get(this.Timeout);
    		
		    XMLElement playlistElement = XML.Load(
			    "<?xml version=\"1.0\" encoding=\"utf-8\" ?><playlist>" +
                Encoding.UTF8.GetString(data, 0, data.Length) + "</playlist>");
    		
		    /* Check for success. */
		    if (playlistElement.HasChild("confirm"))
            {
			    /* Split version string into parts. */
			    String[] parts = playlistElement.GetChild("confirm").GetChildText("version").Split(new Char[] { ',' });
    			
			    /* Set values. */
			    playlist.Revision = Int64.Parse(parts[0]);
			    playlist.Collaborative = (Int32.Parse(parts[3]) == 1);
    			
			    /* Add the tracks, since operation was successful. */
			    playlist.Tracks.InsertRange(position, tracks);
    			
			    if (playlist.GetChecksum() != Int64.Parse(parts[2]))
                {
				    //System.out.println("Checksum error!");

                    //FIXME: Maybe shouldn't halt here
                    throw new Exception("Checksum error!");
			    }
    			
			    return true;
		    }
    		
		    return false;
        }

        /**
	     * Remove a track from a playlist.
	     * 
	     * @param playlist The playlist.
	     * @param position The position of the track to remove.
	     * 
	     * @return true on success and false on failure.
	     */
        public Boolean PlaylistRemoveTrack(Playlist playlist, Int32 position)
        {
            return this.PlaylistRemoveTracks(playlist, position, 1);
        }

        /**
	     * Remove multiple tracks from a playlist.
	     * 
	     * @param playlist The playlist.
	     * @param position The position of the tracks to remove.
	     * @param count    The number of track to remove.
	     * 
	     * @return true on success and false on failure.
	     */
        public Boolean PlaylistRemoveTracks(Playlist playlist, Int32 position, Int32 count)
        {
            String user = this._session.Username;
		
		    /* Check if user is allowed to edit playlist. */
		    if (!playlist.Collaborative && playlist.Author != user)
            {
			    return false;
		    }
    		
		    /* Create a sublist view (important!) and clone it by constructing a new ArrayList. */
		    List<Track> tracks = new List<Track>(
			    playlist.Tracks.GetRange(position, count)
		    );
    		
		    /* First remove the track(s) to calculate the new checksum. This needs to be done in single steps! */
		    for (int i = 0; i < tracks.Count; i++)
            {
			    playlist.Tracks.RemoveAt(position);
		    }
    		
		    String xml = String.Format(
			    "<change><ops><del><i>{0}</i><k>{1}</k></del></ops>" +
			    "<time>{2}</time><user>{3}</user></change>" +
			    "<version>{4},{5},{6},{7}</version>",
			    position, count, Time.GetUnixTimestamp(), user,
                PadLeft(playlist.Revision + 1, '0', 10),
                PadLeft(playlist.Tracks.Count, '0', 10),
			    PadLeft(playlist.GetChecksum(), '0', 10),
                playlist.Collaborative ? 1 : 0
		    );
    		
		    /* Add the track(s) again, because we need the old checksum for sending. */
		    playlist.Tracks.InsertRange(position, tracks);
    		
		    /* Create channel callback */
		    ChannelCallback callback = new ChannelCallback();
    		
		    /* Send change playlist request. */
		    try
            {
			    this._protocol.SendChangePlaylist(callback, playlist, xml);
		    }
		    catch (ProtocolException)
            {
			    return false;
		    }
    		
		    /* Get response. */
		    byte[] data = callback.Get(this.Timeout);
    		
		    XMLElement playlistElement = XML.Load(
			    "<?xml version=\"1.0\" encoding=\"utf-8\" ?><playlist>" +
                Encoding.UTF8.GetString(data, 0, data.Length) + "</playlist>");
    		
		    /* Check for success. */
		    if (playlistElement.HasChild("confirm")){
			    /* Split version string into parts. */
			    String[] parts = playlistElement.GetChild("confirm").GetChildText("version").Split(new Char[] { ',' });
    			
			    /* Set values. */
			    playlist.Revision = Int64.Parse(parts[0]);
			    playlist.Collaborative = (Int32.Parse(parts[3]) == 1);
    			
			    /* Remove the track(s), since operation was successful. */
			    for (int i = 0; i < tracks.Count; i++)
                {
				    playlist.Tracks.RemoveAt(position);
			    }
    			
			    if (playlist.GetChecksum() != Int64.Parse(parts[2]))
                {
				    //System.out.println("Checksum error!");

                    //FIXME: Maybe shouldn't halt here
                    throw new Exception("Checksum error!");
			    }
    			
			    return true;
		    }
    		
		    return false;
        }

        //FIXME
        // TODO: playlistMoveTrack(s) : <mov><i>6</i><j>2</j></mov>


        /**
	     * Rename a playlist.
	     * 
	     * @param playlist The {@link Playlist} to rename.
	     * @param name     The new name for the playlist.
	     * 
	     * @return true on success or false on failure.
	     * 
	     * @see Playlist
	     */
        public Boolean PlaylistRename(Playlist playlist, String name)
        {
            String user = this._session.Username;
		
		    /* Check if user is allowed to rename playlist. */
		    if (playlist.Author != user)
            {
			    return false;
		    }
    		
		    String xml = String.Format(
			    "<change><ops><name>{0}</name></ops>" +
			    "<time>{1}</time><user>{2}</user></change>" +
			    "<version>{3},{4},{5},{6}</version>",
			    name, Time.GetUnixTimestamp(), user,
			    PadLeft(playlist.Revision + 1, '0', 10),
                PadLeft(playlist.Tracks.Count, '0', 10),
			    PadLeft(playlist.GetChecksum(), '0', 10),
                playlist.Collaborative ? 1 : 0
		    );
    		
		    /* Create channel callback */
		    ChannelCallback callback = new ChannelCallback();
    		
		    /* Send change playlist request. */
		    try
            {
			    this._protocol.SendChangePlaylist(callback, playlist, xml);
		    }
		    catch (ProtocolException)
            {
			    return false;
		    }
    		
		    /* Get response. */
		    byte[] data = callback.Get(this.Timeout);
    		
		    XMLElement playlistElement = XML.Load(
			    "<?xml version=\"1.0\" encoding=\"utf-8\" ?><playlist>" +
                Encoding.UTF8.GetString(data, 0, data.Length) + "</playlist>");		
    		
		    if (playlistElement.HasChild("confirm"))
            {
			    /* Split version string into parts. */
			    String[] parts = playlistElement.GetChild("confirm").GetChildText("version").Split(new Char[] { ',' });
    			
			    /* Set values. */
			    playlist.Revision = Int64.Parse(parts[0]);
			    playlist.Collaborative = (Int32.Parse(parts[3]) == 1);
			    playlist.Name = name;
    			
			    if(playlist.GetChecksum() != Int64.Parse(parts[2]))
                {
				    //System.out.println("Checksum error!");

                    //FIXME: Maybe shouldn't halt here
                    throw new Exception("Checksum error!");
			    }
    			
			    return true;
		    }
    		
		    return false;
        }

        /**
	     * Set playlist collaboration.
	     * 
	     * @param playlist      The {@link Playlist} to change.
	     * @param collaborative Whether it should be collaborative or not.
	     * 
	     * @return true on success or false on failure.
	     * 
	     * @see Playlist
	     */
        public Boolean PlaylistSetCollaborative(Playlist playlist, Boolean collaborative)
        {
            String user = this._session.Username;
		
		    /* Check if user is allowed to set playlist collaboration. */
		    if (playlist.Author != user)
            {
			    return false;
		    }
    		
		    String xml = String.Format(
			    "<change><ops><pub>{0}</pub></ops>" +
			    "<time>{1}</time><user>{2}</user></change>" +
			    "<version>{3},{4},{5},{6}</version>",
			    collaborative?1:0, Time.GetUnixTimestamp(), user,
			    PadLeft(playlist.Revision + 1, '0', 10),
                PadLeft(playlist.Tracks.Count, '0', 10),
			    PadLeft(playlist.GetChecksum(), '0', 10),
                playlist.Collaborative ? 1 : 0
		    );
    		
		    /* Create channel callback */
		    ChannelCallback callback = new ChannelCallback();
    		
		    /* Send change playlist request. */
		    try
            {
			    this._protocol.SendChangePlaylist(callback, playlist, xml);
		    }
		    catch (ProtocolException)
            {
			    return false;
		    }
    		
		    /* Get response. */
		    byte[] data = callback.Get(this.Timeout);
    		
		    XMLElement playlistElement = XML.Load(
			    "<?xml version=\"1.0\" encoding=\"utf-8\" ?><playlist>" +
                Encoding.UTF8.GetString(data, 0, data.Length) + "</playlist>");		
    		
		    if (playlistElement.HasChild("confirm"))
            {
			    /* Split version string into parts. */
			    String[] parts = playlistElement.GetChild("confirm").GetChildText("version").Split(new Char[] { ',' });
    			
			    /* Set values. */
			    playlist.Revision = Int64.Parse(parts[0]);
			    playlist.Collaborative = (Int32.Parse(parts[3]) == 1);
    			
			    if (playlist.GetChecksum() != Int64.Parse(parts[2]))
                {
				    //System.out.println("Checksum error!");

                    //FIXME: Maybe shouldn't halt here
                    throw new Exception("Checksum error!");
			    }
    			
			    return true;
		    }
    		
		    return false;
        }

        /// <summary>
        /// Get the music data of a track
        /// </summary>
        /// <param name="track">Object identifying the track to be played</param>
        /// <returns>An unencrypted stream of OGG data</returns>
        public MusicStream GetMusicStream(Track track, Media.File file)
        {
            /* Create channel callbacks. */
            ChannelCallback callback = new ChannelCallback();
            ChannelHeaderCallback headerCallback = new ChannelHeaderCallback();

            /* Send play request (token notify + AES key). */
            try
            {
                this._protocol.SendPlayRequest(callback, track, file);
            }
            catch (ProtocolException)
            {
                return null;
            }

            /* Get AES key. */
            byte[] key = callback.Get(this.Timeout);

            /* Send header request to check for HTTP stream. */
            try
            {
                this._protocol.SendSubstreamRequest(headerCallback, file, 0, 0);
            }
            catch (ProtocolException)
            {
                return null;
            }

            /* Get list of HTTP stream URLs. */
            String[] urls = headerCallback.Get(this.Timeout);

            /* If we got 4 HTTP stream URLs use them, otherwise use default channel streaming. */
            if (urls.Length == 4)
            {
                //this.player = new HTTPStreamPlayer(urls, track, key, listener);
                throw new NotImplementedException();
            }

            MusicStream ms = new MusicStream();
            _streamer = new ChannelStreamer(_protocol, file, key, ms);

            return ms;
        }


        private String PadLeft(String input, Char padChar, Int32 length)
        {
            if (input.Length < length)
                return new String(padChar, length - input.Length) + input;
            return input;
        }

        private String PadLeft(Int32 input, Char padChar, Int32 length)
        {
            return PadLeft(input.ToString(), padChar, length);
        }

        private String PadLeft(Int64 input, Char padChar, Int32 length)
        {
            return PadLeft(input.ToString(), padChar, length);
        }

        #endregion


        #region construction

        public SharpSpotConnection(ICache cache, TimeSpan timeout)
        {
		    this._cache = cache;
		    this._timeout = timeout;
	    }

        public SharpSpotConnection() : this(new FileCache(), new TimeSpan(0, 0, 10))
        {
        }

        #endregion


        #region CommandListener Members

        public void CommandReceived(Int32 command, Byte[] payload)
        {
            switch (command)
            {
                case (Int32)Command.COMMAND_SECRETBLK:
                    /* Check length. */
                    if (payload.Length != 336)
                    {
                        //System.err.format("Got command 0x02 with len %d, expected 336!\n", payload.length);
                        //FIXME: Error
                        throw new Exception("Got command 0x02 with len " + payload.Length + ", expected 336!");
                    }

                    /* Check RSA public key. */
                    byte[] rsaPublicKey = this._session.RSAPublicKey.ToByteArray();

                    for (int i = 0; i < 128; i++)
                    {
                        if (payload[16 + i] != rsaPublicKey[i])
                        {
                            //System.err.format("RSA public key doesn't match! %d\n", i);
                            throw new Exception("RSA public key doesn't match.");

                            //break;
                        }
                    }

                    /* Send cache hash. */
                    try
                    {
                        this._protocol.SendCacheHash();
                    }
                    catch (ProtocolException)
                    {
                        /* Just don't care. */
                    }

                    break;
                case (Int32)Command.COMMAND_PING:
                    {
                        /* Ignore the timestamp but respond to the request. */
                        /* int timestamp = IntegerUtilities.bytesToInteger(payload); */
                        try
                        {
                            this._protocol.SendPong();
                        }
                        catch (ProtocolException)
                        {
                            /* Just don't care. */
                        }

                        break;
                    }
                case (Int32)Command.COMMAND_CHANNELDATA:
                    {
                        Channel.Process(payload);
                        break;
                    }
                case (Int32)Command.COMMAND_CHANNELERR:
                    {
                        Channel.Error(payload);
                        break;
                    }
                case (Int32)Command.COMMAND_AESKEY:
                    {
                        /* Channel id is at offset 2. AES Key is at offset 4. */
                        Byte[] buffer = new Byte[payload.Length - 2];
                        Array.Copy(payload, 2, buffer, 0, buffer.Length);
                        Channel.Process(buffer);

                        break;
                    }
                case (Int32)Command.COMMAND_SHAHASH:
                    {
                        /* Do nothing. */
                        break;
                    }
                case (Int32)Command.COMMAND_COUNTRYCODE:
                    {
                        //System.out.println("Country: " + new String(payload, Charset.forName("UTF-8")));
                        this._user.Country = Encoding.UTF8.GetString(payload, 0, payload.Length);

                        /* Release 'country' permit. */
                        this._userSemaphore.Release();

                        break;
                    }
                case (Int32)Command.COMMAND_P2P_INITBLK:
                    {
                        /* Do nothing. */
                        break;
                    }
                case (Int32)Command.COMMAND_NOTIFY:
                    {
                        /* HTML-notification, shown in a yellow bar in the official client. */
                        /* Skip 11 byte header... */
                        /*System.out.println("Notification: " + new String(
                            Arrays.copyOfRange(payload, 11, payload.length), Charset.forName("UTF-8")
                        ));*/
                        this._user.Notification = Encoding.UTF8.GetString(payload, 11, payload.Length - 11);

                        break;
                    }
                case (Int32)Command.COMMAND_PRODINFO:
                    {
                        XMLElement prodinfoElement = XML.Load(Encoding.UTF8.GetString(payload, 0, payload.Length));

                        this._user = Media.User.FromXMLElement(prodinfoElement, this._user);

                        /* Release 'prodinfo' permit. */
                        this._userSemaphore.Release();

                        break;
                    }
                case (Int32)Command.COMMAND_WELCOME:
                    {
                        /* Request ads. */
                        //this.protocol.sendAdRequest(new ChannelAdapter(), 0);
                        //this.protocol.sendAdRequest(new ChannelAdapter(), 1);

                        break;
                    }
                case (Int32)Command.COMMAND_PAUSE:
                    {
                        /* TODO: Show notification and pause. */

                        break;
                    }
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Spoffice.Website.Helpers;
using Lucene.Net.Documents;
using System.Configuration;
using Spoffice.Website.Services;
using Spoffice.Website.Models.Output;
using System.Security.Cryptography;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
namespace Spoffice.FileSystem
{
    public class MusicBrowser : IMusicBrowser
    {
        private string indexPath;
        private string musicRootPath;
        private Directory dir;
        private string[] supported = new string[] { "mp3", "ogg" };
        public MusicBrowser()
        {
            musicRootPath = ConfigurationManager.AppSettings["FileSystem.MusicRootPath"];
            if (String.IsNullOrEmpty(musicRootPath) || !System.IO.Directory.Exists(musicRootPath))
            {
                throw new Exception("FileSystem.MusicRootPath is not set or it doesn't exist as a folder");
            }
            System.IO.FileInfo musicDirInfo = new System.IO.FileInfo(musicRootPath);

            indexPath = String.Format("{1}{0}{2}{0}{3}", System.IO.Path.DirectorySeparatorChar, CacheHelper.CachePath, "filesystem", "index");
            if (System.IO.Directory.Exists(indexPath))
            {
                System.IO.Directory.Delete(indexPath, true);
            }
            CacheHelper.CreateFolder(indexPath);

            dir = FSDirectory.GetDirectory(indexPath, true);

            IndexWriter indexWriter = new IndexWriter(dir, new StandardAnalyzer(), true);

            IndexMusic(indexWriter, musicDirInfo);

            indexWriter.Optimize();
            indexWriter.Close();
            
        }
        private void IndexMusic(IndexWriter writer, System.IO.FileInfo file)
        {
            if (System.IO.Directory.Exists(file.FullName))
            {
                string[] files = System.IO.Directory.GetFileSystemEntries(file.FullName);
                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        IndexMusic(writer, new System.IO.FileInfo(files[i]));
                    }
                } 
            }
            else
            {
                if (supported.Contains(file.Extension.ToLower()))
                {
                    writer.AddDocument(IndexTags(file));
                }
            }
        }
        private Document IndexTags(System.IO.FileInfo f)
        {

            Document doc = new Document();
            doc.Add(new Field("path", f.FullName, Field.Store.YES,
                 Field.Index.UN_TOKENIZED));

            Entagged.AudioFile tags = new Entagged.AudioFile(f.FullName);
          
            doc.Add(new Field("title", tags.Title, Field.Store.YES,
                 Field.Index.UN_TOKENIZED));
            doc.Add(new Field("artist", tags.Artist, Field.Store.YES,
                 Field.Index.UN_TOKENIZED));
            doc.Add(new Field("album", tags.Album, Field.Store.YES,
                 Field.Index.UN_TOKENIZED));
            doc.Add(new Field("length", tags.Duration.Milliseconds.ToString(), Field.Store.YES,
                 Field.Index.UN_TOKENIZED));
            doc.Add(new Field("id", GetDeterministicGuid(f.FullName).ToString(), Field.Store.YES,
                 Field.Index.UN_TOKENIZED));
            doc.Add(new Field("artistid", GetDeterministicGuid(tags.Artist).ToString(), Field.Store.YES,
                 Field.Index.UN_TOKENIZED));
            doc.Add(new Field("albumid", GetDeterministicGuid(tags.Album).ToString(), Field.Store.YES,
                 Field.Index.UN_TOKENIZED));

            return doc;
        }

        private Guid GetDeterministicGuid(string input)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] inputBytes = Encoding.Default.GetBytes(input);
            byte[] hashBytes = provider.ComputeHash(inputBytes);
            Guid hashGuid = new Guid(hashBytes);
            return hashGuid;
        }

        #region IMusicBrowser Members

        public List<ArtistOutput> SearchForArtist(string query)
        {
            throw new NotImplementedException();
        }

        public List<AlbumOutput> SearchForAlbum(string query)
        {
            throw new NotImplementedException();
        }

        public List<TrackOutput> SearchForTrack(string query)
        {
            throw new NotImplementedException();
        }

        public ArtistOutput GetArtistById(Guid id)
        {
            IndexSearcher searcher = new IndexSearcher(dir);
            QueryParser qp = new QueryParser("artistid", new SimpleAnalyzer());
            Query q = qp.Parse(id.ToString());
            Hits hits = searcher.Search(q);
            ArtistOutput artist = new ArtistOutput();
            artist.Albums = new List<AlbumOutput>();
            for (int i = 0; i < hits.Length(); i++)
            {
                Document doc = hits.Doc(i);
                artist.Name = doc.Get("artist");
                artist.Id = new Guid(doc.Get("artistid"));
                artist.Albums.Add(new AlbumOutput
                {
                    Id = new Guid(doc.Get("albumid")),
                    Name = doc.Get("album")
                });
            }
            return artist;
        }

        public AlbumOutput GetAlbumById(Guid id)
        {
            throw new NotImplementedException();
        }

        public TrackOutput GetTrackById(Guid id)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

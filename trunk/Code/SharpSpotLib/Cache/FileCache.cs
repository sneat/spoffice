using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SharpSpotLib.Cache
{
    public class FileCache : ICache
    {
        #region constants

        private const String CACHE_DIRECTORY_NAME = "SharpSpotCache";

        #endregion

        #region fields

        /**
	     * The directory for storing cache data.
	     */
        private String _directory;

        #endregion


        #region methods

        private void ClearDirectory(String directory, Boolean includeSubDirs)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directory);
                foreach (FileInfo fi in dirInfo.GetFiles())
                {
                    try
                    {
                        fi.Delete();
                    }
                    catch (Exception)
                    {
                        throw; //FIXME
                    }
                }
                if (includeSubDirs)
                {
                    foreach (DirectoryInfo di in dirInfo.GetDirectories())
                    {
                        try
                        {
                            di.Delete(true);
                        }
                        catch (Exception)
                        {
                            throw; //FIXME
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw; //FIXME
            }
        }

        private String GetFullPath(String category, String hash)
        {
            return Path.Combine(Path.Combine(_directory, category), hash);
        }

        #endregion


        #region Cache members

        public void Clear()
        {
            ClearDirectory(_directory, true);
        }

        public void Clear(String category)
        {
            ClearDirectory(Path.Combine(_directory, category), false);
        }

        public Boolean Contains(String category, String hash)
        {
            return File.Exists(GetFullPath(category, hash));
        }

        public Byte[] Load(String category, String hash)
        {
            try
            {
                using (FileStream fs = new FileStream(GetFullPath(category, hash), FileMode.Open, FileAccess.Read))
                {
                    Byte[] buffer = new Byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    return buffer;
                }
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
                File.Delete(GetFullPath(category, hash));
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

        private void EnsureDirectories(String fileName)
        {
            DirectoryInfo dirInfo = new FileInfo(fileName).Directory;
            EnsureDirectories(dirInfo);            
        }

        private void EnsureDirectories(DirectoryInfo dir)
        {
            if (!dir.Parent.Exists)
            {
                EnsureDirectories(dir.Parent);
                dir.Create();
            }
            else if (!dir.Exists)
                dir.Create();
        }

        public void Store(String category, String hash, Byte[] data, Int32 size)
        {
            try
            {
                String fileName = GetFullPath(category, hash);
                EnsureDirectories(fileName);
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(data, 0, size);
                }
            }
            catch (Exception ex)
            {
                //Ignore errors
            }
        }

        public String[] List(String category)
        {
            List<String> fileList = new List<String>();
            try
            {
                DirectoryInfo di = new DirectoryInfo(Path.Combine(_directory, category));
                foreach (FileInfo fi in di.GetFiles())
                    fileList.Add(fi.Name);
            }
            catch (Exception)
            {
                throw; //FIXME
            }
            return fileList.ToArray();
        }

        #endregion


        #region construction

        public FileCache()
            : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CACHE_DIRECTORY_NAME))
        {

        }

        public FileCache(String directory)
        {
            this._directory = directory;
        }

        #endregion
    }
}

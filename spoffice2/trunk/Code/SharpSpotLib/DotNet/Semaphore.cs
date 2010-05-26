using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SharpSpotLib.Util
{
    /*
     *  Thanks to staceyw @ CodeProject.com
     *   http://www.codeproject.com/KB/recipes/dijkstracountingsemaphore.aspx
     */
    internal class Semaphore
    {
        #region fields

        private Int32 _availableCount = 1;
        private Int32 _maxCount = 1;
        private readonly Object _lock = new Object();

        #endregion


        #region properties

        public Int32 AvailablePermits { get { return _availableCount; } }

        #endregion


        #region methods

        public void AcquireUninterruptibly()
        {
            lock (_lock)
            {
                while (_availableCount == 0)
                {
                    try
                    {
                        Monitor.Wait(_lock);
                    }
                    catch (Exception)
                    {
                        Monitor.Pulse(_lock);
                        throw;
                    }
                }
                _availableCount--;
            }
        }

        public void Release(Int32 releaseCount)
        {
            if (releaseCount < 1)
                throw new ArgumentOutOfRangeException("count");
            lock (_lock)
            {
                if (_availableCount + releaseCount > _maxCount)
                    throw new Exception("Can't release that many.");
                _availableCount += releaseCount;
                Monitor.PulseAll(_lock);
            }
        }

        public void Release()
        {
            Release(1);
        }

        public Boolean TryAcquire(TimeSpan timeout)
        {
            return TryAcquire(1, timeout);
        }

        public Boolean TryAcquire(Int32 acquireCount, TimeSpan timeout)
        {
            lock (_lock)
            {
                while (_availableCount < acquireCount)
                {
                    try
                    {
                        if (!Monitor.Wait(_lock, timeout))
                            return false;
                    }
                    catch (Exception)
                    {
                        Monitor.Pulse(_lock);
                        throw;
                    }
                }
                _availableCount -= acquireCount;
                return true;
            }
        }


        #endregion


        #region construction

        public Semaphore(Int32 maxCount) : this(maxCount, maxCount)
        {
        }

        public Semaphore(Int32 availableCount, Int32 maxCount)
        {
            if (maxCount < 1)
                throw new ArgumentOutOfRangeException("maxCount", "Max count must be >= 1.");
            _availableCount = availableCount;
            _maxCount = maxCount;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SharpSpotLib.Util
{
    internal class ByteBuffer
    {
        #region fields

        private MemoryStream _ms;
        private Int32 _limit = 0;

        #endregion

        #region properties

        public Int32 Remaining
        {
            get
            {
                return Limit - Position;
            }
        }

        public Int32 Position
        {
            get
            {
                if (_ms.Position > Int32.MaxValue)
                    throw new OverflowException();
                return (Int32)_ms.Position;
            }
            set
            {
                if (value < Limit)
                    _ms.Position = value;
            }
        }

        public Int32 Limit
        {
            get { return _limit; }
            set { _limit = value; }
        }

        #endregion

        #region methods

        public Byte[] ToArray()
        {
            return _ms.ToArray();
        }

        public void Get(Byte[] buffer)
        {
            _ms.Read(buffer, 0, buffer.Length);
        }

        public Byte Get()
        {
            Int32 b = _ms.ReadByte();
            if (b >= Byte.MinValue && b <= Byte.MaxValue)
                return (Byte)b;
            throw new OverflowException();
        }

        public void Put(Byte[] data)
        {
            Put(data, 0, data.Length);
        }

        public void Put(Int32 index, Byte[] data)
        {
            Int64 originalPosition = _ms.Position;
            _ms.Position = index;
            Put(data);
            _ms.Position = originalPosition;
        }

        public void Put(Byte[] data, Int32 offset, Int32 length)
        {
            _ms.Write(data, offset, length);
            /*using (BinaryWriter bw = new BinaryWriter(_ms))
                bw.Write(data);*/
        }

        public void Put(Byte data)
        {
            _ms.WriteByte(data);
        }

        public void PutInt(Int32 data)
        {
            Put(IntegerUtilities.ToBytes(data));
        }

        public Int32 GetInt()
        {
            Byte[] buffer = new Byte[4];
            _ms.Read(buffer, 0, buffer.Length);
            return IntegerUtilities.BytesToInteger(buffer);
        }

        public void PutShort(Int16 data)
        {
            Put(ShortUtilities.ToBytes(data));
        }

        public void PutShort(Int32 index, Int16 data)
        {
            Put(index, ShortUtilities.ToBytes(data));
        }

        public Int16 GetShort()
        {
            Byte[] buffer = new Byte[2];
            _ms.Read(buffer, 0, buffer.Length);
            return ShortUtilities.BytesToShort(buffer);
        }

        public Byte GetByte()
        {
            return (Byte)_ms.ReadByte();
        }

        public void Flip()
        {
            this.Limit = this.Position;
            _ms.Position = 0;
        }

        #endregion

        #region construction

        public ByteBuffer(Int32 capacity)
        {
            _ms = new MemoryStream(capacity);
            _limit = capacity;
        }

        public ByteBuffer(Byte[] data)
        {
            _ms = new MemoryStream(data);
            _limit = data.Length;
        }

        public ByteBuffer(Byte[] data, Int32 offset, Int32 length)
        {
            _ms = new MemoryStream(data, offset, length);
        }

        public static ByteBuffer Allocate(Int32 capacity)
        {
            return new ByteBuffer(capacity);
        }

        public static ByteBuffer Wrap(Byte[] data)
        {
            return new ByteBuffer(data);
        }

        public static ByteBuffer Wrap(Byte[] data, Int32 offset, Int32 length)
        {
            return new ByteBuffer(data, offset, length);
        }

        #endregion
    }
}

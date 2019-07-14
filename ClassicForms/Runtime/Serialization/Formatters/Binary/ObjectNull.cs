﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Runtime.Serialization.Formatters.Binary
{
    internal sealed class ObjectNull : IStreamable
    {
        // Fields
        internal int nullCount;

        // Methods
        internal ObjectNull()
        {
        }

        public void Dump()
        {
        }

        private void DumpInternal()
        {
            //if (BCLDebug.CheckEnabled("BINARY") && (this.nullCount != 1))
            //{
            //    int nullCount = this.nullCount;
            //}
        }

        public void Read(__BinaryParser input)
        {
            this.Read(input, BinaryHeaderEnum.ObjectNull);
        }

        public void Read(__BinaryParser input, BinaryHeaderEnum binaryHeaderEnum)
        {
            switch (binaryHeaderEnum)
            {
                case BinaryHeaderEnum.ObjectNull:
                    this.nullCount = 1;
                    return;

                case BinaryHeaderEnum.MessageEnd:
                case BinaryHeaderEnum.Assembly:
                    break;

                case BinaryHeaderEnum.ObjectNullMultiple256:
                    this.nullCount = input.ReadByte();
                    return;

                case BinaryHeaderEnum.ObjectNullMultiple:
                    this.nullCount = input.ReadInt32();
                    break;

                default:
                    return;
            }
        }

        internal void SetNullCount(int nullCount)
        {
            this.nullCount = nullCount;
        }

        //public void Write(__BinaryWriter sout)
        //{
        //    if (this.nullCount == 1)
        //    {
        //        sout.WriteByte(10);
        //    }
        //    else if (this.nullCount < 0x100)
        //    {
        //        sout.WriteByte(13);
        //        sout.WriteByte((byte)this.nullCount);
        //    }
        //    else
        //    {
        //        sout.WriteByte(14);
        //        sout.WriteInt32(this.nullCount);
        //    }
        //}
    }

}

﻿using System;
using System.IO;
using Native = ENet.Managed.Structures;
using ENet.Managed.Platforms;
using System.Text;

namespace ENet.Managed
{
    public unsafe class ENetPacket
    {
        internal byte[] m_Payload;

        public byte Channel { get; }
        public ENetPacketFlags Flags { get; }

        internal ENetPacket() { }

        internal ENetPacket(Native.ENetPacket* packet, byte channel)
        {
            Channel = channel;
            Flags = packet->Flags;
            m_Payload = new byte[packet->DataLength.ToUInt32()];
            fixed (byte* dest = m_Payload)
            {
                Platform.Current.MemoryCopy((IntPtr)dest, (IntPtr)packet->Data, packet->DataLength);
            }
        }

        public byte[] GetPayloadFinal() => m_Payload;
        public byte[] GetPayloadCopy()
        {
            byte[] clone = new byte[m_Payload.Length];
            ENetUtils.MemoryCopy(clone, m_Payload, clone.Length);
            return clone;
        }

        public MemoryStream GetPayloadStream(bool copy)
        {
            if (copy)
            {
                return new MemoryStream(GetPayloadCopy(), false);
            }

            return new MemoryStream(m_Payload, false);
        }

        public BinaryReader GetBinaryReader(bool copy) => new BinaryReader(GetPayloadStream(copy));
        public BinaryReader GetBinaryReader(Encoding encoding, bool copy) => new BinaryReader(GetPayloadStream(copy), encoding);
    }
}

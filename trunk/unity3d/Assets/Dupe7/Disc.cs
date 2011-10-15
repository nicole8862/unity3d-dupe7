using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dupe7
{
	class LCGRand
	{
		public static UInt32 Next (UInt32 a)
		{
			return (UInt32)(((UInt64)a * 279470273uL) % 4294967291uL);
		}
	}

	public enum DiscState
	{
		Open,
		Cracked,
		Shut
	}

	public enum DiscMarkedState
	{
		Unmarked,
		StripMarked,
		DiscMarked
	}

	[Serializable()]
	public struct Disc : ISerializable
	{
		public int num;
		public DiscState state;

		public Disc (SerializationInfo info, StreamingContext unused)
		{
			num = info.GetInt32 ("num");
			state = (DiscState)info.GetInt32 ("state");
		}

		public void GetObjectData (SerializationInfo info, StreamingContext unused)
		{
			info.AddValue ("num", num);
			info.AddValue ("state", (int)state);
		}
	}
}

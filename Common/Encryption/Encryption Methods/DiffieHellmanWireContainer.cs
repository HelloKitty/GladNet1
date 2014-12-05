using Org.Mentalis.Security.Cryptography;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	[ProtoContract]
	public class DiffieHellmanWireContainer
	{
		[ProtoMember(1)]
		public DHParameters Parameters { get; private set; }

		[ProtoMember(2)]
		public byte[] PublicKey { get; private set; }

		public DiffieHellmanWireContainer(DHParameters parameters, byte[] pKey)
		{
			Parameters = parameters;
			PublicKey = pKey;
		}

		/// <summary>
		/// Protobuf-net constructor
		/// </summary>
		public DiffieHellmanWireContainer()
		{

		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace GladNet.Common
{
	public class DiffieHellmanAESEncryptor : EncryptionBase
	{
		readonly ECDiffieHellmanCng internalDiffieHellmanImplementation;

		public DiffieHellmanAESEncryptor()
			: base()
		{
			//Prepre diffiehellman key exchange, diffiehellman will produce key material via a hash algorithm that defaults to
			//SHA 256
			internalDiffieHellmanImplementation = new ECDiffieHellmanCng();
			internalDiffieHellmanImplementation.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
		}

		public override byte EncryptionTypeByte
		{
			get { return 1; }
		}

		public override bool SetKeyPublicKey(byte[] pKey)
		{
			throw new NotImplementedException();
		}

		public override byte[] Encrypt(byte[] toEncrypt, out byte[] addtionalBytes)
		{
			throw new NotImplementedException();
		}

		public override byte[] Decrypt(byte[] toDecrypt, byte[] additionalBytes)
		{
			throw new NotImplementedException();
		}

		public override byte[] GetPublicKey()
		{
			throw new NotImplementedException();
		}

		public override byte[] NetworkInitRequiredData()
		{
			throw new NotImplementedException();
		}

		public override bool SetNetworkInitRequiredData(byte[] data)
		{
			throw new NotImplementedException();
		}
	}
}

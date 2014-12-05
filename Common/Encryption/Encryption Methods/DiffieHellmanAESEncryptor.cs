using Org.Mentalis.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class DiffieHellmanAESEncryptor : EncryptionBase
	{
		private readonly DiffieHellmanManaged internalEncryptionObj;
		private byte[] secretKey;

		public DiffieHellmanAESEncryptor()
			: base()
		{
			internalEncryptionObj = new DiffieHellmanManaged();
		}

		public override byte EncryptionTypeByte
		{
			get { return 1; }
		}

		public override bool SetKeyPublicKey(byte[] pKey)
		{
			throw new LoggableException("Cannot set public key of DiffieHellman mentalis implementation with only one byte array.", null, Logger.LogType.Error);
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
			DHParameters parameters = this.internalEncryptionObj.ExportParameters(false);

			return Serializer<GladNetProtobufNetSerializer>.Instance
				.Serialize(new DiffieHellmanWireContainer(parameters, internalEncryptionObj.CreateKeyExchange()));
		}

		public override byte[] NetworkInitRequiredData()
		{
			return this.GetPublicKey();
		}

		public override bool SetNetworkInitRequiredData(byte[] data)
		{
			if (data != null)
			{
				DiffieHellmanWireContainer container = Serializer<GladNetProtobufNetSerializer>.Instance.Deserialize<DiffieHellmanWireContainer>(data);

				if (container == null)
				{
					throw new LoggableException("Failed to set DiffieHellman params.", null, Logger.LogType.Error);
				}

				//We can import the parameters again on the client. We don't really want to but it's the easy way to handle the response. In this method
				//We cannot determine if we're the server or the client requesting this.
				internalEncryptionObj.ImportParameters(container.Parameters);

				if (container.PublicKey == null)
					throw new LoggableException("Recieved a null public key for mentalis DiffieHellman exchange.", null, Logger.LogType.Error);

				secretKey = internalEncryptionObj.DecryptKeyExchange(container.PublicKey);

				Console.WriteLine("DH KeyLength: " + secretKey.Length);

				return secretKey != null;
			}
			else
				return false;
		}

		public override void Dispose()
		{
			this.internalEncryptionObj.Clear();
		}
	}
}

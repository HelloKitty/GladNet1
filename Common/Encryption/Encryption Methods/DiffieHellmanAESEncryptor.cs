using Org.Mentalis.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GladNet.Common
{
	public class DiffieHellmanAESEncryptor : EncryptionBase
	{
		public static byte[] Bytes;

		private DiffieHellmanManaged internalEncryptionObj;
		private byte[] secretKey;
		private bool SentPublicKey;

		public DiffieHellmanAESEncryptor()
			: base()
		{
			internalEncryptionObj = new DiffieHellmanManaged(256, 160, DHKeyGeneration.Random);
			SentPublicKey = false;
		}

		public override byte EncryptionTypeByte
		{
			get { return EncryptionBase.DefaultByte; }
		}

		public override bool SetKeyPublicKey(byte[] pKey)
		{
			throw new LoggableException("Cannot set public key of DiffieHellman mentalis implementation with only one byte array.", null, LogType.Error);
		}

		public override byte[] Encrypt(byte[] toEncrypt, out byte[] addtionalBytes)
		{
			if (toEncrypt == null)
				throw new LoggableException("Tried to encrypt a null array.", new ArgumentException("In DiffieHellmanAESEncryptor: Encrypt. Parameter was null", "toEncrypt"), LogType.Error);

			try
			{
				using (AesCryptoServiceProvider aesObj = new AesCryptoServiceProvider())
				{
					aesObj.Key = this.secretKey;

					ICryptoTransform encryptor = aesObj.CreateEncryptor(aesObj.Key, aesObj.IV);

					using (MemoryStream ms = new MemoryStream())
					{
						using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
						{
							using (BinaryWriter sw = new BinaryWriter(cs))
							{
								sw.Write(toEncrypt);
							}
						}
						//This is additional salt-like information for the AES algorithm to
						//package with the packet.
						addtionalBytes = aesObj.IV;
						return ms.ToArray();
					}
				}
			}
			catch(CryptographicException e)
			{
				throw new LoggableException("Failed to encrypt package in DiffieHellmanAESEncryptor.", e, LogType.Error);
			}
		}

		public override byte[] Decrypt(byte[] toDecrypt, byte[] additionalBytes)
		{
			if (toDecrypt == null || toDecrypt.Length == 0)
				throw new LoggableException("Tried to decrypt a null array.", new ArgumentException("In DiffieHellmanAESEncryptor: Decrypt. Parameter was null", "toDecrypt"), LogType.Error);

			if (additionalBytes == null || additionalBytes.Length == 0)
				throw new LoggableException("IV For AES decryption was null (additionalBytes)", new ArgumentException("In DiffieHellmanAESEncryptor parameter for Decrypt was null", "additionalBytes"), LogType.Error);

			try
			{
				using (AesCryptoServiceProvider aesObj = new AesCryptoServiceProvider())
				{
					aesObj.Key = this.secretKey;
					aesObj.IV = additionalBytes;

					ICryptoTransform decryptor = aesObj.CreateDecryptor(aesObj.Key, aesObj.IV);

					using (MemoryStream ms = new MemoryStream(toDecrypt))
					{
						using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
						{
							using (BinaryReader br = new BinaryReader(cs))
							{
								//Reads every single byte in the stream
								return br.ReadBytes((int)toDecrypt.Length);
							}
						}
					}
				}
			}
			catch (CryptographicException e)
			{
				throw new LoggableException("Failed to decrypt package in DiffieHellmanAESEncryptor.", e, LogType.Error);
			}
		}

		public override byte[] GetPublicKey()
		{
			DHParameters parameters = this.internalEncryptionObj.ExportParameters(false);

			return Serializer<GladNetProtobufNetSerializer>.Instance
				.Serialize(new DiffieHellmanWireContainer(parameters, internalEncryptionObj.CreateKeyExchange()));
		}

		public override byte[] NetworkInitRequiredData()
		{
			SentPublicKey = true;
			return this.GetPublicKey();
		}

		public override bool SetNetworkInitRequiredData(byte[] data)
		{
			if (data != null)
			{
				DiffieHellmanWireContainer container = Serializer<GladNetProtobufNetSerializer>.Instance.Deserialize<DiffieHellmanWireContainer>(data);

				if (container == null)
				{
					throw new LoggableException("Failed to set DiffieHellman params.", null, LogType.Error);
				}

				//If we're the second peer we want to import the parameters.
				if (!SentPublicKey)
				{
					this.internalEncryptionObj.Clear();
					this.internalEncryptionObj = new DiffieHellmanManaged(container.Parameters.P, container.Parameters.G, 160);
				}

				if (container.PublicKey == null)
					throw new LoggableException("Recieved a null public key for mentalis DiffieHellman exchange.", null, LogType.Error);

				secretKey = internalEncryptionObj.DecryptKeyExchange(container.PublicKey);

				/*Console.WriteLine("DH KeyLength: " + secretKey.Length);

				for (int i = 0; i < secretKey.Length; i++)
					Console.Write(secretKey[i] + " ");*/

				Bytes = secretKey;

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

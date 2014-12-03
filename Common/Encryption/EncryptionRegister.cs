using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class EncryptionRegister : IRegisterable<EncryptionBase, byte>
	{
		private Dictionary<byte, EncryptionBase> encryptionCollection;

		public EncryptionRegister(int initialSize = 1)
		{
			if (initialSize <= 0)
				throw new ArgumentException("Cannot be less than 1.", "int initialSize");

			encryptionCollection = new Dictionary<byte, EncryptionBase>(initialSize);
		}

		public EncryptionBase this[byte key]
		{
			get { return encryptionCollection[key]; }
		}

		public bool Register(EncryptionBase obj, byte key)
		{
			if (encryptionCollection.ContainsKey(key))
				return false;

			encryptionCollection.Add(key, obj);

			return true;
		}

		public bool UnRegister(byte key)
		{
			return encryptionCollection.Remove(key);
		}

		public EncryptionBase GetValue(byte key)
		{
			return encryptionCollection[key];
		}

		public bool HasKey(byte key)
		{
			return encryptionCollection.ContainsKey(key);
		}

		public EncryptionBase.Decryptor GetDecrypter(byte key)
		{
			if (HasKey(key))
			{
				return this[key].Decrypt;
			}
			else
				return null;
		}

		public EncryptionBase.Encryptor GetEncrypter(byte key)
		{
			if (HasKey(key))
			{
				return this[key].Encrypt;
			}
			else
				return null;
		}
	}
}

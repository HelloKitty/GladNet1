using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface IEncryptable
	{
		byte EncryptionMethodByte { get; }

		bool isEncrypted { get; }
		bool wasEncrypted { get; }

		byte[] EncryptionAdditionalBlob { get; }

		void Encrypt(EncryptionBase encryptionObject);
		bool Decrypt(EncryptionBase encryptionObject);
	}
}

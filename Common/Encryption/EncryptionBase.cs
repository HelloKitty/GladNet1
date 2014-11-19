using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace GladNet.Common
{
	public abstract class EncryptionBase
	{
		/// <summary>
		/// Indicates the method of encryption, this should be unique so to identify encryption uses by remote users
		/// for packets.
		/// </summary>
		public abstract byte EncryptionTypeByte { get; }

		/// <summary>
		/// Sets the public key for the encryption method.
		/// </summary>
		/// <param name="pKey">Public key to use.</param>
		/// <returns>True if the key fits the method.</returns>
		public abstract bool SetKeyPublicKey(byte[] pKey);

		public abstract byte[] GetPublicKey(byte[] pKey);

		/// <summary>
		/// Encrypts a blob of bytes and returns the encrypted byte[]
		/// </summary>
		/// <param name="toEncrypt">Blob of bytes to encrypt.</param>
		/// <param name="addtionalBytes">Additional information needed for encryption.</param>
		/// <returns>Returns a byte[] represented an encrypted version of toEncrypted (toEncrypted may be encrypted itself)</returns>
		public abstract byte[] Encrypt(byte[] toEncrypt, out byte[] addtionalBytes);

		/// <summary>
		/// Decrypts a given byte[] and using the additionalBytes if needed.
		/// </summary>
		/// <param name="toDecrypt">The byte[] to be decrypted.</param>
		/// <param name="additionalBytes">Addtional bytes that the encryption method may require.</param>
		/// <returns>Returns a decrypted byte[] (toDecrypt may already be decrypted itself)</returns>
		public abstract byte[] Decrypt(byte[] toDecrypt, byte[] additionalBytes);
	}
}

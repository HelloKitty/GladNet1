using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface IPacketConverter : IConverter<IPackage, SerializerBase, NetworkPackage>, 
		IConverter<IEncryptablePackage, SerializerBase, EncryptionBase, NetworkPackage>
	{
		NetworkPackageType Convert<NetworkPackageType>(IPackage package, SerializerBase serializer) where NetworkPackageType
			: NetworkPackage, new();

		NetworkPackageType Convert<NetworkPackageType>(IEncryptablePackage package, SerializerBase serializer, EncryptionBase decryptor) where NetworkPackageType
			: NetworkPackage, new();
	}
}

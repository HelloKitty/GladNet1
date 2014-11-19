#region copyright
/// GladNet Copyright (C) 2014 Andrew Blakely 
/// andrew.blakely@ymail.com
/// GitHub: HeIIoKitty
/// Unity3D: Glader
/// Please refer to the repo License file for licensing information
/// If this source code has been distributed without a copy of the original license file then this is an illegal copy and you should delete it
#endregion
using Common.Packet.Serializers;
using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public abstract class MessageReciever
	{
		//These are internal because we want child classes in the assembly to have access but not child classes outside the assembly
		protected readonly SerializationManager SerializerRegister;

		protected readonly PacketConverter Converter;

		public MessageReciever()
		{
			//Create the registers for serializers and the messagehandlers for a given serializer too.
			SerializerRegister = new SerializationManager();
			//Register profobuf-net as it's used internally
			//Create the message converter that will hold references to 
			//HighlevelMessageConverter = new LidgrenMessageConverter();
			Converter = new PacketConverter();
		}

		/// <summary>
		/// Provides a method for users to register their own serializer with the networking. This will create a handler to handle packet serialized with the serializer
		/// as long as the reciever also register the serializer too.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public bool RegisterSerializer<T>() where T : SerializerBase
		{
			if (SerializerRegister.HasKey(Serializer<T>.Instance.SerializerUniqueKey))
				throw new Exception("Failed to register Serializer of Type: " + Serializer<T>.Instance.GetType().FullName + " due to a already inuse serializer key.");

			return this.SerializerRegister.Register(Serializer<T>.Instance, Serializer<T>.Instance.SerializerUniqueKey);
		}

		public bool RegisterProtobufPacket(Type t)
		{
			return Packet.Register(t, false);
		}

		/// <summary>
		/// Called internally when the application starts up. The implementing class should register all the custom Protobuf-net (The default serializer for the Packet base class)
		/// packet classes so that the serializer is aware of them for both sending and recieving the packages. All internal packages that aren't written by the implementing library are handled
		/// internally.
		/// </summary>
		/// <param name="registerAsDefaultFunc">The defauly packet registeration function.</param>
		protected abstract void RegisterProtobufPackets(Func<Type, bool> registerAsDefaultFunc);

		//(No longer internal due to Unity3D Requirements) This is internal because we don't want child classes having access to it but we need some derived classes to have access.
		protected T GeneratePackage<T>(LidgrenTransferPacket packet, EncryptionBase decrypter)
			where T : NetworkPackage, new()
		{
			if (SerializerRegister.GetValue(packet.SerializerKey) == null)
				throw new LoggableException("Packet serializer not found with get.", null, Logger.LogType.Error);

			return Converter.BuildIncomingNetPackage<T>(packet, SerializerRegister.GetValue(packet.SerializerKey), decrypter);
		}

		//(No longer internal due to Unity3D Requirements) This is internal because we don't want child classes having access to it but we need some derived classes to have access.
		protected T GeneratePackage<T>(LidgrenTransferPacket packet)
			where T : NetworkPackage, new()
		{
			if (SerializerRegister.GetValue(packet.SerializerKey) == null)
				throw new LoggableException("Packet serializer not found with get.", null, Logger.LogType.Error);

			return Converter.BuildIncomingNetPackage<T>(packet, SerializerRegister.GetValue(packet.SerializerKey));
		}
	}
}

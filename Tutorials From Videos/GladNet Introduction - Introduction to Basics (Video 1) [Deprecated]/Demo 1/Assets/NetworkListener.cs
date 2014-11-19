using UnityEngine;
using System.Collections;
using GladNet.Client;
using GladNet.Common;
using System;
using ProtoBuf.Meta;
using System.Linq;

public class NetworkListener : MonoBehaviour, IListener 
{
	public string IPAddress;
	public int Port;
	public string ApplicationName;
	public string HailMessage;

	GladNetPeer peer;

	void Start () 
	{
		peer = new GladNetPeer(this);
		peer.Connect(IPAddress, Port, HailMessage, ApplicationName);
		peer.StartListener();
		StartCoroutine(PollerMethod());
	}

	IEnumerator PollerMethod()
	{
		WaitForSeconds waitTime = new WaitForSeconds(0.1f);

		while(peer.Poll())
		{
			yield return waitTime;
		}
	}

	public void OnStatusChange(StatusChange status)
	{
		Debug.Log(status.ToString());

		switch(status)
		{
			case StatusChange.Connected:
				peer.SendRequest(new MessagePacket("Hello, this is a message.", "GladNetUser"), 5, Packet.DeliveryMethod.ReliableUnordered);
				break;
		}
	}

	public void RecievePackage(ResponsePackage responsePackage)
	{
		switch(responsePackage.Code)
		{
			case 5:
				var response = (ResponsePacket)responsePackage.PacketObject;
				Debug.Log(response.Response);
				break;
		}
	}

	public void RecievePackage(EventPackage eventPackage)
	{
		
	}

	public void RegisterProtobufPackets(Func<System.Type, bool> registerMethod)
	{
		registerMethod(typeof(MessagePacket));
		registerMethod(typeof(ResponsePacket));
	}
}

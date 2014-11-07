#pragma once
#include "INetworkPeerInterface.h"
#include <RakPeerInterface.h>
#include <cstdint>

class ClientPeer : public INetworkPeerInterface
{
public:
	ClientPeer() 
		: internalPeer(RakNet::RakPeerInterface::GetInstance()), internalSD(nullptr)
	{

	}

	//Provides access to a RakNet defined 64bit GUID of the client. This is only defined after connection.
	virtual uint64_t GetConnectionGUID() const override;

	//Allows the user to set the maximum number of connections possible involving this peer and set the internal listener.
	virtual void InitPeer(const IPeerListener* const listenerObj, uint32_t maximumSimultaneousConnections = 1) override;
	virtual bool ConnectToServer(const std::string& endPointIP, uint16_t port, const std::string& applicationName) override;
	virtual void Disconnect() override;

	virtual void Poll() override;

	~ClientPeer();

private:
	RakNet::RakPeerInterface* internalPeer;
	const IPeerListener* internalListener;
	RakNet::SocketDescriptor* internalSD;
};
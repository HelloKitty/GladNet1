#pragma once
#include <string>
#include "IPeerListener.h"
#include <cstdint>

class INetworkPeerInterface
{
public:
	//Accessor and Mutators
	virtual uint64_t GetConnectionGUID() const = 0;

	virtual void InitPeer(const IPeerListener* const listenerObj, uint32_t  maximumSimultaneousConnections = 1) = 0;
	virtual bool ConnectToServer(const std::string& endPointIP, uint16_t port, const std::string& applicationName) = 0;
	virtual void Disconnect() = 0;

	virtual void Poll() = 0;
};
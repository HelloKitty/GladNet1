#pragma once
#include "FClientPeer.h"
#include <stdexcept>

void ClientPeer::Poll()
{

}

//Provides access to a RakNet defined 64bit GUID of the client. This is only defined after connection.
uint64_t ClientPeer::GetConnectionGUID() const
{

}

//Allows the user to set the maximum number of connections possible involving this peer and set the internal listener.
//Must be called before connecting
void ClientPeer::InitPeer(const IPeerListener* const listenerObj, uint32_t maximumSimultaneousConnections)
{
	//Not threadsafe; can't use std::mutex because of UE4. Possibly implement custom mutex in the future.
	if (this->internalSD != nullptr)
		delete this->internalSD;

	internalSD = new RakNet::SocketDescriptor();

	//Don't delete the listener, we don't own it.
	this->internalListener = listenerObj;

	this->internalPeer->Startup(maximumSimultaneousConnections, this->internalSD, 1);
}

bool ClientPeer::ConnectToServer(const std::string& endPointIP, uint16_t port, const std::string& applicationName)
{
	//TODO: Better exception system.
	if (endPointIP.length() == 0)
		throw runtime_error("Cannot connect to a host with no endpoint.");

	if (applicationName.length() == 0)
		throw runtime_error("Cannot connect to a server with no application name.");

	//Attempts to make a general connection to the remote IP specified.
	RakNet::ConnectionAttemptResult result = this->internalPeer->Connect(endPointIP.c_str(), port, applicationName.c_str(), applicationName.length() + 1);

	return result == RakNet::ConnectionAttemptResult::CONNECTION_ATTEMPT_STARTED;
}

void ClientPeer::Disconnect()
{

}
#pragma once
#include <cstdint>

enum class StatusChange : uint8_t
{
	Connected = 0,
	Connecting = 1,
	Disconnected = 2,
	FailedToConnect = 3
};
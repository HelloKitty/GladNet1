#pragma once
#include "StatusChange.h"
#include "FEventPackage.h"
#include "FResponsePackage.h"

class IPeerListener
{
public:
	virtual void OnStatusChange(const StatusChange change) = 0;
	virtual void OnEventRecieve(const EventPackage& ePackage) = 0;
	virtual void OnResponseRecieve(const ResponsePackage& rPackage) = 0;
};
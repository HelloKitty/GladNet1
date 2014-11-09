﻿using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Client
{
	public interface IListener
	{
		void RecievePackage(EventPackage eventPackage);
		void RecievePackage(ResponsePackage responsePackage);

		void OnStatusChange(StatusChange status);
	}
}
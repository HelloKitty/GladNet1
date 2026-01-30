#region copyright
/// GladNet Copyright (C) 2014 X 
/// X
/// GitHub: HeIIoKitty
/// Unity3D: Glader
/// Please refer to the repo License file for licensing information
/// If this source code has been distributed without a copy of the original license file then this is an illegal copy and you should delete it
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GladNet.Server;
using GladNet.Common;
using GladNet.Server.Common;

namespace GladNet.Server.App.Main
{
	internal class Driver
	{
		//Args
		//DLLNAME APPNAME HAILMESSAGE PORT PIPEHANDLE
		static void Main(string[] args)
		{
			

			AssemblyInteropObj<object> serverObject;

			if(args[0] != null)
				serverObject = ServerInstanceFromConfig(args);
			else
				serverObject = ServerInstanceFromConsole();


			if (args[4] != null)
				serverObject.ExecuteMethod("StartPipeListener", new object[1] { args[4] });

			serverObject.ExecuteMethod("InternalOnStartup");
			serverObject.ExecuteMethod("Poll");
			//If we hit this point the server is outside of its execution loop so we need to shut it down.
			serverObject.ExecuteMethod("InternalOnShutdown");

			Console.ReadKey();
		}

		private static AssemblyInteropObj<object> ServerInstanceFromConsole()
		{
			Console.WriteLine("GladNet Server");
			Console.Write("Enter the name of server assembly: ");
			string assName = ReadAssemblyName();
			Console.WriteLine("\n");

			//try loading the assembly
			Assembly ass = LoadServerAssembly(assName);


			Type coreType = TryGetCoreType(ass);

			if (coreType == null)
				PublishError("Failed to load type: returned null", true, Console.WriteLine, () => { Console.ReadKey(); });

			return new AssemblyInteropObj<object>(Activator.CreateInstance(coreType), false);
		}

		//DLLNAME APPNAME HAILMESSAGE PORT PIPEHANDLE
		private static AssemblyInteropObj<object> ServerInstanceFromConfig(string[] args)
		{
			//try loading the assembly
			Assembly ass = LoadServerAssembly(args[0]);
			Type coreType = TryGetCoreType(ass);

			if (coreType == null)
				PublishError("Failed to load type: returned null", true, Console.WriteLine, () => { Console.ReadKey(); });

			int result;
			int.TryParse(args[3], out result);

			return new AssemblyInteropObj<object>(Activator.CreateInstance(coreType
				, args[1], args[2], result), false);
		}

		static void StartServerFromConfig(string xmlConfig)
		{
			
		}

		static string ReadAssemblyName()
		{
			return Console.ReadLine();
		}

		static Assembly LoadServerAssembly(string name)
		{
			try
			{
				return Assembly.UnsafeLoadFrom(name);
			}
			catch(Exception e)
			{
				PublishError("Failed to load assembly. Error: " + e.Message, true, 
					Console.WriteLine, () => { Console.ReadKey(); });
				throw;
			}
		}
		
		static Type TryGetCoreType(Assembly ass)
		{
			int countFound = 0;
			Type coreType = null;
			try
			{
				foreach (Type t in ass.GetTypes())
				{
					if(t.BaseType != null && t.BaseType.CustomAttributes != null)// && t.Name != typeof(ServerCore<>).Name)
						if (t.BaseType.CustomAttributes.Where(x => x.AttributeType.Name.Contains("CoreAttribute")/*x.AttributeType.Name.Contains("Core") || x.AttributeType.Name.Contains("CoreAttribute")*/).Count() > 0)
						{
							coreType = t;
							countFound++;
						}
				}

				switch(countFound)
				{
					case 0:
						return null;
					case 1:
						return coreType;
					default:
						PublishError("Multiple ServerCores found in the assembly. Only one can exist.", true,
					Console.WriteLine, () => { Console.ReadKey(); });
						return null;
				}
			}
			catch (Exception e)
			{
				PublishError("Failed to get core from assembly. Error: " + e.Message + e.Data, true, 
					Console.WriteLine, () => { Console.ReadKey(); });
				throw;
			}
		}

		static void PublishError(string errorMessage, bool shouldExit, 
			Action<string> errorReportMethod, Action pauseMethod)
		{
			errorReportMethod(errorMessage);
			pauseMethod.Invoke();
			pauseMethod.Invoke();

			if (shouldExit)
				Environment.Exit(1);
		}
	}
}

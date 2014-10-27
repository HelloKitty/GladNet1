using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GladNet.Server;
using AutoMapper;
using GladNet.Common;

namespace GladNet.Server.App.Main
{
	internal class Driver
	{
		static void Main(string[] args)
		{
			Console.WriteLine("GladNet Server");
			Console.Write("Enter the name of server assembly: ");
			string assName = ReadAssemblyName();
			Console.WriteLine("\n");

			//try loading the assembly
			Assembly ass = LoadServerAssembly(assName);

			Type coreType = TryGetCoreType(ass);

			if (coreType != null)
			{
				AssemblyInteropObj<object> serverObject = 
					new AssemblyInteropObj<object>(Activator.CreateInstance(coreType, "hi"), false);

				serverObject.ExecuteMethod("InternalOnStartup");
				serverObject.ExecuteMethod("Poll");
			}
			else
				PublishError("Failed to load type: returned null", true, Console.WriteLine, () => { Console.ReadKey(); });

			Console.ReadKey();
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
				//return Assembly.LoadFile(Environment.CurrentDirectory + @"\" + name);
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
						//For some reason we can't expect the types to compare properly. Just use names
						if (t.BaseType.CustomAttributes.Where(x => x.AttributeType.Name.Contains("CoreAttribute")/*x.AttributeType.Name.Contains("Core") || x.AttributeType.Name.Contains("CoreAttribute")*/).Count() > 0)
						{
							//Console.WriteLine("Looking at type");
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

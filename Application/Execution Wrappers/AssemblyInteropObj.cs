#region copyright
/// GladNet Copyright (C) 2014 Andrew Blakely 
/// andrew.blakely@ymail.com
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

namespace GladNet.Common
{
	public class AssemblyInteropObj<ObjectType> where ObjectType : class
	{
		private readonly object _source;

		public AssemblyInteropObj(object source, bool verifyInterop = true)
		{
			_source = source;

			//Console.WriteLine("Source:");
			//foreach (var m in _source.GetType().GetMembers(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod))
			//	Console.WriteLine(m.Name);

			//Console.ReadKey();

			/*Console.WriteLine("TypeParam:");
			foreach (var m in typeof(ObjectType).GetMembers(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic))
				Console.WriteLine(m.Name);*/

			if (verifyInterop)
				//verify interopability
				foreach (var member in _source.GetType().GetMembers(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (typeof(ObjectType).GetMembers(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic)
						.Where(x => x.Name == member.Name).Count() == 0)
							throw new Exception("AssemblyInteropObj<" + typeof(ObjectType).Name + "> has a member named " + member.Name + " that source type does not.");
				}
		}

		public object ExecuteMethod(string methodName, object[] parameters = null)
		{
			try
			{
				if (parameters == null)
					return _source.GetType().InvokeMember(methodName, 
						BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
						null, _source, new object[0]);
					//return _source.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic).Invoke(_source, new object[0]);
				else
					return _source.GetType().InvokeMember(methodName,
						BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
						null, _source, parameters);
			}
			//Gotta catch 'em all. Waaaay too many could potentially occur.
			catch(Exception e)
			{
				//throw new Exception("Failed to invoke " + methodName + " on underlying source object. Check signature.");
				throw;
			}
		}

		public object AccessMember(string memberName)
		{
			return _source.GetType().GetField(memberName, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField).GetValue(_source);
		}

	}
}

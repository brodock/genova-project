using System;
using System.Collections;
using Server;

namespace Server.Mobiles
{
	public class ODBCSpawnerType
	{
		public static Type GetType( string name )
		{
			return ScriptCompiler.FindTypeByName( name );
		}
	}
}
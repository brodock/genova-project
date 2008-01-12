// The procedures used in this file were copied from RunUO B36's distro. The purpose for
// copying these methods from the distro were in the event the distro changes, The scripts
// read from this file and will not have to be modified if necessary. Plus, some modifications
// were necessary to some of the distro methods.

using System;
using System.Collections;
using System.Reflection;
using Server;
using CPA = Server.CommandPropertyAttribute;

namespace Server.MegaSpawnerSystem
{
	public class DistroMatch
	{
		private static Type typeofItem = typeof( Item ), typeofMobile = typeof( Mobile );

		private static void Match( string match, Type[] types, ArrayList results )
		{
			match = match.ToLower();

			for ( int i = 0; i < types.Length; ++i )
			{
				Type t = types[i];

				if ( (typeofMobile.IsAssignableFrom( t ) || typeofItem.IsAssignableFrom( t )) && t.Name.ToLower().IndexOf( match ) >= 0 && !results.Contains( t ) )
				{
					ConstructorInfo[] ctors = t.GetConstructors();

					for ( int j = 0; j < ctors.Length; ++j )
					{
						if ( ctors[j].GetParameters().Length == 0 && ctors[j].IsDefined( typeof( ConstructableAttribute ), false ) )
						{
							results.Add( t );
							break;
						}
					}
				}
			}
		}

		public static ArrayList Match( string match )
		{
			ArrayList results = new ArrayList();
			Type[] types;

			Assembly[] asms = ScriptCompiler.Assemblies;

			for ( int i = 0; i < asms.Length; ++i )
			{
				types = ScriptCompiler.GetTypeCache( asms[i] ).Types;
				Match( match, types, results );
			}

			types = ScriptCompiler.GetTypeCache( Core.Assembly ).Types;
			Match( match, types, results );

			results.Sort( new TypeNameComparer() );

			return results;
		}

		private class TypeNameComparer : IComparer
		{
			public int Compare( object x, object y )
			{
				Type a = x as Type;
				Type b = y as Type;

				return a.Name.CompareTo( b.Name );
			}
		}
	}
}
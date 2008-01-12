//By Meta Ridley
using System;
using Server;
using Server.Items;
using Server.Engines.CannedEvil;
using Server.Network;

namespace Server.Mobiles
{
	public class Harmengorf : BaseChampion
	{
		public override ChampionSkullType SkullType{ get{ return ChampionSkullType.Death; } }

		[Constructable]
		public Harmengorf() : base( AIType.AI_Melee )
		{
			Name = "Harmengorf";
			Title = "o Mestre Demonio";
			Body = 38;

			SetStr( 305, 425 );
			SetDex( 72, 150 );
			SetInt( 505, 750 );

			SetHits( 6800 );
			SetStam( 102, 300 );

			SetDamage( 25, 35 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 60, 70 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 97.6, 100.0 );
			SetSkill( SkillName.Wrestling, 97.6, 100.0 );

			Fame = 22500;
			Karma = -22500;

			VirtualArmor = 70;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 5 );
			AddLoot( LootPack.FilthyRich );
		}

		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool BardImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }

		public override bool ShowFameTitle{ get{ return false; } }
		public override bool ClickTitle{ get{ return false; } }


		public void SpawnImps( Mobile target )
		{
			Map map = this.Map;

			if ( map == null )
				return;

			int imps = 0;

			foreach ( Mobile m in this.GetMobilesInRange( 10 ) )
			{
				if ( m is Imp )
					++imps;
			}

			if ( imps < 16 )
			{

				int newImps = Utility.RandomMinMax( 3, 6 );

				for ( int i = 0; i < newImps; ++i )
				{
					BaseCreature imp;

					switch ( Utility.Random( 5 ) )
					{
						default:
						case 0: case 1:	imp = new Imp(); break;

					}

					imp.Team = this.Team;

					bool validLocation = false;
					Point3D loc = this.Location;

					for ( int j = 0; !validLocation && j < 10; ++j )
					{
						int x = X + Utility.Random( 3 ) - 1;
						int y = Y + Utility.Random( 3 ) - 1;
						int z = map.GetAverageZ( x, y );

						if ( validLocation = map.CanFit( x, y, this.Z, 16, false, false ) )
							loc = new Point3D( x, y, Z );
						else if ( validLocation = map.CanFit( x, y, z, 16, false, false ) )
							loc = new Point3D( x, y, z );
					}

					imp.MoveToWorld( loc, map );
					imp.Combatant = target;
				}
			}
		}

		public void DoSpecialAbility( Mobile target )
		{
				SpawnImps( target );
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );
			if ( attacker is BaseCreature && (((BaseCreature)attacker).Controlled || ((BaseCreature)attacker).Summoned))
			{
				DoSpecialAbility( attacker );
			}
		}

		public Harmengorf( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class Travesty : BasePeerless
	{
		[Constructable]
		public Travesty() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a travesty";
			Body = 0x108;

			SetStr( 909, 949 );
			SetDex( 901, 948 );
			SetInt( 903, 947 );

			SetHits( 35000 );

			SetDamage( 25, 30 );
			
			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 52, 67 );
			SetResistance( ResistanceType.Fire, 51, 68 );
			SetResistance( ResistanceType.Cold, 51, 69 );
			SetResistance( ResistanceType.Poison, 51, 70 );
			SetResistance( ResistanceType.Energy, 50, 68 );

			SetSkill( SkillName.Wrestling, 100.1, 119.7 );
			SetSkill( SkillName.Tactics, 102.3, 118.5 );
			SetSkill( SkillName.MagicResist, 101.2, 119.6 );
			SetSkill( SkillName.Anatomy, 100.1, 117.5 );
			
			PackTalismans( 5 );
			PackResources( 8 );
		}

		public Travesty( Serial serial ) : base( serial )
		{
		}
		
		public override void OnDeath( Container c )
		{
			base.OnDeath( c );	
			
			c.DropItem( new EyeOfTheTravesty() );
			c.DropItem( new OrdersFromMinax() );
			
			switch ( Utility.Random( 3 ) )
			{
				case 0: c.DropItem( new TravestysSushiPreparations() ); break;
				case 1: c.DropItem( new TravestysFineTeakwoodTray() ); break;
				case 2: c.DropItem( new TravestysCollectionOfShells() ); break;
			}
			
			if ( Utility.RandomDouble() < 0.6 )				
				c.DropItem( new ParrotItem() );
			
			if ( Utility.RandomDouble() < 0.1 )
				c.DropItem( new TragicRemainsOfTravesty() );
				
			if ( Utility.RandomDouble() < 0.05 )
				c.DropItem( new ImprisonedDog() );
				
			if ( Utility.RandomDouble() < 0.05 )
				c.DropItem( new MarkOfTravesty() );
				
			if ( Utility.RandomDouble() < 0.025 )
				c.DropItem( new CrimsonCincture() );
		}
		
		public override bool CanAnimateDead{ get{ return true; } }
		public override BaseCreature Animates{ get{ return new LichLord(); } }
		public override bool GivesMinorArtifact{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 5; } }
		
		private bool m_SpawnedHelpers;

		public override void OnThink()
		{
			base.OnThink();
			
			if ( Combatant == null )
				return;			
				
			if ( Combatant.Player && Name != Combatant.Name )
				Morph();	
				
			if ( Hits < HitsMax * 0.01 && !m_SpawnedHelpers )
				Spawn();
				
			if ( Hits > HitsMax * 0.015 )
				m_SpawnedHelpers = false;
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosSuperBoss, 8 );
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
		
		private List<Item> m_Items;
		private Timer m_Timer;
		
		public virtual void Morph()
		{			
			Body = Combatant.Body; 
			Hue = Combatant.Hue; 
			Name = Combatant.Name;
			Title = Combatant.Title;
			Female = Combatant.Female;
			
			Item nItem;
			
			m_Items = new List<Item>();
  				
			foreach ( Item item in Combatant.Items )
			{
				if ( item.Layer != Layer.Backpack && item.Layer != Layer.Mount )
				{
					nItem = CloneItem( item ); 
					AddItem( nItem );
					m_Items.Add( nItem );
				}
			}
			
			PlaySound( 0x511 );
			FixedParticles( 0x376A, 1, 14, 5045, EffectLayer.Waist );
			
			if ( m_Timer != null )
				m_Timer.Stop();
				
			FixedParticles( 0x376A, 1, 14, 0x13B5, EffectLayer.Waist );
			PlaySound( 0x511 );

			
			Timer.DelayCall( TimeSpan.FromSeconds( 2 ), TimeSpan.FromSeconds( 2 ), 3, new TimerCallback( Clone ) );				
			m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 5 ), TimeSpan.FromSeconds( 5 ), new TimerCallback( EndMorph ) );
		}

		public virtual Item CloneItem( Item item )
		{
			Item newItem = new Item( item.ItemID );
			
			newItem.Name = item.Name;			
			newItem.Hue = item.Hue;
			newItem.Layer = item.Layer;

			return newItem;
		}
		
		public virtual void Clone()
		{
			new Clone( this ).MoveToWorld( GetSpawnPosition( 2 ), Map );
		}
		
		public virtual void EndMorph()
		{
			if ( Combatant != null && Name == Combatant.Name )
				return;
		
			if ( m_Items != null )
			{
				for ( int i = m_Items.Count - 1; i >= 0; i-- )
				{
					Item item = m_Items[ i ];
					
					if ( item != null )
						item.Delete();
				}
			}
			
			if ( m_Timer != null )
				m_Timer.Stop();			
				
			if ( Combatant != null )
			{
				Morph();
				
				return;
			}
				
			m_Timer = null;
			
			Title = null;
			Name = "a travesty";
			Body = 0x108;
			Hue = 0;
			
			PlaySound( 0x511 );
			FixedParticles( 0x376A, 1, 14, 5045, EffectLayer.Waist );
		}
		
		public virtual void Spawn()
		{
			m_SpawnedHelpers = true;
			
			for ( int i = 0; i < 10; i ++ )
				switch ( Utility.Random( 3 ) )
				{
					case 0: new DragonsFlameMage().MoveToWorld( GetSpawnPosition( 10 ), Map ); break;
					case 1: new SerpentsFangAssassin().MoveToWorld( GetSpawnPosition( 10 ), Map ); break;
					case 2: new TigersClawThief().MoveToWorld( GetSpawnPosition( 10 ), Map ); break;
				}
		}
	}
}

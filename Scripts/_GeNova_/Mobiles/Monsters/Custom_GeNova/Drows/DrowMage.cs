using System;
using System.Collections;
using Server;
using Server.Misc;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    [CorpseName("a glowing drow corpse")]
    public class DrowMage : BaseCreature
    {
        [Constructable]
        public DrowMage()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            SpeechHue = Utility.RandomDyedHue();
            Title = "Drow Mago";
            Hue = 902;

            if (this.Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
            }

            SetStr(326, 545);
            SetDex(91, 510);
            SetInt(161, 585);

            SetHits(222, 308);
            SetDamage(23, 46);

            SetDamageType(ResistanceType.Physical, 30);

            SetResistance(ResistanceType.Physical, 30, 80);
            SetResistance(ResistanceType.Fire, 20, 80);
            SetResistance(ResistanceType.Cold, 20, 80);
            SetResistance(ResistanceType.Poison, 20, 80);
            SetResistance(ResistanceType.Energy, 40, 80);

            SetSkill(SkillName.EvalInt, 97.5, 150.0);
            SetSkill(SkillName.Magery, 92.5, 135.0);
            SetSkill(SkillName.Meditation, 97.5, 300.0);
            SetSkill(SkillName.MagicResist, 97.5, 130.0);
            SetSkill(SkillName.Tactics, 92.5, 135.0);
            SetSkill(SkillName.Wrestling, 92.5, 135.0);

            Fame = 1000;
            Karma = -1000;

            Item robe = new Robe();
            robe.Hue = 1107;
            AddItem(robe);

            Item hair = new Item(Utility.RandomList(0x203B, 0x2049, 0x2048, 0x204A));
            hair.Hue = 1153;
            hair.Layer = Layer.Hair;
            hair.Movable = false;

            AddItem(hair);

            PackGold(770, 1770);
            PackReg(10, 15);
            PackArmor(2, 5, 0.8);
            PackWeapon(3, 5, 0.8);
            PackSlayer();
            //PackItem( new Bandage( Utility.RandomMinMax( 1, 15 ) ) );

            if (0.1 > Utility.RandomDouble())
                PackItem(new TribalBerry());

            PackNecroScroll(3); // Curse Weapon
            PackNecroScroll(1); // Blood Oath
            PackNecroScroll(10); // Strangle

            AddItem(new Boots());
            //AddItem(new DrowRingmailGloves());
            //AddItem(new DrowChainChest());
            //AddItem(new DrowChainLegs());
            //AddItem(new Piwafwi());
        }

        public void AddArcane(Item item)
        {
            if (item is IArcaneEquip)
            {
                IArcaneEquip eq = (IArcaneEquip)item;
                eq.CurArcaneCharges = eq.MaxArcaneCharges = 20;
            }

            item.Hue = ArcaneGem.DefaultArcaneHue;
            item.LootType = LootType.Newbied;
            AddItem(item);
        }

        public override int Meat { get { return 1; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool ShowFameTitle { get { return false; } }
        public override bool CanRummageCorpses { get { return true; } }

        public override OppositionGroup OppositionGroup
        {
            get { return OppositionGroup.SavagesAndOrcs; }
        }

        public override bool IsEnemy(Mobile m)
        {
            if (m.BodyMod == 183 || m.BodyMod == 184)
                return false;

            return base.IsEnemy(m);
        }

        public override void AggressiveAction(Mobile aggressor, bool criminal)
        {
            base.AggressiveAction(aggressor, criminal);
            if (aggressor.BodyMod == 183 || aggressor.BodyMod == 184)
            {
                AOS.Damage(aggressor, 50, 0, 100, 0, 0, 0);
                aggressor.BodyMod = 0;
                aggressor.HueMod = -1;
                aggressor.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
                aggressor.PlaySound(0x307);
                aggressor.SendLocalizedMessage(1040008); // Your skin is scorched as the tribal paint burns away!

                if (aggressor is PlayerMobile)
                    ((PlayerMobile)aggressor).SavagePaintExpiration = TimeSpan.Zero;
            }
        }

        public override void AlterMeleeDamageTo(Mobile to, ref int damage)
        {
            if (to is Dragon || to is WhiteWyrm || to is SwampDragon || to is Drake || to is Nightmare || to is Daemon)
                damage *= 5;
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (0.1 > Utility.RandomDouble())
                BeginSavageDance();
        }



        public void BeginSavageDance()
        {
            ArrayList list = new ArrayList();

            foreach (Mobile m in this.GetMobilesInRange(8))
            {
                if (m != this && m is SavageShaman)
                    list.Add(m);
            }

            Animate(111, 5, 1, true, false, 0); // Do a little dance...

            if (AIObject != null)
                AIObject.NextMove = DateTime.Now + TimeSpan.FromSeconds(1.0);

            if (list.Count >= 3)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    SavageShaman dancer = (SavageShaman)list[i];

                    dancer.Animate(111, 5, 1, true, false, 0); // Get down tonight...

                    if (dancer.AIObject != null)
                        dancer.AIObject.NextMove = DateTime.Now + TimeSpan.FromSeconds(1.0);
                }

                Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerCallback(EndSavageDance));
            }
        }

        public void EndSavageDance()
        {
            if (Deleted)
                return;

            ArrayList list = new ArrayList();

            foreach (Mobile m in this.GetMobilesInRange(8))
                list.Add(m);

            if (list.Count > 0)
            {
                switch (Utility.Random(3))
                {
                    case 0: /* greater heal */
                        {
                            foreach (Mobile m in list)
                            {
                                bool isFriendly = (m is Savage || m is SavageRider || m is SavageShaman || m is SavageRidgeback);

                                if (!isFriendly)
                                    continue;

                                if (m.Poisoned || MortalStrike.IsWounded(m) || !CanBeBeneficial(m))
                                    continue;

                                DoBeneficial(m);

                                // Algorithm: (40% of magery) + (1-10)

                                int toHeal = (int)(Skills[SkillName.Magery].Value * 0.4);
                                toHeal += Utility.Random(1, 10);

                                m.Heal(toHeal);
                                m.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
                                m.PlaySound(0x202);
                            }
                            break;
                        }
                    case 1: /* lightning */
                        {
                            foreach (Mobile m in list)
                            {
                                bool isFriendly = (m is Savage || m is SavageRider || m is SavageShaman || m is SavageRidgeback);

                                if (isFriendly)
                                    continue;

                                if (!CanBeHarmful(m))
                                    continue;

                                DoHarmful(m);

                                double damage;

                                if (Core.AOS)
                                {
                                    int baseDamage = 6 + (int)(Skills[SkillName.EvalInt].Value / 5.0);
                                    damage = Utility.RandomMinMax(baseDamage, baseDamage + 3);
                                }
                                else
                                    damage = Utility.Random(12, 9);

                                m.BoltEffect(0);

                                SpellHelper.Damage(TimeSpan.FromSeconds(0.25), m, this, damage, 0, 0, 0, 0, 100);
                            }

                            break;
                        }
                    case 2: /* poison */
                        {
                            foreach (Mobile m in list)
                            {
                                bool isFriendly = (m is Savage || m is SavageRider || m is SavageShaman || m is SavageRidgeback);

                                if (isFriendly)
                                    continue;

                                if (!CanBeHarmful(m))
                                    continue;

                                DoHarmful(m);

                                if (m.Spell != null)
                                    m.Spell.OnCasterHurt();

                                m.Paralyzed = false;

                                double total = Skills[SkillName.Magery].Value + Skills[SkillName.Poisoning].Value;

                                double dist = GetDistanceToSqrt(m);

                                if (dist >= 3.0)
                                    total -= (dist - 3.0) * 10.0;

                                int level;

                                if (total >= 200.0 && Utility.Random(1, 100) <= 10)
                                    level = 3;
                                else if (total > 170.0)
                                    level = 2;
                                else if (total > 130.0)
                                    level = 1;
                                else
                                    level = 0;

                                m.ApplyPoison(this, Poison.GetPoison(level));
                                m.FixedParticles(0x374A, 10, 15, 5021, EffectLayer.Waist);
                                m.PlaySound(0x474);
                            }
                            break;
                        }
                }
            }
        }

        public DrowMage(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
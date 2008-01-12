/***************************************************************************
 *   copyright            : (C) GeNova Project
 *   webSite              : http://code.google.com/p/genovaproject/
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using Server.Network;
using Server;
using Server.Gumps;
using Server.Mobiles;

namespace GeNova.Server.Engines
{
    public class FoodDecay : Timer
    {
        public FoodDecay()
            : base(TimeSpan.FromMinutes(40.0), TimeSpan.FromMinutes(40.0))
        {
            Priority = TimerPriority.OneMinute;
        }

        public static void Initialize()
        {
            new FoodDecay().Start();
        }
        public static void IniciarThread()
        {
            System.Threading.ThreadStart threadStart = new System.Threading.ThreadStart(FoodDecayProcessamento.Processar);
            new System.Threading.Thread(threadStart).Start();
        }

        protected override void OnTick()
        {
            IniciarThread();
        }
    }

    public abstract class FoodDecayProcessamento
    {
        public static void Processar()
        {
            foreach (NetState state in NetState.Instances)
            {
                HungerDecay(state.Mobile);
                ThirstDecay(state.Mobile);
            }
        }
        private static void HungerDecay(Mobile jogador)
        {
            int dano = 0;
            if (jogador != null)
            {
                if (jogador.Alive && jogador.Hunger >= 1 && jogador.AccessLevel == AccessLevel.Player)
                {
                    jogador.Hunger -= 1;
                    if (jogador.Hunger <= 20 && jogador.Hunger > 10)
                        jogador.SendMessage(70, "Você está bem alimentado!");
                    if (jogador.Hunger <= 10 && jogador.Hunger > 5)
                        jogador.SendMessage(88, "Você está razoavelmente bem alimentado, ainda não sente fome.");
                    else if (jogador.Hunger <= 5 && jogador.Hunger > 2)
                    {
                        jogador.SendMessage(40, "Você começa a sentir muita fome. Está enfraquecendo!");
                        dano = 3;
                        jogador.Hits -= (jogador.Hits / dano);
                        jogador.Stam -= (jogador.Stam / dano);
                        jogador.Mana -= (jogador.Mana / dano);
                    }
                    else if (jogador.Hunger <= 2 && jogador.Hunger > 0)
                    {
                        jogador.SendMessage(38, "Você se sente fraco por estar faminto, precisa se alimentar!");
                        dano = 2;
                        jogador.Hits -= (jogador.Hits / dano);
                        jogador.Stam -= (jogador.Stam / dano);
                        jogador.Mana -= (jogador.Mana / dano);
                    }
                }
                else if (jogador.Alive && jogador.AccessLevel == AccessLevel.Player && jogador.Hunger == 0)
                {
                    jogador.SendMessage(38, "Você se sente muito fraco, quase morrendo de fome!");
                    dano = 2;
                    jogador.Hits -= ((jogador.Hits / dano) + 3);
                    jogador.Stam = 10;
                    jogador.Mana = 0;
                }
            }
        }
        private static void ThirstDecay(Mobile jogador)
        {
            int dano = 0;
            if (jogador != null)
            {
                if (jogador.Alive && jogador.Thirst >= 1 && jogador.AccessLevel == AccessLevel.Player)
                {
                    jogador.Thirst -= 1;
                    if (jogador.Thirst <= 20 && jogador.Thirst > 10)
                        jogador.SendMessage(70, "Sua sede está saciada!");
                    if (jogador.Thirst <= 10 && jogador.Thirst > 5)
                        jogador.SendMessage(88, "Você sente sua garganta um tanto seca!");
                    else if (jogador.Thirst <= 5 && jogador.Thirst > 2)
                    {
                        jogador.SendMessage(40, "Você começa a sentir muita sede!");
                        dano = 3;
                        jogador.Hits -= (jogador.Hits / dano);
                        jogador.Stam -= (jogador.Stam / dano);
                        jogador.Mana -= (jogador.Mana / dano);
                    }
                    else if (jogador.Thirst <= 2 && jogador.Thirst > 0)
                    {
                        jogador.SendMessage(38, "Você se sente fraco! Precisar beber alguma coisa!");
                        dano = 2;
                        jogador.Hits -= (jogador.Hits / dano);
                        jogador.Stam -= (jogador.Stam / dano);
                        jogador.Mana -= (jogador.Mana / dano);
                    }
                }
                else if (jogador.Alive && jogador.AccessLevel == AccessLevel.Player && jogador.Thirst == 0)
                {
                    jogador.SendMessage(38, "Você se sente muito fraco, quase morrendo de sede!");
                    dano = 2;
                    jogador.Hits -= ((jogador.Hits / dano) + 3);
                    jogador.Stam = 10;
                    jogador.Mana = 0;
                }
            }
        }
    }
}

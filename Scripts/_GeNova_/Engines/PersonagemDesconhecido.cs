using System;
using System.Collections.Generic;
using System.Text;
using Server;
using GeNova.Server.Variados;

namespace GeNova.Server.Engines
{
    public abstract class PersonagemDesconhecido
    {
        public static readonly string NomePadrao = "Desconhecido";

        public static void VerificarComRestricoes(Mobile personagem)
        {
            if (personagem.Name.Equals(NomePadrao))
            {
                personagem.Frozen = true;
                personagem.MoveToWorld(PosicionamentoNoMapa.BritainCentro(), Map.Felucca);
                personagem.Hidden = true;                
            }
        }
        public static void SetarParaNomeDesconhecido(Mobile personagem)
        {
            personagem.Name = NomePadrao;
        }

        public static bool Confirmar(Mobile personagem)
        {
            return personagem.Name.Equals(NomePadrao);
        }
    }
}

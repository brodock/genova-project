using System;
using System.Collections.Generic;
using System.Text;
using Server;
using GeNova.Server.Variados;
using Server.Items;

namespace GeNova.Server.Engines
{
    public class PersonagemEmMundoRestrito
    {
        public static readonly bool Ativo = true;
        public static readonly PMList[] GeNovaLists = new PMList[] { PMList.Felucca };
        public static readonly string Mensagem = "Você está tentando acessar um mundo restrito.";

        public static void VerificarComRestricoes(Mobile personagem)
        {
            if (Confirmar(personagem))
            {
                personagem.MoveToWorld(PosicionamentoNoMapa.BritainCentro(), Map.Felucca);
                personagem.SendMessage("Você estava em um mundo restrito e fora movido para o ponto inicial.");
            }
        }

        public static bool Confirmar(Mobile personagem)
        {
            return personagem.Map != Map.Felucca;
        }
    }
}

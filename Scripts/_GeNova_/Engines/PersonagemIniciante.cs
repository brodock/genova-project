using System;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Mobiles;

namespace GeNova.Server.Engines
{
    public abstract class PersonagemIniciante
    {
        public static void VerificarComRestricoes(Mobile personagem)
        {
            PlayerMobile iniciante = (PlayerMobile)personagem;
            if (iniciante.Young)
                iniciante.Young = false;
        }

        public static bool Confirmar(Mobile personagem)
        {
            PlayerMobile iniciante = (PlayerMobile)personagem;
            return iniciante.Young;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Server;

namespace GeNova.Server.Variados
{
    public abstract class PosicionamentoNoMapa
    {
        #region Enumeradores
        private enum Eixos
        {
            X = 0,
            Y = 1,
            Z = 2
        }
        #endregion

        #region Metodos de Auxilio
        private static string FormatarEixos(Dictionary<Eixos, int> posicoes)
        {
            int registros = 0;
            StringBuilder retorno = new StringBuilder();
            retorno.Append("(");
            foreach (int posicao in posicoes.Values)
            {
                registros++;
                if (registros.Equals(posicoes.Count))
                    retorno.Append(posicao);
                else
                    retorno.AppendFormat("{0}, ", posicao);
            }
            retorno.Append(")");
            return retorno.ToString();
        }
        #endregion

        #region Metodos Principais
        public static Point3D BritainCentro()
        {
            Dictionary<Eixos, int> posicao = new Dictionary<Eixos,int>();
            posicao.Add(Eixos.X, 1495);
            posicao.Add(Eixos.Y, 1629);
            posicao.Add(Eixos.Z, 10);
            string valor = FormatarEixos(posicao);
            return Point3D.Parse(valor);
        }
        #endregion
    }
}

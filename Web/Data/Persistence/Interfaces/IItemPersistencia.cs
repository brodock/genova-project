using System;
using System.Collections.Generic;
using System.Text;
using Persistence.Items;
using Persistence.ControleTransacao;

namespace Persistence.Interfaces
{
    public interface IItemPersistencia
    {
        // Sem controle de Transação
        bool Incluir();
        bool Alterar();
        bool Excluir();

        void DefinirTipoPersistencia(ColecaoPersistencia colecao);
    }
}

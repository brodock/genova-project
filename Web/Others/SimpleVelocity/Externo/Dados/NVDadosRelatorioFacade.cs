using System;
using System.Collections.Generic;
using System.Text;
using NVelocity;

namespace SimpleVelocity.Dados
{
    /// <summary>
    /// Classe responsável por adicionar itens ao contexto do Relatório NVelocity.
    /// </summary>
    public class NVDadosRelatorioFacade
    {
        private VelocityContext _context;        

        internal VelocityContext Contexto
        {
            get { return this._context; }
        }

        public NVDadosRelatorioFacade()
        {
            this._context = new VelocityContext();
        }

        /// <summary>
        /// Adicionar dado ao Relatório .vm.
        /// </summary>
        /// <param name="chave">Chave usada no .vm.</param>
        /// <param name="valor">Recebe um object, um valor tanto primitivo quanto um objeto qualquer do sistema.</param>
        public void Adicionar(string chave, object valor)
        {
            this._context.Put(chave, valor);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using SimpleVelocity.Dados;

namespace SimpleVelocity.Estrutura
{
    public abstract class EstruturaClasseRelatorio
    {
        /*Atributos NVelocity*/
        private NVRelatorioFacade _relatorio;
        private NVDadosRelatorioFacade _dadosRelatorio;
        
        /*Atributos Primitivos*/
        private string _caminhoRelatorio;

        /*Lista de Tipos - Visível*/
        private Dictionary<string, object> _listaTipoDadosRelatorio = new Dictionary<string, object>();
        public Dictionary<string, object> GetListaTipos
        {
            get { return _listaTipoDadosRelatorio; }
        }

        public EstruturaClasseRelatorio(string caminhoRelatorio)
        {
            this._dadosRelatorio = new NVDadosRelatorioFacade();
            this._caminhoRelatorio = caminhoRelatorio;                        
        }

        protected void AdicionarDadosRelatorio(string chave, object valor)
        {
            this.AdicionarDadosRelatorio(chave, valor, valor.GetType());
        }
        protected void AdicionarDadosRelatorio(string chave, object valor, string tipo)
        {
            this.AdicionarDadosRelatorio(chave, valor, Type.GetType(tipo));
        }
        protected void AdicionarDadosRelatorio(string chave, object valor, Type tipo)
        {
            this._dadosRelatorio.Adicionar(chave, valor);
            this._listaTipoDadosRelatorio.Add(chave, tipo);
        }
        
        public StringBuilder RecuperarRelatorio()
        {
            this.PreencherRelatorio();

            this._relatorio = new NVRelatorioFacade(this._dadosRelatorio, this._caminhoRelatorio);
            return this._relatorio.RecuperarRelatorio();
        }        

        protected abstract void PreencherRelatorio();
    }
}

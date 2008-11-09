using System;
using System.Collections.Generic;
using System.Text;
using NVelocity.App;
using SimpleVelocity.Interno;
using SimpleVelocity.Dados;
using SimpleVelocity.Exceptions;

namespace SimpleVelocity
{
    /// <summary>
    /// Classe base para utilização de Relatórios NVelocity.
    /// </summary>
    public class NVRelatorioFacade
    {
        #region Atributos
        private VelocityEngine _engine;

        private NVDadosRelatorioFacade _dados;
        private string _pathRelatorio;
        #endregion

        #region Propriedades
        internal VelocityEngine Engine
        {
            get { return this._engine; }
        }

        internal NVDadosRelatorioFacade Dados
        {
            get { return _dados; }
        }
        internal string CaminhoRelatorio
        {
            get { return _pathRelatorio; }
        }
        #endregion

        #region Construtores
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="dados">Dados que serão utilizados pelo relatório.</param>
        /// <param name="pathRelatorio">Path interno, após: ./_content/NVRelatorios/</param>
        public NVRelatorioFacade(NVDadosRelatorioFacade dados, string pathRelatorio)
        {
            this._engine = new VelocityEngine();
            this._dados = dados;
            this._pathRelatorio = pathRelatorio;
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Método que retorna um StringBuilder com o relatório montado.
        /// </summary>
        /// <returns>StringBuilder</returns>
        public StringBuilder RecuperarRelatorio()
        {
            try
            {
                // Iniciar
                PropriedadesFacade propriedades = new PropriedadesFacade();
                this.Iniciar(propriedades);

                // Formatar
                FormatoFacade formato = new FormatoFacade(this);
                formato.SetarFormato();

                // Recuperar
                return formato.RecuperarFormatoComDados();
            }
            catch (Exception)
            {
                throw new NVRelatorioException();
            }
        }

        internal void Iniciar(PropriedadesFacade propriedades)
        {
            // Iniciar Velicity Engine.
            this._engine.Init(propriedades.RecuperarConfiguracoes());
        }
        #endregion
    }
}

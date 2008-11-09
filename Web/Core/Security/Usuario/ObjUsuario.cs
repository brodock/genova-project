using System;
using System.Collections.Generic;
using System.Text;
using Security;
using Security.Criptografia;
using Security.Usuario.Excessoes;
using Security.Usuario.Enumeradores;
using Persistence.Utilitarios;
using Utils;
using System.IO;

namespace Security.Usuario
{
    public class ObjUsuario : ModeloObjetoBase
    {
        #region Atributos
        private int _id;
        private string _login;
        private string _senha;
        private TipoUsuario _tipo;
        private string _avatar;
        private bool _materializado;

        private bool _autenticado;
        #endregion

        #region Propriedades
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Login
        {
            get { return _login; }
            set { _login = value; }
        }

        public string Senha
        {
            get { return _senha; }
            set { _senha = value; }
        }

        public TipoUsuario Tipo
        {
            get { return _tipo; }
            set { _tipo = value; }
        }

        public string Avatar
        {
            get { return _avatar; }
            set { _avatar = value; }
        }

        /// <summary>
        /// WebPathAvatars + Avatar
        /// </summary>
        public string AvatarCompleto
        {
            get
            {
                if (String.IsNullOrEmpty(this.Avatar))
                    return ConfiguracoesWeb.ExibicaoPadrao;
                
                // Verificando se o arquivo existe fisicamente
                string pathAvatarCompleto = String.Concat(ConfiguracoesWeb.PathAvatars, this.Avatar);
                if (File.Exists(pathAvatarCompleto))
                {
                    string avatarCompleto = String.Concat(ConfiguracoesWeb.WebPathAvatars, this.Avatar);
                    return avatarCompleto;
                }
                else
                    return ConfiguracoesWeb.ExibicaoPadrao;                
            }
        }

        public bool Materializado
        {
            get { return _materializado; }
        }

        public bool Autenticado
        {
            get { return _autenticado; }
        }
        #endregion

        public ObjUsuario()
            : base("GeNova_Web_Usuario", "IdUsuario")
        {
            this.Reset();
        }

        public ObjUsuario(int idUsuario)
            : this()
        {
            this.Materializar(idUsuario);
        }

        #region Métodos
        public void Materializar(int idUsuario)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT IdUsuario, Login, Senha, Tipo, Avatar");
            query.AppendFormat("FROM {0}\n", this.Tabela);
            query.AppendFormat("WHERE 1=1 AND {0} = {1}", this.ChavePrimaria, idUsuario);
            LeitorFacade leitor = new LeitorFacade(query);

            if (leitor.LerLinha())
            {
                this.ID = Conversoes.Inteiro32(leitor.GetValor("IdUsuario"));
                this.Login = leitor.GetValor("Login").ToString();
                this.Senha = leitor.GetValor("Senha").ToString();
                this.Tipo = (TipoUsuario)Conversoes.Inteiro32(leitor.GetValor("Tipo"));
                this.Avatar = leitor.GetValor("Avatar").ToString();
                this._materializado = true;
            }
        }

        internal void Autenticar(string senha)
        {
            CriptografiaMD5 criptografia = new CriptografiaMD5();

            if (!criptografia.Comparar(senha, this.Senha))
                throw new SenhaInvalidaException(Erros.MsgSenhaInvalida);
            else
                this._autenticado = true;
        }

        protected override void Reset()
        {
            this._id = int.MinValue;
            this._login = string.Empty;
            this._senha = string.Empty;
            this._tipo = TipoUsuario.Usuario;
            this._avatar = string.Empty;
            this._materializado = false;
        }

        protected override void PreencherListaItems()
        {
            if (this.ItemsPersistencia.Count <= 0)
            {
                this.ItemsPersistencia.AdicionarItem("IdUsuario", this.ID, this.ID.GetType());
                this.ItemsPersistencia.AdicionarItem("Login", this.Login, this.Login.GetType());
                this.ItemsPersistencia.AdicionarItem("Senha", this.Senha, this.Senha.GetType());
                this.ItemsPersistencia.AdicionarItem("Tipo", (int)this.Tipo, typeof(int));
                this.ItemsPersistencia.AdicionarItem("Avatar", this.Avatar, this.Avatar.GetType(), String.IsNullOrEmpty(this.Avatar));
            }
        }

        protected override bool Validar()
        {
            if (String.IsNullOrEmpty(this.Login))
                throw new Exception(Erros.ValorInvalido("Web User", "Login"));

            if (String.IsNullOrEmpty(this.Senha))
                throw new Exception(Erros.ValorInvalido("Web User", "Password"));

            return true;
        }
        #endregion        
    }
}

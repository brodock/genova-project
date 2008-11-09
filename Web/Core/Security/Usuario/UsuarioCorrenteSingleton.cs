using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Security.Usuario
{
    public class UsuarioCorrenteSingleton
    {
        #region Atributos
        protected const string SESSAO_ATUAL_USUARIO = "_UsuarioAtual_";
        private ObjUsuario _usuarioCorrente = new ObjUsuario();
        
        private volatile static UsuarioCorrenteSingleton instance;
        private static object syncRoot = new Object();
        #endregion

        #region Propriedades
        internal ObjUsuario Usuario
        {
            get { return _usuarioCorrente; }
            set { _usuarioCorrente = value; }
        }
        #endregion

        protected UsuarioCorrenteSingleton()
        {
        }

        public static UsuarioCorrenteSingleton Istance
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session[SESSAO_ATUAL_USUARIO] == null)
                    {
                        lock (syncRoot)
                        {
                            #region Trecho causava brecha de segurança, mantendo usuario logado para diferentes browsers ou computadores.
                            /*
                            if (instance == null)
                            {
                                instance = new UsuarioCorrenteSingleton();
                                HttpContext.Current.Session[SESSAO_ATUAL_USUARIO] = instance;
                            }
                            */
                            #endregion

                            // Se a sessão é nula é porque não esta autenticado.
                            instance = new UsuarioCorrenteSingleton();
                            HttpContext.Current.Session[SESSAO_ATUAL_USUARIO] = instance;
                        }
                    }
                    else
                    {
                        lock (syncRoot)
                        {
                            instance = (UsuarioCorrenteSingleton)HttpContext.Current.Session[SESSAO_ATUAL_USUARIO];
                        }
                    }
                }
                else
                {
                    if (instance == null)
                    {
                        lock (syncRoot)
                        {
                            if (instance == null)
                                instance = new UsuarioCorrenteSingleton();
                        }
                    }
                }
                return instance;
            }
        }

        public static void Destroy()
        {
            instance = null;
            HttpContext.Current.Session[SESSAO_ATUAL_USUARIO] = null;
        }
    }
}

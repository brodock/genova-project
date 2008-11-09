using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Reflection;
using SimpleVelocity.Estrutura;

namespace SimpleVelocity.Reflexao
{
    public abstract class NVRelatorioReflexao
    {
        public static void PreencherTreeView(ref TreeView arvore, EstruturaClasseRelatorio relatorio)
        {
            foreach (KeyValuePair<string, object> dado in relatorio.GetListaTipos)
            {
                /*Setando nome e tipo do dado incluído*/
                string nomeObjeto = String.Format("${0}", dado.Key);
                Type tipoObjeto = Type.GetType(dado.Value.ToString());

                TreeNode noArvore = new TreeNode();
                noArvore.Text = String.Format("{0} : {1}", nomeObjeto, tipoObjeto.Name);

                MemberInfo[] metodos = tipoObjeto.GetMembers(BindingFlags.Static |
                                                             BindingFlags.Public |
                                                             BindingFlags.Instance |
                                                             BindingFlags.DeclaredOnly |
                                                             BindingFlags.CreateInstance);
                foreach (MemberInfo membro in metodos)
                {
                    if (membro.MemberType == MemberTypes.Constructor | membro.MemberType == MemberTypes.Property)
                        continue;

                    string descricao = String.Format("{0} is {1}", membro, membro.MemberType);
                    noArvore.ChildNodes.Add(new TreeNode(descricao));
                }

                arvore.Nodes.Add(noArvore);
            }
            arvore.ExpandAll();
        }
    }
}

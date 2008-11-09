using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using Persistence.Utilitarios;

namespace WebApplicationWebControls
{
    public abstract class ControlesUtil
    {
        public static void PreencherDropDownList(DropDownList dropDownList, LeitorFacade leitor, string nomeColunaDescricao, string nomeColunaValor)
        {
            PreencherDropDownList(dropDownList, leitor, nomeColunaDescricao, nomeColunaValor, false);
        }
        public static void PreencherDropDownList(DropDownList dropDownList, LeitorFacade leitor, string nomeColunaDescricao, string nomeColunaValor, bool permitirSelecionarTodos)
        {
            dropDownList.Items.Clear();

            while (leitor.LerLinha())
            {
                string descricao = leitor.GetValor(nomeColunaDescricao).ToString();
                string valor = leitor.GetValor(nomeColunaValor).ToString();

                ListItem item = new ListItem(descricao, valor);
                dropDownList.Items.Add(item);
            }

            if (permitirSelecionarTodos)
                dropDownList.Items.Insert(0, new ListItem("..Selecionar todos..", "0"));
            else
                dropDownList.Items.Insert(0, new ListItem("..Selecione..", "0"));
        }
    }
}

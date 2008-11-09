using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace WebApplicationController
{
    public class Mensagens
    {
        public static void MostrarAlerta(Page pagina, string mensagem)
        {
            string mensagemProcessamento = mensagem;            
            mensagemProcessamento = mensagemProcessamento.Replace(Environment.NewLine, "\\\n");            
            mensagemProcessamento = mensagemProcessamento.Replace("\'", "\\\'");
            mensagemProcessamento = mensagemProcessamento.Replace("\"", "\\\"");            

            StringBuilder javaScript = new StringBuilder();
            javaScript.AppendFormat("alert('{0}');", mensagemProcessamento);

            pagina.ClientScript.RegisterStartupScript(typeof(object), "mensagemAlerta", javaScript.ToString(), true);
        }
    }
}
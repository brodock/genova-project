using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using Utils;
using WebApplicationWebControls.Upload.Exceptions;

namespace WebApplicationWebControls.Upload.Imagens
{
    public class UploadImagem
    {
        private FileUpload _arquivo;
        private string _path;
        private HttpServerUtility _utilitarioHttp;

        private int _tamanhoArquivoKB;
        private int _largura;
        private int _altura;
        private List<string> _extensoes;

        #region Propriedades
        public string NomeArquivo
        {
            get { return this._arquivo.FileName; }
        }
        #endregion

        public UploadImagem(FileUpload arquivo, string path, HttpServerUtility utilitarioHttp, int tamanhoArquivoKB, int largura, int altura)
        {
            this._arquivo = arquivo;
            this._path = path;
            this._utilitarioHttp = utilitarioHttp;
            this._tamanhoArquivoKB = tamanhoArquivoKB * 1024;
            this._largura = largura;
            this._altura = altura;
            this.PreencherExtensoesValidas();
        }

        private void PreencherExtensoesValidas()
        {
            this._extensoes = new List<string>();
            this._extensoes.Add("image/jpeg");
            this._extensoes.Add("image/gif");
            this._extensoes.Add("image/png");            
        }

        public bool Upload()
        {
            if (String.IsNullOrEmpty(this._arquivo.FileName))
                throw new NomeArquivoInvalidoException("Nome do Arquivo inválido.");

            if (this._arquivo.PostedFile.ContentLength.Equals(0))
                throw new ArquivoInvalidoException("Arquivo inválido.");

            if (!this._extensoes.Contains(this._arquivo.PostedFile.ContentType))
                throw new ExtensaoInvalidaException("Extensão do arquivo inválida.");

            System.Drawing.Image imagem = System.Drawing.Image.FromStream(this._arquivo.PostedFile.InputStream);
            if (!this._largura.Equals(int.MinValue) && imagem.Width > this._largura)
                throw new LarguraInvalidaException(String.Format("Largura do arquivo deve ser {0}cm.", this._largura));
            if (!this._altura.Equals(int.MinValue) && imagem.Height > this._altura)
                throw new LarguraInvalidaException(String.Format("Altura do arquivo deve ser {0}cm.", this._altura));

            if (this._arquivo.PostedFile.ContentLength > this._tamanhoArquivoKB)
                throw new TamanhoInvalidoException(String.Format("Tamanho do arquivo excedeu o limite permitido. Máximo de {0}KB.", this._tamanhoArquivoKB / 1024));

            string nomeDoArquivo = Path.GetFileName(this._arquivo.FileName);
            try
            {
                this._arquivo.PostedFile.SaveAs(String.Concat(this._utilitarioHttp.MapPath(this._path), "/", nomeDoArquivo));
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                throw new AcessoNaoAutorizadoException("Permissão para enviar o arquivo foi negada.");
            }
        }
    }
}

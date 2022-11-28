using ApiMvc.Database;

namespace ApiMvc.Functions
{
    public class FuncProduto:IDisposable
    {
        private bool disposedValue;

        public FuncProduto()
        {
           
        }
        public static string PegarNomeUrl(string? url)
        {
            if(string.IsNullOrEmpty(url))
                return string.Empty;
            string separar = url;
            string[] separar1 = separar.Split("%2F");
            string[] separar2 = separar1[3].Split("?");
            return separar2[0];
        }
        public async Task SubirImg(IWebHostEnvironment _iweb, ModelProduto p ,string idUser , int opcao, string email, string senha, string caminho)
        {
            using (Auth auth = new())
            {
                if (p.ImgArquivo != null)
                {
                    if (p.ImgArquivo.Length > 0)
                    {
                        var path = Path.Combine(_iweb.WebRootPath, $"img\\Temp\\{p.ImgArquivo.FileName}");
                        try
                        {
                            using Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                            p.ImgArquivo.CopyTo(stream);

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        using Auth storage = new();
                        var url = await storage.UploadImage(_iweb, p.ImgArquivo, idUser, email, senha, caminho);
                        using UserBase data = new();
                        string nomeVendedor = await data.RetornaNome(idUser);
                        p.NomeVendedor = nomeVendedor;
                        p.UrlImg = url;
                        p.ImgArquivo = null;
                        using ProductBase pBase = new();
                        switch (opcao)
                        {
                            case 0: 
                                p.Path = caminho;
                                await pBase.AddProdutoAsync(idUser,p);
                                break;
                            case 1:
                                p.Path = caminho;
                                await pBase.AlterarProdutoAsync(idUser,p);
                                break;
                        }                      
                    }
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

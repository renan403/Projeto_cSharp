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
            using Auth auth = new();
            if (p.File != null)
            {
                if (p.File.Length > 0)
                {
                    try
                    {
                        var path = Path.Combine(_iweb.ContentRootPath, $"Img\\Temp\\{p.File.FileName}");
                        using Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                        p.File.CopyTo(stream);

                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                    using Auth storage = new();
                    var url = await storage.UploadImage(_iweb, p.File, idUser, email, senha, caminho);
                    using UserBase data = new();
                    string nomeVendedor = await data.RetornaNome(idUser);
                    p.NomeVendedor = nomeVendedor;
                    p.UrlImg = url;
                    p.File = null;
                    //using ProductBase pBase = new();
                    //switch (opcao)
                    //{
                    //    case 0: 
                    //        p.Path = caminho;
                    //        await pBase.AddProdutoAsync(idUser, _iweb, p);
                    //        break;
                    //    case 1:
                    //        p.Path = caminho;
                    //        await pBase.AlterarProdutoAsync(idUser,p);
                    //        break;
                    //}                      
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

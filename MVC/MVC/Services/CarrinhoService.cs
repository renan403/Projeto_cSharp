using MVC.Models;

namespace MVC.Services
{
    public class CarrinhoService
    {
        public async Task<Dictionary<string, ModelProduto>?> RetornaCarrinho(string UrlRtCar)
        {
            HttpClient client = new ();
            var rt = await client.GetAsync(UrlRtCar);
            if (rt.IsSuccessStatusCode)
            {
                return await rt.Content.ReadFromJsonAsync<Dictionary<string,ModelProduto>>();
            }
            return null;
        }
    }
}

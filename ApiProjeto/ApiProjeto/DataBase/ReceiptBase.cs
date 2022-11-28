using ApiMvc.FireData;
using ApiMvc.Repositories;

namespace ApiMvc.Database
{
    public class ReceiptBase : Base ,IReceiptRepository
    {


        public async Task<Array> RetornarNotaFiscal(string userId)
        {
            var key = await GetUserKey(userId);
            var notas = await _client.Child($"Usuarios/{key}/NotaFiscal").OnceAsync<ModelNotaFiscal>();
            return notas.OrderByDescending(m => m.Object.Registro).ToArray();
        }
        public async Task<bool> GerarNotaFiscal(string userId, ModelNotaFiscal notaFiscal)
        {
            float? total = 0;
            Gerador gera = new();
            for (int i = 0; i < notaFiscal.Produto.Count; i++)
            {
                notaFiscal.Produto[i].Data = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy");
                notaFiscal.Produto[i].Enviado = true;
                notaFiscal.Produto[i].Recebido = false;
                total += notaFiscal.Produto[i].ValorTotal;
            }
            notaFiscal.Produto[0].ValorTotal = (float?)Math.Round((decimal)total, 2);
            notaFiscal.Registro = DateTime.Now;
            string numPedido = $"{ gera.AleatoriosComprarProd() }-{ gera.AleatoriosComprarProd()}";
            var key = await GetUserKey(userId);
            await _client.Child($"Usuarios/{key}/NotaFiscal/{numPedido}").PatchAsync(notaFiscal);
            return true;
        }
        public async Task<ModelNotaFiscal> PegarNotaFiscal(string userId, string nota)
        {
            var key = await GetUserKey(userId);

            return await _client.Child($"Usuarios/{key}/NotaFiscal/{nota}").OnceSingleAsync<ModelNotaFiscal>();
        }
        public async Task ConfirmarPedido(string userId, string nota, string nome)
        {
            var key = await GetUserKey(userId);
            var produtos = await PegarNotaFiscal(userId, nota);
            ModelDestinatario destinatario = new() { Nome = nome, Data = DateTime.Now.ToString("dd 'de' MMMMM 'de' yyyy") };
            foreach (var produto in produtos.Produto)
            {
                produtos.Destinatario = destinatario;
                produto.Recebido = true;
            }
            await _client.Child($"Usuarios/{key}/NotaFiscal/{nota}").PatchAsync(produtos);
        }
    }
}

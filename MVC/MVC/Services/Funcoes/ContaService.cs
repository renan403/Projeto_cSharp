﻿using Firebase.Database;
using MVC.Models;

namespace MVC.Services.Funcoes
{
    public class Result
    {
        public List<dynamic>? ProdutosEqtd;
        public float? ValorTotal;
        public int? QuantiaTotal;
        public List<ModelProduto>? Prod;
    }
    public class ContaService
    {
        private readonly IConfiguration _iconfig;
        public ContaService(IConfiguration iconfig)
        {
            _iconfig = iconfig;
        }

        public async Task<Result> FinalizarCompra(Dictionary<string,ModelProduto> model)
        {
            float? valorTotal = 0;
            int? quantiaTotal = 0;
            List<dynamic> produtosEqtd = new();

           
            List<ModelProduto> prod = new();
            foreach (var idProduto in model)
            {
                var ID = idProduto.Value.IdProduto;
                ProdutoService prodServ = new(_iconfig);
                ModelProduto item = await prodServ.RetornaProdutoPorID(ID) ?? new ModelProduto { };
                valorTotal += item.PrecoProd * idProduto.Value.QtdPorProd;
                quantiaTotal += idProduto.Value.QtdPorProd;

                List<dynamic> unirLista = new()
                {
                    item,
                    idProduto.Value.QtdPorProd ?? 0,
                    idProduto.Key
                };
                produtosEqtd.Add(unirLista);

                // Implementar na model para gerar nota

                item.Qtd = idProduto.Value.QtdPorProd;
                item.ValorTotal = (float)Math.Round((decimal)((decimal)item.PrecoProd * idProduto.Value.QtdPorProd), 2);
                item.Cancelado = false;
                item.Data = DateTime.Now.ToString("dd/MM/yyyy");
                prod.Add(item);
            }
            Result rt = new()
            {
                Prod= prod,
                ProdutosEqtd = produtosEqtd,
                QuantiaTotal = quantiaTotal,
                ValorTotal = valorTotal,
            };
            return rt;


        }
    }
}
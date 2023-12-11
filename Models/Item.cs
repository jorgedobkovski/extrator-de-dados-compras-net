using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extratorTermoHomologacaoComprasNet.Models
{
    public class Item
    {
        public int Numero { get; set; }
        public string? Descricao { get; set; }
        public string? DescricaoDetalhada { get; set; }
        public decimal? Quantidade { get; set; }
        public decimal? ValorEstimado { get; set; }
        public string? UnidadeFornecimento { get; set; }
        public string? Situacao { get; set; }
        public string? Pregoeiro { get; set; }
        public string? FornecedorNomeRazaoSocial { get; set; }
        public string? FornecedorCPFCNPJ { get; set; }
        public decimal? MelhorLance { get; set; }
        public decimal? ValorNegociado { get; set; }
        public string? TratamentoDiferenciadoMEEPP { get; set; }
        public decimal? IntervaloMinimoEntreLances { get; set; }

    }
}

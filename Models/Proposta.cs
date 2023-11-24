using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extratorTermoHomologacaoComprasNet.Models
{
    public class Proposta
    {
        public string FornecedorCPFCNPJ { get; set; }
        public string FornecedorNomeRazaoSocial { get; set; }
        public bool PorteMeEpp { get; set; }
        public decimal ValorOferdao { get; set; }
        public decimal ValorProposta { get; set; }
        public decimal ValorNegociado { get; set; }
        public decimal QuantidadeOfertada { get; set; }
        public string Situacao { get; set; }
    }
}

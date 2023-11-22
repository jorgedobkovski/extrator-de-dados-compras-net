using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extratorTermoHomologacaoComprasNet.Models
{
    public class Certame
    {
        public int Numero { get; set; }
        public int Ano { get; set; }
        public string Processo { get; set; }
        public string HoraHomologacao { get; set; }
        public string DataHomologacao { get; set; }
        public string Pregoeiro { get; set; }
        public string FundamentacaoLegal { get; set; }
        public string CriterioDeJulgamento { get; set; }
        public bool CompraEmergencial { get; set; }
        public string ObjetoDaCompra { get; set; }
        public string EntregaDePropostas { get; set; }
        public string DataDeAbertura { get; set; }
        public string Caracteristica { get; set; }
        public string ModoDeDisputa { get; set; }

    }
}

using extratorTermoHomologacaoComprasNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace extratorTermoHomologacaoComprasNet.Builders
{
    public static class ExtratorPropostaBuilder
    {
        public static List<Proposta> ExtrairInformacoesDaProposta(string conteudoPdf)
        {

            var conteudoBrutoContendoInformacoesDaProposta = IsolarInformacoesBrutasDaProposta(conteudoPdf);
            var conteudoBrutoContendoInformacoesDoPropostaSemQuebrasDeLinha = conteudoBrutoContendoInformacoesDaProposta.Replace("\n", " ").Replace("\r", " ");

            List<string> stringPropostas = ExtrairStringPropostas(conteudoBrutoContendoInformacoesDaProposta);

            int numeroProposta = 1;

            List<Proposta> propostas = new List<Proposta>();

            foreach (var stringProposta in stringPropostas)
            {
                Console.WriteLine($"Proposta {numeroProposta}:");
                //Console.WriteLine($"{stringProposta}");
                numeroProposta++;
                         
                var proposta = ExtrairDadosDaProposta(stringProposta);
                propostas.Add(proposta);
            }

            return propostas;
           
        }

        private static string IsolarInformacoesBrutasDaProposta(string texto)
        {
            //Isolando informações do Certame a partir do título Pregao Numero até título Mensagens do chat da compra
            try
            {
                var tituloPropostasDoItem = "\nPropostas do Item ";
                var indexTituloPropostasDoItem = texto.IndexOf(tituloPropostasDoItem);
                var tituloLancesDoItem = "\nLances do Item ";
                var indexTituloLancesDoItem = texto.IndexOf(tituloLancesDoItem);
                var conteudoBrutoContendoInformacoesDoCertame = "";

                if (indexTituloPropostasDoItem >= 0 && indexTituloLancesDoItem >= 0)
                {
                    // Calcula o comprimento da substring entre os dois índices
                    int startIndex = indexTituloPropostasDoItem + tituloPropostasDoItem.Length;
                    int length = indexTituloLancesDoItem - startIndex;

                    // Extrai a substring
                    conteudoBrutoContendoInformacoesDoCertame = texto.Substring(startIndex, length);
                }

                return conteudoBrutoContendoInformacoesDoCertame;
            }
            catch (Exception)
            {
                throw;
            }
        }

        static List<string> ExtrairStringPropostas(string textoPropostas)
        {
            var patternInicio = @"\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}";
            var patternFinal = @"Quantidade ofertada: \b\d+\b"; // Este padrão pega números inteiros

            var matchesInicio = Regex.Matches(textoPropostas, patternInicio);
            var matchesFinal = Regex.Matches(textoPropostas, patternFinal);

            List<string> propostas = new List<string>();

            for (int i = 0; i < matchesInicio.Count; i++)
            {
                int indexInicio = textoPropostas.IndexOf(matchesInicio[i].Value);
                int indexFinal = textoPropostas.IndexOf(matchesFinal[i].Value, indexInicio);

                string proposta = textoPropostas.Substring(indexInicio, indexFinal + matchesFinal[i].Length - indexInicio);
                propostas.Add(proposta);
            }

            return propostas;
        }

        static Proposta ExtrairDadosDaProposta(string stringProposta)
        {
            var patternCPFCNPJ = @"\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}";

            var matchesCPFCNPJ = Regex.Match(stringProposta, patternCPFCNPJ);

            var textoTravessaoAposCNPJ = " - ";
            var conteudoBrutoAPartirDaRazaoSocial = stringProposta.Substring(matchesCPFCNPJ.Length + textoTravessaoAposCNPJ.Length);
            var conteudoBrutoAPartirDaRazaoSocialSemQuebrasDeLinha = conteudoBrutoAPartirDaRazaoSocial.Replace("\n", " ").Replace("\r", " ");
            var textoRSReais = " R$ ";
            var indexTextoRSReais = conteudoBrutoAPartirDaRazaoSocialSemQuebrasDeLinha.IndexOf(textoRSReais);
            var nomeRazaoSocial = conteudoBrutoAPartirDaRazaoSocialSemQuebrasDeLinha.Substring(0, indexTextoRSReais).Replace("Proposta", "");
            var conteudoBrutoAPatirDoValorOfertado = conteudoBrutoAPartirDaRazaoSocial.Substring(indexTextoRSReais + textoRSReais.Length);
            var textoDesclassificada = "\ndesclassificada\n";
            var indexTextoDesclassificada = conteudoBrutoAPatirDoValorOfertado.IndexOf(textoDesclassificada);
            var stringValorOfertado = "";
            if(indexTextoDesclassificada == -1)
            {
                var indexPrimeiroEspacoAposValorOfertado = conteudoBrutoAPatirDoValorOfertado.IndexOf(" ");
                stringValorOfertado = conteudoBrutoAPatirDoValorOfertado.Substring(0, indexPrimeiroEspacoAposValorOfertado);                
            }
            else
            {
                stringValorOfertado = conteudoBrutoAPatirDoValorOfertado.Substring(0, indexTextoDesclassificada);
            }
            
            var valorOfertado = decimal.Parse(stringValorOfertado);           
            var textoTravessaoAposValorOfertado = " -\n";
            var textoPropostaAdjudicada = " Proposta adjudicada\n";
            var indexTextoTravessaoAposValorOfertado = conteudoBrutoAPatirDoValorOfertado.IndexOf(textoTravessaoAposValorOfertado);
            var indexTextoPropostaAdjudicada = conteudoBrutoAPatirDoValorOfertado.IndexOf(textoPropostaAdjudicada);
            bool propostaAdjudicada = indexTextoPropostaAdjudicada != -1;
            var situacaoItem = propostaAdjudicada ? "Adjudicada" : "Fracassada";


            //VERIFICAR SE, NA PROPOSTA É INFORMADO A MARCA/FABRICANTE E MODELO/VERSAO
            if (conteudoBrutoAPatirDoValorOfertado.IndexOf("\nModelo/versão: ") == -1 || conteudoBrutoAPatirDoValorOfertado.IndexOf("\nMarca/Fabricante: ") == -1)
            {
                var textoPorteMeEppEquiparadaDoisPontos = "\nPorte MeEpp/Equiparada: ";
                var indexTextoPorteMeEppEquiparadaDoisPontos = conteudoBrutoAPatirDoValorOfertado.IndexOf(textoPorteMeEppEquiparadaDoisPontos);
                var conteudoAposTextoPorteMeEppEquiparadaDoisPontos = conteudoBrutoAPatirDoValorOfertado.Substring(indexTextoPorteMeEppEquiparadaDoisPontos + textoPorteMeEppEquiparadaDoisPontos.Length);
                var textoValorProposta = "\nValor proposta: R$ ";
                var indexTextoValorProposta = conteudoAposTextoPorteMeEppEquiparadaDoisPontos.IndexOf(textoValorProposta);

                var porteEmpresaMeEpp = conteudoAposTextoPorteMeEppEquiparadaDoisPontos.Substring(0, indexTextoValorProposta);
                var porteEmpresaMeEppBool = porteEmpresaMeEpp.IndexOf("Sim") != -1 ? true : false;                  

                var conteudoBrutoAposValorProposta = conteudoAposTextoPorteMeEppEquiparadaDoisPontos.Substring(indexTextoValorProposta + textoValorProposta.Length);
                var textoValorNegociado = " Valor negociado: ";
                var indexTextoValorNegociado = conteudoBrutoAposValorProposta.IndexOf(textoValorNegociado);
                var stringValorProposta = conteudoBrutoAposValorProposta.Substring(0, indexTextoValorNegociado);
                var valorProposta = decimal.Parse(stringValorProposta);

                var conteudoBrutoAposValorNegociado = conteudoBrutoAposValorProposta.Substring(indexTextoValorNegociado + textoValorNegociado.Length);
                var textoQuantidadeOfertada = " Quantidade ofertada: ";
                var indexTextoQuantidadeOfertada = conteudoBrutoAposValorNegociado.IndexOf(textoQuantidadeOfertada);
                var textoReais = "R$ ";
                var textoNaoInformado = "Não informado";
                var stringValorNegociado = conteudoBrutoAposValorNegociado.Substring(0, indexTextoQuantidadeOfertada);
                decimal valorNegociado;
                if (stringValorNegociado.Equals(textoNaoInformado))
                {
                    valorNegociado = 0;
                }
                else
                {
                    stringValorNegociado = stringValorNegociado.Substring(textoReais.Length);
                    valorNegociado = decimal.Parse(stringValorNegociado);
                }


                var conteudoBrutoAposQuantidadeOfertada = conteudoBrutoAposValorNegociado.Substring(indexTextoQuantidadeOfertada + textoQuantidadeOfertada.Length);
                var stringQuantidadeOfertada = conteudoBrutoAposQuantidadeOfertada.Substring(0);
                var quantidadeOfertada = decimal.Parse(stringQuantidadeOfertada);

                var proposta = new Proposta
                {
                    FornecedorCPFCNPJ = matchesCPFCNPJ.Value,
                    FornecedorNomeRazaoSocial = nomeRazaoSocial,
                    Situacao = situacaoItem,
                    PorteMeEpp = porteEmpresaMeEppBool,
                    ValorOfertado = valorOfertado,
                    ValorProposta = valorProposta,
                    ValorNegociado = valorNegociado,
                    QuantidadeOfertada = quantidadeOfertada,
                };

                Console.WriteLine("- - - - - - - - - - - - - - - - - - -");
                Console.WriteLine("CNPJ/CPF:              " + proposta.FornecedorCPFCNPJ);
                Console.WriteLine("RAZAO SOCIAL:          " + proposta.FornecedorNomeRazaoSocial);
                Console.WriteLine("SITUAÇÃO:              " + proposta.Situacao);
                Console.WriteLine("VALOR OFERTADO:        " + proposta.ValorOfertado);
                Console.WriteLine("ME/EPP?                " + proposta.PorteMeEpp);
                Console.WriteLine("VALOR PROPOSTA:        " + proposta.ValorProposta);
                Console.WriteLine("VALOR NEGOCIADO:       " + proposta.ValorNegociado);
                Console.WriteLine("QUANTIDADE OFERTADA:   " + proposta.QuantidadeOfertada);
                Console.WriteLine("- - - - - - - - - - - - - - - - - - -");

                return proposta;
            }
            else
            {
                var textoPorteMeEppEquiparadaDoisPontos = "\nPorte MeEpp/Equiparada: ";
                var indexTextoPorteMeEppEquiparadaDoisPontos = conteudoBrutoAPatirDoValorOfertado.IndexOf(textoPorteMeEppEquiparadaDoisPontos);
                var conteudoAposTextoPorteMeEppEquiparadaDoisPontos = conteudoBrutoAPatirDoValorOfertado.Substring(indexTextoPorteMeEppEquiparadaDoisPontos + textoPorteMeEppEquiparadaDoisPontos.Length);
                var textoMarcaFabricanteDoisPontos = "\nMarca/Fabricante: ";
                var indexTextoMarcaFabricanteDoisPontos = conteudoAposTextoPorteMeEppEquiparadaDoisPontos.IndexOf(textoMarcaFabricanteDoisPontos);
                var porteEmpresaMeEpp = conteudoAposTextoPorteMeEppEquiparadaDoisPontos.Substring(0, indexTextoMarcaFabricanteDoisPontos);
                var porteEmpresaMeEppBool = porteEmpresaMeEpp.IndexOf("Sim") != -1 ? true : false;


                var conteudoBrutoAposTextoMarcaFabricante = conteudoAposTextoPorteMeEppEquiparadaDoisPontos.Substring(indexTextoMarcaFabricanteDoisPontos + textoMarcaFabricanteDoisPontos.Length);
                var textoModeloVersaoDoisPontos = "\nModelo/versão: ";
                var indexTextoModeloVersaoDoisPontos = conteudoBrutoAposTextoMarcaFabricante.IndexOf(textoModeloVersaoDoisPontos);
                var marcaFabricante = conteudoBrutoAposTextoMarcaFabricante.Substring(0, indexTextoModeloVersaoDoisPontos);

                var conteudoBrutoAposModeloVersao = conteudoBrutoAposTextoMarcaFabricante.Substring(indexTextoModeloVersaoDoisPontos + textoModeloVersaoDoisPontos.Length);
                var textoValorProposta = "\nValor proposta: R$ ";
                var indexTextoValorProposta = conteudoBrutoAposModeloVersao.IndexOf(textoValorProposta);
                var modeloVersao = conteudoBrutoAposModeloVersao.Substring(0, indexTextoValorProposta);

                var conteudoBrutoAposValorProposta = conteudoBrutoAposModeloVersao.Substring(indexTextoValorProposta + textoValorProposta.Length);
                var textoValorNegociado = " Valor negociado: ";
                var indexTextoValorNegociado = conteudoBrutoAposValorProposta.IndexOf(textoValorNegociado);
                var stringValorProposta = conteudoBrutoAposValorProposta.Substring(0, indexTextoValorNegociado);
                var valorProposta = decimal.Parse(stringValorProposta);

                var conteudoBrutoAposValorNegociado = conteudoBrutoAposValorProposta.Substring(indexTextoValorNegociado + textoValorNegociado.Length);
                var textoQuantidadeOfertada = " Quantidade ofertada: ";
                var indexTextoQuantidadeOfertada = conteudoBrutoAposValorNegociado.IndexOf(textoQuantidadeOfertada);
                var textoReais = "R$ ";
                var textoNaoInformado = "Não informado";
                var stringValorNegociado = conteudoBrutoAposValorNegociado.Substring(0, indexTextoQuantidadeOfertada);
                decimal valorNegociado;
                if (stringValorNegociado.Equals(textoNaoInformado))
                {
                    valorNegociado = 0.0000M;
                }
                else
                {
                    stringValorNegociado = stringValorNegociado.Substring(textoReais.Length);
                    valorNegociado = decimal.Parse(stringValorNegociado);
                }


                var conteudoBrutoAposQuantidadeOfertada = conteudoBrutoAposValorNegociado.Substring(indexTextoQuantidadeOfertada + textoQuantidadeOfertada.Length);
                var stringQuantidadeOfertada = conteudoBrutoAposQuantidadeOfertada.Substring(0);
                var quantidadeOfertada = decimal.Parse(stringQuantidadeOfertada);

                var proposta = new Proposta
                {
                    FornecedorCPFCNPJ = matchesCPFCNPJ.Value,
                    FornecedorNomeRazaoSocial = nomeRazaoSocial,
                    Situacao = situacaoItem,
                    PorteMeEpp = porteEmpresaMeEppBool,
                    ValorOfertado = valorOfertado,
                    MarcaFabricante = marcaFabricante,
                    ModeloVersao = modeloVersao,
                    ValorProposta = valorProposta,
                    ValorNegociado = valorNegociado,
                    QuantidadeOfertada = quantidadeOfertada,
                };

                Console.WriteLine("- - - - - - - - - - - - - - - - - - -");
                Console.WriteLine("CNPJ/CPF:              " + proposta.FornecedorCPFCNPJ);
                Console.WriteLine("RAZAO SOCIAL:          " + proposta.FornecedorNomeRazaoSocial);
                Console.WriteLine("SITUAÇÃO:              " + proposta.Situacao);
                Console.WriteLine("VALOR OFERTADO:        " + proposta.ValorOfertado);
                Console.WriteLine("ME/EPP?                " + proposta.PorteMeEpp);
                Console.WriteLine("MARCA/FABRICANTE:      " + proposta.MarcaFabricante);
                Console.WriteLine("MODELO/VERSÃO:         " + proposta.ModeloVersao);
                Console.WriteLine("VALOR PROPOSTA:        " + proposta.ValorProposta);
                Console.WriteLine("VALOR NEGOCIADO:       " + proposta.ValorNegociado);
                Console.WriteLine("QUANTIDADE OFERTADA:   " + proposta.QuantidadeOfertada);
                Console.WriteLine("- - - - - - - - - - - - - - - - - - -");

                return proposta;
            }
            
        }
    }
}

using extratorTermoHomologacaoComprasNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extratorTermoHomologacaoComprasNet.Builders
{
    public static class ExtratorItemBuilder
    {
        public static Item ExtrairInformacoesDoItem(string conteudoPdf)
        {
            Item item = new Item();

            var conteudoBrutoContendoInformacoesDoItem = IsolarInformacoesBrutasDoItem(conteudoPdf);

            //Separando as informações brutas dos itens
            int indexTituloItemNumero = -1;
            var conteudoDoItem = "";

            if (conteudoBrutoContendoInformacoesDoItem != "")
            {
                indexTituloItemNumero = conteudoBrutoContendoInformacoesDoItem.IndexOf("Item ");
                conteudoDoItem = conteudoBrutoContendoInformacoesDoItem.Substring(indexTituloItemNumero);
            }
            else
            {
                Console.WriteLine("Índices não encontrados. ETAPA: Separando as informações dos itens das informações brutas (eventos da compra - propostas)");
            }

            //Coletando informações do item 
            int numeroItem = 0;
            var descricaoDoItem = "";
            var descricaoDetalhadaDoItem = "";
            decimal quantidadeDoItem = 0;
            decimal valorEstimadoDoItem = 0;
            string unidadeDeFornecimentoDoItem = "";
            string situacaoDoItem = "";
            string pregoeiro = "";
            string razaoSocialFornecedorVencedorDoItem = "";
            string cnpjFornecedorVencedorDoItem = "";
            decimal melhorLanceDoItem = 0;
            decimal valorNegociadoDoItem = 0;
            string tratamentoDiferenciadoMEEPP = "";
            decimal intervaloMinimoEntreLances = 0;

            if (conteudoDoItem != "" && indexTituloItemNumero != -1)
            {

                //separando título Item X - Descricao
                var indexPrimeiraQuebraDeLinhaPosDescricao = conteudoDoItem.IndexOf('\n');
                var tituloItemNumeroDescricao = conteudoDoItem.Substring(0, indexPrimeiraQuebraDeLinhaPosDescricao);
                var indexHifenPosNumero = tituloItemNumeroDescricao.IndexOf(" - ");

                //seprando numero do item e convertendo para int
                var stringNumeroDoItem = tituloItemNumeroDescricao.Substring(5, indexHifenPosNumero - 5); //"Item " = 5 caracteres
                numeroItem = int.Parse(stringNumeroDoItem);

                //separando descricao do item
                descricaoDoItem = tituloItemNumeroDescricao.Substring(indexHifenPosNumero + 3);

                //separando descricao detalhada
                var conteudoDoItemSemTitulo = conteudoDoItem.Substring(tituloItemNumeroDescricao.Length + 1);
                var indexLabelQuantidadeDoisPontos = conteudoDoItem.IndexOf("\nQuantidade: ");
                var informacoesDoItem = conteudoDoItem.Substring(indexLabelQuantidadeDoisPontos + 1);
                var informacoesDoItemSemQuebrasDeLinha = informacoesDoItem.Replace("\n", "").Replace("\r", "");
                var tamanhoDescricaoDetalhadaDoItem = (conteudoDoItemSemTitulo.Length - informacoesDoItem.Length) - 1; //-1 para tirar a quebra de linha
                descricaoDetalhadaDoItem = conteudoDoItemSemTitulo.Substring(0, tamanhoDescricaoDetalhadaDoItem).Replace("\n", " ").Replace("\r", " "); //retirando quebras de linhas

                //separando quantidade do item
                var indexDaPrimeiraQuebraDeLinhaPosLabelQuantidade = informacoesDoItem.IndexOf("\n"); // localiza a primeira quebra de linha a partir da label quantidade:
                var conteudoLinhaComQuantidadeEValorEstimado = informacoesDoItem.Substring(0, indexDaPrimeiraQuebraDeLinhaPosLabelQuantidade);
                var labelValorEstimado = " Valor estimado: R$ ";
                var indexLabelValorEstimadoDoisPontos = conteudoLinhaComQuantidadeEValorEstimado.IndexOf(labelValorEstimado);
                var stringQuantidadeDoItem = conteudoLinhaComQuantidadeEValorEstimado.Substring(12, indexLabelValorEstimadoDoisPontos - 12); //"Quantidade: " = 12 caracteres
                quantidadeDoItem = Decimal.Parse(stringQuantidadeDoItem);

                //separando valor estimado
                var stringValorEstimadoDoItem = conteudoLinhaComQuantidadeEValorEstimado.Substring(indexLabelValorEstimadoDoisPontos + labelValorEstimado.Length);
                valorEstimadoDoItem = Decimal.Parse(stringValorEstimadoDoItem);

                //separando linha com: Unidade de fornecimento e situacao
                var labelUnidadeDeFornecimento = "\nUnidade de fornecimento: ";
                var labelSituacao = " Situação: ";
                var indexLabelUnidadeDeFornecimento = informacoesDoItem.IndexOf(labelUnidadeDeFornecimento);
                var indexLabelSituacao = informacoesDoItem.IndexOf(labelSituacao);
                var informacoesBrutasDoItemAPartirDaUnidadeDeFornecimento = informacoesDoItem.Substring(indexLabelUnidadeDeFornecimento);
                var informacoesBrutasDoItemAPartirDaSituacao = informacoesDoItem.Substring(indexLabelSituacao);
                var tamanhoUnidadeDeFornecimento = (informacoesBrutasDoItemAPartirDaUnidadeDeFornecimento.Length - labelUnidadeDeFornecimento.Length) - informacoesBrutasDoItemAPartirDaSituacao.Length;
                unidadeDeFornecimentoDoItem = informacoesBrutasDoItemAPartirDaUnidadeDeFornecimento.Substring(labelUnidadeDeFornecimento.Length, tamanhoUnidadeDeFornecimento);

                //coletando situacao do item
                var textoIntervaloMinimoEntreLances = "\nIntervalo mínimo entre lances: R$ ";
                var indexTextoIntervaloMinimoEntreLances = informacoesBrutasDoItemAPartirDaSituacao.IndexOf(textoIntervaloMinimoEntreLances);
                var textoAdjudicadoEHomologadoPor = "\nAdjucado e Homologado por ";
                var indexTextoAdjudicadoEHomologadoPor = informacoesBrutasDoItemAPartirDaSituacao.IndexOf(textoAdjudicadoEHomologadoPor);
                var indexFimSituacaoDoItem = -1;

                if(indexTextoIntervaloMinimoEntreLances == -1)
                {
                    indexFimSituacaoDoItem = indexTextoAdjudicadoEHomologadoPor;
                } else
                {
                    indexFimSituacaoDoItem = indexTextoIntervaloMinimoEntreLances;
                }
                
                situacaoDoItem = informacoesBrutasDoItemAPartirDaSituacao.Substring(labelSituacao.Length, indexFimSituacaoDoItem - labelSituacao.Length);

                var textoTratamentoDiferenciadoMEEPP = "\nTratamento Diferenciado ME/EPP: ";
                var indexTextoTratamentoDiferenciadoMEEPP = informacoesBrutasDoItemAPartirDaSituacao.IndexOf(textoTratamentoDiferenciadoMEEPP);

                //coletando Informacoes  do intervalo minimo entre lances
                
                var indexFinalIntervaloMinimoEntreLances = -1;
                if (indexTextoIntervaloMinimoEntreLances != -1)
                {
                    var conteudoAPartirMinimoEntreLances = informacoesBrutasDoItemAPartirDaSituacao.Substring(indexTextoIntervaloMinimoEntreLances + textoIntervaloMinimoEntreLances.Length);
                    if(indexTextoTratamentoDiferenciadoMEEPP == -1)
                    {
                        indexFinalIntervaloMinimoEntreLances = conteudoAPartirMinimoEntreLances.IndexOf(textoAdjudicadoEHomologadoPor);
                    }
                    else
                    {
                        indexFinalIntervaloMinimoEntreLances = conteudoAPartirMinimoEntreLances.IndexOf(textoTratamentoDiferenciadoMEEPP);
                    }
                    var stringIntervaloMinimoEntreLances = conteudoAPartirMinimoEntreLances.Substring(0, indexFinalIntervaloMinimoEntreLances);
                    intervaloMinimoEntreLances = Decimal.Parse(stringIntervaloMinimoEntreLances);
                }

                //coletando informações do tratamento diferenciado
                
                if(indexTextoTratamentoDiferenciadoMEEPP != -1)
                {
                    var conteudoAposTextoTratamentoDiferenciadoMEEPP = informacoesBrutasDoItemAPartirDaSituacao.Substring(indexTextoTratamentoDiferenciadoMEEPP + textoTratamentoDiferenciadoMEEPP.Length);
                    var indexFinalTratamentoDiferenciadoMEEPP = conteudoAposTextoTratamentoDiferenciadoMEEPP.IndexOf(textoAdjudicadoEHomologadoPor);
                    var stringTratamentoDiferenciadoMEEPP = conteudoAposTextoTratamentoDiferenciadoMEEPP.Substring(0, indexFinalTratamentoDiferenciadoMEEPP);
                    tratamentoDiferenciadoMEEPP = stringTratamentoDiferenciadoMEEPP.Replace("\n", "").Replace("\r", "");
                }

                //coletando informações do pregoeiro
                var textoTravessaoAntesDoNomePregoeiro = " - ";
                var indexTravessaoAntesDoNomePregoeiro = informacoesBrutasDoItemAPartirDaSituacao.IndexOf(textoTravessaoAntesDoNomePregoeiro);
                var conteudoAPartirDoNomeDoPregoeiro = informacoesBrutasDoItemAPartirDaSituacao.Substring(indexTravessaoAntesDoNomePregoeiro).Replace("\n", " ").Replace("\r", " "); //retirando quebras de linhas para evitar problemas.
                var textoParaAposNomeDoPregoeiro = " para ";
                var indexParaAposONomeDoPregoeiro = conteudoAPartirDoNomeDoPregoeiro.IndexOf(textoParaAposNomeDoPregoeiro);
                var conteudoAPartirRazaoSocialFornecedorVencedor = conteudoAPartirDoNomeDoPregoeiro.Substring(indexParaAposONomeDoPregoeiro);
                var tamanhoNomeDoPregoeiro = (conteudoAPartirDoNomeDoPregoeiro.Length - textoTravessaoAntesDoNomePregoeiro.Length) - conteudoAPartirRazaoSocialFornecedorVencedor.Length;
                pregoeiro = conteudoAPartirDoNomeDoPregoeiro.Substring(textoTravessaoAntesDoNomePregoeiro.Length, tamanhoNomeDoPregoeiro);


                //coletando razao social fornecedor
                var textoVirgulaCNPJ = ", CNPJ ";
                var indexTextoVirgulaCNPJ = conteudoAPartirRazaoSocialFornecedorVencedor.IndexOf(textoVirgulaCNPJ);
                var conteudoAPartirTextoVirgulaCPNJ = conteudoAPartirRazaoSocialFornecedorVencedor.Substring(indexTextoVirgulaCNPJ);
                var tamanhoRazaoSocialFornecedor = (conteudoAPartirRazaoSocialFornecedorVencedor.Length - textoParaAposNomeDoPregoeiro.Length) - conteudoAPartirTextoVirgulaCPNJ.Length;
                razaoSocialFornecedorVencedorDoItem = conteudoAPartirRazaoSocialFornecedorVencedor.Substring(textoParaAposNomeDoPregoeiro.Length, tamanhoRazaoSocialFornecedor);

                //coletando CNPJ do fornecedor
                var textoMelhorLanceDoisPontos = ", melhor lance: R$ ";
                var indexTextoMelhorLanceDoisPontos = conteudoAPartirTextoVirgulaCPNJ.IndexOf(textoMelhorLanceDoisPontos);
                var conteudoAPartirTextoMelhorLanceDoisPontos = conteudoAPartirTextoVirgulaCPNJ.Substring(indexTextoMelhorLanceDoisPontos);
                var tamanhoCNPJCPFdoFornecedor = (conteudoAPartirTextoVirgulaCPNJ.Length - textoVirgulaCNPJ.Length) - conteudoAPartirTextoMelhorLanceDoisPontos.Length;
                cnpjFornecedorVencedorDoItem = conteudoAPartirTextoVirgulaCPNJ.Substring(textoVirgulaCNPJ.Length, tamanhoCNPJCPFdoFornecedor);

                //coletando melhor lance valor
                var conteudoAposTextoMelhorLanceDoItem = conteudoAPartirTextoMelhorLanceDoisPontos.Substring(textoMelhorLanceDoisPontos.Length);
                var textoValorNegociado = ", valor negociado: R$ ";
                var indexTextoValorNegociado = conteudoAposTextoMelhorLanceDoItem.IndexOf(textoValorNegociado);
                var stringMelhorLanceDoItem = "";


                if (indexTextoValorNegociado == -1)
                {
                    stringMelhorLanceDoItem = conteudoAposTextoMelhorLanceDoItem;
                    melhorLanceDoItem = Decimal.Parse(stringMelhorLanceDoItem); 
                }
                else
                {
                    stringMelhorLanceDoItem = conteudoAposTextoMelhorLanceDoItem.Substring(0, indexTextoValorNegociado);
                    melhorLanceDoItem = Decimal.Parse(stringMelhorLanceDoItem);
                    var conteudoAposTextoValorNegociado = conteudoAposTextoMelhorLanceDoItem.Substring(indexTextoValorNegociado + textoValorNegociado.Length);
                    var stringValorNegociado = conteudoAposTextoValorNegociado;
                    valorNegociadoDoItem = Decimal.Parse(stringValorNegociado);
                }

               


            }
            else
            {
                Console.WriteLine("Índices não encontrados. ETAPA: Coletando cada informacao individulmente");
            }

            item.Numero = numeroItem;
            item.Descricao = descricaoDoItem;
            item.DescricaoDetalhada = descricaoDetalhadaDoItem;
            item.Quantidade = quantidadeDoItem;
            item.ValorEstimado = valorEstimadoDoItem;
            item.UnidadeFornecimento = unidadeDeFornecimentoDoItem;
            item.Situacao = situacaoDoItem;
            item.Pregoeiro = pregoeiro;
            item.FornecedorNomeRazaoSocial = razaoSocialFornecedorVencedorDoItem;
            item.FornecedorCPFCNPJ = cnpjFornecedorVencedorDoItem;
            item.MelhorLance = melhorLanceDoItem;
            item.ValorNegociado = valorNegociadoDoItem;
            item.IntervaloMinimoEntreLances = intervaloMinimoEntreLances;
            item.TratamentoDiferenciadoMEEPP = tratamentoDiferenciadoMEEPP;

            Console.WriteLine("- - - - - - i t e m - - - - - - - - - - - - - - - - - - - - - -");
            Console.WriteLine("Número:                                   " + item.Numero);
            Console.WriteLine("Descrição:                                " + item.Descricao);
            Console.WriteLine("Descrição Detalhada:                      " + item.DescricaoDetalhada);
            Console.WriteLine("Quantidade:                               " + item.Quantidade);
            Console.WriteLine("Valor Estimado:                           " + item.ValorEstimado);
            Console.WriteLine("Unidade de Fornecimento:                  " + item.UnidadeFornecimento);
            Console.WriteLine("Situação:                                 " + item.Situacao);
            Console.WriteLine("Pregoeiro:                                " + item.Pregoeiro);
            Console.WriteLine("Razão Social do Fornecedor Vencedor:      " + item.FornecedorNomeRazaoSocial);
            Console.WriteLine("CNPJ/CPF do Fornecedor Vencedor:          " + item.FornecedorCPFCNPJ);
            Console.WriteLine("Melhor Lance:                             " + item.MelhorLance);
            Console.WriteLine("Valor Negociado:                          " + item.ValorNegociado);
            Console.WriteLine("Intervalo Minimo entre:                   " + item.IntervaloMinimoEntreLances);
            Console.WriteLine("Tratamento Diferenciado:                  " + item.TratamentoDiferenciadoMEEPP);
            Console.WriteLine(" - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");

            return item;
        }

        private static string IsolarInformacoesBrutasDoItem(string texto)
        {
            //Isolando informações do Item a partir de Eventos da Compra até Proposta do Item:
            try
            {
                var indexTituloEventosDaCompra = texto.IndexOf("\nEventos da compra\nData/Hora Descrição\n");
                var indexTituloPropostasDoItem = texto.IndexOf("\nPropostas do Item ");
                var conteudoBrutoContendoInformacoesDoItem = "";

                if (indexTituloEventosDaCompra >= 0 && indexTituloPropostasDoItem >= 0)
                {
                    // Calcula o comprimento da substring entre os dois índices
                    int startIndex = indexTituloEventosDaCompra + "\nEventos da compra\nData/Hora Descrição\n".Length;
                    int length = indexTituloPropostasDoItem - startIndex;

                    // Extrai a substring
                    conteudoBrutoContendoInformacoesDoItem = texto.Substring(startIndex, length);
                }

                return conteudoBrutoContendoInformacoesDoItem;
            }
            catch (Exception)
            {
                throw;
            }            
        }
    }
}

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

            Console.WriteLine(conteudoBrutoContendoInformacoesDoItem);

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
                var textoAdjudicadoEHomologadoPor = "\nAdjucado e Homologado por ";
                var indexTextoAdjudicadoEHomologadoPor = informacoesBrutasDoItemAPartirDaSituacao.IndexOf(textoAdjudicadoEHomologadoPor);
                var informacoesAPartirDoTextoAdjudicadoEHomologadoPor = informacoesBrutasDoItemAPartirDaSituacao.Substring(indexTextoAdjudicadoEHomologadoPor);
                var tamanhoSituacaoDoItem = (informacoesBrutasDoItemAPartirDaSituacao.Length - labelSituacao.Length) - informacoesAPartirDoTextoAdjudicadoEHomologadoPor.Length;
                situacaoDoItem = informacoesBrutasDoItemAPartirDaSituacao.Substring(labelSituacao.Length, tamanhoSituacaoDoItem);

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
                var stringMelhorLanceDoItem = conteudoAPartirTextoMelhorLanceDoisPontos.Substring(textoMelhorLanceDoisPontos.Length);
                melhorLanceDoItem = Decimal.Parse(stringMelhorLanceDoItem);

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

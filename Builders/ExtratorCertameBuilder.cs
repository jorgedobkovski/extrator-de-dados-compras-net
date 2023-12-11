using extratorTermoHomologacaoComprasNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extratorTermoHomologacaoComprasNet.Builders
{
    public static class ExtratorCertameBuilder
    {
        public static Certame ExtrairInformacoesDoCertame(string conteudoPdf)
        {
            Certame certame = new Certame();

            var conteudoBrutoContendoInformacoesDoCertame = IsolarInformacoesBrutasDoCertame(conteudoPdf);
            var conteudoBrutoContendoInformacoesDoCertameSemQuebrasDeLinha = conteudoBrutoContendoInformacoesDoCertame.Replace("\n", " ").Replace("\r", " ");

            //coletando numero e ano do certame
            var indexDaPrimeiraQubraDeLinha = conteudoBrutoContendoInformacoesDoCertame.IndexOf("\n");
            var conteudoAposTextoNumeroAnoAta = conteudoBrutoContendoInformacoesDoCertame.Substring(indexDaPrimeiraQubraDeLinha);
            var tamanhoTextoTituloNumeroAno = conteudoBrutoContendoInformacoesDoCertame.Length - conteudoAposTextoNumeroAnoAta.Length;
            var textoTituloNumeroAno = conteudoBrutoContendoInformacoesDoCertame.Substring(0, tamanhoTextoTituloNumeroAno); 

            var numeroAnoVetor = textoTituloNumeroAno.Split('/'); //divide o titulo pela barra no numero do certame e no ano
            var numeroCertame = numeroAnoVetor[0];
            var anoCertame = numeroAnoVetor[1];

            // coletando hora de homologação
            var textoAs = "\nÀs ";
            var indexTextoAs = conteudoAposTextoNumeroAnoAta.IndexOf(textoAs);
            var textoHorasDoDia = " horas do dia ";
            var indexTextoHorasDoDia = conteudoAposTextoNumeroAnoAta.IndexOf(textoHorasDoDia);
            var conteudoAposTextoHorasDoDia = conteudoAposTextoNumeroAnoAta.Substring(indexTextoHorasDoDia);
            var tamanhoHorasDeHomologacaoDoCertame = (conteudoAposTextoNumeroAnoAta.Length - textoAs.Length) - conteudoAposTextoHorasDoDia.Length;
            var horaDeHomologacao = conteudoAposTextoNumeroAnoAta.Substring(textoAs.Length, tamanhoHorasDeHomologacaoDoCertame);

            //coletando data de homologacao
            var textoDataHomologacao = " horas do dia ";
            var indexTextoDataHomologacao = conteudoBrutoContendoInformacoesDoCertameSemQuebrasDeLinha.IndexOf(textoDataHomologacao);
            var conteudoAposTextoDataHomologacao = conteudoBrutoContendoInformacoesDoCertameSemQuebrasDeLinha.Substring(indexTextoDataHomologacao + textoDataHomologacao.Length);
            var textoFimDataHomologacao = ", após constatada a regularidade dos atos procedimentais, ";
            var indexTextoFimDataHomologacao = conteudoAposTextoDataHomologacao.IndexOf(textoFimDataHomologacao);
            var dataHomologacao = conteudoAposTextoDataHomologacao.Substring(0, indexTextoFimDataHomologacao);
            dataHomologacao = dataHomologacao.Trim(); // Remover espaços extras antes ou depois da data de homologação

            //coletando informações do pregoeiro
            var textoPregoeiro = ", após constatada a regularidade dos atos procedimentais, a autoridade competente, ";
            var indexTextoPregoeiro = conteudoBrutoContendoInformacoesDoCertameSemQuebrasDeLinha.IndexOf(textoPregoeiro);
            var conteudoAposTextoPregoeiro = conteudoBrutoContendoInformacoesDoCertameSemQuebrasDeLinha.Substring(indexTextoPregoeiro + textoPregoeiro.Length);
            var indexFimPregoeiro = conteudoAposTextoPregoeiro.IndexOf(", HOMOLOGA ");
            var pregoeiro = conteudoAposTextoPregoeiro.Substring(0, indexFimPregoeiro);
            pregoeiro = pregoeiro.Trim(); // Remover espaços extras antes ou depois do nome do pregoeiro

            // coletando número do processo
            var textoNumeroProcesso = "referente ao Processo nº ";
            var indexTextoNumeroProcesso = conteudoAposTextoPregoeiro.IndexOf(textoNumeroProcesso);
            var conteudoAposTextoNumeroProcesso = conteudoAposTextoPregoeiro.Substring(indexTextoNumeroProcesso + textoNumeroProcesso.Length);
            var indexFimNumeroProcesso = conteudoAposTextoNumeroProcesso.IndexOf(", Pregão ");
            var numeroProcesso = conteudoAposTextoNumeroProcesso.Substring(0, indexFimNumeroProcesso);

            // Coletando a fundamentação legal
            var textoFundamentacaoLegal = "\nFundamentação legal: ";
            var indexTextoFundamentacaoLegal = conteudoBrutoContendoInformacoesDoCertame.IndexOf(textoFundamentacaoLegal);
            var conteudoAposTextoFundamentacaoLegal = conteudoBrutoContendoInformacoesDoCertame.Substring(indexTextoFundamentacaoLegal + textoFundamentacaoLegal.Length);
            var textoFimFundamentacaoLegal = " Característica: ";
            var indexTextoFimFundamentacaoLegal = conteudoAposTextoFundamentacaoLegal.IndexOf(textoFimFundamentacaoLegal);
            var fundamentacaoLegal = conteudoAposTextoFundamentacaoLegal.Substring(0, indexTextoFimFundamentacaoLegal);

            // Coletando a característica
            var textoCaracteristica = " Característica: ";
            var indexTextoCaracteristica = conteudoAposTextoFundamentacaoLegal.IndexOf(textoCaracteristica);
            var conteudoAposTextoCaracteristica = conteudoAposTextoFundamentacaoLegal.Substring(indexTextoCaracteristica + textoCaracteristica.Length);
            var textoFimCaracteristica = "\nCritério de julgamento: ";
            var indexTextoFimCaracteristica = conteudoAposTextoCaracteristica.IndexOf(textoFimCaracteristica);
            var caracteristica = conteudoAposTextoCaracteristica.Substring(0, indexTextoFimCaracteristica);

            // Coletando o critério de julgamento
            var textoCriterioJulgamento = "\nCritério de julgamento: ";
            var indexTextoCriterioJulgamento = conteudoAposTextoFundamentacaoLegal.IndexOf(textoCriterioJulgamento);
            var conteudoAposTextoCriterioJulgamento = conteudoAposTextoFundamentacaoLegal.Substring(indexTextoCriterioJulgamento + textoCriterioJulgamento.Length);
            var textoFimCriterioJulgamento = " Modo de disputa: ";
            var indexTextoFimCriterioJulgamento = conteudoAposTextoCriterioJulgamento.IndexOf(textoFimCriterioJulgamento);
            var criterioJulgamento = conteudoAposTextoCriterioJulgamento.Substring(0, indexTextoFimCriterioJulgamento);

            // Coletando o modo de disputa
            var textoModoDisputa = " Modo de disputa: ";
            var indexTextoModoDisputa = conteudoAposTextoCriterioJulgamento.IndexOf(textoModoDisputa);
            var conteudoAposTextoModoDisputa = conteudoAposTextoCriterioJulgamento.Substring(indexTextoModoDisputa + textoModoDisputa.Length);
            var textoFimModoDisputa = "\nCompra emergencial: ";
            var indexTextoFimModoDisputa = conteudoAposTextoModoDisputa.IndexOf(textoFimModoDisputa);
            var modoDisputa = conteudoAposTextoModoDisputa.Substring(0, indexTextoFimModoDisputa);

            // Coletando a informação sobre compra emergencial
            var textoCompraEmergencial = "Compra emergencial: ";
            var indexTextoCompraEmergencial = conteudoAposTextoModoDisputa.IndexOf(textoCompraEmergencial);
            var conteudoAposTextoCompraEmergencial = conteudoAposTextoModoDisputa.Substring(indexTextoCompraEmergencial + textoCompraEmergencial.Length);
            var textoFimCompraEmergencial = "\nObjeto da compra: ";
            var indexTextoFimCompraEmergencial = conteudoAposTextoCompraEmergencial.IndexOf(textoFimCompraEmergencial);
            var compraEmergencial = conteudoAposTextoCompraEmergencial.Substring(0, indexTextoFimCompraEmergencial);

            // Coletando o objeto da compra
            var textoObjetoCompra = "\nObjeto da compra: ";
            var indexTextoObjetoCompra = conteudoAposTextoCompraEmergencial.IndexOf(textoObjetoCompra);
            var conteudoAposTextoObjetoCompra = conteudoAposTextoCompraEmergencial.Substring(indexTextoObjetoCompra + textoObjetoCompra.Length);
            var conteudoAposTextoObjetoCompraSemQuebrasDeLinha = conteudoAposTextoObjetoCompra.Replace("\n", " ").Replace("\r", " ");
            var textoFimObjetoCompra = " Entrega de propostas: ";
            var indexTextoFimObjetoCompra = conteudoAposTextoObjetoCompraSemQuebrasDeLinha.IndexOf(textoFimObjetoCompra);
            var objetoCompra = conteudoAposTextoObjetoCompraSemQuebrasDeLinha.Substring(0, indexTextoFimObjetoCompra);

            // Coletando informações sobre a entrega de propostas
            var textoEntregaPropostas = "Entrega de propostas: ";
            var indexTextoEntregaPropostas = conteudoAposTextoObjetoCompra.IndexOf(textoEntregaPropostas);
            var conteudoAposTextoEntregaPropostas = conteudoAposTextoObjetoCompra.Substring(indexTextoEntregaPropostas + textoEntregaPropostas.Length);
            var textoFimEntregaPropostas = "\nAbertura da sessão pública: ";
            var indexTextoFimEntregaPropostas = conteudoAposTextoEntregaPropostas.IndexOf(textoFimEntregaPropostas);
            var informacaoEntregaPropostas = conteudoAposTextoEntregaPropostas.Substring(0, indexTextoFimEntregaPropostas);

            // Coletando informações sobre a abertura da sessão pública
            var textoAberturaSessaoPublica = "\nAbertura da sessão pública: ";
            var indexTextoAberturaSessaoPublica = conteudoBrutoContendoInformacoesDoCertame.IndexOf(textoAberturaSessaoPublica);
            var conteudoAposTextoAberturaSessaoPublica = conteudoBrutoContendoInformacoesDoCertame.Substring(indexTextoAberturaSessaoPublica + textoAberturaSessaoPublica.Length);
            var informacaoAberturaSessaoPublica = conteudoAposTextoAberturaSessaoPublica.Trim();

            certame.Numero = int.Parse(numeroCertame);
            certame.Ano = int.Parse(anoCertame);
            certame.HoraHomologacao = horaDeHomologacao;
            certame.DataHomologacao = dataHomologacao;
            certame.Pregoeiro = pregoeiro;
            certame.Processo = numeroProcesso;
            certame.FundamentacaoLegal = fundamentacaoLegal;
            certame.Caracteristica = caracteristica;
            certame.CriterioDeJulgamento = criterioJulgamento;
            certame.ModoDeDisputa = modoDisputa;
            certame.CompraEmergencial = compraEmergencial == "Sim";
            certame.ObjetoDaCompra = objetoCompra;
            certame.EntregaDePropostas = informacaoEntregaPropostas;
            certame.AberturaSessaoPublica = informacaoAberturaSessaoPublica;

            Console.WriteLine("I N F O R M A Ç Õ E S   A T É   O   M O M E N T O :");
            Console.WriteLine("- - - - - - c e r t a m e - - - - - - - - - - - - - - - - -");
            Console.WriteLine("Número certame:                " + certame.Numero);
            Console.WriteLine("Ano certame:                   " + certame.Ano);
            Console.WriteLine("Hora de Homologação:           " + certame.HoraHomologacao);
            Console.WriteLine("Data de Homologação:           " + certame.DataHomologacao);
            Console.WriteLine("Pregoeiro:                     " + certame.Pregoeiro);
            Console.WriteLine("Número do processo:            " + certame.Processo);
            Console.WriteLine("Fundamentação legal:           " + certame.FundamentacaoLegal);
            Console.WriteLine("Característica:                " + certame.Caracteristica);
            Console.WriteLine("Critério de julgamento:        " + certame.CriterioDeJulgamento);
            Console.WriteLine("Modo de disputa:               " + certame.ModoDeDisputa);
            Console.WriteLine("Compra emergencial:            " + certame.CompraEmergencial);
            Console.WriteLine("Objeto da compra:              " + certame.ObjetoDaCompra);
            Console.WriteLine("Entrega de propostas:          " + certame.EntregaDePropostas);
            Console.WriteLine("Abertura da sessão pública:    " + certame.AberturaSessaoPublica);
            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - -");

            return certame;
        }

        private static string IsolarInformacoesBrutasDoCertame(string texto)
        {
            //Isolando informações do Certame a partir do título Pregao Numero até título Mensagens do chat da compra
            try
            {
                var tituloPregaoNumero = "\nPREGÃO ";
                var indexTituloPregaoNumero = texto.IndexOf(tituloPregaoNumero);
                var tituloMensagensDoChatDaCompra = "\nMensagens do chat da compra\n";
                var indexTituloMensagensDoChatDaCompra = texto.IndexOf(tituloMensagensDoChatDaCompra);
                var conteudoBrutoContendoInformacoesDoCertame = "";

                if (indexTituloPregaoNumero >= 0 && indexTituloMensagensDoChatDaCompra >= 0)
                {
                    // Calcula o comprimento da substring entre os dois índices
                    int startIndex = indexTituloPregaoNumero + tituloPregaoNumero.Length;
                    int length = indexTituloMensagensDoChatDaCompra - startIndex;

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
    }
}

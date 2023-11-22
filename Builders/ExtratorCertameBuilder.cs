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

            Console.WriteLine(conteudoBrutoContendoInformacoesDoCertame);

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

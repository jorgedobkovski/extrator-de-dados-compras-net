using extratorTermoHomologacaoComprasNet.Models;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;

using (PdfReader leitor = new PdfReader("C:/temp/pdf/termo.pdf"))
{
    StringBuilder texto = new StringBuilder();
    for (int i = 1; i <= leitor.NumberOfPages; i++)
    {
        texto.Append(PdfTextExtractor.GetTextFromPage(leitor, i));
    }

    var conteudoPdf = texto.ToString();
    //Console.WriteLine(conteudoPdf);

    var item = new Item();

    //Isolando informações do Item a partir de Eventos da Compra até Proposta do Item:

    var indexTituloEventosDaCompra = conteudoPdf.IndexOf("\nEventos da compra\nData/Hora Descrição\n");

    var indexTituloPropostasDoItem = conteudoPdf.IndexOf("\nPropostas do Item ");

    var conteudoBrutoContendoInformacoesDoItem = "";

    if (indexTituloEventosDaCompra >= 0 && indexTituloPropostasDoItem >= 0)
    {
        // Calcula o comprimento da substring entre os dois índices
        int startIndex = indexTituloEventosDaCompra + "\nEventos da compra\nData/Hora Descrição\n".Length;
        int length = indexTituloPropostasDoItem - startIndex;

        // Extrai a substring
        conteudoBrutoContendoInformacoesDoItem = conteudoPdf.Substring(startIndex, length);
    }
    else
    {
        Console.WriteLine("Índices não encontrados. ETAPA: Coletando informações brutas a entre os eventos da compra e as propostas do item.");
    }

    //Separando as informações dos itens
    int indexTituloItemNumero = -1;
    var conteudoDoItem = "";

    if (conteudoBrutoContendoInformacoesDoItem != "")
    {
        indexTituloItemNumero = conteudoBrutoContendoInformacoesDoItem.IndexOf("\nItem ");
        conteudoDoItem = conteudoBrutoContendoInformacoesDoItem.Substring(indexTituloItemNumero);
    } else
    {
        Console.WriteLine("Índices não encontrados. ETAPA: Separando as informações dos itens das informações brutas (eventos da compra - propostas)");
    }

    Console.WriteLine(conteudoDoItem);
    Console.ReadLine();
}

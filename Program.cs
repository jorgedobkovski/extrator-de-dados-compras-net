using extratorTermoHomologacaoComprasNet.Models;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Drawing;
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

    //Separando as informações brutas dos itens
    int indexTituloItemNumero = -1;
    var conteudoDoItem = "";

    if (conteudoBrutoContendoInformacoesDoItem != "")
    {
        indexTituloItemNumero = conteudoBrutoContendoInformacoesDoItem.IndexOf("Item ");
        conteudoDoItem = conteudoBrutoContendoInformacoesDoItem.Substring(indexTituloItemNumero);
    } else
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
    string melhorLanceDoItem = "";
    string fornecedorVencedorDoItem = "";

    if(conteudoDoItem != "" && indexTituloItemNumero != -1)
    {

        //separando título Item X - Descricao
        var indexPrimeiraQuebraDeLinhaPosDescricao = conteudoDoItem.IndexOf('\n');
        var tituloItemNumeroDescricao = conteudoDoItem.Substring(0, indexPrimeiraQuebraDeLinhaPosDescricao);
        var indexHifenPosNumero = tituloItemNumeroDescricao.IndexOf(" - ");

        //seprando numero do item e convertendo para int
        var stringNumeroDoItem = tituloItemNumeroDescricao.Substring(5, indexHifenPosNumero-5); //"Item " = 5 caracteres
        numeroItem = int.Parse(stringNumeroDoItem);

        //separando descricao do item
        descricaoDoItem = tituloItemNumeroDescricao.Substring(indexHifenPosNumero + 3);

        //separando descricao detalhada
        var conteudoDoItemSemTitulo = conteudoDoItem.Substring(tituloItemNumeroDescricao.Length + 1);
        var indexLabelQuantidadeDoisPontos = conteudoDoItem.IndexOf("\nQuantidade: ");
        var informacoesDoItem = conteudoDoItem.Substring(indexLabelQuantidadeDoisPontos+1);
        var tamanhoDescricaoDetalhadaDoItem = (conteudoDoItemSemTitulo.Length - informacoesDoItem.Length)-1; //-1 para tirar a quebra de linha
        descricaoDetalhadaDoItem = conteudoDoItemSemTitulo.Substring(0, tamanhoDescricaoDetalhadaDoItem);

        //separando quantidade do item
        var indexDaPrimeiraQuebraDeLinhaPosLabelQuantidade = informacoesDoItem.IndexOf("\n");
        var conteudoLinhaComQuantidadeEValorEstimado = informacoesDoItem.Substring(0, indexDaPrimeiraQuebraDeLinhaPosLabelQuantidade);
        var labelValorEstimado = " Valor estimado: R$ ";
        var indexLabelValorEstimadoDoisPontos = conteudoLinhaComQuantidadeEValorEstimado.IndexOf(labelValorEstimado);
        var stringQuantidadeDoItem = conteudoLinhaComQuantidadeEValorEstimado.Substring(12, indexLabelValorEstimadoDoisPontos-12); //"Quantidade: " = 12 caracteres
        quantidadeDoItem = Decimal.Parse(stringQuantidadeDoItem);

        //separando valor estimado
        var stringValorEstimadoDoItem = conteudoLinhaComQuantidadeEValorEstimado.Substring(indexLabelValorEstimadoDoisPontos + labelValorEstimado.Length);
        valorEstimadoDoItem = Decimal.Parse(stringValorEstimadoDoItem);

        Console.WriteLine(valorEstimadoDoItem);
    } else
    {
        Console.WriteLine("Índices não encontrados. ETAPA: Coletando cada informacao individulmente");
    }

    Console.ReadLine();
}

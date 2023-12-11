using extratorTermoHomologacaoComprasNet.Builders;
using extratorTermoHomologacaoComprasNet.Models;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Drawing;
using System.Text;

using (PdfReader leitor = new PdfReader("C:/temp/pdf/termoA.pdf"))
{
    StringBuilder texto = new StringBuilder();
    for (int i = 1; i <= leitor.NumberOfPages; i++)
    {
        texto.Append(PdfTextExtractor.GetTextFromPage(leitor, i));
    }

    var conteudoPdf = texto.ToString();
    //Console.WriteLine(conteudoPdf);

    var certame = ExtratorCertameBuilder.ExtrairInformacoesDoCertame(conteudoPdf);
    var item = ExtratorItemBuilder.ExtrairInformacoesDoItem(conteudoPdf);
    var propostas = ExtratorPropostaBuilder.ExtrairInformacoesDaProposta(conteudoPdf);

    Console.ReadLine();


}

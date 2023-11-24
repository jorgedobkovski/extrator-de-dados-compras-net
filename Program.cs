using extratorTermoHomologacaoComprasNet.Builders;
using extratorTermoHomologacaoComprasNet.Models;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Drawing;
using System.Text;

using (PdfReader leitor = new PdfReader("C:/temp/pdf/termoD.pdf"))
{
    StringBuilder texto = new StringBuilder();
    for (int i = 1; i <= leitor.NumberOfPages; i++)
    {
        texto.Append(PdfTextExtractor.GetTextFromPage(leitor, i));
    }

    var conteudoPdf = texto.ToString();
    //Console.WriteLine(conteudoPdf);

    var certame = ExtratorCertameBuilder.ExtrairInformacoesDoCertame(conteudoPdf);

    //Console.WriteLine("Número:                                   " + item.Numero);
    //Console.WriteLine("Descrição:                                " + item.Descricao);
    //Console.WriteLine("Descrição Detalhada:                      " + item.DescricaoDetalhada);
    //Console.WriteLine("Quantidade:                               " + item.Quantidade);
    //Console.WriteLine("Valor Estimado:                           " + item.ValorEstimado);
    //Console.WriteLine("Unidade de Fornecimento:                  " + item.UnidadeFornecimento);
    //Console.WriteLine("Situação:                                 " + item.Situacao);
    //Console.WriteLine("Pregoeiro:                                " + item.Pregoeiro);
    //Console.WriteLine("Razão Social do Fornecedor Vencedor:      " + item.FornecedorNomeRazaoSocial);
    //Console.WriteLine("CNPJ/CPF do Fornecedor Vencedor:          " + item.FornecedorCPFCNPJ);
    //Console.WriteLine("Melhor Lance:                             " + item.MelhorLance);

    //Console.ReadLine();


}

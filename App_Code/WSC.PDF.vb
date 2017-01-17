Imports Microsoft.VisualBasic
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports System.IO

Namespace WSC
    Public Class PDF
        Public Shared Function HtmlToPDF(html As String) As Byte()
            Dim converter = New NReco.PdfGenerator.HtmlToPdfConverter()
            converter.Size = NReco.PdfGenerator.PageSize.Letter
            converter.CustomWkHtmlArgs = "--encoding utf-8"
            Return converter.GeneratePdf(html)
        End Function
    End Class

End Namespace




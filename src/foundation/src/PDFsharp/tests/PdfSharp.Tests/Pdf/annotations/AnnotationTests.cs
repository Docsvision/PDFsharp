// PDFsharp - A .NET library for processing PDF
// See the LICENSE file in the solution root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using FluentAssertions;
using PdfSharp.Diagnostics;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Annotations;
using PdfSharp.Pdf.IO;
using PdfSharp.Quality;
using PdfSharp.Snippets.Font;
using PdfSharp.TestHelper;
using Xunit;

namespace PdfSharp.Tests.PDF
{
    [Collection("PDFsharp")]
    public class AnnotationTests
    {
        [Fact(Skip = "Disabled until /Annots bug is fixed")]
        public void Exception_with_duplicate_annotations()
        {
            var document1 = new PdfDocument();
            var page1 = document1.AddPage();
            page1.AddDocumentLink(new(new XRect(new XSize(1, 1))), 1);
            var stream1 = new MemoryStream();
            document1.Save(stream1, false);
            stream1.Position = 0;

            var document2 = new PdfDocument();
            var page2 = document2.AddPage();
            page2.AddDocumentLink(new(new XRect(new XSize(1, 1))), 1);
            var stream2 = new MemoryStream();
            document2.Save(stream2, false);
            stream2.Position = 0;

            var documentInput1 = PdfReader.Open(stream1, PdfDocumentOpenMode.Import);
            var documentInput2 = PdfReader.Open(stream2, PdfDocumentOpenMode.Import);

            var documentMerged = new PdfDocument();
            var resultPagesOffset = documentMerged.PageCount;
            documentMerged.Pages.InsertRange(resultPagesOffset, documentInput1);
            resultPagesOffset = documentMerged.PageCount;
            documentMerged.Pages.InsertRange(resultPagesOffset, documentInput2);
            documentMerged.Pages.Count.Should().Be(2);
        }

        [Fact]
        public void TestPdfImageStampAnnotation()
        {
            var imagePath = "pdfsharp-6.x/images/jpeg/extern/Zoo_JPEG_8BIM.jpg";
            var fullName = IOUtility.GetAssetsPath(imagePath)!;
            var image = XImage.FromFile(fullName);

            var pdf = new PdfDocument();
            var page = pdf.AddPage();

            var annotation = new PdfImageStampAnnotation(pdf, image);
            var location = new XPoint(200, 200);
            var size = new XSize(200, 100);
            var rectangle = new XRect(location, size);
            annotation.Rectangle = new PdfRectangle(rectangle);
           
            page.Annotations.Add(annotation);

            var stream = new MemoryStream();
            pdf.Save(stream);
            pdf.Close();

            stream.Seek(0, SeekOrigin.Begin);
            var loadedPdf = PdfReader.Open(stream);
            var loadedPage = loadedPdf.Pages[0];
            loadedPage.Annotations.Count.Should().Be(1);
            var loadedAnnotation = loadedPage.Annotations[0];
            loadedAnnotation.Should().NotBeNull();
            loadedAnnotation.Rectangle.Location.Should().Be(location);
            loadedAnnotation.Rectangle.Size.Should().Be(size);
        }
    }
}

// PDFsharp - A .NET library for processing PDF
// See the LICENSE file in the solution root for more information.

using PdfSharp.Drawing;
using PdfSharp.Pdf.Advanced;

namespace PdfSharp.Pdf.Annotations
{
    /// <summary>
    /// Represents a PDF image stamp annotation.
    /// </summary>
    public sealed class PdfImageStampAnnotation : PdfAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfImageStampAnnotation"/> class.
        /// </summary>
        /// <param name="document">The PDF document to which this annotation belongs.</param>
        /// <param name="image">The image to be used for the stamp.</param>
        public PdfImageStampAnnotation(PdfDocument document, XImage image)
            : base(document)
        {
            InitializeStamp();
            SetAppearance(image);
        }

        void InitializeStamp()
        {
            Elements.SetName(Keys.Subtype, "/Stamp");
        }

        void SetAppearance(XImage image)
        {
            var pdfForm = CreatePdfFormFromImage(image);
            var appearance = new PdfDictionary(this.Owner);
            appearance.Elements[Keys.N] = pdfForm.Reference;
            this.Elements[PdfAnnotation.Keys.AP] = appearance;
        }

        PdfFormXObject CreatePdfFormFromImage(XImage xImage)
        {
            var xSize = new XSize(xImage.PixelWidth, xImage.PixelHeight);
            var xForm = new XForm(this.Owner, xSize);
            using (XGraphics graphics = XGraphics.FromForm(xForm))
            {
                graphics.DrawImage(xImage, 0, 0, xImage.PixelWidth, xImage.PixelHeight);
            }
            return xForm.PdfForm;
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal new class Keys : PdfAnnotation.Keys
        {
            /// <summary>
            /// (Optional) The normal appearance of the annotation.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string N = "/N";

            /// <summary>
            /// Gets meta information for the <see cref="Keys"/> class.
            /// </summary>
            public static DictionaryMeta Meta => _meta ??= CreateMeta(typeof(Keys));

            static DictionaryMeta? _meta;
        }

        /// <summary>
        /// Gets the KeysMeta of this dictionary type.
        /// </summary>
        internal override DictionaryMeta Meta => Keys.Meta;
    }
}

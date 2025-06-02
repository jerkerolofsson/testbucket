using System.Text;
using System.Text.RegularExpressions;

using TestBucket.Contracts.Requirements;
using TestBucket.Domain.Files.Models;

using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;

namespace TestBucket.Domain.Requirements.Import.Strategies
{
    public class PdfImporter : IDocumentImportStrategy
    {

        private static readonly Regex s_regexOrderedList = new Regex(@"\d*\.*$");

        // Numeric headers
        private static readonly Regex s_h4 = new Regex(@"^\d*\.\d*\.\d*\.\d", RegexOptions.Multiline);
        private static readonly Regex s_h3 = new Regex(@"^\d*\.\d*\.\d", RegexOptions.Multiline);
        private static readonly Regex s_h2 = new Regex(@"^\d\.\d", RegexOptions.Multiline);
        private static readonly Regex s_h1 = new Regex(@"^\d\.", RegexOptions.Multiline);

        public Task ImportAsync(RequirementSpecificationDto spec, FileResource fileResource)
        {
            string name = fileResource.Name ?? "pdf";
            var text = new StringBuilder();
            using (PdfDocument document = PdfDocument.Open(fileResource.Data))
            {
                foreach (Page page in document.GetPages())
                {
                    var letters = page.Letters; // no preprocessing

                    // 1. Extract words
                    var wordExtractor = NearestNeighbourWordExtractor.Instance;

                    var words = wordExtractor.GetWords(letters);

                    // 2. Segment page
                    var pageSegmenterOptions = new DocstrumBoundingBoxes.DocstrumBoundingBoxesOptions()
                    {

                    };

                    var pageSegmenter = new DocstrumBoundingBoxes(pageSegmenterOptions);

                    var textBlocks = pageSegmenter.GetBlocks(words);

                    var blocks = RecursiveXYCut.Instance.GetBlocks(words);

                    // 3. Postprocessing
                    var readingOrder = UnsupervisedReadingOrderDetector.Instance;
                    var orderedTextBlocks = readingOrder.Get(blocks);

                    // 4. Add debug info - Bounding boxes and reading order

                    int bulletCounter = 0;

                    foreach (var block in orderedTextBlocks)
                    {
                        var textInBlock = block.Text.Normalize(NormalizationForm.FormKC);

                        // Special case for ordered lists, e.g. "1.", "2." ... "10."
                        bool isOrderedList = s_regexOrderedList.IsMatch(textInBlock);
                        bool isH1 = s_h1.Match(textInBlock).Success;
                        bool isH2 = s_h2.Match(textInBlock).Success;
                        bool isH3 = s_h3.Match(textInBlock).Success;
                        bool isH4 = s_h4.Match(textInBlock).Success;

                        // If not a header, see if we can match by font size
                        var maxFontSize = block.TextLines.SelectMany(w => w.Words).SelectMany(w => w.Letters).Max(l => l.FontSize);
                        if(!isH1 && !isH2 && !isH3 && !isH4)
                        {
                            if (maxFontSize >= 20)
                            {
                                isH1 = true;
                            }
                            else if (maxFontSize >= 16)
                            {
                                isH2 = true;
                            }
                            else if (maxFontSize >= 14)
                            {
                                isH3 = true;
                            }
                        }

                        // Special handling for unordered-lists
                        if (textInBlock == "•")
                        {
                            bulletCounter++;
                            continue;
                        }
                        else
                        {
                            if (bulletCounter > 0)
                            {
                                var ul = new StringBuilder();
                                foreach (var line in textInBlock.Split('\n'))
                                {
                                    if (bulletCounter > 0)
                                    {
                                        ul.AppendLine("- " + line);
                                        bulletCounter--;
                                    }
                                    else
                                    {
                                        ul.AppendLine(line);
                                    }
                                }
                                textInBlock = ul.ToString();
                            }
                        }

                        if (isH4)
                        {
                            text.Append("\n#### ");
                            text.Append(textInBlock);
                            text.AppendLine();
                            text.AppendLine();
                        }
                        else if (isH3)
                        {
                            text.Append("\n### ");
                            text.Append(textInBlock);
                            text.AppendLine();
                            text.AppendLine();
                        }
                        else if (isH2)
                        {
                            text.Append("\n## ");
                            text.Append(textInBlock);
                            text.AppendLine();
                            text.AppendLine();
                        }

                        else if (isH1)
                        {
                            text.Append("\n# ");
                            text.Append(textInBlock);
                            text.AppendLine();
                            text.AppendLine();
                        }
                        else
                        {
                            text.Append(textInBlock);
                            text.AppendLine();
                        }
                    }
                }
            }

            spec.Description = text.ToString();
            return Task.FromResult(spec);
        }

    }
}

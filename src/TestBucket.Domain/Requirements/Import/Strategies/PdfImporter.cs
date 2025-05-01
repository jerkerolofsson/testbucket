using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using TestBucket.Domain.Files.Models;
using TestBucket.Domain.Requirements.Models;

using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using UglyToad.PdfPig;
using TestBucket.Contracts.Requirements;

namespace TestBucket.Domain.Requirements.Import.Strategies
{
    class PdfImporter : IDocumentImportStrategy
    {

        private static readonly Regex s_regexOrderedList = new Regex(@"\d*\.$");

        // Numeric headers
        private static readonly Regex s_h4 = new Regex(@"^\d*\.\d*\.\d*\.\d*$");
        private static readonly Regex s_h3 = new Regex(@"^\d*\.\d*\.\d*$");
        private static readonly Regex s_h2 = new Regex(@"^\d*\.\d*$");
        private static readonly Regex s_h1 = new Regex(@"^\d*$");

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
                        bool isH1 = s_h1.IsMatch(textInBlock);
                        bool isH2 = s_h2.IsMatch(textInBlock);
                        bool isH3 = s_h3.IsMatch(textInBlock);
                        bool isH4 = s_h4.IsMatch(textInBlock);

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

                        if (isOrderedList)
                        {
                            text.Append(textInBlock);
                        }
                        else
                        {
                            if (isH1)
                            {
                                text.Append("\n# ");
                                text.Append(textInBlock);
                                text.Append(' ');
                            }
                            else if (isH2)
                            {
                                text.Append("\n## ");
                                text.Append(textInBlock);
                                text.Append(' ');
                            }
                            else if (isH3)
                            {
                                text.Append("\n### ");
                                text.Append(textInBlock);
                                text.Append(' ');
                            }
                            else if (isH4)
                            {
                                text.Append("\n#### ");
                                text.Append(textInBlock);
                                text.Append(' ');
                            }
                            else
                            {
                                text.Append(textInBlock);
                                text.AppendLine();
                            }
                        }
                    }
                }
            }

            spec.Description = text.ToString();
            return Task.FromResult(spec);
        }

    }
}

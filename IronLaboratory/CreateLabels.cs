using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace IronLaboratory
{
    public class CreateLabels
    {
        private const double PaperSizeWidth = 192;
        private const double PaperSizeHeight = 98;

        private const double LeftMargin = 8;
        private const double TopMargin = 3.5;

        private FixedPage CreatePage()
        {
            FixedPage page = new FixedPage
            {
                Background = Brushes.White,
                Width = PaperSizeWidth,
                Height = PaperSizeHeight

            };
            return page;
        }

        public FixedDocument CreateDocument(DataTable dataTable, List<int> nList)
        {
            int[] num = nList.ToArray();

            FixedDocument doc = new FixedDocument();
            doc.DocumentPaginator.PageSize = new Size(PaperSizeWidth, PaperSizeHeight);

            double pageCount = dataTable.Rows.Count;

            if (pageCount > 0)
            {
                for (int i = 0; i < pageCount; i++)
                {
                    string s1 = (string)dataTable.Rows[i]["Registration"];
                    string s2 = (string)dataTable.Rows[i]["Material"];
                    string s3 = (string)dataTable.Rows[i]["Experiment"];
                    string s4 = (string)dataTable.Rows[i]["Clock"];
                    string s5 = DateConversion.ConvertToPersian((DateTime)dataTable.Rows[i]["OnDate"]);
                    int id = (int)dataTable.Rows[i]["SampleId"];
                    string s6 = (string)dataTable.Rows[i]["OrderId"];

                    for (int n = 1; n <= num[i]; n++)
                    {
                        SingleLabelUC label = new SingleLabelUC(s1, s2, s3, s4, s5, id, s6);

                        PageContent page = new PageContent();
                        FixedPage fixedPage = this.CreatePage();

                        FixedPage.SetTop(label, TopMargin);
                        FixedPage.SetLeft(label, LeftMargin);
                        fixedPage.Children.Add(label);

                        fixedPage.Measure(new Size(PaperSizeWidth, PaperSizeHeight));
                        fixedPage.Arrange(new Rect(new Point(), new Size(PaperSizeWidth, PaperSizeHeight)));
                        fixedPage.UpdateLayout();


                        ((IAddChild)page).AddChild(fixedPage);
                        doc.Pages.Add(page);
                    }
                }
            }
            return doc;
        }
    }
}

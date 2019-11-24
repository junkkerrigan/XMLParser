using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace XMLProcessing
{
    public class CD
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Country { get; set; }
        public string Company { get; set; }
        public float Price { get; set ; }
        public int Year { get; set; }
    }

    public partial class XMLParser : Form
    {
        RichTextBox ContentContainer;
        ComboBox TitleFilter, ArtistFilter, CountryFilter, CompanyFilter;
        TextBox PriceFilterFrom, PriceFilterTo, YearFilterFrom, YearFilterTo;
        XDocument Catalog;

        public XMLParser()
        {
            InitializeComponent();
            ContentContainer = new RichTextBox
            {
                Location = new Point(30, 30),
                Size = new Size((Width - 120) / 2, ClientSize.Height - 80)
            };
            InitializeFilters();
            LoadData();

            SizeChanged += XMLParser_SizeChanged;

            Controls.Add(ContentContainer);
            Controls.Add(TitleFilter);
            Controls.Add(ArtistFilter);
            Controls.Add(CountryFilter);
            Controls.Add(CompanyFilter);
            Controls.Add(PriceFilterFrom);
            Controls.Add(PriceFilterTo);
            Controls.Add(YearFilterFrom);
            Controls.Add(YearFilterTo);
        }

        void LoadData()
        {
            Catalog = XDocument.Load("../../data.xml");
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            var data = from cd in Catalog.Element("CATALOG").Elements("CD")
                       select new CD
                       {
                           Title = cd.Element("TITLE").Value,
                           Artist = cd.Element("ARTIST").Value,
                           Country = cd.Element("COUNTRY").Value,
                           Company = cd.Element("COMPANY").Value,
                           Price = Convert.ToSingle(cd.Element("PRICE").Value, nfi),
                           Year = Convert.ToInt32(cd.Element("YEAR").Value, nfi),
                       };
            Console.WriteLine(data.ToList()[0].Artist);
        }

        void InitializeFilters()
        {
            TitleFilter = new ComboBox
            {
                Location = new Point(90 + ContentContainer.Width, 30),
                Size = new Size(ContentContainer.Width - 30, 100),
                Font = new Font("Verdana", 16),
                Text = "Title"
            };

            ArtistFilter = new ComboBox
            {
                Location = new Point(90 + ContentContainer.Width,
                TitleFilter.Bounds.Bottom + 30),
                Size = new Size(ContentContainer.Width - 30, 100),
                Font = new Font("Verdana", 16),
                Text = "Artist",
            };

            CountryFilter = new ComboBox
            {
                Location = new Point(90 + ContentContainer.Width,
                ArtistFilter.Bounds.Bottom + 30),
                Size = new Size(ContentContainer.Width - 30, 100),
                Font = new Font("Verdana", 16),
                Text = "Country",
            };

            CompanyFilter = new ComboBox
            {
                Location = new Point(90 + ContentContainer.Width,
                CountryFilter.Bounds.Bottom + 30),
                Size = new Size(ContentContainer.Width - 30, 100),
                Font = new Font("Verdana", 16),
                Text = "Company",
            };

            PriceFilterFrom = new TextBox
            {
                Location = new Point(90 + ContentContainer.Width,
                CompanyFilter.Bounds.Bottom + 30),
                Size = new Size((ContentContainer.Width - 70) / 2, 100),
                Font = new Font("Verdana", 14),
                Text = "Price: from",
            };

            PriceFilterTo = new TextBox
            {
                Location = new Point((190 + 3 * ContentContainer.Width) / 2,
                CompanyFilter.Bounds.Bottom + 30),
                Size = new Size((ContentContainer.Width - 70) / 2, 100),
                Font = new Font("Verdana", 14),
                Text = "Price: to",
            };

            YearFilterFrom = new TextBox
            {
                Location = new Point(90 + ContentContainer.Width,
                PriceFilterTo.Bounds.Bottom + 20),
                Size = new Size((ContentContainer.Width - 70) / 2, 100),
                Font = new Font("Verdana", 14),
                Text = "Year: from",
            };

            YearFilterTo = new TextBox
            {
                Location = new Point((190 + 3 * ContentContainer.Width) / 2,
                PriceFilterTo.Bounds.Bottom + 20),
                Size = new Size((ContentContainer.Width - 70) / 2, 100),
                Font = new Font("Verdana", 14),
                Text = "Year: to",
            };
        }

        void XMLParser_SizeChanged(object sender, EventArgs e)
        {
            ContentContainer.Size = new Size((Width - 120) / 2, Height - 90);

            TitleFilter.Location = new Point(90 + ContentContainer.Width, 30);
            TitleFilter.Size = new Size(ContentContainer.Width - 30, 100);

            ArtistFilter.Location = new Point(90 + ContentContainer.Width,
                TitleFilter.Bounds.Bottom + 30);
            ArtistFilter.Size = new Size(ContentContainer.Width - 30, 100);

            CountryFilter.Location = new Point(90 + ContentContainer.Width,
                ArtistFilter.Bounds.Bottom + 30);
            CountryFilter.Size = new Size(ContentContainer.Width - 30, 100);

            CompanyFilter.Location = new Point(90 + ContentContainer.Width,
                CountryFilter.Bounds.Bottom + 30);
            CompanyFilter.Size = new Size(ContentContainer.Width - 30, 100);

            PriceFilterFrom.Location = new Point(90 + ContentContainer.Width,
                CompanyFilter.Bounds.Bottom + 30);
            PriceFilterFrom.Size = new Size((ContentContainer.Width - 70) / 2, 100);

            PriceFilterTo.Location = new Point((190 + 3 * ContentContainer.Width) / 2,
                CompanyFilter.Bounds.Bottom + 30);
            PriceFilterTo.Size = new Size((ContentContainer.Width - 70) / 2, 100);

            YearFilterFrom.Location = new Point(90 + ContentContainer.Width,
                PriceFilterTo.Bounds.Bottom + 20);
            YearFilterFrom.Size = new Size((ContentContainer.Width - 70) / 2, 100);

            YearFilterTo.Location = new Point((190 + 3 * ContentContainer.Width) / 2,
                PriceFilterTo.Bounds.Bottom + 20);
            YearFilterTo.Size = new Size((ContentContainer.Width - 70) / 2, 100);
        }
    }
}

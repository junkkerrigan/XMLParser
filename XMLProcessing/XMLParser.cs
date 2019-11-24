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
        public string Price { get; set; }
        public string Year { get; set; }

        public string InfoToDisplay(int cnt)
        {
            return
                $"CD #{cnt}\n" +
                $"Title: {Title}\n" +
                $"Artist: {Artist}\n" +
                $"Country: {Country}\n" +
                $"Company: {Company}\n" +
                $"Price: {Price}\n" +
                $"Year: {Year}\n";
        }
    }

    public class DiskFilter 
    {
        static NumberFormatInfo nfi = new NumberFormatInfo
        {
            NumberDecimalSeparator = "."
        };
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string PriceFrom { get; set; } = float.MinValue.ToString(nfi);
        public string PriceTo { get; set; } = float.MaxValue.ToString(nfi);
        public string YearFrom { get; set; } = int.MinValue.ToString(nfi);
        public string YearTo { get; set; } = int.MaxValue.ToString(nfi);
    }

    public class Catalog
    {
        List<CD> _disks;
        
        public Catalog()
        {
            _disks = new List<CD>();
        }

        public void addDisk(CD disk)
        {
            _disks.Add(disk);
        }

        public void addDisks(List<CD> disks)
        {
            _disks.AddRange(disks);
        }

        public List<CD> Select(DiskFilter filterBy)
        {
            NumberFormatInfo nfi = new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            };
            foreach (var p in filterBy.GetType().GetProperties())
            {
                Console.WriteLine($"{p.Name}: {p.GetValue(filterBy)}");
            }
            return new List<CD>(_disks
                .Where(d => 
                    d.Title.ToLower().Contains(filterBy.Title.ToLower().Trim())
                    || string.IsNullOrWhiteSpace(filterBy.Title)
                )
                .Where(d => 
                    d.Company.ToLower().Contains(filterBy.Company.ToLower().Trim())
                    || string.IsNullOrWhiteSpace(filterBy.Company)
                )
                .Where(d => 
                    d.Country.ToLower().Contains(filterBy.Country.ToLower().Trim())
                    || string.IsNullOrWhiteSpace(filterBy.Country)
                )
                .Where(d => 
                    d.Artist.ToLower().Contains(filterBy.Artist.ToLower().Trim())
                    || string.IsNullOrWhiteSpace(filterBy.Artist)
                )
                .Where(d => {
                    if (string.IsNullOrWhiteSpace(filterBy.YearFrom)) return true;
                    if (!float.TryParse(filterBy.YearFrom, out float yearFilter)) return false;
                    return float.Parse(d.Year, nfi) >= yearFilter;
                })
                .Where(d => {
                    if (string.IsNullOrWhiteSpace(filterBy.YearTo)) return true;
                    if (!float.TryParse(filterBy.YearTo, out float yearFilter)) return false;
                    return float.Parse(d.Year, nfi) <= yearFilter;
                })
                .Where(d => {
                    if (string.IsNullOrWhiteSpace(filterBy.PriceFrom)) return true;
                    try
                    {
                        float priceFilter = Convert.ToSingle(filterBy.PriceFrom, nfi);
                        return float.Parse(d.Price, nfi) >= priceFilter;
                    }
                    catch
                    {
                        return false;
                    }
                })
                .Where(d =>
                {
                    if (string.IsNullOrWhiteSpace(filterBy.PriceTo)) return true;
                    try
                    {
                        float priceFilter = Convert.ToSingle(filterBy.PriceTo, nfi);
                        return float.Parse(d.Price, nfi) <= priceFilter;
                    }
                    catch
                    {
                        return false;
                    }
                }));
        }

        public string FilteredToDisplay(DiskFilter filter)
        {
            string res = "";
            var filtered = Select(filter);
            for (int i = 1; i <= filtered.Count(); i++)
            {
                res += filtered[i - 1].InfoToDisplay(i) + "\n\n";
            }
            return res;
        }
    }

    public partial class XMLParser : Form
    {
        /*static Dictionary<string, string> _placeholders = new Dictionary<string, string>()
        {
            { "Title", "Title" },
            { "Artist", "Artist" },
            { "Company", "Company" },
            { "Country", "Country" },
            { "PriceFrom", "Price: from" },
            { "PriceTo", "Price: to" },
            { "YearFrom", "Year: from" },
            { "YearTo", "Year: to" },
        };*/
        RichTextBox ContentContainer;
        ComboBox TitleFilter, ArtistFilter, CountryFilter, CompanyFilter;
        TextBox PriceFilterFrom, PriceFilterTo, YearFilterFrom, YearFilterTo;
        XDocument XMLCatalog;
        DiskFilter FiltersData = new DiskFilter();
        Catalog Disks = new Catalog();

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
            FiltersData = new DiskFilter();

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
            XMLCatalog = XDocument.Load("../../data.xml");
            Disks.addDisks(
                (from cd in XMLCatalog.Element("CATALOG").Elements("CD")
                select new CD
                {
                    Title = cd.Element("TITLE").Value,
                    Artist = cd.Element("ARTIST").Value,
                    Country = cd.Element("COUNTRY").Value,
                    Company = cd.Element("COMPANY").Value,
                    Price = cd.Element("PRICE").Value,
                    Year = cd.Element("YEAR").Value,
                })
                .ToList()
            );
            FillSuggestions();
            ContentContainer.Text = Disks.FilteredToDisplay(FiltersData);
        }

        void FillSuggestions()
        {
            TitleFilter.Items.AddRange(
                (from cd in XMLCatalog.Element("CATALOG").Elements("CD")
                 select cd.Element("TITLE").Value)
                .Distinct()
                .ToArray()
            );
            ArtistFilter.Items.AddRange(
                (from cd in XMLCatalog.Element("CATALOG").Elements("CD")
                 select cd.Element("ARTIST").Value)
                .Distinct()
                .ToArray()
            );
            CompanyFilter.Items.AddRange(
                (from cd in XMLCatalog.Element("CATALOG").Elements("CD")
                 select cd.Element("COMPANY").Value)
                .Distinct()
                .ToArray()
            );
            CountryFilter.Items.AddRange(
                (from cd in XMLCatalog.Element("CATALOG").Elements("CD")
                 select cd.Element("COUNTRY").Value)
                .Distinct()
                .ToArray()
            );

        }

        void InitializeFilters()
        {
            TitleFilter = new ComboBox
            {
                Location = new Point(90 + ContentContainer.Width, 30),
                Size = new Size(ContentContainer.Width - 30, 100),
                Font = new Font("Verdana", 16),
                Text = "Title",
                Name = "Title",
            };

            ArtistFilter = new ComboBox
            {
                Location = new Point(90 + ContentContainer.Width,
                TitleFilter.Bounds.Bottom + 30),
                Size = new Size(ContentContainer.Width - 30, 100),
                Font = new Font("Verdana", 16),
                Text = "Artist",
                Name = "Artist",
            };

            CountryFilter = new ComboBox
            {
                Location = new Point(90 + ContentContainer.Width,
                ArtistFilter.Bounds.Bottom + 30),
                Size = new Size(ContentContainer.Width - 30, 100),
                Font = new Font("Verdana", 16),
                Text = "Country",
                Name = "Country",
            };

            CompanyFilter = new ComboBox
            {
                Location = new Point(90 + ContentContainer.Width,
                CountryFilter.Bounds.Bottom + 30),
                Size = new Size(ContentContainer.Width - 30, 100),
                Font = new Font("Verdana", 16),
                Text = "Company",
                Name = "Company",
            };

            PriceFilterFrom = new TextBox
            {
                Location = new Point(90 + ContentContainer.Width,
                CompanyFilter.Bounds.Bottom + 30),
                Size = new Size((ContentContainer.Width - 70) / 2, 100),
                Font = new Font("Verdana", 14),
                Text = "Price: from",
                Name = "PriceFrom",
            };

            PriceFilterTo = new TextBox
            {
                Location = new Point((190 + 3 * ContentContainer.Width) / 2,
                CompanyFilter.Bounds.Bottom + 30),
                Size = new Size((ContentContainer.Width - 70) / 2, 100),
                Font = new Font("Verdana", 14),
                Text = "Price: to",
                Name  = "PriceTo",
            };

            YearFilterFrom = new TextBox
            {
                Location = new Point(90 + ContentContainer.Width,
                PriceFilterTo.Bounds.Bottom + 20),
                Size = new Size((ContentContainer.Width - 70) / 2, 100),
                Font = new Font("Verdana", 14),
                Text = "Year: from",
                Name = "YearFrom",
            };

            YearFilterTo = new TextBox
            {
                Location = new Point((190 + 3 * ContentContainer.Width) / 2,
                PriceFilterTo.Bounds.Bottom + 20),
                Size = new Size((ContentContainer.Width - 70) / 2, 100),
                Font = new Font("Verdana", 14),
                Text = "Year: to",
                Name = "YearTo",
            };
            
            TitleFilter.TextChanged += Filter_TextChanged;
            CompanyFilter.TextChanged += Filter_TextChanged;
            CountryFilter.TextChanged += Filter_TextChanged;
            ArtistFilter.TextChanged += Filter_TextChanged;
            YearFilterTo.TextChanged += Filter_TextChanged;
            YearFilterFrom.TextChanged += Filter_TextChanged;
            PriceFilterFrom.TextChanged += Filter_TextChanged;
            PriceFilterTo.TextChanged += Filter_TextChanged;

            TitleFilter.Enter += Filter_Enter;
            CompanyFilter.Enter += Filter_Enter;
            CountryFilter.Enter += Filter_Enter;
            ArtistFilter.Enter += Filter_Enter;
            YearFilterTo.Enter += Filter_Enter;
            YearFilterFrom.Enter += Filter_Enter;
            PriceFilterFrom.Enter += Filter_Enter;
            PriceFilterTo.Enter += Filter_Enter;
        }

        private void Filter_Enter(object sender, EventArgs e)
        {
            if (sender is TextBox) (sender as TextBox).Text = "";
            else if (sender is ComboBox) (sender as ComboBox).Text = "";
        }

        private void Filter_TextChanged(object sender, EventArgs e)
        {
            string newValue = (sender as TextBox == null)
                ? (sender as ComboBox).Text 
                : (sender as TextBox).Text;
            string propName = (sender as TextBox == null)
                ? (sender as ComboBox).Name
                : (sender as TextBox).Name;
            FiltersData.GetType().GetProperty(propName).SetValue(FiltersData, newValue);

            ContentContainer.Text = Disks.FilteredToDisplay(FiltersData);
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

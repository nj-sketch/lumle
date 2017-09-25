using System.Collections.Generic;

namespace Lumle.Module.CMS.ViewModels
{
    public class DashboardChartVM
    {
        public class Chart
        {
            public string type { get; set; }
        }

        public class Title
        {
            public string text { get; set; }
        }

        public class Subtitle
        {
            public string text { get; set; }
        }

        public class XAxis
        {
            public List<string> categories { get; set; }
            public bool crosshair { get; set; }
        }

        public class Title2
        {
            public string text { get; set; }
        }

        public class YAxis
        {
            public int min { get; set; }
            public Title2 title { get; set; }
        }

        public class Tooltip
        {
            public string headerFormat { get; set; }
            public string pointFormat { get; set; }
            public string footerFormat { get; set; }
            public bool shared { get; set; }
            public bool useHTML { get; set; }
        }

        public class Column
        {
            public double pointPadding { get; set; }
            public int borderWidth { get; set; }
        }

        public class PlotOptions
        {
            public Column column { get; set; }
        }

        public class Series
        {
            public string name { get; set; }
            public List<double> data { get; set; }
        }

        public class RootObject
        {
            public Chart chart { get; set; }
            public Title title { get; set; }
            public Subtitle subtitle { get; set; }
            public XAxis xAxis { get; set; }
            public YAxis yAxis { get; set; }
            public Tooltip tooltip { get; set; }
            public PlotOptions plotOptions { get; set; }
            public List<Series> series { get; set; }
        }
    }
}

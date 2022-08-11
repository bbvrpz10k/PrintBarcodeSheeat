using CsvHelper.Configuration.Attributes;

namespace PrintBarcodeSheeat
{
    public class DataBarCode
        {
        [Ignore]
        public Settings Settings { get; set; }
        [Index(0)]
        public string codeText { get; set; }
        [Index(1)]
        public string DescriptionText { get; set; }
    }
}

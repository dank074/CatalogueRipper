using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogueRipper
{
    public class Page
    {
        public int Id { get; set; }
        public int Icon { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public string Caption { get; set; }
        public List<Page> SubPages { get; set; } = new List<Page>();
        public string Layout { get; internal set; }
        public List<string> Images { get; internal set; } = new List<string>();
        public List<string> Texts { get; internal set; } = new List<string>();
        public List<CatalogItem> Items { get; internal set; } = new List<CatalogItem>();
        public bool Loaded { get; internal set; }
    }
}

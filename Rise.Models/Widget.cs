using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Models
{
    public class Widget : DbObject, IEquatable<Widget>
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string PageTag { get; set; }

        public bool Equals(Widget other)
        {
            return Title.Equals(other.Title) &&
                   PageTag.Equals(other.PageTag);
        }
    }
}
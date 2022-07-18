using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaApp2.Model
{
    public class PizzaCell
    {
        public Pizza pizza { get; set; }
        public bool isFavorite { get; set; }
        public string ImageSourceFav { get { return isFavorite ? "star2.png" : "star1.png"; } }
        
    }
}

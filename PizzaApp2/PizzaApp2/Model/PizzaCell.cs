using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace PizzaApp2.Model
{
    public class PizzaCell : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Pizza pizza { get; set; }
        public bool isFavorite { get; set; }
        public string ImageSourceFav { get { return isFavorite ? "star2.png" : "star1.png"; } }
        public ICommand FavClickCommand { get; set; }

        public Action<PizzaCell> favChangedAction { get; set; }

        public PizzaCell()
        {
            FavClickCommand = new Command((ogj) =>
            {
                Console.WriteLine("FavClickCommand");

                isFavorite = !isFavorite;

                OnPropertyChanged("ImageSourceFav");

                favChangedAction.Invoke(this);
            });
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    
}

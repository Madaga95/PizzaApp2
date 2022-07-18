using Newtonsoft.Json;
using PizzaApp2.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static System.Environment;

namespace PizzaApp2
{
    public partial class MainPage : ContentPage
    {
        List<Pizza> pizzas;
        List<string> pizzasFav = new List<string>();
        enum e_tri
        {
            TRI_AUCUN,
            TRI_PRIX,
            TRI_NOM
        }
        e_tri tri = e_tri.TRI_AUCUN;

        const string KEY_TRI = "tri";

        // on créer notre fichier pour notre appareil

       
        string jsonFileName = Path.Combine(Environment.GetFolderPath(SpecialFolder.LocalApplicationData), "pizzas.json");


        public MainPage()
        {
            InitializeComponent();

            pizzasFav.Add("4 fromages");
            pizzasFav.Add("indienne");
            pizzasFav.Add("tartiflette");

            

            // ici on fait de la persistance
            if (Application.Current.Properties.ContainsKey(KEY_TRI))
            {
                tri = (e_tri)Application.Current.Properties[KEY_TRI];
                sortButton.Source = GetImageSourceFromTri(tri);
            }


            // ici on rafraichis notre liste 
            listView.RefreshCommand = new Command((obj) =>
            {
                DownloadData((pizzas) =>
                {
                    if (pizzas != null)
                    {
                        listView.ItemsSource = GetPizzaCells(GetPizzasFromTri(tri, pizzas), pizzasFav);
                    }
                    listView.IsRefreshing = false;
                });
            });

            listView.IsVisible = false;
            waitLayout.IsVisible = true;

            if (File.Exists(jsonFileName))
            {
                string pizzasJson = File.ReadAllText(jsonFileName);
                if (!String.IsNullOrEmpty(pizzasJson))
                {
                    pizzas = JsonConvert.DeserializeObject<List<Pizza>>(pizzasJson);
                    listView.ItemsSource = GetPizzaCells(GetPizzasFromTri(tri, pizzas), pizzasFav);
                    listView.IsVisible = true;
                    waitLayout.IsVisible = false;
                }  
            }





            DownloadData((pizzas) =>
            {
                
                if(pizzas != null)
                {
                    listView.ItemsSource = GetPizzaCells(GetPizzasFromTri(tri, pizzas), pizzasFav);
                }
                
                listView.IsVisible = true;
                waitLayout.IsVisible = false;
            });
        }

        // on télécharge nos pizza à partir du serveur puis on les désérialises
        public void DownloadData(Action<List<Pizza>> action)
        {
            const string URL = "https://drive.google.com/uc?export=download&id=1ox5aHVj9OCizWUFDf8-WNsHTyVJwHdHC";

            using (var webclient = new WebClient())
            {
                webclient.DownloadFileCompleted += (object sender, AsyncCompletedEventArgs e) =>
                {
                    Exception ex = e.Error;

                    if (ex == null)
                    {
                       

                        string pizzasJson = File.ReadAllText(jsonFileName);

                        pizzas = JsonConvert.DeserializeObject<List<Pizza>>(pizzasJson);


                        Device.BeginInvokeOnMainThread(() =>
                        {
                            action.Invoke(pizzas);
                        });
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await DisplayAlert("Erreur", "Une erreur réseau s'est produite: " + ex.Message, "OK");
                            action.Invoke(null);
                        });
                    }
                    /*
                    try
                    {
                        string pizzaJson = e.Result;

                        pizzas = JsonConvert.DeserializeObject<List<Pizza>>(pizzaJson);


                        Device.BeginInvokeOnMainThread(() =>
                        {
                            action.Invoke(pizzas);
                        });
                        // webclient.DownloadStringAsync(new Uri(URL));

                        webclient.DownloadFileAsync(new Uri(URL), jsonFileName);
                    }
                    catch (Exception ex)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert("Erreur", "Une erreur réseau s'est produite: " + ex.Message, "OK");
                        });
                    }
                    */
                };
                webclient.DownloadFileAsync(new Uri(URL), jsonFileName);



            }
        }

        // fonction qui nous permet de faire un tri à chaque fois qu'on vas cliquer
        void SortButton_Clicked(object sender, EventArgs e)
        {
            if (tri == e_tri.TRI_AUCUN)
            {
                tri = e_tri.TRI_NOM;
            }
            else if (tri == e_tri.TRI_NOM)
            {
                tri = e_tri.TRI_PRIX;
            }
            else if (tri == e_tri.TRI_PRIX)
            {
                tri = e_tri.TRI_AUCUN;
            }
            sortButton.Source = GetImageSourceFromTri(tri);
            listView.ItemsSource = GetPizzaCells(GetPizzasFromTri(tri, pizzas), pizzasFav);

            // on sauvegarde nos données
            Application.Current.Properties[KEY_TRI] = (int)tri;
            Application.Current.SavePropertiesAsync();
        }


        // on fait en sorte d'afficher la bonne image pour le tri
        private string GetImageSourceFromTri(e_tri t)
        {
            switch (t)
            {
                case e_tri.TRI_NOM:
                    return "sort_nom.png";
                case e_tri.TRI_PRIX:
                    return "sort_prix.png";
            }
            return "sort_none.png";
        }

        // on récupére la liste des pizzas puis on les compares

        private List<Pizza> GetPizzasFromTri(e_tri t, List<Pizza> l)
        {
            if (l == null)
            {
                return null;
            }

            switch (t)
            {
                case e_tri.TRI_NOM:
                    {
                        List<Pizza> ret = new List<Pizza>(l);

                        ret.Sort((p1, p2) =>
                        {
                            return p1.nom.CompareTo(p2.nom);
                        });


                        return ret;
                    }

                case e_tri.TRI_PRIX:
                    {
                        List<Pizza> ret = new List<Pizza>(l);

                        ret.Sort((p1, p2) =>
                        {
                            return p2.prix.CompareTo(p1.prix);
                        });

                        return ret;

                    }
            }
            return l;
        }

        // on créer une fonction pour une liste de pizza favori
        private List<PizzaCell> GetPizzaCells(List<Pizza> p, List<string> f)
        {
            List<PizzaCell> ret = new List<PizzaCell>();

            if(p == null)
            {
                return ret;
            }

            foreach(Pizza pizza in p)
            {
                bool isFav = f.Contains(pizza.nom);
                ret.Add(new PizzaCell { pizza = pizza, isFavorite = isFav});
            }

            return ret;
        }
    }
}
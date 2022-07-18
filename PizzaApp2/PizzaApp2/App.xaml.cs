using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PizzaApp2
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var navigationPage = new NavigationPage(new MainPage());
            navigationPage.BackgroundColor = Color.FromHex("#1abbd4");
            MainPage = navigationPage;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

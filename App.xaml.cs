namespace PM2EXAMEN7669
{
    public partial class App : Application
    {
        public static Controllers.PlacesDB PlacesDBController { get; private set; }
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            PlacesDBController = new Controllers.PlacesDB();
        }
    }
}

namespace PM2EXAMEN7669
{
    public partial class App : Application
    {
        public static Controllers.DBSitioMaps DBSitioMapsController { get; private set; }
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            DBSitioMapsController = new Controllers.DBSitioMaps();
        }
    }
}

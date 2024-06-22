using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Threading.Tasks;

namespace PM2EXAMEN7669.Views
{
    public partial class ShowMap : ContentPage
    {
        private Models.PlaceMaps SelectedSitio { get; }
        private Location userLocation;

        public ShowMap(Models.PlaceMaps selectedSitio)
        {
            InitializeComponent();
            SelectedSitio = selectedSitio;
            this.Appearing += ShowMap_Appearing;
            labelSitio.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OpenGoogleMapsForDirections(SelectedSitio.latitud, SelectedSitio.longitud))
            });
        }

        private async void ShowMap_Appearing(object sender, EventArgs e)
        {
            try
            {
                // Obtiene la ubicaci�n actual
                userLocation = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Default,
                    Timeout = TimeSpan.FromSeconds(10)
                });

                if (userLocation != null)
                {
                    mapa.IsShowingUser = true; // Aseg�rate de que IsShowingUser est� habilitado
                    await colocarSitio(SelectedSitio.latitud, SelectedSitio.longitud);
                    MoveMapToRegion(SelectedSitio.latitud, SelectedSitio.longitud);
                }
                else
                {
                    await DisplayAlert("Alerta", "No se pudo obtener la ubicaci�n actual.", "Ok");
                }
            }
            catch (FeatureNotEnabledException)
            {
                await DisplayAlert("Alerta", "El GPS se encuentra desactivado. Por favor active su GPS y abra la aplicaci�n de nuevo.", "Ok");
                Application.Current.Quit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in VerMapa_Appearing: {ex.Message}");
            }
            finally
            {
                this.Appearing -= ShowMap_Appearing; // Asegura que no se llame m�ltiples veces
            }
        }

        public async Task colocarSitio(double latitude, double longitude)
        {
            string? adminDistrict = null;
            string? adminDistrict2 = null;
            string? countryRegion = null;

            string jsonResponse = await GetGeocodeDataAsync(latitude, longitude);

            if (jsonResponse != null)
            {
                try
                {
                    // Parse the JSON response using JsonDocument
                    JsonDocument jsonDocument = JsonDocument.Parse(jsonResponse);

                    // Obtiene el campo adminDistrict del JSON
                    adminDistrict = jsonDocument.RootElement
                        .GetProperty("resourceSets")[0]
                        .GetProperty("resources")[0]
                        .GetProperty("address")
                        .GetProperty("adminDistrict")
                        .GetString();

                    // Obtiene el campo adminDistrict2 del JSON
                    adminDistrict2 = jsonDocument.RootElement
                        .GetProperty("resourceSets")[0]
                        .GetProperty("resources")[0]
                        .GetProperty("address")
                        .GetProperty("adminDistrict2")
                        .GetString();

                    // Obtiene el campo countryRegion del JSON
                    countryRegion = jsonDocument.RootElement
                        .GetProperty("resourceSets")[0]
                        .GetProperty("resources")[0]
                        .GetProperty("address")
                        .GetProperty("countryRegion")
                        .GetString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing JSON: {ex.Message}");
                }
            }

            // Agrega el Marcador en la ubicaci�n
            var marker = new Pin
            {
                Type = PinType.Place,
                Location = new Location(latitude, longitude),
                Label = SelectedSitio.descripcion,
                Address = $"{adminDistrict2}, {adminDistrict}, {countryRegion}"
            };

            labelSitio.Text = $"{adminDistrict2}, {adminDistrict}, {countryRegion}";
            mapa.Pins.Add(marker);
        }

        private async Task<string?> GetGeocodeDataAsync(double latitude, double longitude)
        {
            string BingMapsApiKey = "AmqOyRiuHf-iIWHzpYSqeUp8vo-37HMW7p8v_ctVnwcRkQX0mEVc_I7pmEJ14mAy";
            try
            {
                string apiUrl = $"https://dev.virtualearth.net/REST/v1/Locations/{latitude},{longitude}?o=json&key={BingMapsApiKey}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        // Lee la respuesta como String
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        return jsonResponse;
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }

        public void MoveMapToRegion(double latitude, double longitude)
        {
            // Se mueve a la ubicaci�n del pin
            Location location = new Location(latitude, longitude);
            MapSpan mapSpan = new MapSpan(location, 0.01, 0.01);
            mapa.MoveToRegion(mapSpan);
        }

        private void btnShare_Clicked(object sender, EventArgs e)
        {
            string locationUrl = $"https://maps.google.com/?q={SelectedSitio.latitud},{SelectedSitio.longitud}";
            string message = $"�Ubicacion de los lugares visitados!: {SelectedSitio.descripcion}";

            Share.RequestAsync(new ShareTextRequest
            {
                Text = $"{message}\n{locationUrl}",
                Title = "Comparte ubicacion"
            });
        }

        private void btnDirections_Clicked(object sender, EventArgs e)
        {
            OpenGoogleMapsForDirections(SelectedSitio.latitud, SelectedSitio.longitud);
        }

        private void OpenGoogleMapsForDirections(double latitude, double longitude)
        {
            if (userLocation != null)
            {
                string userLatitude = userLocation.Latitude.ToString().Replace(',', '.');
                string userLongitude = userLocation.Longitude.ToString().Replace(',', '.');
                string destinationLatitude = latitude.ToString().Replace(',', '.');
                string destinationLongitude = longitude.ToString().Replace(',', '.');

                string googleMapsUrl = $"https://www.google.com/maps/dir/?api=1&origin={userLatitude},{userLongitude}&destination={destinationLatitude},{destinationLongitude}";

                Launcher.OpenAsync(new Uri(googleMapsUrl));
            }
            else
            {
                DisplayAlert("Alerta", "No se pudo obtener la ubicaci�n actual.", "Ok");
            }
        }
    }
}
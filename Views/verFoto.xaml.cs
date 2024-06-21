using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace PM2EXAMEN7669.Views
{
    public partial class verFoto : ContentPage
    {
        private Models.SitioMaps SelectedSitio { get; }

        public verFoto(Models.SitioMaps selectedSitio)
        {
            InitializeComponent();
            SelectedSitio = selectedSitio;
            BindingContext = selectedSitio;
        }

        private void btnSalir_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void btnSharePhoto_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (imgSitio.Source is FileImageSource fileImageSource)
                {
                    string localPath = fileImageSource.File;

                    await Xamarin.Essentials.Share.RequestAsync(new Xamarin.Essentials.ShareFileRequest
                    {
                        Title = "Compartir Imagen",
                        File = new Xamarin.Essentials.ShareFile(localPath)
                    });
                }
                else if (imgSitio.Source is StreamImageSource streamImageSource)
                {
                    var filePath = Path.Combine(Xamarin.Essentials.FileSystem.CacheDirectory, "shared_image.png");

                    using (var originalStream = await streamImageSource.Stream(new System.Threading.CancellationToken()))
                    {
                        if (originalStream != null)
                        {
                            // Rewind the stream to the beginning
                            originalStream.Seek(0, SeekOrigin.Begin);

                            using (var memoryStream = new MemoryStream())
                            {
                                await originalStream.CopyToAsync(memoryStream);
                                memoryStream.Seek(0, SeekOrigin.Begin);

                                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                                {
                                    await memoryStream.CopyToAsync(fileStream);
                                }
                            }

                            await Xamarin.Essentials.Share.RequestAsync(new Xamarin.Essentials.ShareFileRequest
                            {
                                Title = "Compartir Imagen",
                                File = new Xamarin.Essentials.ShareFile(filePath)
                            });
                        }
                        else
                        {
                            await DisplayAlert("Error", "No se puede compartir la imagen. Fuente de imagen no válida.", "OK");
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Error", "No se puede compartir la imagen. Fuente de imagen no válida.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error al intentar compartir la imagen: {ex.Message}", "OK");
            }
        }
    }
}
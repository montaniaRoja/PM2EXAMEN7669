using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PM2EXAMEN7669.Views
{
    public partial class listaSitios : ContentPage
    {
        private Controllers.DBSitioMaps controller;
        private List<Models.SitioMaps> sitios;
        private Models.SitioMaps SelectedSitio { get; set; }
        private Grid selectedGrid = null;

        public listaSitios()
        {
            NavigationPage.SetHasBackButton(this, false);
            InitializeComponent();
            controller = new Controllers.DBSitioMaps();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            sitios = await controller.getListSitio();
            listUbicacion.ItemsSource = sitios;
        }

        private void btnSalir_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void btnEliminar_Clicked(object sender, EventArgs e)
        {
            if (SelectedSitio != null)
            {
                var confirmed = await DisplayAlert("Borrar Sitio Visitado", $"Esta seguro que desea borrar el sitio visitado: {SelectedSitio.descripcion}?", "Si", "No");

                if (confirmed)
                {
                    var result = await controller.deleteSitio(SelectedSitio.Id);

                    if (result > 0)
                    {
                        await DisplayAlert("Sitio Borrado", $"{SelectedSitio.descripcion} ha sido borrado exitosamente!", "OK");
                        sitios = await controller.getListSitio();
                        listUbicacion.ItemsSource = sitios;
                        SelectedSitio = null;
                    }
                    else
                    {
                        await DisplayAlert("Error", "No se pudo borrar el sitio", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Error", "Seleccione un sitio antes de eliminar", "OK");
            }
        }

        private async void btnActualizar_Clicked(object sender, EventArgs e)
        {
            if (SelectedSitio != null)
            {
                await Navigation.PushAsync(new Views.editSitio(SelectedSitio));
            }
        }

        private async void btnVerMaps_Clicked(object sender, EventArgs e)
        {
            if (SelectedSitio != null)
            {
                await Navigation.PushAsync(new Views.verMapa(SelectedSitio));
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            if (SelectedSitio != null)
            {
                VisualStateManager.GoToState(selectedGrid, "Normal");
            }

            selectedGrid = sender as Grid;
            SelectedSitio = selectedGrid?.BindingContext as Models.SitioMaps;

            if (SelectedSitio != null)
            {
                VisualStateManager.GoToState(selectedGrid, "Selected");
            }
        }

        private async void TapGestureFoto_Tapped(object sender, TappedEventArgs e)
        {
            if (SelectedSitio != null)
            {
                await Navigation.PushAsync(new Views.verFoto(SelectedSitio));
                SelectedSitio = null;
            }
        }
    }
}
using PM2EXAMEN7669.Extensions;
namespace PM2EXAMEN7669.Views;

public partial class EditPlace : ContentPage
{
    private Models.PlaceMaps SelectedSitio { get; }
    private Models.PlaceMaps editedSitio;

    public EditPlace(Models.PlaceMaps selectedSitio)
    {
        InitializeComponent();
        SelectedSitio = selectedSitio;
        BindingContext = editedSitio = selectedSitio;
    }

    private async void btnActualizar_Clicked(object sender, EventArgs e)
    {
        // Update the 'descripcion' field in the editedSitio object
        editedSitio.descripcion = entryDescripcion.Text;

        // Call the update method with the editedSitio object
        await App.PlacesDBController.updateSitios(editedSitio);

        // Display an alert or navigate back
        await DisplayAlert("Sitio Actualizado!", "La información del sitio ha sido actualizada!", "OK");
        await Navigation.PopAsync();
    }

    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}
using System;
using Xamarin.Forms;

namespace MVVM
{
	public partial class FirstPage : ContentPage
	{
		FirstPageViewModel vm;
		public FirstPage()
		{
			InitializeComponent();
			Init();
		}

		void Init() {
			 vm = new FirstPageViewModel();
			//Set bindings
			lblAddress.BindingContext = vm;
			lblAddress.SetBinding(Label.TextProperty, "Address", stringFormat:"Current address: {0}");

			lblWeather.BindingContext = vm;
			lblWeather.SetBinding(Label.TextProperty, "WeatherDescr", stringFormat: "Current weather: {0}");

			ai.BindingContext = vm;
			ai.SetBinding(ActivityIndicator.IsRunningProperty, "IsOperationInProgress");

			btnExecute.BindingContext = vm;
			btnExecute.SetBinding(Button.IsEnabledProperty, "UIEnabled");

			btnReset.BindingContext = vm;
			btnReset.SetBinding(Button.IsEnabledProperty, "UIEnabled");
		}


		async void OnLocationButtonClicked(object sender, EventArgs args)
		{
			vm.IsOperationInProgress = true;
			var result = await vm.UpdatePosition();
			if ((result!=null)&&(result.IsValid))
			{
				//retrieve weather
				try
				{
var weather = await vm.UpdateWeather(result.Lat, result.Lon);
				if (weather == null)
					await DisplayAlert("Error", "An error occurred while retrieving weather information", "Ok");
				}
				catch (Exception ex)
				{
					await DisplayAlert("Exception", string.Format("{0} --- {1}",ex.Message,ex.InnerException.Message), "Ok");
				}

			}
			vm.IsOperationInProgress = false;
		}

		async void OnResetButtonClicked(object sender, EventArgs args)
		{
			vm.WeatherDescr = string.Empty;
			vm.Address = string.Empty;
		}
	}
}

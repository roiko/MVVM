using System;
using System.ComponentModel;
using Xamarin.Forms;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace MVVM
{
	public class FirstPageViewModel : INotifyPropertyChanged
	{
		string _className = "FirstPageViewModel";
		public event PropertyChangedEventHandler PropertyChanged;

		CustomPosition _position = new CustomPosition();

		private string _address = "";
		public string Address { 
			get { return _address;}
			set {
				if (_address == value)
					return;
				_address = value;
				OnPropertyChanged("Address");
			}
		}

		private string _weatherdescr = "";
		public string WeatherDescr { 
			get { return _weatherdescr;}
			set {
				if (_weatherdescr.Equals(value))
					return;
				_weatherdescr = value;
				OnPropertyChanged("WeatherDescr");
			}
		}


		private int _count = 0;
		public int Count { 
			get { return _count;}
			set {
				if (_count == value)
					return;
				_count = value;
				OnPropertyChanged("Count");
			}
		}

		public bool UIEnabled {
			get { return !IsOperationInProgress;}
		}

		private bool _isOperationInProgress = false;
		public bool IsOperationInProgress { 
			get { return _isOperationInProgress;}
			set {
				_isOperationInProgress = value;
				OnPropertyChanged("IsOperationInProgress");
                OnPropertyChanged("UIEnabled");
			}
		}


		public FirstPageViewModel()
		{
		}

		public async Task<CustomPosition> UpdatePosition()
		{
			try
			{
				//IsOperationInProgress = true;
				if (!Plugin.Geolocator.CrossGeolocator.Current.IsGeolocationAvailable)
				{
					await Application.Current.MainPage.DisplayAlert("Geolocation is unavailable!", "It seems that geolocation is not available on this device!", "Close");
					//IsOperationInProgress = false;
					return null;
				}
				if (!Plugin.Geolocator.CrossGeolocator.Current.IsGeolocationEnabled)
				{
					await Application.Current.MainPage.DisplayAlert("Geolocation is unavailable!", "You must enable phone geolocation first!", "Close");
					//IsOperationInProgress = false;
					return null;
				}
				Position pos = await CrossGeolocator.Current.GetPositionAsync(timeout: new TimeSpan(0, 0, 25));
				if (PositionIsValid(pos))
				{
					var addrs = await CrossGeolocator.Current.GetAddressesForPositionAsync(pos);
					if (addrs.Count() > 0)
					{
						_position.Alt = pos.Altitude;
						_position.Lon = pos.Longitude;
						_position.Lat = pos.Latitude;
						_position.Street = addrs.First().Thoroughfare;
						_position.Number = addrs.First().SubThoroughfare;
						_position.City = addrs.First().Locality;
						_position.Country = addrs.First().CountryName;
						_position.IsValid = true;
						Address = String.Format("{0} {1}, {2} {3}", _position.Street, _position.Number, _position.City, _position.Country);
					}
				}
				else
				{
					//cant 'retrieve position
					_position.IsValid = false;
				}
				return _position;
			}
			catch (Exception ex)
			{
				var dudu = ex;
				return null;
			}
			finally
			{
				//IsOperationInProgress = false;
			}
		}

		private bool PositionIsValid(Position p_pos)
		{
			if (p_pos.Timestamp.Year==DateTime.Now.Year)
				return true;
			return false;
		}


		public async Task<RESTClient.WeatherResult> UpdateWeather(double lat, double lon)
		{
			RESTClient.WeatherResult weatherResult = null;
			try
			{
				RESTClient.Client client = new RESTClient.Client();
				weatherResult = await client.GetCurrentWeather(lat, lon);
				WeatherDescr = string.Format("{0} ({1})", weatherResult.Main, weatherResult.Description);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Exception occurred in UpdateWeather method!");
				throw (ex);
			}
			return weatherResult;
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this,
	   			new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}

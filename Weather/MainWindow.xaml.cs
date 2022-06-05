using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Net;
using System.ComponentModel;


namespace Weather
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DefaultTime();
        }

        string APIKey = "5bec5f1a1460771fdab0079a0261ee62";

        
        public void DefaultTime()
        {
            DateTimeBox.Text = DateTime.Now.ToString("dddd , hh:mm:ss");
        }

        public void buttonSearch_Click(object sender, EventArgs e)
        {
            getWeather();
        }

        void getWeather()
        {
            using (WebClient web = new WebClient())
            {
                string url = string.Format("https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}", SearchCity.Text, APIKey);
                var json = web.DownloadString(url);
                WeatherForecast.Root Forecast = JsonConvert.DeserializeObject<WeatherForecast.Root>(json);

                double temperatura = Math.Round((Forecast.main.temp) - 273);

                BoxSunset.Text = convertDateTime(Forecast.sys.sunset).ToShortTimeString();
                BoxSunrise.Text = convertDateTime(Forecast.sys.sunrise).ToShortTimeString();
                BoxWind.Text = Forecast.wind.speed.ToString();
                DateTimeBox.Text = DateTime.Now.ToString("dddd, hh:mm");
                BoxTemperature.Text = temperatura.ToString() + "°";
                BoxWeather.Text = Forecast.weather[0].description;
                BoxHumidity.Text = "Wilgotność - " + Forecast.main.humidity.ToString() + "%";

                Image= new BitmapImage(new Uri( "https://api.openweathermap.org/img/w/" + Forecast.weather[0].icon + ".png"));


            }
        }

        DateTime convertDateTime(long millisec)
        {
            DateTime day = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).ToLocalTime();
            day = day.AddSeconds(millisec).ToLocalTime();

            return day;
        }

        

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

    }
}

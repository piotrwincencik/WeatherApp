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
using System.Xml.Linq;
using Microsoft.Win32;
using System.IO;
using static Weather.WeekControl;

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
            getWeekWeather();
        }

        public double lon;
        public double lat;

        public void getWeather()
        {
            using (WebClient web = new WebClient())
            {
                string url = string.Format("https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}", SearchCity.Text, APIKey);


                var json = web.DownloadString(url);
                WeatherForecast.Root Forecast = JsonConvert.DeserializeObject<WeatherForecast.Root>(json);

                double temperature = Math.Round((Forecast.main.temp) - 273);
                var sunrise = new DateTime(((Forecast.sys.sunrise + Forecast.timezone) * 1000));

                BoxSunset.Text = (UnixTimestampToDateTime(Forecast.sys.sunset)).ToString("HH:mm");
                BoxSunrise.Text = (UnixTimestampToDateTime(Forecast.sys.sunrise)).ToString("HH:mm");

                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(1000);

                BoxWind.Text = Forecast.wind.speed.ToString();
                DateTimeBox.Text = DateTime.Now.ToString("dddd, hh:mm");
                BoxTemperature.Text = temperature.ToString() + "C";
                BoxWeather.Text = Forecast.weather[0].description;
                BoxHumidity.Text = "humidity - " + Forecast.main.humidity.ToString() + "%";

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("https://api.openweathermap.org/img/w/" + Forecast.weather[0].icon + ".png");
                bitmap.EndInit();
                IconPicture.Source = bitmap;
                IconPicture2.Source = bitmap;

                lon = Forecast.coord.lon;
                lat = Forecast.coord.lat;
            }
        }

        void getWeekWeather()
        {
            using (WebClient web = new WebClient())
            {
                string url = string.Format("https://api.openweathermap.org/data/2.5/onecall?lat={0}&lon={1}&exclude=minutely,hourly,current,alerts&appid={2}", lat, lon, APIKey);
                var json = web.DownloadString(url);
                WeekWeather.WeekWeatherInfo WeekWeatherInfo = JsonConvert.DeserializeObject<WeekWeather.WeekWeatherInfo>(json);

                

                WeekControl WC;
                for(int i = 0; i < 5; i++)
                {
                    WC = new WeekControl();
                    FLP.Children.Add(WC);

                    WC.BoxDayWeek.Text = (UnixTimestampToDateTime(WeekWeatherInfo.daily[i].dt)).ToString("ddd");
                    WC.TemperatureWeek.Text = Math.Round(WeekWeatherInfo.daily[i].temp.day -273).ToString() + "C";


                    BitmapImage bitmap2 = new BitmapImage();
                    bitmap2.BeginInit();
                    bitmap2.UriSource = new Uri("https://api.openweathermap.org/img/w/" + WeekWeatherInfo.daily[i].weather[0].icon + ".png");
                    bitmap2.EndInit();

                    WC.BoxPictureWeek.Source = bitmap2;
                }



            }
        }
        
        public static DateTime UnixTimestampToDateTime(long unixTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
            long unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
            return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Local).AddHours(-5);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        public void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            WebClient web = new WebClient();
            string url = string.Format("https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}", SearchCity.Text, APIKey);
            string json = web.DownloadString(url);
            WeatherForecast.Root Forecast = JsonConvert.DeserializeObject<WeatherForecast.Root>(json);

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Dokument w formacie json (*.json)|*.json";
            dialog.Title = "Zapisz pogodę do pliku JSON";
            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, (json).ToString());
            }
        }

    }
}

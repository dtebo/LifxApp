using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Data.Json;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LifxApp
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //Request token
        private static string token;

        public LightCollection Lights { get; set; }

        public enum LightStates
        {
            On,
            Off,
            Unknown
        }

        LightData ldObj = new LightData();
        Color lightColor;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loading += MainPage_Loading;
        }

        private async void MainPage_Loading(FrameworkElement sender, object args)
        {
            //Load the application settings
            Settings s = await SettingsService.LoadSettings();

            if (s == null)
            {
                s = new LifxApp.Settings { ApiToken = "cbc52bed809139dc195cb4cc37390e9cda1785bdd87eb0df191dcfbe5f46a92f" };
                await SettingsService.SaveSettings(s);
            }

            //Grab the ApiToken
            token = s.ApiToken;

            //Get the current color slider values.
            //TODO: Update to initialize the color sliders to match the current light color.
            lightColor = new Color { R = (byte)R.Value, G = (byte)G.Value, B = (byte)B.Value };

            //Debug Code - Uncomment to test list binding

            string result = await GetLightInfo();
            LightCollection lc = new LightCollection();
            lc = lc.FromJSON(result);
            Lights = lc;
            myList.ItemsSource = Lights;
            
            /*var str = "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" >" +
                          "<Grid>" +
                              "<StackPanel>" +
                                  "<TextBlock Text=\"{Binding label}\"/>";

            for (int i = 0; i < Lights.Lights.Count; i++)
            {
                str +=            "<Slider x:Name=\"R\"" + i + "Value=\"{Binding color.R}\"/>" +
                                  "<Slider x:Name=\"G\"" + i + "Value=\"{Binding color.G}\"/>" +
                                  "<Slider x:Name=\"B\"" + i + "Value=\"{Binding color.B}\"/>";
            }

            str +=            "</StackPanel>" +
                          "</Grid>" +
                      "</DataTemplate>";

            DataTemplate template = (DataTemplate)Windows.UI.Xaml.Markup.XamlReader.Load(str);
            myList.ItemTemplate = template;*/

            //End Debug Code
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            //ldObj.color = lightColor.ToRGBString();
            ldObj.power = LightStates.On.ToString().ToLower();

            //Send request and get the result
            string result = await SetLightState(ldObj);

            JsonObject obj = JsonObject.Parse(result);
        }

        async Task<string> SetLightState(LightData ld)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            Uri uri = new Uri("https://api.lifx.com/v1/lights/all/state");
            string data = ldObj.ToJSON();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, uri);
            request.Content = new HttpStringContent(data, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

            var result = await client.SendRequestAsync(request);
            return result.Content.ToString();
        }

        async Task<string> GetLightInfo()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            Uri uri = new Uri("https://api.lifx.com/v1/lights/all");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);

            var LightInfo = await client.SendRequestAsync(request);
            return LightInfo.Content.ToString();
        }

        private void R_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            lightColor.R = (byte)R.Value;
        }

        private void G_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            lightColor.G = (byte)G.Value;
        }

        private void B_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            lightColor.B = (byte)B.Value;
        }
    }
}

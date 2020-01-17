using System;

using HttpClientBestPractices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using EfficientApiCalls;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace HttpClientImplementation.ViewModels
{
    public class BenchmarkViewModel : BaseViewModel
    {
        public BenchmarkViewModel()
        {
            Title = "About";

            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://xamarin.com"));

            MainAsync(CancellationToken.None).Wait();
        }

        private static Uri Uri = new Uri("http://localhost:5000/api/values");

        private static async Task MainAsync(CancellationToken cancellationToken)
        {
            RequestProvider requestProvider = new RequestProvider();

            const int maxLoop = 100;




            //await BenchmarkHelper.BenchAsync(requestProvider.GetAsync, maxLoop, nameof(requestProvider.GetAsync), Uri, "",  cancellationToken);

            //await BenchmarkHelper.BenchAsync(DeserializeOptimizedFromStreamCallAsync, maxLoop, nameof(Program.DeserializeOptimizedFromStreamCallAsync), cancellationToken);
            //await BenchmarkHelper.BenchAsync(requestProvider.GetAsync<Model>(), maxLoop, nameof(Program.PostBasicAsync), model, cancellationToken);

        }


        public ICommand OpenWebCommand { get; }
    }
}
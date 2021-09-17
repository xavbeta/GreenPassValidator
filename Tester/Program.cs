using DGCValidator.Services;
using GreenPass;
using GreenPass.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


using System;
using System.IO;
using System.Threading.Tasks;

namespace Tester
{
    class Program
    {
        private static ServiceProvider _sp;
        private static CertificateManager _certManager;

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            string gpText = GetGPText(args);

            Initialize();
            var res = await LoadGP(gpText);

            int validity = GetCertificateValidity(res);
            var expiration = res.IssuedDate.AddHours(validity);

            Console.WriteLine($"Documento associato a {res.Dgc.Nam.Fn} {res.Dgc.Nam.Gn}");
            Console.WriteLine($"Il Green Pass è {(expiration > DateTime.Now? "Valido fino al " + expiration : "Scaduto")}");

        }

        private static string GetGPText(string[] args)
        {
            if (args.Length > 0)
                return File.ReadAllText(args[0]);
            else
                return ""; //TODO: Read QR Code From Camera
        }

        private static int GetCertificateValidity(SignedDGC res)
        {
            if (IsVaxinated(res)) return 12 *  30 * 24;
            if (IsRecovered(res)) return 6 * 30 * 24;
            if (IsTested(res)) return 72;

            return 0;
        }

        private static bool IsVaxinated(SignedDGC res) => res.Dgc.V != null;
        private static bool IsRecovered(SignedDGC res) => res.Dgc.R != null;
        private static bool IsTested(SignedDGC res) => res.Dgc.T != null;

        private static async Task<SignedDGC> LoadGP(string greepassPayload)
        {
            return await _sp.GetRequiredService<ValidationService>().Validate(greepassPayload);
        }

        private static void Initialize()
        {
            Console.WriteLine("Configuration loading...");

            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true).Build();
            var sc = new ServiceCollection();
            sc.AddGreenPassValidator(config);
            _sp = sc.BuildServiceProvider();
            _certManager = _sp.GetRequiredService<CertificateManager>();
        }

        private enum CertificateType
        {
            VAXINATED,
            RECOVERED,
            TESTED
        }
    }
}

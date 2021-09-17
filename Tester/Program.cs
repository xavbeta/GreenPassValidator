using DGCValidator.Services;
using GreenPass;
using GreenPass.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


using System;
using System.Threading.Tasks;

namespace Tester
{
    class Program
    {
        private static ServiceProvider _sp;
        private static CertificateManager _certManager;

        static async System.Threading.Tasks.Task Main(string[] args)
        {

            Initialize();
            var res = await LoadGP("HC1:6BFOXN%TS3DHPVO13J /G-/2YRVA.QKW80RBXG4CH23IRM*4$HU:6NKQC:3DCV4*XUA2PSGH.+HIMI4UU5NITK292W7*RBT1ON1XVHWVHE 9HOP+MMBT16Y51Y9AT1 %P6IAXPMMCGCNNQ7TO%0YE9/MVEK0WLIFO5ON1NX7CXBFS92.HR$PJWH.ZJ1Y98S1*T5D-J7*JQO33UBLXJ9*HFP1N I SI5K1*TB3:U-1VVS1UU15%HVLICUHPVFNXUJRHQJA8RUEIAYQE*C2:JG*PEMN9FTIPPAAMI PQVW5/O10+HT+6SZ4RZ4E%5B/9BL5Y$U*.1EY10T9YWP*PMYZQ H9Q$FN*JBX33UQ7541-ST*QGTA4W7.Y7G+SB.V Q5FN9EDKX*H/T12E4I*3156N$BEIV8OP :N%+3VFBZGK$1S7KHH*0R8KTPLL25KACUNLW S50OZ$1EW5BP5T%N9DWI/D 8KBUFF:0G%BDPB:A9C/3BDLK-3Z20V8N91");

            int validity = GetCertificateValidity(res);
            var expiration = res.IssuedDate.AddHours(validity);

            Console.WriteLine($"Documento associato a {res.Dgc.Nam.Fn} {res.Dgc.Nam.Gn}");
            Console.WriteLine($"Il Green Pass è {(expiration > DateTime.Now? "Valido fino al " + expiration : "Scaduto")}");

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

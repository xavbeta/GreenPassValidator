using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace GreenPassTester
{
    class Program
    {
        private readonly ServiceProvider _sp;

        static async void Main(string[] args)

            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true).Build();
            var sc = new ServiceCollection();
            sc.AddGreenPassValidator(config);
            _sp = sc.BuildServiceProvider();
            _certManager = _sp.GetRequiredService<CertificateManager>();
        
            var scanResult = "HC1:6BFOXN%TS3DHPVO13J /G-/2YRVA.QKW80RBXG4CH23IRM*4$HU:6NKQC:3DCV4*XUA2PSGH.+HIMI4UU5NITK292W7*RBT1ON1XVHWVHE 9HOP+MMBT16Y51Y9AT1 %P6IAXPMMCGCNNQ7TO%0YE9/MVEK0WLIFO5ON1NX7CXBFS92.HR$PJWH.ZJ1Y98S1*T5D-J7*JQO33UBLXJ9*HFP1N I SI5K1*TB3:U-1VVS1UU15%HVLICUHPVFNXUJRHQJA8RUEIAYQE*C2:JG*PEMN9FTIPPAAMI PQVW5/O10+HT+6SZ4RZ4E%5B/9BL5Y$U*.1EY10T9YWP*PMYZQ H9Q$FN*JBX33UQ7541-ST*QGTA4W7.Y7G+SB.V Q5FN9EDKX*H/T12E4I*3156N$BEIV8OP :N%+3VFBZGK$1S7KHH*0R8KTPLL25KACUNLW S50OZ$1EW5BP5T%N9DWI/D 8KBUFF:0G%BDPB:A9C/3BDLK-3Z20V8N91";//insert a valid green pass data here, it can be obtained scanning the QR Code
            var res = await _sp.GetRequiredService<ValidationService>().Validate(scanResult);
            Console.WriteLine(res.ExpirationDate + " "+ res.ExpirationDate > DateTime.Now? "VALIDO": "SCADUTO");

        }
    }
}

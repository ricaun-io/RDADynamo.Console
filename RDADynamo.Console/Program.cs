using Autodesk.Forge.Oss.DesignAutomation;
using Autodesk.Forge.Oss.DesignAutomation.Attributes;
using Autodesk.Forge.Oss.DesignAutomation.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RDADynamo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var task = Task.Run(async () =>
            {
                await RunDynamo();
            });
            task.GetAwaiter().GetResult();
        }

        private static async Task RunDynamo()
        {

            IDesignAutomationService designAutomationService = new RevitDesignAutomationService("RDADynamo")
            {
                EngineVersions = new[] { "2024" },
                EnableConsoleLogger = true,
                ForceUpdateActivity = true,
                //ForceCreateWorkItemReport = true,
                EnableReportConsoleLogger = true,
                ForgeEnvironment = "test",
            };

            if (File.Exists("RDADynamo.bundle.zip") == false)
            {
                Console.WriteLine("Download RDADynamo.bundle.zip");
                var bundle = await RequestService.Instance.GetFileAsync("https://github.com/tothom/RDADynamo-example-implementation/releases/download/0.0.1/RDADynamo.bundle.zip");
            }

            await designAutomationService.Initialize("RDADynamo.bundle.zip");

            var sampleFolder = "set-parameter";

            await designAutomationService.Run<DynamoParameters>((parameters) =>
            {
                parameters.RevitFile = @$".\samples\{sampleFolder}\sample.rvt";
                parameters.DynamoInput = @$".\samples\{sampleFolder}\input.zip";
            });

            //await designAutomationService.Delete();
        }
    }

    internal class DynamoParameters
    {
        [ParameterActivityInputOpen]
        [ParameterInput("rvtFile.rvt", Required = true)]
        public string RevitFile { get; set; }
        [ParameterInput("input.zip", Required = true, Zip = true)]
        public string DynamoInput { get; set; }
        [ParameterOutput("result", Zip = true, DownloadFile = true)]
        public string DynamoOutput { get; set; }

        [ParameterOutput("result.rvt", DownloadFile = true)]
        public string RevitOutputFile { get; set; }
    }
}

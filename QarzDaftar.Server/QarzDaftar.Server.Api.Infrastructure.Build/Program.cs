using ADotNet.Clients;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks;
using ADotNet.Models.Pipelines.GithubPipelines.DotNets.Tasks.SetupDotNetTaskV1s;

var githubPipeline = new GithubPipeline
{
    Name = "QarzDaftar Build Pipline",

    OnEvents = new Events
    {
        PullRequest = new PullRequestEvent
        {
            Branches = new string[] { "master" }
        },

        Push = new PushEvent
        {
            Branches = new string[] { "master" }
        }
    },

    Jobs = new Dictionary<string, Job>
    {
        {
            "Build",
            new Job
            {
                RunsOn = BuildMachines.Windows2022,

                Steps = new List<GithubTask>
                {
                    new CheckoutTaskV2
                    {
                        Name = "Checking Out Code"
                    },

                    new SetupDotNetTaskV1
                    {
                        Name = "Seting Up .Net",

                        TargetDotNetVersion = new TargetDotNetVersion
                        {
                            DotNetVersion = "8.0.412",
                        }
                    },

                    new RunTask
                    {
                        Name = "Restore Packages",
                        Run = "dotnet restore QarzDaftar.Server.sln"
                    },
                    new RunTask
                    {
                        Name = "Build Project",
                        Run = "dotnet build QarzDaftar.Server.sln --no-restore"
                    },
                    new RunTask
                    {
                        Name = "Run Tests",
                        Run = "dotnet test QarzDaftar.Server.sln --no-build --verbosity normal"
                    }
                }
            }
        }
    }
};

var client = new ADotNetClient();

client.SerializeAndWriteToFile(
    adoPipeline: githubPipeline,
    path: "../../../../../.github/workflows/dotnet.yml");
#r "paket: groupref Build //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.DotNet
open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators //enables !! and globbing
let buildDir = "./build/"
let dotNetCliCommon =
  let defaults = Fake.DotNet.DotNet.Options.Create()
  { defaults with DotNetCliPath = "/usr/local/share/dotnet/dotnet"}

Target.create "InstallSdk" (fun _ ->
  let version = DotNet.getSDKVersionFromGlobalJson() |> DotNet.CliVersion.Version
  DotNet.install (fun opts -> { opts with Version = version}) dotNetCliCommon
  |> ignore
)

Target.create "Clean" (fun _ ->
  !! buildDir ++ "./**/bin" ++ "./**/obj"
  |> Seq.map(fun f -> printfn "%s" f; f)
  |> Shell.cleanDirs
)

Target.create "Restore" (fun _ ->
  !! "./**/*.fsproj"
  |> Seq.iter (fun proj -> DotNet.restore (fun ro -> { ro with Common = dotNetCliCommon }) proj)
)

Target.create "BuildLibrary" (fun _ ->
  !! "./TaskResultBuilder.fs/TaskResultBuilder.fs.fsproj"
  |> MSBuild.runRelease (fun bo -> { bo with RestorePackagesFlag = false }) buildDir "Build"
  |> Trace.logItems "AppBuild-Output: "
)


Target.create "RunUnitTests" (fun _ ->
  DotNet.test (fun testOptions -> { testOptions with NoRestore = true }) "./Tests/TaskBuilder.Tests.fsproj"
)
// Default target
Target.create "Default" (fun _ ->
  Trace.trace "Hello World from FAKE"
)


open Fake.Core.TargetOperators

"InstallSdk"
==> "Clean"
==> "Restore"
==> "BuildLibrary"
==> "RunUnitTests"
==> "Default"

// start build
Target.runOrDefault "Default"
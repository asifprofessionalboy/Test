when i put this code in csproj
<ItemGroup>
	<Content Include="wwwroot\faceApi\**\*">
		<CopyToOutputDirectory>Always</CopyToOutputDirectory>
	</Content>
</ItemGroup>

 then getting this error 

Severity	Code	Description	Project	File	Line	Suppression State
Error (active)	NETSDK1022	Duplicate 'Content' items were included. The .NET SDK includes 'Content' items from your project directory by default. You can either remove these items from your project file, or set the 'EnableDefaultContentItems' property to 'false' if you want to explicitly include them in your project file. For more information, see https://aka.ms/sdkimplicititems. The duplicate items were: 'wwwroot\faceApi\face_landmark_68_model-shard1'; 'wwwroot\faceApi\face_landmark_68_model-weights_manifest.json'; 'wwwroot\faceApi\tiny_face_detector_model-shard1'; 'wwwroot\faceApi\tiny_face_detector_model-weights_manifest.json'	GFAS	C:\Program Files\dotnet\sdk\8.0.400\Sdks\Microsoft.NET.Sdk\targets\Microsoft.NET.Sdk.DefaultItems.Shared.targets	2183	

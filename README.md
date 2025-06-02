<ItemGroup>
  <Content Include="wwwroot\faceApi\**\*">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </Content>
</ItemGroup>




i have this csproj

<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <Compile Remove="Controllers\COAController.cs\**" />
    <Content Remove="Controllers\COAController.cs\**" />
    <EmbeddedResource Remove="Controllers\COAController.cs\**" />
    <None Remove="Controllers\COAController.cs\**" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Dapper" Version="2.1.4" />
    <PackageReference Include="Emgu.CV" Version="4.10.0.5680" />
    <PackageReference Include="Emgu.CV.runtime.windows" Version="4.10.0.5680" />
    <PackageReference Include="MailKit" Version="4.0.0" />
    <PackageReference Include="Microsoft.Data.SqlClient" Version="5.1.4" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="6.0.36" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="6.0.36">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="6.0.36" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="6.0.36">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="6.0.18" />
  </ItemGroup>

  <ItemGroup>
    <Folder Include="wwwroot\CapturedImage\" />
    <Folder Include="wwwroot\lib\jquery-validation\dist\" />
  </ItemGroup>

  <ItemGroup>
    <None Include="wwwroot\faceApi\face_landmark_68_model-shard1" />
    <None Include="wwwroot\faceApi\tiny_face_detector_model-shard1" />
  </ItemGroup>

</Project>

#!/usr/bin/env python3
"""Generate i-ERP foundation source files. Idempotent overwrite."""
from __future__ import annotations

import os
from pathlib import Path
from textwrap import dedent

ROOT = Path(__file__).resolve().parents[1]


def write(rel: str, content: str) -> None:
    path = ROOT / rel
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(dedent(content).lstrip("\n").replace("\r\n", "\n"), encoding="utf-8")
    print(f"wrote {rel}")


# ---------------------------------------------------------------------------
# Project files
# ---------------------------------------------------------------------------

write(
    "src/BuildingBlocks/iERP.SharedKernel/iERP.SharedKernel.csproj",
    """
    <Project Sdk="Microsoft.NET.Sdk">
      <PropertyGroup>
        <RootNamespace>iERP.SharedKernel</RootNamespace>
        <AssemblyName>iERP.SharedKernel</AssemblyName>
      </PropertyGroup>
    </Project>
    """,
)

write(
    "src/BuildingBlocks/iERP.Application.Abstractions/iERP.Application.Abstractions.csproj",
    """
    <Project Sdk="Microsoft.NET.Sdk">
      <PropertyGroup>
        <RootNamespace>iERP.Application.Abstractions</RootNamespace>
        <AssemblyName>iERP.Application.Abstractions</AssemblyName>
      </PropertyGroup>
      <ItemGroup>
        <ProjectReference Include="..\\iERP.SharedKernel\\iERP.SharedKernel.csproj" />
      </ItemGroup>
    </Project>
    """,
)

write(
    "src/BuildingBlocks/iERP.Infrastructure/iERP.Infrastructure.csproj",
    """
    <Project Sdk="Microsoft.NET.Sdk">
      <PropertyGroup>
        <RootNamespace>iERP.Infrastructure</RootNamespace>
        <AssemblyName>iERP.Infrastructure</AssemblyName>
      </PropertyGroup>
      <ItemGroup>
        <FrameworkReference Include="Microsoft.AspNetCore.App" />
      </ItemGroup>
      <ItemGroup>
        <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" />
        <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" />
        <PackageReference Include="Microsoft.EntityFrameworkCore" />
        <PackageReference Include="Microsoft.EntityFrameworkCore.Relational" />
        <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />
        <PackageReference Include="FluentValidation" />
        <PackageReference Include="FluentValidation.DependencyInjectionExtensions" />
        <PackageReference Include="StackExchange.Redis" />
        <PackageReference Include="Microsoft.Extensions.Caching.StackExchangeRedis" />
        <PackageReference Include="Hangfire.AspNetCore" />
        <PackageReference Include="Hangfire.PostgreSql" />
        <PackageReference Include="Hangfire.Core" />
        <PackageReference Include="Azure.Messaging.ServiceBus" />
        <PackageReference Include="Azure.Storage.Blobs" />
        <PackageReference Include="OpenTelemetry.Extensions.Hosting" />
        <PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" />
        <PackageReference Include="OpenTelemetry.Instrumentation.Http" />
        <PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" />
        <PackageReference Include="AspNetCore.HealthChecks.NpgSql" />
        <PackageReference Include="AspNetCore.HealthChecks.Redis" />
        <PackageReference Include="Microsoft.Extensions.Options.ConfigurationExtensions" />
      </ItemGroup>
      <ItemGroup>
        <ProjectReference Include="..\\iERP.SharedKernel\\iERP.SharedKernel.csproj" />
        <ProjectReference Include="..\\iERP.Application.Abstractions\\iERP.Application.Abstractions.csproj" />
      </ItemGroup>
    </Project>
    """,
)

MODULE_CSProj = """
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>{ns}</RootNamespace>
    <AssemblyName>{asm}</AssemblyName>
  </PropertyGroup>
  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Relational" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />
    <PackageReference Include="FluentValidation" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\\..\\..\\BuildingBlocks\\iERP.SharedKernel\\iERP.SharedKernel.csproj" />
    <ProjectReference Include="..\\..\\..\\BuildingBlocks\\iERP.Application.Abstractions\\iERP.Application.Abstractions.csproj" />
    <ProjectReference Include="..\\..\\..\\BuildingBlocks\\iERP.Infrastructure\\iERP.Infrastructure.csproj" />
  </ItemGroup>
</Project>
"""

modules = {
    "Platform": "src/Modules/Platform/iERP.Modules.Platform/iERP.Modules.Platform.csproj",
    "Engines": "src/Modules/Engines/iERP.Modules.Engines/iERP.Modules.Engines.csproj",
    "CRM": "src/Modules/CRM/iERP.Modules.CRM/iERP.Modules.CRM.csproj",
    "Catalog": "src/Modules/Catalog/iERP.Modules.Catalog/iERP.Modules.Catalog.csproj",
    "Sales": "src/Modules/Sales/iERP.Modules.Sales/iERP.Modules.Sales.csproj",
    "Procurement": "src/Modules/Procurement/iERP.Modules.Procurement/iERP.Modules.Procurement.csproj",
    "Inventory": "src/Modules/Inventory/iERP.Modules.Inventory/iERP.Modules.Inventory.csproj",
    "Finance": "src/Modules/Finance/iERP.Modules.Finance/iERP.Modules.Finance.csproj",
    "Banking": "src/Modules/Banking/iERP.Modules.Banking/iERP.Modules.Banking.csproj",
    "Projects": "src/Modules/Projects/iERP.Modules.Projects/iERP.Modules.Projects.csproj",
    "HR": "src/Modules/HR/iERP.Modules.HR/iERP.Modules.HR.csproj",
    "Manufacturing": "src/Modules/Manufacturing/iERP.Modules.Manufacturing/iERP.Modules.Manufacturing.csproj",
    "Assets": "src/Modules/Assets/iERP.Modules.Assets/iERP.Modules.Assets.csproj",
    "Marine": "src/Modules/Marine/iERP.Modules.Marine/iERP.Modules.Marine.csproj",
    "Reporting": "src/Modules/Reporting/iERP.Modules.Reporting/iERP.Modules.Reporting.csproj",
    "AI": "src/Modules/AI/iERP.Modules.AI/iERP.Modules.AI.csproj",
}

for name, path in modules.items():
    write(path, MODULE_CSProj.format(ns=f"iERP.Modules.{name}", asm=f"iERP.Modules.{name}"))

# Platform needs Identity EF package
write(
    "src/Modules/Platform/iERP.Modules.Platform/iERP.Modules.Platform.csproj",
    """
    <Project Sdk="Microsoft.NET.Sdk">
      <PropertyGroup>
        <RootNamespace>iERP.Modules.Platform</RootNamespace>
        <AssemblyName>iERP.Modules.Platform</AssemblyName>
      </PropertyGroup>
      <ItemGroup>
        <FrameworkReference Include="Microsoft.AspNetCore.App" />
      </ItemGroup>
      <ItemGroup>
        <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" />
        <PackageReference Include="Microsoft.EntityFrameworkCore" />
        <PackageReference Include="Microsoft.EntityFrameworkCore.Relational" />
        <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />
        <PackageReference Include="FluentValidation" />
      </ItemGroup>
      <ItemGroup>
        <ProjectReference Include="..\\..\\..\\BuildingBlocks\\iERP.SharedKernel\\iERP.SharedKernel.csproj" />
        <ProjectReference Include="..\\..\\..\\BuildingBlocks\\iERP.Application.Abstractions\\iERP.Application.Abstractions.csproj" />
        <ProjectReference Include="..\\..\\..\\BuildingBlocks\\iERP.Infrastructure\\iERP.Infrastructure.csproj" />
      </ItemGroup>
    </Project>
    """,
)

# AI needs Semantic Kernel
write(
    "src/Modules/AI/iERP.Modules.AI/iERP.Modules.AI.csproj",
    """
    <Project Sdk="Microsoft.NET.Sdk">
      <PropertyGroup>
        <RootNamespace>iERP.Modules.AI</RootNamespace>
        <AssemblyName>iERP.Modules.AI</AssemblyName>
      </PropertyGroup>
      <ItemGroup>
        <FrameworkReference Include="Microsoft.AspNetCore.App" />
      </ItemGroup>
      <ItemGroup>
        <PackageReference Include="Microsoft.EntityFrameworkCore" />
        <PackageReference Include="Microsoft.EntityFrameworkCore.Relational" />
        <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />
        <PackageReference Include="Microsoft.SemanticKernel" />
        <PackageReference Include="FluentValidation" />
      </ItemGroup>
      <ItemGroup>
        <ProjectReference Include="..\\..\\..\\BuildingBlocks\\iERP.SharedKernel\\iERP.SharedKernel.csproj" />
        <ProjectReference Include="..\\..\\..\\BuildingBlocks\\iERP.Application.Abstractions\\iERP.Application.Abstractions.csproj" />
        <ProjectReference Include="..\\..\\..\\BuildingBlocks\\iERP.Infrastructure\\iERP.Infrastructure.csproj" />
      </ItemGroup>
    </Project>
    """,
)

module_refs = "\n".join(
    f'    <ProjectReference Include="..\\Modules\\{n}\\iERP.Modules.{n}\\iERP.Modules.{n}.csproj" />'
    if n not in ("Platform", "Engines", "CRM", "AI", "HR")
    else (
        f'    <ProjectReference Include="..\\Modules\\Platform\\iERP.Modules.Platform\\iERP.Modules.Platform.csproj" />'
        if n == "Platform"
        else f'    <ProjectReference Include="..\\Modules\\Engines\\iERP.Modules.Engines\\iERP.Modules.Engines.csproj" />'
        if n == "Engines"
        else f'    <ProjectReference Include="..\\Modules\\CRM\\iERP.Modules.CRM\\iERP.Modules.CRM.csproj" />'
        if n == "CRM"
        else f'    <ProjectReference Include="..\\Modules\\AI\\iERP.Modules.AI\\iERP.Modules.AI.csproj" />'
        if n == "AI"
        else f'    <ProjectReference Include="..\\Modules\\HR\\iERP.Modules.HR\\iERP.Modules.HR.csproj" />'
    )
    for n in [
        "Platform",
        "Engines",
        "CRM",
        "Catalog",
        "Sales",
        "Procurement",
        "Inventory",
        "Finance",
        "Banking",
        "Projects",
        "HR",
        "Manufacturing",
        "Assets",
        "Marine",
        "Reporting",
        "AI",
    ]
)

write(
    "src/iERP.Migrations/iERP.Migrations.csproj",
    f"""
    <Project Sdk="Microsoft.NET.Sdk">
      <PropertyGroup>
        <RootNamespace>iERP.Migrations</RootNamespace>
        <AssemblyName>iERP.Migrations</AssemblyName>
      </PropertyGroup>
      <ItemGroup>
        <PackageReference Include="Microsoft.EntityFrameworkCore.Design">
          <PrivateAssets>all</PrivateAssets>
          <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
        </PackageReference>
        <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />
      </ItemGroup>
      <ItemGroup>
        <ProjectReference Include="..\\BuildingBlocks\\iERP.Infrastructure\\iERP.Infrastructure.csproj" />
    {module_refs}
      </ItemGroup>
    </Project>
    """,
)

write(
    "src/iERP.Api/iERP.Api.csproj",
    f"""
    <Project Sdk="Microsoft.NET.Sdk.Web">
      <PropertyGroup>
        <RootNamespace>iERP.Api</RootNamespace>
        <AssemblyName>iERP.Api</AssemblyName>
      </PropertyGroup>
      <ItemGroup>
        <PackageReference Include="Microsoft.AspNetCore.OpenApi" />
        <PackageReference Include="Swashbuckle.AspNetCore" />
        <PackageReference Include="Microsoft.EntityFrameworkCore.Design">
          <PrivateAssets>all</PrivateAssets>
          <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
        </PackageReference>
      </ItemGroup>
      <ItemGroup>
        <ProjectReference Include="..\\BuildingBlocks\\iERP.Infrastructure\\iERP.Infrastructure.csproj" />
        <ProjectReference Include="..\\iERP.Migrations\\iERP.Migrations.csproj" />
    {module_refs}
      </ItemGroup>
    </Project>
    """,
)

write(
    "src/iERP.Worker/iERP.Worker.csproj",
    f"""
    <Project Sdk="Microsoft.NET.Sdk.Worker">
      <PropertyGroup>
        <RootNamespace>iERP.Worker</RootNamespace>
        <AssemblyName>iERP.Worker</AssemblyName>
      </PropertyGroup>
      <ItemGroup>
        <PackageReference Include="Microsoft.Extensions.Hosting" />
        <PackageReference Include="Hangfire.AspNetCore" />
        <PackageReference Include="Hangfire.PostgreSql" />
      </ItemGroup>
      <ItemGroup>
        <ProjectReference Include="..\\BuildingBlocks\\iERP.Infrastructure\\iERP.Infrastructure.csproj" />
        <ProjectReference Include="..\\iERP.Migrations\\iERP.Migrations.csproj" />
    {module_refs}
      </ItemGroup>
    </Project>
    """,
)

write(
    "tests/iERP.ArchitectureTests/iERP.ArchitectureTests.csproj",
    """
    <Project Sdk="Microsoft.NET.Sdk">
      <PropertyGroup>
        <RootNamespace>iERP.ArchitectureTests</RootNamespace>
        <IsPackable>false</IsPackable>
      </PropertyGroup>
      <ItemGroup>
        <PackageReference Include="Microsoft.NET.Test.Sdk" />
        <PackageReference Include="xunit" />
        <PackageReference Include="xunit.runner.visualstudio" />
        <PackageReference Include="FluentAssertions" />
        <PackageReference Include="NetArchTest.Rules" />
        <PackageReference Include="coverlet.collector" />
      </ItemGroup>
      <ItemGroup>
        <ProjectReference Include="..\\..\\src\\BuildingBlocks\\iERP.SharedKernel\\iERP.SharedKernel.csproj" />
        <ProjectReference Include="..\\..\\src\\BuildingBlocks\\iERP.Application.Abstractions\\iERP.Application.Abstractions.csproj" />
        <ProjectReference Include="..\\..\\src\\BuildingBlocks\\iERP.Infrastructure\\iERP.Infrastructure.csproj" />
        <ProjectReference Include="..\\..\\src\\Modules\\CRM\\iERP.Modules.CRM\\iERP.Modules.CRM.csproj" />
        <ProjectReference Include="..\\..\\src\\Modules\\Sales\\iERP.Modules.Sales\\iERP.Modules.Sales.csproj" />
        <ProjectReference Include="..\\..\\src\\Modules\\Platform\\iERP.Modules.Platform\\iERP.Modules.Platform.csproj" />
        <ProjectReference Include="..\\..\\src\\iERP.Api\\iERP.Api.csproj" />
      </ItemGroup>
    </Project>
    """,
)

write(
    "tests/iERP.UnitTests/iERP.UnitTests.csproj",
    """
    <Project Sdk="Microsoft.NET.Sdk">
      <PropertyGroup>
        <RootNamespace>iERP.UnitTests</RootNamespace>
        <IsPackable>false</IsPackable>
      </PropertyGroup>
      <ItemGroup>
        <PackageReference Include="Microsoft.NET.Test.Sdk" />
        <PackageReference Include="xunit" />
        <PackageReference Include="xunit.runner.visualstudio" />
        <PackageReference Include="FluentAssertions" />
        <PackageReference Include="coverlet.collector" />
      </ItemGroup>
      <ItemGroup>
        <ProjectReference Include="..\\..\\src\\BuildingBlocks\\iERP.SharedKernel\\iERP.SharedKernel.csproj" />
      </ItemGroup>
    </Project>
    """,
)

write(
    "tests/iERP.IntegrationTests/iERP.IntegrationTests.csproj",
    """
    <Project Sdk="Microsoft.NET.Sdk">
      <PropertyGroup>
        <RootNamespace>iERP.IntegrationTests</RootNamespace>
        <IsPackable>false</IsPackable>
      </PropertyGroup>
      <ItemGroup>
        <PackageReference Include="Microsoft.NET.Test.Sdk" />
        <PackageReference Include="xunit" />
        <PackageReference Include="xunit.runner.visualstudio" />
        <PackageReference Include="FluentAssertions" />
        <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" />
        <PackageReference Include="coverlet.collector" />
      </ItemGroup>
      <ItemGroup>
        <ProjectReference Include="..\\..\\src\\iERP.Api\\iERP.Api.csproj" />
      </ItemGroup>
    </Project>
    """,
)

print("csproj done")

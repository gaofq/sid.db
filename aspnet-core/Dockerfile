# 使用.NET 10 SDK镜像作为构建环境
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
# 设置工作目录为/src
WORKDIR /src

# 复制项目公共配置文件
COPY common.props .
# 复制NuGet包源配置文件
COPY NuGet.Config .

# 复制各个项目的.csproj文件（分层缓存，只有项目文件变化时才会重新还原依赖）
COPY src/Sid.DbFieldManager.Domain.Shared/Sid.DbFieldManager.Domain.Shared.csproj src/Sid.DbFieldManager.Domain.Shared/
COPY src/Sid.DbFieldManager.Domain/Sid.DbFieldManager.Domain.csproj src/Sid.DbFieldManager.Domain/
COPY src/Sid.DbFieldManager.Application.Contracts/Sid.DbFieldManager.Application.Contracts.csproj src/Sid.DbFieldManager.Application.Contracts/
COPY src/Sid.DbFieldManager.Application/Sid.DbFieldManager.Application.csproj src/Sid.DbFieldManager.Application/
COPY src/Sid.DbFieldManager.EntityFrameworkCore/Sid.DbFieldManager.EntityFrameworkCore.csproj src/Sid.DbFieldManager.EntityFrameworkCore/
COPY src/Sid.DbFieldManager.HttpApi/Sid.DbFieldManager.HttpApi.csproj src/Sid.DbFieldManager.HttpApi/
COPY src/Sid.DbFieldManager.HttpApi.Host/Sid.DbFieldManager.HttpApi.Host.csproj src/Sid.DbFieldManager.HttpApi.Host/
COPY src/Sid.DbFieldManager.HttpApi.Client/Sid.DbFieldManager.HttpApi.Client.csproj src/Sid.DbFieldManager.HttpApi.Client/
COPY src/Sid.DbFieldManager.DbMigrator/Sid.DbFieldManager.DbMigrator.csproj src/Sid.DbFieldManager.DbMigrator/

# 还原NuGet依赖包
RUN dotnet restore src/Sid.DbFieldManager.HttpApi.Host/Sid.DbFieldManager.HttpApi.Host.csproj

# 复制所有源代码
COPY . .

# 发布项目到/app目录，Release配置，不重复还原依赖
RUN dotnet publish src/Sid.DbFieldManager.HttpApi.Host/Sid.DbFieldManager.HttpApi.Host.csproj \
    -c Release \
    -o /app \
    --no-restore

# 同时发布数据库迁移工具到/app/db-migrator目录
RUN dotnet publish src/Sid.DbFieldManager.DbMigrator/Sid.DbFieldManager.DbMigrator.csproj \
    -c Release \
    -o /app/db-migrator \
    --no-restore

# 运行阶段，使用更小的ASP.NET Core Runtime镜像
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
# 设置工作目录为/app
WORKDIR /app
# 暴露8090端口供外部访问
EXPOSE 8090

# 设置环境变量为生产环境
ENV ASPNETCORE_ENVIRONMENT=Production
# 设置ASP.NET Core监听地址为所有网卡的8090端口
ENV ASPNETCORE_URLS=http://+:8090

# 从构建阶段复制发布好的程序文件
COPY --from=build /app .

# ABP框架会检查wwwroot/libs目录是否存在客户端库，纯API项目不需要这些库，创建空目录绕过检查
RUN mkdir -p /app/wwwroot/libs

# 使用非root用户运行，提升容器安全性
USER $APP_UID
# 容器启动入口，运行后端服务
ENTRYPOINT ["dotnet", "Sid.DbFieldManager.HttpApi.Host.dll"]

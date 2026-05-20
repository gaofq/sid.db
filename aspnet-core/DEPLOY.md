# 后端服务 Docker 部署文档

## 一、环境要求
- Docker Engine 20.10+
- 可选：Docker Compose 2.0+（整体部署推荐）
- 数据库：SQL Server 2016+（可以是独立部署或Docker容器部署）

## 二、独立部署步骤
### 1. 构建镜像
在 `aspnet-core` 目录下执行：
```bash
docker build -t sid-db-field-manager-backend .
```

### 2. 运行容器
```bash
docker run -d \
  --name sid-db-backend \
  -p 8090:8090 \
  -e "ConnectionStrings__Default=Server=host.docker.internal;Database=SidDbFieldManager;User Id=sa;Password=your_password;TrustServerCertificate=True" \
  -e "ASPNETCORE_ENVIRONMENT=Production" \
  -e "App__CorsOrigins=http://localhost:8091,http://your-domain.com" \
  --restart always \
  sid-db-field-manager-backend
```

#### 参数说明：
| 参数 | 说明 |
|------|------|
| `-p 8090:8090` | 端口映射，宿主机8090端口映射到容器内8090端口 |
| `ConnectionStrings__Default` | SQL Server数据库连接字符串，`host.docker.internal`可以访问宿主机的服务 |
| `ASPNETCORE_ENVIRONMENT` | 运行环境，可选值：Development/Staging/Production |
| `App__CorsOrigins` | 允许跨域的前端地址，多个地址用逗号分隔 |
| `--restart always` | 容器自动重启策略，开机自动启动 |

### 3. 验证部署
访问：`http://localhost:8090/swagger`，可以看到Swagger接口文档表示部署成功。

## 三、常用操作
### 查看日志
```bash
docker logs -f sid-db-backend
```

### 停止容器
```bash
docker stop sid-db-backend
```

### 启动容器
```bash
docker start sid-db-backend
```

### 删除容器
```bash
docker rm -f sid-db-backend
```

### 进入容器内部
```bash
docker exec -it sid-db-backend /bin/bash
```

## 四、配置说明
### 1. 环境变量列表
| 环境变量 | 默认值 | 说明 |
|----------|--------|------|
| `ASPNETCORE_ENVIRONMENT` | `Production` | 运行环境 |
| `ASPNETCORE_URLS` | `http://+:8090` | 服务监听地址 |
| `ConnectionStrings__Default` | - | 必选，数据库连接字符串 |
| `App__CorsOrigins` | `*` | 跨域允许的来源 |
| `App__EnableSwagger` | `true` | 是否启用Swagger文档（生产环境建议关闭） |

### 2. 持久化目录
如果需要自定义配置文件或日志持久化，可以挂载以下目录：
```bash
-v /your/path/appsettings.json:/app/appsettings.Production.json
-v /your/path/logs:/app/Logs
```

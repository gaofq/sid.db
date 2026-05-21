# Docker 常用命令手册

## 📋 基础概念
| 术语 | 说明 |
|------|------|
| **镜像(Image)** | 应用的只读模板，包含运行环境和代码（相当于操作系统ISO文件） |
| **容器(Container)** | 镜像的运行实例，每个容器独立运行（相当于一台虚拟主机） |
| **Docker Compose** | 多容器编排工具，通过`docker-compose.yml`一次性管理多个关联容器 |
| **Volume** | 持久化存储，容器删除后数据不会丢失 |

---

## 🛠️ Docker 基础常用命令
### 1. 镜像相关命令
| 命令 | 功能 |
|------|------|
| `docker images` | 列出本地所有镜像 |
| `docker pull 镜像名:标签` | 从远程仓库拉取镜像（比如`docker pull mcr.microsoft.com/dotnet/aspnet:10.0`） |
| `docker build -t 镜像名:标签 .` | 基于当前目录的Dockerfile构建镜像 |
| `docker rmi 镜像名/镜像ID` | 删除镜像 |
| `docker image prune -a` | 清理所有没有被使用的镜像 |

### 2. 容器相关命令
| 命令 | 功能 |
|------|------|
| `docker ps` | 列出正在运行的容器 |
| `docker ps -a` | 列出所有容器（包括停止的） |
| `docker start 容器名/容器ID` | 启动已停止的容器 |
| `docker stop 容器名/容器ID` | 停止运行中的容器 |
| `docker restart 容器名/容器ID` | 重启容器 |
| `docker rm 容器名/容器ID` | 删除已停止的容器 |
| `docker rm -f 容器名/容器ID` | 强制删除正在运行的容器 |
| `docker exec -it 容器名/容器ID /bin/sh` | 进入容器内部执行命令 |
| `docker inspect 容器名/容器ID` | 查看容器详细信息（IP、配置等） |

### 3. 日志相关命令
| 命令 | 功能 |
|------|------|
| `docker logs 容器名/容器ID` | 查看容器日志 |
| `docker logs -f 容器名/容器ID` | 实时跟踪容器日志 |
| `docker logs --tail 100 容器名/容器ID` | 查看最新100行日志 |
| `docker logs -f --tail 100 容器名/容器ID` | 实时跟踪最新100行日志 |

### 4. 资源清理命令
| 命令 | 功能 |
|------|------|
| `docker system prune` | 清理停止的容器、未使用的网络、悬空镜像、构建缓存 |
| `docker system prune -a` | 更彻底清理：除了上面的，还会清理所有没有被容器使用的镜像 |
| `docker volume prune` | 清理未被使用的数据卷（注意：会删除没用的持久化数据，谨慎使用） |

---

## 🚀 Docker Compose 常用命令
> 注意：需要在`docker-compose.yml`所在目录执行
> 新版Docker命令是`docker compose`（中间没有横杠），旧版是`docker-compose`，本项目的部署脚本已经自动兼容两者。

### 1. 基础操作命令
| 命令 | 功能 |
|------|------|
| `docker compose up` | 前台启动所有服务（日志直接输出到控制台，按Ctrl+C停止） |
| `docker compose up -d` | **后台启动所有服务**（最常用） |
| `docker compose up -d --build` | **重新构建镜像并后台启动**（代码修改后需要重新构建用这个） |
| `docker compose up -d --force-recreate` | **强制重建所有容器并启动**（配置修改后需要生效用这个） |
| `docker compose stop` | 停止所有运行中的服务 |
| `docker compose start` | 启动已停止的所有服务 |
| `docker compose restart` | 重启所有服务 |
| `docker compose down` | 停止并删除所有容器、网络（不会删除镜像和持久化数据） |
| `docker compose down -v` | 停止删除容器的同时删除数据卷（会清空持久化数据，谨慎使用） |

### 2. 日志与状态命令
| 命令 | 功能 |
|------|------|
| `docker compose logs` | 查看所有服务日志 |
| `docker compose logs -f` | 实时跟踪所有服务日志 |
| `docker compose logs 服务名` | 查看指定服务日志（比如`docker compose logs backend`看后端日志） |
| `docker compose logs -f 服务名` | 实时跟踪指定服务日志 |
| `docker compose ps` | 查看所有服务运行状态 |
| `docker compose top` | 查看所有服务运行的进程 |

### 3. 构建与管理命令
| 命令 | 功能 |
|------|------|
| `docker compose build` | 只构建镜像，不启动服务 |
| `docker compose build --no-cache` | 无缓存构建镜像，强制重新拉取所有依赖 |
| `docker compose pull` | 拉取镜像文件中指定的最新镜像 |
| `docker compose exec 服务名 命令` | 在指定服务容器内执行命令（比如`docker compose exec backend bash`进入后端容器） |

---

## 🎯 本项目专属常用命令
### 1. Windows环境（PowerShell）
```powershell
# 后台启动项目（自动构建镜像）
.\deploy.ps1 up -d

# 重新构建并启动
.\deploy.ps1 up -d --build

# 强制重建容器（配置修改后生效）
.\deploy.ps1 up -d --force-recreate

# 查看运行状态
.\deploy.ps1 status

# 查看后端实时日志
.\deploy.ps1 logs backend -f

# 查看前端实时日志
.\deploy.ps1 logs frontend -f

# 停止项目
.\deploy.ps1 down

# 清理所有相关资源（镜像、容器）
.\deploy.ps1 clean
```

### 2. Linux环境
```bash
# 给部署脚本加执行权限（第一次用需要执行）
chmod +x deploy.sh

# 后台启动项目
./deploy.sh up -d

# 重新构建并启动
./deploy.sh up -d --build

# 强制重建容器
./deploy.sh up -d --force-recreate

# 查看状态
./deploy.sh status

# 查看后端日志
./deploy.sh logs backend -f

# 停止项目
./deploy.sh down
```

---

## 🔍 常见问题排查命令
### 1. 容器启动失败排查
```bash
# 先看容器状态
docker compose ps

# 看错误日志
docker compose logs 服务名 --tail 200
```

### 2. 网络问题排查
```bash
# 查看容器IP
docker inspect 容器名 | grep IPAddress

# 测试容器端口连通性
telnet 容器IP 端口

# 进入容器内部测试网络
docker exec -it 容器名 /bin/sh
ping 数据库IP
telnet 数据库IP 1433
```

### 3. 资源占用排查
```bash
# 查看容器资源占用（CPU、内存、磁盘）
docker stats

# 查看磁盘使用情况
docker system df
```

---

## ⚠️ 生产环境注意事项
1. **不要用`docker rm -f`强制删除正在运行的生产容器，会导致业务中断**
2. **生产环境清理资源前先执行`docker system prune --dry-run`看看会清理什么，确认无误再执行**
3. **重要数据一定要持久化到Volume或者宿主机目录，不要存在容器内部**
4. **定期备份数据库和重要持久化数据**
5. **生产环境不要用`--build`参数在线构建镜像，应该提前构建好镜像上传到镜像仓库再部署**
6. **配置容器资源限制（CPU、内存），防止单个容器占用过多资源影响其他服务**

# SQL Server 2022 Docker 部署手册

## 📋 版本说明
我们使用的是**SQL Server 2022 最新LTS版本**，无旧版TLS协议兼容问题，性能和安全性都大幅提升：
- `Developer版`：免费，功能和企业版完全一致，适合开发测试环境
- `Enterprise版`：付费，适合生产环境，需要购买授权
- 支持x86/arm64架构，兼容Windows/Linux/Mac全平台

---

## 🚀 一键部署方法
### 1. 首次启动
> 本项目已经把SQL Server集成到docker-compose.yml里，**不需要单独安装数据库**，直接启动整个项目即可：
```powershell
# Windows
.\deploy.ps1 up -d

# Linux
./deploy.sh up -d
```
启动顺序：先启动SQL Server → 再启动后端 → 最后启动前端，全自动不需要干预。

### 2. 初始化数据库
第一次启动SQL Server是空的，需要执行数据库迁移创建表结构：
#### 方法一：用项目自带的DbMigrator工具（推荐）
```bash
# 进入aspnet-core/src/Sid.DbFieldManager.DbMigrator目录
cd aspnet-core/src/Sid.DbFieldManager.DbMigrator
# 运行迁移工具，会自动创建数据库、表、初始化种子数据
dotnet run
```
#### 方法二：手动还原备份
如果有现成的数据库备份文件(.bak)，可以用SSMS连接到数据库后手动还原。

---

## 🔌 数据库连接信息
| 项 | 值 | 说明 |
|----|----|------|
| **地址** | `localhost` 或服务器IP | 外部工具连接用这个 |
| **端口** | `1433` | 默认SQL Server端口，可在docker-compose.yml修改 |
| **账号** | `sa` | 系统管理员账号 |
| **密码** | `Fkh6G2B4s73N@2024` | 可在docker-compose.yml的SA_PASSWORD自定义 |
| **数据库名** | `DbFieldManager` | 项目专用数据库 |
| **容器内部连接地址** | `sqlserver:1433` | 同一个docker网络里的服务可以直接用服务名访问，不需要IP |

### 外部连接示例
用SSMS、Navicat等工具连接时：
- 服务器名：`你的服务器IP,1433`
- 身份验证：SQL Server身份验证
- 登录名：`sa`
- 密码：`Fkh6G2B4s73N@2024`

---

## 🛠️ 常用运维命令
### 1. 基础操作命令
| 命令 | 功能 |
|------|------|
| `docker compose logs sqlserver -f` | 实时查看SQL Server运行日志 |
| `docker compose restart sqlserver` | 重启SQL Server |
| `docker compose stop sqlserver` | 停止SQL Server |
| `docker compose start sqlserver` | 启动已停止的SQL Server |
| `docker exec -it dbfm-sqlserver /bin/bash` | 进入SQL Server容器内部 |

### 2. 数据备份
#### 自动备份（推荐）
```bash
# 执行备份，备份文件会保存在当前目录
docker exec -t dbfm-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Fkh6G2B4s73N@2024 -Q "BACKUP DATABASE DbFieldManager TO DISK = '/var/opt/mssql/backup/DbFieldManager_$(date +%Y%m%d_%H%M%S).bak' WITH INIT"

# 把备份文件从容器复制到宿主机
docker cp dbfm-sqlserver:/var/opt/mssql/backup ./sql_backup
```
可以把这个命令写成定时脚本，每天自动备份。

#### 手动备份
用SSMS连接到数据库后，右键数据库 → 任务 → 备份，可视化操作。

### 3. 数据恢复
```bash
# 1. 把备份文件复制到容器内
docker cp 你的备份文件.bak dbfm-sqlserver:/var/opt/mssql/restore/

# 2. 执行恢复命令
docker exec -t dbfm-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Fkh6G2B4s73N@2024 -Q "RESTORE DATABASE DbFieldManager FROM DISK = '/var/opt/mssql/restore/你的备份文件.bak' WITH REPLACE, RECOVERY"
```

### 4. 修改SA密码
```bash
docker exec -t dbfm-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 旧密码 -Q "ALTER LOGIN sa WITH PASSWORD = '新密码'"
```
修改后需要同步更新docker-compose.yml里的连接串密码。

### 5. 完全重置数据库
> 注意：会清空所有数据，谨慎操作！
```bash
# 停止服务
docker compose down
# 删除数据卷
docker volume rm sid.db_sqlserver_data
# 重新启动
docker compose up -d
```

---

## 📈 生产环境优化建议
### 1. 版本选择
- 把`MSSQL_PID`从`Developer`改成`Enterprise`，购买对应的企业版授权
- 固定镜像版本号，不要用`latest`，比如改成`mcr.microsoft.com/mssql/server:2022-CU12-ubuntu-22.04`，避免意外升级

### 2. 性能优化
```yaml
# 在sqlserver服务下添加资源限制
deploy:
  resources:
    limits:
      cpus: '4'
      memory: 8G
    reservations:
      cpus: '2'
      memory: 4G
```
SQL Server至少需要4G内存才能流畅运行，生产环境建议分配8G以上内存。

### 3. 数据持久化优化
- 把数据卷挂载到宿主机SSD磁盘，大幅提升IO性能：
```yaml
volumes:
  - /你的SSD磁盘路径/sqlserver_data:/var/opt/mssql
```
- 定期清理事务日志，避免磁盘占满

### 4. 安全优化
- 修改默认SA密码，使用强密码
- 不要把1433端口对外网暴露，只允许白名单IP访问
- 开启SQL Server审计功能，记录所有操作日志
- 定期安装安全补丁，更新镜像版本

### 5. 高可用
生产环境建议配置：
- 主从复制，读写分离
- 自动故障切换
- 异地备份

---

## 🔍 常见问题
### 1. SQL Server启动失败，提示内存不足
SQL Server默认要求至少2G内存才能启动，检查宿主机剩余内存，至少保留4G以上可用内存。如果内存不够，可以添加环境变量限制内存：
```yaml
environment:
  - MSSQL_MEMORY_LIMIT_MB=2048
```

### 2. 外部工具连不上数据库
- 检查宿主机防火墙/安全组是否开放1433端口
- 确认docker-compose.yml里的端口映射是`"1433:1433"`
- 检查SA密码是否正确，密码必须至少8位，包含大小写字母、数字、特殊字符

### 3. 后端服务启动时连不上数据库
SQL Server第一次启动需要初始化大概1-2分钟，后端会自动重试3次，等数据库初始化完成后会自动连接成功，如果还是失败，手动重启后端服务即可：
```bash
docker compose restart backend
```

### 4. 数据库文件占满磁盘
- 定期清理备份文件
- 收缩数据库日志
- 清理不用的历史数据

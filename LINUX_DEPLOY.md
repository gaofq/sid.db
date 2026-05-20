# Linux 环境 Docker 部署文档

## 一、系统环境要求
- 操作系统：CentOS 7+/Ubuntu 18.04+/Debian 10+
- Docker Engine 20.10+
- Docker Compose 2.0+
- 内存：至少2G可用内存
- 磁盘：至少10G可用磁盘空间
- 数据库：SQL Server 2016+（可以是同服务器部署或远程服务器）

## 二、环境安装（如果已有环境可跳过）
### 1. 安装 Docker
```bash
# 卸载旧版本
sudo yum remove docker docker-client docker-client-latest docker-common docker-latest docker-latest-logrotate docker-logrotate docker-engine

# 安装依赖
sudo yum install -y yum-utils device-mapper-persistent-data lvm2

# 添加Docker源
sudo yum-config-manager --add-repo https://download.docker.com/linux/centos/docker-ce.repo

# 安装Docker
sudo yum install -y docker-ce docker-ce-cli containerd.io

# 启动Docker并设置开机自启
sudo systemctl start docker
sudo systemctl enable docker

# 验证安装
docker --version
```

### 2. 安装 Docker Compose
```bash
# 下载Docker Compose（v2.27.0版本，可根据需要更新版本号）
sudo curl -L "https://github.com/docker/compose/releases/download/v2.27.0/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose

# 加执行权限
sudo chmod +x /usr/local/bin/docker-compose

# 验证安装
docker-compose --version
```

### 3. 配置当前用户免sudo执行docker（可选）
```bash
sudo groupadd docker
sudo gpasswd -a ${USER} docker
sudo systemctl restart docker
newgrp docker  # 刷新用户组，无需重新登录
```

## 三、部署步骤
### 1. 上传代码
将整个项目代码上传到Linux服务器任意目录，比如`/opt/sid-db-field-manager`：
```bash
mkdir -p /opt/sid-db-field-manager
cd /opt/sid-db-field-manager
# 上传代码到这个目录，或者git clone
```

### 2. 配置数据库连接
编辑`.env`文件（没有则新建）：
```env
# 数据库连接字符串，根据实际情况修改
DB_CONNECTION=Server=你的数据库IP,1433;Database=DbFieldManager;User Id=sa;Password=你的数据库密码;TrustServerCertificate=true

# 字符串加密密钥，可保持默认或自定义
STRING_ENCRYPT_PASSPHRASE=xLUxRRN16IANIM4W
```

### 3. 给部署脚本加执行权限
```bash
chmod +x deploy.sh
```

### 4. 启动服务
```bash
# 后台启动并自动构建镜像
./deploy.sh up -d --build
```

### 5. 验证部署
```bash
# 查看服务状态
./deploy.sh status

# 确保两个服务都是Up状态
# dbfm-backend   Up
# dbfm-frontend  Up
```

### 6. 开放防火墙端口
如果服务器开启了防火墙，需要开放访问端口：
```bash
# CentOS/Firewalld
sudo firewall-cmd --add-port=8090/tcp --permanent
sudo firewall-cmd --add-port=8091/tcp --permanent
sudo firewall-cmd --reload

# Ubuntu/UFW
sudo ufw allow 8090/tcp
sudo ufw allow 8091/tcp
sudo ufw reload
```

## 四、访问系统
- 后端接口文档：`http://你的服务器IP:8090/swagger`
- 前端系统地址：`http://你的服务器IP:8091`
- 默认管理员账号：`admin` / `1q2w3E*`

## 五、常用运维操作
### 查看服务日志
```bash
# 查看所有日志
./deploy.sh logs

# 实时查看后端日志
./deploy.sh logs backend -f

# 实时查看前端日志
./deploy.sh logs frontend -f
```

### 重启服务
```bash
./deploy.sh restart

# 只重启某个服务
./deploy.sh restart backend
```

### 停止服务
```bash
./deploy.sh down
```

### 升级版本
```bash
# 拉取最新代码
git pull

# 重新构建并启动
./deploy.sh up -d --build
```

### 清理资源（开发环境使用）
```bash
./deploy.sh clean
```

## 六、常见问题解决
### 1. 后端连接不上数据库
- 检查数据库连接字符串是否正确
- 检查数据库是否允许远程访问
- 检查数据库服务器防火墙是否开放1433端口
- 如果数据库部署在当前服务器，连接地址可以用`host.docker.internal`代替IP

### 2. 前端访问显示502 Bad Gateway
- 检查后端服务是否正常启动：`./deploy.sh status`
- 查看后端日志排查错误：`./deploy.sh logs backend`
- 检查Nginx配置是否正确

### 3. 前端访问接口跨域错误
- 检查docker-compose.yml中`App__CorsOrigins`配置是否包含前端访问地址
- 多个地址用逗号分隔，比如：`App__CorsOrigins=http://localhost:8091,http://你的域名`

### 4. 权限错误：Permission denied
- 确保当前用户有docker执行权限，或者加sudo运行
- 如果是挂载目录权限问题，执行：`chmod -R 755 挂载目录`
- SELinux问题：临时关闭SELinux测试 `setenforce 0`，或者配置SELinux规则

### 5. host.docker.internal无法访问宿主机
- 我们的docker-compose.yml已经配置了`extra_hosts: - "host.docker.internal:host-gateway"`，支持Linux系统
- 如果还是不行，直接使用宿主机的公网IP或者内网IP代替

## 七、生产环境建议
1. **配置HTTPS**：使用Nginx或者反向代理配置SSL证书，建议使用Let's Encrypt免费证书
2. **数据备份**：定期备份SQL Server数据库，建议每日自动备份
3. **日志持久化**：配置ELK或者其他日志收集系统，收集服务运行日志
4. **监控告警**：配置Prometheus+Grafana监控服务运行状态和资源使用
5. **安全加固**：关闭不必要的端口，配置防火墙规则，限制数据库访问IP
6. **容器资源限制**：在docker-compose.yml中添加cpu和内存限制，防止服务占用过多资源

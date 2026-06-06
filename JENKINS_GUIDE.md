# Jenkins 部署指南

## 前置条件

1. 安装 Jenkins 服务器
2. Jenkins 安装 Docker 和 Docker Compose 插件
3. **Jenkins 节点上必须安装 Docker 和 Docker Compose**
   - 如果 Jenkins 运行在 Docker 容器中，需要挂载 docker.sock
4. 配置 Git 凭证（如果是私有仓库）

## Pipeline 功能说明

### 参数配置

- **DEPLOY_ENV**: 部署环境（prod/staging）
- **FORCE_BUILD**: 是否强制重新构建镜像（默认 true）
- **RUN_TESTS**: 是否运行单元测试（默认 false）

### 重要说明

Pipeline 会直接在 Jenkins 的工作目录中运行，不需要指定额外的项目目录。这样即使 Jenkins 运行在 Docker 容器中也能正常工作。

### 流程说明

1. **Check Environment**: 检查 Docker 环境
2. **Checkout**: 从 Git 仓库拉取代码
3. **Run Tests**: 运行单元测试（可选）
4. **Build Backend**: 构建后端 Docker 镜像
5. **Build Frontend**: 构建前端 Docker 镜像
6. **Stop Services**: 停止旧服务
7. **Deploy**: 部署新服务
8. **Verify Deployment**: 验证部署

## Jenkins 配置步骤

### 1. 创建 Jenkins 任务

1. 登录 Jenkins，点击 "New Item"
2. 输入任务名称，选择 "Pipeline"，点击 OK

### 2. 配置 Pipeline

**重要：必须选择 "Pipeline script from SCM"**

在 Pipeline 配置页面：

1. **Pipeline script from SCM**
   - 选择 "Git"
   - Repository URL: `https://github.com/gaofq/sid.db.git`
   - Credentials: 选择 Git 凭证（如果需要）
   - Branch: `*/main`
   - Script Path: `Jenkinsfile`

2. 保存配置

### 3. 首次运行

1. 点击 "Build with Parameters"
2. 选择参数，点击 "Build"

## 常见问题

### 1. Docker 未安装错误

**错误信息**：`docker: not found`

**解决方法**：
在 Jenkins 节点上安装 Docker：

```bash
# Ubuntu/Debian
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# 将 jenkins 用户加入 docker 组
sudo usermod -aG docker jenkins
sudo systemctl restart jenkins
```

### 2. checkout scm 错误

**错误信息**：`‘checkout scm’ is only available when using “Multibranch Pipeline” or “Pipeline script from SCM”`

**解决方法**：
在 Jenkins 任务配置中，必须选择 **"Pipeline script from SCM"**，而不是 "Pipeline script"。

### 3. Docker 权限问题

如果遇到权限问题，将 jenkins 用户加入 docker 组：

```bash
sudo usermod -aG docker jenkins
sudo systemctl restart jenkins
```

### 4. Docker Compose 版本问题

确保 Docker Compose 版本兼容，可以使用 `docker compose` 或 `docker-compose`

### 5. 端口冲突

修改 `docker-compose.yml` 中的端口映射

### 6. 项目目录权限问题

首先查看运行 Jenkins 的用户：

```bash
# 查看 Jenkins 进程的用户
ps aux | grep jenkins

# 或者查看 Jenkins 安装用户
cat /etc/passwd | grep -i jenkins

# 如果不确定的话，直接给所有用户读写权限（临时方案）
sudo chmod -R 777 /www/wwwroot/gaofq/sid.db

# 或者给 www 用户权限（如果用的是宝塔面板）
sudo chown -R www:www /www/wwwroot/gaofq/sid.db
sudo chmod -R 755 /www/wwwroot/gaofq/sid.db
```

### 7. docker-compose.yml 文件不存在

确保项目目录中有 `docker-compose.yml` 文件。Pipeline 会在 Checkout 阶段拉取代码到项目目录。

### 8. 部署失败排查

如果部署失败，请查看 Jenkins 日志中的详细输出：
- 检查目录结构
- 检查 Docker 版本
- 检查 docker-compose.yml 是否存在
- 查看 Docker 容器状态和日志

## 进阶配置

### 添加钉钉通知

修改 Jenkinsfile，在 `post` 块中添加：

```groovy
post {
    success {
        echo '✅ 部署成功！'
        // 钉钉机器人通知
        sh '''
        curl -X POST "https://oapi.dingtalk.com/robot/send?access_token=YOUR_TOKEN" \\
        -H "Content-Type: application/json" \\
        -d '{"msgtype": "text", "text": {"content": "部署成功！"}}'
        '''
    }
}
```

### 多环境部署

修改 `docker-compose.yml` 支持多环境，或创建多个 compose 文件。


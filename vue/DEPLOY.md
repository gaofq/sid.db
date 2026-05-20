# 前端应用 Docker 部署文档

## 一、环境要求
- Docker Engine 20.10+
- 可选：Docker Compose 2.0+（整体部署推荐）
- 后端服务：已经部署好的后端API服务

## 二、独立部署步骤
### 1. 构建镜像
在 `vue` 目录下执行：
```bash
docker build -t sid-db-field-manager-frontend .
```

### 2. 运行容器
```bash
docker run -d \
  --name sid-db-frontend \
  -p 8091:80 \
  --link sid-db-backend:backend \
  --restart always \
  sid-db-field-manager-frontend
```

#### 自定义后端地址部署：
如果后端服务不是和前端在同一个Docker网络，或者后端地址有变化，可以修改Nginx配置后运行：
```bash
docker run -d \
  --name sid-db-frontend \
  -p 8091:80 \
  -v /your/path/nginx.conf:/etc/nginx/conf.d/default.conf \
  --restart always \
  sid-db-field-manager-frontend
```

## 三、验证部署
访问：`http://localhost:8091`，可以看到登录页面表示部署成功。

## 四、常用操作
### 查看日志
```bash
docker logs -f sid-db-frontend
```

### 停止容器
```bash
docker stop sid-db-frontend
```

### 启动容器
```bash
docker start sid-db-frontend
```

### 删除容器
```bash
docker rm -f sid-db-frontend
```

### 进入容器内部
```bash
docker exec -it sid-db-frontend /bin/sh
```

## 五、配置说明
### 1. Nginx 配置说明
默认的`nginx.conf`配置做了两个核心功能：
- **SPA路由支持**：所有页面请求重定向到`index.html`，解决Vue Router history模式刷新404问题
- **API反向代理**：`/api`开头的请求自动转发到后端服务`http://backend:8090`

#### 自定义Nginx配置示例：
如果需要修改后端地址，修改`nginx.conf`中的代理配置：
```nginx
location /api/ {
    proxy_pass http://your-backend-domain-or-ip:port/;
    proxy_set_header Host $host;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
}
```

### 2. 自定义前端配置
如果需要修改前端的环境配置（比如API地址、标题等），可以在构建镜像前修改`.env.production`文件，或者挂载配置文件：
```bash
-v /your/path/config.js:/usr/share/nginx/html/config.js
```

### 3. HTTPS部署
如果需要配置HTTPS，修改Nginx配置添加SSL证书：
```nginx
server {
    listen 443 ssl;
    server_name your-domain.com;
    ssl_certificate /etc/nginx/cert/cert.pem;
    ssl_certificate_key /etc/nginx/cert/cert.key;
    
    # 其他配置...
}
```
运行时挂载证书目录：
```bash
-v /your/cert/path:/etc/nginx/cert
-p 443:443
```

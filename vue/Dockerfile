# 使用Node.js 22 Alpine镜像作为构建环境（轻量级，体积小）
FROM node:22-alpine AS build
# 设置工作目录为/app
WORKDIR /app

# 复制package.json和package-lock.json（分层缓存，只有依赖变化时才会重新安装）
COPY package.json package-lock.json ./
# 安装依赖，ci命令严格按照lock文件版本安装，确保环境一致性
RUN npm ci

# 复制所有前端源代码
COPY . .
# 构建生产版本，产物自动输出到dist目录
RUN npm run build

# 运行阶段，使用Nginx Alpine轻量级镜像作为web服务器
FROM nginx:alpine AS runtime
# 复制自定义Nginx配置，处理SPA路由回退和后端API反向代理
COPY nginx.conf /etc/nginx/conf.d/default.conf
# 从构建阶段复制编译好的前端静态文件到Nginx静态资源目录
COPY --from=build /app/dist /usr/share/nginx/html

# 暴露80端口供外部访问
EXPOSE 80
# 启动Nginx，前台运行（容器必须保持一个前台进程运行）
CMD ["nginx", "-g", "daemon off;"]

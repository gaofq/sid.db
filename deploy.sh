#!/bin/bash

# Linux Docker 一键部署脚本
# 使用方式：
#   ./deploy.sh up [--build] [-d]    # 启动服务
#   ./deploy.sh down                 # 停止并删除服务
#   ./deploy.sh restart              # 重启服务
#   ./deploy.sh logs [backend/frontend] [-f]  # 查看日志
#   ./deploy.sh build                # 构建镜像
#   ./deploy.sh status               # 查看运行状态
#   ./deploy.sh clean                # 清理镜像和容器

# 检查docker compose命令
if command -v docker compose &> /dev/null; then
    DOCKER_COMPOSE="docker compose"
elif command -v docker-compose &> /dev/null; then
    DOCKER_COMPOSE="docker-compose"
else
    echo "❌ 错误：未找到docker compose或docker-compose命令，请先安装Docker Compose"
    exit 1
fi

# 处理命令
CMD="$1"
shift

case "$CMD" in
    "up")
        echo "🚀 启动服务..."
        $DOCKER_COMPOSE up $@
        ;;
    "down")
        echo "🛑 停止服务..."
        $DOCKER_COMPOSE down $@
        ;;
    "restart")
        echo "🔄 重启服务..."
        $DOCKER_COMPOSE restart $@
        ;;
    "logs")
        echo "📋 查看日志..."
        $DOCKER_COMPOSE logs $@
        ;;
    "build")
        echo "🏗️  构建镜像..."
        $DOCKER_COMPOSE build $@
        ;;
    "status"|"ps")
        echo "📊 查看服务状态..."
        $DOCKER_COMPOSE ps
        ;;
    "clean")
        echo "🧹 清理资源..."
        $DOCKER_COMPOSE down -v --rmi all
        docker system prune -f
        echo "✅ 清理完成"
        ;;
    *)
        echo "使用方式："
        echo "  ./deploy.sh up [--build] [-d]    # 启动服务，--build强制构建，-d后台运行"
        echo "  ./deploy.sh down                 # 停止并删除服务"
        echo "  ./deploy.sh restart              # 重启服务"
        echo "  ./deploy.sh logs [服务名] [-f]   # 查看日志，-f实时跟踪"
        echo "  ./deploy.sh build                # 构建镜像"
        echo "  ./deploy.sh status               # 查看运行状态"
        echo "  ./deploy.sh clean                # 清理镜像和容器"
        exit 1
        ;;
esac

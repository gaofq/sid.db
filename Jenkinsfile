pipeline {
    agent any
    
    // Git 仓库地址：https://github.com/gaofq/sid.db.git
    // 请在 Jenkins 任务配置中设置此仓库地址为 "Pipeline script from SCM"
    
    parameters {
        choice(
            name: 'DEPLOY_ENV',
            choices: ['prod', 'staging'],
            description: '部署环境'
        )
        booleanParam(
            name: 'FORCE_BUILD',
            defaultValue: true,
            description: '是否强制重新构建镜像'
        )
        booleanParam(
            name: 'RUN_TESTS',
            defaultValue: false,
            description: '是否运行单元测试'
        )
    }
    
    environment {
        DOCKER_REGISTRY = ''  // 如果使用私有镜像仓库，填写仓库地址
        PROJECT_NAME = 'dbfm'
        GIT_BRANCH = 'main'
        GIT_REPO_URL = 'https://github.com/gaofq/sid.db.git'
    }
    
    stages {
        stage('Configure Git') {
            steps {
                echo '🔧 配置 Git...'
                script {
                    // 配置 Git 安全目录
                    sh 'git config --global --add safe.directory "*"'
                    sh 'git config --global user.name "Jenkins"'
                    sh 'git config --global user.email "jenkins@example.com"'
                }
            }
        }
        
        stage('Check Environment') {
            steps {
                echo '🔍 检查环境...'
                script {
                    echo "当前工作目录: ${pwd()}"
                    sh 'ls -la'
                    
                    // 检查 Docker 是否可用
                    echo '检查 Docker...'
                    def dockerExists = sh(script: 'which docker', returnStatus: true) == 0
                    echo "Docker 可用: ${dockerExists}"
                    
                    if (dockerExists) {
                        sh 'docker --version'
                    }
                    
                    echo '检查 Docker Compose...'
                    def dockerComposeExists = sh(script: 'which docker-compose', returnStatus: true) == 0
                    def dockerComposeV2Exists = sh(script: 'docker compose version', returnStatus: true) == 0
                    echo "Docker Compose (v1) 可用: ${dockerComposeExists}"
                    echo "Docker Compose (v2) 可用: ${dockerComposeV2Exists}"
                    
                    if (!dockerExists) {
                        error '❌ Docker 未安装，请在 Jenkins 节点上安装 Docker'
                    }
                }
            }
        }
        
        stage('Checkout') {
            steps {
                echo '🚀 拉取代码...'
                script {
                    // 先清理目录
                    sh 'rm -rf * .git 2>/dev/null || true'
                    // 直接克隆仓库
                    sh "git clone ${env.GIT_REPO_URL} ."
                    sh "git checkout ${env.GIT_BRANCH}"
                    sh 'ls -la'
                }
            }
        }
        
        stage('Run Tests') {
            when {
                expression { params.RUN_TESTS == true }
            }
            steps {
                echo '🧪 运行单元测试...'
                dir('aspnet-core') {
                    sh 'dotnet test'
                }
            }
        }
        
        stage('Build Backend') {
            steps {
                echo '🏗️  构建后端镜像...'
                script {
                    def buildCmd = params.FORCE_BUILD ? 'docker compose build --no-cache backend' : 'docker compose build backend'
                    echo "执行命令: ${buildCmd}"
                    sh buildCmd
                }
            }
        }
        
        stage('Build Frontend') {
            steps {
                echo '🏗️  构建前端镜像...'
                script {
                    def buildCmd = params.FORCE_BUILD ? 'docker compose build --no-cache frontend' : 'docker compose build frontend'
                    echo "执行命令: ${buildCmd}"
                    sh buildCmd
                }
            }
        }
        
        stage('Stop Services') {
            steps {
                echo '🛑 停止旧服务...'
                script {
                    try {
                        sh 'docker compose down'
                    } catch (Exception e) {
                        echo "停止服务时出错（可能服务未运行）: ${e.getMessage()}"
                    }
                }
            }
        }
        
        stage('Deploy') {
            steps {
                echo '🚀 部署新服务...'
                sh 'docker compose up -d'
            }
        }
        
        stage('Verify Deployment') {
            steps {
                echo '🔍 验证部署...'
                sh 'docker compose ps'
                echo '⏳ 等待服务启动...'
                sleep 30
                echo '📋 查看服务日志...'
                sh 'docker compose logs --tail=50'
            }
        }
    }
    
    post {
        success {
            echo '✅ 部署成功！'
            sh 'docker compose ps'
        }
        failure {
            echo '❌ 部署失败！'
            script {
                echo '📁 当前目录内容:'
                sh 'ls -la'
                
                try {
                    echo '🐳 Docker 状态:'
                    sh 'docker ps -a'
                    
                    echo '📝 服务日志:'
                    sh 'docker compose logs --tail=100 2>/dev/null || docker logs --tail=100 $(docker ps -aq) 2>/dev/null || true'
                } catch (Exception e) {
                    echo "获取日志时出错: ${e.getMessage()}"
                }
            }
        }
        always {
            echo '🎉 Pipeline 执行完成'
        }
    }
}

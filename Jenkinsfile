pipeline {
    agent any
    
    // Git 仓库：https://github.com/gaofq/sid.db.git
    // 服务器源码目录：/www/wwwroot/gaofq/sid.db/
    
    options {
        skipDefaultCheckout()  // 跳过 SCM 自动 checkout，我们自己控制
    }
    
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
        GIT_BRANCH = 'main'
        GIT_REPO_URL = 'https://github.com/gaofq/sid.db.git'
    }
    
    stages {
        stage('Configure Git') {
            steps {
                echo '🔧 配置 Git 安全目录...'
                sh 'git config --global --add safe.directory "*"'
            }
        }
        
        stage('Checkout') {
            steps {
                echo '� 拉取代码...'
                sh "git clone --branch ${env.GIT_BRANCH} ${env.GIT_REPO_URL} ."
                sh 'ls -la'
            }
        }
        
        stage('Check Environment') {
            steps {
                echo '🔍 检查环境...'
                script {
                    sh 'docker --version'
                    sh 'docker compose version || docker-compose --version || true'
                }
            }
        }
        
        stage('Run Tests') {
            when { expression { params.RUN_TESTS == true } }
            steps {
                echo '🧪 运行单元测试...'
                dir('aspnet-core') {
                    sh 'dotnet test || true'
                }
            }
        }
        
        stage('Build Backend') {
            steps {
                echo '🏗️  构建后端镜像...'
                script {
                    def cmd = params.FORCE_BUILD ? 'docker compose build --no-cache backend' : 'docker compose build backend'
                    sh cmd
                }
            }
        }
        
        stage('Build Frontend') {
            steps {
                echo '🏗️  构建前端镜像...'
                script {
                    def cmd = params.FORCE_BUILD ? 'docker compose build --no-cache frontend' : 'docker compose build frontend'
                    sh cmd
                }
            }
        }
        
        stage('Stop Services') {
            steps {
                echo '🛑 停止旧服务...'
                sh 'docker compose down || true'
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
                sh 'ls -la'
                try {
                    sh 'docker ps -a'
                    sh 'docker compose logs --tail=100 2>/dev/null || true'
                } catch (Exception e) {
                    echo "获取日志失败: ${e.getMessage()}"
                }
            }
        }
        always {
            echo '🎉 Pipeline 执行完成'
        }
    }
}

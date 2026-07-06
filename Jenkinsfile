pipeline {
    agent any
    
    // 源码目录已挂载：/www/wwwroot/gaofq/sid.db -> /var/jenkins_home/workspace/sid.db
    
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
    }
    
    stages {
        stage('Check Environment') {
            steps {
                echo '🔍 检查环境...'
                sh 'docker --version'
                sh 'docker compose version || true'
                sh 'ls -la'
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
            sh 'docker compose logs --tail=100 2>/dev/null || true'
        }
    }
}

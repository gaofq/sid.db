# Sid.DbFieldManager - 数据库字段管理系统

一个基于 ABP Framework + Vue 3 的 SQL Server 数据库字段管理系统，取代传统的 Excel 管理方式。

## 功能特性

- **多数据库管理**：管理多个目标 SQL Server 数据库连接
- **表字段定义**：可视化添加/编辑/删除数据库表字段
- **SQL自动生成**：自动生成 `ALTER TABLE` + `sp_addextendedproperty` 语句
- **一键执行**：选择字段后自动连接到目标数据库执行 SQL
- **完整审计**：所有操作自动记录创建人、创建时间、执行日志
- **Excel导入**：支持从 Excel 批量导入字段定义

## 技术栈

| 层级 | 技术 | 版本 |
|------|------|------|
| 后端框架 | ABP Framework Community (LGPL) | 10.3.0 |
| 前端框架 | Vue 3 + Vite + Ant Design Vue | 3.x / 6.x / 4.x |
| 数据库 | SQL Server (EF Core) | - |
| .NET | .NET 10.0 | - |

## 项目结构

```
Sid.DbFieldManager/
├── aspnet-core/                         # ABP 后端
│   └── src/
│       ├── Sid.DbFieldManager.Domain/             # 领域实体
│       ├── Sid.DbFieldManager.Domain.Shared/      # 共享类型
│       ├── Sid.DbFieldManager.Application.Contracts/  # DTOs, 服务接口
│       ├── Sid.DbFieldManager.Application/        # 业务逻辑实现
│       ├── Sid.DbFieldManager.EntityFrameworkCore/ # EF Core 配置
│       ├── Sid.DbFieldManager.HttpApi/            # API 控制器
│       ├── Sid.DbFieldManager.HttpApi.Host/       # 启动项目
│       └── Sid.DbFieldManager.DbMigrator/         # 数据库迁移
├── vue/                                 # Vue 3 前端
│   └── src/
│       ├── api/                         # API 调用层
│       ├── views/
│       │   ├── databases/               # 目标数据库管理
│       │   ├── tables/                  # 表管理
│       │   ├── fields/                  # 字段管理 (核心工作区)
│       │   └── execution-logs/          # 执行日志
│       └── router/                      # 路由配置
└── README.md
```

## 快速开始

### 前置条件

- .NET 10.0 SDK
- Node.js 22+
- SQL Server (LocalDB 或完整版)

### 后端启动

```bash
# 1. 修改连接字符串
编辑 aspnet-core/src/Sid.DbFieldManager.HttpApi.Host/appsettings.json
修改 ConnectionStrings.Default 为你的 SQL Server 连接

# 2. 运行数据库迁移
cd aspnet-core/src/Sid.DbFieldManager.DbMigrator
dotnet run

# 3. 启动后端 API
cd aspnet-core/src/Sid.DbFieldManager.HttpApi.Host
dotnet run
# API 启动于 https://localhost:44354
```

### 前端启动

```bash
cd vue
npm install
npm run dev
# 前端启动于 http://localhost:5173
```

## 使用流程

1. **添加目标数据库**：配置要管理的 SQL Server 连接
2. **添加数据表**：注册需要管理字段的表（如 mporder）
3. **添加字段**：定义字段名、类型、是否可空、描述等
4. **预览SQL**：选中字段，点击"预览SQL"查看生成的脚本
5. **执行SQL**：点击"执行SQL"直接在目标数据库运行

## 生成的SQL示例

```sql
-- ==========================================
-- Table: [dbo].[mporder]
-- Description: 客户订单表
-- Generated: 2026-05-12 17:00:00
-- ==========================================

ALTER TABLE [dbo].[mporder] ADD [mporder_IsSameDayInWarehouse] bit NOT NULL DEFAULT 0
EXEC sp_addextendedproperty N'MS_Description', N'是否当日入仓', N'user', N'dbo', N'table', N'mporder', N'column', N'mporder_IsSameDayInWarehouse'
GO

ALTER TABLE [dbo].[mporder] ADD [mporder_LogisticsNumber] nvarchar(100) NULL
EXEC sp_addextendedproperty N'MS_Description', N'物流单号', N'user', N'dbo', N'table', N'mporder', N'column', N'mporder_LogisticsNumber'
GO
```

## 许可证

MIT License - 开源免费使用

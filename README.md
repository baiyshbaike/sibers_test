# Sibers Project Manager / Управление проектами Сиберс

## English

A comprehensive project management system built with ASP.NET Core Web API backend and Vue.js frontend. This application implements a 3-tier architecture with Entity Framework Core, role-based authentication, and full CRUD operations for projects, employees, and tasks.

### Features / Особенности

- **3-Tier Architecture**: DAL (Data Access Layer), BLL (Business Logic Layer), API (Presentation Layer)
- **Authentication & Authorization**: ASP.NET Core Identity with JWT tokens and role-based access control
- **Project Management**: Create, read, update, delete projects with filtering and sorting
- **Employee Management**: Manage employees with role assignments (Supervisor, ProjectManager, Employee)
- **Task Management**: Track tasks with status, priority, and assignment
- **File Management**: Upload and download project documents with drag & drop support
- **Advanced Filtering**: Filter projects by date ranges, priority, and team members
- **Responsive UI**: Modern Vue.js frontend with TailwindCSS styling
- **Docker Support**: Complete containerization with docker-compose

### Technology Stack / Технологический стек

**Backend:**
- .NET 10.0
- ASP.NET Core Web API
- Entity Framework Core with SQL Server
- ASP.NET Core Identity
- JWT Authentication
- AutoMapper
- xUnit, Moq, FluentAssertions (Testing)

**Frontend:**
- Vue 3 with TypeScript
- Pinia (State Management)
- Vue Router
- Axios (HTTP Client)
- TailwindCSS (Styling)
- Vite (Build Tool)

**Database:**
- Microsoft SQL Server
- Code-first migrations with Entity Framework Core

### Roles / Роли

1. **Supervisor**: Full access to all resources
2. **ProjectManager**: Can manage projects and assign tasks
3. **Employee**: Can view assigned projects and update task status

### Getting Started / Начало работы

#### Prerequisites / Предварительные требования

- .NET 10.0 SDK
- Node.js 18+ and npm
- SQL Server or Docker
- Git

#### Installation / Установка

1. **Clone the repository / Клонирование репозитория:**
   ```bash
   git clone <repository-url>
   cd sibers_task
   ```

2. **Environment Configuration / Конфигурация окружения:**
   ```bash
   cp .env.example .env
   # Edit .env with your database and JWT settings
   ```

#### Running the Application / Запуск приложения

**Docker (Recommended) / Docker (Рекомендуется):**
```bash
docker-compose up --build
```

**Manual / Вручную:**

1. **Start Backend / Запуск бэкенда:**
   ```bash
   cd SibersProject.API
   dotnet run
   ```

2. **Start Frontend / Запуск фронтенда:**
   ```bash
   cd sibers-frontend
   npm run dev
   ```

#### Access Points / Точки доступа

- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5050
- **Swagger Documentation**: http://localhost:5050 (root path)

### Default Users / Пользователи по умолчанию

The application seeds default users on first run:

| Role | Email | Password |
|------|-------|----------|
| Supervisor | supervisor@sibers.local | Admin123! |
| Project Manager | manager1@sibers.local | Manager123! |
| Employee | employee1@sibers.local | Employee123! |

### API Endpoints / Конечные точки API

#### Authentication / Аутентификация
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login

#### Projects / Проекты
- `GET /api/projects` - Get projects with filtering
- `GET /api/projects/{id}` - Get project details
- `POST /api/projects` - Create new project
- `PUT /api/projects/{id}` - Update project
- `DELETE /api/projects/{id}` - Delete project
- `POST /api/projects/{id}/employees/{employeeId}` - Add employee to project
- `DELETE /api/projects/{id}/employees/{employeeId}` - Remove employee from project
- `POST /api/projects/{id}/documents` - Upload documents
- `GET /api/projects/{id}/documents/{documentId}/download` - Download document
- `DELETE /api/projects/{id}/documents/{documentId}` - Delete document

#### Employees / Сотрудники
- `GET /api/employees` - Get employees with filtering
- `GET /api/employees/{id}` - Get employee details
- `POST /api/employees` - Create new employee
- `PUT /api/employees/{id}` - Update employee
- `DELETE /api/employees/{id}` - Delete employee
- `GET /api/employees/search` - Search employees

#### Tasks / Задачи
- `GET /api/tasks` - Get tasks with filtering
- `GET /api/tasks/{id}` - Get task details
- `POST /api/tasks` - Create new task
- `PUT /api/tasks/{id}` - Update task
- `DELETE /api/tasks/{id}` - Delete task

### Testing / Тестирование

Run unit tests:
```bash
cd SibersProject.Tests
dotnet test
```

**Note / Примечание:**
- Docker-compose automatically handles all build operations, database migrations, and dependency management
- Docker-compose автоматически выполняет все операции сборки, миграции базы данных и управление зависимостями

### Project Structure / Структура проекта

```
sibers_task/
├── SibersProject.DAL/          # Data Access Layer
│   ├── Entities/               # Database entities
│   ├── Repositories/           # Repository pattern implementation
│   ├── Filters/               # Query filters
│   └── Data/                 # DbContext and configurations
├── SibersProject.BLL/          # Business Logic Layer
│   ├── Services/               # Business services
│   ├── DTOs/                 # Data transfer objects
│   └── Mappings/             # AutoMapper profiles
├── SibersProject.API/         # Presentation Layer
│   ├── Controllers/            # API controllers
│   └── Extensions/           # Service extensions
├── SibersProject.Tests/       # Unit tests
├── sibers-frontend/           # Vue.js frontend
│   ├── src/
│   │   ├── components/        # Vue components
│   │   ├── views/            # Page views
│   │   ├── stores/           # Pinia stores
│   │   └── types/           # TypeScript types
│   └── public/              # Static assets
├── docker-compose.yml         # Docker composition
└── README.md                # This file
```

### Contributing / Вклад в проект

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

### License / Лицензия

This project is licensed under the MIT License.

---

## Русский

Комплексная система управления проектами, построенная на ASP.NET Core Web API бэкенде и Vue.js фронтенде. Приложение реализует 3-уровневую архитектуру с Entity Framework Core, аутентификацией на основе ролей и полными CRUD операциями для проектов, сотрудников и задач.

### Особенности

- **3-уровневая архитектура**: DAL (уровень доступа к данным), BLL (бизнес-логика), API (представление)
- **Аутентификация и авторизация**: ASP.NET Core Identity с JWT-токенами и контролем доступа на основе ролей
- **Управление проектами**: Создание, чтение, обновление, удаление проектов с фильтрацией и сортировкой
- **Управление сотрудниками**: Управление сотрудниками с назначением ролей (Supervisor, ProjectManager, Employee)
- **Управление задачами**: Отслеживание задач со статусом, приоритетом и назначением
- **Управление файлами**: Загрузка и скачивание документов проекта с поддержкой drag & drop
- **Расширенная фильтрация**: Фильтрация проектов по диапазонам дат, приоритету и членам команды
- **Адаптивный интерфейс**: Современный Vue.js фронтенд со стилизацией TailwindCSS
- **Поддержка Docker**: Полная контейнеризация с docker-compose

### Технологический стек

**Бэкенд:**
- .NET 10.0
- ASP.NET Core Web API
- Entity Framework Core с SQL Server
- ASP.NET Core Identity
- JWT-аутентификация
- AutoMapper
- xUnit, Moq, FluentAssertions (Тестирование)

**Фронтенд:**
- Vue 3 с TypeScript
- Pinia (Управление состоянием)
- Vue Router
- Axios (HTTP-клиент)
- TailwindCSS (Стилизация)
- Vite (Сборщик)

**База данных:**
- Microsoft SQL Server
- Code-first миграции с Entity Framework Core

### Роли

1. **Supervisor**: Полный доступ ко всем ресурсам
2. **ProjectManager**: Может управлять проектами и назначать задачи
3. **Employee**: Может просматривать назначенные проекты и обновлять статус задач

### Начало работы

#### Предварительные требования

- .NET 10.0 SDK
- Node.js 18+ и npm
- SQL Server или Docker
- Git

#### Установка

1. **Клонирование репозитория:**
   ```bash
   git clone <repository-url>
   cd sibers_task
   ```

2. **Настройка бэкенда:**
   ```bash
   cd SibersProject.API
   dotnet restore
   ```

3. **Конфигурация окружения:**
   ```bash
   cp .env.example .env
   # Отредактируйте .env с настройками вашей базы данных и JWT
   ```

4. **Миграция базы данных:**
   ```bash
   cd SibersProject.API
   dotnet ef database update --project ../SibersProject.DAL
   ```

5. **Настройка фронтенда:**
   ```bash
   cd sibers-frontend
   npm install
   ```

#### Запуск приложения

**Вариант 1: Docker (Рекомендуется):**
```bash
docker-compose up --build
```

**Вариант 2: Вручную:**

1. **Запуск бэкенда:**
   ```bash
   cd SibersProject.API
   dotnet run
   ```

2. **Запуск фронтенда:**
   ```bash
   cd sibers-frontend
   npm run dev
   ```

#### Точки доступа

- **Фронтенд**: http://localhost:3000
- **Бэкенд API**: http://localhost:5050
- **Документация Swagger**: http://localhost:5050 (корневой путь)

### Пользователи по умолчанию

Приложение создает пользователей по умолчанию при первом запуске:

| Роль | Email | Пароль |
|------|-------|----------|
| Supervisor | supervisor@sibers.local | Admin123! |
| Project Manager | manager1@sibers.local | Manager123! |
| Employee | employee1@sibers.local | Employee123! |

### Конечные точки API

#### Аутентификация
- `POST /api/auth/register` - Регистрация нового пользователя
- `POST /api/auth/login` - Вход пользователя

#### Проекты
- `GET /api/projects` - Получение проектов с фильтрацией
- `GET /api/projects/{id}` - Получение деталей проекта
- `POST /api/projects` - Создание нового проекта
- `PUT /api/projects/{id}` - Обновление проекта
- `DELETE /api/projects/{id}` - Удаление проекта
- `POST /api/projects/{id}/employees/{employeeId}` - Добавление сотрудника к проекту
- `DELETE /api/projects/{id}/employees/{employeeId}` - Удаление сотрудника из проекта
- `POST /api/projects/{id}/documents` - Загрузка документов
- `GET /api/projects/{id}/documents/{documentId}/download` - Скачивание документа
- `DELETE /api/projects/{id}/documents/{documentId}` - Удаление документа

#### Сотрудники
- `GET /api/employees` - Получение сотрудников с фильтрацией
- `GET /api/employees/{id}` - Получение деталей сотрудника
- `POST /api/employees` - Создание нового сотрудника
- `PUT /api/employees/{id}` - Обновление сотрудника
- `DELETE /api/employees/{id}` - Удаление сотрудника
- `GET /api/employees/search` - Поиск сотрудников

#### Задачи
- `GET /api/tasks` - Получение задач с фильтрацией
- `GET /api/tasks/{id}` - Получение деталей задачи
- `POST /api/tasks` - Создание новой задачи
- `PUT /api/tasks/{id}` - Обновление задачи
- `DELETE /api/tasks/{id}` - Удаление задачи

### Тестирование

Запуск модульных тестов:
```bash
cd SibersProject.Tests
dotnet test
```

### Структура проекта

```
sibers_task/
├── SibersProject.DAL/          # Уровень доступа к данным
│   ├── Entities/               # Сущности базы данных
│   ├── Repositories/           # Реализация паттерна репозитория
│   ├── Filters/               # Фильтры запросов
│   └── Data/                 # DbContext и конфигурации
├── SibersProject.BLL/          # Уровень бизнес-логики
│   ├── Services/               # Бизнес-сервисы
│   ├── DTOs/                 # Объекты передачи данных
│   └── Mappings/             # Профили AutoMapper
├── SibersProject.API/         # Уровень представления
│   ├── Controllers/            # API-контроллеры
│   └── Extensions/           # Расширения сервисов
├── SibersProject.Tests/       # Модульные тесты
├── sibers-frontend/           # Vue.js фронтенд
│   ├── src/
│   │   ├── components/        # Vue-компоненты
│   │   ├── views/            # Страницы
│   │   ├── stores/           # Хранилища Pinia
│   │   └── types/           # Типы TypeScript
│   └── public/              # Статические ресурсы
├── docker-compose.yml         # Docker-композиция
└── README.md                # Этот файл
```

### Вклад в проект

1. Сделайте форк репозитория
2. Создайте ветку функций
3. Внесите изменения
4. Добавьте тесты для новой функциональности
5. Отправьте pull request

### Лицензия

Этот проект лицензирован под MIT License.

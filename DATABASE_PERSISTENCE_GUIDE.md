# Логика сохранения данных в локальную БД (SQLite)

## Обзор

Приложение "Admissions Reserve" использует локальную базу данных SQLite для сохранения информации об абитуриентах. Данная документация описывает архитектуру и использование системы сохранения данных.

## Архитектура

### 1. Основные классы

#### DatabaseHelper
Базовый класс для работы с подключением к SQLite БД.
- Находится в: `Model/DatabaseHelper.cs`
- Предоставляет методы для получения подключения к БД

#### DataService
Основной сервис для работы с данными.
- Находится в: `Model/DataService.cs`
- Содержит методы CRUD для всех основных сущностей (Applicants, EducationDocuments, и т.д.)
- Методы:
  - `CreateApplicant()`, `UpdateApplicant()`, `GetApplicant()` - работа с абитуриентами
  - `CreateEducationDocument()`, `UpdateEducationDocument()`, `GetEducationDocument()` - работа с документами об образовании
  - `LogChange()` - логирование всех изменений в таблицу `ChangeLogs`

#### DatabasePersistenceHelper (НОВЫЙ)
Новый вспомогательный класс для сохранения данных на страницах.
- Находится в: `Model/DatabasePersistenceHelper.cs`
- Предоставляет методы для работы с:
  - Относительными документами (RelativeDocuments)
  - Индивидуальными достижениями (IndividualAchievements)
  - Приложенными документами (AttachedDocuments)

### 2. Модель данных

Основные сущности в БД:
- **Applicants** - абитуриенты
- **IdentityDocuments** - документы удостоверения личности
- **EducationDocuments** - документы об образовании
- **RelativeDocuments** - данные о родственниках
- **IndividualAchievements** - индивидуальные достижения
- **AttachedDocuments** - приложенные документы
- **ChangeLogs** - журнал изменений

## Реализованная логика по страницам

### 1. IdentityPage (Страница удостоверения личности)
**Статус**: ✅ Уже имеет сохранение
- Сохраняет основные данные абитуриента в таблицу `Applicants`
- При загрузке страницы создается новый абитуриент в БД (если это новая запись)
- Логирует изменения через `DataService.LogChange()`

```csharp
currentApplicant.Id = DataService.CreateApplicant(currentApplicant);
```

### 2. ContactsPage (Страница контактов)
**Статус**: ✅ Обновлена с логированием
- Сохраняет контактную информацию абитуриента
- Использует `DataService.UpdateApplicant()`
- Логирует все изменения

```csharp
DataService.UpdateApplicant(currentApplicant);
DataService.LogChange("Applicants", currentApplicant.Id, "UPDATE");
MessageBox.Show("Контактные данные успешно сохранены!", "Успех", ...);
```

### 3. ApplicationTypeAndEducationPage (Страница образования)
**Статус**: ✅ Уже имеет сохранение
- Сохраняет документы об образовании в таблицу `EducationDocuments`
- Использует методы:
  - `DataService.CreateEducationDocument()` - для новых записей
  - `DataService.UpdateEducationDocument()` - для обновления
- Логирует изменения через `DataService.LogChange()`

```csharp
if (currentEducationDocument.Id == 0)
{
    currentEducationDocument.Id = DataService.CreateEducationDocument(currentEducationDocument);
    DataService.LogChange("EducationDocuments", currentEducationDocument.Id, "INSERT");
}
else
{
    DataService.UpdateEducationDocument(currentEducationDocument);
    DataService.LogChange("EducationDocuments", currentEducationDocument.Id, "UPDATE");
}
```

### 4. RelativesPage (Страница родственников)
**Статус**: ✅ Обновлена с использованием DatabasePersistenceHelper
- Сохраняет данные о родственниках в таблицу `RelativeDocuments`
- При добавлении:
```csharp
int relativeId = DatabasePersistenceHelper.SaveRelativeDocument(
    SessionManager.CurrentApplicantId.Value,
    relationDegree,
    lastName,
    firstName,
    patronymic,
    birthDate,
    phone,
    email,
    workPlace,
    position,
    blockReason,
    isBlocked
);
DataService.LogChange("RelativeDocuments", relativeId, "INSERT");
```

- При удалении:
```csharp
DatabasePersistenceHelper.DeleteRelativeDocument(item.Id, SessionManager.CurrentApplicantId.Value);
DataService.LogChange("RelativeDocuments", item.Id, "DELETE");
```

- При загрузке страницы загружаются существующие данные из БД

### 5. IndividualAchievementsPage (Страница индивидуальных достижений)
**Статус**: ✅ Обновлена с использованием DatabasePersistenceHelper
- Сохраняет достижения в таблицу `IndividualAchievements`
- При добавлении:
```csharp
int achievementId = DatabasePersistenceHelper.SaveIndividualAchievement(
    SessionManager.CurrentApplicantId.Value,
    category,
    achievementName,
    year,
    points,
    documentName,
    documentPath
);
DataService.LogChange("IndividualAchievements", achievementId, "INSERT");
```

- При удалении:
```csharp
DatabasePersistenceHelper.DeleteIndividualAchievement(item.Id, SessionManager.CurrentApplicantId.Value);
DataService.LogChange("IndividualAchievements", item.Id, "DELETE");
```

- При загрузке загружаются существующие данные из БД:
```csharp
var achievements = DatabasePersistenceHelper.LoadIndividualAchievements(
    SessionManager.CurrentApplicantId.Value
);
```

### 6. AttachedDocumentsPage (Страница приложенных документов)
**Статус**: ✅ Обновлена с использованием DatabasePersistenceHelper
- Сохраняет приложенные документы в таблицу `AttachedDocuments`
- При добавлении:
```csharp
int documentId = DatabasePersistenceHelper.SaveAttachedDocument(
    SessionManager.CurrentApplicantId.Value,
    documentType,
    seriesNumber,
    category,
    additionalData,
    issueDate,
    documentInfo,
    attachmentPath,
    attachmentName
);
DataService.LogChange("AttachedDocuments", documentId, "INSERT");
```

- При удалении:
```csharp
DatabasePersistenceHelper.DeleteAttachedDocument(item.Id, SessionManager.CurrentApplicantId.Value);
DataService.LogChange("AttachedDocuments", item.Id, "DELETE");
```

- При загрузке загружаются существующие данные из БД

## Особенности реализации

### 1. SessionManager
Используется для хранения текущего абитуриента в памяти во время сеанса:
```csharp
if (SessionManager.CurrentApplicant != null)
{
    // Абитуриент уже создан в БД
}
```

### 2. Логирование изменений
Все изменения логируются в таблицу `ChangeLogs`:
```csharp
DataService.LogChange("TableName", recordId, "INSERT|UPDATE|DELETE");
```

### 3. Параметризованные запросы
Все SQL запросы используют параметризованные запросы для защиты от SQL-инъекций:
```csharp
cmd.Parameters.AddWithValue("@ApplicantId", applicantId);
```

### 4. Обработка null значений
При работе с nullable типами используется `DBNull.Value`:
```csharp
cmd.Parameters.AddWithValue("@BirthDate", 
    birthDate.HasValue ? (object)birthDate.Value : DBNull.Value);
```

## Интеграция с UI

### Паттерн MVVM-подобный
- Page (представление)
- ObservableCollection для привязки данных
- INotifyPropertyChanged для уведомления об изменениях

### Обработка ошибок
```csharp
try
{
    // Сохранение данных
    int id = DatabasePersistenceHelper.SaveData(...);
    DataService.LogChange(...);
    MessageBox.Show("Успех!", "Успех", ...);
}
catch (Exception ex)
{
    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", ...);
}
```

## Таблицы в БД

### RelativeDocuments
```sql
CREATE TABLE RelativeDocuments (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ApplicantId INTEGER NOT NULL,
    RelationDegree TEXT,
    LastName TEXT,
    FirstName TEXT,
    Patronymic TEXT,
    BirthDate DATETIME,
    Phone TEXT,
    Email TEXT,
    WorkPlace TEXT,
    Position TEXT,
    BlockReason TEXT,
    IsBlocked INTEGER,
    CreatedAt DATETIME,
    UpdatedAt DATETIME,
    FOREIGN KEY(ApplicantId) REFERENCES Applicants(Id)
);
```

### IndividualAchievements
```sql
CREATE TABLE IndividualAchievements (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ApplicantId INTEGER NOT NULL,
    Category TEXT,
    AchievementName TEXT,
    Year TEXT,
    Points INTEGER,
    DocumentName TEXT,
    DocumentPath TEXT,
    CreatedAt DATETIME,
    UpdatedAt DATETIME,
    FOREIGN KEY(ApplicantId) REFERENCES Applicants(Id)
);
```

### AttachedDocuments
```sql
CREATE TABLE AttachedDocuments (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ApplicantId INTEGER NOT NULL,
    DocumentType TEXT,
    SeriesNumber TEXT,
    Category TEXT,
    AdditionalData TEXT,
    IssueDate DATETIME,
    DocumentInfo TEXT,
    AttachmentPath TEXT,
    AttachmentName TEXT,
    CreatedAt DATETIME,
    UpdatedAt DATETIME,
    FOREIGN KEY(ApplicantId) REFERENCES Applicants(Id)
);
```

## Примеры использования

### Сохранение нового элемента
```csharp
int id = DatabasePersistenceHelper.SaveRelativeDocument(
    applicantId: 1,
    relationDegree: "Родитель",
    lastName: "Иванов",
    firstName: "Иван",
    patronymic: "Иванович",
    birthDate: new DateTime(1960, 5, 15),
    phone: "+7 (999) 123-45-67",
    email: "ivan@example.com",
    workPlace: "ООО Компания",
    position: "Директор",
    blockReason: "",
    isBlocked: false
);
```

### Загрузка элементов
```csharp
var achievements = DatabasePersistenceHelper.LoadIndividualAchievements(applicantId: 1);
foreach (var achievement in achievements)
{
    Console.WriteLine($"{achievement.Category}: {achievement.Points} баллов");
}
```

### Удаление элемента
```csharp
DatabasePersistenceHelper.DeleteAttachedDocument(
    documentId: 5,
    applicantId: 1
);
```

## Преимущества этой архитектуры

1. ✅ **Централизованное хранилище** - все данные сохраняются в локальной БД
2. ✅ **Логирование** - все изменения фиксируются в ChangeLogs
3. ✅ **Переиспользуемость** - DatabasePersistenceHelper можно использовать на разных страницах
4. ✅ **Безопасность** - параметризованные запросы защищают от SQL-инъекций
5. ✅ **Обработка ошибок** - структурированная обработка исключений
6. ✅ **Юзабилити** - пользователь видит подтверждающие сообщения о сохранении

## Тестирование

Для проверки работы функциональности:

1. Создайте новую запись абитуриента (IdentityPage)
2. Заполните контактные данные (ContactsPage) - данные будут сохранены и залогированы
3. Добавьте родственника (RelativesPage) - данные в таблице RelativeDocuments
4. Добавьте достижение (IndividualAchievementsPage) - данные в таблице IndividualAchievements
5. Добавьте документ (AttachedDocumentsPage) - данные в таблице AttachedDocuments
6. Проверьте таблицу ChangeLogs - должны быть записи о всех операциях

Все данные сохраняются при переходе между страницами и при перезагрузке приложения.

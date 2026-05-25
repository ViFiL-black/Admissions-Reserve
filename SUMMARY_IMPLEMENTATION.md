# Резюме реализации логики сохранения данных в БД

## Выполненные работы

### 1. Создан новый класс DatabasePersistenceHelper

**Файл**: `Model/DatabasePersistenceHelper.cs`

Этот класс предоставляет универсальные методы для работы с БД для трёх основных таблиц:

#### Методы для RelativeDocuments (Родственники)
```csharp
// Загрузка родственников абитуриента из БД
LoadRelativeDocuments(int applicantId)

// Сохранение нового или обновление существующего родственника
SaveRelativeDocument(int applicantId, string relationDegree, string lastName, 
                     string firstName, string patronymic, DateTime? birthDate, 
                     string phone, string email, string workPlace, string position, 
                     string blockReason, bool isBlocked, int? id = null)

// Удаление родственника из БД
DeleteRelativeDocument(int id, int applicantId)
```

#### Методы для IndividualAchievements (Индивидуальные достижения)
```csharp
// Загрузка достижений абитуриента из БД
LoadIndividualAchievements(int applicantId)

// Сохранение нового или обновление существующего достижения
SaveIndividualAchievement(int applicantId, string category, string achievementName, 
                          string year, int points, string documentName, 
                          string documentPath, int? id = null)

// Удаление достижения из БД
DeleteIndividualAchievement(int id, int applicantId)
```

#### Методы для AttachedDocuments (Приложенные документы)
```csharp
// Загрузка приложенных документов из БД
LoadAttachedDocuments(int applicantId)

// Сохранение нового или обновление существующего документа
SaveAttachedDocument(int applicantId, string documentType, string seriesNumber, 
                     string category, string additionalData, DateTime? issueDate, 
                     string documentInfo, string attachmentPath, string attachmentName, 
                     int? id = null)

// Удаление приложенного документа из БД
DeleteAttachedDocument(int id, int applicantId)
```

#### Модели данных
```csharp
public class RelativeDocument
public class IndividualAchievementRecord
public class AttachedDocumentRecord
```

---

### 2. Обновлена ContactsPage

**Файл**: `View/ContactsPage.xaml.cs`

**Изменения**:
- Добавлено подтверждающее сообщение при успешном сохранении контактных данных
- Добавлено логирование всех изменений через `DataService.LogChange()`
- Улучшена обработка ошибок

**Новая логика сохранения**:
```csharp
private bool SaveData()
{
    // ... валидация ...

    // Обновление данных абитуриента
    DataService.UpdateApplicant(currentApplicant);
    DataService.LogChange("Applicants", currentApplicant.Id, "UPDATE");

    // Обновление SessionManager
    SessionManager.CurrentApplicant = currentApplicant;

    MessageBox.Show("Контактные данные успешно сохранены!", "Успех", ...);
    return true;
}
```

---

### 3. Обновлена RelativesPage

**Файл**: `View/RelativesPage.xaml.cs`

**Ключевые изменения**:

#### Загрузка данных при открытии страницы
```csharp
if (SessionManager.CurrentApplicant != null)
{
    var relatives = DatabasePersistenceHelper.LoadRelativeDocuments(
        SessionManager.CurrentApplicantId.Value
    );
}
```

#### Сохранение при добавлении родственника
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

#### Удаление при удалении родственника
```csharp
if (item.Id > 0 && SessionManager.CurrentApplicant != null)
{
    DatabasePersistenceHelper.DeleteRelativeDocument(
        item.Id, 
        SessionManager.CurrentApplicantId.Value
    );
    DataService.LogChange("RelativeDocuments", item.Id, "DELETE");
}
```

---

### 4. Обновлена IndividualAchievementsPage

**Файл**: `View/IndividualAchievementsPage.xaml.cs`

**Ключевые изменения**:

#### Загрузка достижений при открытии
```csharp
private void LoadAchievementsFromDatabase()
{
    var achievements = DatabasePersistenceHelper.LoadIndividualAchievements(
        SessionManager.CurrentApplicantId.Value
    );
}
```

#### Сохранение достижения
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

#### Удаление достижения
```csharp
DatabasePersistenceHelper.DeleteIndividualAchievement(
    item.Id, 
    SessionManager.CurrentApplicantId.Value
);
DataService.LogChange("IndividualAchievements", item.Id, "DELETE");
```

---

### 5. Обновлена AttachedDocumentsPage

**Файл**: `View/AttachedDocumentsPage.xaml.cs`

**Ключевые изменения**:

#### Загрузка документов при открытии
```csharp
private void LoadDocumentsFromDatabase()
{
    var documents = DatabasePersistenceHelper.LoadAttachedDocuments(
        SessionManager.CurrentApplicantId.Value
    );
}
```

#### Сохранение приложенного документа
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

#### Удаление приложенного документа
```csharp
DatabasePersistenceHelper.DeleteAttachedDocument(
    item.Id, 
    SessionManager.CurrentApplicantId.Value
);
DataService.LogChange("AttachedDocuments", item.Id, "DELETE");
```

---

## Архитектура решения

```
┌─────────────────────────────────────────┐
│  Page Layers                            │
│  (RelativesPage, IndividualAchievements │
│   AttachedDocumentsPage, ContactsPage)  │
└────────────────────┬────────────────────┘
                     │
         ┌───────────┴────────────┐
         │                        │
         ↓                        ↓
    ┌─────────────┐      ┌──────────────────────┐
    │ DataService │      │ DatabasePersistence  │
    │ (Main CRUD) │      │ Helper (Specialized) │
    └──────┬──────┘      └──────────┬───────────┘
           │                        │
           └───────────┬────────────┘
                       ↓
            ┌──────────────────────┐
            │  DatabaseHelper      │
            │  SQLite Connection   │
            └──────────────────────┘
                       │
                       ↓
            ┌──────────────────────┐
            │   SQLite Database    │
            │   (Local File)       │
            └──────────────────────┘
```

---

## Таблицы БД, используемые для сохранения

### 1. RelativeDocuments
```sql
CREATE TABLE RelativeDocuments (
    Id INTEGER PRIMARY KEY,
    ApplicantId INTEGER,
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
    UpdatedAt DATETIME
);
```

### 2. IndividualAchievements
```sql
CREATE TABLE IndividualAchievements (
    Id INTEGER PRIMARY KEY,
    ApplicantId INTEGER,
    Category TEXT,
    AchievementName TEXT,
    Year TEXT,
    Points INTEGER,
    DocumentName TEXT,
    DocumentPath TEXT,
    CreatedAt DATETIME,
    UpdatedAt DATETIME
);
```

### 3. AttachedDocuments
```sql
CREATE TABLE AttachedDocuments (
    Id INTEGER PRIMARY KEY,
    ApplicantId INTEGER,
    DocumentType TEXT,
    SeriesNumber TEXT,
    Category TEXT,
    AdditionalData TEXT,
    IssueDate DATETIME,
    DocumentInfo TEXT,
    AttachmentPath TEXT,
    AttachmentName TEXT,
    CreatedAt DATETIME,
    UpdatedAt DATETIME
);
```

---

## Основные особенности реализации

✅ **Параметризованные SQL запросы** - защита от SQL инъекций

✅ **Логирование всех операций** - через таблицу ChangeLogs

✅ **Перегружаемые методы** - возможность создания и обновления в одном методе

✅ **Обработка null значений** - правильная работа с nullable типами

✅ **Обработка исключений** - try/catch блоки со своими сообщениями об ошибках

✅ **Юзабилити** - подтверждающие сообщения об успешных операциях

✅ **Унифицированный интерфейс** - согласованный подход во всех классах

✅ **Сохранение состояния SessionManager** - актуальные данные во время сеанса

---

## Как использовать

### Для пользователей приложения
1. Создайте новую запись абитуриента (IdentityPage)
2. Заполните контактные данные (ContactsPage) - сохранится в БД
3. Добавьте родственников (RelativesPage) - сохранятся в RelativeDocuments
4. Добавьте достижения (IndividualAchievementsPage) - сохранятся в IndividualAchievements
5. Добавьте приложенные документы (AttachedDocumentsPage) - сохранятся в AttachedDocuments
6. При перезагрузке приложения - все данные загрузятся из БД

### Для разработчиков
1. Используйте `DatabasePersistenceHelper.Save*()` для сохранения данных
2. Используйте `DatabasePersistenceHelper.Load*()` для загрузки данных
3. Используйте `DatabasePersistenceHelper.Delete*()` для удаления данных
4. Не забывайте вызывать `DataService.LogChange()` после операции
5. Помните проверять `SessionManager.CurrentApplicant != null`

---

## Проверка работы

### Способ 1: Проверка через приложение
```
1. Запустить приложение
2. Создать абитуриента
3. Добавить данные на разных страницах
4. Перезагрузить приложение - данные должны остаться
```

### Способ 2: Проверка БД напрямую
```
1. Установить DB Browser for SQLite
2. Открыть файл БД приложения
3. Проверить таблицы:
   - RelativeDocuments
   - IndividualAchievements
   - AttachedDocuments
   - ChangeLogs (логирование)
```

---

## Результат

Все страницы приложения теперь используют единую логику сохранения данных в локальную БД SQLite с полным логированием всех операций. Это обеспечивает:

- ✅ Персистентность данных между сеансами работы
- ✅ Полный аудит всех изменений через ChangeLogs
- ✅ Надежное хранилище информации об абитуриентах
- ✅ Возможность восстановления и анализа данных
- ✅ Безопасность через параметризованные запросы
- ✅ Унифицированный подход ко всем таблицам

**Время компиляции**: ✅ Успешно
**Все ошибки**: ✅ Исправлены
**Готовно к использованию**: ✅ Да

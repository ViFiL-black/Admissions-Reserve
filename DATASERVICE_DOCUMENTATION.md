# Документация по работе с новыми методами DataService

## Обзор

В файл `DataService.cs` добавлены методы для работы со следующими таблицами БД:
- **Relatives** - родственники абитуриента
- **ApplicantLanguages** - языки абитуриента
- **SportAchievements** - спортивные достижения
- **IndividualAchievements** - индивидуальные достижения
- **ApplicationPriorities** - приоритеты образовательных программ
- **AttachedDocuments** - прикрепленные документы
- **CompetitionPriorities** - приоритеты конкурсов

## Структура таблиц

### Таблица Relatives
Хранит информацию о родственниках абитуриента.

```sql
CREATE TABLE Relatives (
    Id INTEGER PRIMARY KEY,
    ApplicantId INTEGER NOT NULL,
    Inn TEXT,
    RelationDegree TEXT,
    LastName TEXT,
    FirstName TEXT,
    Patronymic TEXT,
    BirthDate TEXT,
    Phone TEXT,
    Email TEXT,
    WorkPlace TEXT,
    Position TEXT,
    IsBlocked INTEGER,
    BlockReason TEXT,
    CreatedAt TEXT,
    UpdatedAt TEXT,
    FOREIGN KEY (ApplicantId) REFERENCES Applicants(Id)
)
```

### Таблица ApplicantLanguages
Хранит информацию о владении языками.

```sql
CREATE TABLE ApplicantLanguages (
    Id INTEGER PRIMARY KEY,
    ApplicantId INTEGER NOT NULL,
    LanguageId INTEGER,
    LanguageLevelId INTEGER,
    IsPrimary INTEGER,
    CreatedAt TEXT,
    UpdatedAt TEXT,
    FOREIGN KEY (ApplicantId) REFERENCES Applicants(Id),
    FOREIGN KEY (LanguageId) REFERENCES Languages(Id),
    FOREIGN KEY (LanguageLevelId) REFERENCES LanguageLevels(Id)
)
```

### Таблица SportAchievements
Хранит информацию о спортивных достижениях.

```sql
CREATE TABLE SportAchievements (
    Id INTEGER PRIMARY KEY,
    ApplicantId INTEGER NOT NULL,
    SportType TEXT,
    Achievement TEXT,
    Rank TEXT,
    Year INTEGER,
    CreatedAt TEXT,
    UpdatedAt TEXT,
    FOREIGN KEY (ApplicantId) REFERENCES Applicants(Id)
)
```

### Таблица IndividualAchievements
Хранит информацию об индивидуальных достижениях.

```sql
CREATE TABLE IndividualAchievements (
    Id INTEGER PRIMARY KEY,
    ApplicantId INTEGER NOT NULL,
    Achievement TEXT,
    CreatedAt TEXT,
    UpdatedAt TEXT,
    FOREIGN KEY (ApplicantId) REFERENCES Applicants(Id)
)
```

### Таблица ApplicationPriorities
Хранит приоритеты образовательных программ.

```sql
CREATE TABLE ApplicationPriorities (
    Id INTEGER PRIMARY KEY,
    ApplicantId INTEGER NOT NULL,
    PriorityOrder INTEGER,
    ProgramCode TEXT,
    ProgramName TEXT,
    StudyForm TEXT,
    EducationBase TEXT,
    Department TEXT,
    AdmissionType TEXT,
    Branch TEXT,
    IsSelected INTEGER,
    CreatedAt TEXT,
    UpdatedAt TEXT,
    FOREIGN KEY (ApplicantId) REFERENCES Applicants(Id)
)
```

### Таблица AttachedDocuments
Хранит информацию о прикрепленных файлах.

```sql
CREATE TABLE AttachedDocuments (
    Id INTEGER PRIMARY KEY,
    ApplicantId INTEGER NOT NULL,
    DocumentName TEXT,
    DocumentType TEXT,
    FilePath TEXT,
    FileSize INTEGER,
    UploadedAt TEXT,
    CreatedAt TEXT,
    UpdatedAt TEXT,
    FOREIGN KEY (ApplicantId) REFERENCES Applicants(Id)
)
```

### Таблица CompetitionPriorities
Хранит приоритеты конкурсов.

```sql
CREATE TABLE CompetitionPriorities (
    Id INTEGER PRIMARY KEY,
    ApplicantId INTEGER NOT NULL,
    CompetitionName TEXT,
    PriorityOrder INTEGER,
    IsSelected INTEGER,
    CreatedAt TEXT,
    UpdatedAt TEXT,
    FOREIGN KEY (ApplicantId) REFERENCES Applicants(Id)
)
```

## Методы DataService

### Методы для Relatives

```csharp
// Создание нового родственника
int relativeId = DataService.CreateRelative(
    applicantId: 1,
    inn: "1234567890",
    relationDegree: "Отец",
    lastName: "Петров",
    firstName: "Иван",
    patronymic: "Иванович",
    birthDate: new DateTime(1960, 01, 15),
    phone: "89991234567",
    email: "ivan@example.com",
    workplace: "ООО Компания",
    position: "Директор"
);

// Получение всех родственников
List<Relatives> relatives = DataService.GetApplicantRelatives(applicantId: 1);

// Удаление родственника
DataService.DeleteRelative(relativeId: 1);
```

### Методы для ApplicantLanguages

```csharp
// Добавление языка
int languageId = DataService.CreateApplicantLanguage(
    applicantId: 1,
    languageId: 2,  // Например, Английский
    languageLevelId: 3,  // Например, Средний
    isPrimary: false
);

// Получение всех языков абитуриента
List<ApplicantLanguages> languages = DataService.GetApplicantLanguages(applicantId: 1);

// Удаление языка
DataService.DeleteApplicantLanguage(languageId: 1);
```

### Методы для SportAchievements

```csharp
// Добавление спортивного достижения
int achievementId = DataService.CreateSportAchievement(
    applicantId: 1,
    sportType: "Футбол",
    achievement: "Чемпион региона",
    rank: "1-е место",
    year: 2022
);

// Получение всех достижений
List<SportAchievements> achievements = DataService.GetApplicantSportAchievements(applicantId: 1);

// Удаление достижения
DataService.DeleteSportAchievement(achievementId: 1);
```

### Методы для IndividualAchievements

```csharp
// Добавление индивидуального достижения
int achievementId = DataService.CreateIndividualAchievement(
    applicantId: 1,
    achievement: "Опубликована научная статья"
);

// Получение всех индивидуальных достижений
List<IndividualAchievements> achievements = DataService.GetApplicantIndividualAchievements(applicantId: 1);

// Удаление достижения
DataService.DeleteIndividualAchievement(achievementId: 1);
```

### Методы для ApplicationPriorities

```csharp
// Добавление приоритета
int priorityId = DataService.CreateApplicationPriority(
    applicantId: 1,
    priorityOrder: 1,
    programCode: "09.03.01",
    programName: "Информатика и вычислительная техника",
    studyForm: "Очная",
    educationBase: "Среднее (полное) общее образование",
    department: "Факультет ИВТ",
    admissionType: "Бюджет",
    branch: "Основной"
);

// Получение всех приоритетов
List<ApplicationPriorities> priorities = DataService.GetApplicantPriorities(applicantId: 1);

// Удаление приоритета
DataService.DeleteApplicationPriority(priorityId: 1);
```

### Методы для AttachedDocuments

```csharp
// Добавление прикрепленного документа
int docId = DataService.CreateAttachedDocument(
    applicantId: 1,
    documentName: "Диплом о высшем образовании",
    documentType: "PDF",
    filePath: "C:\\Documents\\diploma.pdf",
    fileSize: 1024000
);

// Получение всех документов
List<AttachedDocuments> documents = DataService.GetApplicantAttachedDocuments(applicantId: 1);

// Удаление документа
DataService.DeleteAttachedDocument(documentId: 1);
```

### Методы для CompetitionPriorities

```csharp
// Добавление конкурса
int compId = DataService.CreateCompetitionPriority(
    applicantId: 1,
    competitionName: "Общий конкурс",
    priorityOrder: 1
);

// Получение всех конкурсов
List<CompetitionPriorities> competitions = DataService.GetApplicantCompetitions(applicantId: 1);

// Удаление конкурса
DataService.DeleteCompetitionPriority(competitionId: 1);
```

## Интеграция с страницами

### Пример для RelativesPage

```csharp
// В конструкторе страницы
if (SessionManager.CurrentApplicant != null)
{
    var relatives = DataService.GetApplicantRelatives(SessionManager.CurrentApplicantId.Value);
    foreach (var relative in relatives)
    {
        // Добавить в ObservableCollection
    }
}

// При добавлении родственника
int id = DataService.CreateRelative(
    SessionManager.CurrentApplicantId.Value,
    inn, relationDegree, lastName, firstName, patronymic,
    birthDate, phone, email, workplace, position
);
DataService.LogChange("Relatives", id, "CREATE");
```

### Пример для AdditionalInfoPage

```csharp
// Загрузка языков
var languages = DataService.GetApplicantLanguages(SessionManager.CurrentApplicantId.Value);
var sportAchievements = DataService.GetApplicantSportAchievements(SessionManager.CurrentApplicantId.Value);
var individualAchievements = DataService.GetApplicantIndividualAchievements(SessionManager.CurrentApplicantId.Value);

// Сохранение языка
int langId = DataService.CreateApplicantLanguage(
    SessionManager.CurrentApplicantId.Value,
    selectedLanguage.Id,
    selectedLevel.Id,
    isPrimary
);
DataService.LogChange("ApplicantLanguages", langId, "CREATE");
```

## Справочные таблицы

### Languages
Содержит доступные языки:
- Русский
- Английский
- Немецкий
- Французский
- Испанский
- Китайский
- Японский
- Корейский

### LanguageLevels
Содержит уровни владения языком:
1. Элементарный (1)
2. Базовый (2)
3. Средний (3)
4. Выше среднего (4)
5. Продвинутый (5)
6. Беглый (6)

## Логирование

Все операции CRUD следует логировать:

```csharp
DataService.LogChange("TableName", recordId, "CREATE|UPDATE|DELETE");
```

Это создает запись в таблице `ChangeHistory` для аудита.

## Обработка ошибок

Все методы используют try-catch для безопасной работы с БД. Рекомендуется обрабатывать исключения на уровне страницы:

```csharp
try
{
    var result = DataService.CreateRelative(...);
}
catch (SQLiteException ex)
{
    MessageBox.Show($"Ошибка БД: {ex.Message}", "Ошибка");
}
catch (Exception ex)
{
    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
}
```

## Заключение

Все новые методы полностью интегрированы с DatabaseHelper и используют параметризованные запросы для безопасности. Для использования методов требуется наличие активного абитуриента в SessionManager.

# Архитектура БД и приложения

## 📊 Диаграмма связей таблиц

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              ADMISSIONS RESERVE DB                           │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────┐
│    Applicants       │ (Основная таблица абитуриентов)
├─────────────────────┤
│ Id (PK)             │
│ LastName            │
│ FirstName           │
│ Patronymic          │
│ BirthDate           │
│ ... (другие поля)   │
│ CreatedAt           │
│ UpdatedAt           │
└──────────┬──────────┘
           │ 1:N
           │
    ┌──────┴─────────────────────┬────────────────────────┬──────────────────┐
    │                            │                        │                  │
    ▼                            ▼                        ▼                  ▼
┌───────────────────┐  ┌──────────────────┐  ┌────────────────────┐  ┌──────────────┐
│ IdentityDocuments │  │   Relatives      │  │ ApplicantLanguages │  │     ...      │
├───────────────────┤  ├──────────────────┤  ├────────────────────┤  └──────────────┘
│ Id (PK)           │  │ Id (PK)          │  │ Id (PK)            │
│ ApplicantId (FK)  │  │ ApplicantId (FK) │  │ ApplicantId (FK)   │
│ DocumentTypeId(FK)│  │ Inn              │  │ LanguageId (FK)    │
│ Series            │  │ RelationDegree   │  │ LanguageLevelId(FK)│
│ Number            │  │ LastName         │  │ IsPrimary          │
│ IssuedBy          │  │ FirstName        │  │ CreatedAt          │
│ IssueDate         │  │ Patronymic       │  │ UpdatedAt          │
│ DepartmentCode    │  │ BirthDate        │  └────────────────────┘
│ IsPrimary         │  │ Phone            │         │ N:1 ┐
│ AddedDate         │  │ Email            │         │     │
└───────────────────┘  │ WorkPlace        │    ┌────┴─────▼─────────┐
          │            │ Position         │    │                    │
          │            │ IsBlocked        │    ▼                    ▼
          │            │ BlockReason      │  ┌──────────┐      ┌──────────────┐
          │            │ CreatedAt        │  │Languages │      │LanguageLevels│
          └─── N:1     │ UpdatedAt        │  ├──────────┤      ├──────────────┤
                │      └──────────────────┘  │ Id (PK)  │      │ Id (PK)      │
                │                             │ Name     │      │ Name         │
                ▼                             │ (Русский,│      │ SortOrder    │
        ┌──────────────────┐               │ Английский,       │              │
        │IdentityDocTypes  │               │ Немецкий,│      │ (Элементарный,
        ├──────────────────┤               │ ...)     │      │  Базовый,    │
        │ Id (PK)          │               └──────────┘      │  Средний, ...)
        │ Name             │                                  └──────────────┘
        │ (Паспорт,        │
        │  Загранпас..)    │
        └──────────────────┘

    ┌──────────────────────────────────────────────────────────────────────┐
    │                    ДРУГИЕ ТАБЛИЦЫ (связаны с Applicants)             │
    └──────────────────────────────────────────────────────────────────────┘

    ┌────────────────────┐  ┌─────────────────────┐  ┌──────────────────────┐
    │ SportAchievements  │  │IndividualAchievements│ │ApplicationPriorities│
    ├────────────────────┤  ├─────────────────────┤  ├──────────────────────┤
    │ Id (PK)            │  │ Id (PK)             │  │ Id (PK)              │
    │ ApplicantId (FK)   │  │ ApplicantId (FK)    │  │ ApplicantId (FK)     │
    │ SportType          │  │ Achievement         │  │ PriorityOrder        │
    │ Achievement        │  │ CreatedAt           │  │ ProgramCode          │
    │ Rank               │  │ UpdatedAt           │  │ ProgramName          │
    │ Year               │  └─────────────────────┘  │ StudyForm            │
    │ CreatedAt          │                           │ EducationBase        │
    │ UpdatedAt          │                           │ Department           │
    └────────────────────┘                           │ AdmissionType        │
                                                     │ Branch               │
    ┌────────────────────┐  ┌─────────────────────┐  │ IsSelected           │
    │AttachedDocuments   │  │CompetitionPriorities│  │ CreatedAt            │
    ├────────────────────┤  ├─────────────────────┤  │ UpdatedAt            │
    │ Id (PK)            │  │ Id (PK)             │  └──────────────────────┘
    │ ApplicantId (FK)   │  │ ApplicantId (FK)    │
    │ DocumentName       │  │ CompetitionName     │
    │ DocumentType       │  │ PriorityOrder       │
    │ FilePath           │  │ IsSelected          │
    │ FileSize           │  │ CreatedAt           │
    │ UploadedAt         │  │ UpdatedAt           │
    │ CreatedAt          │  └─────────────────────┘
    │ UpdatedAt          │
    └────────────────────┘

    ┌────────────────────────┐  ┌────────────────────┐
    │EducationDocuments      │  │ChangeHistory      │
    ├────────────────────────┤  ├────────────────────┤
    │ Id (PK)                │  │ Id (PK)            │
    │ ApplicantId (FK)       │  │ TableName          │
    │ ApplicationTypeId (FK) │  │ RecordId           │
    │ FirstTimeEducation     │  │ Action (CREATE,    │
    │ CountryId (FK)         │  │        UPDATE,     │
    │ City                   │  │        DELETE)     │
    │ EducationalOrg         │  │ ChangedAt          │
    │ DocumentTypeId (FK)    │  └────────────────────┘
    │ EducationLevelId (FK)  │
    │ Series, Number         │
    │ IssueDate, Year        │
    │ Grades (Satisfactory,  │
    │  Good, Excellent)      │
    │ AverageScore           │
    │ ... (остальные поля)   │
    └────────────────────────┘
```

## 🏗️ Архитектура приложения

```
┌─────────────────────────────────────────────────────────────────┐
│                      ADMISSIONS RESERVE APP                       │
└─────────────────────────────────────────────────────────────────┘

┌──────────────────────┐
│   USER INTERFACE     │
│   (XAML Pages)       │
└──────────────────────┘
         │
    ┌────┴─────────────────────────────────────────┐
    │                                              │
    ▼                                              ▼
┌──────────────────┐                    ┌──────────────────────┐
│ WelcomePage      │                    │ ApplicantWizardPage  │
├──────────────────┤                    ├──────────────────────┤
│ - LoginWindow    │                    │ - IdentityPage       │
│ - Navigation     │                    │ - ContactsPage       │
└──────────────────┘                    │ - DocumentsPage      │
                                        │ - RelativesPage ✅   │
                                        │ - AdditionalInfo     │
                                        │ - Priorities         │
                                        │ - Competitions       │
                                        │ - AttachedDocuments  │
                                        │ - IndividualAchiev.  │
                                        └──────────────────────┘

         ▼ (использует)

┌──────────────────────────────────────┐
│         SESSION MANAGER              │
├──────────────────────────────────────┤
│ - CurrentApplicant                   │
│ - CurrentApplicantId                 │
│ - LoadApplicant(id)                  │
│ - Clear()                            │
└──────────────────────────────────────┘

         ▼ (использует)

┌──────────────────────────────────────┐
│           DATA SERVICE               │
├──────────────────────────────────────┤
│ Методы для Applicants:               │
│ - CreateApplicant()                  │
│ - UpdateApplicant()                  │
│ - GetApplicant()                     │
│ - DeleteApplicant()                  │
│                                      │
│ Методы для Relatives:                │
│ - CreateRelative() ✅                │
│ - GetApplicantRelatives() ✅         │
│ - DeleteRelative() ✅                │
│                                      │
│ Методы для Languages:                │
│ - CreateApplicantLanguage() ✅       │
│ - GetApplicantLanguages() ✅         │
│ - DeleteApplicantLanguage() ✅       │
│                                      │
│ Методы для SportAchievements:        │
│ - CreateSportAchievement() ✅        │
│ - GetApplicantSportAchievements() ✅ │
│ - DeleteSportAchievement() ✅        │
│                                      │
│ Методы для IndividualAchievements:   │
│ - CreateIndividualAchievement() ✅   │
│ - GetApplicantIndividualAchiev() ✅  │
│ - DeleteIndividualAchievement() ✅   │
│                                      │
│ Методы для ApplicationPriorities:    │
│ - CreateApplicationPriority() ✅     │
│ - GetApplicantPriorities() ✅        │
│ - DeleteApplicationPriority() ✅     │
│                                      │
│ Методы для AttachedDocuments:        │
│ - CreateAttachedDocument() ✅        │
│ - GetApplicantAttachedDocuments() ✅ │
│ - DeleteAttachedDocument() ✅        │
│                                      │
│ Методы для CompetitionPriorities:    │
│ - CreateCompetitionPriority() ✅     │
│ - GetApplicantCompetitions() ✅      │
│ - DeleteCompetitionPriority() ✅     │
│                                      │
│ Методы для справочников:             │
│ - GetAll<T>()                        │
│ - GetByCondition<T>()                │
│                                      │
│ Логирование:                         │
│ - LogChange()                        │
└──────────────────────────────────────┘

         ▼ (использует)

┌──────────────────────────────────────┐
│      DATABASE HELPER                 │
├──────────────────────────────────────┤
│ - GetConnection()                    │
│ - GetDatabasePath()                  │
│ - InitializeDatabase()               │
│ - CreateAllTablesIfNotExist()        │
│ - SeedDataIfEmpty()                  │
└──────────────────────────────────────┘

         ▼ (использует)

┌──────────────────────────────────────┐
│      SQLite DATABASE                 │
├──────────────────────────────────────┤
│ File: AdmissionsReserve.db           │
│ Location: App_Data/                  │
│                                      │
│ Таблицы (20+):                       │
│ - Applicants                         │
│ - Relatives ✅                       │
│ - ApplicantLanguages ✅              │
│ - Languages ✅                       │
│ - LanguageLevels ✅                  │
│ - SportAchievements ✅               │
│ - IndividualAchievements ✅          │
│ - ApplicationPriorities ✅           │
│ - AttachedDocuments ✅               │
│ - CompetitionPriorities ✅           │
│ - ChangeHistory                      │
│ - И другие справочные таблицы        │
└──────────────────────────────────────┘
```

## 🔄 Flow диаграмма для добавления данных

```
┌─────────────────────────────┐
│ Пользователь открывает      │
│ страницу (например,         │
│ RelativesPage)              │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│ Конструктор страницы        │
│ InitializeData()            │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│ LoadRelativesFromDatabase() │
│ GetApplicantRelatives(id)   │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│ Заполнить UI элементы       │
│ ObservableCollection        │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│ Пользователь заполняет      │
│ форму и нажимает "Добавить" │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│ Валидация данных            │
└──────────────┬──────────────┘
               │
          ┌────┴─────┐
          │ (валидно) │
          ▼           │
┌─────────────────────────────┐
│ Вызов CreateRelative()      │
│ с параметрами              │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│ INSERT в Relatives таблицу  │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│ Получить ID новой записи    │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│ LogChange("Relatives",      │
│            id, "CREATE")    │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│ Добавить в ObservableColl.  │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│ Показать сообщение "Успех"  │
└─────────────────────────────┘
```

## 📈 Статус реализации

```
┌──────────────────────────────────────────┐
│     СТАТУС РЕАЛИЗАЦИИ ПО КОМПОНЕНТАМ     │
└──────────────────────────────────────────┘

DatabaseHelper.cs
┌─────────────────────────────────────────┐
│ ████████████████████████ 100%           │
│ ✅ ГОТОВО                               │
└─────────────────────────────────────────┘

DataService.cs
┌─────────────────────────────────────────┐
│ ████████████████████████ 100%           │
│ ✅ ГОТОВО                               │
└─────────────────────────────────────────┘

Models.cs
┌─────────────────────────────────────────┐
│ ████████████████████████ 100%           │
│ ✅ ГОТОВО                               │
└─────────────────────────────────────────┘

Model1.Context.cs
┌─────────────────────────────────────────┐
│ ████████████████████████ 100%           │
│ ✅ ГОТОВО                               │
└─────────────────────────────────────────┘

RelativesPage
┌─────────────────────────────────────────┐
│ ██████████░░░░░░░░░░░░░░░░░░ 50%        │
│ ⏳ ЧАСТИЧНО (загрузка + добавление)     │
└─────────────────────────────────────────┘

AdditionalInfoPage
┌─────────────────────────────────────────┐
│ ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░ 0%       │
│ ⏳ ТРЕБУЕТСЯ ОБНОВЛЕНИЕ                 │
└─────────────────────────────────────────┘

Остальные страницы
┌─────────────────────────────────────────┐
│ ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░ 0%       │
│ ⏳ ТРЕБУЕТСЯ ОБНОВЛЕНИЕ                 │
└─────────────────────────────────────────┘

ИТОГО:
┌─────────────────────────────────────────┐
│ █████████████░░░░░░░░░░░░░░░░░░ 43%     │
│ ✅ БАЗА ГОТОВА К ИСПОЛЬЗОВАНИЮ         │
└─────────────────────────────────────────┘
```

---

**Последняя обновка:** 2024  
**Версия БД:** 1.0  
**Версия API:** 1.0
